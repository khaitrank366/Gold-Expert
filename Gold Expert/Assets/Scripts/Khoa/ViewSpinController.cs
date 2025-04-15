using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ViewSpinController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private RectTransform content;         // Kéo object Content vào đây
    [SerializeField] private float slotHeight = 150f;        // Chiều cao mỗi slot
    [SerializeField] private int spinSteps = 20;             // Số bước quay (1 bước = trượt 1 slot)
    [SerializeField] private float stepDuration = 0.08f;     // Thời gian mỗi bước quay

    private Vector2 startPos;

    private void Start()
    {
        startPos = content.anchoredPosition;
    }

    public void StartSpin()
    {
        DOTween.Kill(content); // Ngắt tween cũ nếu có
        content.anchoredPosition = startPos;

        SpinStep(0);
    }
[Button]
    private void SpinStep(int currentStep)
    {
        if (currentStep >= spinSteps)
        {
            Debug.Log("✅ Spin done!");
            // TODO: Gán 3 kết quả cuối cùng ở đây
            return;
        }

        // Di chuyển xuống 1 slot (theo chiều âm của Y)
        content.DOAnchorPosY(content.anchoredPosition.y - slotHeight, stepDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Đưa slot đầu xuống cuối
                Transform first = content.GetChild(0);
                first.SetAsLastSibling();

                // Bù lại vị trí content để giữ nguyên khung nhìn (không bị khoảng trống)
                content.anchoredPosition += new Vector2(0, slotHeight);

                // Gọi bước tiếp theo
                SpinStep(currentStep + 1);
            });
    }
}
