using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public GameObject damagePopupPrefab;

    private void Awake()
    {
        instance = this;
    }
}
