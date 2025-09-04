using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Plank : MonoBehaviour
{
    [SerializeField] public List<HingeHolePair> hingeHoleList = new List<HingeHolePair>();

    public float plankHoleCheckThreshold;
    private PlankHole coaxialPlankHole = null;
    private float attachThreshold = 0.05f;

    public bool IsBlocking(Hole holeTo)
    {
        BoxCollider2D plankCollider = GetComponent<BoxCollider2D>();
        CircleCollider2D holeToCollider = holeTo.GetComponent<CircleCollider2D>();

        coaxialPlankHole = GetPlankHoleCoaxialTo(holeTo);

        if (HasAtLeastOneCoaxialHole(holeTo))
            return false;

        var holeToPlankDistance = plankCollider.Distance(holeToCollider);

        return holeToPlankDistance.isOverlapped;
    }

    private bool HasAtLeastOneCoaxialHole(Hole holeTo)
    {
        return hingeHoleList.Where(p => Vector2.Distance(p.hole.transform.position, holeTo.transform.position)
                    < plankHoleCheckThreshold).Any();
    }

    private PlankHole GetPlankHoleCoaxialTo(Hole hole)
    {
        return hingeHoleList
            .FirstOrDefault(p =>
                Vector2.Distance(p.hole.transform.position, hole.transform.position) < plankHoleCheckThreshold
                    )?.hole;
    }

    public void AttachToBolt(Hole hole)
    {
        PlankHole plankHole = GetPlankHoleCoaxialTo(hole);

        var plankHoleJoint = GetJointForHole(plankHole);

        if (plankHole)
        {
            plankHoleJoint.enabled = true;
            plankHoleJoint.connectedBody = hole.GetBolt().GetComponent<Rigidbody2D>();
            plankHoleJoint.autoConfigureConnectedAnchor = false;
            plankHoleJoint.anchor = plankHole.transform.localPosition;
            plankHoleJoint.connectedAnchor = Vector2.zero;
        }
    }

    public HingeJoint2D GetJointForHole(PlankHole plankHole)
    {
        var pair = hingeHoleList.FirstOrDefault(p => p.hole == plankHole);
        return pair?.hingeJoint;
    }

    public void AttachAllBolts()
    {
        foreach (var hingeHole in hingeHoleList)
        {
            hingeHole.hingeJoint.enabled = false;
            if (hingeHole.hole.HasBolt())
            {
                hingeHole.hingeJoint.enabled = true;
                hingeHole.hingeJoint.autoConfigureConnectedAnchor = false;
                hingeHole.hingeJoint.connectedBody = hingeHole.hole.GetBolt().GetComponent<Rigidbody2D>();
                hingeHole.hingeJoint.anchor = hingeHole.hole.transform.localPosition;
                hingeHole.hingeJoint.connectedAnchor = Vector2.zero;
            }
        }  
    }

    private IEnumerable<PlankHole> GetPlankHoles(Hole hole)
    {
        foreach (var hingeHole in hingeHoleList)
            if (Vector2.Distance(hingeHole.hole.transform.position, hole.transform.position)
                    < plankHoleCheckThreshold)
                yield return hingeHole.hole;
    }

    public void DetachBolt(Hole hole)
    {
        PlankHole plankHole = GetPlankHoleCoaxialTo(hole);

        var plankHoleJoint = GetJointForHole(plankHole);

        if (plankHole)
        {
            plankHoleJoint.enabled = false;
        }
    }
}

[System.Serializable]
public class HingeHolePair
{ 
    public HingeJoint2D hingeJoint;
    public PlankHole hole;
}