using UnityEngine;
using System.Collections.Generic;

public class StarPattern : MonoBehaviour
{
	public EmitterInstance[] Emitters;

	[HideInInspector]
	public bool Completed = false;

	protected List<StarPatternEmitter> EmitterControllers;
	protected List<StarPatternEmitter> EmittersToRemove;

	public void Start()
	{
		EmitterControllers = new List<StarPatternEmitter>();
		EmittersToRemove = new List<StarPatternEmitter>();
		for(int i = 0; i < Emitters.Length; i++)
		{
			GameObject prefab = Emitters[i].EmitterPrefab;
			GameObject emitter = Instantiate(prefab, Emitters[i].EmitterStartPosition, Quaternion.identity);
			StarPatternEmitter comp = emitter.GetComponent<StarPatternEmitter>();
			EmitterControllers.Add(comp);
			comp.StartEmitter();
		}
	}

	public void Update()
	{
		if (!Completed)
		{
			if (EmitterControllers.Count > 0)
			{
				EmittersToRemove.Clear();
				for (int i = 0; i < EmitterControllers.Count; i++)
				{
					if (EmitterControllers[i].Completed)
						EmittersToRemove.Add(EmitterControllers[i]);
				}

				for (int i = 0; i < EmittersToRemove.Count; i++)
					EmitterControllers.Remove(EmittersToRemove[i]);
			}
			else
			{
				Completed = true;
			}
		}
	}

	[System.Serializable]
	public class EmitterInstance
	{
		public GameObject EmitterPrefab;
		public Vector3 EmitterStartPosition;
	}
}
