using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SlotMachine : MonoBehaviour
{
    List<SlotReel> reels;
    [SerializeField] ISlotResultHandler resultHandler;
    [SerializeField] List<SlotReelConfig> reelConfigSO;
    public SlotMachine(List<SlotReelConfig> reelConfigs, ISlotResultHandler handler)
    {
        reels = reelConfigs.Select(config => new SlotReel(config)).ToList();
        resultHandler = handler;
    }

    public List<SlotSymbolData> Spin()
    {
        var results = reels.Select(reel => reel.Spin()).ToList();
        //resultHandler.HandleResult(results);
        return results;
    }

    [ContextMenu(nameof(Test))]
    public void Test()
    {
        var tempReel = reelConfigSO.Select(config => new SlotReel(config)).ToList();
        var results = tempReel.Select(reel => reel.Spin()).ToList();
        resultHandler.HandleResult(results);
    }
}

public abstract class ISlotResultHandler : MonoBehaviour
{
    public abstract void HandleResult(List<SlotSymbolData> result);
}