using UnityEngine;
using System;
using System.Collections.Generic;

public static class GameData
{
    //Constants 
    public const float cometStartY = 10;
    public const float cometDest = -5f;
    public const int minimumStars = 3;

    //Star Speed
    public const float minStarSpeed = 2;
    public const float maxStarSpeed = 2;

    //Star timing
	 public static readonly Vector2[] starSpawnTimers = { new Vector2(0.5f, 1f), new Vector2(0.5f, 0.9f), new Vector2(0.4f, 0.8f), new Vector2(0.4f, 0.7f), new Vector2(0.3f, 0.6f), new Vector2(0.3f, 0.5f), };

    //Star spawn
    public const float fieldSize = 4;
    public const float starSize = 0.65f;
    public const float starSpawnY = 8;//15;

    //Comet parameters
    public const float cometAcceleration = .003f;
    public const float accelerationIncreaseRate = 100;
	public const float cometAccelerationBase = 0.2f;
	public const float cometAccelerationIncrease = 0.075f;
    public const float dangerLimit = 0f;
  public const float safetyLimit = 2.0f;
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
