using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public GameObject TargetTip;
    public Vector3 CurrentPosition;
    private Vector3 OldPosition;
    private GameObject BodyOffset;
    private LegController LegController;

    private float legLerp = 1;
    public bool doMove = true;

    private void Awake()
    {
        setup();
    }


    public void setContoller(LegController lc)
    {
        LegController = lc;
    }
    private void setup()
    {
        TargetTip = getTargetGameobject();
        BodyOffset = new GameObject();
        BodyOffset.transform.parent = transform.parent.parent;
        BodyOffset.transform.position = TargetTip.transform.position;
        BodyOffset.name = string.Format("{0} BodyOffset", name);
        CurrentPosition = TargetTip.transform.position;
        OldPosition = CurrentPosition;
    }

    public void updateLeg()
    {
        updateTargetPosition();

        CheckDistance();

        checkIfMove();
    }

    private void checkIfMove()
    {
        if (legLerp < 1)
        {
            Vector3 footpos = Vector3.Lerp(OldPosition, CurrentPosition, legLerp);
            footpos.y += Mathf.Sin(legLerp * Mathf.PI) * LegController.stepHeight;

            TargetTip.transform.position = footpos;
            legLerp += Time.deltaTime * LegController.stepSpeed * Vector3.Distance(OldPosition, CurrentPosition);
        }
        else
        {
            OldPosition = CurrentPosition;
        }

    }

    public void setMoving()
    {
        if (!doMove) doMove = true;
    }

    public void updateTargetPosition()
    {
        TargetTip.transform.position = CurrentPosition;
    }

    public void CheckDistance()
    {
        Ray ray = new Ray(BodyOffset.transform.position + Vector3.up * 3, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, LayerMask.GetMask("Ground")))
        {
            if((Vector3.Distance(hit.point, CurrentPosition) > LegController.stepThreshold) && doMove == true)
            {
                legLerp = 0;
                CurrentPosition = hit.point;
            }
        }
    }

    public bool getIsMoving() { return (legLerp > 1) ? false : true; }

    private GameObject getTargetGameobject()
    {
        Transform[] gs = transform.GetComponentsInChildren<Transform>();
        foreach (Transform go in gs) { if (go.name == "Rig 1_target") return go.gameObject; }
        return null;
    }

    private void OnDrawGizmos()
    {
        if (BodyOffset != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(BodyOffset.transform.position, 0.25f);
        }

        if (CurrentPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(CurrentPosition, 0.25f);
        }
    }

}
