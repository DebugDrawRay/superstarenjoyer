using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

	public void Quit()
	{
		Application.Quit();
	}
}
