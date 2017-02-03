using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class GameController : MonoBehaviour
{
	public enum State
	{
		Setup,
		Start,
		InGame,
		Pause,
		End,
		Transition
	}
	public State currentState;

	public int currentScore;

	[Header("Player")]
	public GameObject player;
	private GameObject currentPlayer;
	protected PlayerController currentPlayerController;
	public Transform playerSpawn;

	[Header("Comet Properties")]
	public GameObject comet;
	public Transform cometSpawn;
	private Rigidbody cometRigid;
	protected Transform cometMeshTransform;
	protected Rotate cometRotate;
	public bool frozen = false;

	protected float cometBoostTimer = 0f;

	private float currentDistance;
	private float currentTarget;

	//Current tween
	private Tween currentTween;
	private bool hit;

	//Timing
	private float timeToSpeedIncrease;
	private int currentAccelerationLevel;
	private bool inDanger;
	protected float levelTimer;

	//Other Controllers
	protected StarManager starManager;
	public PlayerActions controller;

	[Header("Debug")]
	public bool spawnComet;
	protected State PrePauseState;


	//Events
	public delegate void CollisionEvent(float strength, float speed);
	public static event CollisionEvent CometCollisionEvent;



	public static void TriggerCometCollision(float strength, float speed)
	{
		if (CometCollisionEvent != null)
		{
			CometCollisionEvent(strength, speed);
		}
	}

	public delegate void BoostEvent();
	public static event BoostEvent CometBoostEvent;
	public static void TriggerCometBoost()
	{
		if (CometBoostEvent != null)
		{
			CometBoostEvent();
		}
	}

	public delegate void ScoreEvent(int score);
	public static event ScoreEvent AddScoreEvent;

	public static void TriggerAddScore(int score)
	{
		if (AddScoreEvent != null)
		{
			AddScoreEvent(score);
		}

	}

	public delegate void Trigger();
	public static event Trigger EndGameTrigger;

	public static void TriggerEndGame()
	{
		if (EndGameTrigger != null)
		{
			EndGameTrigger();
		}
	}

	public void Pause()
	{
		if (currentState == State.Pause)
		{
			UnPause();
		}
		else
		{
			DoPause();
		}
	}

	public void DoPause()
	{
		currentPlayerController.DisableMovement();
		PrePauseState = currentState;
		currentState = State.Pause;

		Time.timeScale = 0;
		UiController.PauseScreenEvent(true);
	}

	public void UnPause()
	{
		currentPlayerController.EnableMovement();
		currentState = PrePauseState;

		Time.timeScale = 1;
		UiController.PauseScreenEvent(false);
	}

	void Awake()
	{
		AssignEvents();
		starManager = GetComponent<StarManager>();
	}

	void Start()
	{
		controller = PlayerActions.BindAll();
	}

	void AssignEvents()
	{
		CometCollisionEvent += AddDistanceToComet;
		CometBoostEvent += BoostComet;
		AddScoreEvent += AddScore;
		EndGameTrigger += EndGame;
	}

	void OnDestroy()
	{
		CometCollisionEvent -= AddDistanceToComet;
		CometBoostEvent -= BoostComet;
		AddScoreEvent -= AddScore;
		EndGameTrigger -= EndGame;
	}
	void Update()
	{
		RunStates();
	}

	void SpawnObjects()
	{
		currentPlayer = (GameObject)Instantiate(player, playerSpawn.position, playerSpawn.rotation);
		currentPlayerController = currentPlayer.GetComponent<PlayerController>();
		if (spawnComet)
		{
			GameObject newComet = (GameObject)Instantiate(comet, cometSpawn.position, cometSpawn.rotation);
			cometRigid = newComet.GetComponent<Rigidbody>();
			cometMeshTransform = newComet.transform.FindChild("Commet_fbx");
			cometRotate = newComet.GetComponentInChildren<Rotate>();
		}
	}

	void SetupLevel()
	{
		currentDistance = GameData.cometStartY;
		timeToSpeedIncrease = GameData.accelerationIncreaseRate;
		currentAccelerationLevel = 0;
		levelTimer = GameData.levelTime;
	}

	void RunStates()
	{
		switch (currentState)
		{
			case State.Setup:
				SpawnObjects();
				SetupLevel();
				currentState = State.Start;
				break;
			case State.Start:
				UpdateUi();
				UiController.TriggerVelocityEvent(GameData.cometAccelerationBase);
				AudioController.Instance.StartMusic();
				StartCoroutine(StartSequence());
				currentState = State.Transition;
				break;
			case State.InGame:
				UpdateComet();
				UpdateLevel();
				UpdateUi();
				if (controller.Pause.WasPressed)
				{
					Pause();
				}
				break;
			case State.Pause:
				if (controller.Pause.WasPressed)
				{
					Pause();
				}
				break;
			case State.End:
				break;
			case State.Transition:
				break;
		}
	}

	void UpdateComet()
	{
		if (!hit)
		{
			float modifier = cometBoostTimer >= 0 ? GameData.cometBoostMultiplier : 1;
			currentDistance = Mathf.MoveTowards(currentDistance, GameData.cometDest, modifier * (GameData.cometAccelerationBase + (GameData.cometAccelerationIncrease * currentAccelerationLevel)) * Time.deltaTime);
		}
		cometRigid.transform.position = new Vector2(0, currentDistance);

		//if (currentDistance <= GameData.dangerLimit)
		//{
		//    if (!inDanger)
		//    {
		//        AudioController.Instance.FadeToDanger(true);
		//        inDanger = true;
		//        UiController.TriggerCometDanger(true);
		//    }
		//}
		//else
		//{
		//    if (inDanger)
		//    {
		//        AudioController.Instance.FadeToDanger(false);
		//        inDanger = false;
		//        UiController.TriggerCometDanger(false);
		//    }
		//}

		// okay, Doug's trying something now
		//
		// I want to make it so that the comet has two thresholds, one for going into danger music
		//   and another for coming out of it. The danger threshold is lower on the screen than the
		//   safety threshold so that we don't have the music flopping back and forth repeatedly
		//   when the player is dancing around the line
		if (inDanger)
		{
			if (currentDistance >= GameData.safetyLimit)
			{
				inDanger = false;

				AudioController.Instance.FadeToDanger(inDanger);
				UiController.TriggerCometDanger(inDanger);
			}
		}
		else
		{
			if (currentDistance <= GameData.dangerLimit)
			{
				inDanger = true;

				AudioController.Instance.FadeToDanger(inDanger);
				UiController.TriggerCometDanger(inDanger);
			}
		}

		if (cometBoostTimer >= 0)
			cometBoostTimer -= Time.deltaTime;
	}

	void UpdateLevel()
	{
		if (levelTimer <= 0)
		{
			levelTimer = GameData.levelTime;
			currentAccelerationLevel++;

			starManager.IncreaseSpawnLevel();
			Debug.Log("NEW LEVEL: " + currentAccelerationLevel);
			UiController.TriggerVelocityEvent(GameData.cometAccelerationBase + (GameData.cometAccelerationIncrease * currentAccelerationLevel));
		}
		else
		{
			levelTimer -= Time.deltaTime;
		}
	}

	void BoostComet()
	{
		cometBoostTimer += GameData.cometBoostTimerAdd;
	}

	void UpdateUi()
	{
		UiController.TriggerScoreEvent(currentScore);
		UiController.TriggerDistanceEvent(currentDistance);
	}

	void UpdateMeters()
	{
	}

	void AddDistanceToComet(float strength, float speed)
	{
		Tweener shakeTween = cometMeshTransform.DOShakePosition(0.1f, 0.4f, 80, 180);
		shakeTween.OnComplete(() => cometMeshTransform.localPosition = new Vector3(0, 4.22f, 0));

		hit = true;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		float newDistance = currentDistance + strength;
		currentTween = DOTween.To(() => currentDistance, x => currentDistance = x, currentDistance + strength, speed).SetEase(Ease.OutExpo);
		currentTween.OnComplete(() => hit = false);
	}

	void AddScore(int score)
	{
		currentScore += score;
	}

	void EndGame()
	{
		AudioController.Instance.EndMusic();
		currentState = State.Transition;
		StartCoroutine(EndSequence());
		//currentState = State.End;
	}

	IEnumerator StartSequence()
	{
		cometRigid.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel);
		yield return new WaitUntil(() => cometRigid.transform.position.y >= GameData.cometStartY);
		currentPlayer.transform.DOMoveY(GameData.playerStartY, GameData.cometStartAccel);
		yield return new WaitUntil(() => currentPlayer.transform.position.y >= GameData.playerStartY);
		currentPlayerController.EnableMovement();
		starManager.EnableSpawning();
		currentState = State.InGame;
	}

	IEnumerator EndSequence()
	{
		//UiController.stopAddingToTimeLasted = true;
		cometRigid.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel * 1.2f);
		currentPlayer.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel * 1.2f).SetEase(Ease.InOutBack);
		currentPlayer.transform.DOMoveX(0, GameData.cometStartAccel).SetEase(Ease.InOutBack);
		yield return new WaitUntil(() => cometRigid.transform.position.y >= GameData.cometStartY);
		currentPlayerController.DisableMovement();
		starManager.DisableSpawning();

		UiController.TriggerKillScreen("Nothing");
		currentState = State.End;
	}
}
