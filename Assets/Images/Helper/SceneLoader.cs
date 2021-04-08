using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private readonly float pauseTime = 0.2f;

    public void LoadLevel(int index)
	{
        StartCoroutine(LoadScene(index));
	}

    IEnumerator LoadScene(int index)
    {
        yield return new WaitForSeconds(pauseTime);

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
