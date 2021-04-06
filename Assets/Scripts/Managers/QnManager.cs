using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Manager))]
[RequireComponent(typeof(AnimManager))]
[RequireComponent(typeof(SceneLoader))]
public class QnManager : MonoBehaviour
{
    [Header("Game Type")]
    [Tooltip("Learning Mode: Do not allow incorrect answers and show info and hints.")]
    public bool learningMode = false;

    [Tooltip("Which stage to start with.")]
    public int startingStageIndex = 0;

    [Tooltip("Log to console (disable on build).")]
    public bool debug = false;

    [Header("Randomize Questions and Answers")]
    [SerializeField] private bool randomize = false;

    [Header("All Stage UI")]
    [SerializeField] private GameObject[] stagePanels;
    [SerializeField] private GameObject[] textPanels;
    [SerializeField] private GameObject[] startButtons;
    [SerializeField] private GameObject[] backButtons;
    [SerializeField] private GameObject[] skipButtons;
    [SerializeField] private GameObject[] nextButtons;
    [SerializeField] private TMP_Text[] startTexts;
    [SerializeField] private TMP_Text[] finalTexts;
    [SerializeField] private ProgressBar[] progressBars;
    [SerializeField] private GameObject infoPanels;
    [SerializeField] private GameObject infoIntroPanel;
    [SerializeField] private GameObject hintButton;
    [SerializeField] private GameObject hintButtonHelp;
    [SerializeField] private StagePanel stagePanel;

    [Header("Movement Stage")]
    [SerializeField] private List<TMP_Text> movementTexts;
    [SerializeField] public List<string> movementNames;
    [SerializeField] private List<TMP_Text> movementInfoTexts;

    [Header("Muscle Stage")]
    [SerializeField] private List<TMP_Text> muscleTexts;
    [SerializeField] private List<string> muscleNames;
    [SerializeField] private List<TMP_Text> muscleInfoTexts;

    [Header("Bones Stage")]
    [SerializeField] private List<TMP_Text> bonesTexts;
    [SerializeField] private List<string> bonesNames;
    [SerializeField] private List<TMP_Text> bonesInfoTexts;

    [Header("Landmarks Stage")]
    [SerializeField] private List<TMP_Text> landmarksTexts;
    [SerializeField] private List<string> landmarksNames;
    [SerializeField] private List<TMP_Text> landmarksInfoTexts;

    [Header("Start and Finish Stages")]
    [SerializeField] private GameObject startStage;
    [SerializeField] private GameObject finalStage;

    // Management variables
    private Manager gm;
    private AnimManager am;
    private SceneLoader sl;
    private int currentStage = 0;
    private int currentQnProgress = 0;
    private int currentTotQns;

    // Multi-stage management
    private readonly int[] stageProgress = new int[4];
    [HideInInspector] public PointerHighlight[] lastHighlights = new PointerHighlight[4];
    private readonly List<PointerHighlight>[] allPrevHighlights = {
        new List<PointerHighlight>(), new List<PointerHighlight>(),
        new List<PointerHighlight>(), new List<PointerHighlight>() 
    }; 

    // List combinations
    private List<TMP_Text>[] allTexts;
    private List<TMP_Text>[] allInfoTexts;
    private List<string>[] allNames;
    private bool[][] allAnswers;

    void Start()
    {
        gm = GetComponent<Manager>();
        am = GetComponent<AnimManager>();
        sl = GetComponent<SceneLoader>();

        // Setup to clean up start method
        SetupLists();

        infoPanels.SetActive(learningMode);
        hintButton.SetActive(false);
        hintButtonHelp.SetActive(false);
        stagePanel.gameObject.SetActive(false);
    }

    private void SetupLists()
	{
        if (randomize)
        { // Shuffle questions from each stage with same random seed

            // Pick a random seed
            int seed = Random.Range(0, 1000);

            // Shuffle all lists with the same seed 
            movementTexts = ShuffleQuestions(movementTexts, seed);
            movementNames = ShuffleAnswers(movementNames, seed);
            movementInfoTexts = ShuffleQuestions(movementInfoTexts, seed);

            muscleTexts = ShuffleQuestions(muscleTexts, seed);
            muscleNames = ShuffleAnswers(muscleNames, seed);
            muscleInfoTexts = ShuffleQuestions(muscleInfoTexts, seed);

            bonesTexts = ShuffleQuestions(bonesTexts, seed);
            bonesNames = ShuffleAnswers(bonesNames, seed);
            bonesInfoTexts = ShuffleQuestions(bonesInfoTexts, seed);

            landmarksTexts = ShuffleQuestions(landmarksTexts, seed);
            landmarksNames = ShuffleAnswers(landmarksNames, seed);
            landmarksInfoTexts = ShuffleQuestions(landmarksInfoTexts, seed);
        }

        // Combine all lists into arrays of lists to manager between stages
        allTexts = new List<TMP_Text>[4] {
            movementTexts,
            muscleTexts,
            bonesTexts,
            landmarksTexts
        };
        allNames = new List<string>[4] {
            movementNames,
            muscleNames,
            bonesNames,
            landmarksNames
        };
        allInfoTexts = new List<TMP_Text>[4] {
            movementInfoTexts,
            muscleInfoTexts,
            bonesInfoTexts,
            landmarksInfoTexts
        };
        allAnswers = new bool[4][] {
            new bool[movementTexts.Count],
            new bool[muscleTexts.Count],
            new bool[bonesTexts.Count],
            new bool[landmarksTexts.Count]
        };
    }

