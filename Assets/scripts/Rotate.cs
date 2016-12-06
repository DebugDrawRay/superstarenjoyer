using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public float xRotateSpeed;
    public float yRotateSpeed;
    public float zRotateSpeed;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {   
        //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x + (xRotateSpeed * Time.deltaTime), transform.rotation.y + (yRotateSpeed * Time.deltaTime), transform.rotation.z + (zRotateSpeed * Time.deltaTime)));
        if (xRotateSpeed != 0 && yRotateSpeed != 0 && zRotateSpeed != 0)
        {
            transform.Rotate(new Vector3(1, 1, 1) * (Time.deltaTime * xRotateSpeed));
        }
        else if (xRotateSpeed != 0 && yRotateSpeed == 0 && zRotateSpeed == 0)
        {
            transform.Rotate(new Vector3(1, 0, 0) * (Time.deltaTime * xRotateSpeed));
        }
    }
}
