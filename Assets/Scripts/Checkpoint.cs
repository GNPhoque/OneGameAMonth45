using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[SerializeField] public int distance;

	public Action<PlayerController, Checkpoint> OnPlayerEntered;

	private void OnTriggerEnter(Collider other)
	{
		PlayerController pc = null;
		if (pc = other.transform.parent?.GetComponent<PlayerController>())
		{
			OnPlayerEntered?.Invoke(pc, this);
		}
	}
}
