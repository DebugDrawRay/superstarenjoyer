using UnityEngine;
using System.Collections;
using InControl;

public class ControllerInputManager : MonoBehaviour
{
	public static ControllerInputManager Instance;

	void Awake()
	{
		Instance = this;
	}

	public void VibrateController(float intensity, float time)
	{
		InputDevice controller = InputManager.ActiveDevice;
		if (controller != null)
		{
			controller.Vibrate(intensity);
			StartCoroutine(StopVibration(controller, time));
		}
	}

	protected IEnumerator StopVibration(InputDevice controller, float time)
	{
		yield return new WaitForSeconds(time);
		Debug.Log("Stopping Vibration");
		controller.StopVibration();
	}
}
