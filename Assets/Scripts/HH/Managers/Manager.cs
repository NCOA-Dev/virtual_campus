using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QnManager))]
public class Manager : MonoBehaviour
{
    [Header("Anatomy Man")]
    [SerializeField] private GameObject man;

    [Header("Stage Layers")]
    [SerializeField] private GameObject[] layers;
    [SerializeField] private GameObject[] highlightLayers;

    [Tooltip("Extra body with QL for muscle stage")]
    [SerializeField] private GameObject extraMuscles;

    [Tooltip("Censor bar for movement stage")]
    [SerializeField] private GameObject censor;

    [Header("Non-VR Only")]
    [SerializeField] private bool zoomFOV = false;

    // Question manager
    private QnManager qm;

    // Spin variables
    private bool spin;
    private readonly float spinSpeed = 80f;
    private float currentSpinSpeed;

    // Zoom variables
    private bool zoom;
    private float zoomSpeed = -0.1f;
    private float currentZoomSpeed;
    private readonly float minZoom = 45f;
    private readonly float maxZoom = 90f;
    private readonly float minScale = 0.6f;
    private readonly float maxScale = 0.8f;

    // All highlight objects
    private PointerHighlight[] allHighlights;

    // Start is called before the first frame update
    void Start()
    {
        qm = GetComponent<QnManager>();
        allHighlights = GameObject.FindObjectsOfType<PointerHighlight>();

        if (zoomFOV)
		{
            zoomSpeed = 10;
        }

        // Deactivate all layers except starting layer
        foreach (GameObject layer in layers)
		{
            layer.SetActive(false);
		}
        layers[qm.startingStageIndex].SetActive(true);

        extraMuscles.SetActive(qm.startingStageIndex == 1);

        foreach (GameObject highlight in highlightLayers)
        {
            highlight.SetActive(false);
        }

        // 411 meaning all
        EnableHighlights(411, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (spin && man != null)
		{
            man.transform.RotateAround(man.transform.position, Vector3.up, currentSpinSpeed * Time.deltaTime);

            if (extraMuscles != null)
			{
                extraMuscles.transform.rotation = man.transform.rotation;
            }
        }

        if (zoom)
        {
            if (zoomFOV && Player.Instance != null)
			{
                Player.Instance.cam.fieldOfView = Mathf.Clamp(
                    Player.Instance.cam.fieldOfView + currentZoomSpeed * Time.deltaTime, minZoom, maxZoom);
            }
            else if (man != null)
			{
                float scale = Mathf.Clamp(man.transform.localScale.x + currentZoomSpeed * Time.deltaTime, minScale, maxScale);
                man.transform.localScale = new Vector3(scale, scale, scale);

                if (extraMuscles != null)
				{
                    extraMuscles.transform.localScale = man.transform.localScale;
                }

                if (censor != null)
                {
                    censor.transform.localScale = man.transform.localScale;

                    float y = censor.transform.position.y;
                    //y = (man.transform.localScale.z + 0.3f) * 0.03f;

                    // Move distance based on scale
                    censor.transform.position = new Vector3(censor.transform.position.x, y, 
                        man.transform.localScale.z - 0.7f);
                }
            }
        }
    }

    public void EnableStage(int stage)
	{
        foreach (GameObject layer in layers)
		{
            layer.SetActive(false);
		}

        layers[stage].SetActive(true);

        extraMuscles.SetActive(stage == 1);
        censor.SetActive(stage == 0);

        foreach (GameObject highlight in highlightLayers)
		{
            highlight.SetActive(false);
		}

        highlightLayers[stage].SetActive(true);
    }

    public void EnableHighlights(int stage, bool on)
	{
        foreach (PointerHighlight highlight in allHighlights)
		{
            if (highlight.stage == stage || stage == 411)
			{
                highlight.interactable = on;
            }
        }
	}

    public void Spin(bool right)
	{
        if (right)
		{
            currentSpinSpeed = -spinSpeed;
		}
        else
		{
            currentSpinSpeed = spinSpeed;
        }
        spin = true;
    }

    public void StopSpin()
	{
        spin = false;
    }

    public void Zoom(bool forward)
    {
        if (forward)
        {
            currentZoomSpeed = -zoomSpeed;
        }
        else
        {
            currentZoomSpeed = zoomSpeed;
        }
        zoom = true;
    }

    public void StopZoom()
    {
        zoom = false;
    }
}
