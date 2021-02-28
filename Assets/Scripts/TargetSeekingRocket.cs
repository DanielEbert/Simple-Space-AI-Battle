using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSeekingRocket : MonoBehaviour {

    public Transform Target;
    public float moveSpeed;
    public float rotationSpeed;
    public float startTime = 1f;

    void Start()
    {
        startTime += Time.time; 
    }

    void Update()
    {
        if (Target == null) return;

        transform.position += transform.up * Time.deltaTime * moveSpeed;

        if(startTime < Time.time)
        {
            Rotate(Target.position);
        }
    }

    void Rotate(Vector2 target)
    {
        Vector3 diff = target - (Vector2)transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), rotationSpeed * Time.deltaTime);
    }
}
