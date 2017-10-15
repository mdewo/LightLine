using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(LoadDemoScene());
	}

	IEnumerator LoadDemoScene()
	{
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene("DemoScene");
	}
}