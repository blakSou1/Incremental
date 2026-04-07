using UnityEngine;

public class PieceState
{
    public CMSEntity model;
    public InteractiveObject view;
    public bool isPlayed;
    public bool isDead;
}

public class InteractiveObject : MonoBehaviour
{
    public SpriteRenderer iconPiece;

    public AnimationDataSO SpawnBlackAnimDataSO;
    public AnimationDataSO SpawnWhiteAnimDataSO;

    public AnimationDataSO MoveBlackAnimDataSO;
    public AnimationDataSO MoveWhiteAnimDataSO;

    [HideInInspector] public AnimationController animationController;
    [HideInInspector] public MoveableBase moveable;
    
    public PieceState state;

    public void Start()
    {
        moveable = GetComponent<MoveableBase>();
        animationController = GetComponent<AnimationController>();
        animationController.Init();

        StartCoroutine(animationController.FadeCoroutine(true, .3f, iconPiece));
    }

    public PieceBase GetBaseModel()
    {
        return state.model as PieceBase;
    }

    public void SetState(PieceState pieceState)
    {
        state = pieceState;
        state.view = this;
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

        SetColorIcon(color);
    }

    private void SetColorIcon(GridBox.Status color)
    {
        if (!iconPiece) return;

        if (color == GridBox.Status.Black)
            iconPiece.color = Color.white;
        else if (color == GridBox.Status.White)
            iconPiece.color = Color.black;
    }

    public void FlipAnim(GridBox.Status color)
    {
        StartCoroutine(animationController.FadeCoroutine(false, .1f, iconPiece));
        SetColorIcon(color);

        if (color == GridBox.Status.Black)
            animationController.SetAnimation(MoveBlackAnimDataSO);
        else if (color == GridBox.Status.White)
            animationController.SetAnimation(MoveWhiteAnimDataSO);
        else
            return;

        animationController.endAnimation.AddListener(G.mainEnterPoint.PlayerInputUpdate);
        animationController.endAnimation.AddListener(EndAnimFlipToFabe);
    }

    private void EndAnimFlipToFabe()
    {
        if(gameObject.activeInHierarchy)
            StartCoroutine(animationController.FadeCoroutine(true, .3f, iconPiece));
    }
}