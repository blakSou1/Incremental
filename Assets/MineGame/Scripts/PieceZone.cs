using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PieceZone : MonoBehaviour
{
    public List<InteractiveObject> objects = new();

    public float spacing = 1f;

    public Rect capture;


    public UnityAction<InteractiveObject> OnClickDice;

    public void Claim(InteractiveObject toClaim)
    {
        if (toClaim.zone != null)
            toClaim.zone.Release(toClaim);

        toClaim.zone = this;

        var pos = 0;
        var insertFilters = G.interactor.FindAll<IFilterInsertPos>();
        foreach (var inf in insertFilters)
            pos = inf.OverrideIndex(pos, this, toClaim);
        objects.Insert(pos, toClaim);
    }

    List<InteractiveObject> alignedSet = new();

    private void Update()
    {
        alignedSet.Clear();
        for (var index = 0; index < objects.Count; index++)
        {
            var o = objects[index];

            alignedSet.Add(o);
        }

        for (var i = 0; i < alignedSet.Count; i++)
        {
            var targetPos = GetTargetPos(i, alignedSet);
            alignedSet[i].moveable.targetPosition = targetPos;
            alignedSet[i].order = i;
        }
    }

    public void Release(InteractiveObject toClaim)
    {
        if (objects.Contains(toClaim))
            objects.Remove(toClaim);
    }

    private Vector3 GetTargetPos(int i, List<InteractiveObject> setToWatch)
    {
        if (spacing < 1)
        {
            float totalOffset = 0f;

            // Calculate total offset by summing half the width of the current object and the previous objects' widths
            for (int j = 0; j < i; j++)
            {
                totalOffset += setToWatch[j].Width;
            }

            // Offset the current object by half of its own width for proper centering
            totalOffset += setToWatch[i].Width / 2f;

            // Calculate the current object position centered around the full set
            float centeredOffset = totalOffset - (GetTotalSetWidth(setToWatch) / 2f);

            // Return the new target position, taking into account spacing
            return transform.position + Vector3.right * centeredOffset;
        }

        var offset = i * spacing - (setToWatch.Count / 2f - 0.5f) * spacing;
        var targetPos = transform.position + Vector3.right * offset;
        return targetPos;
    }

    private float GetTotalSetWidth(List<InteractiveObject> setToWatch)
    {
        float totalWidth = 0f;
        foreach (var obj in setToWatch)
        {
            totalWidth += obj.Width;
        }
        return totalWidth;
    }

    void OnDrawGizmos()
    {
        if (capture.size != Vector2.zero)
        {
            Gizmos.color = new Color(0.5f, 0, 0, 0.25f);
            Gizmos.DrawCube(transform.position, capture.size);
            Gizmos.DrawWireCube(transform.position, capture.size);
        }
        else
        {
            Gizmos.color = new Color(0.5f, 0, 0, 0.25f);
            Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        }
    }
}

public interface IFilterInsertPos
{
    int OverrideIndex(int dindx, PieceZone pieceZone, InteractiveObject toClaim);
}