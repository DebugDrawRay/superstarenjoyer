using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ConstellationManager : MonoBehaviour
{
    public static ConstellationManager Instance;

    [Header("Constellation Things")]
    public Transform ConstellationDisplayParent;

	[Header("Constellation Naming")]
    public TextAsset ConstellationNounsText;
    public TextAsset ConstellationAdjectivesText;
    protected string[] ConstellationNouns;
    protected string[] ConstellationAdjectives;

	[Header("Constellation Data")]
    public ConstellationsData ConstellationData;

	//Colors
    protected Color ConstellationBackgroundColor = new Color(1, 1, 1, 0.3f);
    protected Color ConstellationDisplayBackgroundColor = new Color(1, 1, 1, 0.8f);

    [Header("Scene Stuff")]
    public GameObject LinkPrefab;
    protected float Speed = 2;

	//Invincibility
    protected float InvincibilityCountdown = 0f;
    protected float InvincibiltyCountdownMax = 1f;
  
	[Header("Prefabs")]
	//added by Logan
	public GameObject starHitCometParticle;
	public GameObject scorePopup;//for the whole constellation
    public GameObject linkScorePopup;

	//CONSTELLATION MANAGEMENT
	protected List<Constellation> Constellations;
	protected Constellation CurrentConstellation;
	protected Guid? LastStarId;

	void Awake()
    {
        Instance = this;
        LastStarId = null;

        ConstellationNameSetup();
    }

    void Start()
    {
        Constellations = new List<Constellation>();
    }

    public int AddStar(Star star)
    {
		if (CurrentConstellation == null)
		{
			CreateConstellation();
		}

        //If star not already stored
        if (!CurrentConstellation.ContainsStar(star.StarId))
        {
            CurrentConstellation.AddStar(star.StarId, star);
            star.Controller.starScorePopup.GetComponent<Renderer>().enabled = true;
            star.Controller.starScorePopup.GetComponent<Animator>().enabled = true;
        }

        //There is a last star and it's not the same as the last star
        if (LastStarId != null && star.StarId != LastStarId)
        {
            Star lastStar = CurrentConstellation.GetStar((Guid)LastStarId);

			//Is not currently linked to star
			if (!lastStar.IsLinkedToStar(star.StarId) || !star.IsLinkedToStar(lastStar.StarId))
            {
				CreateLink(CurrentConstellation, star, lastStar);
                InvincibilityCountdown = InvincibiltyCountdownMax;

                if (CurrentConstellation.StarCount > 1 && lastStar != star)
                {
					//1st star should not make a link sound
					//make sure you're not running into the same star you're already attached to
                    AudioController.Instance.PlaySfx(SoundBank.SoundEffects.NewLink);
                }
            }
			else
			{
				Debug.Log("Stars were previously linked");
			}
        }
		else
		{
			if (LastStarId != null)
			{
				Debug.Log("Same star. No link");
			}
			else
			{
				Debug.Log("No last star. No link");
			}
		}

		AudioController.Instance.PlayStarObtainedSFX(CurrentConstellation.StarCount);

		LastStarId = star.StarId;
		return CurrentConstellation.StarCount;
    }

	protected void CreateConstellation()
	{
		CurrentConstellation = new Constellation();
		CurrentConstellation.ConstellationParent = new GameObject("Constellation");
	}

	public void CreateLink(Constellation constellation, Star star, Star lastStar)
	{
		//Error Checking - make sure links are being properly linked to both stars
		if (lastStar.IsLinkedToStar(star.StarId) != star.IsLinkedToStar(lastStar.StarId))
			Debug.Log("Previous Linkes weren't properly created");

		//Create Link Object
		GameObject linkObject = Instantiate(LinkPrefab);
		StarLink starLink = new StarLink(linkObject, star.Position, lastStar.Position);
		linkObject.transform.SetParent(CurrentConstellation.ConstellationParent.transform);
		constellation.AddStarLink(starLink.LinkId, starLink);

		//Link Stars to Eachother
		lastStar.AddStarLink(star.StarId, starLink.LinkId);
		star.AddStarLink(lastStar.StarId, starLink.LinkId);

		//Score popup between links
		if (CurrentConstellation.StarCount > 1)
		{
			if (lastStar.Position != star.Position)//don't spawn a popup score unless there is actually a link
			{
				Vector3 scorePos = lastStar.Position + (star.Position - lastStar.Position) / 2;
				scorePos = new Vector3(scorePos.x + 0.25f, scorePos.y, scorePos.z);
				GameObject a = Instantiate(linkScorePopup, scorePos, Quaternion.identity) as GameObject;
				a.transform.parent = linkObject.transform;
			}
		}
	}

	public bool CompleteConstellation()
    {
        if (CurrentConstellation != null && CurrentConstellation.StarCount >= GameData.minimumStars)
        {
            LastStarId = null;
			Constellation completedConstellation = CurrentConstellation;
			CurrentConstellation = null;

			//Create Background
            GameObject constellationBackgorund = new GameObject("Background");
            constellationBackgorund.transform.SetParent(completedConstellation.ConstellationParent.transform);
            constellationBackgorund.transform.position = GetAverageStarPosition(completedConstellation.GetAllStars());
            SpriteRenderer bgSpriteRenderer = constellationBackgorund.AddComponent<SpriteRenderer>();
            bgSpriteRenderer.sprite = GetRandomConstellationImage();
            bgSpriteRenderer.color = ConstellationBackgroundColor;

			//Create Name
            string constellationName = GenerateConstellationName(completedConstellation.StarCount);
			completedConstellation.ConstellationName = constellationName;
            Debug.Log("Constellation Name:" + constellationName);

			//Move stars to separate layer and set up for comet collisioin
			List<Guid> keys = completedConstellation.GetStarIds();
            for (int i = 0; i < keys.Count; i++)
            {
                Guid key = keys[i];
                Star star = completedConstellation.GetStar(key);
                star.Controller.UpdateLayerToSendStar();
                star.Controller.starTriggered.AddListener(() => StarToCometCollision(completedConstellation, key));
                star.Controller.gameObject.transform.SetParent(completedConstellation.ConstellationParent.transform);
            }

            //Send data to Visualization and score
            //Score
            int score = GameData.scorePerStar;
            float totalConnections = 1 + (GameData.scoreConnectionMulti * completedConstellation.LinkCount);
            float totalStars = 1 + (GameData.constSizeMulti * completedConstellation.StarCount);
            int myScore = (int)((score * totalConnections) * totalStars);//calculate the score
            GameController.TriggerAddScore(myScore);//send it

			//Spawn Score Popups
            //added by Logan
            GameObject a = Instantiate(scorePopup, Vector3.zero, Quaternion.identity) as GameObject;//spawn score text and position it in the middle of the constellation
            a.GetComponent<TextMesh>().text = (myScore).ToString();//make it the correct score amount

			//Different Score than above? Re-Evaluate
            UiController.TriggerScoreData(completedConstellation.StarCount, completedConstellation.LinkCount, score, constellationName);

			//Constellation Created Event
            UiController.TriggerConstellationEvent(completedConstellation.ConstellationParent);

            //Create Mini
            CreateDisplayConstellation(completedConstellation);

			//Throw Constellation Toward Comet
            Tweener tween = completedConstellation.ConstellationParent.transform.DOMoveY(GameData.cometStartY * 2, GameData.sendSpeed).SetEase(Ease.InOutBack);
            tween.OnComplete(() => DestroyConstellation(completedConstellation));

			//Play SFX
            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationComplete);
            AudioController.Instance.PlayAtEnd(SoundBank.SoundEffects.ConstellationComplete, SoundBank.SoundEffects.ConstellationSent, false);

            return true;
        }
        else
        {
            return false;
        }
    }

    protected void BreakStarLinks(Constellation constellation, Guid starId)
    {
		var star = constellation.GetStar(starId);

		//Remove star from linked stars
		List<Guid> linkedStars = star.GetLinkedStarIds();
        for (int i = 0; i < linkedStars.Count; i++)
        {
			Star linkedStar = constellation.GetStar(linkedStars[i]);
			if (linkedStar != null)
			{
				//Destroy Link Object
				Guid? linkId = star.GetStarLinkId(linkedStars[i]);
				if (linkId != null)
				{
					StarLink link = constellation.GetStarLink((Guid)linkId);
					if (link != null)
					{
						Destroy(link.LineGameObject);
					}

					//Remove star from reference
					linkedStar.RemoveLinkedStar(star.StarId);
				}
			}
        }

		//Return to original layer
		star.Controller.UpdateLayerToStar();

		//Remove star from constellation
		constellation.RemoveStar(starId);

		PooledObject pool = star.Controller.gameObject.GetComponent<PooledObject>();
		pool.ReturnToPool();
	}

    public void BreakCurrentConstellation()
    {
        LastStarId = null;

		if (CurrentConstellation != null)
		{
			//Release Stars
			var keys = CurrentConstellation.GetStarIds();
			for (int i = 0; i < keys.Count; i++)
			{
				Star star = CurrentConstellation.GetStar(keys[i]);
				star.Controller.StartMovement(1.4f);
				star.Controller.FadeOutStar();
			}

			//Destroy Links
			//DestroyConstellationLinks(CurrentConstellation);
			Destroy(CurrentConstellation.ConstellationParent);

			AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationBroken);
			CurrentConstellation = null;
		}
    }

	protected void DestroyConstellation(Constellation constellation)
	{
		//Destroy Stars
		List<Guid> starIds = constellation.GetStarIds();
		for (int i = 0; i < starIds.Count; i++)
		{
			Star star = constellation.GetStar(starIds[i]);
			if (star.Controller != null)
			{
				//Shouldn't be destroying them
				//Destroy(star.Controller.gameObject);
				star.Controller.gameObject.SetActive(false);
			}
		}

		//Destroy Links
		//DestroyConstellationLinks(constellation);
		Destroy(constellation.ConstellationParent);
	}

    protected void StarToCometCollision(Constellation constellation, Guid starId)
    {
        if (constellation.ContainsStar(starId))
        {
            Star star = constellation.GetStar(starId);

            //Explosion
            Vector3 particlePosition = star.Controller.gameObject.transform.position;
            particlePosition.y += 1f;
            Instantiate(starHitCometParticle, particlePosition, Quaternion.identity);

            //Pushback
            float strength = GameData.baseStrength + (GameData.strengthMultiplier * star.LinkCount);
            GameController.TriggerCometCollision(strength, GameData.cometCollisionSpeed);

            BreakStarLinks(constellation, starId);
            if (constellation.StarCount <= 0)
            {
                Destroy(constellation.ConstellationParent);
            }
            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationHit);

        }
    }

    protected Vector2 GetAverageStarPosition(List<Star> stars)
    {
        float minX = stars[0].Position.x;
        float minY = stars[0].Position.y;
        float maxX = stars[0].Position.x;
        float maxY = stars[0].Position.y;

        for (int i = 1; i < stars.Count; i++)
        {
            var starPos = stars[i].Position;
            minX = Mathf.Min(minX, starPos.x);
            minY = Mathf.Min(minY, starPos.y);
            maxX = Mathf.Max(maxX, starPos.x);
            maxY = Mathf.Max(maxY, starPos.y);
        }

        return new Vector2(((float)minX + (float)maxX) / 2, ((float)minY + (float)maxY) / 2);
    }


    #region Constellation Name Generation

    protected void ConstellationNameSetup()
    {
        ConstellationNouns = ConstellationNounsText.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        ConstellationAdjectives = ConstellationAdjectivesText.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    protected string GenerateConstellationName(int starCount)
    {
        int wordCount = Mathf.Max(1, Mathf.FloorToInt(starCount / 2));
        string finalName = "The ";
        for (int i = 0; i < wordCount - 1; i++)
        {
            finalName += ConstellationAdjectives[UnityEngine.Random.Range(0, ConstellationAdjectives.Length)] + " ";
        }
        finalName += ConstellationNouns[UnityEngine.Random.Range(0, ConstellationNouns.Length)];
        return finalName;
    }

    protected Sprite GetRandomConstellationImage()
    {
        return ConstellationData.ConstellationSprites[UnityEngine.Random.Range(0, ConstellationData.ConstellationSprites.Length)];
    }

    #endregion

    #region Workshop

    protected void CreateDisplayConstellation(Constellation original)
    {
        //Remove previous constellations
        for (int i = 0; i < ConstellationDisplayParent.childCount; i++)
        {
            Destroy(ConstellationDisplayParent.GetChild(0).gameObject);
        }

        //Duplicate Constellation
        GameObject duplicateConstellation = Instantiate(original.ConstellationParent);
        for (int i = 0; i < duplicateConstellation.transform.childCount; i++)
        {
            var child = duplicateConstellation.transform.GetChild(i);
            var poolComp = child.GetComponentInChildren<PooledObject>();
            Destroy(poolComp);

            child.tag = "DisplayStar";

            var starComp = child.GetComponentInChildren<StarController>();
            Destroy(starComp);

            var lineComp = child.GetComponentInChildren<LineRenderer>();
			if (lineComp != null)
			{
				lineComp.SetWidth(0.1f, 0.1f);
			}
        }

        //Up opacity on background
        SpriteRenderer background = duplicateConstellation.transform.FindChild("Background").GetComponent<SpriteRenderer>();
        background.color = ConstellationDisplayBackgroundColor;

        Vector3 center = GetAverageStarPosition(original.GetAllStars());

        duplicateConstellation.transform.SetParent(ConstellationDisplayParent);
        duplicateConstellation.transform.localScale = Vector3.one;
        duplicateConstellation.transform.localPosition = Vector3.zero - center;

        UiController.TriggerConstellationFadeEvent(original.ConstellationName);
        StartCoroutine(DisplayConstellationSuicide(duplicateConstellation));
    }

    protected IEnumerator DisplayConstellationSuicide(GameObject constellation)
    {
        yield return new WaitForSeconds(6.5f);
        if (constellation != null)
            Destroy(constellation);
    }

    #endregion
}
