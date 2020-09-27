using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
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
}
