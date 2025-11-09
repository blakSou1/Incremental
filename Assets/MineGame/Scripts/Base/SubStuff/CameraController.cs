using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float timeToMove = 0.8f;

    private void MoveToGamePos(Vector3 pos)
    {
        transform.DOMove(new Vector3(pos.x, pos.y, transform.position.z), timeToMove).SetEase(Ease.OutBack, 0.75f);
    }

    public void MoveToIndex(int index)
    {
        MoveToGamePos(G.Main.cameraPositionRoom[index].position);
    }
}
