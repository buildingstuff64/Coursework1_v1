using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public float dashSpeed;
    public float dashCooldown;
    public float turnSpeed;

    private Rigidbody rb;
    private TrailRenderer trail;
    private Plane ground;

    private float currentDashCooldown;
    public float currentDashCharges = 3;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        ground = new Plane(Vector3.up, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        rb.AddForce(moveDirection * moveSpeed * Time.deltaTime * 1000, ForceMode.Force);

        //Dash
        currentDashCooldown += Time.deltaTime;
        if (currentDashCooldown > dashCooldown)
        {
            currentDashCharges += (currentDashCharges < 3) ? 1 : 0;
            currentDashCooldown = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentDashCharges > 0)
        {
            rb.AddForce(moveDirection * dashSpeed, ForceMode.Impulse);
            currentDashCharges--;
        }
        trail.emitting = (rb.velocity.magnitude > moveSpeed * 5) ? true : false;

        //look
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ground = new Plane(Vector3.up, transform.position);
        if (ground.Raycast(ray, out float d))
        {
            Vector3 rotation = ray.GetPoint(d) - transform.position;
            Quaternion rotationQuaternion = Quaternion.LookRotation(rotation, Vector3.up);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotationQuaternion, Time.deltaTime * turnSpeed);
            transform.rotation = rotationQuaternion;
        }

    }

}
