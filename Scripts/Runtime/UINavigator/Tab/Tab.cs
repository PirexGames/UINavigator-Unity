using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PirexGames.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Tab : EView
    {
        public Toggle toggleThisTab;
    }
}