using UnityEngine;

namespace BandCproductions
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public int rank;
        public string suit;
        public Sprite sprite;
    }
}
