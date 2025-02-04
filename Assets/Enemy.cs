using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Enemy : MonoBehaviour
{
    public float currentHealth = 100;
    public float damageFlashTime = 2;
    public Color flashColor;

    private float currentFlashDuration = 100;
    private List<Material> materials;
    private List<Material> ogMaterials;
    private List<float> h, s, v;

    private HealthBar healthBar;
    private float maxHealth;

    public void takeDamage(float d)
    {
        currentHealth -= d;

        if (currentHealth <= 0) death();

        currentFlashDuration = 0;

        healthBar.updateHealthbar(maxHealth, currentHealth);
        
    }

    private void death()
    {
        print("death");

        GameObject g = Instantiate(PrefabManager.instance.enemyDeathExplosionPrefab);
        g.transform.position = transform.position + Vector3.up * 3;
        g.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject);
        Destroy(g, 15);


    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = currentHealth;
        CreateHealthbar();

        materials = new List<Material>();
        h = new List<float>();
        s = new List<float>();
        v = new List<float>();

        ogMaterials = new List<Material>();
        MeshRenderer[] m = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in m)
        {
            materials.AddRange(mr.materials);
            ogMaterials.AddRange(mr.materials);
        }


        for (int i = 0; i < materials.Count; i++)
        {
            float _h, _s, _v;
            Color.RGBToHSV(materials[i].color, out _h, out _s, out _v);
            h.Add(_h);
            s.Add(_s);
            v.Add(_v);
        }

        foreach (DamageComponent g in transform.GetComponentsInChildren<DamageComponent>()) g.setEnemy(this);

    }

    // Update is called once per frame
    void Update()
    {
        currentFlashDuration += Time.deltaTime;
        if (currentFlashDuration < damageFlashTime)
        {
            float emission = Mathf.PingPong(currentFlashDuration * (2 / damageFlashTime), 1.0f);

            for (int i = 0; i < materials.Count; i++)
            {
                
                Color c = Color.HSVToRGB(h[i], s[i] - emission, v[i] + emission);
                materials[i].color = c;
            }
        }

    }

    private void CreateHealthbar()
    {
        GameObject hbar = Instantiate(PrefabManager.instance.healthbarPrefab, transform);
        healthBar = hbar.GetComponent<HealthBar>();
    }
}
