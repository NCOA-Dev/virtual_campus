using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: Will need separate functionality for VR
// -Sync to main camera rather than player go

public class Clipboard : MonoBehaviour
{
    Transform parent;
    [SerializeField] private GameObject player;

    private bool lookingAtClipboard = false;
    private bool lookedForLong = false; // looked for long enough for it to matter
    private readonly float lookTime = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent;
    }

    void Update()
    {
        if (lookedForLong && transform.parent != null)
		{ // Detach from parent when looking at clipboard
            transform.parent = null;
		}
		else if (!lookedForLong && transform.parent == null)
		{ // Slowly rotate towards clipboard instead of jumping instantly
            Quaternion rotateTo = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, 4f * Time.deltaTime);

            if (Mathf.Abs(transform.eulerAngles.y - player.transform.eulerAngles.y) < 1f)
			{ // Set the parent - ending this rotation
                transform.parent = parent;
            }
        }
		else if (!lookedForLong)
		{ // If the player isnt looking at the clipboard, rotate its yaw to player's yaw
            transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        }

        // Ensure clipboard isn't lost when player moves
        transform.position = parent.transform.position;
    }

    public void Looking(bool looking)
	{
        lookingAtClipboard = looking;

        StartCoroutine(LookForTime(lookTime));
	}

    private IEnumerator LookForTime(float time)
	{
        lookedForLong = false;

        yield return new WaitForSeconds(time);

        lookedForLong = lookingAtClipboard;
    }
}
