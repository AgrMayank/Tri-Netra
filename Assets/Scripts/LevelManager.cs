using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

	public float autoLoadLevelTime;

    void Start()
    {
		if(autoLoadLevelTime == 0)
		{
			Debug.Log ("Level auto load disabled.");
		}
		else
		{
			Invoke ("LoadNextLevel", autoLoadLevelTime);
		}
	}

	public void LoadLevel(string name)
	{
		Debug.Log ("New Level Load : " + name);
		SceneManager.LoadScene (name);
	}

	public void QuitRequest ()
	{
		Debug.Log ("Quit Requested!");
		Application.Quit ();
	}

	public void LoadNextLevel ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
	}
}
