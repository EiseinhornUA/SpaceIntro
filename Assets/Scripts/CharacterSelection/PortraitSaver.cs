using UnityEngine;
using System.IO;

public class PortraitSaver : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;
    private const string fileName = "CharacterPortrait.png";

    public void SaveImage()
    {
        Texture2D texture = CaptureRenderTexture(renderTexture);
        byte[] bytes = EncodeToPNG(texture);
        WriteToFile(bytes, fileName);
        PlayerPrefs.SetString("PortraitPath", Path.Combine(Application.persistentDataPath, fileName));
        Destroy(texture);
    }

    private Texture2D CaptureRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = currentRenderTexture;
        return texture;
    }

    private byte[] EncodeToPNG(Texture2D texture)
    {
        return texture.EncodeToPNG();
    }

    private void WriteToFile(byte[] bytes, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, bytes);
    }
}
