using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyWalking : MonoBehaviour
{
    Vector3 currFR;
    Vector3 currFL;
    Vector3 currBR;
    Vector3 currBL;



    public GameObject FRtarget;
    public GameObject BRtarget;
    public GameObject FLtarget;
    public GameObject BLtarget;

    public GameObject bdFR;
    public GameObject bdBR;
    public GameObject bdFL;
    public GameObject bdBL;

    [Header("Variables")]
    public float moveForce = 5;
    public float distanceThreshold = 1.5f;
    public float legMoveSpeed = 5f;
    public float bodyOffset = 1;

    Leg lgFR;
    Leg lgFL;
    Leg lgBR;
    Leg lgBL;

    Rigidbody rb;


    public GameObject[] gunEnds;
    public GameObject bullet;
    private float bullettime = 0;


    private void Start()
    {
        lgFR = new Leg(bdFR, FRtarget, new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), distanceThreshold, 1);
        lgFL = new Leg(bdFL, FLtarget, new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), distanceThreshold, 1);
        lgBR = new Leg(bdBR, BRtarget, new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), distanceThreshold, 1);
        lgBL = new Leg(bdBL, BLtarget, new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), distanceThreshold, 1);

        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        float height = 0;
        lgFR.updateLeg(legMoveSpeed, rb.velocity);
        lgFL.updateLeg(legMoveSpeed, rb.velocity);
        lgBR.updateLeg(legMoveSpeed, rb.velocity);
        lgBL.updateLeg(legMoveSpeed, rb.velocity);

        height += lgFR.Target.transform.position.y;
        height += lgFL.Target.transform.position.y;
        height += lgBR.Target.transform.position.y;
        height += lgBL.Target.transform.position.y;
        height /= 8;
        //print(height);

        transform.position = new Vector3(transform.position.x, height + bodyOffset, transform.position.z);

        //Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //GetComponent<Rigidbody>().AddForce(transform.forward * 2);

        if (Input.GetKey(KeyCode.W)) GetComponent<Rigidbody>().AddForce(transform.forward * moveForce);
        if (Input.GetKey(KeyCode.S)) GetComponent<Rigidbody>().AddForce(-transform.forward * moveForce);
        if (Input.GetKey(KeyCode.A)) GetComponent<Rigidbody>().AddForce(-transform.right * moveForce);
        if (Input.GetKey(KeyCode.D)) GetComponent<Rigidbody>().AddForce(transform.right * moveForce);



        if (Input.GetKey(KeyCode.Q))
        {
            GetComponent<Rigidbody>().AddRelativeTorque(Vector3.down);
        }

        if (Input.GetKey(KeyCode.E))
        {
            GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up);
        }




        //if (GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        //{
        //    lgFR.lerpTodoy(legMoveSpeed);
        //    lgFL.lerpTodoy(legMoveSpeed);
        //    lgBR.lerpTodoy(legMoveSpeed);
        //    lgBL.lerpTodoy(legMoveSpeed);
        //}
        //else
        //{
        //    lgFR.lerp2 = 0;
        //    lgFL.lerp2 = 0;
        //    lgBR.lerp2 = 0;
        //    lgBL.lerp2 = 0;
        //}

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Debug.Log("mouse down");
            for (int i = 0; i < gunEnds.Length; i++)
            {
                ParticleSystem pt = gunEnds[i].GetComponent<ParticleSystem>();
                if (pt.isStopped)
                {
                    pt.Play();
                }

                if (bullettime > 0.1f)
                {
                    GameObject b = Instantiate(bullet, pt.transform.position, Quaternion.identity);
                    b.GetComponent<Rigidbody>().AddForce(transform.forward * Random.Range(2.35f, 2.7f), ForceMode.Impulse);
                    Destroy(b, 2f);
                    bullettime = 0;
                    rb.AddForce(-transform.forward, ForceMode.Impulse);
                }
                bullettime += Time.deltaTime;

            }
        }
        else
        {
            for (int i = 0; i < gunEnds.Length; i++)
            {
                ParticleSystem pt = gunEnds[i].GetComponent<ParticleSystem>();
                if (pt.isPlaying)
                {
                    pt.Stop();
                }
                
            }
        }
    }

    Vector3 updateLeg(Vector3 curr, GameObject target, GameObject body)
    {
        if (Vector3.Distance(target.transform.position, body.transform.position) > distanceThreshold)
        {
            //Debug.Log(Vector3.Distance(target.transform.position, body.transform.position));
            return body.transform.position;
        }
        return curr;
    }

    class Leg
    {
        public Vector3 Curr;
        private Vector3 Old;
        public GameObject Body;
        public GameObject Target;

        private float lerp;
        public float lerp2;
        private float threshold;
        private float stepheight;

        public Leg(GameObject Body, GameObject Target, Vector3 init, float threshold, float stepheight)
        {
            this.Curr = Target.transform.position + init;
            this.Body = Body;
            this.Target = Target;
            this.threshold = threshold;
            this.Old = Curr;
            this.stepheight = stepheight;
        }

        public void updateLeg(float speed, Vector3 vel)
        {
            Target.transform.position = this.Curr;

            Ray ray = new Ray(Body.transform.position + Vector3.up*3, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                if (Vector3.Distance(Curr, hit.point) > threshold)
                {
                    if (Random.Range(0f, 1f) < 0.1f)
                    {
                        lerp = 0;
                        Curr = hit.point;
                        Curr -= hit.point - Curr;
                    }

                }
            }
            if (lerp < 1)
            {
                Vector3 footpos = Vector3.Lerp(Old, Curr, lerp);
                footpos.y += Mathf.Sin(lerp * Mathf.PI) * stepheight;

                Target.transform.position = footpos;
                lerp += Time.deltaTime * speed;
            }
            else
            {
                Old = Curr;
            }
        }

        public void lerpTodoy(float speed)
        {
            if (lerp2 < 1 && lerp > 1)
            {
                Vector3 footpos = Vector3.Lerp(Old, Curr, lerp2);
                footpos.y += Mathf.Sin(lerp2 * Mathf.PI) * stepheight;
                Target.transform.position = footpos;
                lerp2 += Time.deltaTime * speed;
            }

        }

    }
}
