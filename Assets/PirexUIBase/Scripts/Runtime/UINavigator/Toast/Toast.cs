using UnityEngine;
using TMPro;

namespace Pirex.UI.Base
{
    public class Toast : UIView
    {
        [SerializeField] protected TextMeshProUGUI contentTxt;
        public virtual void Init(string content)
        {
            contentTxt.text = content;
        }
    }
}