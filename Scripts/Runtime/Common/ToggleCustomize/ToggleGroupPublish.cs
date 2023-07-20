using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PirexGames.UI.Base
{
    public class ToggleGroupPublish : ToggleGroup
    {
        public List<Toggle> ListToggle => m_Toggles;
    }
}
