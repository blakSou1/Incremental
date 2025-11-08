using UnityEngine;
using System.Collections;

public class LimbFlail : MonoBehaviour
{
    [System.Serializable]
    public class Limb
    {
        public Rigidbody2D rigidbody;
        public Vector2 forceDirection = Vector2.up;
        public float baseForce = 50f;
        public float torqueForce = 25f;
    }

    [SerializeField] private Limb[] limbs;
    [SerializeField] private float flailDuration = 1.5f;
    [SerializeField] private float flailIntensity = 1f;

    private bool isFlailing = false;

    public void StartPainFlail()
    {
        if (!isFlailing)
        {
            StartCoroutine(FlailLimbs());
        }
    }

    private IEnumerator FlailLimbs()
    {
        isFlailing = true;
        float timer = 0f;

        while (timer < flailDuration)
        {
            foreach (Limb limb in limbs)
            {
                if (limb.rigidbody != null)
                {
                    // Осциллирующая сила для более естественного движения
                    float sinWave = Mathf.Sin(timer * 20f) * flailIntensity;

                    Vector2 force = limb.forceDirection * limb.baseForce * sinWave;
                    limb.rigidbody.AddForce(force);

                    float torque = Mathf.Cos(timer * 15f) * limb.torqueForce;
                    limb.rigidbody.AddTorque(torque);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isFlailing = false;
    }

    // Для отладки в редакторе
    [ContextMenu("Test Flail")]
    private void TestFlail()
    {
        StartPainFlail();
    }
}