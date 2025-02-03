using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour, IProjectile
{
    private float damage;
    public void onFire(Gun gun)
    {
        damage = Random.Range(gun.DamageMin, gun.DamageMax);
    }

    public void onHit()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        Idamageable d = other.gameObject.GetComponent<Idamageable>();
        if (other != null && d != null)
        {
            d.takeDamage(damage);
            GetComponent<SphereCollider>().isTrigger = false;
            print("hit");
            createDamagePopup(other, damage);
            Destroy(gameObject);
        }
    }

    private void createDamagePopup(Collider other, float damage)
    {
        GameObject g = Instantiate(PrefabManager.instance.damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup popup = g.GetComponent<DamagePopup>();
        popup.Setup(Mathf.RoundToInt(damage));
    }

}
