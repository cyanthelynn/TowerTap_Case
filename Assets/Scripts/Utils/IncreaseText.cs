using TMPro;
using UnityEngine;

public class IncreaseText : MonoBehaviour
{
    private float _endYPosition;
    private float _movementDuration;
    private AnimationCurve _movementCurve;
    private float _fadeDuration;
    private AnimationCurve _fadeCurve;
    private TextMeshProUGUI _amountTextMesh;
    
    public void SetText(string text)
    {
        _amountTextMesh.text = text;
    }
}
