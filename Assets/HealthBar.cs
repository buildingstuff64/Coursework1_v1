using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public static float disapperSpeed = 0.5f;
    public static float disapperTime = 5;

    [SerializeField] private Image[] _image;

    private float sinceLastChange = 0;
    private Color[] ogColor = new Color[2];

    private void Start()
    {
        ogColor[0] = _image[0].color;
        ogColor[1] = _image[1].color;
    }

    public void updateHealthbar(float max, float current)
    {
        _image[0].fillAmount = current / max;
        sinceLastChange = 0;
    }

    private void Update()
    {
        sinceLastChange += Time.deltaTime;

        if (sinceLastChange > disapperTime)
        {
            for (int i = 0; i < 2; i++)
            {
                Color c = _image[i].color;
                c.a -= disapperSpeed * Time.deltaTime * ogColor[i].a;
                _image[i].color = c;
            }

        }
        else
        {
            _image[0].color = ogColor[0];
            _image[1].color = ogColor[1];
        }

        transform.LookAt(Camera.main.transform.position, Vector3.up);
    }
}
