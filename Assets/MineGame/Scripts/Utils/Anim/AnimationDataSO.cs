using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Animation", menuName = "Animations/AnimationDataSO")]
public class AnimationDataSO : ScriptableObject
{
    [field: SerializeField] public float framerate { get; private set; }
    [field: SerializeField] public List<Sprite> frames { get; private set; } = new List<Sprite>();
    [field: SerializeField] public List<Frame> framesSettings { get; private set; } = new List<Frame>();
    [field: SerializeField] public Vector2 animationOffset { get; private set; }
    [field: SerializeField] public bool isLoop { get; private set; } = false;

}

[Serializable]
public class Frame
{
    [field: SerializeField] public int indexFrame { get; private set; }
    [field: SerializeField] public UnityEvent Event { get; private set; }
    [field: SerializeField] public float pause { get; private set; } = 0;
}
