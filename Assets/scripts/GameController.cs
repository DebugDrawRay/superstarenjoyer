﻿using UnityEngine;
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

    [Header("Star Properties")]
    public StarManager starMan;

    [Header("Debug")]
    public bool spawnComet;

	[Header("Pause Screen")]
	public GameObject PauseScreen;

    public PlayerActions controller;
    //Events
    public delegate void CollisionEvent(float strength, float speed);
    public static event CollisionEvent CometCollisionEvent;

    public static void TriggerCometCollision(float strength, float speed)
    {
        if(CometCollisionEvent != null)
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
        if(AddScoreEvent != null)
        {
            AddScoreEvent(score);
        }

    }

    public delegate void Trigger();
    public static event Trigger EndGameTrigger;

    public static void TriggerEndGame()
    {
        if(EndGameTrigger != null)
        {
            EndGameTrigger();
        }
    }

	 public void Pause()
	{
		if (PauseScreen.activeInHierarchy)
			UnPause();
		else
		{
			Time.timeScale = 0;
			PauseScreen.SetActive(true);
		}
	}

	public void UnPause()
	{
		Time.timeScale = 1;
		PauseScreen.SetActive(false);
	}

    void Awake()
    {
        AssignEvents();
		starMan = GetComponent<StarManager>();
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
        switch(currentState)
        {
            case State.Setup:
                SpawnObjects();
                SetupLevel();
                currentState = State.Start;
                break;
            case State.Start:
                UpdateUi();
                UiController.TriggerVelocityEvent(GameData.cometAcelerationLevels[currentAccelerationLevel] * GameData.speedScalar);
                AudioController.Instance.StartMusic();
                StartCoroutine(StartSequence());
                currentState = State.Transition;
                break;
            case State.InGame:
                UpdateComet();
                UpdateLevel();
                UpdateUi();
                if(controller.Pause.WasPressed)
                {
                    Pause();
                }
                break;
            case State.Pause:
                break;
            case State.End:
                UiController.TriggerKillScreen("Nothing");
                starMan.enabled = false;
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
			currentDistance = Mathf.MoveTowards(currentDistance, GameData.cometDest, modifier * GameData.cometAcelerationLevels[currentAccelerationLevel] * Time.deltaTime);
		}
		cometRigid.transform.position = new Vector2(0, currentDistance);

        if (currentDistance <= GameData.dangerLimit)
        {
            if (!inDanger)
            {
                AudioController.Instance.FadeToDanger(true);
                inDanger = true;
                UiController.TriggerCometDanger(true);
            }
        }
        else
        {
            if (inDanger)
            {
                AudioController.Instance.FadeToDanger(false);
                inDanger = false;
                UiController.TriggerCometDanger(false);
            }
        }

		if (cometBoostTimer >= 0)
			cometBoostTimer -= Time.deltaTime;
    }

	void UpdateLevel()
	{
		if (currentAccelerationLevel < GameData.cometAcelerationLevels.Length - 1)
		{
			if (levelTimer <= 0)
			{
				levelTimer = GameData.levelTime;
				currentAccelerationLevel++;
				starMan.spawnLevel++;
				Debug.Log("NEW LEVEL: " + currentAccelerationLevel);
                UiController.TriggerVelocityEvent(GameData.cometAcelerationLevels[currentAccelerationLevel]);
            }
            else
			{
				levelTimer -= Time.deltaTime;
			}
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
        if(currentTween != null)
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
        currentPlayer.GetComponent<PlayerController>().canMove = true;
        starMan.enabled = true;
        currentState = State.InGame;
    }

	IEnumerator EndSequence()
	{
		cometRigid.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel * 1.2f);
		currentPlayer.transform.DOMoveY(GameData.cometStartY, GameData.cometStartAccel * 1.2f).SetEase(Ease.InOutBack);
		currentPlayer.transform.DOMoveX(0, GameData.cometStartAccel).SetEase(Ease.InOutBack);
		yield return new WaitUntil(() => cometRigid.transform.position.y >= GameData.cometStartY);
		currentPlayer.GetComponent<PlayerController>().canMove = false;
		starMan.enabled = false;
		currentState = State.End;
	}
}
