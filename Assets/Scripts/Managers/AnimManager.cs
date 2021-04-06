using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(QnManager))]
public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private UIPointerHandler[] buttons;
    [SerializeField] private UIPointerHandler submitButton;
    [SerializeField] private TMP_Text[] infoTexts;
    [SerializeField] private TMP_Text[] infoTitles;
    [SerializeField] private string[] movementInfo;
    [SerializeField] private GameObject infoPanel;

    private string selectedAns = "";
    private readonly string[] infoTitleTexts = new string[4];
    private int selectedIndex = 0;
    private QnManager qm;
    private List<string> ansNames;

    // Answer tracking
    private readonly string[] abcd = new string[4];
    private readonly int[] abcdAnimIndex = new int[4];
    private int ansIndex;

    private void Awake()
	{
        qm = GetComponent<QnManager>();
        ansNames = new List<string>(qm.movementNames);
    }

	private void Start()
	{ 
        // Save current title texts of infoTitles
        for (int i = 0; i < infoTitles.Length; i++)
        {
            infoTitleTexts[i] = infoTitles[i].text;
        }

        // Disable buttons on start
        Disable();
    }

    // Enable/disable movements stage buttons
    public void Disable()
	{
		foreach (UIPointerHandler button in buttons)
		{
            button.gameObject.SetActive(false);
            button.Activate(false);
        }
        submitButton.gameObject.SetActive(false);
        submitButton.Activate(false);

        for (int i = 0; i < infoTexts.Length; i++)
        {
            infoTexts[i].gameObject.SetActive(false);
            infoTitles[i].text = infoTitleTexts[i];
        }
    }

    public void Enable(int currentQuestion)
    {
        foreach (UIPointerHandler button in buttons)
        {
            button.gameObject.SetActive(true);
            button.Activate(false);
        }
        submitButton.gameObject.SetActive(true);
        submitButton.Activate(true);

        // Randomize button answers
        RandomizeButtons(currentQuestion);

        // Stop animation
        anim.SetInteger("play-anim", -1);

        for (int i = 0; i < infoTexts.Length; i++)
		{
            infoTexts[i].gameObject.SetActive(false);
            infoTitles[i].text = infoTitleTexts[i];
        }
        infoPanel.SetActive(true);
    }

    // Called on button press
    // qnNum is buttons 0,1,2,3
    public void Press(int qnNum)
	{
        foreach (UIPointerHandler button in buttons)
		{
            button.Activate(false);
        }
        buttons[qnNum].Activate(true);
        submitButton.Activate(false);

        foreach (TMP_Text info in infoTexts)
		{
            info.gameObject.SetActive(false);
		}
        infoTexts[qnNum].gameObject.SetActive(true);
        infoTexts[qnNum].text = movementInfo[ansNames.IndexOf(abcd[qnNum])];

        // Play animation
        anim.SetInteger("play-anim", ansNames.IndexOf(abcd[qnNum]));

        selectedAns = abcd[qnNum];
        selectedIndex = qnNum;
        infoPanel.SetActive(false);
    }

    private void RandomizeButtons(int currentQuestion)
	{
        // Clone the given list
        List<string> potentialNames = new List<string>(qm.movementNames);
        string answer = potentialNames[currentQuestion];
        potentialNames.RemoveAt(currentQuestion);

        for (int i = 0; i < abcd.Length; i++)
		{
            int index = Random.Range(0, potentialNames.Count);
            abcd[i] = potentialNames[index];
            abcdAnimIndex[i] = ansNames.IndexOf(potentialNames[index]);

            potentialNames.RemoveAt(index);
        }

        ansIndex = Random.Range(0, 3);
        abcd[ansIndex] = answer;
        abcdAnimIndex[ansIndex] = ansNames.IndexOf(answer);
    }

    public void Hint()
	{
        infoTitles[selectedIndex].text = CapitalizeFirstLetters(abcd[selectedIndex]);
        buttons[ansIndex].Hint(true);
    }

    private IEnumerator FlashRed(UIPointerHandler btn)
	{
        foreach (UIPointerHandler button in buttons)
        {
            button.interactable = false;
        }

        for (int i = 0; i < 4; i++)
        {
            btn.Activate(false);
            btn.interactable = false;
            yield return new WaitForSeconds(0.1f);

            btn.Activate(true);
            yield return new WaitForSeconds(0.5f);
        }

        foreach (UIPointerHandler button in buttons)
        {
            button.interactable = true;
        }
        btn.interactable = false;
    }

    // Submit the selected answer (submit button)
    public void Submit()
	{
        if (qm.learningMode && !qm.IsCorrect(selectedAns))
		{ // Prevent incorrect answers in learning mode
            StartCoroutine(FlashRed(buttons[selectedIndex]));
            submitButton.Activate(true);
        }
        else
		{
            anim.SetInteger("play-anim", -1);

            qm.AnswerQuestion(selectedAns);
        }
    }

    // Capitalize the first letter of each word in a given string - title creator
    private static string CapitalizeFirstLetters(string sentence)
    {
        char[] array = sentence.ToCharArray();
        if (array.Length >= 1)
        {
            if (char.IsLower(array[0]))
            {
                array[0] = char.ToUpper(array[0]);
            }
        }

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] == ' ')
            {
                if (char.IsLower(array[i + 1]))
                {
                    array[i + 1] = char.ToUpper(array[i + 1]); 
                }
            }
        }

        return new string(array);
    }
}
