using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering.Universal;

public class GameMain : MonoBehaviour
{
    //GamePos
    public readonly Vector3 MachinePos = Vector3.zero;
    public readonly Vector3 DoorPos = new(-19.5f, 0, 0);
    public readonly Vector3 UpgradePos = new(19.5f, 0, 0);

    public Camera MainCamera;
    public Machine Machine;
    public TVLogic TV;
    [HideInInspector] public PipeController PipeController;
    public ParticleSystem ps;
    public WeightedRandomSelector RandomSelector;
    public bool isLockDown;
    public Light2D Light2D;
    public List<GameObject> ObjectToLockDown;
    public List<GameObject> ObjectToTurnOn;

    private void Awake()
    {
        MainCamera = FindFirstObjectByType<Camera>();
        MainCamera.gameObject.AddComponent<CameraShake>();
    }

    public void Start()
    {
        Init().Forget();
    }

    public async UniTask Init()
    {
        G.Main = this;
        G.AudioManager.PlayMusic(R.Audio.MainGameMusic);
        Machine = FindFirstObjectByType<Machine>();
        TV = FindFirstObjectByType<TVLogic>();
        PipeController = FindFirstObjectByType<PipeController>();
        PipeController.AutoSpawner().Forget();
        
        ConfigItemsInPipe configItems = CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigItemsInPipe>())!.Get<ConfigItemsInPipe>().DeepCopy();
        RandomSelector = new WeightedRandomSelector();
        foreach (var it in configItems.pipeItems)
        {
            RandomSelector.AddObject(it.DeepCopy());
        }
        
        await UniTask.Delay(5000);
        Machine.slots[0].OpenSlot();
    }

    public void LockDown()
    {
        isLockDown = true;
        Light2D.intensity = 0.002f;
        foreach (var VARIABLE in ObjectToLockDown)
        {
            VARIABLE.SetActive(false);
        }
        foreach (var VARIABLE in ObjectToTurnOn)
        {
            VARIABLE.SetActive(true);
        }
        ps.Play();
        Machine.StopMachine();
    }

    public void OpenNewSlot()
    {
        Machine.slots.Find(x => x.isClosed)?.OpenSlot();
    }

    public bool CheckCanSpawnNewItem()
    {
        int realCount = GameObject.FindObjectsByType<MovableObject>(FindObjectsSortMode.None).Count(x => x is not Cassete);
        int requaredCount = G.Main.Machine.slots.FindAll(x => !x.isClosed).ToList().Count + 6;
        return realCount <= requaredCount;
    }

}
[Serializable]
public class ConfigItemsInPipe : EntityComponentDefinition
{
    public Material baseAllShader;
    public GameObject boomVFX;
    public List<WeightedGameObject> pipeItems;
}
[Serializable]
public class WeightedGameObject
{
    public GameObject gameObject;
    public int weight;
    public WeightedGameObject()
    {
        gameObject = null;
        weight = 0;
    }
    public WeightedGameObject(GameObject g, int w)
    {
        gameObject = g;
        weight = w;
    }
}

public class WeightedRandomSelector
{
    public List<WeightedGameObject> weightedObjects = new();
    public void AddObject(WeightedGameObject obj)
    {
        weightedObjects.Add(obj);
    }
    public GameObject SpinRoulette()
    {
        if (weightedObjects.Count == 0)
        {
            Debug.LogWarning("No objects in the roulette!");
            return null;
        }

        // Вычисляем общий вес
        int totalWeight = 0;
        foreach (var item in weightedObjects)
        {
            totalWeight += item.weight;
        }

        // Генерируем случайное число в диапазоне общего веса
        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;

        // Выбираем объект на основе случайного числа
        foreach (var item in weightedObjects)
        {
            currentWeight += item.weight;
            if (randomValue < currentWeight)
            {
                return item.gameObject;
            }
        }
        return weightedObjects[^1].gameObject;
    }

    public void UpdateWeights()
    {
        for (int i = 0; i < weightedObjects.Count; i++)
        {
            if (i == 0)
            {
                weightedObjects[i].weight -= 30;
            }
            else if(i == 1)
            {
                weightedObjects[i].weight -= 10;
            }
            else if(i == 2)
            {
                weightedObjects[i].weight -= 3;
            }
            else
            {
                weightedObjects[i].weight += 1;
            }
        }
    }
}
