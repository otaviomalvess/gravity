using System.Collections;
using UnityEngine;

public class GameControl : MonoBehaviour
{
	#region Serialize
		[SerializeField] Player 	pl; 		// Player
		[SerializeField] GameObject map; 		// Game map
		[SerializeField] Room 		curRoom; 	// The room the player is current in
		[SerializeField] Canvas		canvas;
	#endregion

	#region Vars
		bool isPaused = true;
	#endregion

	Transform[] roomFragilePlatforms;

	void Awake()
	{
		DisableRooms();
		Pause();
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

	public void LoadNextRoom(Room nextRoom, Transform nextCheckpoint)
	{
		nextRoom.gameObject.SetActive(true);
		pl.SetCheckpoint(nextCheckpoint);
		StartCoroutine(WaitCamPan(nextRoom));
	}

	IEnumerator WaitCamPan(Room nextRoom)
	{
		yield return new WaitForSeconds(.5f);

		curRoom.gameObject.SetActive(false);
		curRoom = nextRoom;
	}

	public void ReloadRoom()
	{
		curRoom.ReloadRoom();
	}

	public void UpdateRoom(Vector2 dir)
	{
		curRoom.UpdatePlatformTrigger(dir);
	}

	public void Pause()
	{
		isPaused 		= !isPaused;
		pl.IsPaused 	= isPaused;
		Time.timeScale 	= isPaused ? 0f : 1f;
		canvas.gameObject.SetActive(isPaused);
	}
}
