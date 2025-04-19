using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReelConfig", menuName = "SlotMachine/ReelConfig")]
public class SlotReelConfig : ScriptableObject
{
    public List<SlotSymbolData> symbols;
}
