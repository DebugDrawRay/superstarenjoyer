using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class StarController : MonoBehaviour
{
	//public enum starType;

	public GameData.StarType theStarType;
	public UnityEvent starTriggered;
	public float destroyStarWhenBelowThisYValue;
	private float starSpeed;

	[HideInInspector]
	public GameData.Star starData;

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
    public Animator theAnimator;

    public bool hasBeenTouched;

	void Awake()
	{
		starData = new GameData.Star(this);
		starRenderer = GetComponentInChildren<MeshRenderer>();
		starMaterial = GetComponentInChildren<MeshRenderer>().material;

		startColor = starMaterial.color;
		startEmissionColor = starMaterial.GetColor("_EmissionColor");
		startSpecularColor = starMaterial.GetColor("_SpecColor");
		properties = new MaterialPropertyBlock();

		
		starMaterial.SetFloat("_Mode", 3f);
		starMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
	}

	// Use this for initialization
	void Start()
	{
		starSpeed = Random.Range(GameData.minStarSpeed, GameData.maxStarSpeed);
		StartMovement();
	}

	// Update is called once per frame
	void Update()
	{
		if (gameObject.layer == LayerMask.NameToLayer("Stars"))
		{
			if (transform.position.y < destroyStarWhenBelowThisYValue)
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

	void OnEnable()
	{
		properties.SetColor("_Color", startColor);
		properties.SetColor("_SpecColor", startSpecularColor);
		properties.SetColor("_EmissionColor", startEmissionColor);
		starRenderer.SetPropertyBlock(properties);
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
		GetComponent<Collider>().enabled = false;
	}

	public void StopMovement()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	public void StartMovement(float speedModifier = 1)
	{
		float speed = starSpeed * speedModifier;

		GetComponent<Collider>().enabled = true;
		GetComponent<Rigidbody>().velocity = new Vector3(0, -1, 0) * speed;
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
}
