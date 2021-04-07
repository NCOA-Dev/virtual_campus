using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
	[Header("Referenced Variables")]
	[SerializeField] private GameObject[] saveOnLoad;
	[SerializeField] private TMP_Text stageText;
	[SerializeField] private Animator elevatorAnim;
	[SerializeField] private HazardManager hm;
	[SerializeField] private PointerButton[] elevButtons;
	[SerializeField] private GameObject teleportBlocker;
	[SerializeField] private GameObject upArrow;
	[SerializeField] private GameObject downArrow;
	[SerializeField] private GameObject[] teleporters;

	[Header("Hazard Objects")]
	[SerializeField] private GameObject[] hazards;

	private int currentLvl = 0;
	private int prevLevel = 0;
	private readonly int totalLevels = 3;
	private bool loading = false;
	private int floorsToNextLevel = 0;

	private void Awake()
	{
		foreach (GameObject obj in saveOnLoad)
		{
			DontDestroyOnLoad(obj);
		}
	}

	private void Start()
	{
		foreach (GameObject hazard in hazards)
		{
			hazard.SetActive(false);
		}

		elevButtons[0].Activate(false);

		foreach (GameObject teleporter in teleporters)
		{
			teleporter.SetActive(false);
		}
	}

	private void Update()
	{
		// Lock cursor
		if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked && CursorOnScreen())
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

#if UNITY_WEBGL
			Resolution resolution = Screen.resolutions[Screen.resolutions.Length - 1];
			Screen.SetResolution(resolution.width, resolution.height, true);
#endif
		}




	}

	private bool CursorOnScreen()
	{
		return Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.x < Screen.width - 1 & Input.mousePosition.y < Screen.height - 1;
	}

	public void PressButton(int level)
	{
		if (!loading && prevLevel != level)
		{
			floorsToNextLevel = Mathf.Abs(currentLvl - level);
			currentLvl = level;

			if (elevatorAnim.GetBool("open"))
			{ // If elevator open, wait for it to close
				elevatorAnim.SetBool("open", false);
				StartCoroutine(ElevatorLoad(currentLvl, 8f, NextLevel));
			}
			else
			{ // If elevator already closed
				StartCoroutine(ElevatorLoad(currentLvl, 6f, NextLevel));
			}
		}
		else if (prevLevel != level)
		{ // Change elevator direction
			elevatorAnim.SetBool("open", false);

			// Elevator goes up if new level is higher than previous and vice versa
			elevatorAnim.SetBool("up", level > prevLevel);
			
			currentLvl = level;
			StopAllCoroutines();
			StartCoroutine(ElevatorLoad(currentLvl, 6f, NextLevel));
		}
		else
		{ // Already on the right floor
			StopAllCoroutines();
			currentLvl = level;
			elevatorAnim.SetBool("open", true);

			elevButtons[currentLvl].Activate(false);
		}

		// Elevator goes up if new level is higher than previous and vice versa
		elevatorAnim.SetBool("up", level > prevLevel);

		upArrow.SetActive(level > prevLevel);
		downArrow.SetActive(level < prevLevel);

		loading = true;
		// Set click colour (only if confirmed click)
		foreach (PointerButton eb in elevButtons)
		{
			if (eb != elevButtons[currentLvl] && eb.activated)
			{
				eb.ClickCol(false);
			}
			else if (eb.activated)
			{
				eb.ClickCol(true);
			}
		}
	}

	public void NextButton()
	{
		if (prevLevel < totalLevels)
		{
			currentLvl = prevLevel + 1;
			PressButton(currentLvl);
		}
	}

	public void PrevButton()
	{
		if (prevLevel != 0)
		{
			currentLvl = prevLevel - 1;
			PressButton(currentLvl);
		}
	}

	// Setup the next level
	private void NextLevel()
	{
		stageText.text = "Floor " + currentLvl;
		if (currentLvl == 0)
		{
			stageText.text = "Ground";
		}

		elevatorAnim.SetBool("open", true);
		loading = false;

		foreach (GameObject hazard in hazards)
		{
			hazard.SetActive(false);
		}
		if (currentLvl >= 1)
		{
			hazards[currentLvl - 1].SetActive(true);

			foreach (GameObject teleporter in teleporters)
			{
				teleporter.SetActive(true);
			}
		}
		else
		{
			foreach (GameObject teleporter in teleporters)
			{
				teleporter.SetActive(false);
			}
		}

		// Disable pressed button and enable others
		foreach (PointerButton btn in elevButtons)
		{
			btn.Activate(true);
		}
		elevButtons[currentLvl].Activate(false);
		upArrow.SetActive(false);
		downArrow.SetActive(false);

		prevLevel = currentLvl;

		hm.StartRoom(currentLvl);
	}

	// Load next scene around set pauses and run the given method when done
	private IEnumerator ElevatorLoad(int index, float timeBeforeLoad, Action doAfter)
	{
		teleportBlocker.SetActive(true);
		yield return new WaitForSeconds(timeBeforeLoad);

		// Load scene
		yield return new WaitForSeconds(0.1f);
		AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index + 1);

		while (!asyncLoad.isDone)
		{
			yield return null;
		}
		Cursor.visible = false;

		yield return new WaitForSeconds(0.5f);
		teleportBlocker.SetActive(false);
		doAfter();
	}
}
