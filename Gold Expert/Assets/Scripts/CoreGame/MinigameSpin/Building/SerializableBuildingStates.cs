
using System.Collections.Generic;

[System.Serializable]
public class SerializableBuildingStates
{
    public List<BuildingData> Buildings = new List<BuildingData>();

    public SerializableBuildingStates(Dictionary<string, BuildingData> dict)
    {
        foreach (var kvp in dict)
        {
            Buildings.Add(kvp.Value);
        }
    }

    public Dictionary<string, BuildingData> ToDictionary()
    {
        Dictionary<string, BuildingData> result = new Dictionary<string, BuildingData>();
        foreach (var b in Buildings)
        {
            result[b.BuildingId] = b;
        }
        return result;
    }
}
