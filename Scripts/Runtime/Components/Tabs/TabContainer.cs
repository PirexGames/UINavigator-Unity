using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace PirexGames.Core.UINavigator
{
    [Serializable]
    public struct TabMapping
    {
        public AssetReferenceGameObject tabPrefab;
        public string customData;
        public Toggle toggle;
    }

    public class TabContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private List<TabMapping> mappings = new List<TabMapping>();
        [SerializeField] private int defaultTabIndex = 0;

        private TabView _currentTab;
        private int _currentTabIndex = -1;
        private string _currentKey;

        private void Start()
        {
            if (contentRoot == null) contentRoot = GetComponent<RectTransform>();

            for (int i = 0; i < mappings.Count; i++)
            {
                var mapping = mappings[i];
                int index = i; // Closure capture
                if (mapping.toggle != null)
                {
                    mapping.toggle.onValueChanged.AddListener((isOn) => 
                    {
                        if (isOn) OnChangeTab(index).Forget();
                    });
                }
            }

            // Init first tab
            if (mappings.Count > defaultTabIndex)
            {
                var defaultToggle = mappings[defaultTabIndex].toggle;
                if (defaultToggle != null)
                {
                    defaultToggle.isOn = true;
                    // Note: We check if it's already the current key to avoid double init
                    OnChangeTab(defaultTabIndex).Forget();
                }
            }
        }

        public async UniTask SwitchTab(int index)
        {
            if (index < 0 || index >= mappings.Count) return;
            
            var mapping = mappings[index];
            if (mapping.toggle != null) mapping.toggle.isOn = true;
            await OnChangeTab(index);
        }

        protected virtual async UniTask OnChangeTab(int index)
        {
            if (_currentTabIndex == index && _currentTab != null) return;
            if (index < 0 || index >= mappings.Count) return;

            var mapping = mappings[index];
            if (mapping.tabPrefab == null) return;

            string key = mapping.tabPrefab.RuntimeKey.ToString();

            // 1. Hide current
            if (_currentTab != null)
            {
                await _currentTab.OnHide();
                if (_currentTab.IsPooling)
                {
                    UIContainer.ReturnToPool(_currentKey, _currentTab.gameObject);
                }
                else
                {
                    Destroy(_currentTab.gameObject);
                }
                _currentTab = null;
            }

            _currentTabIndex = index;
            _currentKey = key;

            // 2. Spawn next using global static helper
            var go = await UIContainer.SpawnView(key, contentRoot);
            if (go != null)
            {
                _currentTab = go.GetComponent<Tab>();
                if (_currentTab != null)
                {
                    _currentTab.NameAddressable = key;
                    _currentTab.TabData = mapping.customData;
                    
                    OnSetupTab(_currentTab, mapping);

                    await _currentTab.DoInitialize();
                    await _currentTab.OnShow();
                }
            }
        }

        protected virtual void OnSetupTab(TabView tab, TabMapping mapping)
        {
            // Override this in subclasses to inject specific data
        }
    }
}