using UnityEngine;

public class MoveableBalatro : MoveableBase
{
    private Vector2 velocity; // Текущая скорость
    private float maxVelocity; // Максимальная скорость

    private void Update()
    {
        // Предполагая, что realDt является дельтой времени между кадрами
        float realDt = Mathf.Clamp(Time.smoothDeltaTime, 1 / 50f, 1 / 100f);
        
        // Вычисляем затухание и максимальную скорость
        float expTimeXY = Mathf.Exp(-50 * realDt);
        maxVelocity = 70 * realDt;

        MoveXY(realDt, expTimeXY);
    }

    private void MoveXY(float dt, float expTimeXY)
    {
        Vector2 T = targetPosition; // Целевая позиция
        Vector2 currentPos = new(transform.position.x, transform.position.y); // Текущая позиция
        
        // Применяем экспоненциальное затухание к скорости
        velocity = expTimeXY * velocity + (1 - expTimeXY) * 35 * dt * (T - currentPos);
        
        // Ограничиваем скорость
        if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
            velocity = velocity.normalized * maxVelocity;

        // Обновляем позицию
        transform.position += 100f * dt * (Vector3)velocity;
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

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, 0.2f);
    }
}