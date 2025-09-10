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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject)
    //    {
    //        Debug.Log("Plank collided with " + collision.gameObject.name);
    //    }
    //}

    public bool IsBlocking(Hole holeTo)
    {
        BoxCollider2D plankCollider = GetComponent<BoxCollider2D>();
        CircleCollider2D holeToCollider = holeTo.GetComponent<CircleCollider2D>();

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

    public PlankHole GetPlankHoleCoaxialTo(Hole hole)
    {
        foreach (var hingeHole in hingeHoleList)
        {
            var hingeHoleToHoleDistance = Vector2.Distance(hingeHole.hole.transform.position, hole.transform.position);
            if (hingeHoleToHoleDistance < plankHoleCheckThreshold)
                return hingeHole.hole;
        }
        return null;
    }

    public void AttachToBolt(Hole hole)
    {
        PlankHole plankHole = GetPlankHoleCoaxialTo(hole);

        if (plankHole)
        {
            var plankHoleJoint = GetJointForHole(plankHole);
            plankHoleJoint.enabled = true;
            plankHoleJoint.connectedBody = hole.GetBolt().GetComponent<Rigidbody2D>();
            plankHoleJoint.autoConfigureConnectedAnchor = false;
            plankHoleJoint.anchor = plankHole.transform.localPosition;
            plankHoleJoint.connectedAnchor = Vector2.zero;
            plankHole.PlaceBolt(hole.GetBolt());
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
            PlankHole plankHole = hingeHole.hole;

            //Bolt[] allBolts = FindObjectsOfType<Bolt>();
            //foreach (Bolt bolt in allBolts)
            //{
            //    if (Vector2.Distance(bolt.transform.position, plankHole.transform.position) < 0.025f)
            //    {
            //        plankHole.PlaceBolt(bolt);
            //    }
            //}

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
            plankHole.RemoveBolt();
        }
    }

    public void HolesPlankIsMoveable(Hole hole, bool enabled)
    {
        if (enabled == false)
        {
            if (HasAtLeastOneCoaxialHole(hole))
                this.GetComponent<Rigidbody2D>().simulated = enabled;
            return;
        }

        this.GetComponent<Rigidbody2D>().simulated = enabled;
    }
}

[System.Serializable]
public class HingeHolePair
{ 
    public HingeJoint2D hingeJoint;
    public PlankHole hole;
}