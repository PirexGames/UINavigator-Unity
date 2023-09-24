using UnityEngine;
using TMPro;

namespace PirexGames.Core.UINavigator
{
    public class Toast : EView
    {
        [SerializeField] protected TextMeshProUGUI contentTxt;
        public virtual void Init(string content)
        {
            contentTxt.text = content;
        }
    }
}