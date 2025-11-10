using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMain : MonoBehaviour
{
    // GamePos
    public List<Transform> cameraPositionRoom;

    [HideInInspector] public Camera MainCamera;
    public WeightedRandomSelector RandomSelector;
    [HideInInspector] public bool isLockDown;

    MovableObject movableObject;

    private void Awake()
    {
        MainCamera = FindFirstObjectByType<Camera>();
        MainCamera.gameObject.AddComponent<CameraShake>();
    }

    public void Start()
    {
        StartCoroutine(InitCoroutine());

        G.inputs.Player.Attack.started += i => Raycast();
        G.inputs.Player.Attack.performed += i => Movable();
        G.inputs.Player.Attack.canceled += i => DisableDrag();
    }

    void Raycast()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
        RaycastHit2D hit = hits.FirstOrDefault(x => x.collider.transform.GetComponent<MovableObject>());
        if (hit.transform == null) return;

        movableObject = hit.transform.GetComponent<MovableObject>();
        movableObject.StartDragging();
    }
    void DisableDrag()
    {
        if (movableObject == null) return;

        movableObject.CheckAndPut();
        movableObject.body.gravityScale = movableObject.gravity;

        movableObject = null;
    }

    void Movable()
    {
        if (movableObject != null && G.inputs.Player.Attack.IsPressed())
            movableObject.StartCoroutine(movableObject.MoveTo());
    }

    private IEnumerator InitCoroutine()
    {
        G.Main = this;
        G.AudioManager.PlayMusic(R.Audio.MainGameMusic);

        ConfigItemsInPipe configItems = CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigItemsInPipe>())!.Get<ConfigItemsInPipe>().DeepCopy();
        RandomSelector = new WeightedRandomSelector();
        foreach (var it in configItems.pipeItems)
            RandomSelector.AddObject(it.DeepCopy());

        yield return null;
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
        int totalWeight = weightedObjects.Sum(item => item.weight);

        // Генерируем случайное число в диапазоне общего веса
        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;

        // Выбираем объект на основе случайного числа
        foreach (var item in weightedObjects)
        {
            currentWeight += item.weight;
            if (randomValue < currentWeight)
                return item.gameObject;
        }
        return weightedObjects[^1].gameObject;
    }

    public void UpdateWeights()
    {
        for (int i = 0; i < weightedObjects.Count; i++)
        {
            if (i == 0)
                weightedObjects[i].weight -= 30;
            else if (i == 1)
                weightedObjects[i].weight -= 10;
            else if (i == 2)
                weightedObjects[i].weight -= 3;
            else
                weightedObjects[i].weight += 1;
        }
    }
}
