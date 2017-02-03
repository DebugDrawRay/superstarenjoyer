using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class StarController : MonoBehaviour
{
	//public enum starType;

	public Star.StarType theStarType;
	public UnityEvent starTriggered;
	public float destroyStarWhenBelowThisYValue;
    public float destroyStarWhenAboveThisYValue;
    public float destroyStarWhenPastThisXValue;
	private float starSpeed;
    public bool usesCustomSpeed;
    public float customSpeed;
    public Vector3 customSpeedDirection;

	[HideInInspector]
	public Star starData;

	public float shrinkSpeed;
	public bool doShrink;

	public GameObject starBoing;
	public GameObject starBoing02;
	public float delayBeforeSecondBoingTimerBase;
	public float delayBeforeSecondBoingTimer;

	protected Material starMaterial;
	protected Renderer starRenderer;
	protected MaterialPropertyBlock properties;
	protected Color startColor;
	protected Color startEmissionColor;
	protected Color startSpecularColor;
	protected Vector3 initialScale;

    public Animator theAnimator;

    public bool hasBeenTouched;

    //this is true if it's the most recent star the player has touched
    public bool latestStar;

    public GameObject starScorePopup;

	void Awake()
	{
		starData = new Star(this);
		starRenderer = GetComponentInChildren<MeshRenderer>();
		starMaterial = GetComponentInChildren<MeshRenderer>().material;

		startColor = starMaterial.color;
		startEmissionColor = starMaterial.GetColor("_EmissionColor");
		startSpecularColor = starMaterial.GetColor("_SpecColor");
		properties = new MaterialPropertyBlock();
		initialScale = transform.localScale;
		
		starMaterial.SetFloat("_Mode", 3f);
		starMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");

		starSpeed = Random.Range(GameData.minStarSpeed, GameData.maxStarSpeed);
	}

	void OnEnable()
	{
		properties.SetColor("_Color", startColor);
		properties.SetColor("_SpecColor", startSpecularColor);
		properties.SetColor("_EmissionColor", startEmissionColor);
		starRenderer.SetPropertyBlock(properties);
		transform.localScale = initialScale;

		//Clear previous star links
		starData.ClearLinks();

		starScorePopup.GetComponent<Renderer>().enabled = false;
		starScorePopup.GetComponent<Animator>().enabled = false;
		usesCustomSpeed = false;
	}

	void Update()
	{
		if (gameObject.layer == LayerMask.NameToLayer("Stars"))
		{
            if (transform.position.y < destroyStarWhenBelowThisYValue || transform.position.y > destroyStarWhenAboveThisYValue || transform.position.x > destroyStarWhenPastThisXValue || transform.position.x < -destroyStarWhenPastThisXValue)
			{
				StopMovement();
				transform.position = new Vector3(0, 0, 0);
				GetComponent<PooledObject>().ReturnToPool();
			}
		}

		if (doShrink == true)
		{
			if (transform.localScale.x > 0.1f)
			{
				transform.localScale = new Vector3(transform.localScale.x - (shrinkSpeed * Time.deltaTime), transform.localScale.y - (shrinkSpeed * Time.deltaTime), transform.localScale.z - (shrinkSpeed * Time.deltaTime));
			}
			else
			{
				Destroy(gameObject);
			}
		}

		if (delayBeforeSecondBoingTimer > 0 && delayBeforeSecondBoingTimer < 100)
		{
			delayBeforeSecondBoingTimer -= Time.deltaTime;
		}
		else if (delayBeforeSecondBoingTimer <= 0)
		{
			starBoing02.gameObject.SetActive(true);
			delayBeforeSecondBoingTimer = 120;
		}
	}

	public void FadeOutStar()
	{
		DeactivateCollider();
		StartCoroutine(FadeOutStarWork());
	}

	protected IEnumerator FadeOutStarWork()
	{
		Color color = startColor;
		Color specularColor = startSpecularColor;
		Color emissionColor = startEmissionColor;

		float alpha = 1;
		while (alpha > 0)
		{
			alpha = Mathf.Max(0, alpha - (Time.deltaTime / 2f));
			color.a = alpha;
			specularColor.a = alpha;

			properties.SetColor("_Color", color);
			properties.SetColor("_SpecColor", specularColor);
			properties.SetColor("_EmissionColor", emissionColor * new Color(alpha , alpha, alpha));

			starRenderer.SetPropertyBlock(properties);
			yield return null;
		}
	}

	public void UpdateLayerToSendStar()
	{
		gameObject.layer = LayerMask.NameToLayer("SentStars");
	}

	public void UpdateLayerToStar()
	{
		gameObject.layer = LayerMask.NameToLayer("Stars");
	}

	public void DeactivateCollider()
	{
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }
	}

	public void StopMovement()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (GetComponent<StarPatternManager>() != null)
        {
            GetComponent<StarPatternManager>().active = false;
        }
	}

	public void StartMovement(float speedModifier = 1)
	{
		Vector3 direction = new Vector3(0, -1, 0);
		StartMovement(direction, speedModifier);
	}

	public void StartMovement(Vector3 direction, float speedModifier = 1)
	{
		float speed = starSpeed * speedModifier;

		//Enable Collider
		if (GetComponent<Collider>() != null)
			GetComponent<Collider>().enabled = true;

		GetComponent<Rigidbody>().velocity = direction * speed;

		//else
		//{
		//    //custom speed and direction
		//    if (GetComponent<Collider>() != null)
		//    {
		//        GetComponent<Collider>().enabled = true;
		//    }
		//    GetComponent<Rigidbody>().velocity = customSpeed * customSpeedDirection;
		//}

		//if (GetComponent<StarPatternManager>() != null)
		//{
		//    GetComponent<StarPatternManager>().active = true;
		//}
	}

	void OnTriggerEnter(Collider coll)
	{
        //Instantiate(starHitCometParticle, transform.position, Quaternion.identity);
		starTriggered.Invoke();
	}

	//this function shrinkles the failed constellation stars
	public void Shrinkle()
	{

		doShrink = true;

	}

    public void DoBoing()
    {
        starBoing.gameObject.SetActive(true);
        delayBeforeSecondBoingTimer = delayBeforeSecondBoingTimerBase;

        //theAnimator.GetComponent<Animator>().SetBool("DoQuickGrowThenShrink", true);
    }

    public void DoGotHitAnim()
    {
        theAnimator.GetComponent<Animator>().SetBool("DoQuickGrowThenShrink", true);
    }

    //there are some values on start that need to be refreshed if spawned from a star pattern emitter (it uses custom speed)
    public void RefreshCustomSpeedValues()
    {
        StartMovement();
    }
}
