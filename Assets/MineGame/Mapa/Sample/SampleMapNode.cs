using UnityEngine;

public class SampleMapNode : MapNodeView
{
    protected override void OnClickNode()
    {
        base.OnClickNode();
        GameObject.FindFirstObjectByType<SampleMapController>().OnClick(this);
    }
}
