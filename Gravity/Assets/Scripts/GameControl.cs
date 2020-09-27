using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class GameControl : MonoBehaviour
{
	[SerializeField] Player 				pl; 		// Player
	[SerializeField] CinemachineConfiner 	cam;
	[SerializeField] GameObject 			map; 		// Game map
	[SerializeField] Gridmap 				curRoom; 	// The room the player is current in

	void Awake()
	{
		DisableRooms();
	}

	void DisableRooms()
	{
		// Except the current
		foreach (Transform child in map.transform)
		{
			if (child.gameObject == curRoom.gameObject) continue;

			child.gameObject.SetActive(false);
		}
	}

	public void LoadNextRoom()
	{	
		// Room
		curRoom.gameObject.SetActive(false);
		curRoom = curRoom.roomExit.nextRoom;
		curRoom.gameObject.SetActive(true);

		// Player
		pl.SetCheckpoint(curRoom.checkpoint);
		
		// Camera
        cam.m_BoundingShape2D = curRoom.bounds;
        cam.InvalidatePathCache();
	}
}
