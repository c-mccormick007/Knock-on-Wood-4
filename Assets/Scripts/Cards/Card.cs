using UnityEngine;

namespace BandCproductions
{
    public class Card : ScriptableObject
    {
        public string cardName;
        public int rank;
        public string suit;
        public Sprite sprite;

        [SerializeField] private int cardId;
        public int CardID => cardId;

        private void OnValidate()
        {
            if (cardId == 0)
            {
                cardId = GetInstanceID(); 
            }
        }

        public int GetCardId()
        {
            return cardId;
        }

    }
}
