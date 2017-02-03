using UnityEngine;
using System.Collections;
using UnityEditor;

public class StarPatternManager : MonoBehaviour {

    public GameObject GameController;

    private GameObject ActiveStars;
    private GameObject InactiveStars;

    public GameObject[] StarTypesAvailable;

    public float StarScale;

    public enum PatternType
    {
        EightStarDiamond,
        Spiral01,
        Wall01
    }
    public PatternType currentPatternType;
    public bool randomPatternChooseOnStart;//chooses a random pattern on start and sticks with it
    public bool randomPatternChooseEveryTime;//chooses a random pattern every time it needs to spawn a pattern
    public float starMoveSpeed;
    public int[] starTypesToSpawn;
    public float spiralRotateAmt;//the amount of rotation between star spawns in the spiral pattern
    public float spiralSpawnTime;//the amount of time between star spawns in the spiral pattern
    public float wallSpacing;//the amount of space between stars in the wall pattern
    public bool loopPattern;//if this is true, it will repeat the pattern until it is destroyed
    public float timeBeforeStartingPatternAgainBase;
    public float timeBeforeStartingPatternAgain;

    //this is the pattern emitter's move speed
    private float moveSpeed;

    public bool active;

    //public float destroyStarWhenBelowThisYValue;

    //var dropdown = PatternType.EightStarDiamond;

    /*public class PoopDoodoo : EditorWindow
    {
        public string[] options = new string[]{"Eight star diamond", "Spiral01"};
        public int index = 0;
        [MenuItem("Examples/ Editor GUILayout Popup usage")]
        static void Init()
        {
            EditorWindow window = GetWindow(typeof(PoopDoodoo));
            window.Show();
        }
        void OnGUI()
        {
            index = EditorGUILayout.Popup(index, options);
            if (GUILayout.Button("Create"))
            {
                
            }
        }
    }*/

    /*int selected = 0;
    string[] options = new string[]{"Eight star diamond", "Spiral01"};
    selected = EditorGUILayout.Popup("Label", selected, options);*/


	// Use this for initialization
	void Start () 
    {
        GameController = GameObject.Find("GameController");
        starMoveSpeed = Random.Range(GameData.minStarSpeed, GameData.maxStarSpeed);
        moveSpeed = Random.Range(GameData.minStarSpeed, GameData.maxStarSpeed);
        if (randomPatternChooseOnStart == true)
        {
            int choice = Random.Range(0, 3);

            if (choice == 0)
            {
                currentPatternType = PatternType.EightStarDiamond;
            }
            else if (choice == 1)
            {
                currentPatternType = PatternType.Spiral01;
            }
            else if (choice == 2)
            {
                currentPatternType = PatternType.Wall01;
            }
        }
        StartMovement(moveSpeed);

        //ActiveStars = GameController.GetComponent<StarManager>().ActiveStars;
        //InactiveStars = GameController.GetComponent<StarManager>().InactiveStars;
	}
	
	// Update is called once per frame
	void Update () 
    {
        /*if (transform.position.y < destroyStarWhenBelowThisYValue)
        {
            Destroy(gameObject);
        }*/

        if (loopPattern == true)
        {
            if (active == true)
            {
                if (timeBeforeStartingPatternAgain > 0)
                {
                    timeBeforeStartingPatternAgain -= Time.deltaTime;
                }
                else
                {
                    DetermineWhichPatternToUse();
                    timeBeforeStartingPatternAgain = timeBeforeStartingPatternAgainBase;
                }
            }
        }
        else
        {
            if (timeBeforeStartingPatternAgain > 0)
            {
                //timeBeforeStartingPatternAgain -= Time.deltaTime;
            }
            else
            {
                DetermineWhichPatternToUse();
                timeBeforeStartingPatternAgain = timeBeforeStartingPatternAgainBase;
            }
        }
	}

    void DetermineWhichPatternToUse()
    {
        starMoveSpeed = Random.Range(GameData.minStarSpeed, GameData.maxStarSpeed);

        if (currentPatternType == PatternType.EightStarDiamond)
        {
            EightStarDiamond(starMoveSpeed, starTypesToSpawn);
        }
        else if (currentPatternType == PatternType.Spiral01)
        {
            StartCoroutine(Spiral01(starMoveSpeed, starTypesToSpawn, spiralRotateAmt, spiralSpawnTime));
        }
        else if (currentPatternType == PatternType.Wall01)
        {
            Wall01(starMoveSpeed, starTypesToSpawn, wallSpacing);
        }
    }

