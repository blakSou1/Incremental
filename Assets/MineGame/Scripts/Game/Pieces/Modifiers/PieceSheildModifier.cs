using System;
using UnityEngine;

[Serializable]
public class PieceSheildModifier : PieceModifierBase
{
    public AnimationClip AnimationSpawnShield;
    public AnimationClip AnimationDestroyShield;

    InteractiveObject interactiveO;

    bool sheild = true;

    public override void Init(InteractiveObject interactiveObject) 
    {
        interactiveO = interactiveObject;
        interactiveO.flipAnim.AddListener(IsFlip);

        interactiveO.SpawnPiece.AddListener(SpawnAnim);
    }

    private void SpawnAnim()
    {
        interactiveO.modefireAnimationController.Play(AnimationSpawnShield);
    }

    private void IsFlip()
    {
        if (sheild)
        {
            sheild = false;

            interactiveO.modefireAnimationController.Play(AnimationDestroyShield);
            interactiveO.modefireAnimationController.onClipEnd.AddListener(EndAnimSheild);
        }
    }
    private void EndAnimSheild(AnimationClip clip)
    {
        interactiveO.state.gridBox.Flip();
        G.UIController.UpdateCountPlayers();
    }
}

public class PieceModifierBase
{
    public virtual void Init(InteractiveObject interactiveObject) { }
}
