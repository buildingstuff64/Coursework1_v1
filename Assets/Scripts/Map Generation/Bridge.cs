using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Island from, to;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            to.spawnEnemies(Random.Range(0, 5));
            cameraMover.instance.idealSize = (from.radius + to.radius) / 2;
        }
    }
}
