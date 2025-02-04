using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public GameObject damagePopupPrefab;
    public GameObject healthbarPrefab;
    public GameObject enemyDeathExplosionPrefab;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameObject g = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup popup = g.GetComponent<DamagePopup>();
        g.transform.position = new Vector3(10000, 0, 10000);
        popup.Setup(Mathf.RoundToInt(0), Color.red);
    }
}
