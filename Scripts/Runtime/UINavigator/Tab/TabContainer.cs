using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PirexGames.UI.Base
{
    [RequireComponent(typeof(ToggleGroupPublish))]
    public class TabContainer : MonoBehaviour
    {
        [SerializeField] private List<Tab> tabs = new List<Tab>();
        private int initTab = 0;
        private void Start()
        {
            SetUp();
            SetInitTab();
            foreach (var tab in tabs)
            {
                tab.toggleThisTab.onValueChanged.AddListener(delegate { OnChangeTab(tab); });
            }
            InitTab(initTab);
        }

        protected virtual void SetInitTab()
        {
            initTab = 0;
        }

        protected virtual void SetUp()
        {
            var toggleGroup = GetComponent<ToggleGroupPublish>();
            foreach (var tab in tabs)
            {
                tab.toggleThisTab.group = toggleGroup;
                tab.gameObject.SetActive(false);
            }

            tabs[0].gameObject.SetActive(true);
        }

        public void InitTab(int tabIndex)
        {
            tabs[tabIndex].toggleThisTab.isOn = true;
        }

        protected async void OnChangeTab(Tab tab)
        {
            if (!tab.toggleThisTab.isOn)
            {
                await tab.OnHide();
                tab.gameObject.SetActive(false);
            }
            else
            {
                tab.gameObject.SetActive(true);
                await tab.OnShow();
            }
        }
    }

}