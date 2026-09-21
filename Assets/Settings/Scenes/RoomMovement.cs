using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RoomMovement : MonoBehaviour
{
    [Header("Точки движения")]
    [SerializeField] private Transform startPoint; 
    [SerializeField] private Transform endPoint;   

    [Header("Настройки движения")]
    [SerializeField] private float speed = 1f;

    public Transform parentGrid;
    public Transform GridCameraPosition;

    private bool isMoving = false;

    public UnityEvent OnReachedEnd = new();

    public void SpawnAndMove(GameObject prefab)
    {
        if (prefab == null)
            return;
        if (startPoint == null || endPoint == null)
            return;

        GameObject spawnedObject = Instantiate(prefab, startPoint.position, startPoint.rotation);
        isMoving = true;

        StartCoroutine(Move(spawnedObject));
    }
    
    private IEnumerator Move(GameObject spawnedObject)
    {
        if (!isMoving || spawnedObject == null) yield break;

        while (true)
        {
            spawnedObject.transform.position = Vector3.MoveTowards(
                spawnedObject.transform.position,
                endPoint.position,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(spawnedObject.transform.position, endPoint.position) < 0.001f)
            {
                spawnedObject.transform.position = endPoint.position;
                isMoving = false;

                OnReachedEnd.Invoke();
                OnReachedEnd.RemoveAllListeners();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}