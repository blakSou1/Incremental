using System;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private List<List<int>> gridWeight = new();

    private void Start()
    {
        InitWeight();
    }

    void InitWeight()
    {
        var l0 = new List<int>() {50, -10, 15, 15, 15, 15, -10, 50};
        var l1 = new List<int>() {-10, -20, -10, -10, -10, -10, -20, -10};
        var l2 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l3 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l4 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l5 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l6 = new List<int>() {-10, -20, -10, -10, -10, -10, -20, -10};
        var l7 = new List<int>() {50, -10, 15, 15, 15, 15, -10, 50};
        
        gridWeight.Add(l0);
        gridWeight.Add(l1);
        gridWeight.Add(l2);
        gridWeight.Add(l3);
        gridWeight.Add(l4);
        gridWeight.Add(l5);
        gridWeight.Add(l6);
        gridWeight.Add(l7);
    }

    public void Execute(GridBox.Status comColor)
    {
        var possibleLocs = G.gridFuncion.GetPossibleLocation(comColor);

        if (possibleLocs == null) return;

        var max = -100;
        var selectedLoc = new Tuple<int, int>(-1, -1);
        
        foreach (var loc in possibleLocs)
        {
            int placedVal = 0;
            
            try
            {
                placedVal = gridWeight[loc.Item1][loc.Item2];
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogError("Index Error! / " + loc.ToString());
                placedVal = 0;
            }

            var flipedSumVal = 0;

            G.gameLogic.CheckPieceValid(comColor, G.gridFuncion.GetMatrix().GetGrid(loc), out List<GridBox> flipedList, false);

            foreach (var fp in flipedList)
            {
                var fp_index = fp.GetIndex();
                var fp_weight = gridWeight[fp_index.Item1][fp_index.Item2];

                flipedSumVal += fp_weight + 10;
            }

            var score = placedVal + flipedSumVal;

            if (score > max)
            {
                max = score;
                selectedLoc = loc;
            }
        }

        G.gameLogic.PlacePiece(comColor, selectedLoc);
    }
}
