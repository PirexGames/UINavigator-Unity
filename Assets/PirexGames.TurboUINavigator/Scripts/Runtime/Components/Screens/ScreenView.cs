using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PirexGames.Core.UINavigator
{
    public class ScreenView : ScreenBaseView
    {
        [SerializeField] protected Button[] btnsBack;

        protected override async UniTask Initialize()
        {
            await base.Initialize();
            if (btnsBack != null)
            {
                for (int i = 0; i < btnsBack.Length; i++)
                    btnsBack[i].onClick.AddListener(OnBack);
            }
        }

        private void OnDisable()
        {
            if (btnsBack == null) return;
            for (int i = 0; i < btnsBack.Length; i++)
                btnsBack[i].onClick.RemoveListener(OnBack);
        }

        protected virtual void OnBack()
        {
            UINavigator.Screens.Pop().Forget();
        }
    }
}
