using UnityEngine;
using TMPro;

namespace PirexGames.UI.Base
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