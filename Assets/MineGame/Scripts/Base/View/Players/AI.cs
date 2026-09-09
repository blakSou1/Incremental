using System;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private List<List<int>> gridWeight = new();
    bool isInitWeigth = false;

    bool isDebug = false;

    public void InitWeight()
    {
        gridWeight.Clear();
        int size = G.configGame.MatrixModel.matrixField.size;

        for (int i = 0; i < size; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < size; j++)
            {
                int value = CalculateWeightParabolic(i, j, size);
                row.Add(value);
            }
            gridWeight.Add(row);
        }
        isInitWeigth = true;
    }

    private int CalculateWeightParabolic(int x, int y, int size)
    {
        // Преобразуем координаты в диапазон [-1, 1]
        float normalizedX = (2f * x / (size - 1)) - 1f;
        float normalizedY = (2f * y / (size - 1)) - 1f;

        // Параболическая функция - чем ближе к центру, тем выше значение
        float distanceFromCenter = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
        float parabolicValue = 1f - distanceFromCenter;

        // Усиливаем углы
        bool isCorner = Mathf.Abs(normalizedX) > 0.9f && Mathf.Abs(normalizedY) > 0.9f;
        if (isCorner) parabolicValue += 1.2f;

        //Усиливаем края
        bool isEdge = Mathf.Abs(normalizedX) == 1f || Mathf.Abs(normalizedY) == 1f;
        if (isEdge && !isCorner) parabolicValue += .7f;

        // Ослабляем клетки рядом с углами
        bool isNearCorner = (Mathf.Abs(normalizedX) > 0.7f && Mathf.Abs(normalizedY) > 0.7f) && !isCorner;
        if (isNearCorner) parabolicValue -= 0.3f;

        return Mathf.RoundToInt(parabolicValue * 30);
    }

    public void DebugMethod()
    {
        if (!isInitWeigth) return;

        if (isDebug)
        {
            DeleteDebugMethod();
            isDebug = !isDebug;
            return;
        }

        isDebug = !isDebug;

        int size = G.configGame.MatrixModel.matrixField.size;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
                G.mainEnterPoint.gridController.matrix.GetGrid(new(i,j)).debugTextWeight.text = $"{gridWeight[i][j]}";
        }
    }
    private void DeleteDebugMethod()
    {
        int size = G.configGame.MatrixModel.matrixField.size;

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                G.mainEnterPoint.gridController.matrix.GetGrid(new(i, j)).debugTextWeight.text = "";
    }

    public void Execute(Status comColor)
    {
        List<GridBox> possibleLocs = G.mainEnterPoint.conditionsOfVictoryAndDefeat.GetPossibleLocation(comColor);

        if (possibleLocs == null || possibleLocs.Count == 0) StartCoroutine(G.mainEnterPoint.conditionsOfVictoryAndDefeat.Pass());

        var max = -100;
        GridBox selectedLoc = null;
        
        foreach (GridBox loc in possibleLocs)
        {
            var locIndex = loc.GetIndex();
            int placedVal = 0;
            
            try
            {
                placedVal = gridWeight[locIndex.Item1][locIndex.Item2];
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogError("Index Error! / " + loc.ToString());
                placedVal = 0;
            }

            var flipedSumVal = 0;

            List<GridBox> dummy = loc.indic?.revColorPieces;

            if (dummy != null)
            {
                foreach (var fp in dummy)
                {
                    var fp_index = fp.GetIndex();
                    var fp_weight = gridWeight[fp_index.Item1][fp_index.Item2];
                    flipedSumVal += fp_weight + 10;
                }
            }

            int randomFactor = UnityEngine.Random.Range(-10, 10);
            var score = placedVal + flipedSumVal + randomFactor;

            if (score > max)
            {
                max = score;
                selectedLoc = loc;
            }
        }

        G.mainEnterPoint.StartCoroutine(G.mainEnterPoint.gameLogic.PlacePiece(comColor, selectedLoc));
    }
}
