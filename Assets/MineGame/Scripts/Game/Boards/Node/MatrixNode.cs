using System;
using UnityEngine;
using UnityEngine.Events;

public class MatrixNode : MonoBehaviour
{
    [NonSerialized]
    public UnityEvent<string> UpEvent = new();

    public virtual void AddListenerUp()
    {
        UpEvent.RemoveAllListeners();
        UpEvent.AddListener(UP);
    }

    public virtual Param GetMatrix()
    {
        return null;
    }

    public virtual void UP(string s)
    {
    }

}
