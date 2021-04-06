using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class Date : MonoBehaviour
{
    void Start()
    {
        TMP_Text dateText = GetComponent<TMP_Text>();
        string day = System.DateTime.Now.ToString("dd MM yy");
        dateText.text = day;
    }
}
