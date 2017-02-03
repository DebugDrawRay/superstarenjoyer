using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPatternEmitter : MonoBehaviour
{
	public float Delay = 0;
	public StarPatternEmitterMovement[] Movements;

	[HideInInspector]
	public bool Completed = false;

	//Movement
	protected StarPatternEmitterMovement CurrentMovement;
	protected int MovementIndex = -1;
	protected float MovementTime = 0;
	protected Vector3 StartPosition;
	protected Vector3 EndPosition;
	protected Vector3 StartRotation;

	//Star Spawning
	protected float SpawnWaitTime = 1;
	protected float SpawnTime = 0;

	public void Update()
	{
		if (CurrentMovement != null)
		{
			//Move Towards
			float t = CurrentMovement.MovementCurve.Evaluate(MovementTime / CurrentMovement.Lifetime);
			transform.position = Vector3.Lerp(StartPosition, EndPosition, t);

			//Rotate Towards
			Vector3 a = CurrentMovement.RotationAmount / CurrentMovement.Lifetime;
			Vector3 rotation = transform.eulerAngles;
			rotation += a * Time.deltaTime;
			transform.eulerAngles = rotation;

			//Incrament Timers
			MovementTime += Time.deltaTime;
			SpawnTime += Time.deltaTime;

			if (SpawnTime > SpawnWaitTime)
			{
				SpawnTime = 0;
				SpawnStars();
			}

			if (MovementTime > CurrentMovement.Lifetime)
			{
				//Set Final Position
				transform.position = EndPosition;

				//Set Final Rotation
				Vector3 finalRotation = StartRotation;
				finalRotation += CurrentMovement.RotationAmount;
				transform.eulerAngles = finalRotation;

				//Begin Next Movement
				StartNextMovement();
			}
		}
	}

	public void StartEmitter()
	{
		StartCoroutine(ExecuteMovementsWork());
	}

	protected IEnumerator ExecuteMovementsWork()
	{
		yield return new WaitForSeconds(Delay);
		StartNextMovement();
	}

	protected void StartNextMovement()
	{
		if (MovementIndex + 1 < Movements.Length)
		{
			//Incrament Index
			MovementIndex++;
			CurrentMovement = Movements[MovementIndex];

			//Set Inital Values
			StartPosition = transform.position;
			EndPosition = transform.position + CurrentMovement.MovementAmount;
			StartRotation = transform.eulerAngles;
			SpawnWaitTime = 1 / CurrentMovement.EmissionSpeed;

			//Reset Timer
			MovementTime = 0;
			SpawnTime = 0;
		}
		else
		{
			Completed = true;
		}
	}

	protected void SpawnStars()
	{
		if (StarManager.Instance != null)
		{
			for (int i = 0; i < CurrentMovement.EmissionDirections.Length; i++)
			{
				//Vector3 direction = CurrentMovement.EmissionDirections[i];
				Vector3 direction = transform.localRotation * CurrentMovement.EmissionDirections[i];
				Vector3 position = transform.position;
				StarManager.Instance.SpawnObject(position, direction);
			}
		}
	}
}

[System.Serializable]
public class StarPatternEmitterMovement
{
	[Header("Movement")]
	public Vector3 MovementAmount;
	public Vector3 RotationAmount;
	public AnimationCurve MovementCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float Lifetime = 1;

	[Header("Emission")]
	public float EmissionSpeed;
	public Vector3[] EmissionDirections;
}
