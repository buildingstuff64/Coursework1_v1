using Assets.Scripts.Game_Management;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, Idamageable
{
    static public PlayerController instance;

    public float moveSpeed;
    public float dashSpeed;
    public float dashCooldown;
    public float turnSpeed;

    private Rigidbody rb;
    private TrailRenderer trail;
    private Plane ground;

    private float currentDashCooldown;
    public float currentDashCharges = 3;

    public float Health;
    private float maxHealth;
    private float timeSinceLastDamaged = 0;
    public float regenTime;
    public float regenRate;
    private float healthrate = 0;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = Health;
        instance = this;
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        ground = new Plane(Vector3.up, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        rb.AddForce(moveDirection * moveSpeed * Time.deltaTime * 1000, ForceMode.Acceleration);

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

        timeSinceLastDamaged += Time.deltaTime;
        if (timeSinceLastDamaged > regenTime && Health < maxHealth)
        {
            healthrate += Time.deltaTime;
            if (healthrate > 1/regenRate) 
            {
                if (Health + regenRate > maxHealth) { Health = maxHealth; }
                else
                {
                    Health += 1;
                    createHealthPopup(1);
                }
                healthrate = 0;
            }
            UIManager.Instance.updateHealthbar(maxHealth, Health);
        }
        if (Health <= 0) { onDeath(); }

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 100);
        if (!isGrounded){
            print("rawww");
            rb.AddForce(Vector3.down * 250f * Time.deltaTime, ForceMode.Impulse);
        }

        if (moveDirection != Vector3.zero || !isGrounded)
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
        }
        
        if (transform.position.y < -10)
        {
            onDeath();
        }

    }

    public void onDeath()
    {
        GameManager.instance.onDeath();
        Destroy(gameObject);

    }

    private void createHealthPopup(float h)
    {
        GameObject g = Instantiate(PrefabManager.instance.damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup popup = g.GetComponent<DamagePopup>();
        Color c = Color.green;
        popup.Setup(Mathf.RoundToInt(h).ToString(), c);
    }

    public float takeDamage(float damage)
    {
        Health -= damage;
        timeSinceLastDamaged = 0;
        UIManager.Instance.updateHealthbar(maxHealth, Health);
        UIManager.Instance.currentDamageFlash = 0;
        return damage;
    }
}
