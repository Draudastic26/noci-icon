using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace drstc.noci
{
    public class NociEditor : EditorWindow
    {
        public const string NOCI_NAME = "NOCI";

        private const int SLIDER_ITERATION_MIN = 1;
        private const int SLIDER_ITERATION_MAX = 10;

        private readonly string urlStyleEditor = "Assets/Noci/Editor/NociEditor.uss";
        private readonly string urlStylePackage = "Packages/com.drstc.noci/Editor/NociEditor.uss";

        private readonly Vector2Int defaultDimension = new Vector2Int(10, 10);
        private readonly int defaultIteration = 2;
        private readonly bool defaultContour = true;
        private readonly Color defaultColorCell = Color.white;
        private readonly Color defaultCollorContour = Color.black;
        private readonly string defaultPath = "Assets/Noci/";
        private readonly int defaultScaleFactor = 20;

        private VisualElement elementIcon;
        private TextField fieldSavePath;
        private Texture2D generatedTex;
        private IntegerField intScaleFactor;

        private NociConfig defaultConfig;
        private Noci noci;

        [MenuItem("Tools/" + NOCI_NAME + " generator")]
        public static void StartWindow()
        {
            // Opens the window, otherwise focuses it if it’s already open.
            var window = GetWindow<NociEditor>();

            // Adds a title to the window.
            window.titleContent = new GUIContent(NOCI_NAME + " generator");

            // Sets a minimum size to the window.
            window.minSize = new Vector2(250, 50);
        }

        private void OnEnable()
        {
            // Set defaults
            defaultConfig = new NociConfig(defaultDimension, defaultIteration, defaultContour);
            defaultConfig.CellColor = defaultColorCell;
            defaultConfig.ContourColor = defaultCollorContour;

            noci = new Noci(defaultConfig);

            // Reference to the root of the window.
            var root = rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(File.Exists(urlStylePackage) ? urlStylePackage : urlStyleEditor));

            var header = new Label(NOCI_NAME);
            header.AddToClassList("heading");

            var btnReroll = new Button() { text = "Reroll" };
            btnReroll.clickable.clicked += () => Reroll();


            // Adds stuff to the root.
            root.Add(header);
            root.Add(createIconElements());
            root.Add(btnReroll);

            root.Add(createConfigElements());
            root.Add(createStyleElements());
            root.Add(createSaveElements());
            SetIconTexture();
        }

        private VisualElement createIconElements()
        {
            elementIcon = new VisualElement();
            elementIcon.AddToClassList("spriteImage");

            var elementContainer = getContainerElement();
            elementContainer.Add(elementIcon);
            return elementContainer;
        }

        private VisualElement createConfigElements()
        {
            var vec2Dimension = new Vector2IntField("Cell dimensions");
            vec2Dimension.value = defaultDimension;
            vec2Dimension.RegisterCallback<FocusOutEvent>((evt) =>
            {
                defaultConfig.Dimension = vec2Dimension.value;
                //And change back the value, sice it might was not updated due to some restrictions
                vec2Dimension.value = defaultConfig.Dimension;
                UpdateConfig();
            });

            var sliderRow = new VisualElement();
            sliderRow.AddToClassList("row");

            var sliderIndicator = new Label(defaultIteration.ToString());
            sliderIndicator.AddToClassList("iteration-lable");

            var sliderIterations = new SliderInt("Iterations", SLIDER_ITERATION_MIN, SLIDER_ITERATION_MAX);
            sliderIterations.value = defaultIteration;
            sliderIterations.AddToClassList("expand");
            sliderIterations.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                defaultConfig.Iterations = evt.newValue;
                sliderIndicator.text = defaultConfig.Iterations.ToString();
                UpdateConfig();
            });

            sliderRow.Add(sliderIterations);
            sliderRow.Add(sliderIndicator);

            var fieldSeed = new IntegerField("Seed");
            fieldSeed.SetEnabled(false);
            fieldSeed.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                noci = new Noci(defaultConfig, evt.newValue);
                SetIconTexture();
            });

            var toggleRandomSeed = new Toggle("Random seed");
            toggleRandomSeed.value = true;
            toggleRandomSeed.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                fieldSeed.SetEnabled(!evt.newValue);
                noci = evt.newValue ? new Noci(defaultConfig) : new Noci(defaultConfig, fieldSeed.value);
                SetIconTexture();
            });

            var elementContainer = getContainerElement();
            elementContainer.Add(vec2Dimension);
            elementContainer.Add(sliderRow);
            elementContainer.Add(toggleRandomSeed);
            elementContainer.Add(fieldSeed);
            return elementContainer;
        }

        private VisualElement createStyleElements()
        {
            var toggleContour = new Toggle("Draw contour");
            toggleContour.value = defaultContour;
            toggleContour.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                defaultConfig.DrawContour = evt.newValue;
                UpdateConfig();
            });

            var colorCell = new ColorField("Cell color");
            colorCell.value = defaultColorCell;
            colorCell.RegisterCallback<ChangeEvent<Color>>((evt) =>
            {
                defaultConfig.CellColor = evt.newValue;
                UpdateConfig();
            });

            var colorContour = new ColorField("Contour color");
            colorContour.value = defaultCollorContour;
            colorContour.RegisterCallback<ChangeEvent<Color>>((evt) =>
            {
                defaultConfig.ContourColor = evt.newValue;
                UpdateConfig();
            });

            var elementContainer = getContainerElement();
            elementContainer.Add(toggleContour);
            elementContainer.Add(colorCell);
            elementContainer.Add(colorContour);
            return elementContainer;
        }

        private VisualElement createSaveElements()
        {
            fieldSavePath = new TextField("Save path");
            fieldSavePath.value = defaultPath;

            intScaleFactor = new IntegerField("Output image scale factor");
            intScaleFactor.value = defaultScaleFactor;

            var btnSave = new Button() { text = "Save" };
            btnSave.clickable.clicked += () => Save();

            var elementContainer = getContainerElement();
            elementContainer.Add(fieldSavePath);
            elementContainer.Add(intScaleFactor);
            elementContainer.Add(btnSave);
            return elementContainer;
        }

        private VisualElement getContainerElement()
        {
            var elementContainer = new VisualElement();
            elementContainer.AddToClassList("container");
            return elementContainer;
        }

        private void UpdateConfig()
        {
            noci.SetConfig(defaultConfig);
            SetIconTexture();
        }

        private void Reroll()
        {
            noci.Reroll();
            SetIconTexture();
        }

        private void SetIconTexture()
        {
            generatedTex = noci.GetTexture2D();
            elementIcon.style.backgroundImage = generatedTex;
        }

        private void Save()
        {
            var fileName = $"noci_{noci.Seed}_{noci.RerollCount}.png";
            var pathWithSeed = Path.Combine(fieldSavePath.value, fileName);
            NociUtils.SaveTextureAsPNG(noci.GetTexture2D(intScaleFactor.value), pathWithSeed);
            AssetDatabase.Refresh();
        }
    }
}