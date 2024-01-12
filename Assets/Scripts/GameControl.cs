using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
	#region Serialize
		[SerializeField] Player 	pl; 				 // Player
		[SerializeField] GameObject map; 				 // Game map
		[SerializeField] Room 		curRoom; 			 // The room the player is current in
		[SerializeField] GameObject canvas;
		[SerializeField] GameObject	pause;
		[SerializeField] Animator 	animDeathTransition; // Player death transition controller
	#endregion

	#region Vars
		bool 	 		isPaused = true;
		MusicController music;
	#endregion

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

	void Start()
	{
		GameObject temp = GameObject.Find("Music");
		if (temp != null) music = temp.GetComponent<MusicController>();
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
		StartCoroutine(Transition());
	}

	IEnumerator Transition()
	{
		animDeathTransition.SetTrigger("start");
		yield return new WaitForSeconds(.5f);
		curRoom.ReloadRoom();
		animDeathTransition.SetTrigger("end");
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
		pause.SetActive(isPaused);
		canvas.GetComponent<PauseMenu>().IsPaused = isPaused;
	}

	public void StartMusic()
	{
		if (music == null) return;
		music.GetComponent<MusicController>().PlayMusic();
	}

	public void End()
	{
		SceneManager.LoadScene(2);
	}
}
