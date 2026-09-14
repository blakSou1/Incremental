using UnityEngine;

public delegate void PointerReactor();

public class ReactToPointer : MonoBehaviour
{
    public string TooltipMessage;
    public float HoverDelay = .2f;
}
