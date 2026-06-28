using UnityEngine;
using System.Collections.Generic;

public class SplitManager : MonoBehaviour
{
    [Tooltip("Drag your Horizontal Knife and Vertical Knife GameObjects here from the Hierarchy.")]
    public GameObject[] knifeObjects;

    [Header("Slice Limits")]
    public int maxHorizontalSlices = 3;
    public int maxVerticalSlices = 3;

    private int horizontalSlicesRemaining;
    private int verticalSlicesRemaining;

    private void Start()
    {
        horizontalSlicesRemaining = maxHorizontalSlices;
        verticalSlicesRemaining = maxVerticalSlices;
    }

    public void ExecuteSplit()
    {
        if (knifeObjects == null || knifeObjects.Length == 0)
        {
            Debug.LogError("[SplitManager] Knife array is empty!");
            return;
        }

        SlicePoint sp = null;
        GameObject activeKnifeObj = null;
        KnifeSnapper activeHorizontal = null;
        Vertical activeVertical = null;

      
        foreach (GameObject knifeObj in knifeObjects)
        {
            if (knifeObj == null || !knifeObj.activeSelf) continue;

            KnifeSnapper horizontalKnife = knifeObj.GetComponent<KnifeSnapper>();
            Vertical verticalKnife = knifeObj.GetComponent<Vertical>();

            if (horizontalKnife != null && horizontalKnife.activeSlicePoint != null)
            {
                sp = horizontalKnife.activeSlicePoint;
                activeHorizontal = horizontalKnife;
                activeKnifeObj = knifeObj;
                break;
            }

            if (verticalKnife != null && verticalKnife.activeSlicePoint != null)
            {
                sp = verticalKnife.activeSlicePoint;
                activeVertical = verticalKnife;
                activeKnifeObj = knifeObj;
                break;
            }
        }

        if (sp == null || activeKnifeObj == null)
        {
            Debug.LogError("[SplitManager] No knife is snapped and ready to cut!");
            return;
        }

        BlockSnapper original = sp.ownerPiece;
        if (original == null || sp.partsToDisconnect == null || sp.partsToDisconnect.Count == 0)
        {
            Debug.LogError("[SplitManager] SlicePoint is missing references!");
            return;
        }

       
        List<Transform> cutParts = new List<Transform>(sp.partsToDisconnect);
        GameObject newPiece = new GameObject("SplitPiece");
        newPiece.transform.position = cutParts[0].position;
        BlockSnapper newSnapper = newPiece.AddComponent<BlockSnapper>();

        
        foreach (Transform part in cutParts)
        {
            if (part != null)
            {
                part.SetParent(newPiece.transform, true);
            }
        }

       
        List<SlicePoint> allSlicePoints = new List<SlicePoint>();
        allSlicePoints.AddRange(original.GetComponentsInChildren<SlicePoint>());
        allSlicePoints.AddRange(newSnapper.GetComponentsInChildren<SlicePoint>());

      
        List<SlicePoint> targetsToDestroy = new List<SlicePoint>();
        targetsToDestroy.Add(sp); 

       
        if (sp.slicePointsToChainDestroy != null)
        {
            foreach (SlicePoint extraPoint in sp.slicePointsToChainDestroy)
            {
                if (extraPoint != null && !targetsToDestroy.Contains(extraPoint))
                {
                    targetsToDestroy.Add(extraPoint);
                }
            }
        }

       
        foreach (SlicePoint deadPoint in targetsToDestroy)
        {
            allSlicePoints.Remove(deadPoint);
        }

       
        foreach (SlicePoint deadPoint in targetsToDestroy)
        {
            Destroy(deadPoint.gameObject);
        }

        
        foreach (SlicePoint pointScript in allSlicePoints)
        {
            if (pointScript == null) continue;

            Transform slicePointObj = pointScript.transform;
            Transform bestMatchingPieceParent = original.transform;
            float closestDistance = float.MaxValue;

            
            foreach (Transform child in original.transform)
            {
                
                if (child.GetComponent<SlicePoint>() != null) continue;
                
                if (child.GetComponent<BlockSnapper>() != null) continue;

                float dist = Vector3.Distance(slicePointObj.position, child.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestMatchingPieceParent = original.transform;
                }
            }

          
            foreach (Transform child in newSnapper.transform)
            {
               
                if (child.GetComponent<SlicePoint>() != null) continue;
                
                if (child.GetComponent<BlockSnapper>() != null) continue;

                float dist = Vector3.Distance(slicePointObj.position, child.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestMatchingPieceParent = newSnapper.transform;
                }
            }

            
            slicePointObj.SetParent(bestMatchingPieceParent, true);

            
            pointScript.ownerPiece = bestMatchingPieceParent.GetComponent<BlockSnapper>();

            
            pointScript.DeterminePartsToDisconnect();
        }
       

       
        original.RefreshChildren();
        newSnapper.RefreshChildren();

        original.RecenterPiece();
        newSnapper.RecenterPiece();

        original.UpdateStartPosition();
        newSnapper.UpdateStartPosition();

       
        if (activeHorizontal != null)
        {
            horizontalSlicesRemaining--;
            if (horizontalSlicesRemaining <= 0)
            {
                activeKnifeObj.SetActive(false);
                Debug.Log("[SplitManager] Horizontal Knife has run out of slices and disappeared!");
            }
            else
            {
                activeKnifeObj.transform.position = activeHorizontal.startPosition;
                activeHorizontal.activeSlicePoint = null;
            }
        }
        else if (activeVertical != null)
        {
            verticalSlicesRemaining--;
            if (verticalSlicesRemaining <= 0)
            {
                activeKnifeObj.SetActive(false);
                Debug.Log("[SplitManager] Vertical Knife has run out of slices and disappeared!");
            }
            else
            {
                activeKnifeObj.transform.position = activeVertical.startPosition;
                activeVertical.activeSlicePoint = null;
            }
        }

        Debug.Log($"[SplitManager] Separation completed! H-Slices left: {horizontalSlicesRemaining}, V-Slices left: {verticalSlicesRemaining}");
    }
}