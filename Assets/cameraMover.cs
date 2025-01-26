using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMover : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dir = new Vector3();
        if (Input.GetKey(KeyCode.I)) dir += Vector3.forward;
        if (Input.GetKey(KeyCode.K)) dir += Vector3.back;
        if (Input.GetKey(KeyCode.J)) dir += Vector3.left;
        if (Input.GetKey(KeyCode.L)) dir += Vector3.right;
        transform.position += dir * speed * GetComponent<Camera>().orthographicSize;

        if (Input.GetKey(KeyCode.Minus))
        {
            GetComponent<Camera>().orthographicSize -= 5f;
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            GetComponent<Camera>().orthographicSize += 5f;
        }

    }
}
