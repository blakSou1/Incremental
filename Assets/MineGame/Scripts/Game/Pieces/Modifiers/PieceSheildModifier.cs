using System;

[Serializable]
public class PieceSheildModifier : PieceModifierBase
{
    public AnimationDataSO AnimationSpawnShield;
    public AnimationDataSO AnimationDestroyShield;

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
        interactiveO.modefireAnimationController.SetAnimation(AnimationSpawnShield);
    }

    private void IsFlip()
    {
        if (sheild)
        {
            sheild = false;

            interactiveO.modefireAnimationController.SetAnimation(AnimationDestroyShield);
            interactiveO.modefireAnimationController.endAnimation.AddListener(EndAnimSheild);
        }
    }
    private void EndAnimSheild()
    {
        interactiveO.state.gridBox.Flip();
        G.UIController.UpdateCountPlayers();
    }
}

public class PieceModifierBase
{
    public virtual void Init(InteractiveObject interactiveObject) { }
}
