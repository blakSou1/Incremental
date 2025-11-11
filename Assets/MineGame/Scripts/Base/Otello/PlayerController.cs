using UnityEngine;
using UnityEngine.InputSystem;
using Index = System.Tuple<int, int>;

public class PlayerController : MonoBehaviour
{
    public GridBox.Status playerColor = GridBox.Status.Black;

    private GameObject cursor;
    private Index curIndex;

    private bool needDisableCursor = false;

    void Start()
    {
        cursor = Instantiate(G.gameMode.curObj, Vector3.zero, Quaternion.identity);
    }
    void Update()
    {
        UpdatePos();

        if (G.inputs.Player.Attack.WasPressedThisFrame())
            G.gameMode.PlacePiece(playerColor, curIndex);
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

    float SnapGrid(float value, float snapSize = 0.5f)
    {
        if (value < 0)
            return Mathf.Round(Mathf.Abs(value / snapSize)) * snapSize * -1;
        else
            return Mathf.Round(value / snapSize) * snapSize;
    }

    void UpdatePos()
    {
        Vector2 curPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (curPos.x <= -1.75 || curPos.y <= -1.75 || curPos.x >= 2.25 || curPos.y >= 2.25)
        {
            DisableCursor();
            return;
        }
        
        EnableCursor();

        Vector2 snapPos = new(SnapGrid(curPos.x), SnapGrid(curPos.y));
        curIndex = GameMode.Vector2ToIndex(snapPos);
        
        cursor.transform.position = snapPos;
    }
}
