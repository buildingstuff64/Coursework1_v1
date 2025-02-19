using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    public GameObject[] barrelEnds;
    public UnityEvent shootEvent;
    public GameObject prefab;
    public float FireRate;
    public float DamageMin;
    public float DamageMax;
    public float Speed;
    public float Spread;
    public float bulletTime;
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

        foreach (GameObject be in barrelEnds)
        {
            GameObject b = Instantiate(prefab, be.transform.position, Quaternion.identity);
            Vector3 f = be.transform.forward * Speed;
            f = Quaternion.Euler(0, Random.Range(-Spread, Spread), 0) * f;
            b.GetComponent<IProjectile>().onFire(this);
            b.GetComponent<Rigidbody>().AddForce(f, ForceMode.VelocityChange);
            Destroy(b, bulletTime);
        }
    }

    public void fireEnemyWeapon()
    {
        currentCooldown += Time.deltaTime;
        if (currentCooldown >= cooldown)
        {
            fireBullet();
        }
    }


}
