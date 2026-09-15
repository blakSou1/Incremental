using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Index = System.Tuple<int, int>;

public class GridController
{
    public Matrix matrix;

    [HideInInspector] public Transform parentIndc;

    [HideInInspector] public List<InteractiveObject> blackPieces = new();
    [HideInInspector] public List<InteractiveObject> whitePieces = new();

    [HideInInspector] public List<Indic> indicObjs = new();
    [HideInInspector] public List<GridBox> indicPositionGrid = new();

    private int size
    {
        get { return G.configGame.MatrixModel.matrixField.size; }
    }

    public void Init()
    {
        parentIndc = new GameObject("IndcPool").transform;
    }

    public void UpdateCountPiece(GridBox revColorPiece)
    {
        if (revColorPiece.GetStat() == Status.Black)
        {
            blackPieces.Add(revColorPiece.piece);
            whitePieces.Remove(revColorPiece.piece);
        }
        else
        {
            whitePieces.Add(revColorPiece.piece);
            blackPieces.Remove(revColorPiece.piece);
        }
    }

    #region SpawnerGrid

    public void StartInitModGrid()
    {
        G.mainEnterPoint.StartCoroutine(InitModifireGrid());
    }

    public IEnumerator InitModifireGrid()
    {
        int size = G.configGame.MatrixModel.matrixField.size;
        var data = matrix.GetData();

        for (int i = 0; i < size; i++)
        {
            for (int s = 0; s < size; s++)
            {
                string id = G.configGame.MatrixModel.matrixField.data.GetValue(i, s).idModifireGrid;

                if (string.IsNullOrEmpty(id))
                    continue;

                if (data[i, s] != null && data[i, s].GetStat() == Status.None)
                    data[i, s].SetModifier(G.configGame.MatrixModel.matrixField.data.GetValue(i, s).idModifireGrid);

                yield return new WaitForSeconds(.15f);
            }
        }
    }

    #endregion

    #region Matrix

    public void NewMatrix()
    {
        G.configGame.MatrixModel = G.configGame.GetConfigLevel().matrixNode.GetMatrix().matrixModel;
        matrix = new Matrix(G.configGame.MatrixModel);
    }

    public Vector2 IndexToVector2(Index index)
    {
        return matrix.GetGrid(index).transform.position;
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
    #endregion

    #region Indic

    public int CreateIndisObject(Status color, InteractiveObject piecePrefabValid)
    {
        ClearIndicObjs();

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                if (piecePrefabValid.GetBaseModel().Get<TagPieceRule>().valid.IsMoveValid(color, matrix.GetGrid(new Index(i, j)), out List<GridBox> revColorPieces))
                {
                    Vector2 v = IndexToVector2(new Index(i, j));
                    Indic indicators = GameObject.Instantiate(G.boardVisualConfig.highlightIndicator, new Vector3(v.x, v.y, parentIndc.transform.position.z), Quaternion.identity);
                    indicators.revColorPieces = revColorPieces;
                    indicators.transform.parent = parentIndc;

                    GridBox box = matrix.GetGrid(new Index(i, j));

                    box.indic = indicators;
                    indicPositionGrid.Add(box);

                    indicObjs.Add(indicators);
                }
            }
        }

        return indicObjs.Count;
    }

    private void ClearIndicObjs()
    {
        foreach (Indic obj in indicObjs)
            GameObject.Destroy(obj.gameObject);

        indicObjs.Clear();
        indicPositionGrid.Clear();
    }

    public void EnableAndDisableIndc(bool enable)
    {
        foreach (Indic i in indicObjs)
            i.gameObject.SetActive(enable);
    }


    #endregion

    #region GetFunction

    public List<GridBox> GetCrossPieces(Index i1, Index i2, bool excludeNone = true)
    {
        var list = new List<GridBox>();

        if (IsAdjacent(i1, i2)) return null;

        if (i1.Item1 == i2.Item1)
        {
            var start = Mathf.Min(i1.Item2, i2.Item2);
            var end = Mathf.Max(i1.Item2, i2.Item2);
            for (var j = start + 1; j <= end - 1; j++)
            {
                var grid = matrix.GetGrid(new Index(i1.Item1, j));
                if (grid.GetStat() != Status.None || !excludeNone)
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
                var grid = matrix.GetGrid(new Index(i, i1.Item2));
                if (grid.GetStat() != Status.None || !excludeNone)
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
                    var grid = matrix.GetGrid(new Index(
                        (b ? i1.Item1 : i2.Item1) + d,
                        (b ? i1.Item2 : i2.Item2) + d)
                    );

                    if (grid.GetStat() != Status.None || !excludeNone)
                        list.Add(grid);
                }

                return list;
            }
            else
            {
                var b = i1.Item1 < i2.Item1;
                var l = Mathf.Abs(i1.Item1 - i2.Item1);

                for (var d = 1; d < l; d++)
                {
                    var grid = matrix.GetGrid(new Index(
                        (b ? i1.Item1 : i2.Item1) + d,
                        (b ? i1.Item2 : i2.Item2) - d)
                    );

                    if (grid.GetStat() != Status.None || !excludeNone)
                        list.Add(grid);
                }

                return list;
            }
        }

        return null;
    }

    public List<GridBox> GetCrossPieces(Index index, bool excludeNone = true)
    {
        var list = new List<GridBox>();
        var cx = index.Item1;
        int cy = index.Item2;

        for (var i = 0; i < size; i++)
        {
            if (i == cx) continue;

            var grid = matrix.GetGrid(new Index(i, index.Item2));
            if (grid.GetStat() != Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var j = 0; j < size; j++)
        {
            if (j == cy) continue;

            var grid = matrix.GetGrid(new Index(index.Item1, j));
            if (grid.GetStat() != Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= size - 1; d++)
        {
            if (cx + d >= size || cy + d >= size) continue;

            var grid = matrix.GetGrid(new Index(cx + d, cy + d));
            if (grid.GetStat() != Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= size - 1; d++)
        {
            if (cx - d < 0 || cy - d < 0) continue;

            var grid = matrix.GetGrid(new Index(cx - d, cy - d));
            if (grid.GetStat() != Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= size - 1; d++)
        {
            if (cx + d >= size || cy - d < 0) continue;

            var grid = matrix.GetGrid(new Index(cx + d, cy - d));
            if (grid.GetStat() != Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= size - 1; d++)
        {
            if (cx - d < 0 || cy + d >= size) continue;

            var grid = matrix.GetGrid(new Index(cx - d, cy + d));
            if (grid.GetStat() != Status.None || !excludeNone)
                list.Add(grid);
        }

        return list;
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

    #endregion

}
