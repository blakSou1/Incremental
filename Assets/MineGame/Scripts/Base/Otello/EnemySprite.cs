using System.Collections;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [HideInInspector] public AnimationController animationController;
    public float speed = .4f;

    Material material;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animationController = GetComponent<AnimationController>();
        animationController.Init();

        material = spriteRenderer.material;
        material.SetFloat("_FadeAmount", 1);
    }

    public void UpdateSprite()
    {
        animationController.SetAnimation(G.configGame.GetConfigLevel().GetConfigEnemy().enemySpawnAnim);
        transform.localScale = G.configGame.GetConfigLevel().GetConfigEnemy().Scale;

        StartCoroutine(EnableSprite());
    }

    public IEnumerator DisableSprite()
    {
        float startTime = Time.time;

        while (Time.time - startTime < speed)
        {
            float fractionOfJourney = Mathf.Clamp01((Time.time - startTime) / speed);

            material.SetFloat("_FadeAmount", fractionOfJourney);

            yield return null;
        }

        material.SetFloat("_FadeAmount", 1);
    }
    public IEnumerator EnableSprite()
    {
        float startTime = Time.time;

        while (Time.time - startTime < speed)
        {
            float fractionOfJourney = Mathf.Clamp01((Time.time - startTime) / speed);

            material.SetFloat("_FadeAmount", 1 - fractionOfJourney);

            yield return null;
        }

        material.SetFloat("_FadeAmount", -.1f);
    }
}
