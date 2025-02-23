using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Enemy.Sub_Enemies
{
    public class Bomber : BaseEnemy
    {

        public UnityEvent explosionEvent;

        private float exploding = 0;
        public float explosionTime = 1;
        public float explosionForce = 10000;
        public float explosionDamage = 25;
        public float explosionRadius = 5;

        public override void maneuver()
        {
            base.maneuver();
            moveTo(PlayerController.instance.transform.position);
            if (doEngage()) { state = EnemyStates.engage; explosionEvent.Invoke(); }
        }


        bool doEngage()
        {
            return (Vector3.Distance(agent.destination, transform.position) < 3);
        }

        public override void engage()
        {
            base.engage();
            exploding += Time.deltaTime;
            if (exploding > explosionTime)
            {

                explode();


                deathEvent.Invoke();
            }

            if (Vector3.Distance(agent.destination, transform.position) > 5)
            {
                state = EnemyStates.maneuver;
                exploding = 0;
            }
        }

        private void explode()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider collider in colliders)
            {

                Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
                if (rb != null && rb.gameObject != this.gameObject)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0, ForceMode.Impulse);
                    if (rb.gameObject.GetComponent<Idamageable>() != null)
                    {
                        rb.gameObject.GetComponent<Idamageable>().takeDamage(explosionDamage);
                    }
                }

            }
        }

        public override void Start()
        {
            deathEvent.AddListener(explode);
            base.Start();
        }
    }

}