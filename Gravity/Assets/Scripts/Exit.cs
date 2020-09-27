using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Exit : MonoBehaviour
{
	public Gridmap 		nextRoom;
	public UnityEvent 	OnLoadEvent;

	void Awake()
	{
		if (OnLoadEvent == null) OnLoadEvent = new UnityEvent();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.layer != 9) return;

		OnLoadEvent.Invoke();
	}
}
