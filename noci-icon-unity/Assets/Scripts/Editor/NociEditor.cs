﻿using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace drstc.nociincon
{

    public class NociEditor : EditorWindow
    {
        private const int SLIDER_ITERATION_MIN = 1;
        private const int SLIDER_ITERATION_MAX = 10;

        private readonly string urlStyle = "Assets/Scripts/Editor/NociEditor.uss";
        private readonly Vector2Int defaultDimension = new Vector2Int(10, 10);
        private readonly int defaultIteration = 2;
        private readonly bool defaultContour = true;
        private readonly string defaultPath = "Assets/Noci/";

        private VisualElement elementIcon;
        private TextField fieldSavePath;
        private Texture2D generatedTex;

        private NociConfig defaultConfig;
        private Noci noci;

        [MenuItem("Tools/" + NociGlobals.NOCI_NAME + " generator")]
        public static void StartWindow()
        {
            // Opens the window, otherwise focuses it if it’s already open.
            var window = GetWindow<NociEditor>();

            // Adds a title to the window.
            window.titleContent = new GUIContent(NociGlobals.NOCI_NAME + " generator");

            // Sets a minimum size to the window.
            window.minSize = new Vector2(250, 50);
        }

        private void OnEnable()
        {
            defaultConfig = new NociConfig(defaultDimension, defaultIteration, defaultContour);
            noci = new Noci(defaultConfig);

            // Reference to the root of the window.
            var root = rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(urlStyle));

            var header = new Label(NociGlobals.NOCI_NAME);
            header.AddToClassList("heading");

            var btnRefresh = new Button() { text = "Reroll" };
            btnRefresh.clickable.clicked += () => Reroll();

            var vec2Dimension = new Vector2IntField("Cell dimensions");
            vec2Dimension.value = defaultDimension;
            vec2Dimension.RegisterCallback<ChangeEvent<Vector2Int>>((evt) =>
            {
                defaultConfig.Dimension = evt.newValue;
                // set back value since it could have been an invalid state
                vec2Dimension.value = defaultConfig.Dimension;
                UpdateConfig();
            });

            var sliderIterations = new SliderInt("Iterations", SLIDER_ITERATION_MIN, SLIDER_ITERATION_MAX);
            sliderIterations.value = defaultIteration;
            sliderIterations.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                defaultConfig.Iterations = evt.newValue;
                UpdateConfig();
            });

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

            var toggleContour = new Toggle("Draw contour");
            toggleContour.value = defaultContour;
            toggleContour.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                defaultConfig.DrawContour = evt.newValue;
                UpdateConfig();
            });

            var btnSave = new Button() { text = "Save" };
            btnSave.clickable.clicked += () => Save();

            fieldSavePath = new TextField("Save path");
            fieldSavePath.value = defaultPath;

            var elementContainer = new VisualElement();
            elementContainer.AddToClassList("container");

            elementIcon = new VisualElement();
            elementIcon.AddToClassList("spriteImage");
            elementContainer.Add(elementIcon);

            SetIconTexture();

            // Adds stuff to the root.
            root.Add(header);
            root.Add(elementContainer);

            root.Add(btnRefresh);
            root.Add(vec2Dimension);
            root.Add(sliderIterations);
            root.Add(toggleRandomSeed);
            root.Add(fieldSeed);
            root.Add(toggleContour);

            root.Add(fieldSavePath);
            root.Add(btnSave);
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
            NociUtils.SaveTextureAsPNG(generatedTex, pathWithSeed);
            AssetDatabase.Refresh();
        }
    }
}