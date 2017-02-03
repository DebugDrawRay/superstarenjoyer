using UnityEngine;
using System;
using System.Collections.Generic;

public class Constellation
{
	public Guid ConstellationId;
	public string ConstellationName;
	public GameObject ConstellationParent;
	public Sprite Background;

	protected Dictionary<Guid, Star> Stars;
	protected Dictionary<Guid, StarLink> Links;

	public int StarCount
	{
		get
		{
			return Stars.Count;
		}
	}

	public int LinkCount
	{
		get
		{
			return Links.Count;
		}
	}

	public Constellation()
	{
		ConstellationId = Guid.NewGuid();
		Stars = new Dictionary<Guid, Star>();
		Links = new Dictionary<Guid, StarLink>();
	}

	public void AddStar(Guid starId, Star star)
	{
		if (!Stars.ContainsKey(starId))
		{
			Stars.Add(starId, star);
		}
	}

	public Star GetStar(Guid starId)
	{
		if (Stars.ContainsKey(starId))
		{
			return Stars[starId];
		}
		return null;
	}

	public List<Guid> GetStarIds()
	{
		return new List<Guid>(Stars.Keys);
	}

	public List<Star> GetAllStars()
	{
		return new List<Star>(Stars.Values);
	}

	public bool ContainsStar(Guid starId)
	{
		return Stars.ContainsKey(starId);
	}

	public void RemoveStar(Guid starId)
	{
		Stars.Remove(starId);
	}

	public void AddStarLink(Guid linkId, StarLink link)
	{
		if (!Links.ContainsKey(linkId))
		{
			Links.Add(linkId, link);
		}
	}

	public StarLink GetStarLink(Guid linkId)
	{
		if (Links.ContainsKey(linkId))
		{
			return Links[linkId];
		}
		return null;
	}

	public List<Guid> GetStarLinkIds()
	{
		return new List<Guid>(Links.Keys);
	}

	public void RemoveStarLink(Guid linkId)
	{
		Links.Remove(linkId);
	}
}
