using UnityEngine;

public class livitar : MonoBehaviour
{
    public float origin = 1;
    public bool i = false;

    private void FixedUpdate()
    {
        float offset = transform.position.x;
        float sin = Mathf.Sin(Time.time * 2 + offset);
        if (i) sin = -sin;

        float movementAmount = origin * sin * 0.1f;
        transform.Translate(0, movementAmount, 0, Space.Self);
        //transform.localPosition = new(transform.localPosition.x, transform.localPosition.y + origin * sin * .1f);
        //transform.localScale = Vector3.one + Vector3.one * sin * .15f;
    }
}
