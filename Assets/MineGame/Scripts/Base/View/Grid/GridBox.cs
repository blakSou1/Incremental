using System.Collections;
using TMPro;
using UnityEngine;
using Index = System.Tuple<int, int>;

public class GridBox : MonoBehaviour
{
    public enum Status { Black = -1, None = 0, White = 1 }

    [HideInInspector] public Indic indic = null;

    private Index index = new(-1, -1);
    private Status stat = Status.None;
    [HideInInspector] public InteractiveObject piece;

    public TMP_Text debugTextWeight;

    [Space]
    [HideInInspector] public GridBoxMode boxMode;
    public GameObject hook;

    public void Flip()
    {
        var newC = (Status) ((int) stat * -1);
        piece.FlipAnim(newC);
        SetStat(newC);
    }

    public void SetModifire(string id)
    {
        var instance = G.chooice.AddModifire(id);

        instance.state.gridBox = this;

        instance.transform.SetParent(hook.transform);
        instance.transform.position = hook.transform.position;

        boxMode = instance;
    }

    public Status GetStat()
    {
        return stat;
    }
    public void SetStat(Status stat)
    {
        this.stat = stat;
    }

    public Index GetIndex()
    {
        return index;
    }
    public void SetIndex(int i, int j)
    {
        index = new Index(i, j);
    }

    public IEnumerator SetPiece(InteractiveObject piece)
    {
        this.piece = piece;

        if(boxMode != null && stat == G.mainEnterPoint.playerColor)
            yield return StartCoroutine(boxMode.ActivationScill());
    }//TODO
}
