using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Exit : MonoBehaviour
{
	[SerializeField] Room 		nextRoom;
	[SerializeField] Transform 	nextCheckpoint;

	public class 	LoadNextRoomEvent : UnityEvent<Room, Transform> {}
	public 			LoadNextRoomEvent OnLoadEvent = new LoadNextRoomEvent();

	void Start()
	{
		GameControl gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
		OnLoadEvent.AddListener(gc.LoadNextRoom);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.layer != 9) return;

		OnLoadEvent.Invoke(nextRoom, nextCheckpoint);
	}
}
