using UnityEngine;
using UnityEngine.Events;

public class PieceState
{
    public CMSEntity model;
    public InteractiveObject view;
    public bool isPlayed;
    public bool isDead;
    public GridBox gridBox;
}

public class InteractiveObject : MonoBehaviour
{
    public SpriteRenderer iconPiece;

    [Space]
    public AnimationDataSO SpawnBlackAnimDataSO;
    public AnimationDataSO SpawnWhiteAnimDataSO;

    [Space]
    public AnimationDataSO MoveBlackAnimDataSO;
    public AnimationDataSO MoveWhiteAnimDataSO;

    [Space]
    [SerializeReference, SubclassSelector] public PieceModifierBase modifier;

    [HideInInspector] public AnimationController animationController;
    public AnimationController modefireAnimationController;

    [HideInInspector] public MoveableBase moveable;
    
    public PieceState state;

    [HideInInspector] public UnityEvent flipAnim;
    [HideInInspector] public UnityEvent SpawnPiece;

    public void Start()
    {
        moveable = GetComponent<MoveableBase>();
        animationController = GetComponent<AnimationController>();
        animationController.Init();
        modefireAnimationController?.Init();

        modifier?.Init(this);

        animationController.SetFadeCoroutine(true, .3f, iconPiece);
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

        SpawnPiece?.Invoke();
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
        animationController.SetFadeCoroutine(false, .1f, iconPiece);
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
        if (gameObject.activeInHierarchy)
            animationController.SetFadeCoroutine(true, .3f, iconPiece);

        flipAnim?.Invoke();
    }
}