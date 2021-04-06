using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private GameObject tutorialPC;
	[SerializeField] private GameObject tutorialVR;
	[SerializeField] private GameObject finishPanel;
	[SerializeField] private GameObject teleportPanel;

	public void PressedButton()
	{
		if (tutorialPC != null && tutorialPC.activeSelf)
		{
			tutorialPC.SetActive(false);
		}
		if (tutorialVR != null && tutorialVR.activeSelf)
		{
			tutorialVR.SetActive(false);
		}
		if (finishPanel != null && !finishPanel.activeSelf)
		{
			finishPanel.SetActive(true);
			StartCoroutine(RemFinish());
		}
	}

	public void Teleported()
	{
		if (teleportPanel != null && teleportPanel.activeSelf)
		{
			teleportPanel.SetActive(false);
		}
	}

	private IEnumerator RemFinish()
	{
		yield return new WaitForSeconds(2.0f);

		if (finishPanel != null)
		{
			finishPanel.SetActive(false);
			finishPanel = null;
		}
	}

}
