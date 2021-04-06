using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class Teleporter : PointerHandler
{
	[Header("Teleporter Only Variables")]
	[SerializeField] public GameObject teleportLocation;
	[SerializeField] public GameObject tube;

	[Header("Material Variables")]
	private Material[] objMats;
	public bool changeColor = true;
	public Color origCol = Color.white;
	public Color hoverCol = Color.white;
	public Color clickCol = Color.white;

	// Start is called before the first frame update
	void Awake()
    {

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

		//tube = this.gameObject;
		objMats = tube.GetComponent<Renderer>().materials;
	}


	public new virtual void OnHover(BaseEventData eventData)
    {
        base.OnHover(eventData);

		if (changeColor)
		{
			SetMeshCols(hoverCol);
		}

		interactable = true;
	}

    public new virtual void OnExitHover(BaseEventData eventData)
    {
        base.OnExitHover(eventData);

		if (changeColor)
		{
			SetMeshCols(origCol);
		}
	}


	public new virtual void OnClick(BaseEventData eventData)
    {
		if (changeColor)
		{
			SetMeshCols(origCol);
		}

		if (transform.CompareTag("Teleporter"))
		{
			Player.Instance.Teleport(this);
		}

		base.OnClick(eventData);
	}

	public new void Click(BaseEventData data)
	{
		OnClick(data);
	}

	private void SetMeshCols(Color col)
	{
		foreach (Material mat in objMats)
		{
			mat.SetColor("_EmissionColor", col);
		}
	}
}
