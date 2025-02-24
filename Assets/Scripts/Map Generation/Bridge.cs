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
            if (to.isEnd)
            {
                to.spawnBoss();
            }

            to.spawnEnemies((int)to.radius/5);
            cameraMover.instance.idealSize = (from.radius + to.radius) / 2;
        }
    }
}
