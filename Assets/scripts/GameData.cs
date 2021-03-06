﻿using UnityEngine;
using System;
using System.Collections.Generic;

public static class GameData
{
	public enum StarType { Star, Circle, Triangle, Square, None };

	[Serializable]
	public class Star
	{
		//Data Objects
		public Guid StarId;
		public StarType Type;
		public Vector3 Position;
		public List<Guid> LinkedStars;

		//Game Objects
		[NonSerialized]
		public StarController Controller;

		public Star(StarController controller = null)
		{
			StarId = Guid.NewGuid();
			LinkedStars = new List<Guid>();
			Controller = controller;
		}
	}

	[Serializable]
	public class Link
	{
		//Data Objects
		public List<Guid> StarIds;
		public Vector3 StartPos;
		public Vector3 EndPos;

		//Game Objects
		public LineRenderer LineComponent;

		public Link()
		{
			StarIds = new List<Guid>(); 
		}
	}

	public class Constellation
	{
		public string ConstellationName;
		public GameObject ConstellationParent;
		public Dictionary<Guid, Star> Stars;
		public List<Link> Links;
		public Sprite Background;
	}

    //Constants 
    public const float cometStartY = 10;
    public const float cometDest = -5f;
    public const int minimumStars = 3;

    //Star Speed
    public const float minStarSpeed = 1;
    public const float maxStarSpeed = 3;

    //Star timing
	 public static readonly Vector2[] starSpawnTimers = { new Vector2(0.4f, 0.7f), new Vector2(0.4f, 0.6f), new Vector2(0.3f, 0.5f), new Vector2(0.2f, 0.4f), new Vector2(0.1f, 0.3f), new Vector2(0.1f, 0.2f), };

    //Star spawn
    public const float fieldSize = 4;
    public const float starSize = .5f;
    public const float starSpawnY = 15;

    //Comet parameters
    public const float cometAcceleration = .003f;
    public const float accelerationIncreaseRate = 100;
    public static readonly float[] cometAcelerationLevels = { .2f, .3f, 0.4f, 0.5f, 0.6f, 0.7f };
    public const float dangerLimit = 0f;
	public const float cometBoostMultiplier = 5f;
	public const float cometBoostTimerAdd = 1f;

	//Level Stuff
	public const float levelTime = 30f;

	//Star power
	 public const float baseStrength = 0.6f;
    public const float strengthMultiplier = 0.4f;
    public const float cometCollisionSpeed = 0.8f;

    //Scoring
    public const int scorePerStar = 10;
    public const float scoreConnectionMulti = .1f;
    public const float constSizeMulti = .25f;

    public const float distanceScalar = 1000;
    public const float speedScalar = 1000;

    //Start Sequence
    public const float cometStartAccel = 2f;
    public const float playerStartAccel = 1f;

    //player
    public const float playerStartY = 0;

    //Constellation parameters
    public const float sendSpeed = 2.5f;
}
