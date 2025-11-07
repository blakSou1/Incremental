using UnityEngine;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    private float timeToMove = 0.8f;
    public void MoveToGamePos(Vector3 pos)
    {
        transform.DOMove(new Vector3(pos.x, pos.y, transform.position.z), timeToMove).SetEase(Ease.OutBack, 0.75f);
    }

    public void MoveToDoor()
    {
        MoveToGamePos(G.Main.DoorPos);
    }
    public void MoveToMachine()
    {
        MoveToGamePos(G.Main.MachinePos);
    }
    public void MoveToUpgrades()
    {
        MoveToGamePos(G.Main.UpgradePos);
    }

    public void GoSuperLeft()
    {
        MoveToGamePos(Vector3.one * 20);
    }
}
