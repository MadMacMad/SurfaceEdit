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
            surface = new Surface (new Dictionary<TextureChannel, TextureProvider> () { { TextureChannel.Albedo, new BlankChannelTextureProvider(new Vector2Int(512, 512), TextureChannel.Metallic) } });

            taff = new PaintTextureAffector (UndoRedoRegister.Instance);
            
            var surfViz = new SurfaceVisualizer (UndoRedoRegister.Instance, surface, Vector2.one, SurfaceVisualizer.SurfaceRenderMode.Channel);

            brush = new DefaultRoundBrush (size, intervals, 64, hardness);

            PaintingManager.Instance.CurrentBrush = brush;
            PaintingManager.Instance.PaintTrigger += () =>
            {
                if ( Input.GetKey (KeyCode.Mouse0) && !Input.GetKey (KeyCode.LeftAlt) )
                {
                    bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out RaycastHit hit);
                    var point = new Vector2 (hit.point.x, hit.point.z);
                    return new PaintingManager.PaintTriggerEntry (isHit, point);
                }
                return new PaintingManager.PaintTriggerEntry (false, Vector2.zero);
            };
            PaintingManager.Instance.OnPaint = e =>
            {
                surface.ResetAll ();
                taff.Paint (e);
                taff.Affect (surface.Textures[TextureChannel.Albedo]);
            };
        }
        private void Update ()
        {
            brush.PercentageSize = new Vector2(size, size);
            brush.PercentageIntervals = intervals;
            (brush as DefaultRoundBrush ).Hardness = hardness;
        }
    }
}
