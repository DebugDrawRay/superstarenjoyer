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
    public TextAsset ConstellationNounsText;
    public TextAsset ConstellationAdjectivesText;
    protected string[] ConstellationNouns;
    protected string[] ConstellationAdjectives;
    public ConstellationsData ConstellationData;
    protected Color ConstellationBackgroundColor = new Color(1, 1, 1, 0.3f);
    protected Color ConstellationDisplayBackgroundColor = new Color(1, 1, 1, 0.8f);

    [Header("Scene Stuff")]
    public GameObject LinkPrefab;
    public Transform StarLinkParent;
    protected float Speed = 2;

    protected float InvincibilityCountdown = 0f;
    protected float InvincibiltyCountdownMax = 1f;

    protected List<GameData.Constellation> Constellations;
    protected Dictionary<Guid, GameData.Star> Stars;
    protected Guid? LastStarId;
    protected List<GameData.Link> Links;

    public GameObject player;

    public GameObject starHitCometParticle;

    //added by Logan
    public GameObject scorePopup;

    void Awake()
    {
        Instance = this;
        Stars = new Dictionary<Guid, GameData.Star>();
        Links = new List<GameData.Link>();
        LastStarId = null;

        ConstellationNameSetup();

        print(gameObject.name);
    }

    void Start()
    {
        Constellations = new List<GameData.Constellation>();
    }

    protected void CheckLink(GameData.Link link)
    {
        RaycastHit hit;

        if (Physics.Linecast(link.StartPos, link.EndPos, out hit))
        {
            if (hit.transform.gameObject.name.Contains("Player"))
            {
                BreakConstellation();
            }
        }
    }

    public void AddStar(GameData.Star star)
    {
        //If star not already stored
        if (!Stars.ContainsKey(star.StarId))
        {
            Stars.Add(star.StarId, star);
        }

        //There is a last star
        if (LastStarId != null)
        {
            GameData.Star lastStar = Stars[(Guid)LastStarId];

            //Is not currently linked to star
            if (!lastStar.LinkedStars.Contains(star.StarId) || !star.LinkedStars.Contains(lastStar.StarId))
            {
                //Error Checking - make sure links are being properly linked to both stars
                if (lastStar.LinkedStars.Contains(star.StarId) != star.LinkedStars.Contains(lastStar.StarId))
                    Debug.Log("Previous Linkes weren't properly created");

                //Link Stars to Eachother
                if (!lastStar.LinkedStars.Contains(star.StarId))
                    lastStar.LinkedStars.Add(star.StarId);
                if (!star.LinkedStars.Contains(lastStar.StarId))
                    star.LinkedStars.Add(lastStar.StarId);

                //Create Link
                var link = new GameData.Link();
                link.StarIds.Add(lastStar.StarId);
                link.StarIds.Add(star.StarId);

                //Create Link Object
                var linkObject = Instantiate(LinkPrefab);
                var line = linkObject.GetComponent<LineRenderer>();
                line.useWorldSpace = false;

                if (StarLinkParent != null)
                {
                    linkObject.transform.SetParent(StarLinkParent);
                    linkObject.transform.localScale = Vector3.one;
                }

                link.LineComponent = line;
                link.StartPos = lastStar.Position;
                link.EndPos = star.Position;
                line.SetPosition(0, new Vector3(link.StartPos.x, link.StartPos.y, 1f));
                line.SetPosition(1, new Vector3(link.EndPos.x, link.EndPos.y, 1f));

                Links.Add(link);
                InvincibilityCountdown = InvincibiltyCountdownMax;
            }
        }

        LastStarId = star.StarId;
    }

    public bool CompleteConstellation()
    {
        Debug.Log("Complete Constellation");
        if (Stars.Count >= GameData.minimumStars)
        {
            LastStarId = null;
            var constellation = new GameData.Constellation();
            constellation.Stars = new Dictionary<Guid, GameData.Star>(Stars);
            constellation.Links = new List<GameData.Link>(Links);

            GameObject constellationParent = new GameObject("Constellation");
            constellation.ConstellationParent = constellationParent;

            GameObject constellationBackgorund = new GameObject("Background");
            constellationBackgorund.transform.SetParent(constellationParent.transform);
            constellationBackgorund.transform.position = GetAverageStarPosition(new List<GameData.Star>(constellation.Stars.Values).ToArray());
            SpriteRenderer bgSpriteRenderer = constellationBackgorund.AddComponent<SpriteRenderer>();
            bgSpriteRenderer.sprite = GetRandomConstellationImage();
            bgSpriteRenderer.color = ConstellationBackgroundColor;


            string constellationName = GenerateConstellationName(Stars.Keys.Count);
            constellation.ConstellationName = constellationName;
            Debug.Log("Constellation Name:" + constellationName);


            var keys = new List<Guid>(constellation.Stars.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                Guid key = keys[i];
                GameData.Star star = constellation.Stars[key];
                star.Controller.UpdateLayerToSendStar();
                star.Controller.starTriggered.AddListener(() => StarToCometCollision(constellation, key));
                star.Controller.gameObject.transform.SetParent(constellationParent.transform);
            }

            for (int i = 0; i < constellation.Links.Count; i++)
            {
                constellation.Links[i].LineComponent.gameObject.transform.SetParent(constellationParent.transform);
            }

            //Send data to Visualization and score
            //Score
            int score = GameData.scorePerStar;
            float totalConnections = 1 + (GameData.scoreConnectionMulti * constellation.Links.Count);
            float totalStars = 1 + (GameData.constSizeMulti * constellation.Stars.Count);
            int myScore = (int)((score * totalConnections) * totalStars);//calculate the score
            GameController.TriggerAddScore(myScore);//send it

            Vector3 myPos = new Vector3(0, 0, 0);

            /*for (int b = 0; b < constellation.Stars.Count; b++)
            {
                myPos = new Vector3(myPos.x - constellation.Stars[b].Position.x, myPos.y - constellation.Stars[b].Position.y, myPos.z - constellation.Stars[b].Position.z);   
            }*/

            //added by Logan
            GameObject a = Instantiate(scorePopup, myPos, Quaternion.identity) as GameObject;//spawn score text and position it in the middle of the constellation
            a.GetComponent<TextMesh>().text = (myScore).ToString();//make it the correct score amount
            player.GetComponent<PlayerController>().ResetStarsInCurrentConstellation();

            UiController.TriggerScoreData(constellation.Stars.Count, constellation.Links.Count, score, constellationName);

            Stars.Clear();
            Links.Clear();

            UiController.TriggerConstellationEvent(constellationParent);
            //Create Mini
            CreateDisplayConstellation(constellation);

            Tweener tween = constellation.ConstellationParent.transform.DOMoveY(GameData.cometStartY * 2, GameData.sendSpeed).SetEase(Ease.InOutBack);
            tween.OnComplete(() => DestroyConstellation(constellation));


            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationComplete);
            AudioController.Instance.PlayAtEnd(AudioController.Instance.effectBus[(int)SoundBank.SoundEffects.ConstellationComplete], SoundBank.Instance.Request(SoundBank.SoundEffects.ConstellationSent), false);
            //StartCoroutine(ConstellationFlyAway(constellation));

            return true;
        }
        else
        {
            return false;
        }
    }

    protected void BreakStarLink(GameData.Constellation constellation, Guid starId)
    {
        var star = constellation.Stars[starId];

        //Remove star from linked stars
        for (int i = 0; i < star.LinkedStars.Count; i++)
        {
            if (constellation.Stars.ContainsKey(star.LinkedStars[i]))
            {
                GameData.Star linkedStar = constellation.Stars[star.LinkedStars[i]];
                linkedStar.LinkedStars.Remove(star.StarId);
            }
        }

        //Get all involved links
        List<GameData.Link> removedLinks = new List<GameData.Link>();
        for (int i = 0; i < constellation.Links.Count; i++)
        {
            if (constellation.Links[i].StarIds.Contains(star.StarId))
                removedLinks.Add(constellation.Links[i]);
        }

        //Destroy Link
        for (int i = 0; i < removedLinks.Count; i++)
        {
            GameData.Link link = removedLinks[i];
            constellation.Stars[link.StarIds[0]].LinkedStars.Remove(constellation.Stars[link.StarIds[1]].StarId);
            constellation.Stars[link.StarIds[1]].LinkedStars.Remove(constellation.Stars[link.StarIds[0]].StarId);
            Destroy(link.LineComponent.gameObject);
            constellation.Links.Remove(link);
        }

        //Return to original layer
        star.Controller.UpdateLayerToStar();

        //Remove star from constellation
        constellation.Stars.Remove(starId);
        PooledObject pool = star.Controller.gameObject.GetComponent<PooledObject>();
        pool.ReturnToPool();
    }

    protected void BreakLink(GameData.Link link)
    {
        //Unlink Stars
        Stars[link.StarIds[0]].LinkedStars.Remove(link.StarIds[1]);
        Stars[link.StarIds[1]].LinkedStars.Remove(link.StarIds[0]);

        //Delete Link
        Destroy(link.LineComponent.gameObject);

        CheckStrandedStar(Stars[link.StarIds[0]]);
        CheckStrandedStar(Stars[link.StarIds[1]]);
    }

    public void BreakConstellation()
    {
        LastStarId = null;

        //Release Stars
        var keys = new List<Guid>(Stars.Keys);
        for (int i = 0; i < Stars.Count; i++)
        {
            Stars[keys[i]].Controller.StartMovement(1.4f);
            Stars[keys[i]].Controller.FadeOutStar();
            //Stars[keys[i]].Controller.Shrinkle();
        }

        //Destroy Links
        for (int i = 0; i < Links.Count; i++)
        {
            var link = Links[i];
            Destroy(link.LineComponent.gameObject);
        }

        Stars = new Dictionary<Guid, GameData.Star>();
        Links = new List<GameData.Link>();
        AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationBroken);

    }

    protected void CheckStrandedStar(GameData.Star star)
    {
        if (star.LinkedStars.Count <= 0)
        {
            star.Controller.StartMovement();
            Stars.Remove(star.StarId);
        }
    }

    protected IEnumerator ConstellationFlyAway(GameData.Constellation constellation)
    {
        var yDisplace = 0f;
        while (yDisplace < 100f && constellation.ConstellationParent != null)
        {
            var pos = constellation.ConstellationParent.transform.position;
            pos.y += Time.deltaTime * Speed;
            constellation.ConstellationParent.transform.position = pos;

            yDisplace += Time.deltaTime * Speed;
            yield return null;
        }
        DestroyConstellation(constellation);
    }

    protected void DestroyConstellation(GameData.Constellation constellation)
    {
        //Destroy Stars
        var stars = new List<GameData.Star>(constellation.Stars.Values);
        for (int i = 0; i < stars.Count; i++)
        {
            var star = stars[i];
            if (star.Controller != null)
            {
                Destroy(star.Controller.gameObject);
            }
        }

        //Destroy Links
        for (int i = 0; i < constellation.Links.Count; i++)
        {
            var link = constellation.Links[i];
            Destroy(link.LineComponent.gameObject);
        }
    }

    protected void StarToCometCollision(GameData.Constellation constellation, Guid starId)
    {
        if (constellation.Stars.ContainsKey(starId))
        {
            GameData.Star star = constellation.Stars[starId];

            //Explosion
            Vector3 particlePosition = constellation.Stars[starId].Controller.gameObject.transform.position;
            particlePosition.y += 1f;
            Instantiate(starHitCometParticle, particlePosition, Quaternion.identity);

            //Pushback
            float strength = GameData.baseStrength + (GameData.strengthMultiplier * star.LinkedStars.Count);
            GameController.TriggerCometCollision(strength, GameData.cometCollisionSpeed);

            BreakStarLink(constellation, starId);
            if (constellation.Stars.Count <= 0)
            {
                Destroy(constellation.ConstellationParent);
            }
            AudioController.Instance.PlaySfx(SoundBank.SoundEffects.ConstellationHit);

        }
    }

    protected Vector2 GetAverageStarPosition(GameData.Star[] stars)
    {
        float minX = stars[0].Position.x;
        float minY = stars[0].Position.y;
        float maxX = stars[0].Position.x;
        float maxY = stars[0].Position.y;

        for (int i = 1; i < stars.Length; i++)
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

    protected void CreateDisplayConstellation(GameData.Constellation original)
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
                lineComp.SetWidth(0.1f, 0.1f);
        }

        //Up opacity on background
        SpriteRenderer background = duplicateConstellation.transform.FindChild("Background").GetComponent<SpriteRenderer>();
        background.color = ConstellationDisplayBackgroundColor;

        Vector3 center = GetAverageStarPosition(new List<GameData.Star>(original.Stars.Values).ToArray());

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
