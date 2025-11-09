using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    private Tweener currentPunchTween;
    public Transform posToSpawn;

    public void CreateIt()
    {
        CreateItem(true);
    }

    private void CreateItem(bool fromLever)
    {
        if (G.Main.isLockDown) return;

        if (fromLever) R.Audio.LevelDown.PlayAsSoundRandomPitch(0.1f);
        StartCoroutine(DelayedCreateItem());
    }

    private IEnumerator DelayedCreateItem()
    {
        yield return new WaitForSeconds(0.45f);
        R.Audio.PipeOutNewObject.PlayAsSoundRandomPitch(0.2f);

        if (currentPunchTween != null && currentPunchTween.IsActive())
        {
            currentPunchTween.Complete();
            currentPunchTween.Kill();
        }

        currentPunchTween = transform.DOPunchScale(Vector3.one * -0.45f, 0.4f, elasticity: 0f, vibrato: 0);
        yield return new WaitForSeconds(0.2f);

        Instantiate(G.Main.RandomSelector.SpinRoulette(),
                    posToSpawn.position + (Vector3.left * Random.Range(-0.3f, 0.3f)),
                    Quaternion.identity);
    }
}
