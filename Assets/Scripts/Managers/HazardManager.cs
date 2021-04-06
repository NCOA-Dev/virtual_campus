using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HazardManager : MonoBehaviour
{
    [Header("Major Panels")]
    [SerializeField] private TextPanel textPanel;
    [SerializeField] private GameObject riskPanel;
    [SerializeField] private GameObject responsePanel;
    [SerializeField] private GameObject completionPanel;

    [Header("Other UI")]
    [SerializeField] private TMP_Text riskHazardText;
    [SerializeField] private TMP_Text responseHazardText;
    [SerializeField] private UIPointerHandler[] responseButtonsCont;
    [SerializeField] private UIPointerHandler[] responseButtonsRep;
    [SerializeField] private GameObject clickBlocker;
    [SerializeField] private TMP_Text locationText;
    [SerializeField] private GameObject dateText;

    [Header("Reception")]
    [SerializeField] private List<string> hazardDescsReception;
    [SerializeField] private int[] riskAnswersReception;
    [SerializeField] private Vector2[] responseAnswersReception;
    [SerializeField] private List<string> hazardFeedbackReception;

    [Header("Office")]
    [SerializeField] private List<string> hazardDescsOffice;
    [SerializeField] private int[] riskAnswersOffice;
    [SerializeField] private Vector2[] responseAnswersOffice;
    [SerializeField] private List<string> hazardFeedbackOffice;

    [Header("Storage Room")]
    [SerializeField] private List<string> hazardDescsStorage;
    [SerializeField] private int[] riskAnswersStorage;
    [SerializeField] private Vector2[] responseAnswersStorage;
    [SerializeField] private List<string> hazardFeedbackStorage;

    [Header("Scene Tracking Variables")]
    [Tooltip("Location name for the scene of equal array index to scene index. Menu scenes should be blank.")]
    [SerializeField] private string[] locationNames;

    // Response tracking variables
    private bool selectedResCont = false;
    private bool selectedResRep = false;
    private string contText = "";
    private string repText = "";

    // Room state tracking
    private int currentRoom = 0;
    private int[] roomProgress;
    private readonly int totalRooms = 3;
    private int selectedRow = 0;

    // Room combination variables
    private List<TextPanel> allTextPanels;
    private List<string>[] allHazardDescs;
    private List<string>[] allHazardFeedback;
    private int[][] allRiskAnswers;
    private Vector2[][] allResponseAnswers;
    private int[][] allRiskAttempts;
    private Vector2[][] allResponseAttempts;
    private bool[][,] allResults;

    void Start()
    {
        // Clone hazard texts
        allHazardDescs = new List<string>[totalRooms];
        allHazardDescs[0] = new List<string>(hazardDescsReception);
        allHazardDescs[1] = new List<string>(hazardDescsOffice);
        allHazardDescs[2] = new List<string>(hazardDescsStorage);

        // Instantiate attempt arrays
        allRiskAttempts = new int[totalRooms][];
        allResponseAttempts = new Vector2[totalRooms][];
        allRiskAttempts[0] = new int[riskAnswersReception.Length];
        allResponseAttempts[0] = new Vector2[responseAnswersReception.Length];
        allRiskAttempts[1] = new int[riskAnswersOffice.Length];
        allResponseAttempts[1] = new Vector2[responseAnswersOffice.Length];
        allRiskAttempts[2] = new int[riskAnswersStorage.Length];
        allResponseAttempts[2] = new Vector2[responseAnswersStorage.Length];

        // Instantiate answer arrays
        allRiskAnswers = new int[totalRooms][];
        allResponseAnswers = new Vector2[totalRooms][];
        allRiskAnswers[0] = riskAnswersReception;
        allResponseAnswers[0] = responseAnswersReception;
        allRiskAnswers[1] = riskAnswersOffice;
        allResponseAnswers[1] = responseAnswersOffice;
        allRiskAnswers[2] = riskAnswersStorage;
        allResponseAnswers[2] = responseAnswersStorage;

        // Instantiate results arrays
        allResults = new bool[totalRooms][,];
        allResults[0] = new bool[riskAnswersReception.Length, 3];
        allResults[1] = new bool[riskAnswersOffice.Length, 3];
        allResults[2] = new bool[riskAnswersStorage.Length, 3];

        // Clone feedback texts
        allHazardFeedback = new List<string>[totalRooms];
        allHazardFeedback[0] = new List<string>(hazardFeedbackReception);
        allHazardFeedback[1] = new List<string>(hazardFeedbackOffice);
        allHazardFeedback[2] = new List<string>(hazardFeedbackStorage);

        roomProgress = new int[totalRooms];

        // Fill attempt arrays with -1 to track if attempted
        for (int i = 0; i < allRiskAttempts.Length; i++)
		{
            for (int j = 0; j < allRiskAttempts[i].Length; j++)
			{
                allRiskAttempts[i][j] = -1;
                allResponseAttempts[i][j].x = -1;
                allResponseAttempts[i][j].y = -1;
            }
        }

        foreach (UIPointerHandler feedbackBtn in textPanel.feedbackButtons)
		{
            feedbackBtn.gameObject.SetActive(false);
		}

        // Create reports with total rooms
        CreateReports(totalRooms);

        StartRoom(0);
    }

    public void StartRoom(int sceneIndex)
	{
        currentRoom = sceneIndex - 1;

        // Deactivate all popup panels
        StopAllCoroutines();
        riskPanel.SetActive(false);
        responsePanel.SetActive(false);
        completionPanel.SetActive(false);
        CloseResponse();
        EnableBtns(responseButtonsCont, -1);
        EnableBtns(responseButtonsRep, -1);

        // Deactivate other text panels and enable current level panel
        foreach (TextPanel tp in allTextPanels)
		{
			tp.SetActive(false);
		}
		allTextPanels[sceneIndex].SetActive(true);
        textPanel = allTextPanels[sceneIndex];

        // Set name of room
        locationText.text = locationNames[sceneIndex];

        // Date text visible on non-menu scenes
        dateText.SetActive(sceneIndex > 0);
    }

    // Called when a hazard is found
    public void FoundHazard(string hazardDesc)
	{
		// Remove the found hazard from the list to ensure its tracked and only found once
		List<string> currentDescs = new List<string>(allHazardDescs[currentRoom]);
		if (currentDescs.Contains(hazardDesc))
		{
			// Remove the text to prevent repetitions
			currentDescs.Remove(hazardDesc);

			// Add the text to the clipboard
			textPanel.hazardTextBoxes[roomProgress[currentRoom]].text = hazardDesc;

			// Activate input boxes
			textPanel.riskBoxes[roomProgress[currentRoom]].interactable = true;
			textPanel.responseBoxes[roomProgress[currentRoom]].interactable = true;

            // Activate feedback buttons
            textPanel.feedbackButtons[roomProgress[currentRoom]].gameObject.SetActive(true);

            // Increment progress
            roomProgress[currentRoom]++;
		}
		else
		{
			Debug.LogWarning(hazardDesc + " not found or already added.");
		}
	}

    // Called by Okay button on completion panel
    public void SubmitCompletionPanel()
    {
        StartCoroutine(WaitThenActivate(0.2f, completionPanel, false));
    }

    #region risk
    // Called when risk box is clicked
    public void SelectRiskBox(int row)
	{
        // Reveal risk panel
        StartCoroutine(WaitThenActivate(0.3f, riskPanel, true));

        string rowText = textPanel.hazardTextBoxes[row].text;
        riskHazardText.text = rowText;

        // Keep track of which row was clicked on
        selectedRow = row;
    }

    // Called when pressed risk button (0: low, 1: med, 2: high)
    public void SelectRisk(int severity)
    {
        string riskText = "";
        Color riskCol = Color.black;
        switch (severity)
        {
            case 0:
                riskText = "Low";
                riskCol = new Color(0.95f, 0.9f, 0f); // yellow
                break;
            case 1:
                riskText = "Moderate";
                riskCol = new Color(1f, 0.5f, 0f); // orange
                break;
            case 2:
                riskText = "High";
                riskCol = new Color(0.9f, 0f, 0f); // red
                break;
        }

        // Record answer
        allRiskAttempts[currentRoom][IndexOf(selectedRow)] = severity;

        // Evaluate results
        bool correct = severity == allRiskAnswers[currentRoom][IndexOf(selectedRow)];
        allResults[currentRoom][IndexOf(selectedRow), 0] = correct; // Store result

        // If not being called by cancel button, set text
        if (severity != -1)
        {
            TMP_Text txt = textPanel.riskBoxes[selectedRow].GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                txt.text = riskText;
                txt.color = riskCol;
            }
            else
            {
                Debug.LogError("Text box not found on " + textPanel.riskBoxes[selectedRow].name);
            }

            // Only set color if not being called by cancel button
            textPanel.riskBoxes[selectedRow].ActivateColor(!correct); // Change color to red if incorrect
        }

        StartCoroutine(WaitThenActivate(0.2f, riskPanel, false));
        CheckIfFinished();
    }
    #endregion

    #region response
    // Called when response box is clicked
    public void SelectResponseBox(int row)
    {
        // Reveal risk panel
        StartCoroutine(WaitThenActivate(0.3f, responsePanel, true));

        string rowText = textPanel.hazardTextBoxes[row].text;
        responseHazardText.text = rowText;

        // Keep track of which row was clicked on
        selectedRow = row;
    }

    // Response select workaround for UnityEvents only accepting one parameter
    public void SelectResponse1(int response)
	{
        SelectResponse(1, response);
    }
    public void SelectResponse2(int response)
    {
        SelectResponse(2, response);
    }
    public void ResponseCancel()
    {
        SelectResponse(-1, -1);
    }

    // Called when pressed response button (section 1, 2, or -1) and response (0, 1, 2)
    private void SelectResponse(int section, int response)
    {
        switch (section)
        {
            case -1: // Called by cancel button
                CloseResponse();
                EnableBtns(responseButtonsCont, -1);
                EnableBtns(responseButtonsRep, -1);
                break;
            case 1: // Control measure section
                selectedResCont = true;
                EnableBtns(responseButtonsCont, response);
                TMP_Text contAnsText = responseButtonsCont[response].GetComponentInChildren<TMP_Text>();
                contText = contAnsText.text;

                // Record answer
                allResponseAttempts[currentRoom][IndexOf(selectedRow)].x = response;

                if (response != allResponseAnswers[currentRoom][IndexOf(selectedRow)].x)
                {  // Incorrect - make text red and store incorrect result
                    contText = Redify(contText);
                    allResults[currentRoom][IndexOf(selectedRow), 1] = false;
                }
                else
                { // Correct - store correct result
                    allResults[currentRoom][IndexOf(selectedRow), 1] = true;
                }
                break;
            case 2: // Reporting method section
                selectedResRep = true;
                EnableBtns(responseButtonsRep, response);
                TMP_Text repAnsText = responseButtonsRep[response].GetComponentInChildren<TMP_Text>();
                repText = repAnsText.text;

                // Record answer
                allResponseAttempts[currentRoom][IndexOf(selectedRow)].y = response;

                if (response != allResponseAnswers[currentRoom][IndexOf(selectedRow)].y)
                { // Incorrect - make text red and store incorrect result
                    repText = Redify(repText);
                    allResults[currentRoom][IndexOf(selectedRow), 2] = false;
                }
                else
                { // Correct - store correct result
                    allResults[currentRoom][IndexOf(selectedRow), 2] = true;
                }
                break;
            default:
                Debug.LogError("Section " + section + " not found.");
                break;
        }


        if (selectedResCont && selectedResRep)
        {
            TMP_Text txt = textPanel.responseBoxes[selectedRow].GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                txt.text = string.Format("• {0} \n• {1}", contText, repText);
            }
            else
            {
                Debug.LogError("Text box not found on " + textPanel.responseBoxes[selectedRow].name);
            }

            // Change color to red if incorrect
            bool correct = allResults[currentRoom][IndexOf(selectedRow), 1] &&
                           allResults[currentRoom][IndexOf(selectedRow), 2];
            textPanel.responseBoxes[selectedRow].ActivateColor(!correct);

            CloseResponse();
            CheckIfFinished();
        }
    }

    private void EnableBtns(UIPointerHandler[] btns, int ignoreIndex)
	{
        for (int i = 0; i < btns.Length; i++)
		{
            if (i != ignoreIndex)
			{
                btns[i].Activate(false);
			}
		}
	}

    private void CloseResponse()
	{
        selectedResCont = false;
        selectedResRep = false;

		StartCoroutine(WaitThenActivate(0.4f, responsePanel, false));
    }
    #endregion

    #region feedback

    // Triggered by feedback buttons
    public void PressFeedbackBtn(int index)
    {
        if (textPanel.feedbackTexts[index] != null && textPanel.feedbackTextBoxes[index] != null)
		{ 
            // Enable/disable feedback
            textPanel.feedbackTexts[index].SetActive(!textPanel.feedbackTexts[index].activeSelf);

            // Reverse arrows
            textPanel.feedbackArrows[index].localScale = new Vector3(textPanel.feedbackArrows[index].localScale.x * -1, 1, 1);

            // Set text
            textPanel.feedbackTextBoxes[index].text = allHazardFeedback[currentRoom][IndexOf(index)];
        }
    }

    #endregion

    #region private
    // Duplicate report info for each room
    private void CreateReports(int rooms)
	{
        allTextPanels = new List<TextPanel>();
        allTextPanels.Add(textPanel);
        for (int i = 0; i < rooms; i++)
		{
            GameObject tp = Instantiate(textPanel.gameObject, textPanel.transform.parent);
            tp.transform.SetSiblingIndex(i);
            tp.SetActive(false);
            allTextPanels.Add(tp.GetComponent<TextPanel>());
        }
    }

	// Called when an answer is submitted 
	private void CheckIfFinished()
	{
        bool allAttempted = true;
        bool allCorrect = true;

        for (int i = 0; i < allRiskAttempts[currentRoom].Length; i++)
		{ // Check if any question is not yet attempted
            if (allRiskAttempts[currentRoom][i] == -1 ||
                allResponseAttempts[currentRoom][i].x == -1 ||
                allResponseAttempts[currentRoom][i].y == -1)
			{
                allAttempted = false;
			}
		}
        
        for (int i = 0; i < allResults[currentRoom].GetLength(0); i++)
        { // Check if all questions are correct
            for (int j = 0; j < allResults[currentRoom].GetLength(1); j++)
			{
                if (!allResults[currentRoom][i, j])
				{
                    allCorrect = false;
                }
			}
		}

        if (allAttempted)
		{
            // do something to finish 
            Debug.Log("All questions attempted.");
		}

        if (allCorrect)
		{
            Debug.LogWarning("All questions correct.");

            // Reveal completion panel
            StartCoroutine(WaitThenActivate(1f, completionPanel, true));
            StartCoroutine(WaitThenActivate(1.2f, riskPanel, false));
            StartCoroutine(WaitThenActivate(1.2f, responsePanel, false));
        }
    }

    // Return the description list index of a hazard text from the given row
    private int IndexOf(int row)
	{
        string text = textPanel.hazardTextBoxes[row].text;
        return allHazardDescs[currentRoom].IndexOf(text);
        //return hazardDescsStorageCopy.IndexOf(text);
    }
     
    // Returns a given string, but made dark red with html/TMP tags
    private string Redify(string text)
	{
        return "<color=#9D0000>" + text + "</color>";
	}

    // Wait then activate or deactivate a given object - block clicks during this wait period
    private IEnumerator WaitThenActivate(float secs, GameObject obj, bool activ)
	{
        // Block clicks during wait time
        clickBlocker.SetActive(true);
        yield return new WaitForSeconds(secs);
        obj.SetActive(activ);
        clickBlocker.SetActive(false);
	}
	#endregion
}
