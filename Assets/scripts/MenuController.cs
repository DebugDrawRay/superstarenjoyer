using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public void LoadScene(string scene)
    {
		Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

	public void Quit()
	{
		Application.Quit();
	}
}
