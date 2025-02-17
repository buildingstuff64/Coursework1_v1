using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    private float disappearTime = .5f;
    private float xMove;
    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        xMove = Random.Range(-1, 1);
    }
    public void Setup(string damageAmount, Color c)
    {
        textMeshPro.SetText(damageAmount);
        textMeshPro.color = c;
    }

    private void Update()
    {
        float movespeed = 2;
        transform.position += new Vector3(xMove, movespeed, 0) * Time.deltaTime;
        disappearTime -= Time.deltaTime;
        if (disappearTime < 0)
        {
            Color c = textMeshPro.color;
            c.a -= 2 * Time.deltaTime;
            textMeshPro.color = c;
            if (c.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
