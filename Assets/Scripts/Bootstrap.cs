using System;
using System.Collections;
using System.Collections.Generic;
using SurfaceEdit.Brushes;
using SurfaceEdit.SurfaceAffectors;
using SurfaceEdit.TextureProviders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceEdit
{
    public class Bootstrap : MonoBehaviour
    {
        [Header ("Main Settings")]
        public TMP_Dropdown textureResolutionDropdown;
        //public TMP_Dropdown surfaceWorldSizeDropdown;
        public Slider skyboxRotationSlider;
        public Slider skyboxBlurSlider;

        [Header("UI Brush Bettings")]
        public Slider brushSizeSlider;
        public Slider brushPressureSlider;
        public Slider brushHardnessSlider;
        public Slider brushColorSlider;

        private ProgramContext context;

        private UndoRedoManager undoRedoManager;
        private PaintingManager paintingManager;

        private LayerStack layerStack;

        private SurfaceVisualizer surfaceVisualizer;
        
        private InputManager inputManager;

        private void Start ()
        {
            UnityMemorizer<Vector3>.Instance.Memorize ("mousePosition", () => Input.mousePosition);

            undoRedoManager = new UndoRedoManager ();
            paintingManager = new PaintingManager ();

            SetupProgramContext ();

            layerStack = new LayerStack (context);
            SetupSurfaceVisualizer ();

            SetupInputManager ();

            TemporarySetupLayersManually ();

            SetupSkyboxManager ();
            SetupUI ();
        }

        private void SetupProgramContext ()
        {
            context = new ProgramContext (undoRedoManager,
                new Channels (new List<Channel> () { Channel.Albedo, Channel.Normal, Channel.Height, Channel.Mask }),
                new TextureResolution (TextureResolutionEnum.x2048),
                new ImmutableTextureResolution (TextureResolutionEnum.x256));
        }

        private void SetupSurfaceVisualizer()
        {
            surfaceVisualizer = new SurfaceVisualizer (undoRedoManager, layerStack.ResultSurface);
            surfaceVisualizer.DisplacementIntensity = .2f;
            surfaceVisualizer.TesselationMultiplier = 4;
            surfaceVisualizer.InvertNormal = true;
            undoRedoManager.Reset ();
        }

        private void SetupInputManager ()
        {
            inputManager = new InputManager ();

            var undoTrigger = new KeyCombination ()
                .Ctrl ()
                .Key (KeyCode.Z)
                .AddTriggeredCallback(undoRedoManager.Undo);

            inputManager.AddTrigger (undoTrigger);

            var redoTrigger = new KeyCombination ()
                .Ctrl ()
                .Shift ()
                .Key (KeyCode.Z)
                .AddTriggeredCallback(undoRedoManager.Redo);

            inputManager.AddTrigger (redoTrigger);

            var skyboxRotationTrigger = new KeyCombination ()
                .Shift ()
                .Key (KeyCode.Mouse0, KeyTriggerType.Press)
                .AddTriggeredCallback(() =>
                {
                    var lastMousePosition = UnityMemorizer<Vector3>.Instance.GetValue ("mousePosition");
                    var mousePosition = Input.mousePosition;
                    var rotation = mousePosition.x - lastMousePosition.x;
                    SkyboxManager.Instance.RotateSkyBoxIncremental (rotation);
                });

            inputManager.AddTrigger (skyboxRotationTrigger);

            var paintTrigger = new KeyCombination ()
                .Key (KeyCode.Mouse0, KeyTriggerType.Press)
                .AddTriggeredCallback(() =>
                {
                    bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out RaycastHit hit);

                    if ( isHit )
                    {
                        var point = new Vector2 (hit.point.x, hit.point.z);
                        paintingManager.PaintTriggered (point);
                    }
                })
                .AddNotTriggeredCallback(paintingManager.PaintNotTriggered);

            inputManager.AddTrigger (paintTrigger);
        }

        private void TemporarySetupLayersManually()
        {
            var layer1 = layerStack.CreateLayer ();

            var albedoFill1 = new TextureFillSurfaceAffector (context, Channel.Albedo, new ResourcesTextureProvider (context.TextureResolution, "Textures/Standard/Mud/Albedo"));
            layer1.AddAffector (albedoFill1);

            var normalFill1 = new TextureFillSurfaceAffector (context, Channel.Normal, new ResourcesTextureProvider (context.TextureResolution, "Textures/Standard/Mud/Normal"));
            layer1.AddAffector (normalFill1);

            var heightFill1 = new TextureFillSurfaceAffector (context, Channel.Height, new ResourcesTextureProvider (context.TextureResolution, "Textures/Standard/Mud/Height"));
            layer1.AddAffector (heightFill1);

            var layer2 = layerStack.CreateLayer ();
            layer2.BlendType = LayerBlendType.AlphaBlend;

            var albedoFill2 = new TextureFillSurfaceAffector (context, Channel.Albedo, new ResourcesTextureProvider (context.TextureResolution, "Textures/Standard/Bricks/Albedo"));
            layer2.AddAffector (albedoFill2);

            var normalFill2 = new TextureFillSurfaceAffector (context, Channel.Normal, new ResourcesTextureProvider (context.TextureResolution, "Textures/Standard/Bricks/Normal"));
            layer2.AddAffector (normalFill2);

            var heightFill2 = new TextureFillSurfaceAffector (context, Channel.Height, new ResourcesTextureProvider (context.TextureResolution, "Textures/Standard/Bricks/Height"));
            layer2.AddAffector (heightFill2);

            var paintTextureAffector = new PaintSurfaceAffector (context, Channel.Mask);

            layer2.AddAffector (paintTextureAffector);

            paintingManager.OnPaintTemporary = e =>
            {
                paintTextureAffector.PaintTemporary (e);
            };
            paintingManager.OnPaintFinal = e =>
            {
                paintTextureAffector.PaintFinal (e);
            };
        }

        private void SetupSkyboxManager ()
        {
            SkyboxManager.Instance.SetSkyboxCubeMap (Resources.Load ("Textures/Cubemaps/DefaultCubeMap") as Cubemap);
        }

        private void SetupUI()
        {
            SetupBrushSettings ();
            SetupMainSettings ();

            void SetupBrushSettings()
            {
                Assert.SoftNotNull (brushSizeSlider, nameof (brushSizeSlider));
                Assert.SoftNotNull (brushPressureSlider, nameof (brushPressureSlider));
                Assert.SoftNotNull (brushHardnessSlider, nameof (brushHardnessSlider));
                Assert.SoftNotNull (brushColorSlider, nameof (brushColorSlider));

                if ( brushSizeSlider != null )
                {
                    brushSizeSlider.value = paintingManager.Brush.PercentageSize.x;
                    brushSizeSlider.onValueChanged.AddListener (v => paintingManager.Brush.PercentageSize = new Vector2 (v, v));
                }

                if ( brushPressureSlider != null )
                {
                    brushPressureSlider.value = paintingManager.Brush.TintColor.a;
                    brushPressureSlider.onValueChanged.AddListener (v =>
                    {
                        var color = paintingManager.Brush.TintColor;
                        color.a = v;
                        paintingManager.Brush.TintColor = color;
                    });
                }

                if ( brushHardnessSlider != null )
                {
                    brushHardnessSlider.value = ( paintingManager.Brush as DefaultRoundBrush ).Hardness;
                    brushHardnessSlider.onValueChanged.AddListener (v => ( paintingManager.Brush as DefaultRoundBrush ).Hardness = v);
                }

                if ( brushColorSlider != null )
                {
                    brushColorSlider.value = paintingManager.Brush.TintColor.r;
                    brushColorSlider.onValueChanged.AddListener (v =>
                    {
                        var alpha = paintingManager.Brush.TintColor.a;
                        var color = new Color (v, v, v, alpha);
                        paintingManager.Brush.TintColor = color;
                    });
                }
            }
            void SetupMainSettings()
            {
                Assert.SoftNotNull (textureResolutionDropdown, nameof (textureResolutionDropdown));
                //Assert.SoftNotNull (surfaceWorldSizeDropdown, nameof (surfaceWorldSizeDropdown));
                Assert.SoftNotNull (skyboxRotationSlider, nameof (skyboxRotationSlider));
                Assert.SoftNotNull (skyboxBlurSlider, nameof (skyboxBlurSlider));

                if (textureResolutionDropdown != null)
                {
                    textureResolutionDropdown.value = (int)Mathf.Log (context.TextureResolution.AsInt, 2) - 9;
                    textureResolutionDropdown.onValueChanged.AddListener (v =>
                     {
                         var resolution = (TextureResolutionEnum)(Mathf.Pow(2, v + 9));
                         context.TextureResolution.SetResolution (resolution);
                     });
                }

                if (skyboxRotationSlider != null)
                {
                    skyboxRotationSlider.value = Mathf.Clamp01(SkyboxManager.Instance.Rotation / 360f);

                    SkyboxManager.Instance.OnRotate += ()
                        => skyboxRotationSlider.value = Mathf.Clamp01 (SkyboxManager.Instance.Rotation / 360f);

                    skyboxRotationSlider.onValueChanged.AddListener (v => SkyboxManager.Instance.RotateSkybox (v * 360));
                }

                if (skyboxBlurSlider != null)
                {
                    skyboxBlurSlider.value = SkyboxManager.Instance.Blurriness;
                    skyboxBlurSlider.onValueChanged.AddListener (v => SkyboxManager.Instance.SetSkyboxBlurAmount (v));
                }
            }
        }
    }
}