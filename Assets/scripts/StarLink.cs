using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class StarLink : MonoBehaviour
{
	//Data Objects
	public Guid LinkId;

	//Game Objects
	public GameObject LineGameObject;
	public LineRenderer LineComponent;

	public StarLink(GameObject go, Vector3 startPosition, Vector3 endPosition)
	{
		//StarIds = new List<Guid>();
		LinkId = new Guid();
		LineGameObject = go;

		LineComponent = go.GetComponent<LineRenderer>();
		LineComponent.useWorldSpace = false;

		LineComponent.SetPosition(0, new Vector3(startPosition.x, startPosition.y, 1f));
		LineComponent.SetPosition(1, new Vector3(endPosition.x, endPosition.y, 1f));
	}
}
