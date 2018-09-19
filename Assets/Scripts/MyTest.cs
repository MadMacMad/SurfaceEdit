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

        private void Start ()
        {
            var surface = new Surface (new Dictionary<TextureChannel, TextureProvider> () { { TextureChannel.Albedo, new WebTextureProvider (linkToGitHubOctocat) } });

            var taff = new PaintTextureAffector (UndoRedoRegister.Instance);
            taff.Paint (new PaintEntry (new DefaultRoundBrush (.1f, .2f, 256, .1f).AsSnapshot (), new Vector2 (0, 0), new Vector2 (.5f, .5f)));
            taff.Affect (surface.Textures[TextureChannel.Albedo]);
            
            var surfViz = new SurfaceVisualizer (UndoRedoRegister.Instance, surface, Vector2.one, SurfaceVisualizer.SurfaceRenderMode.Channel);
        }
    }
}
