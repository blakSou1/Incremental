using TMPro;
using UnityEngine;
using Index = System.Tuple<int, int>;

public class GridBox : MonoBehaviour
{
    public enum Status { Black = -1, None = 0, White = 1 }

    private Index index = new(-1, -1);
    private Status stat = Status.None;
    [HideInInspector] public Piece piece;

    public TMP_Text debugTextWeight;

    public void Flip()
    {
        var newC = (Status) ((int) stat * -1);
        piece.FlipAnim(newC);
        SetStat(newC);
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

    public void SetPiece(Piece piece)
    {
        this.piece = piece;
    }
}
