using System.Collections.Generic;
using SurfaceEdit.Brushes;
using SurfaceEdit.SurfaceAffectors;
using SurfaceEdit.TextureProviders;
using TMPro;
using UnityEngine;

namespace SurfaceEdit.Demos
{
    public class DemoThree : MonoBehaviour
    {
        private LayerStack layerStack;
        private Layer layer1;
        private Layer layer2;
        private SurfaceVisualizer surfaceVisualizer;
        private Brush brush;

        private TextMeshProUGUI ui1;
        private TextMeshProUGUI ui2;

        private TextureResolution textureResolution;
        private ImmutableTextureResolution chunkResolution;

        private RenderTexture first;
        private RenderTexture second;
        private RenderTexture height;

        private GameObject go;
        
        private void Start ()
        {
            var undoRedoRegister = new UndoRedoRegister ();

            var inputManager = new InputManager ();

            var chain = new InputTriggerConflictChain (
                new InputTriggerKeyCombination (undoRedoRegister.Undo)
                    .WhenKeyPress (KeyCode.LeftControl)
                    .WhenAnyKeyDown (KeyCode.Z, KeyCode.F),

                new InputTriggerKeyCombination (undoRedoRegister.Redo)
                    .WhenKeyPress (KeyCode.LeftControl)
                    .WhenKeyPress (KeyCode.LeftShift)
                    .WhenAnyKeyDown (KeyCode.Z, KeyCode.F));

            inputManager.AddTrigger (chain);

            var channels = new Channels ();
            channels.AddChannel (Channel.Albedo);
            channels.AddChannel (Channel.Normal);
            channels.AddChannel (Channel.Height);
            //channels.AddChannel (Channel.Roughness);
            channels.AddChannel (Channel.Mask);

            this.channels = new List<Channel> ();
            this.channels.AddRange (channels.List);
            this.channels.Remove (Channel.Mask);

            textureResolution = new TextureResolution (TextureResolutionEnum.x2048);
            chunkResolution = new ImmutableTextureResolution (TextureResolutionEnum.x256);

            var context = new ProgramContext (undoRedoRegister, channels, textureResolution, chunkResolution);

            layerStack = new LayerStack (context);
            
            layer1 = layerStack.CreateLayer ();

            var albedoFill1 = new TextureFillSurfaceAffector (context, Channel.Albedo, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Mud/Albedo"));
            layer1.AddAffector (albedoFill1);

            var normalFill1 = new TextureFillSurfaceAffector (context, Channel.Normal, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Mud/Normal"));
            layer1.AddAffector (normalFill1);

            var heightFill1 = new TextureFillSurfaceAffector (context, Channel.Height, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Mud/Height"));
            layer1.AddAffector (heightFill1);

            //var roughnessFill1 = new TextureFillLayerAffector (context, Channel.Roughness, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Bricks/Roughness"));
            //layer1.AddAffector (roughnessFill1);

            layer2 = layerStack.CreateLayer ();
            layer2.BlendType = LayerBlendType.AlphaBlend;

            var albedoFill2 = new TextureFillSurfaceAffector (context, Channel.Albedo, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Bricks/Albedo"));
            layer2.AddAffector (albedoFill2);

            var normalFill2 = new TextureFillSurfaceAffector (context, Channel.Normal, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Bricks/Normal"));
            layer2.AddAffector (normalFill2);

            var heightFill2 = new TextureFillSurfaceAffector (context, Channel.Height, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Bricks/Height"));
            layer2.AddAffector (heightFill2);

            //var roughnessFill2 = new TextureFillLayerAffector (context, Channel.Roughness, new ResourcesTextureProvider (textureResolution, "Textures/Standard/Plaster/Roughness"));
            //layer2.AddAffector (roughnessFill2);

            var paintTextureAffector = new PaintSurfaceAffector (context, Channel.Mask);

            layer2.AddAffector (paintTextureAffector);

            surfaceVisualizer = new SurfaceVisualizer (undoRedoRegister, layerStack.ResultSurface);
            surfaceVisualizer.DisplacementIntensity = .2f;
            surfaceVisualizer.TesselationMultiplier = 4;
            surfaceVisualizer.InvertNormal = true;
            undoRedoRegister.Reset ();

            brush = new DefaultRoundBrush (new TextureResolution (TextureResolutionEnum.x128), .15f, .25f, 0);

            brush.TintColor = isBrushBlack ? Color.black * new Color (1, 1, 1, pressure) : Color.white * new Color (1, 1, 1, pressure);

            var paintingManager = new PaintingManager (brush);
            
            paintingManager.PaintTrigger += () =>
            {
                if ( Input.GetKey (KeyCode.Mouse0) && !Input.GetKey (KeyCode.LeftAlt) )
                {
                    bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out RaycastHit hit);
                    var point = new Vector2 (hit.point.x, hit.point.z);
                    return new PaintingManager.PaintTriggerEntry (true, isHit, point);
                }
                return new PaintingManager.PaintTriggerEntry (false, false, Vector2.zero);
            };
            paintingManager.OnPaintTemporary = e =>
            {
                paintTextureAffector.PaintTemporary (e);
            };
            paintingManager.OnPaintFinal = e =>
            {
                paintTextureAffector.PaintFinal (e);
            };
            ui1 = GameObject.Find ("UI1")?.GetComponent<TextMeshProUGUI>();
            ui2 = GameObject.Find ("UI2")?.GetComponent<TextMeshProUGUI>();
            sun = GameObject.Find ("Sun");
            skyboxMaterial = Resources.Load ("Materials/Skybox") as Material;
            rotation = skyboxMaterial.GetVector ("_Euler").x;
            
            //layerStack.ResultSurface.Changed += (s, e) => surfaceVisualizer.Update ();

            //GameObject.Find ("TextureResolution").GetComponentInChildren<TMP_Dropdown> ().onValueChanged.AddListener (i =>
            //{
            //    textureResolution.SetResolution ((TextureResolutionEnum)Mathf.Pow(2, i + 9));
            //});
        }

        private bool isBrushBlack = false;
        private float pressure = .5f;
        private Vector2 lastMousePosition;
        private Material skyboxMaterial;

        private float rotation;
        private bool lastFrameRotation;

        private GameObject sun;

        private List<Channel> channels;
            
        private void Update ()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                isBrushBlack = !isBrushBlack;
                brush.TintColor = isBrushBlack ? Color.black * new Color (1, 1, 1, pressure) : Color.white * new Color (1, 1, 1, pressure);
            }

            if ( Input.GetKeyDown (KeyCode.LeftBracket) )
                brush.PercentageSize = brush.PercentageSize - new Vector2 (.05f, .05f);
            else if ( Input.GetKeyDown (KeyCode.RightBracket) )
                brush.PercentageSize = brush.PercentageSize + new Vector2 (.05f, .05f);

            if ( Input.GetKeyDown (KeyCode.UpArrow) )
                ( brush as DefaultRoundBrush ).Hardness += .1f;
            else if ( Input.GetKeyDown (KeyCode.DownArrow) )
                ( brush as DefaultRoundBrush ).Hardness -= .1f;

            if ( Input.GetKeyDown (KeyCode.LeftArrow) )
            {
                pressure -= .1f;
                pressure = Mathf.Clamp01 (pressure);
                brush.TintColor = isBrushBlack ? Color.black * new Color (1, 1, 1, pressure) : Color.white * new Color (1, 1, 1, pressure);
            }
            else if ( Input.GetKeyDown (KeyCode.RightArrow) )
            {
                pressure += .1f;
                pressure = Mathf.Clamp01 (pressure);
                brush.TintColor = isBrushBlack ? Color.black * new Color(1, 1, 1, pressure) : Color.white * new Color (1, 1, 1, pressure);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (surfaceVisualizer.RenderMode != SurfaceRenderMode.Channel)
                {
                    surfaceVisualizer.RenderMode = SurfaceRenderMode.Channel;
                    surfaceVisualizer.ChannelToRender = Channel.Albedo;
                }
                else
                    surfaceVisualizer.CycleChannelToRenderNext ();
            }
            else if (Input.GetKeyDown(KeyCode.V))
                surfaceVisualizer.RenderMode = SurfaceRenderMode.Surface;

            //ui1.text =
            //    $"Working resolution: {textureResolution.AsInt + " x " + textureResolution.AsInt}\n" +
            //    $"Brush color: {( isBrushBlack ? "Black" : "White" )}\n" +
            //    $"Brush size: {brush.PercentageSize.x}\n" +
            //    $"Brush hardness: {( brush as DefaultRoundBrush ).Hardness}\n" +
            //    $"Brush pressure: {pressure}";

            //ui2.text =
            //    $"Press E and R to change\n" +
            //    $"Press X to change\n" +
            //    $"Press [ and ] to change\n" +
            //    $"Press up arrow and down arrow to change\n" +
            //    $"Press left arrow and right arrow to change\n" +
            //    $"Press Ctrl + Z for undo\n" +
            //    $"Press Ctrl + Shift + Z for undo\n" +
            //    $"Press Shift + RMB to rotate skybox\n" +
            //    $"Press C to change channel to view\n" +
            //    $"Press V to view surface";
        }
    }
}
