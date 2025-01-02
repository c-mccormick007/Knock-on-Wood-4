using UnityEngine;
using UnityEditor;

namespace BandCproductions
{
    public class CardSpriteAssigner : EditorWindow
    {
        private string cardDataPath = "Assets/Resources/Cards";
        private string spriteFolderPath = "Assets/Sprites/cardz/Image/PlayingCards";

        [MenuItem("Tools/Card Sprite Assigner")]
        public static void ShowWindow()
        {
            GetWindow<CardSpriteAssigner>("Card Sprite Assigner");
        }

        private void OnGUI()
        {
            GUILayout.Label("Card Sprite Assigner", EditorStyles.boldLabel);

            cardDataPath = EditorGUILayout.TextField("Card Data Path", cardDataPath);
            spriteFolderPath = EditorGUILayout.TextField("Sprite Folder Path", spriteFolderPath);

            if (GUILayout.Button("Assign Sprites"))
            {
                AssignSpritesToCards();
            }
        }

        private void AssignSpritesToCards()
        {
            if (!AssetDatabase.IsValidFolder(cardDataPath))
            {
                Debug.LogError($"Card data path '{cardDataPath}' does not exist.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(spriteFolderPath))
            {
                Debug.LogError($"Sprite folder path '{spriteFolderPath}' does not exist.");
                return;
            }

            // Load all card assets
            string[] cardAssetPaths = AssetDatabase.FindAssets("t:Card", new[] { cardDataPath });
            if (cardAssetPaths.Length == 0)
            {
                Debug.LogError("No Card assets found in the specified path.");
                return;
            }

            // Load all texture assets
            string[] textureAssetPaths = AssetDatabase.FindAssets("t:Texture2D", new[] { spriteFolderPath });
            if (textureAssetPaths.Length == 0)
            {
                Debug.LogError("No textures found in the specified path.");
                return;
            }

            foreach (string cardGUID in cardAssetPaths)
            {
                string cardPath = AssetDatabase.GUIDToAssetPath(cardGUID);
                Card card = AssetDatabase.LoadAssetAtPath<Card>(cardPath);

                if (card == null)
                {
                    Debug.LogWarning($"Failed to load Card asset at path: {cardPath}");
                    continue;
                }

                string suitName = card.suit.EndsWith("s", System.StringComparison.OrdinalIgnoreCase)
                         ? card.suit.Substring(0, card.suit.Length - 1)
    :                      card.suit;

                // Construct the expected sprite name
                string expectedSpriteName = $"{suitName}{card.rank:00}_Slice"; // Match the sprite name format
                Sprite matchingSprite = null;

                foreach (string textureGUID in textureAssetPaths)
                {
                    string texturePath = AssetDatabase.GUIDToAssetPath(textureGUID);
                    Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(texturePath);

                    foreach (Object subAsset in subAssets)
                    {
                        Debug.Log("sub asset: " + subAsset + "\nexpectedSpriteName = " + expectedSpriteName);
                        if (subAsset is Sprite sprite && string.Equals(sprite.name, expectedSpriteName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            matchingSprite = sprite;
                            break;
                        }
                    }

                    if (matchingSprite != null)
                        break;
                }

                if (matchingSprite != null)
                {
                    card.sprite = matchingSprite;
                    EditorUtility.SetDirty(card); // Mark the card asset as dirty so Unity saves changes
                    Debug.Log($"Assigned sprite '{expectedSpriteName}' to card '{card.cardName}'");
                }
                else
                {
                    Debug.LogWarning($"No matching sprite found for card: {card.cardName} (Expected: {expectedSpriteName})");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Sprite assignment complete!");
        }
    }
}
