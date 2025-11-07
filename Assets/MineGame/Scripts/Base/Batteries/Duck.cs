using UnityEngine;

public class Duck : MovableObject
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        R.Audio.DuckSound.PlayAsSoundRandomPitch(0.2f);
    }
}
