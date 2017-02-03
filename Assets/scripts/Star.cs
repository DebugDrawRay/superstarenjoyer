using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Star : MonoBehaviour
{
	public enum StarType { Star, Circle, Triangle, Square, None };

	//Data Objects
	public Guid StarId;
	public StarType Type;
	public Vector3 Position;
	protected Dictionary<Guid, Guid> StarToLink;

	//Properties
	public int LinkCount
	{
		get
		{
			return StarToLink.Count;
		}
	}

	//Game Objects
	[NonSerialized]
	public StarController Controller;

	public Star(StarController controller)
	{
		StarId = Guid.NewGuid();
		StarToLink = new Dictionary<Guid, Guid>();
		Controller = controller;
	}

	public void ClearLinks()
	{
		StarToLink = new Dictionary<Guid, Guid>();
	}

	public List<Guid> GetLinkedStarIds()
	{
		return new List<Guid>(StarToLink.Keys);
	}

	public bool IsLinkedToStar(Guid starId)
	{
		return StarToLink.ContainsKey(starId);
	}

	public void AddStarLink(Guid starId, Guid linkId)
	{
		if (!StarToLink.ContainsKey(starId))
		{
			StarToLink.Add(starId, linkId);
		}
	}

	public Guid? GetStarLinkId(Guid starId)
	{
		if (StarToLink.ContainsKey(starId))
		{
			return StarToLink[starId];
		}
		return null;
	}

	public void RemoveLinkedStar(Guid starId)
	{
		StarToLink.Remove(starId);
	}
}
