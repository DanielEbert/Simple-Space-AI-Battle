using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Vector2 velocity;
    public float speed = 3;

    public void Update()
    {
        transform.position += (Vector3)velocity.normalized * Time.deltaTime * speed;
    }
}
