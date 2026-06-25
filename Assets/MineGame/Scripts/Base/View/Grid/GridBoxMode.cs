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

        animationController.SetFadeCoroutine(true, .3f, animationController._targetRenderer);
    }

    public IEnumerator ActivationScill(Status stat)
    {
        animSkillController.SetAnimation(ActivationGridAnimDataSO);

        animationController.endAnimation.AddListener(G.mainEnterPoint.PlayerInputUpdate);
        animationController.endAnimation.AddListener(EndAnimActivScill);

        GridModBase model = state.model as GridModBase;

        if (stat == G.mainEnterPoint.playerColor)
            yield return StartCoroutine(model.ActivationScillPlayer());
        else
            yield return StartCoroutine(model.ActivationScillEnemy());
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
