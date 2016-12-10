using UnityEngine;
using System.Collections;

public class MoveObjectTowardsTarget : MonoBehaviour {

    public float delay;

    public float moveSpeed;

    public Vector3 theTarget;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
        else
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, theTarget, step);
        }
	}
}
