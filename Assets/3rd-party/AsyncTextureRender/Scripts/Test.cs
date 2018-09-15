using UnityEngine;
using System.Collections;
using System;

public class Test : MonoBehaviour
{

    public Texture DebugTexture;
    private byte[] Pixels;

    private ComputeBuffer buffer;
    private float[] floats = new float[4];

    // Use this for initialization
    void Start()
    {
        buffer = new ComputeBuffer(4, sizeof(float));
        buffer.SetData(new float[] { 1, 2, 3, 4 });

        AsyncTextureReader.InitDebugLogs();

        Pixels = new byte[DebugTexture.width * DebugTexture.height * 4];

        Debug.LogFormat("Frame: {0}; Request Status: {1}", Time.frameCount, AsyncTextureReader.RequestTextureData(DebugTexture));
        Debug.LogFormat("Frame: {0}; Retrieve Status: {1}", Time.frameCount, AsyncTextureReader.RetrieveTextureData(DebugTexture, Pixels));

#if UNITY_5_5_OR_NEWER
        AsyncTextureReader.RequestBufferData(buffer);
#endif
    }

    private void GetPixels()
    {
        if ( Pixels == null)
            return;

        AsyncTextureReader.Status status = AsyncTextureReader.RetrieveTextureData(DebugTexture, Pixels);
        Debug.LogFormat("Frame: {0}; Retrieve Status: {1}", Time.frameCount, status);
        if (status == AsyncTextureReader.Status.Succeeded)
        {
            // print RGBA of first pixel
            Debug.LogFormat("Pixel RGBA: {0}; {1}; {2}; {3}", Pixels[0], Pixels[1], Pixels[2], Pixels[3]);
            Pixels = null;
            AsyncTextureReader.ReleaseTempResources(DebugTexture);
        }        
    }

    private void GetData()
    {
#if UNITY_5_5_OR_NEWER
        if ( floats == null)
            return;

        AsyncTextureReader.Status status = AsyncTextureReader.RetrieveBufferData(buffer, floats);
        //Debug.LogFormat("Frame: {0}; Retrieve Buffer Status: {1}", Time.frameCount, status);
        if (status == AsyncTextureReader.Status.Succeeded)
        {
            Debug.LogFormat("Buffer Data: {0}; {1}; {2}; {3}", floats[0], floats[1], floats[2], floats[3]);
            floats = null;
            AsyncTextureReader.ReleaseTempResources(buffer);
        }
#endif
    }

    // Update is called at the beginning of frame
    void Update()
    {
        GetPixels();
    }

    // OnPostRender is called at the end of frame
    public void OnPostRender()
    {
        GetPixels();
        GetData();
    }
}
