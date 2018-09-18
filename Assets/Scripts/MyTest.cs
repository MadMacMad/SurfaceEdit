using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tilify.AffectorRenderer;
using Tilify.TextureProviders;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Tilify
{
    public class MyTest : MonoBehaviour
    {
        //private PaintableTexture pt;

        //public Texture2D baseTexture;

        //private Brush brush;

        //private GameObject go;
        //private RenderTexture rt;

        //private Vector2 m_textureRealSize = new Vector2(3, 3);

        //private int m_RenderIgnoreLayerID;

        //private MaterialVisualizer matVis;
        
        private string linkToGitHubOctocat = "https://assets-cdn.github.com/images/modules/logos_page/Octocat.png";

        private void Start ()
        {
            var surface = new Surface (new Dictionary<TextureChannel, TextureProvider> () { { TextureChannel.Albedo, new WebTextureProvider (linkToGitHubOctocat) } });
            var viz = new SurfaceVisualizer (UndoRedoRegister.Instance, surface, Vector2.one, SurfaceVisualizer.SurfaceRenderMode.Channel);

            //m_RenderIgnoreLayerID = LayerMask.NameToLayer ("RenderIgnore");
            //rt = baseTexture.ConvertToRenderTexture ();
            //pt = new PaintableTexture (m_textureRealSize, rt, CommandRegister.Instance, m_RenderIgnoreLayerID);
            //brush = new DefaultRoundBrush (.05f, 256, .5f);

            //var dict = new Dictionary<TextureChannel, TextureProvider>
            //{
            //    { TextureChannel.Albedo, new ResourcesTextureProvider ("Textures/ogio2_4K_Albedo") },
            //    { TextureChannel.Normal, new ResourcesTextureProvider ("Textures/ogio2_4K_Normal") },
            //    { TextureChannel.Roughness, new ResourcesTextureProvider ("Textures/ogio2_4K_Roughness") },
            //    { TextureChannel.Height, new ResourcesTextureProvider ("Textures/ogio2_4K_Displacement") }
            //};
            //var surface = new Surface (dict);

            //var layer = new Layer (UndoRedoRegister.Instance);

            //matVis = new MaterialVisualizer (UndoRedoRegister.Instance, surface, new Vector2(2, 2), .1f, 10, true);
        }
        
        private void Update ()
        {
            //if (Input.GetKey(KeyCode.Alpha4))
            //{
            //    matVis.TesselationMultiplier += 1;
            //}

            //if ( Input.GetKey (KeyCode.Alpha5) )
            //{
            //    matVis.TesselationMultiplier -= 1;
            //}

            //if ( Input.GetKey (KeyCode.Alpha2) )
            //{
            //    UndoRedoRegister.Instance.Undo ();
            //}
            //if ( Input.GetKey (KeyCode.Alpha3) )
            //{
            //    UndoRedoRegister.Instance.Redo ();
            //}

            //if ( !pt.ResultTexture.IsCreated() )
            //    Debug.Log ("NULL");
            //if (Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftAlt))
            //{
            //    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            //    if ( Physics.Raycast (ray, out RaycastHit hit) )
            //    {
            //        pt.Paint (brush, new Vector2(1 - hit.point.x, 1 - hit.point.z));
            //        GameObject.Find("plane_1x1 (1)").GetComponent<MeshRenderer> ().material.mainTexture = pt.ResultTexture;
            //    }
            //}
            //if (Input.GetKey (KeyCode.Alpha2))
            //{
            //    CommandRegister.Instance.Undo ();
            //}
            //if (Input.GetKey (KeyCode.Alpha3))
            //{
            //    CommandRegister.Instance.Redo ();
            //}
        }
    }
}
