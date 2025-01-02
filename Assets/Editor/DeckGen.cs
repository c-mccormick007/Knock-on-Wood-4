using UnityEngine;
using UnityEditor;

namespace BandCproductions
{
    public class DeckGenerator : EditorWindow
    {
        private string outputPath = "Assets/CardData";

        [MenuItem("Tools/Deck Generator")]
        public static void ShowWindow()
        {
            GetWindow<DeckGenerator>("Deck Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Deck Generator", EditorStyles.boldLabel);
            outputPath = EditorGUILayout.TextField("Output Path", outputPath);

            if (GUILayout.Button("Generate Deck"))
            {
                GenerateDeck();
            }
        }

        private void GenerateDeck()
        {
            if (!AssetDatabase.IsValidFolder(outputPath))
            {
                Debug.LogError($"Output path {outputPath} does not exist. Please create the folder or specify a valid path.");
                return;
            }

            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            int[] ranks = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }; // Ace, 2-10, Jack, Queen, King

            foreach (string suit in suits)
            {
                foreach (int rank in ranks)
                {
                    string cardName = GetCardName(rank, suit);
                    Card card = CreateInstance<Card>();
                    card.cardName = cardName;
                    card.rank = rank;
                    card.suit = suit;

                    string assetPath = $"{outputPath}/{cardName}.asset";
                    AssetDatabase.CreateAsset(card, assetPath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Deck successfully generated!");
        }

        private string GetCardName(int rank, string suit)
        {
            switch (rank)
            {
                case 1: return $"Ace of {suit}";
                case 11: return $"Jack of {suit}";
                case 12: return $"Queen of {suit}";
                case 13: return $"King of {suit}";
                default: return $"{rank} of {suit}";
            }
        }
    }
}
