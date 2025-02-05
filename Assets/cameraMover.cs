using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMover : MonoBehaviour
{
    public float speed;
    public bool doPanIn;
    public float panSpeed;
    private float initPan;

    [Header("References")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private Camera cameraUI;
    // Start is called before the first frame update
    void Start()
    {
        if (doPanIn)
        {
            initPan = cameraMain.orthographicSize;
            cameraMain.orthographicSize = 1000;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (doPanIn)
        {
            cameraMain.orthographicSize = Mathf.Lerp(cameraMain.orthographicSize, initPan, Time.deltaTime * panSpeed);
        }


        if (Input.GetKey(KeyCode.Minus))
        {
            if (cameraMain.orthographicSize > 5)
            {
                cameraMain.orthographicSize -= 5f;
            }
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            cameraMain.orthographicSize += 5f;
        }
        cameraUI.orthographicSize = cameraMain.orthographicSize;

        transform.position = Vector3.Slerp(transform.position, PlayerController.instance.transform.position, Time.deltaTime * speed);

    }
}
