using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour {

    public float xScrollSpeed;

    public float yScrollSpeed;

    Renderer rend;

	// Use this for initialization
	void Start () 
    {
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        

        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(rend.material.mainTextureOffset.x + (xScrollSpeed * Time.deltaTime), rend.material.mainTextureOffset.y + (yScrollSpeed * Time.deltaTime));
	}
}
