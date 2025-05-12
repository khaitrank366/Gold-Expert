using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Threading.Tasks;

public enum symbolItem
{
    BigCoin,
    Shield,
    Lightning,
    Hammer
}

public class SlotMachine : Singleton<SlotMachine>,IModalUI
{
    [SerializeField] private CanvasGroup slotCanvasGroup;
    public Toggle autoSpinToggle;
    public SlotReel2[] reels;
    public GameObject symbolPrefab;
    public Sprite[] symbolSprites;
    public Button spinButton;
    
    public float spinSpeed = 2500f;
    public float stopDelay = 0.6f;
    private bool isSpinning = false;
    private bool canPressSpin = true;
    private bool isInOpponentHouse = false;

    private int stoppedCount = 0;
    private List<symbolItem> serverResult;
    private Dictionary<symbolItem, Sprite> itemToSprite;
    #region Debug
        [SerializeField] private symbolItem debugSymbol;
    #endregion


    void Start()
    {
        spinButton.onClick.AddListener(StartSpin);
        InitMapping();
        InitReels();
    }

    void InitMapping()
    {
        if (symbolSprites.Length != System.Enum.GetValues(typeof(symbolItem)).Length)
        {
            Debug.LogError("Thiếu Sprite trong danh sách symbolSprites");
            return;
        }

        itemToSprite = new Dictionary<symbolItem, Sprite>
        {
            { symbolItem.BigCoin, symbolSprites[0] },
            { symbolItem.Shield, symbolSprites[1] },
            { symbolItem.Lightning, symbolSprites[2] },
            { symbolItem.Hammer, symbolSprites[3] }
        };
    }

    void InitReels()
    {
        foreach (var reel in reels)
        {
            reel.symbolPrefab = symbolPrefab;
            reel.symbolSprites = symbolSprites;
            reel.ForcedWinSpriteIndex = null;

            reel.Init();
            reel.SetSpeed(0f);
        }
    }

    public async  void StartSpin()
    {
        if (!canPressSpin || isSpinning || isInOpponentHouse) return;
        if (!await CurrencyManager.Instance.SpendLightning(1))
        {
            Debug.Log("Không đủ sấm sét để quay.");
            return;
        }

        canPressSpin = false;
        isSpinning = true;
        stoppedCount = 0;

        // Tạm thời ép kết quả
        serverResult = new List<symbolItem>
        {
            debugSymbol,
            debugSymbol,
            debugSymbol
        };

        for (int i = 0; i < reels.Length; i++)
        {
            int spriteIndex = (int)serverResult[i];
            reels[i].ForcedWinSpriteIndex = spriteIndex;

            foreach (Transform child in reels[i].transform)
                child.DOKill();

            reels[i].Init();
            reels[i].SetSpeed(spinSpeed);
        }

        Invoke(nameof(StopSpin), 0.8f);
    }
    void StopSpin()
    {
        float delay = 0.5f;
        for (int i = 0; i < reels.Length; i++)
        {
            SlowDown(reels[i], i * delay);
        }
    }

    void SlowDown(SlotReel2 reel, float delay)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            reel.SetSpeed(0f);
            reel.StopWithTween(0.8f);

            stoppedCount++;
            if (stoppedCount == reels.Length)
            {
                DOVirtual.DelayedCall(0.6f, () =>
                {
                    CheckResult();
                    isSpinning = false;

                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        canPressSpin = true;
                        if (autoSpinToggle == null || !autoSpinToggle.isOn)
                            spinButton.interactable = true;

                        if (autoSpinToggle != null && autoSpinToggle.isOn)
                            StartSpin();
                    });
                });
            }
        });
    }

    async Task CheckResult()
    {
        var results = new List<symbolItem>();

        foreach (var reel in reels)
        {
            Sprite sprite = reel.GetCenterSymbol();
            if (sprite == null) return;

            int index = symbolSprites.ToList().IndexOf(sprite);
            if (index >= 0 && index < symbolSprites.Length)
                results.Add((symbolItem)index);
            else
                return;
        }

        if (results.All(r => r == results[0]))
        {
            Debug.Log($"WIN: {results[0]}");

            switch (results[0])
            {
                case symbolItem.BigCoin:
                    CurrencyManager.Instance.AddCoin(2000);
                    AutoSpinIfNeeded();
                    break;

                case symbolItem.Shield: //khien
                    int rs=await CurrencyManager.Instance.AddShield(1);
                    AutoSpinIfNeeded();
                    break;
                case symbolItem.Lightning:
                    CurrencyManager.Instance.AddLightning(1);
                    AutoSpinIfNeeded();
                    break;
                case symbolItem.Hammer:
                    Debug.Log("🛠 Búa xuất hiện → chuyển qua phá nhà đối phương");
                    OpenOpponentHouse();
                    break;
            }
        }
        else
        {
            Debug.Log($"LOSE: {string.Join(", ", results)}");
        }
    }

    void AutoSpinIfNeeded()
    {
        if (isInOpponentHouse) return;

        if (autoSpinToggle != null && autoSpinToggle.isOn)
        {
            StartSpin();
        }
    }

    #region Show/Hide Modal

    async void OpenOpponentHouse()
    {
        isInOpponentHouse = true;

        Hide();
        PlayerDataResponse rs_UserData= await HandleAPI.Instance.GetRandomUserAsync();
       
        // var data =  new PlayerMapProgress
        // {
        //     currentMapId = "Map1",
        //     buildings = new Dictionary<string, BuildingState>
        //     {
        //         ["Statue"] = new BuildingState { level = 1, needRepair = false },
        //         ["Castle"] = new BuildingState { level = 1, needRepair = true },
        //         ["Gate"] = new BuildingState { level = 1, needRepair = false }
        //     }
        // };
       GameUI.Instance.OpponentHouseUI.ShowHouse(rs_UserData);
    }

    public void BackToSpin()
    {
        Show();

        isInOpponentHouse = false;

        Debug.Log("⬅ Đã quay lại màn hình slot");
    }
    

    public void Show()
    {
        slotCanvasGroup.alpha = 1;
        slotCanvasGroup.interactable = true;
        slotCanvasGroup.blocksRaycasts = true;   
    }

    public void Hide()
    {
        slotCanvasGroup.alpha = 0;
        slotCanvasGroup.interactable = false;
        slotCanvasGroup.blocksRaycasts = false;
    }

    public bool IsVisible()
    {
        return slotCanvasGroup.alpha > 0f;
    }

    public void SwitchToMyBuilding()
    {
        Hide();
        BuildingManager.Instance.Show();
    }
    #endregion
}
