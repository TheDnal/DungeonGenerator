using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 startPos = Vector3.up * 25f;
    public float maxHeight = 75;
    public float minheight = 15;
    public float moveSpeed = 25;
    public float maxX, minX, maxZ,minZ;

    void Start()
    {
        //Start pos
        transform.position = startPos;
    }
    void Update()
    {
        //Get velocity inputs
        Vector3 velocity = Vector3.zero;
        if(Input.GetKey(KeyCode.Q) && transform.position.y <= maxHeight)
        {
            velocity += Vector3.up;
        }
        else if(Input.GetKey(KeyCode.E) && transform.position.y >= minheight)
        {
            velocity += Vector3.down;
        }
        //Apply velocity to transform
        
        if(Input.GetKey(KeyCode.W) && transform.position.z <= maxZ)
        {
            velocity += Vector3.forward;
        }   
        else if(Input.GetKey(KeyCode.S) && transform.position.z >= minZ)
        {
            velocity += Vector3.back;
        }

        if(Input.GetKey(KeyCode.D) && transform.position.x <= maxX)
        {
            velocity += Vector3.right;
        }
        else if(Input.GetKey(KeyCode.A) && transform.position.x >= minX)
        {
            velocity += Vector3.left;
        }
        //Clamp
        Camera cam = this.GetComponent<Camera>();
        if(cam.orthographic)
        {
            cam.orthographicSize += velocity.y * moveSpeed * Time.deltaTime;
        }
        transform.position += velocity * moveSpeed * Time.deltaTime;
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y,minheight,maxHeight);
        transform.position = pos;
    }
}
