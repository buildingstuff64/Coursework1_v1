using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMover : MonoBehaviour
{
    public float speed;


    [SerializeField] private Camera cameraMain;
    [SerializeField] private Camera cameraUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {



        if (Input.GetKey(KeyCode.Minus))
        {
            cameraMain.orthographicSize -= 5f;
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            cameraMain.orthographicSize += 5f;
        }
        cameraUI.orthographicSize = cameraMain.orthographicSize;

        transform.position = Vector3.Slerp(transform.position, PlayerController.instance.transform.position, Time.deltaTime * speed);

    }
}
