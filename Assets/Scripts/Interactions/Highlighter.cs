using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Highlighter : MonoBehaviour
{
    [Header("Name of highlight area")]
    public string areaName = "";

    // List of meshes in area to highlight
    [SerializeField] private MeshRenderer[] meshList;
    [SerializeField] private Material highlightMat;
    private List<GameObject> highlightObjs = new List<GameObject>();

    [Header("Whether the object reacts to hover events")]
    public bool hoverOn = true;
    private bool isHovering = false;

    public UnityEvent onClick;

    // Start is called before the first frame update
    void Start()
    {
        if (meshList.Length == 0 || highlightMat == null)
		{
            Debug.LogError("Please assign values to " + this.gameObject);
		}
        else
		{
            CreateHighlightMeshes();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        // Record if hovering even if not ready to change colour
        isHovering = true;


        if (hoverOn)
        {
            Highlight(true);
        }
    }

    private void OnMouseExit()
    {
        isHovering = false;

        if (hoverOn)
        {
            Highlight(false);
        }
    }

    private void OnMouseUp()
    {
        if (hoverOn && isHovering)
        {
            onClick.Invoke();

        }
    }

    public void ChangeToHoveredColour()
    {
    }

    public void ChangeToOriginalColour()
	{

	}

    private void Highlight(bool on)
	{
        foreach (GameObject obj in highlightObjs)
		{
            if (obj)
			{
                obj.SetActive(on);
			}
		}
	}

    private void CreateHighlightMeshes()
    {
        foreach (MeshRenderer rend in meshList)
        {
            GameObject obj;
            if (rend.gameObject.transform != null)
			{
                obj = Instantiate(rend.gameObject, rend.transform) as GameObject;
                obj.transform.localScale *= 1.001f;
                obj.transform.position = obj.transform.parent.position;

                MeshRenderer newRend = obj.GetComponent<MeshRenderer>();

                if (obj && newRend.material != null && highlightMat != null)
                {
                    newRend.material = highlightMat;
                    newRend.material.EnableKeyword("_EMISSION");

                    highlightObjs.Add(obj);
                    obj.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("Material not found");
                }
            }
            else
            {
                Debug.LogWarning("Mesh not found");
            }
            
        }
    }
}
