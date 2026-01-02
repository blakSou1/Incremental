using UnityEngine;

public class SlotModPiece : MonoBehaviour
{
    [HideInInspector] public Piece piece;
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
        G.gameLogic.piece = piece;

        G.gridFuncion.CreateIndisObject(G.PlayerController.playerColor);

    }

    public void Restart()
    {
        activ.SetActive(false);
    }
}
