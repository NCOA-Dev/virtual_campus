using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHazard : PointerHandler
{
    [Header("Hazard Settings")]
    public string hazardDesc = "";
    private HazardManager hm;

    //[Header("Question Manager")]
    //[SerializeField] private QnManager qm;

    // Highlight emission colours to swap between
    [ColorUsage(true, true)] private Color highlightCol = new Color(0.75f, 0.65f, 0.25f, 1f);
    [ColorUsage(true, true)] private Color highlightDoneCol = new Color(0.1f, 0.75f, 0.1f, 1f);
    [ColorUsage(true, true)] private Color highlightWrongCol = new Color(0.745f, 0f, 0f, 1f);

    // Highlighters objects to create
    private List<GameObject> highlightObjs = new List<GameObject>();
    private List<GameObject> highlightDoneObjs = new List<GameObject>();
    private List<GameObject> highlightWrongObjs = new List<GameObject>();
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

    [Header("Optional Settings")]
    [Tooltip("View highlight through other objects.")]
    public bool seeThrough = false;

    // Alternate highlight methods
    [Tooltip("Replace highlight object instead of using transparent material (only works on standard shader).")]
    public bool replaceHighlight = true;
    [Tooltip("Multiplier of highlight intensity.")]
    public float highlightMultiplier = 1f;


    // Translation offset of effect
    [SerializeField] private Vector3 meshHoverTrans = Vector3.zero;

    // Scale offset of hover effect
    [SerializeField] private Vector3 meshHoverScale = Vector3.one;
    [SerializeField] private MeshRenderer ignoreOffsetMesh;

    public virtual void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
		{ // Alternate highlight colours for WebGL
            highlightCol = new Color(0.8f, 0.7f, 0.3f, 1f);
            highlightDoneCol = new Color(0.3f, 0.8f, 0.1f, 1f);
            highlightWrongCol = new Color(0.8f, 0.1f, 0.1f, 1f);
        }

        highlightObjs = CreateHighlightMeshes(highlightCol);
        highlightDoneObjs = CreateHighlightMeshes(highlightDoneCol);
        highlightWrongObjs = CreateHighlightMeshes(highlightWrongCol);
        currentHighlights = highlightObjs;

        hm = GameObject.FindObjectOfType<HazardManager>();

        if (hm == null)
		{
            Debug.LogError("HazardManager not found by " + this.gameObject.name);
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
        if (interactable)
        {
            if (stopIncorrectAnwers) //&& !qm.IsCorrect(areaName))
            { // Prevent question being answered if incorrect in learning mode
                StartCoroutine(IncorrectFlash());
            }
            else
            { // Answer question
                //qm.AnswerQuestion(areaName);
                hm.FoundHazard(hazardDesc);

                completed = true;
                hintBlue = false;

                Highlight(false);
                currentHighlights = highlightDoneObjs;
                Highlight(true);

                //qm.RecHighlight(this);
            }
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

            if (replaceHighlight)
			{
                // Disable highest parent mesh renderer
                MeshRenderer[] mrs = obj.GetComponentsInParent<MeshRenderer>();
                mrs[mrs.Length - 1].enabled = !on;
            }

            // Set layer to view through objects
            if ((seeThrough && (!on || completed)) || flashing)
            {
                obj.layer = 0;
            }
            else if (on && seeThrough)
            {
                obj.layer = highlightLayer;
            }
        }
    }

    // Initializes highlight meshes from given material (called on start)
    // Returns list of highlight objects
    private List<GameObject> CreateHighlightMeshes(Color col)
    {
        List<GameObject> newHighlightObjects = new List<GameObject>();
        MeshRenderer[] meshList = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer rend in meshList)
        {
            if (rend != null)
            {
                GameObject obj;
                obj = Instantiate(rend.gameObject, rend.transform);
                //DestroyImmediate(obj.GetComponent<MeshRenderer>());
                //obj.AddComponent<MeshRenderer>();

                // Create a new material
                Material mat;
                if (replaceHighlight)
                {
                    mat = new Material(rend.material);
                    
                    // Ensure standard shader is used
                    if (mat.shader != Shader.Find("Standard"))
					{
                        mat.shader = Shader.Find("Standard");
                    }
                }
                else
				{
                    mat = new Material(Shader.Find("Standard"));
                    mat.color *= 0.001f;

                    // Change to transparent material
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetFloat("_Mode", 3);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                }

                // Remove color, smoothness, and activate set emission color
                mat.SetFloat("_Glossiness", 0f);
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", col * highlightMultiplier);
                mat.color *= highlightMultiplier;

                mat.renderQueue = 3100;

                // Re-apply to refresh shader
                //mat.shader = Shader.Find(mat.shader.name);
                mat.EnableKeyword("_EMISSION");

                // Remove new object's children
                foreach (Transform child in obj.transform)
                {
                    Destroy(child.gameObject);
                }

                obj.transform.rotation = obj.transform.parent.rotation;
                obj.GetComponent<Renderer>().material = mat;

                // Offset scale and position
                obj.transform.localScale = meshHoverScale;
                obj.transform.localPosition = meshHoverTrans;

                if (ignoreOffsetMesh != null)
                {
                    if (rend == ignoreOffsetMesh)
					{
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                    }
                }

                newHighlightObjects.Add(obj);
                obj.SetActive(false);
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
            currentHighlights = highlightWrongObjs;
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
