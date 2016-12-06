using UnityEngine;
using System.Collections;

public class DestroyAfterXSeconds : MonoBehaviour {

    public float timeUntilDeath;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (timeUntilDeath > 0)
        {
            timeUntilDeath -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
	}
}
