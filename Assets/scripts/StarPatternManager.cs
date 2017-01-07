using UnityEngine;
using System.Collections;

public class StarPatternManager : MonoBehaviour {

    public GameObject GameController;

    private GameObject ActiveStars;
    private GameObject InactiveStars;

    public GameObject[] StarsToSpawn;

    public float StarScale;

	// Use this for initialization
	void Start () 
    {
        ActiveStars = GameController.GetComponent<StarManager>().ActiveStars;
        InactiveStars = GameController.GetComponent<StarManager>().InactiveStars;


        EightStarDiamond(1, new int[] {1, 1, 1, 1, 1, 1, 1, 1});
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    void SpawnObject(float customSpeed, Vector3 dir, int type)
    {
        if (InactiveStars.transform.childCount <= 0)
        {
            AddToPool(transform.position, Quaternion.identity, Vector3.one, customSpeed, dir, type);
            GetFromPool(transform.position, Quaternion.identity, Vector3.one, customSpeed, dir, type);
        }
        else
        {
            GetFromPool(transform.position, Quaternion.identity, Vector3.one, customSpeed, dir, type);
        }

        //GetFromPool(transform.position, Quaternion.identity, Vector3.one, customSpeed, dir, type);
    }

    //spawns a new object when there's not any inactive pooled objects to use
    void AddToPool(Vector3 pos, Quaternion rot, Vector3 scale, float customSpeed, Vector3 dir, int type)
    {
        GameObject a = new GameObject();

        if (type == 0)
        {
            a = Instantiate(StarsToSpawn[Random.Range(0, StarsToSpawn.Length)], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 1)
        {
            a = Instantiate(StarsToSpawn[0], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 2)
        {
            a = Instantiate(StarsToSpawn[1], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 3)
        {
            a = Instantiate(StarsToSpawn[2], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 4)
        {
            a = Instantiate(StarsToSpawn[3], pos, rot, InactiveStars.transform) as GameObject;
        }

        a.transform.localScale = new Vector3(StarScale, StarScale, StarScale);
        a.name = "Star";
        a.GetComponent<PooledObject>().MyParent = InactiveStars;

        a.GetComponent<StarController>().usesCustomSpeed = true;
        a.GetComponent<StarController>().customSpeed = customSpeed;
        a.GetComponent<StarController>().customSpeedDirection = dir;
        a.GetComponent<StarController>().RefreshCustomSpeedValues();
    }

    //moves an inactive pooled object to the appropriate place and then makes it set active
    void GetFromPool(Vector3 pos, Quaternion rot, Vector3 scale, float customSpeed, Vector3 dir, int type)
    {
        GameObject thePooledObj = new GameObject();

        //loop through all of the inactive stars and find one that is the type you need
        for (int i = 0; i < InactiveStars.transform.childCount; i++)
        {
            if (type == 0)//random
            {
                //grab any star
                thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
            }
            else if(type == 1)//Sphere Star
            {
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == GameData.StarType.Circle)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
            else if(type == 2)//Square Star
            {
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == GameData.StarType.Square)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
            else if(type == 3)//Star Star
            {
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == GameData.StarType.Star)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
            else if(type == 4)//Triangle Star
            {
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == GameData.StarType.Triangle)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
        }

        //GameObject thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
        thePooledObj.GetComponent<StarController>().starScorePopup.GetComponent<Renderer>().enabled = false;
        thePooledObj.GetComponent<StarController>().starScorePopup.GetComponent<Animator>().enabled = false;
        thePooledObj.transform.position = pos;
        thePooledObj.transform.rotation = rot;
        thePooledObj.transform.localScale = new Vector3(StarScale, StarScale, StarScale);
        thePooledObj.transform.SetParent(ActiveStars.transform);
        thePooledObj.GetComponent<StarController>().StartMovement();

        thePooledObj.GetComponent<StarController>().usesCustomSpeed = true;
        thePooledObj.GetComponent<StarController>().customSpeed = customSpeed;
        thePooledObj.GetComponent<StarController>().customSpeedDirection = dir;
        thePooledObj.GetComponent<StarController>().RefreshCustomSpeedValues();
    }

    /*patterns. 0 = random, 1 = sphere, 2 = square, 3 = star star, 4 = triangle************/
    void EightStarDiamond(float speed, int[] types)
    {
        SpawnUp(speed, types[0]);
        SpawnUpRight(speed, types[1]);
        SpawnRight(speed, types[2]);
        SpawnDownRight(speed, types[3]);
        SpawnDown(speed, types[4]);
        SpawnDownLeft(speed, types[5]);
        SpawnLeft(speed, types[6]);
        SpawnUpLeft(speed, types[7]);
    }

    /*patterns************/

    /*pieces of patterns*********/
    void SpawnUp(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0, 1, 0), type);
    }

    void SpawnUpRight(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0.5f, 0.5f, 0), type);
    }

    void SpawnRight(float speed, int type)
    {
        SpawnObject(speed, new Vector3(1, 0, 0), type);
    }

    void SpawnDownRight(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0.5f, -0.5f, 0), type);
    }

    void SpawnDown(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0, -1, 0), type);
    }

    void SpawnDownLeft(float speed, int type)
    {
        SpawnObject(speed, new Vector3(-0.5f, -0.5f, 0), type);
    }

    void SpawnLeft(float speed, int type)
    {
        SpawnObject(speed, new Vector3(-1, 0, 0), type);
    }

    void SpawnUpLeft(float speed, int type)
    {
        SpawnObject(speed, new Vector3(-0.5f, 0.5f, 0), type);
    }

    /*pieces of patterns*********/
}
