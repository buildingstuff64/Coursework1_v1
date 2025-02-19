using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Enemy.Sub_Enemies
{
    public class MachineGunner : BaseEnemy
    {
        public Gun gun;

        public UnityEvent overheatingEffect;
        private float overheating = 0;
        public float timeTillOverheat = 2;

        public UnityEvent cooldownEffect;
        private float currentCooldown = 0;
        public float cooldownTime = 1;

       public override void maneuver()
       {
            base.maneuver();
            Vector3 v = -playerDirection.normalized * manuverRange;
            moveTo(PlayerController.instance.transform.position + v);
            Debug.DrawLine(PlayerController.instance.transform.position + v, transform.position, Color.red, 0.1f);
            if (doEngage()) { state = EnemyStates.engage; overheatingEffect.Invoke(); }
       }

        public override void finiteStateMachine()
        {
            base.finiteStateMachine();
            switch (state)
            {
                case EnemyStates.cooldown:
                    cooldown();
                    break;
                default:
                    break;

            }
        }

        public override void engage()
        {
            base.engage();
            TurretTarget = PlayerController.instance.transform.position;
            fireWeapons();

        }

        private bool doEngage()
        {
            return Vector3.Distance(agent.destination, transform.position) <= 0.1 && playerAngle < 5 ? true : false;     
        }

        private void fireWeapons()
        {
            overheating += Time.deltaTime;
            gun.fireEnemyWeapon();
            if (overheating > timeTillOverheat)
            {
                state = EnemyStates.cooldown;
                cooldownEffect.Invoke();
                overheating = 0;
            }
        }

        private void cooldown()
        {
            currentCooldown += Time.deltaTime;
            if (currentCooldown > cooldownTime)
            {
                state = EnemyStates.maneuver;
                currentCooldown = 0;
            }
        }
    }
}