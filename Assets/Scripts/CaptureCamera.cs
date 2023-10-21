using UnityEngine;
using System.IO;

public class CaptureCamera : MonoBehaviour
{
    [SerializeField]
    Camera cam;
	[SerializeField] string fileDirectory = @"C:\Users\owner\source\repos";
	[SerializeField] string fileName = "ico_GearItem__";
	RenderTexture renderTexture;
    [SerializeField]
    Vector2 targetSize = new Vector2(512,512);
    Texture2D lastPicture;
    
    void Start()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
        if (cam == null)
        {
            Debug.LogError("Camera not found");
            enabled = false;
        }
        else
        {
            renderTexture = new RenderTexture((int)targetSize.x, (int)targetSize.y, 1024);
            cam.targetTexture = renderTexture;
        }
    }

    /* This method is used to make the icon object for the inventory;
     * It takes a screenshot of the camera render in a 512x512 size. */
    void OnGUI()
	{
		cam.Render();
		GUI.Box(new Rect(0, 0, targetSize.x, targetSize.y), cam.targetTexture);
		fileName = GUILayout.TextField(fileName, GUILayout.Width(500));
		if (GUILayout.Button("Capture"))
        {
            RenderTexture.active = renderTexture;


            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();

            byte[] bytes;
            bytes = texture.EncodeToPNG();
			string filePath = Path.Combine(fileDirectory, fileName);
            System.IO.File.WriteAllBytes(filePath + ".png", bytes);
            Debug.Log(filePath + ".png");
            lastPicture = texture;
		}
	}
}
