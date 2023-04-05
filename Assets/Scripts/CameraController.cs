using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float maxHeight, minHeight;
    public float speed = 10;
    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;
        if(Input.GetKey(KeyCode.Q))
        {
            velocity += Vector3.down;
        }
        else if(Input.GetKey(KeyCode.E))
        {
            velocity += Vector3.up;
        }
        velocity.Normalize();
        velocity *= speed;
        Vector3 pos = transform.position;
        pos += velocity * Time.deltaTime;
        if(pos.y > maxHeight)
        {
            pos.y = maxHeight;
        }
        else if(pos.y < minHeight)
        {
            pos.y = minHeight;
        }
        transform.position = pos;
    }
}
