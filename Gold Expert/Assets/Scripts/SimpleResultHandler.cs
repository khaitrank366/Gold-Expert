using System.Collections.Generic;
using UnityEngine;

public class SimpleResultHandler : ISlotResultHandler
{
    public override void HandleResult(List<SlotSymbolData> result)
    {
        foreach (var symbol in result)
        {
            Debug.Log($"Got: {symbol.symbolType}");
        }

        // Ví dụ thêm: đếm số lượng mỗi symbol
        var countMap = new Dictionary<SymbolType, int>();

        foreach (var symbol in result)
        {
            if (!countMap.ContainsKey(symbol.symbolType))
                countMap[symbol.symbolType] = 0;

            countMap[symbol.symbolType]++;
        }

        foreach (var kvp in countMap)
        {
            Debug.Log($"Symbol {kvp.Key} appeared {kvp.Value} time(s)");
        }
    }
}
