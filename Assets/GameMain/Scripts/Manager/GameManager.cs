using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public float playerRebirthTime = 1.5f;

	GameManager instance = null;
	public GameManager Instance { get { return instance; } }

	Player player;
	Vector3 startPos;

	private void Awake()
	{
		instance = this;
		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		player = FindObjectOfType<Player>();
		if (player == null)
			Debug.LogError("Missing player object!");
		startPos = player.transform.position;
	}

	private void OnEnable()
	{
		EventManager.AddListener(GameEventType.onPlayerDead, OnPlayerDead);
	}

	private void OnDisable()
	{
		EventManager.RemoveListener(GameEventType.onPlayerDead, OnPlayerDead);
	}

	void OnPlayerDead()
	{
		StartCoroutine(OnPlayerDeadEnumerator());
	}

	IEnumerator OnPlayerDeadEnumerator()
	{
		yield return new WaitForSeconds(playerRebirthTime);
		player.Rebirth(startPos);
	}
}