    // Start all stages - called by "Got It" button
    public void Begin()
	{
        startStage.SetActive(false);
        stagePanel.gameObject.SetActive(true);

        foreach (GameObject helpBox in GameObject.FindGameObjectsWithTag("HelpBox"))
		{
            helpBox.SetActive(false);
		}

        // Setup the stage to begin with 
        SetupStage(startingStageIndex);
    }

    // Answer a question (called by highlight)
    public void AnswerQuestion(string name)
	{
        if (allNames[currentStage].Count > currentQnProgress)
		{
            // If answer is correct
            if (name == allNames[currentStage][currentQnProgress] ||
                (name == "horizontal flexion" && allNames[currentStage][currentQnProgress] == "flexion") ||
                (name == "horizontal extension" && allNames[currentStage][currentQnProgress] == "extension"))
            // Allow flexion and extension answers to be correct on horizontal questions
            {
                if (debug) { Debug.Log(name + " is correct!"); }
                allAnswers[currentStage][currentQnProgress] = true;
            }
            else
            {
                if (debug) { Debug.Log(name + " is incorrect!"); }
                allAnswers[currentStage][currentQnProgress] = false;
            }
        }
        else if (debug)
		{
            Debug.LogWarning("This stage has already been completed.");
		}

        NextQuestion();
    }

    // Fetch whether an answer is correct (called by highlight)
    public bool IsCorrect(string name)
    {
        bool correct = false;

        // If not completed stage
        if (allNames[currentStage].Count > currentQnProgress)
        {
            if (name == allNames[currentStage][currentQnProgress] ||
                (name == "horizontal flexion" && allNames[currentStage][currentQnProgress] == "flexion") ||
                (name == "horizontal extension" && allNames[currentStage][currentQnProgress] == "extension"))
            // Allow flexion and extension answers to be correct on horizontal questions
            {
                correct = true;
            }
        }

        return correct;
    }

    // Setup the next question
    private void NextQuestion()
	{
        if (currentTotQns > currentQnProgress)
		{ 
            allTexts[currentStage][currentQnProgress].gameObject.SetActive(false);
            allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(false);

            currentQnProgress++;
            stageProgress[currentStage] = currentQnProgress;

            progressBars[currentStage].Set(currentQnProgress);
            stagePanel.UpdateStagePanel(currentStage);

            backButtons[currentStage].SetActive(currentQnProgress != 0);

            if (currentStage == 0 && currentQnProgress != currentTotQns)
			{
                am.Enable(currentQnProgress);
			}

            if (currentQnProgress == currentTotQns)
            { // If finished all questions in stage
                finalTexts[currentStage].gameObject.SetActive(true);
                hintButton.SetActive(false);
                hintButtonHelp.SetActive(true);
                infoIntroPanel.SetActive(true);

                // Set final text of this stage
                if (learningMode)
				{
                    string res;
                    if (Random.Range(0.0f, 1.0f) < 0.5f) { res = "Great Job!"; }
                    else { res = "Amazing!"; }
                    finalTexts[currentStage].text = res;
                }
                else
				{
                    finalTexts[currentStage].text = string.Format("Result: {0} / {1}", TotalCorrect(allAnswers[currentStage]), currentTotQns);
                }

                if (debug) { Debug.Log("Stage " + currentStage + " complete."); }

                skipButtons[currentStage].SetActive(false);
                nextButtons[currentStage].SetActive(true);

                if (currentStage == 0)
                {
                    am.Disable();
                }
            }
            else if (allTexts[currentStage][currentQnProgress] != null)
            {
                allTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
                allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
            }

            backButtons[currentStage].SetActive(currentQnProgress != 0);
        }
        else if (debug)
		{
            Debug.LogWarning("This stage has already been completed.");
        }
    }

