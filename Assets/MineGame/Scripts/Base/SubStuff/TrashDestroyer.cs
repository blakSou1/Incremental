using UnityEngine;
using DG.Tweening;

public class TrashDestroyer : MonoBehaviour
{
    private Tweener currentPunchTween;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<MovableObject>() != null)
        {
            R.Audio.ThrowOut.PlayAsSoundRandomPitch(0.2f);
            Destroy(other.gameObject);
            if (currentPunchTween != null && currentPunchTween.IsActive())
            {
                currentPunchTween.Complete();
                currentPunchTween.Kill();
            }
            currentPunchTween = transform.GetChild(0).DOPunchScale(Vector3.one * -0.12f, 0.3f, elasticity: 0f, vibrato: 0).OnComplete(() => currentPunchTween = null);;
        }
    }
}
    