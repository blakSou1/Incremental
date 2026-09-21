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
    public AnimationClip SpawnBlackAnimDataSO;
    public AnimationClip SpawnWhiteAnimDataSO;

    [Space]
    public AnimationClip MoveBlackAnimDataSO;
    public AnimationClip MoveWhiteAnimDataSO;

    [Space]
    [SerializeReference, SubclassSelector] public PieceModifierBase modifier;

    [HideInInspector] public ClipPlayer animationController;
    public ClipPlayer modefireAnimationController;

    [HideInInspector] public MoveableBase moveable;
    
    public PieceState state;

    [HideInInspector] public UnityEvent flipAnim;
    [HideInInspector] public UnityEvent SpawnPiece;

    public void Init()
    {
        moveable = GetComponent<MoveableBase>();
        animationController = GetComponent<ClipPlayer>();

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

    public void SetColor(Status color)
    {
        if (color == Status.Black)
            animationController.Play(SpawnBlackAnimDataSO);
        else if (color == Status.White)
            animationController.Play(SpawnWhiteAnimDataSO);
        else
            Destroy(this.gameObject);

        SetColorIcon(color);

        SpawnPiece?.Invoke();
    }

    private void SetColorIcon(Status color)
    {
        if (!iconPiece) return;

        Color targetColor;

        if (color == Status.Black)
            targetColor = Color.white;
        else if (color == Status.White)
            targetColor = Color.black;
        else
            return;

        var renderers = iconPiece.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var sr in renderers)
            sr.color = targetColor;
    }

    public void FlipAnim(Status color)
    {
        animationController.SetFadeCoroutine(false, .1f, iconPiece);
        SetColorIcon(color);

        if (color == Status.Black)
            animationController.Play(MoveBlackAnimDataSO);
        else if (color == Status.White)
            animationController.Play(MoveWhiteAnimDataSO);
        else
            return;

        animationController.onClipEnd.AddListener(EndAnimFlipToFabe);
    }

    private void EndAnimFlipToFabe(AnimationClip clip)
    {
        if (gameObject.activeInHierarchy)
            animationController.SetFadeCoroutine(true, .3f, iconPiece);

        flipAnim?.Invoke();
    }
}