    // Setup the stage
    public void SetupStage(int stage)
	{
        if (stagePanels[currentStage] != null)
		{ // Remove previous stage
            stagePanels[currentStage].SetActive(false);
        }

        gm.EnableStage(stage);

        stagePanels[currentStage].SetActive(false);
        hintButton.SetActive(false);
        hintButtonHelp.SetActive(false);

        stagePanels[stage].SetActive(true);
        currentStage = stage;
        currentQnProgress = stageProgress[currentStage];
        currentTotQns = allTexts[currentStage].Count;

        // Setup if not already set up
        if (!skipButtons[stage].activeSelf && currentQnProgress < currentTotQns)
		{
            //startTexts[stage].gameObject.SetActive(true);
            backButtons[stage].SetActive(false);
            skipButtons[stage].SetActive(false);
            nextButtons[stage].SetActive(false);
            startButtons[stage].SetActive(true);
            nextButtons[currentStage].SetActive(false);
            infoIntroPanel.SetActive(true);
        }
        else if (currentQnProgress < allInfoTexts[currentStage].Count)
		{
            allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
            infoIntroPanel.SetActive(false);
            hintButton.SetActive(true);
            hintButtonHelp.SetActive(true);
        }
        else
		{
            infoIntroPanel.SetActive(true);
            hintButton.SetActive(false);
            hintButtonHelp.SetActive(false);
        }

        stagePanel.ActivateStage(stage);
    }

    // Start the stage (triggered by start button)
    public void StartStage(int stage)
    {
        infoIntroPanel.SetActive(false);
        if (currentStage == 0 && currentQnProgress != currentTotQns)
        {
            am.Enable(currentQnProgress);
        }

        allTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
        allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(true);

        startTexts[currentStage].gameObject.SetActive(false);

        gm.EnableHighlights(stage, true);

        startButtons[currentStage].gameObject.SetActive(false);
        skipButtons[currentStage].gameObject.SetActive(true);

        hintButton.SetActive(true);
        hintButtonHelp.SetActive(true);
    }

    // Back button
    public void Back()
	{
        if (currentQnProgress < currentTotQns)
		{ // If has not completed this stage
            allTexts[currentStage][currentQnProgress].gameObject.SetActive(false);
            allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(false);
            allAnswers[currentStage][currentQnProgress] = false;
        }
        else
		{
            finalTexts[currentStage].gameObject.SetActive(false);
		}

        if (lastHighlights[currentStage] != null)
        {
            lastHighlights[currentStage].UndoHighlight();
            allPrevHighlights[currentStage].Remove(lastHighlights[currentStage]);

            if (allPrevHighlights[currentStage].Count > 0)
			{
                lastHighlights[currentStage] = allPrevHighlights[currentStage][allPrevHighlights[currentStage].Count - 1];
            }
        }

        currentQnProgress--;
        stageProgress[currentStage] = currentQnProgress;

        progressBars[currentStage].Set(currentQnProgress);
        allTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
        allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(true);

        UndoHints();

        if (currentStage == 0 && currentQnProgress != currentTotQns)
        {
            am.Enable(currentQnProgress);
        }

        backButtons[currentStage].SetActive(currentQnProgress != 0);
        skipButtons[currentStage].SetActive(true);
        nextButtons[currentStage].SetActive(false);

        infoIntroPanel.SetActive(false);
        hintButton.SetActive(true);
        hintButtonHelp.SetActive(true);
    }

    // Skip button
    public void Skip()
    {
        if (allNames[currentStage].Count > currentQnProgress)
		{
            if (allTexts[currentStage][currentQnProgress] != null)
            {
                allTexts[currentStage][currentQnProgress].gameObject.SetActive(false);
            }
            if (allInfoTexts[currentStage][currentQnProgress] != null)
            {
                allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(false);
            }

            string name = allNames[currentStage][currentQnProgress];
            allNames[currentStage].RemoveAt(currentQnProgress);
            allNames[currentStage].Add(name);

            TMP_Text text = allTexts[currentStage][currentQnProgress];
            allTexts[currentStage].RemoveAt(currentQnProgress);
            allTexts[currentStage].Add(text);

            TMP_Text infoText = allInfoTexts[currentStage][currentQnProgress];
            allInfoTexts[currentStage].RemoveAt(currentQnProgress);
            allInfoTexts[currentStage].Add(infoText);

            if (allTexts[currentStage][currentQnProgress] != null)
            {
                allTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
            }
            if (allInfoTexts[currentStage][currentQnProgress] != null)
            {
                allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
            }

            if (currentStage == 0 && currentQnProgress != currentTotQns)
            {
                am.Enable(currentQnProgress);
            }

            UndoHints();
            backButtons[currentStage].SetActive(currentQnProgress != 0);
        }
    }

