using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tilify.Brushes;
using Tilify.TextureAffectors;
using Tilify.TextureProviders;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Tilify
{
    public class MyTest : MonoBehaviour
    {
        private string linkToGitHubOctocat = "https://assets-cdn.github.com/images/modules/logos_page/Octocat.png";
        private string pathToLenna = "Textures/Standard/Lenna";

        [Range(0, 1)]
        public float size = .1f;
        [Range (0, 1)]
        public float intervals = .1f;
        [Range (0, 1)]
        public float hardness = .5f;

        private Surface surface;
        private PaintTextureAffector taff;

        private Brush brush;

        private void Start ()
        {
            surface = new Surface (new Dictionary<TextureChannel, TextureProvider> () { { TextureChannel.Albedo, new BlankChannelTextureProvider(new Vector2Int(4096, 4096), TextureChannel.Metallic) } });

            taff = new PaintTextureAffector (UndoRedoRegister.Instance);
            
            var surfViz = new SurfaceVisualizer (UndoRedoRegister.Instance, surface, Vector2.one, SurfaceVisualizer.SurfaceRenderMode.Channel);

            brush = new DefaultRoundBrush (size, intervals, 64, hardness);
        }

        private Vector2 last;

        private void Update ()
        {
            bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out RaycastHit hit);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                surface.ResetAll ();
                taff.Reset ();
                taff.Affect (surface.Textures[TextureChannel.Albedo]);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                brush = new DefaultRoundBrush (size, intervals, 64, hardness);
            }

            var point = new Vector2 (hit.point.x, hit.point.z);

            if ( Input.GetKeyDown (KeyCode.Mouse0) && !Input.GetKey (KeyCode.LeftAlt) )
            {
                last = point;
            }
            if ( isHit && Input.GetKey (KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftAlt) )
            {
                if ( point == last )
                    return;
                taff.Paint (new PaintEntry (brush.AsSnapshot (), last, point));
                last = point;
                surface.ResetAll ();
                taff.Affect (surface.Textures[TextureChannel.Albedo]);
            }
            if ( Input.GetKeyDown (KeyCode.Alpha1) )
            {
                //UndoRedoRegister.Instance.Undo ();
            }
            if ( Input.GetKeyDown (KeyCode.Alpha2) )
            {
                //UndoRedoRegister.Instance.Redo ();
            }
        }
    }
}
