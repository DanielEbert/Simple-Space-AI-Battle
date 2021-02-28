using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{

    float nextShot = 0;
    public float reloadTime = .3f;
    public float maxSpeed = 3;
    public float rotationSpeed;
    public string[] anims;
    [Range(0,1)]
    public float runOnHitChance;
    public float gunDeviation = 5f;

    Vector2 velocity = Vector2.zero;
    Vector2 currentRandomVec = Vector2.zero;
    Vector3 interceptPoint = Vector3.zero;
    float interceptTime = float.MaxValue;
    public bool inAnimation = false;
    float nextAnimTime = 0;
    float animWaitTime;

    public float health;
    [HideInInspector]
    public float maxHealth;

    Transform target;
    public GameObject bullet;
    GameObject UI;

    public void Start()
    {
        UI = Instantiate(Manager.ShipUIPrefab);
        UI.GetComponent<ShipUI>().followObject = this.transform;

        InvokeRepeating("NewRandomVec", 0, 5);

        maxHealth = health;
        velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(1f, 3f);

        if (tag == "Ally")
        {
            Manager.AllyShips.Add(gameObject);
        }
        else if (tag == "Enemy")
        {
            Manager.EnemyShips.Add(gameObject);
        }

        nextAnimTime = Random.Range(1f, 2f);
        animWaitTime = Random.Range(1f, 2f);
    }

    public void Update()
    {
        target = GetClosestEnemy(); if (target == null) return;

        if (!inAnimation && Time.time > nextAnimTime && anims.Length > 0) StartCoroutine("SpellAnim"); if (inAnimation) return;

        interceptPoint = FirstOrderIntercept(transform.position, Vector3.zero, 8f, target.transform.position, target.GetComponent<Ship>().velocity);
        Rotate(interceptPoint);

        StandardMovement();

        Shoot();
    }

    void StandardMovement()
    {
        velocity += ((Vector2)transform.up + currentRandomVec * .5f) * Time.deltaTime * 1;

        if (velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed;

        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Bullet(Clone)")
        {
            health--;
            Destroy(col.gameObject);

            if(health <= 0)
            {
                Death();
            }

            //RunChance
            float random = Random.Range(0f, 1f);
            if (runOnHitChance > random)
            {
                StartCoroutine("RunAnim", -(col.transform.position - transform.position));
            }
        }
    }

    void Death()
    {
        if(tag == "Ally")
        {
            Manager.AllyShips.Remove(this.gameObject);
        }
        else if(tag == "Enemy")
        {
            Manager.EnemyShips.Remove(this.gameObject);
        }

        Destroy(UI.gameObject);

        Destroy(this.gameObject);
    }

    IEnumerator RunAnim(Vector3 direction)
    {
        //int rnd = Random.Range(0, 2);

        //if (rnd == 0)
        //{
        //    direction = new Vector3(-direction.x, direction.y, 0);
        //}
        //else if (rnd == 1)
        //{
        //    direction = new Vector3(direction.x, -direction.y, 0);
        //}
        //else if (rnd == 2) print("We fucked up");

        velocity += (Vector2)Random.insideUnitCircle.normalized * 3;
        if (velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed;

        yield return null;

        //inAnimation = true;
        //float endTime = Time.time + 1;

        //while (Time.time < endTime)
        //{
        //    Rotate(transform.position + direction);

        //    velocity += (Vector2)transform.up * Time.deltaTime * 3;
        //    if (velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed;

        //    transform.position += (Vector3)velocity * Time.deltaTime;

        //    //Shoot();

        //    yield return null;
        //}

        //inAnimation = false;
    }

    IEnumerator SpellAnim()
    {
        if (!inAnimation)
        {
            inAnimation = true;
            string selectedAnim = anims[Random.Range(0, anims.Length)];
            Transform curTarget = target;

            if (selectedAnim == "LongRangeShot")
            {
                velocity = Vector2.zero;
                float addedRotSpeed = rotationSpeed * 3 - rotationSpeed;
                rotationSpeed += addedRotSpeed;

                float endTime = Time.time + 2;
                while (Time.time <= endTime)
                {
                    if (curTarget == null)
                    {
                        if (target == null) break;  //No more enemys
                        curTarget = target;

                    }

                    interceptPoint = FirstOrderIntercept(transform.position, Vector3.zero, 8f, curTarget.transform.position, curTarget.GetComponent<Ship>().velocity);
                    Rotate(interceptPoint);
                    yield return null;
                }

                //Shoot


                inAnimation = false;
                nextAnimTime = Time.time + animWaitTime;
                rotationSpeed -= addedRotSpeed;
            }   //TODO: Graphics
            else if (selectedAnim == "Teleport")
            {
                float teleSpeed = .3f;

                Vector2 targetPos = (Vector2)transform.position + Random.insideUnitCircle * Random.Range(3f, 5f);
                float counter = 0;
                SpriteRenderer ren = GetComponent<SpriteRenderer>();
                Color col = ren.color;

                while (counter <= 1)
                {
                    if (counter <= 0.5f)
                    {
                        col.a = 1 - counter * 2;
                    }
                    else
                    {
                        transform.position = targetPos;
                        col.a = counter * 2 - 1;
                    }

                    ren.color = col;
                    counter += Time.deltaTime / teleSpeed;
                    yield return null;
                }

                inAnimation = false;
                nextAnimTime = Time.time + animWaitTime;
            }
        }
    }

    void NewRandomVec()
    {
        currentRandomVec = (Vector2)Random.insideUnitCircle;
        currentRandomVec.Normalize();
    }

    Transform GetClosestEnemy()
    {
        float minDist = float.MaxValue;
        float curDist;
        GameObject curTarget = null;

        if (tag == "Ally")
        {
            foreach (GameObject go in Manager.EnemyShips)
            {
                if ((curDist = Vector2.Distance(transform.position, go.transform.position)) < minDist)
                {
                    minDist = curDist;
                    curTarget = go;
                }
            }
        }
        else if (tag == "Enemy")
        {
            foreach (GameObject go in Manager.AllyShips)
            {
                if ((curDist = Vector2.Distance(transform.position, go.transform.position)) < minDist)
                {
                    minDist = curDist;
                    curTarget = go;
                }
            }
        }

        if (curTarget == null) return null;
        return curTarget.transform;
    }

    void Shoot()
    {
        float angle = 0;
        Vector3 relative = transform.InverseTransformPoint(interceptPoint);
        angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;

        if (Time.time > nextShot && Vector2.Distance(transform.position, target.position) < 8 && Mathf.Abs(angle) < 5)
        {
            nextShot = Time.time + reloadTime;

            GameObject bul = Instantiate(bullet, transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + Random.Range(-gunDeviation, gunDeviation)));
            Physics2D.IgnoreCollision(bul.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
            bul.GetComponent<Bullet>().velocity = transform.up;
            Destroy(bul, 1);

            if (tag == "Ally") bul.GetComponent<SpriteRenderer>().color = Color.blue;
            if (tag == "Enemy") bul.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    void Rotate(Vector2 target)
    {
        Vector3 diff = target - (Vector2)transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), rotationSpeed * Time.deltaTime);
    }

    //first-order intercept using absolute target position
    public Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        interceptTime = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + interceptTime * (targetRelativeVelocity);
    }

    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
}
