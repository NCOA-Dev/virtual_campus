using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [HideInInspector] public Camera cam;

    [Tooltip("Disable the automatic VR click (for PC builds).")]
    public bool disableClick = false;

    [SerializeField] private Teleporter homeTp;
    private Teleporter prevTp;

    [Header("Cursor Click")]
    [SerializeField] private Image clickRing;
    [SerializeField] private GameObject ringBase;
    [SerializeField] [Range(0, 1)] float clickProgress = 0f;

    // Altered by button interactions
    [HideInInspector] public bool isHovering = false;
    [HideInInspector] public float btnSpeedMultiplier = 1.0f;

    private readonly float clickSpeed = 1f;
    private bool hasPaused = false;
    private bool isPaused = false;
    private bool isOffPaused = false;

    private void Awake()
	{
        Instance = this;
        cam = Camera.main;
    }

    void Start()
    {
        if (homeTp != null)
		{
            prevTp = homeTp;
            prevTp.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isHovering)
		{
            if (!hasPaused && !isPaused)
			{
                StartCoroutine(HoverPause());
            }
            else if (hasPaused)
			{
                if (clickRing)
                {
                    clickRing.fillAmount = clickProgress;
                    clickProgress += Time.deltaTime * clickSpeed * btnSpeedMultiplier;
                }
            }

            if (clickProgress >= 1)
            {
                clickProgress = 0;
                clickRing.fillAmount = clickProgress;
                Click();
            }
        }
        else if (clickProgress > 0) 
		{
            clickRing.fillAmount = clickProgress;
            clickProgress -= Time.deltaTime * (clickSpeed * 100f * btnSpeedMultiplier);
        }
        else if (ringBase && ringBase.activeSelf && !isOffPaused)
		{
            clickProgress = 0;
            clickRing.fillAmount = clickProgress;
            StartCoroutine(HoverOffPause());
        }
    }

    public void StopHovering()
	{
        isHovering = false;
        clickProgress = 0;
        if (clickRing)
		{
            clickRing.fillAmount = clickProgress;
        }
        hasPaused = false;
        isPaused = false;
	}

    public void Teleport(Teleporter tp)
	{
        if (prevTp)
		{
            prevTp.tube.SetActive(true);
        }
        float y = transform.position.y;

        Vector3 newPos = tp.teleportLocation.transform.position;
        transform.position = new Vector3(newPos.x, y, newPos.z);
        tp.tube.SetActive(false);
        prevTp = tp;

        isHovering = false;
        ringBase.SetActive(false);
        hasPaused = false;
    }

    private void Click()
	{
		PointerEventData pointerData = new PointerEventData(EventSystem.current) {
			position = Input.mousePosition
		};
		List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            bool found = false;

            for (int i = 0; i < results.Count; i++)
			{
                if (!found)
				{
                    PointerHandler interactable = results[i].gameObject.GetComponent<PointerHandler>();
                    UIPointerHandler UIInteractable = results[i].gameObject.GetComponent<UIPointerHandler>();
                    Teleporter teleporter = results[i].gameObject.GetComponent<Teleporter>();
                    PointerHighlight highlight = results[i].gameObject.GetComponent<PointerHighlight>();
                    PointerButton btn = results[i].gameObject.GetComponent<PointerButton>();
                    PointerHazard hazard = results[i].gameObject.GetComponentInParent<PointerHazard>();

                    if (UIInteractable != null)
                    {
                        UIInteractable.Click(pointerData);
                        found = true;
                    }
                    if (hazard != null)
					{
                        hazard.Click(pointerData);
                        found = true;
                    }
                    else if (btn != null)
					{
                        btn.Click(pointerData);
                        found = true;
                    }
                    else if (teleporter != null)
                    {
                        teleporter.Click(pointerData);
                        found = true;
                    }
                    else if (highlight != null)
                    {
                        highlight.Click(pointerData);
                        found = true;
                    }
                    else if (interactable != null)
                    {
                        interactable.Click(pointerData);
                        found = true;
                    }
                }
            }
        }
    }

    private IEnumerator HoverPause()
	{
        if (disableClick)
		{
            yield return null;
		}
        else
		{
            isPaused = true;
            yield return new WaitForSeconds(0.2f);
            if (!ringBase.activeSelf)
            {
                ringBase.SetActive(true);
            }

            yield return new WaitForSeconds(0.4f);

            isPaused = false;
            if (ringBase && isHovering)
            {
                ringBase.SetActive(true);
                hasPaused = true;
            }
        }
    }

    private IEnumerator HoverOffPause()
    {
        isOffPaused = true;
        yield return new WaitForSeconds(0.01f);
        isOffPaused = false;

        if (ringBase && ringBase.activeSelf && !isHovering && clickRing.fillAmount < 0.1f)
        {
            clickRing.fillAmount = 0;
            ringBase.SetActive(false);
            hasPaused = false;
        }
    }
}
