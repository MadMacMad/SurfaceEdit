using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurfaceEdit.Brushes;
using SurfaceEdit.TextureAffectors;
using SurfaceEdit.TextureProviders;
using TMPro;
using UnityEngine;

namespace SurfaceEdit.Demos
{
    public class DemoTwo : MonoBehaviour
    {
        private LayerStack layerStack;
        private Layer layer1;
        private Layer layer2;
        private SurfaceVisualizer surfaceVisualizer;
        private Brush brush;

        private TextMeshProUGUI ui1;
        private TextMeshProUGUI ui2;

        private TextureResolution resolution;


        private void Start ()
        {
            UndoRedoRegister.Instance.SetUndoTrigger (() => Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.Z));
            UndoRedoRegister.Instance.SetRedoTrigger (() => Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.Z));

            var channels = new TextureChannelCollection ();
            channels.AddChannel (TextureChannel.Albedo);
            channels.AddChannel (TextureChannel.Normal);
            channels.AddChannel (TextureChannel.Height);
            channels.AddChannel (TextureChannel.Roughness);
            channels.AddChannel (TextureChannel.Mask);

            this.channels = channels.List.ToList();
            this.channels.Remove (TextureChannel.Mask);

            resolution = new TextureResolution (TextureResolutionEnum.x2048);

            layerStack = new LayerStack (UndoRedoRegister.Instance, resolution, channels);
            
            layer1 = layerStack.CreateLayer ();

            var albedoFill1 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Mud/Albedo"));
            layer1.AddSurfaceAffector (albedoFill1, TextureChannel.Albedo);

            var normalFill1 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Mud/Normal"));
            layer1.AddSurfaceAffector (normalFill1, TextureChannel.Normal);

            var heightFill1 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Mud/Height"));
            layer1.AddSurfaceAffector (heightFill1, TextureChannel.Height);

            var roughnessFill1 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Mud/Roughness"));
            layer1.AddSurfaceAffector (roughnessFill1, TextureChannel.Roughness);

            layer2 = layerStack.CreateLayer ();
            layer2.BlendType = LayerBlendType.AlphaBlend;
            
            var albedoFill2 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Cobblestone/Albedo"));
            layer2.AddSurfaceAffector (albedoFill2, TextureChannel.Albedo);

            var normalFill2 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Cobblestone/Normal"));
            layer2.AddSurfaceAffector (normalFill2, TextureChannel.Normal);

            var heightFill2 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Cobblestone/Height"));
            layer2.AddSurfaceAffector (heightFill2, TextureChannel.Height);

            var roughnessFill2 = new TextureFillTextureAffector (UndoRedoRegister.Instance, new ResourcesTextureProvider (resolution, "Textures/Standard/Mud/Roughness"));
            layer2.AddSurfaceAffector (roughnessFill2, TextureChannel.Roughness);

            var paintTextureAffector = new PaintTextureAffector (UndoRedoRegister.Instance);

            layer2.AddSurfaceAffector (paintTextureAffector, TextureChannel.Mask);
            
            surfaceVisualizer = new SurfaceVisualizer (UndoRedoRegister.Instance, layerStack.ResultSurface, Vector2.one);
            surfaceVisualizer.DisplacementIntensity = .15f;
            surfaceVisualizer.TesselationMultiplier = 20;
            surfaceVisualizer.InvertNormal = true;

            brush = new DefaultRoundBrush (new TextureResolution (TextureResolutionEnum.x64), .2f, .25f, 0);
            
