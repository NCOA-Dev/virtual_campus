using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHighlight : PointerHandler
{
    [Header("Name of highlight area")]
    public string areaName = "";
    public int stage = -1;

    [Header("Question Manager")]
    [SerializeField] private QnManager qm;

    [Header("Lists of meshes to highlight")]
    [SerializeField] private MeshRenderer[] meshList;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Material highlightDoneMat;
    private List<GameObject> highlightObjs = new List<GameObject>();
    private List<GameObject> highlightDoneObjs = new List<GameObject>();
    private List<GameObject> highlightErrorObjs = new List<GameObject>();
    private List<GameObject> highlightHintObjs = new List<GameObject>();
    private List<GameObject> currentHighlights;

    // Whether highlight is completed
    private bool completed = false;

    // Whether QnManager is in learning mode
    private bool stopIncorrectAnwers = false;

    // Whether highlight is flashing after an incorrect answer
    private bool flashing = false;

    // Whether highlight is blue and shown as hint
    private bool hintBlue = false;

    // Layer index of object (to display through objects with depth camera)
    private readonly int highlightLayer = 8;

    [Header("Incorrect highlight")]
    [SerializeField] private Material incorrectHighlight;
    [SerializeField] private Material incorrectHighlightTrans;

    [Header("Hint highlight")]
    [SerializeField] private Material hintHighlight;
    [SerializeField] private Material hintHighlightTrans;

    [Header("Optional: View through other objects")]
    public bool seeThrough = false;

    [Header("Optional: Transparent highlight")]
    [SerializeField] private bool hasTransparentMesh = false;
    [SerializeField] private int transMeshIndex = 1;
    [SerializeField] private Material matTrans;
    [SerializeField] private Material doneMatTrans;

    [Tooltip("Whether this highlighter is a bony landmark.")]
    public bool landmark = false;

    [Tooltip("Whether this highlighter is for the tutorial only.")]
    public bool tutorial = false;

    // Scale offset of landmark hover effect
    private Vector3 meshHoverScale = new Vector3(1.5f, 1.5f, 1.5f);

    public virtual void Start()
    {
        if (tutorial)
		{
            highlightObjs = CreateHighlightMeshes(highlightMat);
            highlightDoneObjs = CreateHighlightMeshes(highlightDoneMat);
            currentHighlights = highlightObjs;
        }
        else if (qm == null || meshList.Length == 0 || highlightMat == null || highlightDoneMat == null || qm == null
                 || incorrectHighlight == null || hintHighlight == null)
        {
            Debug.LogError("Please assign all variables to " + gameObject);
        }
        else
        {
            if (hasTransparentMesh && doneMatTrans != null)
            {
                highlightDoneMat = doneMatTrans;
            }

            highlightObjs = CreateHighlightMeshes(highlightMat);
            highlightDoneObjs = CreateHighlightMeshes(highlightDoneMat);
            highlightErrorObjs = CreateHighlightMeshes(incorrectHighlight);
            highlightHintObjs = CreateHighlightMeshes(hintHighlight);
            currentHighlights = highlightObjs;

            stopIncorrectAnwers = qm.learningMode;
        }

        // Initialize custom trigger events
        AddEventTriggerListener(
            GetComponent<EventTrigger>(),
            EventTriggerType.PointerEnter,
            OnHover);
        AddEventTriggerListener(
            GetComponent<EventTrigger>(),
            EventTriggerType.PointerExit,
            OnExitHover);
        AddEventTriggerListener(
            GetComponent<EventTrigger>(),
            EventTriggerType.PointerClick,
            OnClick);
    }

    public new virtual void OnHover(BaseEventData eventData)
    {
        base.OnHover(eventData);

        if (interactable && !completed)
        {
            if (hintBlue)
            {
                Highlight(false);
                currentHighlights = highlightObjs;
            }
            Highlight(true);
        }
    }

    public new virtual void OnExitHover(BaseEventData eventData)
    {
        base.OnExitHover(eventData);

        if (interactable && !completed)
        {
            Highlight(false);
            if (hintBlue)
            {
                currentHighlights = highlightHintObjs;
                Highlight(true);
            }
        }
    }

    public new virtual void OnClick(BaseEventData eventData)
    {
        if (interactable && !tutorial)
        {
            if (stopIncorrectAnwers && !qm.IsCorrect(areaName))
            { // Prevent question being answered if incorrect in learning mode
                StartCoroutine(IncorrectFlash());
            }
            else
            { // Answer question
                qm.AnswerQuestion(areaName);

                completed = true;
                hintBlue = false;

                Highlight(false);
                currentHighlights = highlightDoneObjs;
                Highlight(true);

                if (hasTransparentMesh)
                {
                    Collider[] cols = GetComponents<Collider>();

                    foreach (Collider col in cols)
                    {
                        if (col)
                        {
                            col.enabled = false;
                        }
                    }
                }

                qm.RecHighlight(this);
            }
        }
        else if (tutorial)
		{
            Highlight(false);
            currentHighlights = highlightDoneObjs;
            Highlight(true);
        }

        base.OnClick(eventData);
    }

    // Undo this highlight
    public void UndoHighlight()
    {
        completed = false;

        Highlight(false);
        currentHighlights = highlightObjs;
        interactable = true;
        hintBlue = false;

        if (hasTransparentMesh)
        {
            Collider[] cols = GetComponents<Collider>();

            foreach (Collider col in cols)
            {
                if (col)
                {
                    col.enabled = true;
                }
            }
        }
    }

    // Switch this highlight on or off
    private void Highlight(bool on)
    {
        foreach (GameObject obj in currentHighlights)
        {
            if (obj)
            {
                obj.SetActive(on);
            }

            // Set layer to view through objects
            if (landmark && ((seeThrough && (!on || completed)) || flashing)) 
            {
                obj.layer = highlightLayer;
            } 
            else if ((seeThrough && (!on || completed)) || flashing)
            {
                obj.layer = 0;
            }
            else if (on && seeThrough)
            {
                obj.layer = highlightLayer;
            }
        }

        foreach (MeshRenderer mesh in meshList)
		{
            if (mesh != null)
			{
                mesh.gameObject.GetComponent<MeshRenderer>().enabled = !on;
			}
		}
    }

    // Initializes highlight meshes from given material (called on start)
    // Returns list of highlight objects
    private List<GameObject> CreateHighlightMeshes(Material mat)
    {
        List<GameObject> newHighlightObjects = new List<GameObject>();

        foreach (MeshRenderer rend in meshList)
        {
            if (rend != null)
            {
                GameObject obj;
                obj = Instantiate(rend.gameObject, rend.transform);

                // Remove new object's children
                foreach (Transform child in obj.transform)
                {
                    Destroy(child.gameObject);
                }

                //obj.transform.localScale *= 1.001f;
                obj.transform.rotation = obj.transform.parent.rotation;
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;

                if (mat == highlightMat && landmark)
				{
                    obj.transform.localScale = meshHoverScale;
                }

                if (mat == incorrectHighlight && !landmark)
				{
                    obj.layer = 0;
				}

                MeshRenderer newRend = obj.GetComponent<MeshRenderer>();

                if (obj && newRend.material && hasTransparentMesh && rend == (MeshRenderer)meshList.GetValue(transMeshIndex))
				{ // Set transparent material if set transparent mesh
                    if (mat == highlightDoneMat)
                    {
                        newRend.material = doneMatTrans;
                    }
                    else if (mat == incorrectHighlight)
					{
                        newRend.material = incorrectHighlightTrans;
                    }
                    else if (mat == hintHighlight)
                    {
                        newRend.material = hintHighlightTrans;
                    }
                    else
                    {
                        newRend.material = matTrans;
                    }

                    newRend.material.EnableKeyword("_EMISSION");

                    newHighlightObjects.Add(obj);
                    obj.SetActive(false);
                }
                else if (obj && newRend.material != null && mat != null)
                { // Otherwise set as given material
                    newRend.material = mat;
                    newRend.material.EnableKeyword("_EMISSION");

                    newHighlightObjects.Add(obj);
                    obj.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("Material not found!");
                }
            }
            else
            {
                Debug.LogWarning("Mesh not found");
            }
        }

        return newHighlightObjects;
    }

    // Trigger custom click event by player click
    public new void Click(BaseEventData data)
    {
        OnClick(data);
    }

    // Trigger hint highlighter
    public void Hint()
    {
        Highlight(false);
        currentHighlights = highlightHintObjs;
        Highlight(true);

        hintBlue = true;
    }

    // Reset hint highlighter and remove
    public void RemHint()
    {
        if (hintBlue)
		{
            Highlight(false);
            currentHighlights = highlightObjs;
            Highlight(true);
            Highlight(false);

            hintBlue = false;
        }
    }

    // Flash red mesh when incorrect answer is selected
    private IEnumerator IncorrectFlash()
	{
        flashing = true;
        interactable = false;

        for (int i = 0; i < 4; i++)
		{
            Highlight(false);
            yield return new WaitForSeconds(0.1f);
            currentHighlights = highlightErrorObjs;
            Highlight(true);

            yield return new WaitForSeconds(0.5f);
        }

        Highlight(false);
        currentHighlights = highlightObjs;

        interactable = true;
        flashing = false;
    }

    // Remove all incorrect and hint highlights when disabled
	private void OnDisable()
	{
        if (flashing)
		{
            Highlight(false);
            currentHighlights = highlightObjs;

            interactable = true;
            flashing = false;
        }

        RemHint();
    }
}
