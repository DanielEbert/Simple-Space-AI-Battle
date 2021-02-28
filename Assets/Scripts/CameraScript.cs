using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    Vector3 curTargetPos;

    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        CalcTargetPos();
        transform.position = curTargetPos;
    }

    void CalcTargetPos()
    {
        if (Manager.AllyShips.Count == 0 && Manager.EnemyShips.Count == 0) return;

        Vector2 pos = Vector2.zero;
        int count = 0;

        foreach (GameObject go in Manager.AllyShips)
        {
            pos += (Vector2)go.transform.position;
            count++;
        }

        foreach (GameObject go in Manager.EnemyShips)
        {
            pos += (Vector2)go.transform.position;
            count++;
        }

        pos /= count;

        curTargetPos = new Vector3(pos.x, pos.y, -10f);
    }

    public void Update()
    {
        CalcTargetPos();
        transform.position = Vector3.SmoothDamp(transform.position, curTargetPos, ref velocity, smoothTime);
    }
}
