using UnityEngine;
using System.Collections;

public class StarManager : MonoBehaviour
{
	public static StarManager Instance;

	protected GameObject ActiveStars;
	protected GameObject InactiveStars;
	public GameObject[] StarsToSpawn;
	public GameObject[] PatternsToSpawn;

	public int NumberOfSpawnColumns = 20;

	//Patterns
	StarPattern CurrentPattern;

	//In-Betweens
	private float SpawnTimer;
	protected int SpawnLevel = 0;
	protected bool Spawning = false;
	protected float PatternBreakTime = 0;
	protected float PatternBreakTotalTime = 10f;

	public float starScale;

	void Awake()
	{
		Instance = this;

		ActiveStars = new GameObject("ActiveStars");
		InactiveStars = new GameObject("InactiveStars");
		InactiveStars.SetActive(false);
	}

	void Update()
	{
		if (Spawning)
		{
			if (SpawnTimer > 0)
			{
				SpawnTimer -= Time.deltaTime;
			}
			else
			{
				SpawnObject();
				SpawnTimer = Random.Range(GameData.starSpawnTimers[SpawnLevel].x, GameData.starSpawnTimers[SpawnLevel].y);
			}
			PatternBreakTime += Time.deltaTime;

			if (PatternBreakTime > PatternBreakTotalTime)
			{
				Spawning = false;
				SpawnPattern();
			}
		}
		else
		{
			if (CurrentPattern != null && CurrentPattern.Completed)
			{
				Destroy(CurrentPattern.gameObject);
				EnableSpawning();
			}
		}
	}

	public void EnableSpawning()
	{
		SpawnTimer = Random.Range(GameData.starSpawnTimers[SpawnLevel].x, GameData.starSpawnTimers[SpawnLevel].y);
		PatternBreakTime = 0;
		Spawning = true;
	}

	public void DisableSpawning()
	{
		Spawning = false;
	}

	public void SpawnPattern()
	{
		int patternIndex = Random.Range(0, PatternsToSpawn.Length);
		Debug.Log("Spawning Pattern: " + patternIndex);
		GameObject prefab = PatternsToSpawn[patternIndex];
		GameObject pattern = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
		CurrentPattern = pattern.GetComponent<StarPattern>();
	}

	public void IncreaseSpawnLevel()
	{
		if (SpawnLevel < GameData.starSpawnTimers.Length - 1)
		{
			SpawnLevel++;
		}
	}

	protected Vector3 GetSpawnPos()
	{
		//int spot = Random.Range(0, Mathf.RoundToInt(GameData.fieldSize / GameData.starSize));
		//spot *= Random.value >= 0.5f ? -1 : 1;
		//float xPos = (spot * GameData.starSize);
		//Vector3 theSpawnPos = new Vector3(xPos, GameData.starSpawnY, 0);

		int column = (int)Random.Range(0, NumberOfSpawnColumns);
		float columnSize = (GameData.fieldSize * 2) / NumberOfSpawnColumns;
		float spot = (column * columnSize) - GameData.fieldSize + (columnSize / 2);
		Vector3 theSpawnPos = new Vector3(spot, GameData.starSpawnY, 0);

		return theSpawnPos;
	}

	public void SpawnObject()
	{
		Vector3 position = GetSpawnPos();
		Vector3 direction = Vector3.down;
		SpawnObject(position, direction);
	}

	public void SpawnObject(Vector3 position, Vector3 direction)
	{
		if (InactiveStars.transform.childCount <= 0)
		{
			AddToPool(position, Quaternion.identity, Vector3.one, direction);
		}
		GetFromPool(position, Quaternion.identity, Vector3.one, direction);
	}

	//spawns a new object when there's not any inactive pooled objects to use
	//protected void AddToPool(Vector3 pos, Quaternion rot, Vector3 scale)
	//{
	//	Vector3 direction = Vector3.down;
	//	AddToPool(pos, rot, scale, direction);
	//}

	protected void AddToPool(Vector3 pos, Quaternion rot, Vector3 scale, Vector3 direction)
	{
		GameObject newStar = Instantiate(StarsToSpawn[Random.Range(0, StarsToSpawn.Length)], pos, rot, InactiveStars.transform) as GameObject;
		newStar.name = "Star";
		newStar.transform.localScale = new Vector3(starScale, starScale, starScale);
		newStar.GetComponent<PooledObject>().MyParent = InactiveStars;

		StarController starController = newStar.GetComponent<StarController>();
		starController.StartMovement(direction);
	}

	//protected void GetFromPool(Vector3 pos, Quaternion rot, Vector3 scale)
	//{
	//	Vector3 direction = Vector3.down;
	//	GetFromPool(pos, rot, scale, direction);
	//}

	protected void GetFromPool(Vector3 pos, Quaternion rot, Vector3 scale, Vector3 direction)
	{
		GameObject thePooledObj = InactiveStars.transform.GetChild(0).gameObject;

		thePooledObj.transform.position = pos;
		thePooledObj.transform.rotation = rot;
		thePooledObj.transform.localScale = new Vector3(starScale, starScale, starScale);
		thePooledObj.transform.SetParent(ActiveStars.transform);

		StarController starController = thePooledObj.GetComponent<StarController>();
		starController.StartMovement(direction);
	}
}
