using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : MonoBehaviour
{
    public Volume volume;

    private void Start()
    {
        G.eventManager.VolumeWeugth.AddListener(SetWeigth);
    }

    private void SetWeigth(float weight)
    {
        volume.weight = weight;
    }
}
