using UnityEngine;
using System.Collections;
using InControl;

public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	public float speed;
	public float acceleration;
	protected bool canMove = false;

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

    public int starsInCurrentConstellation;

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
        constManager.player = gameObject;
	}

	void Update()
	{
		if (canMove)
		{
			CompleteConstellationListener();
			DrawConstellationLine();
		}
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
				ControllerInputManager.Instance.VibrateController(0.8f, 0.4f);
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

	public void EnableMovement()
	{
		canMove = true;
	}

	public void DisableMovement()
	{
		canMove = false;
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
				ControllerInputManager.Instance.VibrateController(1f, 0.4f);
			}
            else
            {
                
                if (lastStar != null)
                {
                
                    lastStar.gameObject.GetComponent<StarController>().latestStar = false;
              
                }

                lastStar = isStar;
                isStar.StopMovement();
                isStar.starBoing.gameObject.SetActive(true);//active the star boing
				isStar.delayBeforeSecondBoingTimer = isStar.delayBeforeSecondBoingTimerBase;//start the timer for the 2nd star boing

                isStar.starData.Position = isStar.transform.position;
                ChangeColor(isStar.theStarType);

                int starCount = constManager.AddStar(isStar.starData);

                if (hit.gameObject.GetComponent<StarController>().hasBeenTouched == false)
                {
                    

                    /*if (hit.gameObject.GetComponent<StarController>().latestStar == false)
                    {
                        Debug.Log(starCount);
                        if (starCount == 1)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood00);
                        }
                        else if (starCount == 2)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood01);
                        }
                        else if (starCount == 3)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood02);
                        }
                        else if (starCount == 4)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood03);
                        }
                        else if (starCount == 5)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood04);
                        }
                        else if (starCount == 6)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood05);
                        }
                        else if (starCount == 7)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood06);
                        }
                        else if (starCount == 8)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood07);
                        }
                        else if (starCount == 9)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood08);
                        }
                        else if (starCount == 10)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood09);
                        }
                        else if (starCount == 11)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood10);
                        }
                        else if (starCount == 12)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood11);
                        }
                        else if (starCount == 13)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood12);
                        }
                        else if (starCount == 14)
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood13);
                        }
                        else
                        {
                            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood14);
                        }
                    }*/

                    hit.gameObject.GetComponent<StarController>().latestStar = true;
                    hit.gameObject.GetComponent<StarController>().hasBeenTouched = true;
                }

				ControllerInputManager.Instance.VibrateController(0.4f, 0.1f);

				//AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarGood);
				
				//AudioController.Instance.PlaySfx(SoundBank.SoundEffects.NewLink);

                AudioController.Instance.PlaySfx(SoundBank.SoundEffects.StarTouch);

				

                isStar.DoBoing();
                isStar.DoGotHitAnim();

                FindAllStarsOfSameTypeAndBoingThem(isStar);
            }
        }

		if (hit.gameObject.tag == cometTag)
		{
			ControllerInputManager.Instance.VibrateController(1f, 3f);

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
			AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationComplete, SoundBank.SoundEffects.ConstellationBroken);
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
