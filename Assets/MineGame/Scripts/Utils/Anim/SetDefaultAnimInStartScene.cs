using System.Collections;
using UnityEngine;

public class SetDefaultAnimInStartScene : MonoBehaviour
{
    public AnimationDataSO animationStart;

    private void Start()
    {
        AnimationController c = GetComponent<AnimationController>();
        c.Init();

        StartCoroutine(Time(c));
    }
    private IEnumerator Time(AnimationController c)
    {
        yield return new WaitForSeconds(Random.Range(0, .7f));
        c.SetAnimation(animationStart);
    }
}
