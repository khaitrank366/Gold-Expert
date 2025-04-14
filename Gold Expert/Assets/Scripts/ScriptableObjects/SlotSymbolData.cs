using UnityEngine;

[CreateAssetMenu(fileName = "SymbolData", menuName = "SlotMachine/SymbolData")]
public class SlotSymbolData : ScriptableObject
{
    public SymbolType symbolType;
    public Sprite icon;
    [Range(0f, 100f)] public float weight = 1f;
}

public enum SymbolType
{
    Coin,
    Attack,
    Shield,
    Raid,
    Pig
}