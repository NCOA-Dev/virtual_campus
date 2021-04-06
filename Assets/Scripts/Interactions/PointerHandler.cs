using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(EventTrigger))]
public class PointerHandler : MonoBehaviour
{
    [Tooltip("Whether the object is currently interactable.")]
    public bool interactable;
    
    [Tooltip("Whether the object is interactable only once.")]
    public bool interactableOnce;

    [Tooltip("Only cue hover events - for objects that dont want to be clicked.")]
    public bool hoverOnly;

    [Header("Events")]
    public UnityEvent doOnHover;
    public UnityEvent doOnExitHover;
    public UnityEvent doOnClick;

    //[Header("SFX")]
    //[SerializeField] private GameObject downSound = null;
    //[SerializeField] private GameObject upSound = null;
    //[SerializeField] private GameObject clickSound = null;
    //[SerializeField] private GameObject errorSound = null;
    //[SerializeField] private GameObject dragSound = null;
    //[SerializeField] private GameObject dropSound = null;

    [HideInInspector] public bool hovering = false;

    [Header("Adjustments")]
    public float speedMultiplier = 1.0f;
    public float interactPause = 0.2f;

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
	}

	private void Update()
	{
		if (Player.Instance != null && Player.Instance.isActiveAndEnabled && false)
		{
            PointerEventData mouse1 = EventSystem.current.gameObject.GetComponent<StandaloneInputModuleCustom>().GetLastPointerEventDataPublic(-1);
            OnClick(mouse1);
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

    public void OnHover(BaseEventData eventData)
    {
        if (hoverOnly)
		{
            doOnHover.Invoke();
            hovering = true;
		}
        else if (interactable)
        {
            Player.Instance.isHovering = true;
            Player.Instance.btnSpeedMultiplier = speedMultiplier;

            doOnHover.Invoke();

            hovering = true;
        }

    }

    public void OnExitHover(BaseEventData eventData)
    {
        if (hoverOnly && hovering)
		{
            doOnExitHover.Invoke();
            hovering = false;
        }
        else if (hovering)
        {
            Player.Instance.StopHovering();
            doOnExitHover.Invoke();
            hovering = false;
        }
    }

    public void OnDown(BaseEventData eventData)
    {
        if (interactable)
        {
        }
    }

    public void OnUp(BaseEventData eventData)
    {
        if (interactable)
        {
        }
    }

    public void OnClick(BaseEventData eventData)
    {
        if (interactable)
        {
            doOnClick.Invoke();

            if (this.isActiveAndEnabled && !interactableOnce)
            {
                StartCoroutine(InteractablePause());
            }
            else
            {
                hovering = false;
                interactable = false;
                Player.Instance.StopHovering();
            }
        }
    }

    public void OnDrag(BaseEventData eventData)
    {
    }

    public void OnEndDrag(BaseEventData eventData)
    {
    }

    private IEnumerator InteractablePause()
    {
        interactable = false;
        Player.Instance.StopHovering();
        yield return new WaitForSeconds(interactPause);
        interactable = true;
        if (hovering)
		{
            Player.Instance.isHovering = true;
		}
    }

    public void Click(BaseEventData data)
    {
        OnClick(data);
    }
}
