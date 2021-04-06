using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(EventTrigger))]
public class UIPointerHandler : MonoBehaviour
{
    [Tooltip("Whether this UI item is currently interactable")]
    public bool interactable;

    [Tooltip("Only allow hover events, not clicks.")]
    public bool noClick; 
    public bool interactableOnce;
    public bool changeColor = false;
    private Color origCol = Color.white;
    public Color hoverCol = Color.white;
    public Color clickCol = Color.white;
    public Color activatedCol = Color.clear;
    public Color activatedHoverCol = Color.clear;
    public Color activatedClickCol = Color.clear;
    public Color hintCol = Color.clear;
    private Color trueOrigCol;

    [Header("Events")]
    public UnityEvent doOnHover;
    public UnityEvent doOnExitHover;
    public UnityEvent doOnClick;

    [Header("Custom Click Speed")]
    public float speedMultiplier = 1.0f;

    //[Header("SFX")]
    //[SerializeField] private GameObject downSound = null;
    //[SerializeField] private GameObject upSound = null;
    //[SerializeField] private GameObject clickSound = null;
    //[SerializeField] private GameObject errorSound = null;
    //[SerializeField] private GameObject dragSound = null;
    //[SerializeField] private GameObject dropSound = null;

    private bool startedInteractable = false;
    private bool startedChangeCol = true;
    private bool hovering = false;
    private bool activated = false;
    private Image img;

    private void OnEnable()
	{
        img = GetComponent<Image>();
		if (origCol == Color.white)
		{
            origCol = img.color;

            // Keep track of which values occurred at the first OnEnable
            startedChangeCol = changeColor;
            trueOrigCol = origCol;

            startedInteractable = interactable;
        }

        if (startedInteractable)
		{
            interactable = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
            EventTriggerType.PointerDown,
            OnDown);

        AddEventTriggerListener(
            GetComponent<EventTrigger>(),
            EventTriggerType.PointerUp,
            OnUp);

        AddEventTriggerListener(
            GetComponent<EventTrigger>(),
            EventTriggerType.PointerClick,
            OnClick);
    }
    private void OnDisable()
    {
        if (hovering)
        {
            Player.Instance.StopHovering();
        }
        if (img != null && origCol != null)
		{
            img.color = origCol;
            changeColor = startedChangeCol;
        }
    }

    public static void AddEventTriggerListener(EventTrigger trigger,
                            EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }

    private void OnHover(BaseEventData eventData)
    {
        if (interactable)
        {
            if (changeColor && activated)
            {
                img.color = activatedHoverCol;
            }
            else if (changeColor)
            {
                img.color = hoverCol;
            }

            if (!noClick)
			{
                Player.Instance.isHovering = true;
                Player.Instance.btnSpeedMultiplier = speedMultiplier;
            }

            doOnHover.Invoke();

            hovering = true;
        }
    }

    private void OnExitHover(BaseEventData eventData)
    {
        if (hovering)
        {
            if (changeColor && interactable)
            {
                img.color = origCol;
            }

            Player.Instance.StopHovering();

            doOnExitHover.Invoke();
        }
        hovering = false;
    }

    private void OnDown(BaseEventData eventData)
    {
        if (interactable)
        {
        }
    }

    private void OnUp(BaseEventData eventData)
    {
        if (interactable)
        {
        }
    }

    private void OnClick(BaseEventData eventData)
    {
        if (interactable)
        {
            doOnClick.Invoke();

            if (changeColor && activated)
            {
                img.color = activatedClickCol;
            }
            else if (changeColor)
            {
                img.color = clickCol;
            }

            if (interactableOnce)
            {
                interactable = false;
                changeColor = false;
                Player.Instance.StopHovering();
            }
            else if (this.isActiveAndEnabled)
			{
                StartCoroutine(InteractablePause());
            }
        }
    }

    private void OnDrag(BaseEventData eventData)
    {
    }

    private void OnEndDrag(BaseEventData eventData)
    {
    }

    private IEnumerator InteractablePause()
    {
        interactable = false;

        yield return new WaitForSeconds(0.25f);

        if (!interactableOnce)
		{
            interactable = true;
        }

        if (!hovering && changeColor)
		{
            img.color = origCol;
		}
        if (hovering && changeColor)
		{
            img.color = hoverCol;
        }
    }

    public void Click(BaseEventData data)
    {
        OnClick(data);
    }

    public void Activate(bool on)
	{
        if (on && img != null)
		{
            img.color = activatedCol;
        }
        else if (img != null)
		{
            img.color = origCol;
        }
        changeColor = !on;
        interactable = !on;
    }

    // Change to activate color without disabling
    public void ActivateColor(bool on)
    {
        if (on && img != null)
        {
            origCol = activatedCol;
        }
        else if (img != null)
        {
            origCol = trueOrigCol;
        }
        img.color = origCol;
        activated = on;
    }

    public void Hint(bool on)
    {
        if (on && img != null)
        {
            img.color = hintCol;
        }
        else if (img != null)
        {
            img.color = origCol;
        }

        changeColor = false;
        interactable = on;
    }
}
