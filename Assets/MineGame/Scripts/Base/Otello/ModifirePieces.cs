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
        for (int i = 0; i < nonStandardSlots.Count && i < G.configGame.piece.Count; i++)
        {
            Piece p = GameObject.Instantiate(G.configGame.piece[i],
                nonStandardSlots[i].transform.position,
                nonStandardSlots[i].transform.rotation);

            p.transform.parent = nonStandardSlots[i].transform;
            p.Start();
            SetColorAnimPiece(p);
            nonStandardSlots[i].piece = G.configGame.piece[i];
        }

        // 2. Спавним стандартные фишки
        foreach (SlotModPiece standardSlot in standardSlots)
        {
            Piece p = GameObject.Instantiate(G.configGame.standertPiece,
                standardSlot.transform.position,
                standardSlot.transform.rotation);

            p.transform.parent = standardSlot.transform;
            p.Start();
            SetColorAnimPiece(p);
            standardSlot.piece = G.configGame.standertPiece;

            standardSlot.Click();
        }


    }

    private void SetColorAnimPiece(Piece p)
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
