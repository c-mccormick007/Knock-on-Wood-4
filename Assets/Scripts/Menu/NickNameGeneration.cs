using UnityEngine;
using BandCproductions;
using TMPro;

namespace BandCproductions
{
    public class NickNameGeneration : MonoBehaviour
    {
        private void Awake()
        {
            var nickNameInputField = GetComponentInChildren<TextMeshProUGUI>();
            nickNameInputField.text = LocalPlayerData.NickName;
        }
    }
}