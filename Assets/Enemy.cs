using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Idamageable
{
    public float health = 100;
    public float damageFlashTime = 2;

    private Material ogmat;
    private float currentFlashDuration = 100;
    private float h, s, v;

    public void takeDamage(float d)
    {
        health -= d;

        if (health <= 0)
        {
            health = 100;
        }

        currentFlashDuration = 0;
    }

    private IEnumerator damageFlash()
    {
        Material mat = GetComponent<MeshRenderer>().material;
        float h, s, v;
        Color og = mat.color;
        Color.RGBToHSV(og, out h, out s, out v);
        float duration = 0f;

        while(duration < damageFlashTime)
        {
            duration += Time.deltaTime;
            float emission = Mathf.PingPong(duration * (2 / damageFlashTime), 1.0f);
            Color c = Color.HSVToRGB(h, s - emission, v);
            mat.color = c;
            print(emission);
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    // Start is called before the first frame update
    void Start()
    {
        ogmat = GetComponent<MeshRenderer>().material;
        Color.RGBToHSV(ogmat.color, out h, out s, out v);

    }

    // Update is called once per frame
    void Update()
    {
        currentFlashDuration += Time.deltaTime;
        if (currentFlashDuration < damageFlashTime)
        {
            float emission = Mathf.PingPong(currentFlashDuration * (2 / damageFlashTime), 1.0f);
            Color c = Color.HSVToRGB(h, s - emission, v);
            ogmat.color = c;
            print(emission);
        }


    }
}
