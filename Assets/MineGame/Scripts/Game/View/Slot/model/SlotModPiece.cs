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

        G.mainEnterPoint.gridController.CreateIndisObject(G.mainEnterPoint.gameLogic.ActualColor, piece);
    }

    public void Restart()
    {
        activ.SetActive(false);
    }
}
