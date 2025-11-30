using UnityEngine;

public class Piece : MonoBehaviour
{
    public AnimationDataSO SpawnBlackAnimDataSO;
    public AnimationDataSO SpawnWhiteAnimDataSO;

    public AnimationDataSO MoveBlackAnimDataSO;
    public AnimationDataSO MoveWhiteAnimDataSO;
    
    private AnimationController animationController;

    void Start()
    {
        animationController = GetComponent<AnimationController>();
        animationController.Init();
    }

    public void SetColor(GridBox.Status color)
    {
        Start();

        if (color == GridBox.Status.Black)
            animationController.SetAnimation(SpawnBlackAnimDataSO);
        else if (color == GridBox.Status.White)
            animationController.SetAnimation(SpawnWhiteAnimDataSO);
        else
            Destroy(this.gameObject);
    }

    public void FlipAnim(GridBox.Status to)
    {
        if (to == GridBox.Status.Black)
            animationController.SetAnimation(MoveBlackAnimDataSO);
        else if (to == GridBox.Status.White)
            animationController.SetAnimation(MoveWhiteAnimDataSO);
        else
            return;

        animationController.endAnimation.AddListener(G.gameMode.PlayerInputUpdate);
    }
}
