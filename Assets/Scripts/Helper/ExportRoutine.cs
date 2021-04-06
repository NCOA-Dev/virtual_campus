using System.Collections;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExportRoutine : MonoBehaviour
{
	[Tooltip("Number of frames to be captured within 360 degrees around the model.")]
	[SerializeField] private int totalAngles = 16;

	[Tooltip("Margin as a percentage (0-50)% of the model's length for each screenshot image - 0 being no margin.")]
	[Range(0.0f, 50.0f)]
	[SerializeField] private float imageMargin = 8.0f;

	private Camera screenshotCam;

	public GameObject objToCapture;

	public int imgWidth = 1024;
	public int imgHeight = 1024;

	public bool singleShot = false;

	private void Start()
	{
		// Get tagged depth-only camera to use with screenshots
		screenshotCam = Camera.main;

		// Start the asset capturing routine
		StartCoroutine(CaptureObject(objToCapture, objToCapture.transform.name));
	}


	/// <summary>
	/// Captures screenshots of a given object and saves to a subfolder of the Output directory.
	/// Screenshots occur at angles around the object defined by numSlices.
	/// </summary>
	/// <param name="obj">The object to capture.</param>
	/// <param name="name">The name of the object and its folder.</param>
	private IEnumerator CaptureObject(GameObject obj, string name)
	{
		// Ensure the object fits appropriately in view
		//FitObjectInView(obj);

		// Define the angle to rotate between with each frame
		float angle = 360.0f / totalAngles;

		// Create a folder within the Output folder named after the object
		string objectPath = "Output/" + name;
		CreateFolderInRoot(objectPath);

		if (singleShot)
		{
			SaveTextureToFile(Screenshot(), objectPath, 1);

			yield return null;
		}
		else
		{
			// Iterate through the angles and save a screenshot of each frame to the defined folder
			for (int i = 0; i < totalAngles; i++)
			{
				SaveTextureToFile(Screenshot(), objectPath, i + 1);

				obj.transform.Rotate(0, angle, 0, Space.Self);

				yield return null;
			}
		}
	}

	/// <summary>
	/// Adjust the orthographic camera size to fit the object - at its longest side.
	/// Ensures an object will always fit in view no matter how it is rotated.
	/// Margin between the object and the edge of the camera can be adjusted in the editor.
	/// </summary>
	/// <param name="obj">The object to fit insize the screen.</param>
	private void FitObjectInView(GameObject obj)
	{
		// Get all renderers attached to the object and its children
		Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

		// Store the total bounds of the objects
		Bounds objBounds;

		// Combine all renderer bounding boxes 
		if (renderers.Length > 0)
		{
			objBounds = renderers[0].bounds;

			for (int i = 1; i < renderers.Length; i++)
			{
				objBounds.Encapsulate(renderers[i].bounds);
			}
		}
		else
		{
			Debug.LogError("There are no renderes attached to {0}.", obj);
			return;
		}

		// Get the length of the longest side of the bounding box 
		Vector3 objectSize = objBounds.size;
		float longestSide = Mathf.Max(objectSize.x, objectSize.y, objectSize.z);

		// Centre the object at (0, 0, 0)
		obj.transform.localPosition = Vector3.zero;
		Vector3 truePivot = obj.transform.InverseTransformPoint(objBounds.center);
		obj.transform.localPosition -= truePivot;

		// Convert percentage margin to metres
		float margin = longestSide * (imageMargin / 100);

		// Set the orthographic size to fit the longest side of the bounding box plus the margin
		float viewSize = longestSide / 2 + margin;

		// Ensure view size is never 0 or negative
		if (viewSize < 0.0001f)
		{
			viewSize = 0.0001f;
		}

		// Change size of both cameras to fit the object
		screenshotCam.orthographicSize = viewSize;
		Camera.main.orthographicSize = viewSize;
	}

	/// <summary>
	/// Captures and returns a screenshot of the current view of a camera.
	/// Uses a depth only camera to capture transparency, not the main scene camera.
	/// </summary>
	/// <returns>A 512px by 512px screenshot as a 2D texture.</returns>
	private Texture2D Screenshot()
	{
		// Define a 2D texture to return
		int pixWidth = imgWidth;
		int pixHeight = imgHeight;
		RenderTexture target = new RenderTexture(pixWidth, pixHeight, 32);
		Texture2D result = new Texture2D(pixWidth, pixHeight, TextureFormat.ARGB32, false);
		RenderTexture.active = target;

		// Capture an image as the defined render texture
		screenshotCam.targetTexture = target;
		screenshotCam.Render();
		result.ReadPixels(new Rect(0, 0, pixWidth, pixHeight), 0, 0);
		result.Apply();

		// Cleanup and return the image
		screenshotCam.targetTexture = null;
		RenderTexture.active = null;
		Destroy(target);
		return result;
	}

	/// <summary>
	/// Saves a 2D texture (image) to a .png file.
	/// Formats file names as "frame0001.png, frame0002.png, ..."
	/// </summary>
	/// <param name="screenshot">The image to save.</param>
	/// <param name="path">The path from project root in which to save the image.</param>
	/// <param name="number">The unformatted screenshot number to include in the file name.</param>
	private void SaveTextureToFile(Texture2D screenshot, string path, int number)
	{
		// Encode screenshot into a .png
		byte[] bytes = screenshot.EncodeToPNG();

		// Define an image path from the project root and format its name
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
		string fileNumber = string.Format("{0:0000}", number);
		string filePath = directoryInfo.Parent.ToString() + "\\" + path;

		// Create a new directory for this file if it no longer exists
		if (!Directory.Exists(filePath))
		{
			Directory.CreateDirectory(filePath);
		}

		// Write the encoded data into a .png at the defined file location
		File.WriteAllBytes(filePath + "/frame" + fileNumber + ".png", bytes);
	}

	/// <summary>
	/// Create a new folder within the main project directory. As a parent of the main application data folder.
	/// Does not override folders of the same name.
	/// </summary>
	/// <param name="path">Path of the folder as a string</param>
	private void CreateFolderInRoot(string path)
	{
		// Define a path from the project root
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
		string directoryPath = directoryInfo.Parent.ToString() + "\\" + path;

		// Attempt to create a new folder at the defined path
		try
		{
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}
		catch (IOException ex)
		{
			Debug.LogError(ex.Message);
		}
	}
}
