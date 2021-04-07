using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Animator))]
public class ProgressBar : MonoBehaviour
{
	public TMP_Text progressText;
	[SerializeField] private TMP_Text qaText;
	private Slider slider;
	private Animator anim;

	[Header("Complete button to swap to when complete")]
	[SerializeField] private GameObject completeButton;

	private float currentProgress;
	private float newProgress;
	private float time;
	private float speed = 0.02f;
	private int totalQns;
	private readonly bool percentage = true;
	private string mainTex;

	// set as 0 for questions 0, 1, 2 OR 1 for questions 1, 2, 3
	private readonly int adj = 0;

	private void Start()
	{
		slider = GetComponent<Slider>();
		anim = GetComponent<Animator>();

		mainTex = qaText.text;

		currentProgress = slider.value + adj;
		newProgress = currentProgress;

		totalQns = (int)slider.maxValue;
		qaText.text = string.Format("{0} {1}/{2}", mainTex, currentProgress, totalQns);

		if (currentProgress > 0)
		{
			progressText.text = currentProgress.ToString();
		}
	}

	public void SetCurrentProgress(int progress)
	{
		if (slider != null) 
		{
			currentProgress = progress;
			newProgress = currentProgress;
			slider.value = currentProgress;
		}
	}

	public void Set(int num)
	{
		StartCoroutine(SetProgress(num));
	}

	private IEnumerator SetProgress(int num)
	{
		newProgress = num + adj;

		yield return new WaitForSeconds(0.01f);
		//if (currentProgress == newProgress && currentProgress == totalQns)
		//{
		//	Remove();
		//}
		//else if (!(newProgress == 0 && currentProgress == 0))
		//{
		//	anim.SetTrigger("appear");

		//	if (newProgress >= totalQns && completeButton != null)
		//	{
		//		completeButton.SetActive(false);
		//	}
		//}
	}

	private void Update()
	{
		if (currentProgress != newProgress)
		{
			if (currentProgress < newProgress)
			{
				speed = 0.07f;
			}
			else
			{
				speed = 0.04f;
			}

			if (Approximately(currentProgress, newProgress, 0.05f))
			{
				currentProgress = newProgress;
			}

			// Lerp by speed to new time
			currentProgress = Mathf.Lerp(currentProgress, newProgress, time);
			time += speed * Time.deltaTime;

			slider.value = currentProgress - adj;

			if (percentage)
			{
				if (currentProgress < totalQns * 0.09f)
				{ // Ensure zero is never shown
					progressText.text = "";
				}
				else
				{
					progressText.text = Percentage(currentProgress - adj);
				}
				qaText.text = String.Format("{0} {1}/{2}", mainTex, (int)Mathf.Round(currentProgress), totalQns);
			}
			else
			{
				// If approx. a whole num, set new text to this whole num
				for (int i = 0; i <= totalQns; i++)
				{
					if (Mathf.Abs(currentProgress - i) < 0.5f)
					{
						int newNum = (int)Mathf.Round(i);
						if (newNum <= 0)
						{ // Ensure zero is never shown
							progressText.text = "";
						}
						else
						{
							progressText.text = newNum.ToString() + "/" + totalQns;
						}
					}
				}

				if (currentProgress < totalQns * 0.1f)
				{
					progressText.text = "";
				}
			}

			if (Mathf.Abs(currentProgress - newProgress) < 0.01f)
			{
				currentProgress = newProgress;
				time = 0;

				//if (currentProgress >= totalQns || currentProgress == 0)
				//{ // Remove bar when complete or empty
				//	Remove();
				//}
			}
		}
	}

	public static bool Approximately(float a, float b, float threshold)
	{
		return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
	}

	public void Remove()
	{
		anim.SetTrigger("disappear");
		completeButton.SetActive(true);
	}

	private string Percentage(float value)
	{
		float perc = (value / totalQns) * 100;
		perc = Mathf.Round(perc);

		string res = perc.ToString() + "%";

		return res;
	}

	// Get the exact final percentage (without animations or delays)
	public string GetPercentage()
	{
		float perc = (newProgress / totalQns) * 100;
		perc = Mathf.Round(perc);

		string res = perc.ToString() + "%";

		return res;
	}
}
