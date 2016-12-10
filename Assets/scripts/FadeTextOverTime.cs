using UnityEngine;
using System.Collections;

public class FadeTextOverTime : MonoBehaviour {

    public float delay;

    public float fadeSpeed;

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
            TextMesh tm = GetComponent<TextMesh>();

            Color theColor = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a);

            theColor.a = theColor.a - (fadeSpeed * Time.deltaTime);

            tm.color = theColor;
        }
	}
}
