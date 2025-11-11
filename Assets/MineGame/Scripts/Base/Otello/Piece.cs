using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{    
    public Sprite BlackSprite;
    public Sprite WhiteSprite;

    public Sprite[] FlipSprites;
    
    private SpriteRenderer SR;
    private readonly float flipInterval = 0.03f;    // Higher is Slower

    public bool isAnimating = false;

    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    public void SetColor(GridBox.Status color)
    {
        Start();

        if (color == GridBox.Status.Black)
            SR.sprite = BlackSprite;
        else if (color == GridBox.Status.White)
            SR.sprite = WhiteSprite;
        else
            Destroy(this.gameObject);
    }

    public void FlipAnim(GridBox.Status to)
    {
        if (FlipSprites == null || FlipSprites.Length == 0)
            return;

        if (to == GridBox.Status.Black)
        {
            var curIndex = -1;

            isAnimating = true;
            StartCoroutine(Next());
            IEnumerator Next()
            {
                yield return new WaitForSeconds(flipInterval);

                curIndex++;
                if (curIndex < FlipSprites.Length)
                {
                    SR.sprite = FlipSprites[curIndex];
                    StartCoroutine(Next());
                }
                else
                    isAnimating = false;
            }
        }
        else
        {
            var curIndex = FlipSprites.Length; // Начинаем с конца массива

            isAnimating = true;
            StartCoroutine(Next());
            IEnumerator Next()
            {
                yield return new WaitForSeconds(flipInterval);

                curIndex--;
                if (curIndex > 0)
                {
                    SR.sprite = FlipSprites[curIndex - 1];
                    StartCoroutine(Next());
                }
                else
                    isAnimating = false;
            }
        }
    }
}
