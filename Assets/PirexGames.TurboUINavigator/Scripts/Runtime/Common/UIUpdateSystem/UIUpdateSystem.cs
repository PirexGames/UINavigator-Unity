using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.Core.UINavigator
{
    public class UIUpdateSystem : MonoBehaviour
    {
        private static List<UIUpdateComp> listComps = new List<UIUpdateComp>();

        public static void Register(UIUpdateComp comp)
        {
            listComps.Add(comp);
        }

        public static void UnRegister(UIUpdateComp comp)
        {
            listComps.Remove(comp);
        }

        private void Update()
        {
            for (int i = listComps.Count - 1; i >= 0; i--)
            {
                if (listComps[i].gameObject.activeSelf)
                {
                    listComps[i].OnUpdate();
                }
            }
        }
    }
}
