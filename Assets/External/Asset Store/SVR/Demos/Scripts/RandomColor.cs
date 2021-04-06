using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RandomColor : MonoBehaviour
{
    public void ApplyRandomColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        float a = Random.Range(0f, 1f);

        GetComponent<MeshRenderer>().material.color = new Color(r, g, b, a);
    }
}
