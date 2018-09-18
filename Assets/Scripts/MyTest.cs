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
        private string linkToGitHubOctocat = "https://assets-cdn.github.com/images/modules/logos_page/Octocat.png";

        private void Start ()
        {
            var surface = new Surface (new Dictionary<TextureChannel, TextureProvider> () { { TextureChannel.User1, new WebTextureProvider (linkToGitHubOctocat) } });
            var viz = new SurfaceVisualizer (UndoRedoRegister.Instance, surface, Vector2.one, SurfaceVisualizer.SurfaceRenderMode.Channel);
            viz.RenderedChannel = TextureChannel.Normal;
        }
    }
}
