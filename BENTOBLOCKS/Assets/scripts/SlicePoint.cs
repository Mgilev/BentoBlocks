using UnityEngine;
using System.Collections.Generic;

public class SlicePoint : MonoBehaviour
{
    public enum KnifeTypeRestriction { Any, HorizontalOnly, VerticalOnly }

    [Header("Restrictions")]
    [Tooltip("Which knife is allowed to snap to this point?")]
    public KnifeTypeRestriction allowedKnife = KnifeTypeRestriction.Any;

    [Header("References")]
    [Tooltip("The main BlockSnapper script this slice point belongs to.")]
    public BlockSnapper ownerPiece;

    [Tooltip("This list will now fill automatically based on position and rotation!")]
    public List<Transform> partsToDisconnect = new List<Transform>();

    [Header("Chain Destruction Setup")]
    [Tooltip("Drag and drop other SlicePoints here that should ALSO be deleted when this point is sliced.")]
    public List<SlicePoint> slicePointsToChainDestroy = new List<SlicePoint>();

    public float detectionRadius = 0.5f;

    void Start()
    {
        if (ownerPiece == null)
        {
            ownerPiece = GetComponentInParent<BlockSnapper>();
        }
        DeterminePartsToDisconnect();
    }

    public void DeterminePartsToDisconnect()
    {
        if (ownerPiece == null) return;

        partsToDisconnect.Clear();

        foreach (Transform child in ownerPiece.transform)
        {
            if (child.name.ToLower().Contains("slicepoint")) continue;

            Vector3 localPos = transform.InverseTransformPoint(child.position);

            if (localPos.y > 0.1f)
            {
                partsToDisconnect.Add(child);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - transform.right * 2, transform.position + transform.right * 2);
    }
}