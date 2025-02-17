using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Enemy
{
    public class BaseEnemy : MonoBehaviour
    {
        [Header("Health Stuff")]
        public float currentHealth;
        private float maxHealth;
        private HealthBar healthBar;


        [Header("Events Stuff")]
        public UnityEvent deathEvent;
        public UnityEvent hitEvent;

        [Header("AI Stuff")]
        private NavMeshAgent agent;
        public EnemyStates state = EnemyStates.scan;
        public GameObject scanView;

        [Header("Model Stuff")]
        private List<Material> materials = new List<Material>();
        private float currentFlashPercentage = 100000;
        public float damageFlashTime = 0.1f;

        [Header("Turret Stuff")]
        public GameObject Turret;
        public Vector3 TurretTarget;
        public float currentAngle = 0;
        public float turnSpeed = 100;

        private void Start()
        {
            GameObject hbar = Instantiate(PrefabManager.instance.healthbarPrefab, transform);
            healthBar = hbar.GetComponent<HealthBar>();
            maxHealth = currentHealth;

            deathEvent.AddListener(onDeath);
            hitEvent.AddListener(onDamageHit);
            hitEvent.AddListener(updateHealthBar);

            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                materials.AddRange(mr.materials);
            }

            foreach (DamageComponent g in GetComponentsInChildren<DamageComponent>()) { g.setEnemy(this); }
        }

        private void Update()
        {
            checkDamageFlash();
            updateTurretAngle();

            finiteStateMachine();
        }

        public void takeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0 ) { deathEvent.Invoke(); }

            hitEvent.Invoke();
        }

        private void onDeath()
        {
            GameObject g = Instantiate(PrefabManager.instance.enemyDeathExplosionPrefab);
            g.transform.position = transform.position + Vector3.up * 3;
            g.GetComponent<ParticleSystem>().Play();
            Destroy(gameObject);
            Destroy(g, 15);
        }

        private void onDamageHit()
        {
            currentFlashPercentage = 0;
        }

        private void checkDamageFlash()
        {
            currentFlashPercentage += Time.deltaTime;
            
            float lerp = 0;
            if (currentFlashPercentage <= damageFlashTime / 2)
            {
                lerp = Mathf.Lerp(0, 1, (1 / damageFlashTime) * currentFlashPercentage);
            }
            else
            {
                lerp = Mathf.Lerp(1, 0, (1 / damageFlashTime) * currentFlashPercentage);
            }

            foreach (Material m in materials)
            {
                m.SetFloat("_Lerp", lerp);
            }

        }

        private void updateHealthBar()
        {
            healthBar.updateHealthbar(maxHealth, currentHealth);
        }

        public void updateTurretAngle()
        {
            if (TurretTarget == null) return;
            Vector3 dir = TurretTarget - Turret.transform.position;
            dir.y = 0;
            Quaternion targetRoation = Quaternion.LookRotation(dir);
            Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, targetRoation, turnSpeed * Time.deltaTime);
            currentAngle = Vector3.Angle(Turret.transform.forward, dir);

        }

        public void moveTo(Vector3 position)
        {
            agent.destination = position;
        }

        private void createAlertPopup()
        {
            GameObject g = Instantiate(PrefabManager.instance.damagePopupPrefab, transform.position + Vector3.up * 3, Quaternion.identity);
            DamagePopup popup = g.GetComponent<DamagePopup>();
            popup.Setup("!", Color.red);
            popup.textMeshPro.fontSize = 100;

        }

        private void finiteStateMachine()
        {
            
            switch(state)
            {
                case EnemyStates.scan:
                    scan();
                    break;
                case EnemyStates.maneuver:
                    maneuver();
                    break;
                case EnemyStates.engage:
                    maneuver();
                    break;
                default: 
                    break;
            }
            scanView.SetActive(state == EnemyStates.scan ? true : false);
        }

        public void scan()
        {
            turnSpeed = 10;
            if (currentAngle < 1)
            {
                Vector3 lookpoint = Random.insideUnitSphere;
                lookpoint.y = 0;
                TurretTarget = transform.position + lookpoint;
            }

            float playerAngle = Vector3.Angle(Turret.transform.forward, PlayerController.instance.transform.position - transform.position);
            float playerRange = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            if (playerAngle < 25 && playerRange < 25)
            {
                createAlertPopup();
                state = EnemyStates.maneuver;
            }
        }

        public void maneuver()
        {
            turnSpeed = 100;
            TurretTarget = PlayerController.instance.transform.position;
        }

        public void engage()
        {

        }


        
    }

    public enum EnemyStates { scan, maneuver, engage }
}