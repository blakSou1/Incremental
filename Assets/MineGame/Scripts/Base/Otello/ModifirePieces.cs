using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifirePieces
{
    public List<SlotModPiece> slots;

    public void Init()
    {
        slots = new List<SlotModPiece>(GameObject.FindObjectsOfType<SlotModPiece>());
        slots = slots
        .Where(s => !(s is SlotModPieceStandart))
        .Concat(slots.Where(s => s is SlotModPieceStandart))
        .ToList();

        G.modifirePieces = this;
    }

    public void SpawnPiece()
    {
        var nonStandardSlots = slots.Where(s => !(s is SlotModPieceStandart)).ToList();
        var standardSlots = slots.Where(s => s is SlotModPieceStandart).ToList();

        // 1. Спавним специальные фишки
        for (int i = 0; i < nonStandardSlots.Count && i < G.configGame.piece.Count; i++)
        {
            var p = GameObject.Instantiate(G.configGame.piece[i],
                nonStandardSlots[i].transform.position,
                nonStandardSlots[i].transform.rotation);

            p.transform.parent = nonStandardSlots[i].transform;
            p.Start();
            p.animationController.SetAnimation(p.SpawnWhiteAnimDataSO);
            nonStandardSlots[i].piece = p;
        }

        // 2. Спавним стандартные фишки
        foreach (var standardSlot in standardSlots)
        {
            var p = GameObject.Instantiate(G.configGame.standertPiece,
                standardSlot.transform.position,
                standardSlot.transform.rotation);

            p.transform.parent = standardSlot.transform;
            p.Start();
            p.animationController.SetAnimation(p.SpawnWhiteAnimDataSO);
            standardSlot.piece = p;

            standardSlot.Click();
        }

    }
    public void Restart()
    {
        foreach (var i in slots)
            i.Restart();
    }
}
