using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PirexGames.Core.UINavigator;
using UnityEngine;

public class ExampleController : MonoBehaviour
{
    void Start()
    {
        UINavigator.Screens.Push<MainScreen>("MainScreen").Forget();
    }
}
