using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	#region Public
		public Transform 			checkpoint;
		public Exit 				roomExit;
		public PolygonCollider2D 	bounds;
	#endregion

	#region Private
		GameObject[] fragilePlatforms;
	#endregion

	void Start()
	{
		Transform t = transform.GetChild(5);
		fragilePlatforms = new GameObject[t.childCount];

		int i = 0;
		foreach (Transform child in t)
		{
			fragilePlatforms[i] = child.gameObject;
			i++;
		}
	}
	
	public void ReloadRoom()
	{
		foreach (GameObject o in fragilePlatforms)
		{
			if (o.activeSelf) continue;
			o.SetActive(true);
		}
	}

	public void UpdatePlatformTrigger(Vector2 dir)
	{
		if (fragilePlatforms == null) return;

		foreach (GameObject o in fragilePlatforms)
		{
			o.GetComponent<FragilePlatform>().ChangeTriggerPosition(dir);
		}
	}
}
