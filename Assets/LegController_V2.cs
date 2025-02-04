using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController_V2 : MonoBehaviour
{
    static public float stepHeight = 1f;
    static public float stepSpeed = 10;
    static public float stepThreshold = 0.5f;
    static public float bodyMoveFactor = 0.5f;

    private Leg[] legs;
    private int currentLeg = 0;
    private float bodyOffset;

    private void Start()
    {
        legs = GetComponentsInChildren<Leg>();

        float h = 0;
        for (int i = 0; i < legs.Length; i++) { h += legs[i].TargetTip.transform.position.y; }
        h /= legs.Length;
        bodyOffset = transform.parent.position.y - h;

    }

    private void Update()
    {
        legs[currentLeg].setMoving();

        for (int i = 0; i < legs.Length; i++) { legs[i].updateLeg(); } 

        print(currentLeg);
        if (legs[currentLeg].getIsMoving() == false)
        {
            legs[currentLeg].doMove = false;
            rotateLeg();
        }
        float h = 0;
        for (int i = 0; i < legs.Length; i++) { h += legs[i].TargetTip.transform.position.y; }
        h /= legs.Length;
        transform.parent.position = new Vector3(transform.parent.position.x, h + bodyOffset, transform.parent.position.z);

    }

    private void rotateLeg()
    {
        currentLeg++;
        if (currentLeg >= legs.Length) { currentLeg = 0; }
    }


}
