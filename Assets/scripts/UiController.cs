using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UiController : MonoBehaviour
{
    public delegate void Message<T>(T data);
    public static Message<int> ScoreEvent;

    public static void TriggerScoreEvent(int score)
    {
        if(ScoreEvent != null)
        {
            ScoreEvent(score);
        }
    }

    public static Message<float> DistanceEvent;

    public static void TriggerDistanceEvent(float distance)
    {
        if (DistanceEvent != null)
        {
            DistanceEvent(distance);
        }
    }

    public static Message<string> KillScreenEvent;
    public static void TriggerKillScreen(string message)
    {
        if(KillScreenEvent != null)
        {
            KillScreenEvent(message);
        }
    }

	public static Message<bool> PauseScreenEvent;
	public static void TriggerPauseScreen(bool display)
	{
		if (PauseScreenEvent != null)
		{
			PauseScreenEvent(display);
		}
	}

	public static Message<float> VelocityEvent;

    public static void TriggerVelocityEvent(float velocity)
    {
        if(VelocityEvent != null)
        {
            VelocityEvent(velocity);
        }
    }

    public static Message<GameObject> ConstellationEvent;
    public static void TriggerConstellationEvent(GameObject cons)
    {
        if(ConstellationEvent != null)
        {
            ConstellationEvent(cons);
        }
    }

    public delegate void ScoreData(int starCount, int linkCount, int score, string name);
    public static event ScoreData ScoreDataEvent;

    public static void TriggerScoreData(int starCount, int linkCount, int score, string name)
    {
        if(ScoreDataEvent != null)
        {
            ScoreDataEvent(starCount, linkCount, score, name);
        }
    }

	public static Message<string> ConstellationFadeEvent;
	public static void TriggerConstellationFadeEvent(string info)
	{
		if (ConstellationFadeEvent != null)
		{
			ConstellationFadeEvent(info);
		}
	}

    public static Message<bool> CometDanger;
    public static void TriggerCometDanger(bool inDanger)
    {
        if(CometDanger != null)
        {
            CometDanger(inDanger);
        }
    }

	public Text scoreDisplay;
    public Text distanceDisplay;
    public Text constellationInfo;
    public DistanceMeter distanceMeter;
    public Text velocityDisplay;

	public RawImage ConstellationDisplay;
	protected IEnumerator ConstellationFadeIn;
	protected IEnumerator ConstellationFadeOut;

    private Tween currentTween;

	[Header("Pause/Kill Screen")]
	public GameObject killScreen;
	public GameObject pauseScreen;
	public Button killScreenDefaultButton;
	public Button pauseScreenDefaultButton;

	void Awake()
    {
        AssignEvents();
    }

    void AssignEvents()
    {
        ScoreEvent += DisplayScore;
        DistanceEvent += DisplayDistance;
        DistanceEvent += distanceMeter.ChangeDistance;
        ScoreDataEvent += AddToScoreFeed;
        KillScreenEvent += DisplayKillScreen;
		PauseScreenEvent += DisplayPauseScreen;
        VelocityEvent += DisplayVelocity;
        ConstellationEvent += distanceMeter.DisplayConstellation;
        ConstellationFadeEvent += FadeInConstellation;
        CometDanger += distanceMeter.CometDanger;
	 }

    void OnDestroy()
    {
        ScoreEvent -= DisplayScore;
        DistanceEvent -= DisplayDistance;
        DistanceEvent -= distanceMeter.ChangeDistance;
        ScoreDataEvent -= AddToScoreFeed;
        KillScreenEvent -= DisplayKillScreen;
		PauseScreenEvent -= DisplayPauseScreen;
        VelocityEvent -= DisplayVelocity;
        ConstellationEvent -= distanceMeter.DisplayConstellation;
        ConstellationFadeEvent -= FadeInConstellation;
        CometDanger -= distanceMeter.CometDanger;

    }

    void DisplayScore(int score)
    {
        scoreDisplay.text = score.ToString();
    }

    void DisplayVelocity(float velocity)
    {
        velocityDisplay.text = (velocity * GameData.speedScalar).ToString() + " M/s";
    }

    void AddToScoreFeed(int starCount, int linkCount, int score, string name)
    {
        if(currentTween != null)
        {
            currentTween.Kill();
        }
        Color clear = Color.white;
        clear.a = 0;
    }

    void DisplayDistance(float distance)
    {
        float total = distance + Mathf.Abs(GameData.cometDest);
        distanceDisplay.text = Mathf.RoundToInt(total * GameData.distanceScalar).ToString() + " M";
    }

    void DisplayKillScreen(string message)
    {
        killScreen.SetActive(true);
		killScreenDefaultButton.Select();
    }

	protected void DisplayPauseScreen(bool display)
	{
		if (display)
		{
			//UnPause
			pauseScreen.SetActive(true);
			pauseScreenDefaultButton.Select();
		}
		else
		{
			pauseScreen.SetActive(false);
		}
	}

	void FadeInConstellation(string info)
	{
		if (ConstellationFadeIn != null)
			StopCoroutine(ConstellationFadeIn);

        constellationInfo.text = info;
		ConstellationFadeIn = FadeInConstellationWork();
		StartCoroutine(ConstellationFadeIn);
	}

	IEnumerator FadeInConstellationWork()
	{
		if (ConstellationFadeOut != null)
			StopCoroutine(ConstellationFadeOut);

		float t = 0;
		Color color = ConstellationDisplay.color;
		Color textColor = constellationInfo.color;
		while (color.a < 1)
		{
			color.a = Mathf.Lerp(0, 1, t);
			textColor.a = Mathf.Lerp(0, 1, t);
			ConstellationDisplay.color = color;
			constellationInfo.color = textColor;
			t += (Time.deltaTime * 2);
			yield return null;
		}

		yield return new WaitForSeconds(5);

		ConstellationFadeOut = FadeOutConstellationWork();
		StartCoroutine(ConstellationFadeOut);
	}

	IEnumerator FadeOutConstellationWork()
	{
		float t = 0;
		Color color = Color.white;
		Color textColor = constellationInfo.color;

		while (color.a > 0)
		{
			color.a = Mathf.Lerp(1, 0, t);
			textColor.a = Mathf.Lerp(1, 0, t);
			ConstellationDisplay.color = color;
			constellationInfo.color = textColor;
			t += Time.deltaTime;
			yield return null;
		}

		yield return null;
	}
}
