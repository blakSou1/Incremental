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

        G.modifirePieces.Restart();
        activ.SetActive(true);
        G.gameLogic.pieceObj = piece.state.model.id;

        if(G.gameLogic.piece != null)
        {
            G.gameLogic.ActualPiece();
            G.gridFuncion.CreateIndisObject(G.PlayerController.playerColor);
        }

    }

    public void Restart()
    {
        activ.SetActive(false);
    }
}
