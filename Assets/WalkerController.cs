using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerController : MonoBehaviour
{
    Rigidbody rb;

    LegController_V1[] legs = new LegController_V1[4];

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
