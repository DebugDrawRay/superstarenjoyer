using UnityEngine;
using System.Collections;

public class StarManager : MonoBehaviour
{
    public GameObject ActiveStars;
    public GameObject InactiveStars;

    public GameObject[] StarsToSpawn;

    private float SpawnTimer;
	 protected int spawnLevel = 0;

    public float starScale;

    void Awake()
    {
        ActiveStars = new GameObject("ActiveStars");
        InactiveStars = new GameObject("InactiveStars");
        InactiveStars.SetActive(false);

		Debug.Log("Star manager is here!");
    }

	// Use this for initialization
	void Start () 
    {
        SpawnTimer = Random.Range(GameData.starSpawnTimers[spawnLevel].x, GameData.starSpawnTimers[spawnLevel].y);
    }

    // Update is called once per frame
    void Update () 
    {
        if (SpawnTimer > 0)
        {
            SpawnTimer -= Time.deltaTime;
        }
        else
        {
            SpawnObject();
            SpawnTimer = Random.Range(GameData.starSpawnTimers[spawnLevel].x, GameData.starSpawnTimers[spawnLevel].y);
		}
	}

	public void IncreaseSpawnLevel()
	{
		if (spawnLevel < GameData.starSpawnTimers.Length - 1)
		{
			spawnLevel++;
		}
	}

    Vector3 GetSpawnPos()
    {
        int spot = Random.Range(0, Mathf.RoundToInt(GameData.fieldSize / GameData.starSize));
        float sign = 1;
        if(Random.value >= .5)
        {
            sign = -1;
        }
        float xPos = (spot * GameData.starSize) * sign;
        Vector3 theSpawnPos = new Vector3(xPos, GameData.starSpawnY, 0);
        return theSpawnPos;
    }

    void SpawnObject()
    {
        if (InactiveStars.transform.childCount <= 0)
        {
            AddToPool(GetSpawnPos(), Quaternion.identity, Vector3.one);
        }

        GetFromPool(GetSpawnPos(), Quaternion.identity, Vector3.one);
    }

    //spawns a new object when there's not any inactive pooled objects to use
    void AddToPool(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        //print("spawn new object");
        GameObject a = Instantiate(StarsToSpawn[Random.Range(0, StarsToSpawn.Length)], pos, rot, InactiveStars.transform) as GameObject;
        //a.transform.localScale = scale;
        a.transform.localScale = new Vector3(starScale, starScale, starScale);
        a.name = "Star";
        a.GetComponent<PooledObject>().MyParent = InactiveStars;
    }

    //moves an inactive pooled object to the appropriate place and then makes it set active
    void GetFromPool(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        GameObject thePooledObj = InactiveStars.transform.GetChild(0).gameObject;

		StarController starController = thePooledObj.GetComponent<StarController>();
		starController.starScorePopup.GetComponent<Renderer>().enabled = false;
		starController.starScorePopup.GetComponent<Animator>().enabled = false;
		starController.usesCustomSpeed = false;

		thePooledObj.transform.position = pos;
		thePooledObj.transform.rotation = rot;
		thePooledObj.transform.localScale = new Vector3(starScale, starScale, starScale);
        thePooledObj.transform.SetParent(ActiveStars.transform);
		thePooledObj.GetComponent<StarController>().StartMovement();
    }
}
