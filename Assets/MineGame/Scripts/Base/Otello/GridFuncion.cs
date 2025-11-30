using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class GridFuncion
{
    public GridBox prefabGridBox;

    public Vector2 indentGrid;

    public GameObject indcObj;

    public int item1 = 8;

    private Matrix matrix;

    private Transform parentIndc;

    [HideInInspector] public List<Piece> blackPieces;
    [HideInInspector] public List<Piece> whitePieces;

    [HideInInspector] public List<GameObject> indicObjs = new();

    public void Init()
    {
        parentIndc = new GameObject("IndcPool").transform;
    }

    public void InitIndexPos()
    {
        NewMatrix();
    }

    public void NewMatrix()
    {
        matrix = new Matrix(item1);
    }

    public Vector2 IndexToVector2(Index index)
    {
        return matrix.GetGrid(index).transform.position;
    }

    public Index Vector2ToIndex(Vector2 snapPos)
    {
        GridBox[] firstLevel = new GridBox[matrix.GetData().GetLength(0)];
        for (int s = 0; s < matrix.GetData().GetLength(0); s++)
            firstLevel[s] = matrix.GetData()[s, 0];

        var i = Array.FindIndex(firstLevel, x => x.transform.position.x == snapPos.x);

        firstLevel = new GridBox[matrix.GetData().GetLength(0)];
        for (int s = 0; s < matrix.GetData().GetLength(0); s++)
            firstLevel[s] = matrix.GetData()[0, s];

        var j = Array.FindIndex(firstLevel, y => y.transform.position.y == snapPos.y);

        return new Index(i, j);
    }

    public void ClearIndicObjs()
    {
        foreach (var obj in indicObjs)
            GameObject.Destroy(obj);

        indicObjs.Clear();
    }

    public void ClearAllPieces()
    {
        foreach (var bp in blackPieces)
            GameObject.Destroy(bp.gameObject);

        foreach (var wp in whitePieces)
            GameObject.Destroy(wp.gameObject);

        blackPieces.Clear();
        whitePieces.Clear();

        ClearIndicObjs();

        NewMatrix();
    }

    public IEnumerable<GridBox> GetCrossPieces(Index i1, Index i2, bool excludeNone = true)
    {
        var list = new List<GridBox>();

        if (IsAdjacent(i1, i2)) return null;

        if (i1.Item1 == i2.Item1)
        {
            var start = Mathf.Min(i1.Item2, i2.Item2);
            var end = Mathf.Max(i1.Item2, i2.Item2);
            for (var j = start + 1; j <= end - 1; j++)
            {
                var grid = GetMatrix().GetGrid(new Index(i1.Item1, j));
                if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                    list.Add(grid);
            }

            return list;
        }

        if (i1.Item2 == i2.Item2)
        {
            var start = Mathf.Min(i1.Item1, i2.Item1);
            var end = Mathf.Max(i1.Item1, i2.Item1);
            for (var i = start + 1; i <= end - 1; i++)
            {
                var grid = GetMatrix().GetGrid(new Index(i, i1.Item2));
                if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                    list.Add(grid);
            }

            return list;
        }

        var dx = i1.Item1 - i2.Item1;
        var dy = i1.Item2 - i2.Item2;

        if (Mathf.Abs(dx) == Mathf.Abs(dy))
        {
            if (Mathf.Sign(dx) == Mathf.Sign(dy))
            {
                var b = i1.Item1 < i2.Item1;
                var l = Mathf.Abs(i1.Item1 - i2.Item1);

                for (var d = 1; d < l; d++)
                {
                    var grid = GetMatrix().GetGrid(new Index(
                        (b ? i1.Item1 : i2.Item1) + d,
                        (b ? i1.Item2 : i2.Item2) + d)
                    );

                    if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                    {
                        list.Add(grid);
                    }
                }

                return list;
            }
            else
            {
                var b = i1.Item1 < i2.Item1;
                var l = Mathf.Abs(i1.Item1 - i2.Item1);

                for (var d = 1; d < l; d++)
                {
                    var grid = GetMatrix().GetGrid(new Index(
                        (b ? i1.Item1 : i2.Item1) + d,
                        (b ? i1.Item2 : i2.Item2) - d)
                    );

                    if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                        list.Add(grid);
                }

                return list;
            }
        }

        return null;
    }

    public IEnumerable<GridBox> GetCrossPieces(Index index, bool excludeNone = true)
    {
        var list = new List<GridBox>();
        var (cx, cy) = index;

        for (var i = 0; i < 8; i++)
        {
            if (i == cx) continue;

            var grid = GetMatrix().GetGrid(new Index(i, index.Item2));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var j = 0; j < 8; j++)
        {
            if (j == cy) continue;

            var grid = GetMatrix().GetGrid(new Index(index.Item1, j));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy + d >= 8) continue;

            var grid = GetMatrix().GetGrid(new Index(cx + d, cy + d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy - d < 0) continue;

            var grid = GetMatrix().GetGrid(new Index(cx - d, cy - d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy - d < 0) continue;

            var grid = GetMatrix().GetGrid(new Index(cx + d, cy - d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy + d >= 8) continue;

            var grid = GetMatrix().GetGrid(new Index(cx - d, cy + d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        return list;
    }

    public List<Index> GetCenterCells()
    {
        List<Index> centerCells = new()
        {
            new(item1 / 2 - 1, item1 / 2 - 1),
            new(item1 / 2, item1 / 2 - 1),
            new(item1 / 2, item1 / 2),
            new(item1 / 2 - 1, item1 / 2)
        };

        return centerCells;
    }

    public void ShowPossibleLocation(GridBox.Status color)
    {
        ClearIndicObjs();

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                if (G.gameLogic.CheckPieceValid(color, GetMatrix().GetGrid(new Index(i, j)), out List<GridBox> dummy))
                {
                    var v = IndexToVector2(new Index(i, j));
                    var indc = GameObject.Instantiate(indcObj, new Vector3(v.x, v.y, -1), Quaternion.identity);
                    indc.transform.parent = parentIndc;

                    indicObjs.Add(indc);
                }
            }
        }

        if (indicObjs.Count == 0)
        {
            if (blackPieces.Count + whitePieces.Count == 8 * 8)
            {
                if (blackPieces.Count < whitePieces.Count)
                    WhiteBlack();
                else if (blackPieces.Count > whitePieces.Count)
                    WinBlack();
                else
                    G.gameMode.indicatorText.text = "Drew";

                G.gameMode.isGameEnd = true;

                return;
            }

            G.gameMode.indicatorText.text = "Pass";
            G.gameMode.disableInputForPass = true;
            G.gameMode.StartCoroutine(Next());

            IEnumerator Next()
            {
                yield return new WaitForSeconds(2.0f);
                G.gameMode.indicatorText.text = "";
                G.gameMode.disableInputForPass = false;
            }

            G.gameLogic.PassTurn();
        }
    }

    public List<Index> GetPossibleLocation(GridBox.Status color)
    {
        var list = new List<Index>();

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                if (G.gameLogic.CheckPieceValid(color, G.gridFuncion.GetMatrix().GetGrid(new Index(i, j)), out List<GridBox> dummy))
                    list.Add(new Index(i, j));
            }
        }

        if (list.Count != 0)
            return list;
        else//нет доступных ходов
        {
            if (G.gridFuncion.blackPieces.Count == 0 || G.gridFuncion.whitePieces.Count == 0)
            {
                if (G.gridFuncion.blackPieces.Count == 0)
                {
                    WhiteBlack();
                    return null;
                }
                WinBlack();
                return null;
            }

            if (G.gridFuncion.blackPieces.Count + G.gridFuncion.whitePieces.Count == 8 * 8)
            {
                if (G.gridFuncion.blackPieces.Count < G.gridFuncion.whitePieces.Count)
                    WhiteBlack();
                else if (G.gridFuncion.blackPieces.Count > G.gridFuncion.whitePieces.Count)
                    WinBlack();
                else
                    G.gameMode.indicatorText.text = "Drew";

                G.gameMode.isGameEnd = true;

                return null;
            }

            G.gameMode.indicatorText.text = "Pass";
            G.gameMode.disableInputForPass = true;
            G.gameMode.StartCoroutine(Next());

            IEnumerator Next()
            {
                yield return new WaitForSeconds(2.0f);
                G.gameMode.indicatorText.text = "";
                G.gameMode.disableInputForPass = false;
            }

            G.gameLogic.PassTurn();
            return null;
        }
    }

    private void WinBlack()
    {
        G.gameMode.motionText.ThrowText(new LocString("Black Win!", "Черные победили!"), R.normalVoice);

        G.gameMode.indicatorText.text = "Black Win";
    }
    private void WhiteBlack()
    {
        G.gameMode.motionText.ThrowText(new LocString("White Win!", "Белые победили!"), R.normalVoice);

        G.gameMode.indicatorText.text = "White Win";
    }

    public bool IsAdjacent(Index i1, Index i2)
    {
        for (var x = i1.Item1 - 1; x <= i1.Item1 + 1; x++)
        {
            for (var y = i1.Item2 - 1; y <= i1.Item2 + 1; y++)
                if (x == i2.Item1 && y == i2.Item2) return true;
        }

        return false;
    }

    public Matrix GetMatrix()
    {
        return matrix;
    }
}
