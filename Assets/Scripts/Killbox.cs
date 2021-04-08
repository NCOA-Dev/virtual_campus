using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbox : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Player"))
		{
			NetworkManagerCampus nm = GameObject.FindObjectOfType<NetworkManagerCampus>();
			if (nm)
				nm.RespawnPlayer(other.transform.gameObject);
		}
	}
}
