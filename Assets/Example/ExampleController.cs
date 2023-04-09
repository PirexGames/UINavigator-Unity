using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pirex.UI.Base;
using UnityEngine;

public class ExampleController : MonoBehaviour
{
    void Start()
    {
        ScreenContainer.Push("MainScreen").Forget();
    }
}
