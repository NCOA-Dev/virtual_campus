using UnityEngine;
using System.Collections;
using Mirror;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ReconnectManager : MonoBehaviour
{
	public void AttemptReconnect()
    {
        StartCoroutine(Reconnect());
    }

    private IEnumerator Reconnect()
    {
        // attempt

        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(0);
    }
}
