using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedUI : MonoBehaviour
{
    public bool onInEditor;

    [Range(2, 512)]
    public int meshresolution = 10;

    [Range(5, 100)]
    public float curveradius = 20f;

    [Range(5, 100)] public float height = 30f;

    [Range(30, 270)] public float chordangle = 180f;

    [SerializeField, HideInInspector]
    MeshFilter meshFilter;

    private MeshGenerator face;

    private void OnValidate()
    {
        if (onInEditor)
		{
            Initialize();
        }
    }

	private void Start()
	{
        Initialize();
    }

	void Initialize()
    {

        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = Resources.Load("Materials/CurvedUIMaterial", typeof(Material)) as Material;

        }

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshFilter.sharedMesh = MeshGenerator.GenerateMesh(meshresolution, curveradius, height, chordangle);
    }
}
