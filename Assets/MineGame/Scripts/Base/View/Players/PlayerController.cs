using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static GridBox;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Status playerColor = Status.Black;

    public GameObject cursorPrefab;
    private GameObject cursor;
    private GridBox curentBox;

    public Transform position1;
    public Transform position2;

    public float speed = .5f;

    private bool needDisableCursor = false;

    [NonSerialized] public bool isStopped = true;

    private Transform currentPosition;

    void Start()
    {
        cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity);
        cursor.SetActive(false);
    }
    void Update()
    {
        if (isStopped) return;

        UpdatePos();

        if (G.inputs.Player.Attack.WasPressedThisFrame())
            G.mainEnterPoint.StartCoroutine(G.gameLogic.PlacePiece(playerColor, curentBox));
    }

    private void UpdatePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new(Vector3.forward, new Vector3(0, 0, G.gridController.parentIndc.transform.position.z));

        plane.Raycast(ray, out float distance);
        Vector3 worldPoint = ray.GetPoint(distance);
        Vector2 origin = new(worldPoint.x, worldPoint.y);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(origin, 0.06f);

        curentBox = null;

        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent(out GridBox gridBoxs))
            {
                curentBox = gridBoxs;
                break;
            }
        }

        if (curentBox == null || curentBox.GetStat() != Status.None)
            DisableCursor();
        else
        {
            EnableCursor();
            cursor.transform.position = curentBox.transform.position;
        }
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

    public IEnumerator MoveAndRotate(Transform start, Transform end)
    {
        if (currentPosition != end)
        {
            currentPosition = end;

            float startTime = Time.time;

            while (Time.time - startTime < speed)
            {
                float fractionOfJourney = Mathf.Clamp01((Time.time - startTime) / speed);

                transform.position = Vector3.Lerp(start.position, end.position, fractionOfJourney);
                transform.rotation = Quaternion.Slerp(start.rotation, end.rotation, fractionOfJourney);

                yield return null;
            }

            transform.position = end.position;
            transform.rotation = end.rotation;
        }
    }
}
