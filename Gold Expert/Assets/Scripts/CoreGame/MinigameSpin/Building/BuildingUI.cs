using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementMyConstruction : MonoBehaviour
{
    [Header("UI Components")]
    public Image buildingImage;
    public TextMeshProUGUI costText;
    public Button upgradeButton;
    public TextMeshProUGUI levelText;


    [Header("Data")]
    public string buildingId;
    public Sprite[] levelSprites; // Sprite cho các level 1-5
    public Sprite brokenSprite; // Sprite cho trạng thái cần sửa chữa


    
    void OnEnable()
    {
         upgradeButton.onClick.AddListener(OnClickUpgrade);
        Refresh();
    }
    private void OnDisable()
    {
        upgradeButton.onClick.RemoveAllListeners();
    }

    private void OnClickUpgrade()
    {
        BuildingManager.Instance.UpgradeBuilding(buildingId);
        Refresh();
    }

    private void Refresh()
    {
        int level = BuildingManager.Instance.GetCurrentLevel(buildingId);
        if (level >= 5) return;
        
        // Check if building needs repair
        var state = BuildingManager.Instance.GetBuildingState(buildingId);
        if (state != null && state.needRepair)
        {
            buildingImage.sprite = brokenSprite;
            costText.text = "Repair: 2000";
            if (levelText != null)
            {
                levelText.text = "Needs Repair";
            }
            return;
        }
        
        if (levelSprites != null && levelSprites.Length > 0)
        {
            int index = Mathf.Clamp(level - 1, 0, levelSprites.Length - 1);
            buildingImage.sprite = levelSprites[index];
        }

        int cost = BuildingManager.Instance.GetUpgradeCost(buildingId, level);
        costText.text = cost.ToString();

        if (levelText != null)
        {
            levelText.text = "Level " + level.ToString();
        }
    }
}
