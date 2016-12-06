using UnityEngine;
using System.Collections;

public class ConstellationMainMenuMove : MonoBehaviour {

    public float minSpeed;
    public float maxSpeed;

    public float speed;

	// Use this for initialization
	void Start () 
    {
        speed = Random.Range(minSpeed, maxSpeed);

        GetComponent<Rigidbody>().velocity = new Vector3(0, 1, 0) * speed;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}
}
