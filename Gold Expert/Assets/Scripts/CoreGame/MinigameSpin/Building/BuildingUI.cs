
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image buildingImage;
    public TextMeshProUGUI costText;
    public Button upgradeButton;
    public TextMeshProUGUI levelText;


    [Header("Data")]
    public string buildingId;
    public Sprite[] levelSprites; // Sprite cho các level 1-5

    private void Start()
    {
        upgradeButton.onClick.AddListener(OnClickUpgrade);
        Refresh();
    }

    private void OnClickUpgrade()
    {
        BuildingManager.Instance.UpgradeBuilding(buildingId);
        Refresh();
    }

    private void Refresh()
    {
        int level = BuildingManager.Instance.GetCurrentLevel(buildingId);

        if (levelSprites != null && levelSprites.Length > 0)
        {
            int index = Mathf.Clamp(level - 1, 0, levelSprites.Length - 1);
            buildingImage.sprite = levelSprites[index];
        }

        int cost = BuildingManager.Instance.GetUpgradeCost(buildingId, level);
        costText.text = cost.ToString();

        // ✨ Update thêm Level Text:
        if (levelText != null)
        {
            levelText.text = "Level " + level.ToString();
        }
    }
}