    public void StartMovement(float speedModifier = 1)
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, -1, 0) * speedModifier;
    }

    void SpawnObject(float customSpeed, Vector3 dir, int type, Vector3 offset)
    {
        if (InactiveStars.transform.childCount <= 0)
        {
            AddToPool(transform.position + offset, Quaternion.identity, Vector3.one, customSpeed, dir, type);
            GetFromPool(transform.position + offset, Quaternion.identity, Vector3.one, customSpeed, dir, type);
        }
        else
        {
            GetFromPool(transform.position + offset, Quaternion.identity, Vector3.one, customSpeed, dir, type);
        }

        //GetFromPool(transform.position, Quaternion.identity, Vector3.one, customSpeed, dir, type);
    }

    //spawns a new object when there's not any inactive pooled objects to use
    void AddToPool(Vector3 pos, Quaternion rot, Vector3 scale, float customSpeed, Vector3 dir, int type)
    {
		//From Amanda:
		//You were creating a bunch of unnamed gameobjects
		//When you use new gameobject, it creates a new one in scene.
		GameObject a = null;

        if (type == 0)
        {
            a = Instantiate(StarTypesAvailable[Random.Range(0, StarTypesAvailable.Length)], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 1)
        {
            a = Instantiate(StarTypesAvailable[0], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 2)
        {
            a = Instantiate(StarTypesAvailable[1], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 3)
        {
            a = Instantiate(StarTypesAvailable[2], pos, rot, InactiveStars.transform) as GameObject;
        }
        else if (type == 4)
        {
            a = Instantiate(StarTypesAvailable[3], pos, rot, InactiveStars.transform) as GameObject;
        }

		if (a != null)
		{
			a.transform.localScale = new Vector3(StarScale, StarScale, StarScale);
			a.name = "Star";
			a.GetComponent<PooledObject>().MyParent = InactiveStars;

			a.GetComponent<StarController>().usesCustomSpeed = true;
			a.GetComponent<StarController>().customSpeed = customSpeed;
			a.GetComponent<StarController>().customSpeedDirection = dir;
			a.GetComponent<StarController>().RefreshCustomSpeedValues();
		}
    }

    //moves an inactive pooled object to the appropriate place and then makes it set active
    void GetFromPool(Vector3 pos, Quaternion rot, Vector3 scale, float customSpeed, Vector3 dir, int type)
    {
		//From Amanda:
		//You were creating a bunch of unnamed gameobjects
		//When you use new gameobject, it creates a new one in scene.
		GameObject thePooledObj = null;

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
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == Star.StarType.Circle)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
            else if(type == 2)//Square Star
            {
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == Star.StarType.Square)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
            else if(type == 3)//Star Star
            {
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == Star.StarType.Star)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
            else if(type == 4)//Triangle Star
            {
                if (InactiveStars.transform.GetChild(i).gameObject.GetComponent<StarController>().theStarType == Star.StarType.Triangle)
                {
                    thePooledObj = InactiveStars.transform.GetChild(0).gameObject;
                }
            }
        }

		if (thePooledObj != null)
		{
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

    //coroutine
    IEnumerator Spiral01(float speed, int[] types, float rotateAmt, float timeInterval)
    {
        for (int i = 0; i < types.Length; i++)
        {

            SpawnInEmitterFacingDir(speed, types[i]);

            //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 15));
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + rotateAmt);


            yield return new WaitForSeconds(timeInterval);
        }
    }

    void Wall01(float speed, int[] types, float spacing)
    {
        /*Vector3 currentOffset = new Vector3((0 - ((types.Length * spacing) / 2) + (spacing / 2)), 0, 0);

        for(int i = 0; i < types.Length; i++)
        {
            SpawnDownWithOffset(speed, types[i], currentOffset);
            currentOffset = new Vector3(currentOffset.x + spacing, currentOffset.y, currentOffset.z);
        }*/

        bool left = false;
        float customSpeed01 = 1;
        float customSpeed02 = 1;

        SpawnDown(2, 0);

        for(int i = 0; i < types.Length; i++)
        {
            if (left == false)
            {
                SpawnLeft(customSpeed01, types[i]);
                customSpeed01 += 0.25f;
                left = true;
            }
            else
            {
                SpawnRight(customSpeed02, types[i]);
                customSpeed02 += 0.25f;
                left = false;
            }
        }
    }
    /*patterns************/

    /*pieces of patterns*********/
    void SpawnUp(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0, 1, 0), type, Vector3.zero);
    }

    void SpawnUpRight(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0.5f, 0.5f, 0), type, Vector3.zero);
    }

    void SpawnRight(float speed, int type)
    {
        SpawnObject(speed, new Vector3(1, 0, 0), type, Vector3.zero);
    }

    void SpawnDownRight(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0.5f, -0.5f, 0), type, Vector3.zero);
    }

    void SpawnDown(float speed, int type)
    {
        SpawnObject(speed, new Vector3(0, -1, 0), type, Vector3.zero);
    }

    void SpawnDownLeft(float speed, int type)
    {
        SpawnObject(speed, new Vector3(-0.5f, -0.5f, 0), type, Vector3.zero);
    }

    void SpawnLeft(float speed, int type)
    {
        SpawnObject(speed, new Vector3(-1, 0, 0), type, Vector3.zero);
    }

    void SpawnUpLeft(float speed, int type)
    {
        SpawnObject(speed, new Vector3(-0.5f, 0.5f, 0), type, Vector3.zero);
    }

    void SpawnInEmitterFacingDir(float speed, int type)
    {
        SpawnObject(speed, transform.up, type, Vector3.zero);
    }

    void SpawnDownWithOffset(float speed, int type, Vector3 offset)
    {
        SpawnObject(speed, new Vector3(0, -1, 0), type, offset);
    }

    /*pieces of patterns*********/
}
