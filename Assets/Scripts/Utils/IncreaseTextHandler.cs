using DG.Tweening;
using TMPro;
using TowerTap;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class IncreaseTextHandler : MonoBehaviour
{
    [Header("TEXT PREFABS")] [Space] 
    [SerializeField] private Transform perfectText;
    [SerializeField] private Transform comboText;
    
    
    [Space]
    [Header("INCREASE TEXT SETTINGS")]
    [SerializeField]private float endYPosition;
    [SerializeField]private float movementDuration;
    [SerializeField]private AnimationCurve movementCurve;
    [SerializeField]private float fadeDuration;
    [SerializeField]private AnimationCurve fadeCurve;


    [Inject] private ScoreManager _scoreManager;
    public void SpawnPerfectIncreaseText(Vector3 spawnPos)
    {
        var newPerfectText = GetPerfectText();
        newPerfectText.position = spawnPos;
        newPerfectText.DOMoveY(spawnPos.y + endYPosition,movementDuration).SetEase(movementCurve);
        var randXPos = Random.Range(-0.25f, 0.25f);
        newPerfectText.DOMoveX(transform.position.x+randXPos,movementDuration/3);
        newPerfectText.DOPunchScale(Vector3.one * 0.5f, movementDuration / 2);
        var currentText = newPerfectText.GetComponentInChildren<TextMeshProUGUI>();
        currentText.DOFade(0, fadeDuration).SetEase(fadeCurve).OnComplete(() =>
        {
            currentText.DOFade(1, 0.0001f);
            ReleasePerfectText(newPerfectText);
        });
    }
    public void SpawnComboIncreaseText(Vector3 spawnPos,float yOffset)
    {
        var newComboText = GetComboText();
        var currentText = newComboText.GetComponentInChildren<TextMeshProUGUI>();
        currentText.text = _scoreManager.CurrentComboCount+ "x" + " COMBO!";
        newComboText.position = spawnPos + new Vector3(0,yOffset,0);
        newComboText.DOMoveY(spawnPos.y + endYPosition,movementDuration).SetEase(movementCurve);
        var randXPos = Random.Range(-1f, 1f);
        newComboText.DOMoveX(transform.position.x+randXPos,movementDuration/3);
        newComboText.DOPunchScale(Vector3.one * 0.5f, movementDuration / 2);
        currentText.DOFade(0, fadeDuration).SetEase(fadeCurve).OnComplete(() =>
        {
            currentText.DOFade(1, 0.0001f);
            ReleaseComboText(newComboText);
        });
    }
    
    #region TEXT OBJECT POOLS

    
    private ObjectPool<Transform> _perfectTextPool;
    private ObjectPool<Transform> _comboTextPool;

    private void Awake()
    {
        _perfectTextPool = new ObjectPool<Transform>(
            createFunc: () =>
            {
                var obj = Instantiate(perfectText);
                obj.gameObject.SetActive(false);
                return obj;
            },
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
        );
        _comboTextPool = new ObjectPool<Transform>(
            createFunc: () =>
            {
                var obj = Instantiate(comboText);
                obj.gameObject.SetActive(false);
                return obj;
            },
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
        );
    }

    public Transform GetPerfectText()
    {
        return _perfectTextPool.Get();
    }   

    public void ReleasePerfectText(Transform text)
    {
        
        _perfectTextPool.Release(text);
    }

    public Transform GetComboText()
    {
        return _comboTextPool.Get();
    }

    public void ReleaseComboText(Transform text)
    {
        _comboTextPool.Release(text);
    }

    #endregion
}