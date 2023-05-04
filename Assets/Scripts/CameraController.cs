using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 startPos = Vector3.up * 25f;
    public float maxHeight = 75;
    public float minheight = 15;
    public float moveSpeed = 25;
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
        transform.position += velocity * moveSpeed * Time.deltaTime;
        
        //Clamp
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y,minheight,maxHeight);
        transform.position = pos;
    }
}