    // Change from one stage to another
    public void ChangeToStage(int stage)
    {
        if (currentStage == 0)
		{
            am.Disable();
		}

        stagePanel.ActivateStage(stage);

        // Remove previous stage
        stagePanels[currentStage].SetActive(false);
        if (currentQnProgress < currentTotQns)
        {
            allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(false);
        }

        // Pick up where the last stage was left off
        gm.EnableStage(stage);
        stagePanels[stage].SetActive(true);

        currentStage = stage;
        currentQnProgress = stageProgress[currentStage];
        currentTotQns = allTexts[currentStage].Count;

        if (currentQnProgress >= currentTotQns)
		{ // If completed the stage
            infoIntroPanel.SetActive(true);
            hintButton.SetActive(false);
            hintButtonHelp.SetActive(false);
            nextButtons[stage].gameObject.SetActive(true);
        }
        else if (!skipButtons[stage].activeSelf)
		{ // If the stage has not started, set it up
            startButtons[stage].gameObject.SetActive(true);
            hintButton.SetActive(false);
            hintButtonHelp.SetActive(false);
            infoIntroPanel.SetActive(true);
        }
        else
		{ // Otherwise, set current info UI
            allInfoTexts[currentStage][currentQnProgress].gameObject.SetActive(true);
            infoIntroPanel.SetActive(false);
            hintButton.SetActive(true);
            hintButtonHelp.SetActive(true);

            if (stage == 0)
            {
                am.Enable(currentQnProgress);
            }
        }

        finalStage.SetActive(false);
    }

    // Record the previous highlights
    public void RecHighlight(PointerHighlight highlight)
	{
        lastHighlights[currentStage] = highlight;
        allPrevHighlights[currentStage].Add(highlight);
	}

    // Called by submit / final next button
    // Set up the final "stage"
    public void Finish()
	{
        bool done = true;

        for (int i = startingStageIndex; i < stageProgress.Length; i++)
		{
            if (stageProgress[i] < allTexts[i].Count)
			{
                ChangeToStage(i);
                done = false;
                break;
			}
		}

        if (done)
		{
            stagePanels[currentStage].SetActive(false);
            finalStage.SetActive(true);

            int[] ans = TotalCorrect(allAnswers);

            if (debug)
			{
                Debug.LogWarningFormat("Overall Result: {0} / {1}", ans[1], ans[0]);
            }
        }
	}

    public void FinishGame()
	{
        sl.LoadLevel(0);
    }

    // Trigger hint effect on the current question highlight
    public void HintCurrentHighlight()
	{
        if (currentQnProgress < currentTotQns)
		{
            string name = allNames[currentStage][currentQnProgress];
            PointerHighlight[] allHighlights = FindObjectsOfType<PointerHighlight>();

            bool found = false;

            if (currentStage == 0)
			{
                am.Hint();
                found = true;
			}
            else
			{
                foreach (PointerHighlight highlight in allHighlights)
                {
                    if (name == highlight.areaName)
                    {
                        highlight.Hint();
                        found = true;
                    }
                    else
                    {
                        highlight.RemHint();
                    }
                }
            }

            if (!found && debug)
            {
                Debug.LogError(name + " not found!");
            }
        }
    }

    // Undo all hint effects - called on skip and back
    public void UndoHints()
    {
        PointerHighlight[] allHighlights = FindObjectsOfType<PointerHighlight>();

        foreach (PointerHighlight highlight in allHighlights)
        {
            highlight.RemHint();
        }
    }

    #region private
    private int TotalCorrect(bool[] answers)
	{
        int tot = 0;

        for (int i = 0; i < answers.Length; i++)
		{
            if (answers[i])
			{
                tot++;
			}
		}

        return tot;
	}

    // Returns total answers and total correct answers across all stages
    private int[] TotalCorrect(bool[][] answers)
    {
        int totCorrect = 0;
        int totAnswers = 0;

        for (int i = 0; i < answers.Length; i++)
        {
            for (int j = 0; j < answers[i].Length; j++)
			{
                if (answers[i][j])
                {
                    totCorrect++;
                }
                totAnswers++;
            }
        }
        int[] ans = {
            totAnswers, 
            totCorrect
        };

        return ans;
    }

	// Fisher-Yates Shuffle
	// Used to shuffle questions and answers with same random indeces
	private static List<TMP_Text> ShuffleQuestions(List<TMP_Text> questionList, int randSeed)
    {
        System.Random rand = new System.Random(randSeed);

        int n = questionList.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            TMP_Text value = questionList[k];
            questionList[k] = questionList[n];
            questionList[n] = value;
        }

		return questionList;
    }

    private static List<string> ShuffleAnswers(List<string> answerList, int randSeed)
    {
        System.Random rand = new System.Random(randSeed);

        int n = answerList.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            string value = answerList[k];
            answerList[k] = answerList[n];
            answerList[n] = value;
        }

        return answerList;
    }
	#endregion
}