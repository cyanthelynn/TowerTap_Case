using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button claimButton;

    private string _missionId;
    private Action<string> _onClaim;

    public void Setup(
        Mission def,
        bool isComplete,
        bool isClaimed,
        Action<string> onClaimCallback)
    {
        _missionId = def.id;
        _onClaim = onClaimCallback;

        descriptionText.text = def.description;
        rewardText.text = def.rewardAmount.ToString();

        claimButton.interactable = isComplete && !isClaimed;
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaimClicked);
    }

    private void OnClaimClicked()
    {
        claimButton.interactable = false;
        _onClaim?.Invoke(_missionId);
    }
}