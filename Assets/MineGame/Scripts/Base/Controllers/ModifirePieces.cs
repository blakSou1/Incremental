using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifirePieces
{
    public List<SlotModPiece> modSlots;
    public SlotModPiece standartSlot;

    public void Init()
    {
        var allSlots = GameObject.FindObjectsByType<SlotModPiece>(FindObjectsSortMode.None).ToList();

        standartSlot = allSlots.FirstOrDefault(s => s is SlotModPieceStandart);
        modSlots = allSlots.Where(s => s is not SlotModPieceStandart).ToList();

        G.mainEnterPoint.modifierPieces = this;
    }

    public void AddModPieceInSlot(string piece)
    {
        InteractiveObject p = G.choice.AddPiece(piece);
        p.Start();
        SetColorAnimPiece(p);

        foreach (SlotModPiece i in modSlots)
        {
            if (i.piece == null)
            {
                p.moveable.targetPosition = i.transform.position;
                p.transform.rotation = i.transform.rotation;

                p.transform.parent = i.transform;

                i.piece = p;
            }
        }
    }
    
    public void AddStandertPieceInSlot(string piece)
    {
        InteractiveObject p = G.choice.AddPiece(piece);
        p.Start();
        SetColorAnimPiece(p);

        p.moveable.targetPosition = standartSlot.transform.position;
        p.transform.rotation = standartSlot.transform.rotation;

        p.transform.parent = standartSlot.transform;

        standartSlot.piece = p;

        standartSlot.Click();
    }

    private void SetColorAnimPiece(InteractiveObject p)
    {
        if(G.run.playerColor == Status.White)
            p.animationController.SetAnimation(p.SpawnWhiteAnimDataSO);
        else
            p.animationController.SetAnimation(p.SpawnBlackAnimDataSO);
    }

    public void Restart()
    {
        foreach (var i in modSlots)
            i.Restart();

        standartSlot.Restart();
    }
}
