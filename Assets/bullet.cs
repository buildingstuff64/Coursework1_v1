using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour, IProjectile
{
    private Gradient gradient;
    private float damage;
    public void onFire(Gun gun)
    {
        damage = Random.Range(gun.DamageMin, gun.DamageMax);
        gradient = gun.damagePopupColor;
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
            float realDamage = d.takeDamage(damage);
            GetComponent<SphereCollider>().isTrigger = false;
            print("hit");
            createDamagePopup(other, realDamage, damage);
            Destroy(gameObject);
        }
    }

    private void createDamagePopup(Collider other, float realdamage, float damage)
    {
        GameObject g = Instantiate(PrefabManager.instance.damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup popup = g.GetComponent<DamagePopup>();
        Color c = gradient.Evaluate(0.5f - (damage-realdamage));
        popup.Setup(Mathf.RoundToInt(realdamage), c);
    }

}
