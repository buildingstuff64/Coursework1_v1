using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    [Header("UI")]
    public GameObject damagePopupPrefab;
    public GameObject healthbarPrefab;
    public GameObject StatPage;

    [Header("Map")]
    public GameObject[] Trees;

    [Header("Enemies")]
    public GameObject enemyDeathExplosionPrefab;
    public GameObject enemyMachineGunner;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameObject g = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup popup = g.GetComponent<DamagePopup>();
        g.transform.position = new Vector3(10000, 0, 10000);
        popup.Setup(Mathf.RoundToInt(0).ToString(), Color.red);
    }

    public void spawnEnemy(Vector3 position)
    {
        GameObject e = Instantiate(enemyMachineGunner, position, Quaternion.identity);
        e.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }


    public void spawnRandomTree(Vector3 position)
    {
        GameObject e = Instantiate(Trees[Random.Range(0, Trees.Length)], position, Quaternion.identity);
        e.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        e.transform.localScale *= Random.Range(0.6f, 1.4f);
    }
}

