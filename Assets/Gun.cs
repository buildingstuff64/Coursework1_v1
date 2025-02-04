using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    public GameObject barrelEnd;
    public UnityEvent shootEvent;
    public GameObject prefab;
    public float FireRate;
    public float DamageMin;
    public float DamageMax;
    public float Speed;
    public bool isAutomatic;
    public Gradient damagePopupColor;

    private float currentCooldown;
    private float cooldown;
    // Start is called before the first frame update
    void Start()
    {
        cooldown = 1 / FireRate;
    }

    // Update is called once per frame
    void Update()
    {
        currentCooldown += Time.deltaTime;
        if (isAutomatic)
        {
            if (Input.GetKey(KeyCode.Mouse0) && currentCooldown >= cooldown)
            {
                shootEvent.Invoke();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentCooldown >= cooldown)
            {
                shootEvent.Invoke();
            }
        }
    }

    public void fireBullet()
    {
        currentCooldown = 0;

        GameObject b = Instantiate(prefab, barrelEnd.transform.position, Quaternion.identity);
        b.GetComponent<IProjectile>().onFire(this);
        b.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        Destroy(b, 2f);
        //rb.AddForce(-transform.forward, ForceMode.Impulse);
    }


}
