using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Transform gridParent;

    [HideInInspector]
    public List<Transform> allGridSnapPoints = new List<Transform>();

    [HideInInspector]
    public bool[] occupied;

    void Awake()
    {
        Instance = this;

        BuildGrid();
        SyncOccupiedArray();
    }

    void BuildGrid()
    {
        allGridSnapPoints.Clear();

        foreach (Transform child in gridParent)
        {
            allGridSnapPoints.Add(child);
        }
    }

    public void SyncOccupiedArray()
    {
        occupied = new bool[allGridSnapPoints.Count];
    }

    public void RebuildOccupation()
    {
        for (int i = 0; i < occupied.Length; i++)
            occupied[i] = false;

        BlockSnapper[] pieces = FindObjectsOfType<BlockSnapper>();

        foreach (BlockSnapper piece in pieces)
        {
            piece.RefreshChildren();

            foreach (Transform block in piece.transform)
            {
                if (block.name.ToLower().Contains("slicepoint"))
                    continue;

                for (int i = 0; i < allGridSnapPoints.Count; i++)
                {
                    if (Vector3.Distance(block.position,
                                         allGridSnapPoints[i].position) < 0.2f)
                    {
                        occupied[i] = true;
                    }
                }
            }
        }
    }
}