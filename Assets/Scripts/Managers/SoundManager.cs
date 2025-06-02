using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class SoundManager : MonoBehaviour
{
    [Header("SOUND CLIPS")]
    [SerializeField] private AudioClip placementClip;
    [SerializeField] private AudioClip gameEndClip;

    [Space]
    [SerializeField] private int defaultPoolSize = 10;
    [SerializeField] private int maxPoolSize = 100;
    [SerializeField] private float startPitch = 0.5f;
    [SerializeField] private float pitchIncrement = 0.025f;
    [SerializeField] private float maxPitch = 1.5f;
    [SerializeField] private float resetDelay = 1.5f;

    private IEventBus _eventBus;
    private ObjectPool<AudioSource> _audioPool;
    private float _currentPitch;
    private float _lastPlacementTime = -Mathf.Infinity;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    private void Awake()
    {
        _currentPitch = startPitch;
        _audioPool = new ObjectPool<AudioSource>(
            createFunc: () =>
            {
                var go = new GameObject("PooledAudioSource");
                go.transform.SetParent(transform, false);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                return src;
            },
            actionOnGet: src => src.gameObject.SetActive(true),
            actionOnRelease: src =>
            {
                src.Stop();
                src.gameObject.SetActive(false);
            },
            actionOnDestroy: src => Destroy(src.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultPoolSize,
            maxSize: maxPoolSize
        );
    }

    private void OnEnable()
    {
        _eventBus.Subscribe<BlockPlacedEvent>(OnBlockPlaced);
        _eventBus.Subscribe<PerfectPlacementEvent>(OnBlockPerfectPlacement);
        _eventBus.Subscribe<GameStartEvent>(OnGameStart);
        _eventBus.Subscribe<GameEndedEvent>(OnGameEnded);
    }
    
    private void OnDisable()
    {
        _eventBus.Unsubscribe<BlockPlacedEvent>(OnBlockPlaced);
        _eventBus.Unsubscribe<PerfectPlacementEvent>(OnBlockPerfectPlacement);
        _eventBus.Unsubscribe<GameStartEvent>(OnGameStart);
        _eventBus.Unsubscribe<GameEndedEvent>(OnGameEnded);
    }

    private void OnGameStart(GameStartEvent evt)
    {
        _currentPitch = startPitch;
        _lastPlacementTime = -Mathf.Infinity;
    }

    private void OnBlockPlaced(BlockPlacedEvent evt)
    {
        _currentPitch = startPitch;
       PlayPlacementSound();
    }
    private void OnBlockPerfectPlacement(PerfectPlacementEvent obj)
    {
        if (Time.time - _lastPlacementTime >= resetDelay)
        {
            _currentPitch = startPitch;
        }
        
        _currentPitch = Mathf.Min(maxPitch, _currentPitch + pitchIncrement);
        _lastPlacementTime = Time.time;

        PlayPlacementSound();
    }
    
    private void OnGameEnded(GameEndedEvent evt)
    {
        PlayEndGameSound();
    }

    private void PlayPlacementSound()
    {
        if (placementClip == null) return;

        var src = _audioPool.Get();
        src.clip = placementClip;
        src.pitch = _currentPitch;
        src.Play();
        StartCoroutine(ReturnToPoolAfterDuration(src, placementClip.length / src.pitch));
    }

    private void PlayEndGameSound()
    {
        if (gameEndClip == null) return;

        var src = _audioPool.Get();
        src.pitch = 1;
        src.clip = gameEndClip;
        src.Play();
    }
    private IEnumerator ReturnToPoolAfterDuration(AudioSource src, float duration)
    {
        yield return new WaitForSeconds(duration);
        _audioPool.Release(src);
    }
}
