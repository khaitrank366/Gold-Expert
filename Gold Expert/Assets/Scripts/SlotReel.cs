using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotReel 
{
    private List<SlotSymbolData> symbols;

    public SlotReel(SlotReelConfig config)
    {
        symbols = config.symbols;
    }

    public SlotSymbolData Spin()
    {
        float totalWeight = 0f;

        // Tính tổng trọng số của tất cả biểu tượng
        foreach (var s in symbols)
        {
            totalWeight += s.weight;
        }

        // Dùng random của Unity để chọn 1 giá trị từ 0 đến tổng weight
        float roll = UnityEngine.Random.Range(0f, totalWeight);

        float cumulative = 0f;

        // Tìm biểu tượng tương ứng với kết quả roll
        foreach (var symbol in symbols)
        {
            cumulative += symbol.weight;
            if (roll <= cumulative)
            {
                return symbol;
            }
        }

        // Fallback (hiếm khi xảy ra, chỉ phòng khi roll vượt tổng weight)
        return symbols[0];
    }
}
