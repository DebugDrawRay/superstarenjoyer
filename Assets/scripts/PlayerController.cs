using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	public float speed;
	public float acceleration;
	public bool canMove = false;

	private PlayerActions actions;
	private Rigidbody rigid;

	private ConstellationManager constManager;

	//Star tracking
	private StarController lastStar;

	public GameObject PlayerModel;
	protected Vector3 CurrentRotation;

	[Header("Player Colors")]
	public Material NormalMat;
	public Material PinkMat;
	public Material BlueMat;
	public Material GreenMat;
	public Material YellowMat;

	public static PlayerController Instance;

    public GameObject fireworksPrefab;

	[Header("Comet")]
	public string cometTag;

	[Header("Stars")]
	public LineRenderer line;


	void Awake()
	{
		Instance = this;
		CurrentRotation = PlayerModel.transform.localEulerAngles;
	}

	void Start()
	{
		actions = PlayerActions.BindAll();
		rigid = GetComponent<Rigidbody>();
		constManager = ConstellationManager.Instance;
	}

	void Update()
	{
		CompleteConstellationListener();
		DrawConstellationLine();
	}

	void FixedUpdate()
	{
		if (canMove)
		{
			Movement();
		}
		else
		{
			Spiral();
		}
	}

	void CompleteConstellationListener()
	{
		if (actions.PrimaryAction.WasPressed)
		{
			if (constManager.CompleteConstellation())
			{
				ChangeColor(GameData.StarType.None);
				lastStar = null;
			}
		}
	}

	void Movement()
	{
		//Movement
		Vector2 dir = actions.Move.Value * speed;
		Vector2 newSpeed = Vector2.Lerp(rigid.velocity, dir, acceleration);

		//Rotation	
		Vector3 destRot = CurrentRotation;
		destRot.y = 180 + -actions.Move.Value.x * 50;
		Vector3 rotation = Vector3.Lerp(CurrentRotation, destRot, 20f * Time.deltaTime);
		PlayerModel.transform.localEulerAngles = CurrentRotation = rotation;

		rigid.velocity = newSpeed;
	}

	void Spiral()
	{
		//Rotation	
		Vector3 destRot = CurrentRotation;
		destRot.y = CurrentRotation.y > 360 ? 0 : CurrentRotation.y + 20;
		PlayerModel.transform.localEulerAngles = CurrentRotation = destRot;
	}

	void OnTriggerEnter(Collider hit)
	{
		StarController isStar = hit.GetComponent<StarController>();

		if (isStar)
		{
            if (lastStar != null && lastStar.theStarType != isStar.theStarType)
            {
                Camera.main.GetComponent<CameraController>().DoScreenShake();
				GameController.TriggerCometBoost();
                constManager.BreakConstellation();
                isStar.FadeOutStar();
                ChangeColor(GameData.StarType.None);
                lastStar = null;
            }
            else
            {
                lastStar = isStar;
                isStar.StopMovement();
                //isStar.starBoing.GetComponent<StarBoingController>().StartGrowing();
                isStar.starBoing.gameObject.SetActive(true);//active the star boing

                StarController isStarStarCont = isStar.GetComponent<StarController>();
                isStarStarCont.delayBeforeSecondBoingTimer = isStarStarCont.delayBeforeSecondBoingTimerBase;//start the timer for the 2nd star boing

                isStar.starData.Position = isStar.transform.position;
                ChangeColor(isStar.theStarType);
                constManager.AddStar(isStar.starData);
                AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood);

                isStar.DoBoing();
                isStar.DoGotHitAnim();

                FindAllStarsOfSameTypeAndBoingThem(isStar);
            }
        }

		if (hit.gameObject.tag == cometTag)
		{
			//KILL THE WORLD 
			GameController.TriggerEndGame();


            //spawn fireworks
            Instantiate(fireworksPrefab, transform.position, Quaternion.identity);
            Instantiate(fireworksPrefab, new Vector3(-3, 0, 0), Quaternion.identity);
            Instantiate(fireworksPrefab, new Vector3(3, 0, 0), Quaternion.identity);
            Instantiate(fireworksPrefab, new Vector3(0, 0, 0), Quaternion.identity);

			AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationBroken);

			var colliders = gameObject.GetComponents<Collider>();
			lastStar = null;
			canMove = false;
			constManager.BreakConstellation();
            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationComplete, (int)SoundBank.SoundEffects.ConstellationBroken);
            for (int i = 0; i < colliders.Length; i++)
				colliders[i].enabled = false;

			//gameObject.SetActive(false);
		}

    }

	public void ChangeColor(GameData.StarType type)
	{
		Renderer render = PlayerModel.GetComponent<Renderer>();
		switch (type)
		{
			case GameData.StarType.Circle:
				render.material = BlueMat;
				break;
			case GameData.StarType.Square:
				render.material = PinkMat;
				break;
			case GameData.StarType.Star:
				render.material = YellowMat;
				break;
			case GameData.StarType.Triangle:
				render.material = GreenMat;
				break;
			case GameData.StarType.None:
				render.material = NormalMat;
				break;
		}
	}

	void DrawConstellationLine()
	{
		line.enabled = (lastStar != null);

		if (lastStar != null)
		{
			line.SetPosition(0, line.transform.position);
			line.SetPosition(1, lastStar.transform.position);
		}
	}

    void FindAllStarsOfSameTypeAndBoingThem(StarController _isStar)
    {
        GameObject[] allStars = GameObject.FindGameObjectsWithTag("Star");

        for (int i = 0; i < allStars.Length; i++)
        {
            if (_isStar.theStarType == allStars[i].GetComponent<StarController>().theStarType)//if the star that is passed is the same as the star that you come across in the array
            {
                //ping the star in the allStars array
                if(allStars[i].activeSelf == true)//make sure it's active
                {
                    allStars[i].GetComponent<StarController>().DoBoing();//boing it
                }
            }
        }
    }
}
