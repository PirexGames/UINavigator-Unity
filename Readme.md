# Turbo UINavigator

**Turbo UINavigator** is a high-performance, professional-grade UI navigation framework for Unity. Designed for speed, flexibility, and zero memory overhead, it provides a robust system for managing Screens, Modals, Toasts, and Tooltips with ease.

[![Unity 2021.3+](https://img.shields.io/badge/unity-2021.3%2B-blue.svg)](https://unity.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🚀 Key Features

- **Zero-Allocation Core**: All core systems are optimized to avoid GC allocations during transitions.
- **Addressables Integration**: Native support for Unity Addressables, ensuring efficient asset loading and unloading.
- **Static Hub API**: Control your entire UI via the `UINavigator` static class.
- **Modular Animations**: Use the built-in animation system or plug in **LitMotion** (recommended) or **DOTween** (optional).
- **Smart Tooltips**: Boundary-aware, pivot-independent tooltips with automatic repositioning.
- **Memory Management**: Automatic release of object pools and asset caches during scene transitions.
- **Universal Sheets**: High-performance Tab/Sheet container with O(1) lookup speeds.
- **Object Pooling**: Optional turn on pooling in prefab ui view (Screen, Modal, Alert, Tooltip, Toast...)
  
---

## 📦 Installation

### Prerequisites

Before installing, add the following dependencies to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.unity.addressables": "2.3.1",
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
  }
}
```

*(Optional)* For smooth animations, you can install **LitMotion** (Recommended) or **DOTween**:
- **LitMotion**: Add `"com.annulusgames.lit-motion": "https://github.com/annulusgames/LitMotion.git?path=src/LitMotion/Assets/LitMotion"` to your dependencies, then add `LITMOTION_ENABLED` to **Edit > Project Settings > Player > Scripting Define Symbols**.
- **DOTween**: Install from the [Unity Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676), then add `DOTWEEN_ENABLED` to **Scripting Define Symbols**.

---

### Install Turbo UINavigator

Add the following entry to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.pirexgames.uinavigator": "https://github.com/PirexGames/UINavigator-Unity.git#[version]"
  }
}
```

Replace `[version]` with a specific tag, e.g. `#1.0.0`, or use `#upm` for the latest.

---

## ✨ Quick Start

### 1. Scene Setup
Add a `UINavigator` prefab or create a GameObject with the following components:
- `ScreenContainer`
- `ModalContainer`
- `ToastContainer`
- `TooltipContainer`

Ensure they are registered in the `UINavigator` static hub (done automatically if they are in the scene).

### 2. Basic Usage

#### Opening a Screen
```csharp
using PirexGames.Core.UINavigator;

// Simple push
UINavigator.Screens.Push<MainScreenView>("Screen_Main").Forget();

// Typed push with callback initialization
await UINavigator.Screens.Push<SettingsView>("Screen_Settings", onInit: view => {
    view.Setup(currentConfig);
});
```

#### Showing a Modal
```csharp
// Modals return the instance for further control
var modal = await UINavigator.Modals.Push<ConfirmModal>("Modal_Confirm");
modal.OnConfirm += () => Debug.Log("Confirmed!");
```

#### Triggering Tooltips
```csharp
UINavigator.Tooltips.Show<TooltipBaseAutoView>(
    "Tooltip_Info", 
    "This is a smart tooltip!", 
    targetRect, 
    TooltipPosition.AUTO
).Forget();
```

---

## 🛠 Advanced Features

### Type-Safe Access (Code Generation)
Turbo UINavigator includes a generator that scans your project for custom containers and generates a type-safe hub.

1. Set your Container's **Name Mode** to `Custom` and give it a unique **Container Name**.
2. Run the generator via: `Tools > PirexGames > UINavigator > Regenerate Hub Accessors`.
3. Access your custom containers via `UINavigatorHub` with full IDE IntelliSense:

```csharp
// Instead of searching by string:
var inventory = UINavigator.GetContainer<ModalContainer>("Inventory");

// Use the generated hub:
UINavigatorHub.Inventory.Push<InventoryView>("Modal_Inventory").Forget();
```

*Note: The generator automatically runs whenever you save a Scene or a Prefab.*

### Automatic Memory Cleanup
By default, Turbo UINavigator clears all object pools and Addressable asset caches when the scene changes to prevent memory leaks.
```csharp
UINavigator.AutoReleasePoolOnSceneChange = true;
```

### Zero-Allocation Transitions
Instead of `foreach` and `LINQ`, Turbo UINavigator uses manual loops and internal task pooling to ensure that navigating your UI doesn't cause frame spikes.

---

## 📝 Best Practices

1. **Pooling**: Enable `IsPooling` on your View prefabs for views that are opened/closed frequently (like Toasts).
2. **Cancellation**: Use `view.SessionToken` for your custom async logic inside views to ensure tasks are cancelled when the view is closed.
3. **Pivots**: The system automatically handles pivots. For best results with Tooltips, ensure your UI containers have correctly configured `RectTransforms`.

---

## 🤝 Support
Created by **PirexGames**. For more information or support, contact the team at [contact@pirexgames.com].
