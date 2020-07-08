using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace drstc.nociincon
{
    public class NociEditor : EditorWindow
    {
        private readonly string urlStyle = "Assets/Scripts/Editor/NociEditor.uss";

        private NociConfig defaultConfig;
        private VisualElement iconElement;

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

            var header = new Label("NOCI");
            header.AddToClassList("heading");

            var generateBtn = new Button() { text = "Refresh" };
            generateBtn.clickable.clicked += () => Refresh();

            var container = new VisualElement();
            container.AddToClassList("container");

            iconElement = new VisualElement();
            iconElement.AddToClassList("spriteImage");
            container.Add(iconElement);
            
            iconElement.style.backgroundImage = NociFactory.GetSprite(defaultConfig).texture;

            // Adds it to the root.
            root.Add(header);
            root.Add(generateBtn);
            root.Add(container);
        }

        private void Refresh()
        {
            iconElement.style.backgroundImage = NociFactory.GetSprite(defaultConfig).texture;
        }
    }
}