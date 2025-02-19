using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] public Image healthbarImage;
    [SerializeField] public TMP_Text scoreNtime;
    public int score;
    public float time;

    private void Awake()
    {
        Instance = this;
    }

    public void updateHealthbar(float max, float current)
    {
        healthbarImage.fillAmount = current / max;
        print(current / max);
    }

    private void Update()
    {
        time += Time.deltaTime;
        scoreNtime.text = string.Format("Score - {0}       Time - {1}", score, Time.time.ToString("F2"));
    }

}
