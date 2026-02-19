using UnityEngine;

public class Chooice : MonoBehaviour
{
    public PieceZone picker;
    public PieceZone hand;

    private void Awake()
    {
        G.chooice = this;
    }

    private InteractiveObject CreatePiece(string t)
    {
        var basicPiece = CMS.Get<CMSEntity>(t);
        var state = new PieceState
        {
            model = basicPiece
        };

        var instance = Instantiate(basicPiece.Get<TagPrefab>().prefab);

        instance.moveable.targetPosition = new(10, 10, instance.transform.position.z);
        instance.transform.position = new(10, 10, instance.transform.position.z);

        instance.SetState(state);
        return instance;
    }

    public InteractiveObject AddPiece(string id)
    {
        return CreatePiece(id);
    }
}