using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace drstc.nociincon
{
    public class NociEditor : EditorWindow
    {
        private readonly string urlStyle = "Assets/Scripts/Editor/NociEditor.uss";

        private NociConfig defaultConfig;
        private VisualElement elementIcon;

        private Texture2D generatedTex; 


        [MenuItem("Tools/Noci icon generator")]
        public static void StartWindow()
        {
            // Opens the window, otherwise focuses it if it’s already open.
            var window = GetWindow<NociEditor>();

            // Adds a title to the window.
            window.titleContent = new GUIContent("Noci icon generator");

            // Sets a minimum size to the window.
            window.minSize = new Vector2(250, 50);
        }

        private void OnEnable()
        {
            defaultConfig = new NociConfig(10, 2);

            // Reference to the root of the window.
            var root = rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(urlStyle));

            var header = new Label("NOCI | ICON");
            header.AddToClassList("heading");

            var btnGenerate = new Button() { text = "Refresh" };
            btnGenerate.clickable.clicked += () => Refresh();

            var btnSave = new Button() { text = "Save" };
            btnSave.clickable.clicked += () => Save();

            var elementContainer = new VisualElement();
            elementContainer.AddToClassList("container");

            elementIcon = new VisualElement();
            elementIcon.AddToClassList("spriteImage");
            elementContainer.Add(elementIcon);
            
            Refresh();

            // Adds it to the root.
            root.Add(header);
            root.Add(btnGenerate);
            root.Add(elementContainer);
            root.Add(btnSave);
        }

        private void Refresh()
        {
            generatedTex = NociFactory.GetSprite(defaultConfig).texture;
            elementIcon.style.backgroundImage = generatedTex;
        }
        private void Save()
        {
            NociUtils.SaveTextureAsPNG(generatedTex, "Assets/Sprites", "test");
            AssetDatabase.Refresh();
        }
    }
}