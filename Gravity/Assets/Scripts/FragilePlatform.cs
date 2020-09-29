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
		gameObject.SetActive(false); // TODO: decide if destroy is better than disabling
	}

	public void ChangeTriggerPosition(Vector2 dir)
	{
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
