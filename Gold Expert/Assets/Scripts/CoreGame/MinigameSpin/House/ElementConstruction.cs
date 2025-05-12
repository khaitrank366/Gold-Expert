using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
// Each construction has 2 state : Normal and Repair
// Each state have visual difference
// If attack construct repair -> current user only get 50% coin Init
// Else get full coin init

public class ElementOpponentConstruction : MonoBehaviour
{
    public BuildingState buildingState;
    public TMP_Text nameText;
    public Button button;
   public Action OnClick;    
    void Start()
    {
        button.onClick.AddListener(OnClickButton);
    }

    public void Setup(string name, BuildingState state, Action onClick)
    {
        nameText.text = name;
        buildingState = state;
        OnClick = onClick;

        if(state.needRepair==true)
        {
            button.interactable = false;
        }
    }

    private void OnClickButton()
    {
        OnClick?.Invoke();
    }
}
