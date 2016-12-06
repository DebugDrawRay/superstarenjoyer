using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class DistanceMeter : MonoBehaviour
{
    public GameObject constellationIcon;
    public RectTransform constellationStart;

    public RectTransform comet;
    public RectTransform cometImage;
    public float cometStart;

    public Image cometFire;

    void Start()
    {
        //cometImage.DOShakePosition(.1f, 1, 1000, 100).SetLoops(-1);
    }

    public void ChangeDistance(float distance)
    {
        float currentDistance = (distance + Mathf.Abs(GameData.cometDest)) / (GameData.cometStartY + Mathf.Abs(GameData.cometDest));
        currentDistance = Mathf.Clamp(currentDistance, 0, 1) * cometStart;
        
        comet.anchoredPosition = new Vector2(0, currentDistance);
    }

    public void DisplayConstellation(GameObject cons)
    {
        GameObject newCon = (GameObject)Instantiate(constellationIcon, new Vector2(0, 5000), Quaternion.identity, transform);
        newCon.GetComponent<RectTransform>().anchoredPosition = constellationStart.anchoredPosition;
        newCon.GetComponent<ConstellationIcon>().parent = cons.transform;
        newCon.GetComponent<ConstellationIcon>().destinationY = cometStart;
    }

    public void CometDanger(bool inDanger)
    {
        if(inDanger)
        {
            cometFire.DOColor(Color.white, .25f);
        }
        else
        {
            Color trans = Color.white;
            trans.a = 0;
            cometFire.DOColor(trans, .25f);
        }
    }
}
