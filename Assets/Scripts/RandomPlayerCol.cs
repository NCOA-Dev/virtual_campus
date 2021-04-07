﻿using UnityEngine;
using Mirror;

public class RandomPlayerCol : NetworkBehaviour
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    // Color32 packs to 4 bytes
    [SyncVar(hook = nameof(SetColor))]
    public Color32 color = Color.black;

    [Tooltip("Material recognised as color-changing.")]
    public Material changeColorMat;

    // Unity clones the material when GetComponent<Renderer>().material is called
    // Cache it here and destroy it in OnDestroy to prevent a memory leak
    Material[] cachedMaterials;

    void SetColor(Color32 _, Color32 newColor)
    {
        if (cachedMaterials == null)
        {
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            cachedMaterials = new Material[rends.Length];

            for (int i = 0; i < cachedMaterials.Length; i++)
			{
                if (rends[i].material != null)
				{
                    cachedMaterials[i] = rends[i].material;
                    if (cachedMaterials[i].name == changeColorMat.name + " (Instance)")
					{
                        cachedMaterials[i].color = newColor;
                    }
                }
			}
        }
    }

    void OnDestroy()
    {
        if (cachedMaterials != null)
		{
            foreach (Material cachedMaterial in cachedMaterials)
            {
                if (cachedMaterial != null)
				{
                    Destroy(cachedMaterial);
                }
            }
        }
    }
}
