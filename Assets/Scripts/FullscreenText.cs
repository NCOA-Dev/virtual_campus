using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FullscreenText : MonoBehaviour
{
    private TMP_Text fstext;
    private bool fullscreen = false;
    private bool wasFullscreen = false;

    // Start is called before the first frame update
    void Start()
    {
        fstext = GetComponent<TMP_Text>();

#if UNITY_STANDALONE
        StartCoroutine(FadeOut());
#endif
    }

    // Update is called once per frame
    void Update()
    {
        // Enter/exit fullscreen for windows
#if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Screen.fullScreen)
			{
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else
			{
                Screen.fullScreen = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        fullscreen = Screen.fullScreen;

        if (fullscreen && !wasFullscreen)
        {
            StartCoroutine(FadeOut());
        }

        wasFullscreen = fullscreen;
#endif

        // Fade UI text on fullsreen WebGL
#if UNITY_WEBGL
        fullscreen = Screen.fullScreen;

        if (fullscreen && !wasFullscreen)
        {
            StartCoroutine(FadeOut());
        }

		wasFullscreen = fullscreen;
#endif
    }

    private IEnumerator FadeOut()
	{
        fstext.color = new Color(fstext.color.r, fstext.color.g, fstext.color.b, 1f);
        yield return new WaitForSeconds(3f);
        while (fstext.color.a > 0) {
            yield return new WaitForSeconds(0.01f);
            fstext.color = new Color(fstext.color.r, fstext.color.g, fstext.color.b, fstext.color.a - 0.01f);
		}
	}
}