            PaintingManager.Instance.CurrentBrush = brush;
            PaintingManager.Instance.PaintTrigger += () =>
            {
                if ( Input.GetKey (KeyCode.Mouse0) && !Input.GetKey (KeyCode.LeftAlt) )
                {
                    bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out RaycastHit hit);
                    var point = new Vector2 (hit.point.x, hit.point.z);
                    return new PaintingManager.PaintTriggerEntry (true, isHit, point);
                }
                return new PaintingManager.PaintTriggerEntry (false, false, Vector2.zero);
            };
            PaintingManager.Instance.OnPaintTemporary = e =>
            {
                paintTextureAffector.PaintTemporary (e);
            };
            PaintingManager.Instance.OnPaintFinal = e =>
            {
                paintTextureAffector.Paint (e);
            };
            brush.TintColor = Color.white * new Color (1, 1, 1, .3f);

            ui1 = GameObject.Find ("UI1").GetComponent<TextMeshProUGUI>();
            ui2 = GameObject.Find ("UI2").GetComponent<TextMeshProUGUI>();
            sun = GameObject.Find ("Sun");
            skyboxMaterial = Resources.Load ("Materials/Skybox") as Material;
            rotation = skyboxMaterial.GetVector ("_Euler").x;
            
            layerStack.ResultSurface.NeedUpdate += (s, e) => surfaceVisualizer.Update ();
        }

        private bool isBrushBlack = false;
        private float pressure = .1f;
        private Vector2 lastMousePosition;
        private Material skyboxMaterial;

        private float rotation;
        private bool lastFrameRotation;

        private GameObject sun;

        private List<TextureChannel> channels;

        private void Update ()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                isBrushBlack = !isBrushBlack;
                brush.TintColor = isBrushBlack ? Color.black * new Color (1, 1, 1, pressure) : Color.white * new Color (1, 1, 1, pressure);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                var currentResolutionIndex = (int)resolution.AsEnum;
                var previousIndex = currentResolutionIndex / 2;
                if ( previousIndex >= (int)TextureResolutionEnum.x2 )
                {
                    var newResolution = (TextureResolutionEnum)previousIndex;
                    resolution.SetResolution (newResolution);
                }
            }
            else if ( Input.GetKeyDown (KeyCode.R) )
            {
                var currentIndex = (int)resolution.AsEnum;
                var nextIndex = currentIndex * 2;
                if ( nextIndex <= (int)TextureResolutionEnum.x4096 )
                {
                    var newResolution = (TextureResolutionEnum)nextIndex;
                    resolution.SetResolution (newResolution);
                }
            }

            if ( Input.GetKeyDown (KeyCode.LeftBracket) )
                brush.PercentageSize = brush.PercentageSize - new Vector2 (.1f, .1f);
            else if ( Input.GetKeyDown (KeyCode.RightBracket) )
                brush.PercentageSize = brush.PercentageSize + new Vector2 (.1f, .1f);

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

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse1))
            {
                var mousePosition = (Vector2)Input.mousePosition;

                if ( !lastFrameRotation )
                    lastMousePosition = mousePosition;

                else if (mousePosition != lastMousePosition )
                {
                    var difference = mousePosition - lastMousePosition;

                    sun.transform.Rotate (0, difference.x, 0, Space.World);
                    rotation += difference.x;

                    skyboxMaterial.SetVector ("_Euler", new Vector3 (0, rotation, 0));

                    var r = skyboxMaterial.GetVector ("_Euler");
                    var q = Quaternion.Euler (r.x, r.y, r.z);
                    var m = Matrix4x4.TRS (Vector3.zero, q, Vector3.one);
                    skyboxMaterial.SetVector ("_Rotation1", m.GetRow (0));
                    skyboxMaterial.SetVector ("_Rotation2", m.GetRow (1));
                    skyboxMaterial.SetVector ("_Rotation3", m.GetRow (2));

                    lastMousePosition = mousePosition;
                }
                lastFrameRotation = true;
            }
            else
            {
                lastFrameRotation = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (surfaceVisualizer.RenderMode != SurfaceVisualizer.SurfaceRenderMode.Channel)
                    surfaceVisualizer.RenderMode = SurfaceVisualizer.SurfaceRenderMode.Channel;
                var currentChannel = surfaceVisualizer.RenderedChannel;
                var currentIndex = channels.IndexOf(currentChannel);
                var newIndex = currentIndex + 1;
                if ( newIndex >= channels.Count )
                    newIndex = 0;
                var newChannel = channels[newIndex];
                surfaceVisualizer.RenderedChannel = newChannel;
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                if ( surfaceVisualizer.RenderMode != SurfaceVisualizer.SurfaceRenderMode.Surface )
                    surfaceVisualizer.RenderMode = SurfaceVisualizer.SurfaceRenderMode.Surface;
            }

            ui1.text =
                $"Working resolution: {resolution.Value + " x " + resolution.Value}\n" +
                $"Brush color: {( isBrushBlack ? "Black" : "White" )}\n" +
                $"Brush size: {brush.PercentageSize.x}\n" +
                $"Brush hardness: {( brush as DefaultRoundBrush ).Hardness}\n" +
                $"Brush pressure: {pressure}";

            ui2.text =
                $"Press E and R to change\n" +
                $"Press X to change\n" +
                $"Press [ and ] to change\n" +
                $"Press up arrow and down arrow to change\n" +
                $"Press left arrow and right arrow to change\n" +
                $"Press Ctrl + Z for undo\n" +
                $"Press Ctrl + Shift + Z for undo\n" +
                $"Press Shift + RMB to rotate skybox\n" +
                $"Press C to change channel to view\n" +
                $"Press V to view surface";
        }
    }
}
