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
        var basicPiece = CMS.Get<CMSEntity>(t);
        var state = new PieceState
        {
            model = basicPiece
        };

        var instance = Instantiate(basicPiece.Get<TagPrefab>().prefab);

        instance.moveable.targetPosition = new(50, 50, instance.transform.position.z);
        instance.transform.position = new(50, 50, instance.transform.position.z);

        instance.SetState(state);
        return instance;
    }

    public InteractiveObject AddPiece(string id)
    {
        return CreatePiece(id);
    }
}