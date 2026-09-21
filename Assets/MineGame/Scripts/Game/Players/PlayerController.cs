using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject cursorPrefab;
    private GameObject cursor;
    private GridBox currentBox;

    public float speed = .5f;

    private bool needDisableCursor = false;

    void Start()
    {
        cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity);
        cursor.SetActive(false);

        G.eventManager.UpdatePlayerInput.AddListener(PlayerInputUpdate);
    }

    void Update()
    {
        UpdatePos();

        if (G.inputs.Player.Attack.WasPressedThisFrame())
            StartCoroutine(G.mainEnterPoint.gameLogic.PlacePiece(currentBox));
    }

    private void UpdatePos()
    {
        if (G.roomMovement.parentGrid == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!TryGetPointOnBoard(ray, out Vector3 worldPoint)) return;

        currentBox = GetNearGridBox(worldPoint);

        if (currentBox == null || currentBox.GetStat() != Status.None)
            DisableCursor();
        else
        {
            EnableCursor();
            cursor.transform.position = currentBox.transform.position;
            cursor.transform.rotation = currentBox.transform.rotation;
        }
    }

    private bool TryGetPointOnBoard(Ray ray, out Vector3 worldPoint)
    {
        worldPoint = default;

        Transform board = G.roomMovement.parentGrid;
        if (board == null) return false;

        Plane plane = new(board.up, board.position);

        if (plane.Raycast(ray, out float distance))
        {
            worldPoint = ray.GetPoint(distance);
            return true;
        }

        return false;
    }

    private GridBox GetNearGridBox(Vector3 worldPoint)
    {
        if (G.mainEnterPoint?.gridController?.matrix == null)
            return null;

        Transform board = G.roomMovement.parentGrid;
        Vector3 point = board.InverseTransformPoint(worldPoint);

        GridBox best = null;
        float bestSqr = float.MaxValue;

        var data = G.mainEnterPoint.gridController.matrix.GetData();

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                GridBox box = data[i, j];
                if (box == null) continue;

                Vector3 cell = board.InverseTransformPoint(box.transform.position);
                float sqr = (cell - point).sqrMagnitude;

                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = box;
                }
            }
        }

        float maxDist = G.boardVisualConfig.cellSpacing.magnitude * 0.5f;

        return bestSqr <= maxDist * maxDist ? best : null;
    }

    private void EnableCursor()
    {
        cursor.SetActive(true);
    }

    private void DisableCursor()
    {
        if (cursor || needDisableCursor)
        {
            cursor.SetActive(false);
            needDisableCursor = false;
        }
        else
            needDisableCursor = true;
    }

    private void PlayerInputUpdate(bool isEnablePlayerInput = true)
    {
        if (isEnablePlayerInput)
            G.inputs.Player.Enable();
        else
            G.inputs.Player.Disable();
    }


    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private bool hasSaved = false;

    private Coroutine currentMove;

    public IEnumerator MoveTo(Transform target, float duration, bool savePrevious = true, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogWarning("CameraMover.MoveTo: target is null");
            yield break;
        }

        G.inputs.Look.Disable();

        if (savePrevious && !hasSaved)
        {
            savedPosition = Camera.main.transform.position;
            savedRotation = Camera.main.transform.rotation;
            hasSaved = true;
        }

        if (currentMove != null)
            StopCoroutine(currentMove);

        yield return currentMove = StartCoroutine(MoveRoutine(target.position, target.rotation, duration, onComplete));
    }

    /// <summary>
    /// Возвращает камеру к сохранённой позиции и повороту за заданное время.
    /// </summary>
    public void ReturnToPrevious(float duration, Action onComplete = null)
    {
        if (!hasSaved)
        {
            Debug.LogWarning("CameraMover.ReturnToPrevious: нет сохранённого положения");
            return;
        }

        G.inputs.Look.Enable();

        if (currentMove != null)
            StopCoroutine(currentMove);

        currentMove = StartCoroutine(MoveRoutine(savedPosition, savedRotation, duration, () =>
        {
            hasSaved = false;
            onComplete?.Invoke();
        }));
    }

    /// <summary>
    /// Забыть сохранённое положение (если больше не нужно возвращаться).
    /// </summary>
    public void ClearSaved()
    {
        hasSaved = false;
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, Quaternion targetRot, float duration, Action onComplete)
    {
        if (duration <= 0f)
        {
            Camera.main.transform.position = targetPos;
            Camera.main.transform.rotation = targetRot;
            onComplete?.Invoke();
            yield break;
        }

        Vector3 startPos = Camera.main.transform.position;
        Quaternion startRot = Camera.main.transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float smooth = t * t * (3f - 2f * t);

            Camera.main.transform.position = Vector3.Lerp(startPos, targetPos, smooth);
            Camera.main.transform.rotation = Quaternion.Slerp(startRot, targetRot, smooth);

            yield return null;
        }

        Camera.main.transform.position = targetPos;
        Camera.main.transform.rotation = targetRot;

        currentMove = null;
        onComplete?.Invoke();
    }
}