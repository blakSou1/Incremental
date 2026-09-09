using UnityEngine;

public class SlotModPiece : MonoBehaviour
{
    [HideInInspector] public InteractiveObject piece;
    public GameObject activ;

    private  void Start()
    {
        activ.SetActive(false);
    }

    public void Click()
    {
        if (piece == null) return;

        G.mainEnterPoint.modifierPieces.Restart();
        activ.SetActive(true);
        G.mainEnterPoint.gameLogic.pieceObj = piece;

        if(G.mainEnterPoint.gameLogic.actualPiece != null)
        {
            G.mainEnterPoint.gameLogic.ActualPiece();
            G.mainEnterPoint.gridController.CreateIndisObject(G.PlayerController.playerColor, G.mainEnterPoint.gameLogic.pieceObj);
        }

    }

    public void Restart()
    {
        activ.SetActive(false);
    }
}
