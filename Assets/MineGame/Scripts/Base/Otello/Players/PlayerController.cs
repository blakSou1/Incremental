using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GridBox.Status playerColor = GridBox.Status.Black;

    public GameObject cursorPrefab;
    private GameObject cursor;
    private GridBox curentBox;

    private bool needDisableCursor = false;

    [NonSerialized] public bool isStopped = true;

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
            G.gameLogic.PlacePiece(playerColor, curentBox);
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

    void UpdatePos()
    {
        Vector2 curPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(curPos, 0.06f);

        curentBox = null;

        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent(out GridBox gridBoxs))
            {
                curentBox = gridBoxs;
                break;
            }
        }
        
        if(curentBox == null)
            DisableCursor();
        else
        {
            EnableCursor();
            cursor.transform.position = curentBox.transform.position;
        }
    }
}
