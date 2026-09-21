using UnityEngine;

public class PieceFactory : MonoBehaviour
{
    private Transform parentPiece;

    private void Awake()
    {
        G.pieceFactory = this;
        parentPiece = new GameObject("PiecePool").transform;
        parentPiece.localScale = new(0.05f, 1, 0.05f);
    }

    private InteractiveObject CreatePiece(string t)
    {
        var basicPiece = CMS.Get<CMSEntity>(t);
        var prefab = basicPiece?.Get<TagPrefab>()?.prefab;

        if (basicPiece == null || prefab == null)
        {
            Debug.LogError($"[PieceFactory] Can't create '{t}': entity={basicPiece != null}, prefab={prefab != null}");
            return null;
        }

        var state = new PieceState { model = basicPiece };
        InteractiveObject instance = Instantiate(prefab);
        instance.Init();

        instance.gameObject.transform.SetParent(parentPiece, false);
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