using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMover : MonoBehaviour
{
    static public cameraMover instance;

    public float speed;
    public bool doPanIn;
    public float panSpeed;
    public float idealSize;

    [Header("References")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private Camera cameraUI;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (doPanIn)
        {
            idealSize = cameraMain.orthographicSize;
            cameraMain.orthographicSize = 1000;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.instance == null) { return; }

        cameraMain.orthographicSize = Mathf.Lerp(cameraMain.orthographicSize, idealSize, Time.deltaTime * panSpeed);

        cameraUI.orthographicSize = cameraMain.orthographicSize;

        transform.position = Vector3.Slerp(transform.position, PlayerController.instance.transform.position, Time.deltaTime * speed);

    }
}
