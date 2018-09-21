using SurfaceEdit.Brushes;
using SurfaceEdit.TextureAffectors;
using TMPro;
using UnityEngine;

namespace SurfaceEdit
{
    public class DemoOne : MonoBehaviour
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
        private FillTextureAffector fillAffector;

        private Brush brush;

        private TextMeshProUGUI ui;

        private void Start ()
        {
            var channels = new TextureChannelCollection ();
            channels.AddChannel (TextureChannel.Albedo);

            surface = new Surface (new TextureResolution(TextureResolutionEnum.x2048), channels);

            taff = new PaintTextureAffector (UndoRedoRegister.Instance);
            fillAffector = new FillTextureAffector (UndoRedoRegister.Instance, new Color (0, 0, 0, 1));

            fillAffector.Affect (surface.Textures[TextureChannel.Albedo]);

            var surfViz = new SurfaceVisualizer (UndoRedoRegister.Instance, surface, Vector2.one, SurfaceVisualizer.SurfaceRenderMode.Channel);

            brush = new DefaultRoundBrush (new TextureResolution(TextureResolutionEnum.x64), size, intervals, hardness);

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
            PaintingManager.Instance.OnPaintTemporary = e =>
            {
                surface.ResetAll ();
                fillAffector.Affect (surface.Textures[TextureChannel.Albedo]);
                taff.PaintTemporary (e);
                taff.Affect (surface.Textures[TextureChannel.Albedo]);
            };
            PaintingManager.Instance.OnPaintFinal = e =>
            {
                surface.ResetAll ();
                fillAffector.Affect (surface.Textures[TextureChannel.Albedo]);
                taff.Paint (e);
                taff.Affect (surface.Textures[TextureChannel.Albedo]);
            };


            ui = GameObject.Find ("UI").GetComponent<TextMeshProUGUI>();
        }
        private void Update ()
        {
            if ( Input.GetKeyDown (KeyCode.Alpha1) )
            {
                surface.ResetAll ();
                fillAffector.Affect (surface.Textures[TextureChannel.Albedo]);
                UndoRedoRegister.Instance.Undo ();
                taff.Affect (surface.Textures[TextureChannel.Albedo]);
            }
            if ( Input.GetKeyDown (KeyCode.Alpha2) )
            {
                surface.ResetAll ();
                fillAffector.Affect (surface.Textures[TextureChannel.Albedo]);
                UndoRedoRegister.Instance.Redo ();
                taff.Affect (surface.Textures[TextureChannel.Albedo]);
            }

            if ( Input.GetKey (KeyCode.RightBracket) )
                size += .003f;
            if ( Input.GetKey (KeyCode.LeftBracket) )
                size -= .003f;

            if ( Input.GetKey (KeyCode.UpArrow) )
                intervals += .003f;
            if ( Input.GetKey (KeyCode.DownArrow) )
                intervals -= .003f;

            if ( Input.GetKey (KeyCode.RightArrow) )
                hardness += .003f;
            if ( Input.GetKey (KeyCode.LeftArrow) )
                hardness -= .003f;

            size = Mathf.Clamp (size, .001f, 1);
            intervals = Mathf.Clamp (intervals, .01f, 1);
            hardness = Mathf.Clamp01 (hardness);

            if ( Input.GetKeyDown (KeyCode.Space) )
            {
                brush.PercentageSize = new Vector2 (size, size);
                brush.PercentageIntervals = intervals;
                ( brush as DefaultRoundBrush ).Hardness = hardness;
            }

            ui.text = $"Size = {size} [ and ] to change\nIntervals = {intervals} Up arrow and down arrow to change \nHardness = {hardness} Right arrow and left arrow to change\n\nSpace to apply brush changes\nAlpha 1 key for undo, Alpha 2 key for redo\n"
                + "\nDemo uses compute shaders and geometry shaders\nBrushes are generated on GPU with compute shader\n2K texture is used in painting \nOn 4k texture with many brush instances demo is noticeably lagging.\nI know how to improve the perfomance, but that will be done in the future releases\n\nSources can be found here: https://github.com/grenqa/SurfaceEdit \nVersion: Alpha 1";
        }
    }
}
