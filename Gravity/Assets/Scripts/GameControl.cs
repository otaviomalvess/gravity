using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameControl : MonoBehaviour
{
	[SerializeField] Player 				pl; 		// Player
	[SerializeField] CinemachineConfiner 	cam;
	[SerializeField] GameObject 			map; 		// Game map
	[SerializeField] Gridmap 				curRoom; 	// The room the player is current in

	public void LoadNextRoom()
	{
		curRoom = curRoom.roomExit.nextRoom;
		pl.SetCheckpoint(curRoom.checkpoint);
		
		// Camera
        cam.m_BoundingShape2D = curRoom.bounds;
        cam.InvalidatePathCache();
	}
}
