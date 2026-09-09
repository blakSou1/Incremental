using UnityEngine;

public class PieceFactory : MonoBehaviour
{
    private void Awake()
    {
        G.choice = this;
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
        if (string.IsNullOrEmpty(id))
            id = ConfigGame.standardPiece;

        return CreatePiece(id);
    }


    private GridBoxMode CreateModifire(string t)
    {
        var basicMode = CMS.Get<CMSEntity>(t);
        var state = new modeState
        {
            model = basicMode
        };

        var instance = Instantiate(basicMode.Get<TagPrefabGridBoxMode>().prefab);

        instance.transform.position = new(10, 10, instance.transform.position.z);

        instance.SetState(state);
        return instance;
    }

    public GridBoxMode AddModifire(string id)
    {
        return CreateModifire(id);
    }

}