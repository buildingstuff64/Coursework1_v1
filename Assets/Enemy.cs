using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;
    public float damageFlashTime = 2;

    private float currentFlashDuration = 100;
    private List<Material> materials;
    private List<float> h, s, v;

    public void takeDamage(float d)
    {
        health -= d;

        if (health <= 0) death();

        currentFlashDuration = 0;
    }

    private void death()
    {
        print("death");
    }

    // Start is called before the first frame update
    void Start()
    {
        materials = new List<Material>();
        h = new List<float>();
        s = new List<float>();
        v = new List<float>();

        List<Material> ogMaterials = new List<Material>();
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
                Color c = Color.HSVToRGB(h[i], s[i] - emission, v[i]);
                materials[i].color = c;
            }
        }


    }
}
