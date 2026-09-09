using System.Collections.Generic;
using UnityEngine;

public class PieceController
{
    public void Init()
    {
        G.mainEnterPoint.pieceController = this;
    }

    public void StartInitModPiece()
    {
        G.run.hand = G.run.deck;

        List<string> piece = GetRandomElementsUnique(G.mainEnterPoint.modifierPieces.modSlots.Count, G.run.hand);

        foreach(string i in piece)
        {
            G.mainEnterPoint.modifierPieces.AddModPieceInSlot(i);
        }

        G.mainEnterPoint.modifierPieces.AddStandertPieceInSlot(ConfigGame.standardPiece);
    }

    //TODO

    public static List<string> GetRandomElementsUnique(int count, List<string> list)
    {
        List<string> result = new();

        if (list == null || list.Count == 0) return result;

        count = Mathf.Min(count, list.Count);

        List<string> tempList = new(list);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return result;
    }
}
