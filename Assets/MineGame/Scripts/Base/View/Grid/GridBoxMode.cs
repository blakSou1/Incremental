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
    [SerializeField] private AnimationController animSkillController;

    [HideInInspector] public modeState state;

    [HideInInspector] public AnimationController animationController;

    public AnimationDataSO SpawnModifireGridAnimDataSO;
    public AnimationDataSO ActivationGridAnimDataSO;
    public AnimationDataSO IsActivGridAnimDataSO;

    public void Start()
    {
        animationController = GetComponent<AnimationController>();
        animationController.Init();
        animSkillController.Init();

        StartCoroutine(animationController.FadeCoroutine(true, .3f, animationController._targetRenderer));
    }

    public IEnumerator ActivationScill()
    {
        animSkillController.SetAnimation(ActivationGridAnimDataSO);

        animationController.endAnimation.AddListener(G.mainEnterPoint.PlayerInputUpdate);
        animationController.endAnimation.AddListener(EndAnimActivScill);

        GridModifireBase model = state.model as GridModifireBase;

        yield return StartCoroutine(model.ActivationScill());
    }
    private void EndAnimActivScill()
    {
        animationController.SetAnimation(IsActivGridAnimDataSO);
    }

    public void SetState(modeState stat)
    {
        Start();

        animationController.SetAnimation(SpawnModifireGridAnimDataSO);

        state = stat;
        state.view = this;
    }
}
