using System.Collections;
using UnityEngine;

public class PieceState
{
    public CMSEntity model;
    public int rollValue;
    public InteractiveObject view;
    public bool isPlayed;
    public bool isDead;

    public bool isClaimed;
}

public class InteractiveObject : MonoBehaviour
{
    public SpriteRenderer iconPiece;

    public AnimationDataSO SpawnBlackAnimDataSO;
    public AnimationDataSO SpawnWhiteAnimDataSO;

    public AnimationDataSO MoveBlackAnimDataSO;
    public AnimationDataSO MoveWhiteAnimDataSO;

    [HideInInspector] public AnimationController animationController;
    
    public PieceState state;

    public MoveableBase moveable;

    public PieceZone zone;
    public int order;

    public void Start()
    {
        animationController = GetComponent<AnimationController>();
        animationController.Init();

        StartCoroutine(FadeCoroutine(true, .3f));
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
        StartCoroutine(FadeCoroutine(false, .1f));
        SetColorIcon(color);

        if (color == GridBox.Status.Black)
            animationController.SetAnimation(MoveBlackAnimDataSO);
        else if (color == GridBox.Status.White)
            animationController.SetAnimation(MoveWhiteAnimDataSO);
        else
            return;

        animationController.endAnimation.AddListener(G.gameMode.PlayerInputUpdate);
        animationController.endAnimation.AddListener(EndAnimFlipToFabe);
    }

    private void EndAnimFlipToFabe()
    {
        if(gameObject.activeInHierarchy)
            StartCoroutine(FadeCoroutine(true, .3f));
    }

    protected IEnumerator FadeCoroutine(bool fadeIn, float fadeDuration)
    {
        if (iconPiece == null) yield break;

        float startAlpha = iconPiece.color.a;
        float targetAlpha = fadeIn ? 1f : 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color color = iconPiece.color;
            color.a = currentAlpha;
            iconPiece.color = color;

            yield return null;
        }

        Color finalColor = iconPiece.color;
        finalColor.a = targetAlpha;
        iconPiece.color = finalColor;
    }

    public float Width = 1;


}