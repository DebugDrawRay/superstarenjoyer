using UnityEngine;
using System.Collections;

public class ConstellationIcon : MonoBehaviour
{
    public Transform parent;
    public float destinationY;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (parent == null)
        {
            Destroy(gameObject);
        }
        else
        {
            float distance = parent.position.y;
            float currentDistance = (distance + Mathf.Abs(GameData.cometDest)) / (GameData.cometStartY + Mathf.Abs(GameData.cometDest));
            currentDistance = Mathf.Clamp(currentDistance, 0, 1) * destinationY;
            rect.anchoredPosition = new Vector2(0, currentDistance);
        }
    }
}
