using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifirePieces
{
    public List<SlotModPiece> slots;

    public void Init()
    {
        slots = new List<SlotModPiece>(GameObject.FindObjectsByType<SlotModPiece>(FindObjectsSortMode.None));
        slots = slots
        .Where(s => s is not SlotModPieceStandart)
        .Concat(slots.Where(s => s is SlotModPieceStandart))
        .ToList();

        G.modifirePieces = this;
    }

    public void SpawnPiece()
    {
        List<SlotModPiece> nonStandardSlots = slots.Where(s => s is not SlotModPieceStandart).ToList();
        List<SlotModPiece> standardSlots = slots.Where(s => s is SlotModPieceStandart).ToList();

        // 1. Спавним специальные фишки
        for (int i = 0; i < nonStandardSlots.Count && i < G.run.pieceStorage.Count; i++)
        {
            var p = G.chooice.AddPiece(G.run.pieceStorage[i].model.id);
            p.moveable.targetPosition = nonStandardSlots[i].transform.position;
            p.transform.rotation = nonStandardSlots[i].transform.rotation;

            p.transform.parent = nonStandardSlots[i].transform;
            p.Start();
            SetColorAnimPiece(p);
            nonStandardSlots[i].piece = G.run.pieceStorage[i].view;
        }

        // 2. Спавним стандартные фишки
        foreach (SlotModPiece standardSlot in standardSlots)
        {
            var p = G.chooice.AddPiece(ConfigGame.standertPiece);
            p.moveable.targetPosition = standardSlot.transform.position;
            p.transform.rotation = standardSlot.transform.rotation;

            p.transform.parent = standardSlot.transform;
            p.Start();
            SetColorAnimPiece(p);
            standardSlot.piece = p;

            standardSlot.Click();
        }
    }

    private void SetColorAnimPiece(InteractiveObject p)
    {
        if(G.gameMode.playerColor == GridBox.Status.White)
            p.animationController.SetAnimation(p.SpawnWhiteAnimDataSO);
        else
            p.animationController.SetAnimation(p.SpawnBlackAnimDataSO);
    }

    public void Restart()
    {
        foreach (var i in slots)
            i.Restart();
    }
}
