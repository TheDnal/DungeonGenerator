using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*
        Simple camera controller, works with both orthographic and perspective cameras.
    */
    public Vector3 startPos = Vector3.up * 25f; //Start pos of the camera
    public float maxHeight = 75; //max Y pos
    public float minheight = 15; //min Y pos
    public float moveSpeed = 25; //Move speed 
    public float maxX, minX, maxZ,minZ; // Min/maxes for x and z

    void Start() //reset pos
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

        //Get camera reference
        Camera cam = this.GetComponent<Camera>();
        if(cam.orthographic) //Change ortho size if ortho camera
        {
            cam.orthographicSize += velocity.y * moveSpeed * Time.deltaTime;
        }
        transform.position += velocity * moveSpeed * Time.deltaTime; //apply velocity to camera
        Vector3 pos = transform.position;
        //Clamp position
        pos.y = Mathf.Clamp(pos.y,minheight,maxHeight);
        transform.position = pos;
    }
}
