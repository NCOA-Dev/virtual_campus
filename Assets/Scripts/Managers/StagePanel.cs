using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StagePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text[] stagePercentages;
	[SerializeField] private UIPointerHandler[] stageButtons;
    [SerializeField] private ProgressBar[] progressBars;

	public void UpdateStagePanels()
	{
		// Update texts
		for (int i = 0; i < stagePercentages.Length; i++)
		{
			stagePercentages[i].text = progressBars[i].GetPercentage();
		}
	}

	public void UpdateStagePanel(int stage)
	{
		stagePercentages[stage].text = progressBars[stage].GetPercentage();
	}

	public void ActivateStage(int stage)
	{
		foreach (UIPointerHandler btn in stageButtons)
		{
			btn.Activate(false);
		}

		stageButtons[stage].Activate(true);
	}
}
