using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBalatro : MoveableBase
{
    private Vector3 velocity;
    private float maxVelocity;

    private void Update()
    {
        if (isStop) return;

        float realDt = Mathf.Clamp(Time.smoothDeltaTime, 1 / 50f, 1 / 100f);
        
        float expTimeXY = Mathf.Exp(-50 * realDt);
        maxVelocity = 70 * realDt;

        MoveXYZ(realDt, expTimeXY);
    }

    private void MoveXYZ(float dt, float expTimeXY)
    {
        Vector3 T = targetPosition;
        Vector3 currentPos = transform.position;

        velocity = expTimeXY * velocity + (1 - expTimeXY) * 35 * dt * (T - currentPos);

        if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
            velocity = velocity.normalized * maxVelocity;

        transform.position += 100f * dt * velocity;

        if (events.Count != 0 && Vector3.Distance(transform.position, targetPosition) < .2)
        {
            foreach (var i in events)
                i?.Invoke();
            events.Clear();
        }
    }
}

//public class MoveableSmoothDamp : MoveableBase
//{
//    private Vector3 velocity;
//    public float smoothTime = 0.3F;
//    public float maxVelocity = 10f;
//    private Vector3 currentVelocity;

//    void Start()
//    {
//        targetPosition = transform.position;
//    }

//    protected override void PausableUpdate()
//    {
//        MoveXY();
//    }

//    protected void MoveXY()
//    {
//        if (Vector3.Distance(transform.position, targetPosition) > 0.01f || velocity.magnitude > 0.01f)
//        {
//            Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime, maxVelocity, Time.deltaTime);
//            velocity = (newPosition - (Vector3)transform.position) / Time.deltaTime;

//            if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
//            {
//                velocity = velocity.normalized * maxVelocity;
//            }

//            transform.position = newPosition + velocity * Time.deltaTime;
//            if (Vector3.Distance((Vector3)transform.position, targetPosition) < 0.01f && velocity.magnitude < 0.01f)
//            {
//                transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
//                velocity = Vector3.zero;
//            }
//        }
//    }
//}

public class MoveableBase : ManagedBehaviour
{
    public Vector3 targetPosition;
    [NonSerialized] public bool isStop = false;
    [NonSerialized] public List<Action> events = new();

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, 0.2f);
    }
}