using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BlockSnapper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float snapThreshold = 0.5f;

    private Vector3 startPosition;
    private List<Transform> childBlocks = new List<Transform>();
    private Vector3 dragOffset;

    private bool isSnappedToGrid = false;

    void Awake()
    {
        RefreshChildren();
    }

    void Start()
    {
        UpdateStartPosition();
    }

    public void RefreshChildren()
    {
        childBlocks.Clear();
        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("slicepoint")) continue;
            childBlocks.Add(child);
        }
    }

    public void UpdateStartPosition()
    {
        startPosition = transform.position;
    }

    private Vector3 GetMouseWorldPos(PointerEventData eventData)
    {
        float cameraDepth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, cameraDepth);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LevelManager.isLevelComplete) return;

        RefreshChildren();

        Vector3 mouseWorldPos = GetMouseWorldPos(eventData);
        dragOffset = transform.position - mouseWorldPos;

        GridManager.Instance.RebuildOccupation();
        FindObjectOfType<LevelManager>().CheckCompletion();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (LevelManager.isLevelComplete) return;

        Vector3 mouseWorldPos = GetMouseWorldPos(eventData);
        transform.position = mouseWorldPos + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (LevelManager.isLevelComplete) return;

        var grid = GridManager.Instance.allGridSnapPoints;
        Transform referenceBlock = (childBlocks.Count > 0) ? childBlocks[0] : transform;

        int closestIndex = -1;
        float minDist = snapThreshold;

        for (int i = 0; i < grid.Count; i++)
        {
            float d = Vector3.Distance(referenceBlock.position, grid[i].position);
            if (d < minDist)
            {
                minDist = d;
                closestIndex = i;
            }
        }

        if (closestIndex == -1)
        {
            isSnappedToGrid = false;
            UpdateStartPosition();
            return;
        }

        Vector3 oldPos = transform.position;
        Vector3 snapDisplacement = grid[closestIndex].position - referenceBlock.position;
        transform.position += snapDisplacement;

        if (IsOverlappingOtherPieces())
        {
            transform.position = oldPos;
            return;
        }

        isSnappedToGrid = true;

        
        GridManager.Instance.RebuildOccupation();
        UpdateStartPosition();

        LevelManager.Instance.CheckCompletion();
    }

    bool IsOverlappingOtherPieces()
    {
        float threshold = 0.08f;

        foreach (BlockSnapper other in FindObjectsOfType<BlockSnapper>())
        {
            if (other == this) continue;

            other.RefreshChildren();

            foreach (Transform a in childBlocks)
            {
                foreach (Transform b in other.childBlocks)
                {
                    if (Vector3.Distance(a.position, b.position) < threshold)
                        return true;
                }
            }
        }
        return false;
    }

    public void RecenterPiece()
    {
        if (transform.childCount == 0) return;

        Transform anchorBlock = null;

        foreach (Transform child in transform)
        {
            if (!child.name.ToLower().Contains("slicepoint"))
            {
                anchorBlock = child;
                break;
            }
        }

        if (anchorBlock == null) return;

        Vector3 targetParentPos = anchorBlock.position;

        List<Transform> blocksToMove = new List<Transform>();
        List<Transform> slicePointsToPreserve = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("slicepoint"))
                slicePointsToPreserve.Add(child);
            else
                blocksToMove.Add(child);
        }

        GameObject tempStorage = new GameObject("TempStorage");

        foreach (Transform sp in slicePointsToPreserve)
            sp.SetParent(null, true);

        foreach (Transform block in blocksToMove)
            block.SetParent(tempStorage.transform, true);

        transform.position = targetParentPos;

        foreach (Transform block in blocksToMove)
            block.SetParent(transform, true);

        foreach (Transform sp in slicePointsToPreserve)
            sp.SetParent(transform, true);

        Destroy(tempStorage);

        RefreshChildren();
    }
}