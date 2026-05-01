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

    private int item1
    {
        get { return G.configGridFunction.item1; }
    }

    public void Init()
    {
        G.gridController = this;
        parentIndc = new GameObject("IndcPool").transform;
    }

    public void UpdateCountPiece(GridBox revColorPiece)
    {
        if (revColorPiece.GetStat() == GridBox.Status.Black)
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
        SpawnDamageGrid();

    }
    //TODO

    private void SpawnDamageGrid()
    {
        int random = Random.Range(0, 4);
        RandomSpawnModGridBox(G.configGame.GetConfigLevel().GetConfigEnemy().hp + random, ConfigGame.damageModBox);
    }

    private void RandomSpawnModGridBox(int countSpawnDamageBox = 0, string id = "")
    {
        List<GridBox> freeCells = new();

        var data = matrix.GetData();

        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                if (data[x, y] != null && data[x, y].GetStat() == GridBox.Status.None)
                    freeCells.Add(data[x, y]);
            }
        }

        for (int i = 0; i < countSpawnDamageBox; i++)
        {
            if (freeCells.Count <= 0)
                return;

            int randomIndex = Random.Range(0, freeCells.Count);
            freeCells[randomIndex].SetModifire(id);

            freeCells.RemoveAt(randomIndex);
        }

    }

    #endregion

    #region Matrix

    public void NewMatrix()
    {
        matrix = new Matrix(G.configGridFunction.item1);
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

    public int CreateIndisObject(GridBox.Status color, InteractiveObject piecePrefabValid)
    {
        ClearIndicObjs();

        for (var i = 0; i < item1; i++)
        {
            for (var j = 0; j < item1; j++)
            {
                if (piecePrefabValid.GetBaseModel().CheckPieceValid(color, matrix.GetGrid(new Index(i, j)), out List<GridBox> revColorPieces))
                {
                    Vector2 v = IndexToVector2(new Index(i, j));
                    Indic indc = GameObject.Instantiate(G.configGridFunction.indcObj, new Vector3(v.x, v.y, parentIndc.transform.position.z), Quaternion.identity);
                    indc.revColorPieces = revColorPieces;
                    indc.transform.parent = parentIndc;

                    GridBox box = matrix.GetGrid(new Index(i, j));

                    box.indic = indc;
                    indicPositionGrid.Add(box);

                    indicObjs.Add(indc);
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

    public List<Index> GetCenterCells()
    {
        List<Index> centerCells = new()
        {
            new(G.configGridFunction.item1 / 2 - 1, G.configGridFunction.item1 / 2 - 1),
            new(G.configGridFunction.item1 / 2, G.configGridFunction.item1 / 2 - 1),
            new(G.configGridFunction.item1 / 2, G.configGridFunction.item1 / 2),
            new(G.configGridFunction.item1 / 2 - 1, G.configGridFunction.item1 / 2)
        };

        return centerCells;
    }

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
                var grid = matrix.GetGrid(new Index(i, i1.Item2));
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
                    var grid = matrix.GetGrid(new Index(
                        (b ? i1.Item1 : i2.Item1) + d,
                        (b ? i1.Item2 : i2.Item2) + d)
                    );

                    if (grid.GetStat() != GridBox.Status.None || !excludeNone)
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

                    if (grid.GetStat() != GridBox.Status.None || !excludeNone)
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

        for (var i = 0; i < item1; i++)
        {
            if (i == cx) continue;

            var grid = matrix.GetGrid(new Index(i, index.Item2));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var j = 0; j < item1; j++)
        {
            if (j == cy) continue;

            var grid = matrix.GetGrid(new Index(index.Item1, j));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= item1 - 1; d++)
        {
            if (cx + d >= item1 || cy + d >= item1) continue;

            var grid = matrix.GetGrid(new Index(cx + d, cy + d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= item1 - 1; d++)
        {
            if (cx - d < 0 || cy - d < 0) continue;

            var grid = matrix.GetGrid(new Index(cx - d, cy - d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= item1 - 1; d++)
        {
            if (cx + d >= item1 || cy - d < 0) continue;

            var grid = matrix.GetGrid(new Index(cx + d, cy - d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                list.Add(grid);
        }

        for (var d = 1; d <= item1 - 1; d++)
        {
            if (cx - d < 0 || cy + d >= item1) continue;

            var grid = matrix.GetGrid(new Index(cx - d, cy + d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
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
