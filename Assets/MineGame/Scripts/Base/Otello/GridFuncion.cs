using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class GridFuncion
{
    public GridBox prefabGridBox;

    public Vector2 indentGrid;

    public Indic indcObj;

    public int item1 = 8;

    private Matrix matrix;

    private Transform parentIndc;

    [HideInInspector] public List<InteractiveObject> blackPieces;
    [HideInInspector] public List<InteractiveObject> whitePieces;

    [HideInInspector] public List<Indic> indicObjs = new();

    public void Init()
    {
        parentIndc = new GameObject("IndcPool").transform;

        G.winAndLouse = new();
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

    public void ClearIndicObjs()
    {
        foreach (Indic obj in indicObjs)
            GameObject.Destroy(obj.gameObject);

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
        var (cx, cy) = index;

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

    public void EnableAndDisableIndc(bool enable)
    {
        foreach(Indic i in indicObjs)
            i.gameObject.SetActive(enable);
    }

    public bool ShowPossibleLocation(GridBox.Status color)//возможность хода для игрока
    {
        ClearIndicObjs();

        for (var i = 0; i < item1; i++)
        {
            for (var j = 0; j < item1; j++)
            {
                if (G.gameLogic.pieceModel.GetBaseModel().CheckPieceValid(color, matrix.GetGrid(new Index(i, j)), out List<GridBox> revColorPieces))
                {
                    Vector2 v = IndexToVector2(new Index(i, j));
                    Indic indc = GameObject.Instantiate(indcObj, new Vector3(v.x, v.y, parentIndc.transform.position.z), Quaternion.identity);
                    indc.revColorPieces = revColorPieces;
                    indc.transform.parent = parentIndc;

                    matrix.GetGrid(new Index(i, j)).indic = indc;

                    indicObjs.Add(indc);
                }
            }
        }

        if (indicObjs.Count == 0)
        {
            if (blackPieces.Count + whitePieces.Count == item1 * item1)
            {
                if (G.gridFuncion.blackPieces.Count < G.gridFuncion.whitePieces.Count)
                    WinEnemy();
                else if (G.gridFuncion.blackPieces.Count > G.gridFuncion.whitePieces.Count)
                    WinPlayer();
                else
                    Drav();
                return false;
            }
            else
            {
                List<SlotModPiece> nonStandardSlots = G.modifirePieces.slots.Where(s => s is not SlotModPieceStandart).ToList();

                foreach (SlotModPiece slot in nonStandardSlots)
                {
                    if (slot.piece == null) continue;

                    for (var i = 0; i < item1; i++)
                    {
                        for (var j = 0; j < item1; j++)
                        {
                            if (slot.piece.GetBaseModel().CheckPieceValid(color, matrix.GetGrid(new Index(i, j)), out List<GridBox> revColorPieces))
                            {
                                Vector2 v = IndexToVector2(new Index(i, j));
                                Indic indc = GameObject.Instantiate(indcObj, new Vector3(v.x, v.y, parentIndc.transform.position.z), Quaternion.identity);
                                indc.revColorPieces = revColorPieces;
                                indc.transform.parent = parentIndc;

                                matrix.GetGrid(new Index(i, j)).indic = indc;

                                indicObjs.Add(indc);
                            }
                        }
                    }

                }

            }

            G.gameMode.StartCoroutine(Pass());

            return false;
        }

        return true;
    }

    private void Drav()
    {
        if (blackPieces.Count < whitePieces.Count)
            WinEnemy();
        else if (blackPieces.Count > whitePieces.Count)
            WinPlayer();
        else
            G.gameMode.IndicatorText("Draw");

        G.gameMode.motionText.ThrowText(new LocString("Draw!", "Ничья!"), R.normalVoice);

        G.gameMode.isGameEnd = true;
    }

    private IEnumerator Pass()
    {
        G.gameMode.IndicatorText("Pass");

        if(G.gameMode.playerColor == G.PlayerController.playerColor)
            G.gameMode.motionText.ThrowText(new LocString("No move!", "Нет хода!"), R.normalVoice);
        else
            G.gameMode.motionText.ThrowText(new LocString("The enemy has no move!", "У противника нет хода!"), R.normalVoice);

        yield return new WaitForSeconds(1.3f);

        G.gameMode.disableInputForPass = true;
        G.gameMode.StartCoroutine(Next());

        IEnumerator Next()
        {
            yield return new WaitForSeconds(2.0f);
            G.gameMode.IndicatorText("");
            G.gameMode.disableInputForPass = false;
        }

        G.gameLogic.PassTurn();
    }

    public void CreateIndisObject(GridBox.Status color)
    {
        ClearIndicObjs();

        for (var i = 0; i < item1; i++)
        {
            for (var j = 0; j < item1; j++)
            {
                if (G.gameLogic.pieceModel != null && G.gameLogic.pieceModel.GetBaseModel().CheckPieceValid(color, matrix.GetGrid(new Index(i, j)), out List<GridBox> revColorPieces))
                {
                    Vector2 v = IndexToVector2(new Index(i, j));
                    Indic indc = GameObject.Instantiate(indcObj, new Vector3(v.x, v.y, parentIndc.transform.position.z), Quaternion.identity);
                    indc.revColorPieces = revColorPieces;
                    indc.transform.parent = parentIndc;

                    matrix.GetGrid(new Index(i, j)).indic = indc;

                    indicObjs.Add(indc);
                }
            }
        }
    }

    public List<GridBox> GetPossibleLocation(GridBox.Status color)//возможность хода для противника
    {
        List<GridBox> list = new List<GridBox>();

        for (var i = 0; i < item1; i++)
        {
            for (var j = 0; j < item1; j++)
            {
                if (G.gameLogic.pieceModel.GetBaseModel().CheckPieceValid(color, matrix.GetGrid(new Index(i, j)), out List<GridBox> revColorPieces))
                    list.Add(G.gridFuncion.GetMatrix().GetGrid(new Index(i, j)));
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
                    WinEnemy();
                    return null;
                }
                WinPlayer();
                return null;
            }//не осталось фишек на доске

            if (G.gridFuncion.blackPieces.Count + G.gridFuncion.whitePieces.Count == item1 * item1)
            {

                if (G.gridFuncion.blackPieces.Count < G.gridFuncion.whitePieces.Count)
                    WinEnemy();
                else if (G.gridFuncion.blackPieces.Count > G.gridFuncion.whitePieces.Count)
                    WinPlayer();
                else
                    Drav();

                G.gameMode.isGameEnd = true;

                return null;
            }//заполнено поле

            G.gameMode.StartCoroutine(Pass());
            return null;
        }
    }

    private void WinPlayer()
    {
        G.gameMode.motionText.ThrowText(new LocString("You Win!", "Победа!"), R.normalVoice);
        G.gameMode.IndicatorText("Win");

        G.gameMode.StartCoroutine(G.winAndLouse.Win());
    }
    private void WinEnemy()
    {
        G.gameMode.motionText.ThrowText(new LocString("Loss!", "Проиграл!"), R.normalVoice);
        G.gameMode.IndicatorText("Loss");

        G.gameMode.StartCoroutine(G.winAndLouse.Loss());
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

    public Transform GetParentInd()
    {
        return parentIndc;
    }
    public Matrix GetMatrix()
    {
        return matrix;
    }
}
