using UnityEngine;

public class Chooice : MonoBehaviour
{
    public PieceZone picker;
    public PieceZone hand;
    public Interactor interactor;

    private void Awake()
    {
        G.chooice = this;
    }

    public InteractiveObject CreatePiece(string t)
    {
        var basicDice = CMS.Get<CMSEntity>(t);
        var state = new PieceState
        {
            model = basicDice
        };

        var instance = Instantiate(basicDice.Get<TagPrefab>().prefab);
        instance.SetState(state);
        return instance;
    }

    public InteractiveObject AddPiece(string id)
    {
        var instance = CreatePiece(id);

        return instance;
    }
}