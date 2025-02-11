using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float currentHealth = 100;
    public float damageFlashTime = 2;
    public Color flashColor;

    public enum EnemyStates { scan, move, lockon, attack};
    public EnemyStates state;
    public float scanDistance = 40;
    public float lockonDistance = 50;
    public float engageRange = 20;

    private float currentFlashDuration = 100;
    private List<Material> materials;
    private List<Material> ogMaterials;
    private List<float> h, s, v;

    private HealthBar healthBar;
    private float maxHealth;

    private NavMeshAgent agent;

    public GameObject Turret;
    public float turretTurnSpeed;
    public float shootturnSpeed;
    private Gun gun;
    private float gunHeat = 0;

    public void takeDamage(float d)
    {
        currentHealth -= d;

        if (currentHealth <= 0) death();

        currentFlashDuration = 0;

        healthBar.updateHealthbar(maxHealth, currentHealth);
        
    }

    private void death()
    {
        print("death");

        GameObject g = Instantiate(PrefabManager.instance.enemyDeathExplosionPrefab);
        g.transform.position = transform.position + Vector3.up * 3;
        g.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject);
        Destroy(g, 15);

    }

    // Start is called before the first frame update
    void Start()
    {
        gun = GetComponent<Gun>();
        agent = GetComponent<NavMeshAgent>();
        maxHealth = currentHealth;
        CreateHealthbar();

        materials = new List<Material>();
        h = new List<float>();
        s = new List<float>();
        v = new List<float>();

        ogMaterials = new List<Material>();
        MeshRenderer[] m = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in m)
        {
            materials.AddRange(mr.materials);
            ogMaterials.AddRange(mr.materials);
        }

        for (int i = 0; i < materials.Count; i++)
        {
            float _h, _s, _v;
            Color.RGBToHSV(materials[i].color, out _h, out _s, out _v);
            h.Add(_h);
            s.Add(_s);
            v.Add(_v);
        }

        foreach (DamageComponent g in transform.GetComponentsInChildren<DamageComponent>()) g.setEnemy(this);

    }

    // Update is called once per frame
    void Update()
    {
        checkDamageFlash();

        switch (state)
        {
            case EnemyStates.scan:
                bool b = scanForPlayer(scanDistance);
                if (b) { createPopup(); }
                state = b ? EnemyStates.move : EnemyStates.scan;
                break;
            case EnemyStates.move:
                moveToRange(engageRange);
                state = EnemyStates.lockon;
                break;
            case EnemyStates.lockon:
                state = lockon() ? EnemyStates.attack : EnemyStates.lockon;
                break;
            case EnemyStates.attack:
                attack();
                break;
        }

    }

    private bool scanForPlayer(float inrange)
    {
        return Vector3.Distance(transform.position, PlayerController.instance.transform.position) < inrange ? true : false;
    }

    private void moveToRange(float range)
    {
        Vector3 direction = transform.position - PlayerController.instance.transform.position;
        direction.Normalize();
        direction *= range;
        agent.destination = PlayerController.instance.transform.position + direction;
    }

    private bool lockon()
    {
        Vector3 dir = PlayerController.instance.transform.position - Turret.transform.position;
        Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turretTurnSpeed);
        float angle = Vector3.Angle(Turret.transform.forward, dir);

        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < lockonDistance && angle < gun.Spread)
        {
            return true;
        }
        else return false;
    }

    private void attack()
    {
        Vector3 dir = (PlayerController.instance.transform.position - Turret.transform.position).normalized;
        Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * shootturnSpeed);
        float angle = Vector3.Angle(Turret.transform.forward, dir);
        gun.fireEnemyWeapon();
        gunHeat += Time.deltaTime;
        if (gunHeat > 3 || angle > 90 || Vector3.Distance(transform.position, PlayerController.instance.transform.position) > engageRange)
        {
            state = EnemyStates.scan;
            gunHeat = 0;
        }
    }

    private void checkDamageFlash()
    {
        currentFlashDuration += Time.deltaTime;
        if (currentFlashDuration < damageFlashTime)
        {
            float emission = Mathf.PingPong(currentFlashDuration * (2 / damageFlashTime), 1.0f);

            for (int i = 0; i < materials.Count; i++)
            {

                Color c = Color.HSVToRGB(h[i], s[i] - emission, v[i] + emission);
                materials[i].color = c;
            }
        }
    }

    private void CreateHealthbar()
    {
        GameObject hbar = Instantiate(PrefabManager.instance.healthbarPrefab, transform);
        healthBar = hbar.GetComponent<HealthBar>();
    }

    private void createPopup()
    {
        GameObject g = Instantiate(PrefabManager.instance.damagePopupPrefab, transform.position+Vector3.up*3, Quaternion.identity);
        DamagePopup popup = g.GetComponent<DamagePopup>();
        popup.Setup("!", Color.red);
        popup.GetComponent<TextMeshPro>().fontSize = 50;
    }
}
