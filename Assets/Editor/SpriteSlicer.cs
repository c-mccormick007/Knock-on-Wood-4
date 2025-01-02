using UnityEditor;
using UnityEngine;
using UnityEditor.U2D.Sprites;
using UnityEngine.U2D;

public class CustomCardSlicer : EditorWindow
{
    [MenuItem("Tools/Card Slicer")]
    public static void ShowWindow()
    {
        GetWindow<CustomCardSlicer>("Card Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Card Slicer", EditorStyles.boldLabel);

        if (GUILayout.Button("Apply Slice Settings"))
        {
            ApplySliceSettingsToSelected();
        }
    }

    private void ApplySliceSettingsToSelected()
    {
        foreach (Object obj in Selection.objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
            {
                Debug.LogWarning($"Skipping {obj.name}: Not a Texture asset.");
                continue;
            }

            if (importer.textureType != TextureImporterType.Sprite)
            {
                Debug.LogWarning($"Skipping {obj.name}: Not imported as a Sprite.");
                continue;
            }

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            importer.spriteImportMode = SpriteImportMode.Multiple;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            if (dataProvider == null)
            {
                Debug.LogWarning($"Failed to get ISpriteEditorDataProvider for {obj.name}. Ensure the asset is a Sprite and in 'Multiple' mode.");
                continue;
            }

            dataProvider.InitSpriteEditorDataProvider();

            SpriteRect newSlice = new SpriteRect
            {
                name = obj.name + "_Slice",
                rect = new Rect(397, 59, 1248, 1935),
                border = new Vector4(32, 32, 32, 32),
                alignment = SpriteAlignment.Custom,
                pivot = new Vector2(0.5f, 0.5f)
            };

            SpriteRect[] newSlices = { newSlice };
            dataProvider.SetSpriteRects(newSlices);
            dataProvider.Apply();
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            Debug.Log($"Slicing settings applied to: {obj.name}");
        }
    }
}
