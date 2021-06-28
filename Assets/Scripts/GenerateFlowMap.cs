using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GenerateFlowMap : MonoBehaviour
{

    private Camera _cam;
    private static readonly int CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");

    // Start is called before the first frame update
    void Start()
    {
        var _directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        
        

        
     
        _cam = GetComponent<Camera>();
        
        _cam.depthTextureMode = DepthTextureMode.Depth;
        Texture t = Shader.GetGlobalTexture(CameraDepthTexture);

        Debug.Log(t == null ? "null" : "not null");
        
        
        
        
        //SaveTextureAsPNG(t, _directoryInfo + "image.jpg");
    }
    
    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes =_texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length/1024  + "Kb was saved as: " + _fullPath);
    }
}
