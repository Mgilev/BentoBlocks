using UnityEngine;
using UnityEngine.EventSystems;

public class Vertical : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float snapThreshold = 2.5f;

    [HideInInspector]
    public SlicePoint activeSlicePoint;

    [HideInInspector]
    public Vector3 startPosition; 

    void Start()
    {
        startPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        activeSlicePoint = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = 0;
        transform.position = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SlicePoint[] points = Object.FindObjectsByType<SlicePoint>(FindObjectsSortMode.None);

        SlicePoint closest = null;
        float minDist = snapThreshold;

        foreach (var p in points)
        {
            if (p.allowedKnife == SlicePoint.KnifeTypeRestriction.HorizontalOnly) continue;

            float d = Vector3.Distance(transform.position, p.transform.position);

            if (d < minDist)
            {
                minDist = d;
                closest = p;
            }
        }

        if (closest != null)
        {
            transform.position = new Vector3(closest.transform.position.x, closest.transform.position.y, 0);
            activeSlicePoint = closest;
            Debug.Log($"[Vertical] Successfully snapped to {closest.name}!");
        }
        else
        {
            transform.position = startPosition; 
            activeSlicePoint = null;
        }
    }
}