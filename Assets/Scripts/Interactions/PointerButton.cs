using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class PointerButton : PointerHandler
{
    [Header("Stage Settings")]
    public int level = 0;
    public bool nextButton = false;
    public bool prevButton = false;

    [Header("Button Settings")]
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private GameManager gm;
    [SerializeField] [ColorUsage(true, true)] private Color activCol = Color.grey;
    [SerializeField] [ColorUsage(true, true)] private Color deactivCol = Color.black;
    [SerializeField] [ColorUsage(true, true)] private Color hoverCol = Color.cyan;
    [SerializeField] [ColorUsage(true, true)] private Color clickCol = Color.green;
    public bool changeCol;
    public bool activated = true;

    private Material btnMat;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonObject != null && buttonObject.GetComponent<Renderer>() != null)
		{
            btnMat = buttonObject.GetComponent<Renderer>().material;
            btnMat.SetColor("_EmissionColor", activCol);
        }

        if (!interactable)
		{
            btnMat.SetColor("_EmissionColor", deactivCol);
        }

        anim = GetComponent<Animator>();

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

        if (interactable && changeCol)
        {
            btnMat.SetColor("_EmissionColor", hoverCol);
        }
    }

    public new virtual void OnExitHover(BaseEventData eventData)
    {
        base.OnExitHover(eventData);

        if (interactable && changeCol)
        {
            btnMat.SetColor("_EmissionColor", activCol);
        }
    }

    public new virtual void OnClick(BaseEventData eventData)
    {
        if (interactable)
        {
            if (nextButton)
			{
                gm.NextButton();
            }
            else if (prevButton)
			{
                gm.PrevButton();
			}
            else
			{
                gm.PressButton(level);
			}
            anim.SetTrigger("press");

            if (changeCol)
			{
                btnMat.SetColor("_EmissionColor", hoverCol);
                StartCoroutine(WaitThenSetColor(0.5f, activCol));
            }

            if (interactableOnce)
            {
                interactable = false;
                changeCol = false;
            }
        }

        base.OnClick(eventData);
    }

    public void Activate(bool activ)
	{
        if (activ)
		{
            btnMat.SetColor("_EmissionColor", activCol);
        }
        else
		{
            btnMat.SetColor("_EmissionColor", deactivCol);
        }
        //interactable = activ;
        activated = activ;
        changeCol = activ;
    }

    public void ClickCol(bool activ)
    {
        if (activ)
        {
            btnMat.SetColor("_EmissionColor", clickCol);
        }
        else if (interactable)
        {
            btnMat.SetColor("_EmissionColor", activCol);
        }
        changeCol = !activ;
    }

    // Wait then set color
    private IEnumerator WaitThenSetColor(float time, Color col)
	{
        yield return new WaitForSeconds(time);

        if (changeCol)
		{
            btnMat.SetColor("_EmissionColor", col);
        }
	}


    public new void Click(BaseEventData data)
    {
        OnClick(data);
    }
}
