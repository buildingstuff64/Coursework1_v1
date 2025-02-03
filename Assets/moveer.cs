using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class moveer : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public float turnSpeed;
    public TrailRenderer trailRenderer;
    Plane ground;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ground = new Plane(Vector3.up, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(Vector3.back * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector3.left * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector3.right * speed);
        }

        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed;
        rb.AddForce(moveDirection);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(moveDirection * 50, ForceMode.Impulse);
        }
        if (rb.velocity.magnitude > 15)
        {
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }


        //if (rb.velocity.magnitude > 0.1f)
        //{
        //    Quaternion desiredRotation = Quaternion.LookRotation(rb.velocity);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 10);
        //}

        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit, LayerMask.NameToLayer("CameraGround")))
        //{
        //    Vector3 point = hit.point;
        //}

        //transform.LookAt(finalPoint, Vector3.up);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(ground.Raycast(ray, out float d))
        {
            Vector3 rotation = ray.GetPoint(d) - transform.position;
            Quaternion rotationQuaternion = Quaternion.LookRotation(rotation, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationQuaternion, Time.deltaTime * turnSpeed);
            
        }

    }
}
