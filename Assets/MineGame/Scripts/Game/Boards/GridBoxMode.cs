using System.Collections;
using UnityEngine;

public class modeState
{
    public CMSEntity model;
    public GridBoxMode view;
    public GridBox gridBox;
}

public class GridBoxMode : MonoBehaviour
{
    public SpriteRenderer targetRenderer;

    [SerializeField] private ClipPlayer animSkillController;

    [HideInInspector] public modeState state;

    [HideInInspector] public ClipPlayer animationController;

    public AnimationClip SpawnModifireGridAnimDataSO;
    public AnimationClip ActivationGridAnimDataSO;
    public AnimationClip IsActivGridAnimDataSO;

    public void Start()
    {
        animationController = GetComponent<ClipPlayer>();

        animationController.SetFadeCoroutine(true, .3f, targetRenderer);
    }

    public IEnumerator ActivationScill(Status stat)
    {
        animSkillController.Play(ActivationGridAnimDataSO);

        animationController.onClipEnd.AddListener(EndAnimActivScill);

        GridModBase model = state.model as GridModBase;

        if (stat == G.run.playerColor)
            yield return StartCoroutine(model.ActivationScillPlayer());
        else
            yield return StartCoroutine(model.ActivationScillEnemy());
    }

    private void EndAnimActivScill(AnimationClip clip)
    {
        animationController.Play(IsActivGridAnimDataSO);
    }

    public void SetState(modeState stat)
    {
        Start();

        animationController.Play(SpawnModifireGridAnimDataSO);

        state = stat;
        state.view = this;
    }
}
