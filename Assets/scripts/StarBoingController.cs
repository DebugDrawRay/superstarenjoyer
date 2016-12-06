using UnityEngine;
using System.Collections;

public class StarBoingController : MonoBehaviour {

    public float timeBeforeInvisBase;
    public float timeBeforeInvis;

    public bool startedGrowing;

    public Vector3 startScale;

    public Vector3 scaleSpeed;

	// Use this for initialization
	void Start () 
    {
        timeBeforeInvis = timeBeforeInvisBase;
        startScale = transform.localScale;
	}
	
	// Update is called once per frame
    void Update () 
    {
        if (timeBeforeInvis > 0)
        {
            timeBeforeInvis -= Time.deltaTime;
            transform.localScale = new Vector3(transform.localScale.x + (scaleSpeed.x * Time.deltaTime), transform.localScale.y + (scaleSpeed.y * Time.deltaTime), transform.localScale.z + (scaleSpeed.z * Time.deltaTime));
        }
        else
        {
            if (startedGrowing == true)
            {
                StopGrowing();
            }
        }
	}

    void OnEnable()
    {
        timeBeforeInvis = timeBeforeInvisBase;
        transform.localScale = startScale;
        startedGrowing = true;
    }

    void OnDisable()
    {
        //startedGrowing = false;
    }

    public void StopGrowing()
    {
        gameObject.SetActive(false);
        startedGrowing = false;
    }
}
