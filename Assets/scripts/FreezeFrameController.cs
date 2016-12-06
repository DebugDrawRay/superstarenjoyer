using UnityEngine;
using System.Collections;

public class FreezeFrameController : MonoBehaviour {

    public bool freezeFrameOnStarHitComet;

    public float freezeFrameOnStarHitCometTimerBase;
    public float freezeFrameOnStarHitCometTimer;

    public bool frameFreezed;

    public static float realDeltaTime;
    private float m_PreviousRealTime = 0.0f;

	// Use this for initialization
	void Start () 
    {
        m_PreviousRealTime = Time.realtimeSinceStartup;

        freezeFrameOnStarHitCometTimer = freezeFrameOnStarHitCometTimerBase;

        DoStarHitCometFreezeFrame();
	}
	
	// Update is called once per frame
	void Update () 
    {
        realDeltaTime = Time.realtimeSinceStartup - m_PreviousRealTime;
        m_PreviousRealTime = Time.realtimeSinceStartup;

        if (frameFreezed == true)
        {
            if (freezeFrameOnStarHitCometTimer > 0)
            {
                print("hey there");
                freezeFrameOnStarHitCometTimer -= realDeltaTime;//(Mathf.Abs(Time.realtimeSinceStartup - lastRealTimeSinceStartup)); //Time.unscaledDeltaTime;

            }
            else
            {
                //unfreeze
                Time.timeScale = 1;
                frameFreezed = false;
            }





        }
	}

    void DoStarHitCometFreezeFrame()
    {
        if (freezeFrameOnStarHitComet == true)
        {
            frameFreezed = true;
            Time.timeScale = 0.0f;


        }
    }
}
