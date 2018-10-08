using System.Collections.Generic;
using SurfaceEdit.Brushes;
using SurfaceEdit.Affectors;
using SurfaceEdit.TextureProviders;
using SurfaceEdit.Presenters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SurfaceEdit
{
    public class Bootstrap : MonoBehaviour
    {
        public LayerStackViewData layerStackViewData;
        public ContextMenuViewData contextMenuViewData;
        public ResourceManagerViewData resourcesViewData;

        [Header ("Main Settings")]
        public TMP_Dropdown textureResolutionDropdown;
        //public TMP_Dropdown surfaceWorldSizeDropdown;
        public Slider skyboxRotationSlider;
        public Slider skyboxBlurSlider;

        [Header ("UI Brush Settings")]
        public Slider brushSizeSlider;
        public Slider brushPressureSlider;
        public Slider brushHardnessSlider;
        public Slider brushColorSlider;

        private ApplicationContext context;

        private UndoRedoManager undoRedoManager;
        private PaintingManager paintingManager;
        private ResourceManager resourceManager;

        private LayerStack stack;
        private Layer activeLayer;
        
        private InputManager inputManager;


        private SurfaceVisualizer surfaceVisualizer;
        private LayerStackPresenter layerStackPresenter;
        private ResourceManagerPresenter resourcesPresenter;
        
        private void Start ()
        {
            UnityMemorizer<Vector3>.Instance.Memorize ("mousePosition", () => Input.mousePosition);

            undoRedoManager = new UndoRedoManager ();
            paintingManager = new PaintingManager ();

            SetupProgramContext ();

            resourceManager = new ResourceManager (context);

            stack = new LayerStack (context);
            SetupSurfaceVisualizer ();

            SetupInputManager ();

            SetupUI ();

            TemporarySetupLayersManually ();

            SetupSkyboxUtility ();
        }

        private void SetupProgramContext ()
        {
            var appdataPath = Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData);

            context = new ApplicationContext (undoRedoManager,
                new Channels (new List<Channel> () { Channel.Albedo, Channel.Normal, Channel.Height, Channel.Mask }),
                new TextureResolution (TextureResolutionEnum.x4096),
                new ImmutableTextureResolution (TextureResolutionEnum.x512),
                Path.Combine(appdataPath, nameof(SurfaceEdit)));
        }

        private void SetupSurfaceVisualizer ()
        {
            surfaceVisualizer = new SurfaceVisualizer (undoRedoManager, stack.ResultSurface);
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
                .AddTriggeredCallback (undoRedoManager.Undo);

            inputManager.AddTrigger (undoTrigger);

            var redoTrigger = new KeyCombination ()
                .Ctrl ()
                .Shift ()
                .Key (KeyCode.Z)
                .AddTriggeredCallback (undoRedoManager.Redo);

            inputManager.AddTrigger (redoTrigger);

            var skyboxRotationTrigger = new KeyCombination ()
                .Shift ()
                .Key (KeyCode.Mouse0, KeyTriggerType.Press)
                .AddTriggeredCallback (() =>
                 {
                     var lastMousePosition = UnityMemorizer<Vector3>.Instance.GetValue ("mousePosition");
                     var mousePosition = Input.mousePosition;
                     var rotation = mousePosition.x - lastMousePosition.x;
                     SkyboxUtility.RotateSkyBoxIncremental (rotation);
                 });

            inputManager.AddTrigger (skyboxRotationTrigger);

            var paintTrigger = new KeyCombination ()
                .Key (KeyCode.Mouse0, KeyTriggerType.Press)
                .AddTriggeredCallback (() =>
                 {
                     bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out RaycastHit hit);

                     if ( isHit )
                     {
                         var point = new Vector2 (hit.point.x, hit.point.z);
                         paintingManager.PaintTriggered (point);
                     }
                 })
                .AddNotTriggeredCallback (paintingManager.PaintNotTriggered);

            inputManager.AddTrigger (paintTrigger);

            var invertBrushColorTrigger = new KeyCombination ()
                .Key (KeyCode.X)
                .AddTriggeredCallback (() =>
                 {
                     var color = paintingManager.Brush.TintColor;
                     var newColor = new Color (1f - color.r, 1f - color.g, 1f - color.b, color.a);
                     paintingManager.Brush.TintColor = newColor;
                 });

            inputManager.AddTrigger (invertBrushColorTrigger);
        }

        private void TemporarySetupLayersManually ()
        {
            var layer1 = stack.CreateLayer ();

            var path = Application.dataPath + "/Resources/Textures/Standard/";

            var diff = new List<string> ()
            {
                "Mud/Albedo.jpg",
                "Mud/Normal.jpg",
                "Mud/Height.jpg",
                "Bricks/Albedo.jpg",
                "Bricks/Normal.jpg",
                "Bricks/Height.jpg"
            };
            foreach(var d in diff)
            {
                var result = resourceManager.TryImport (path + d, out Resource res);
                if (!result.IsSuccessfull)
                    Debug.LogWarning (result.ErrorMessage);
            }

            var albedoFill1 = new TextureFillAffector (context, Channel.Albedo, resourceManager.GetResources<Texture2DResource> ("Albedo")[0]);
            layer1.AddAffector (albedoFill1);

            var normalFill1 = new TextureFillAffector (context, Channel.Normal, resourceManager.GetResources<Texture2DResource> ("Normal")[0]);
            layer1.AddAffector (normalFill1);

            var heightFill1 = new TextureFillAffector (context, Channel.Height, resourceManager.GetResources<Texture2DResource> ("Height")[0]);
            layer1.AddAffector (heightFill1);

            var layer2 = stack.CreateLayer ();
            layer2.BlendType = LayerBlendType.AlphaBlend;

            var albedoFill2 = new TextureFillAffector (context, Channel.Albedo, resourceManager.GetResources<Texture2DResource> ("Albedo")[1]);
            layer2.AddAffector (albedoFill2);

            var normalFill2 = new TextureFillAffector (context, Channel.Normal, resourceManager.GetResources<Texture2DResource> ("Normal")[1]);
            layer2.AddAffector (normalFill2);

            var heightFill2 = new TextureFillAffector (context, Channel.Height, resourceManager.GetResources<Texture2DResource> ("Height")[1]);
            layer2.AddAffector (heightFill2);

            var paintTextureAffector = new PaintAffector (context, Channel.Mask, LayerMask.NameToLayer("RendererStation"));

            layer2.AddAffector (paintTextureAffector);

            paintingManager.OnPaintTemporary += e =>
            {
                paintTextureAffector.PaintTemporary (e);
            };
            paintingManager.OnPaintFinal += e =>
            {
                paintTextureAffector.PaintFinal (e);
            };
        }

        private void SetupSkyboxUtility ()
        {
            SkyboxUtility.SetSkyboxCubeMap (Resources.Load ("Textures/Cubemaps/DefaultCubeMap") as Cubemap);
        }

        private void SetupUI ()
        {
            SetupBrushSettings ();
            SetupMainSettings ();

            layerStackPresenter = new LayerStackPresenter (layerStackViewData, contextMenuViewData, stack);
            resourcesPresenter = new ResourceManagerPresenter (resourcesViewData, contextMenuViewData, resourceManager);

            void SetupBrushSettings ()
            {
                Assert.NotNull (brushSizeSlider, nameof (brushSizeSlider));
                Assert.NotNull (brushPressureSlider, nameof (brushPressureSlider));
                Assert.NotNull (brushHardnessSlider, nameof (brushHardnessSlider));
                Assert.NotNull (brushColorSlider, nameof (brushColorSlider));

                brushSizeSlider.value = paintingManager.Brush.PercentageSize.x;
                brushSizeSlider.onValueChanged.AddListener (v => paintingManager.Brush.PercentageSize = new Vector2 (v, v));

                brushPressureSlider.value = paintingManager.Brush.TintColor.a;
                brushPressureSlider.onValueChanged.AddListener (v =>
                {
                    var color = paintingManager.Brush.TintColor;
                    color.a = v;
                    paintingManager.Brush.TintColor = color;
                });
                
                brushHardnessSlider.value = ( paintingManager.Brush as DefaultRoundBrush ).Hardness;
                brushHardnessSlider.onValueChanged.AddListener (v => ( paintingManager.Brush as DefaultRoundBrush ).Hardness = v);
                
                brushColorSlider.value = paintingManager.Brush.TintColor.r;
                paintingManager.Brush.PropertyChanged += (s, e) =>
                {
                    if ( e.propertyName == "TintColor" )
                        brushColorSlider.value = paintingManager.Brush.TintColor.r;
                };
                brushColorSlider.onValueChanged.AddListener (v =>
                {
                    var alpha = paintingManager.Brush.TintColor.a;
                    var color = new Color (v, v, v, alpha);
                    paintingManager.Brush.TintColor = color;
                });

            }
            void SetupMainSettings ()
            {
                Assert.NotNull (textureResolutionDropdown, nameof (textureResolutionDropdown));
                //Assert.NotNull (surfaceWorldSizeDropdown, nameof (surfaceWorldSizeDropdown));
                Assert.NotNull (skyboxRotationSlider, nameof (skyboxRotationSlider));
                Assert.NotNull (skyboxBlurSlider, nameof (skyboxBlurSlider));

                textureResolutionDropdown.value = (int)Mathf.Log (context.TextureResolution.AsInt, 2) - 9;
                textureResolutionDropdown.onValueChanged.AddListener (v =>
                 {
                     var resolution = (TextureResolutionEnum)( Mathf.Pow (2, v + 9) );
                     context.TextureResolution.SetResolution (resolution);
                 });
                
                skyboxRotationSlider.value = Mathf.Clamp01 (SkyboxUtility.SkyboxRotation / 360f);
                SkyboxUtility.OnSkyboxRotate += ()
                    => skyboxRotationSlider.value = Mathf.Clamp01 (SkyboxUtility.SkyboxRotation / 360f);
                skyboxRotationSlider.onValueChanged.AddListener (v => SkyboxUtility.RotateSkybox (v * 360));
                
                skyboxBlurSlider.value = SkyboxUtility.SkyboxBlurriness;
                skyboxBlurSlider.onValueChanged.AddListener (v => SkyboxUtility.SetSkyboxBlurAmount (v));

            }
        }
    }
}