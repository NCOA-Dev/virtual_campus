using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private GameObject[] enterObjects;
	private Chat chat;
	private void Start()
	{
		enterObjects = GameObject.FindGameObjectsWithTag("Enter");
		chat = GameObject.FindObjectOfType<Chat>();
	}

	private void Update()
	{
		if (Cursor.visible && Input.GetKeyDown(KeyCode.Return) && enterObjects != null && chat != null)
		{
			foreach (GameObject obj in enterObjects)
			{
				Button btn = obj.GetComponent<Button>();
				if (btn != null)
				{
					btn.onClick.Invoke();
				}
				Debug.LogWarning(btn.name);
			}
			Debug.LogWarning("hm");
		}
	}
}
