using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
	[SerializeField] BoxCollider2D trigger;

	float 	timeForCollapse 	= 2f;
	bool 	isCollapsing 		= false;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.layer != 9) return;

		isCollapsing = true;
		StartCoroutine(Collapsing());
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.layer != 9 && !isCollapsing) return;

		StopCoroutine(Collapsing());
		Collapse();
	}

	IEnumerator Collapsing()
	{
		yield return new WaitForSeconds(timeForCollapse);
		Collapse();
	}

	void Collapse()
	{
		gameObject.SetActive(false);
	}

	public void ChangeTriggerPosition(Vector2 dir)
	{
		if (transform.rotation.z != 0f) {
			if 		(dir == Vector2.down) 	dir = Vector2.left;
			else if (dir == Vector2.left) 	dir = Vector2.up;
			else if (dir == Vector2.up) 	dir = Vector2.right;
			else if (dir == Vector2.right) 	dir = Vector2.down;
		}

		if (dir == Vector2.up)
		{
			trigger.offset 	= new Vector2(0f, -.5f);
			trigger.size 	= new Vector2(1.8f, .2f);
		} 
		else if (dir == Vector2.down)
		{
			trigger.offset 	= new Vector2(0f, .5f);
			trigger.size 	= new Vector2(1.8f, .2f);
		}
		else if (dir == Vector2.right)
		{
			trigger.offset 	= new Vector2(-1f, 0f);
			trigger.size 	= new Vector2(.2f, .8f);
		}
		else if (dir == Vector2.left)
		{
			trigger.offset 	= new Vector2(1f, 0f);
			trigger.size 	= new Vector2(.2f, .8f);
		}
	}
}
