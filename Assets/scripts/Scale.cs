using UnityEngine;
using System.Collections;

public class Scale : MonoBehaviour {

    public Vector3 scaleSpeed;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.localScale = new Vector3(transform.localScale.x + (scaleSpeed.x * Time.deltaTime), transform.localScale.y + (scaleSpeed.y * Time.deltaTime), transform.localScale.z + (scaleSpeed.z * Time.deltaTime));
	}
}
