using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlsPrinter : MonoBehaviour
{
    public TMP_Text keyText;
    public TMP_Text clickText;
    public TMP_Text mouseTextX;
    public TMP_Text mouseTextY;

    Coroutine prevClick;
    Coroutine prevKey;
    Coroutine prevMouse;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            clickText.text = "LMB";
            if (prevClick != null)
            {
                StopCoroutine(prevClick);
            }
            prevClick = StartCoroutine(WaitThenRem(clickText));
        }
        else if (Input.GetMouseButton(1))
        {
            clickText.text = "RMB";

            if (prevClick != null)
			{
                StopCoroutine(prevClick);
            }
            prevClick = StartCoroutine(WaitThenRem(clickText));
        }
        else if (Input.GetMouseButton(2))
        {
            clickText.text = "MMB";
            if (prevClick != null)
            {
                StopCoroutine(prevClick);
            }
            prevClick = StartCoroutine(WaitThenRem(clickText));
        }

        if (Input.GetAxis("Mouse X") < 0)
        {
            mouseTextX.text = "Left: " + Input.GetAxis("Mouse X");
        }
        if (Input.GetAxis("Mouse X") > 0)
        {
            mouseTextX.text = "Right: " + Input.GetAxis("Mouse X");
        }
        if (Input.GetAxis("Mouse Y") < 0)
        {
            mouseTextY.text = "Down: " + Input.GetAxis("Mouse Y");
        }
        if (Input.GetAxis("Mouse Y") > 0)
        {
            mouseTextY.text = "Up: " + Input.GetAxis("Mouse Y");
        }
    }
        
    void OnGUI()
    {
        Event e = Event.current;

        if (e.isKey)
        {
            string key = e.keyCode.ToString();
            if (key == "None")
			{
                key = "";
			}

            keyText.text = key;
        }

        if (Input.GetKeyUp(e.keyCode))
		{
            if (prevKey != null)
            {
                StopCoroutine(prevKey);
            }
            prevKey = StartCoroutine(WaitThenRem(keyText));
        }
    }

    private IEnumerator WaitThenRem(TMP_Text text)
	{
        yield return new WaitForSeconds(2.0f);
        text.text = "";
	}
}
