using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] public Image healthbarImage;

    private void Awake()
    {
        Instance = this;
    }

    public void updateHealthbar(float max, float current)
    {
        healthbarImage.fillAmount = current / max;
        print(current / max);
    }

}
