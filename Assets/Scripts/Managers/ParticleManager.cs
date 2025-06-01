using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private int defaultPoolSize = 10;
    [SerializeField] private int maxPoolSize = 50;
    [SerializeField] private ParticleSystem perfectParticle;

    private readonly Dictionary<ParticleSystem, ObjectPool<ParticleSystem>> _pools
        = new Dictionary<ParticleSystem, ObjectPool<ParticleSystem>>();

    public void PlayPerfectParticle(Vector3 position,Vector3 scale)
    {
        Play(perfectParticle, position, Quaternion.Euler(-90, 0, 0),scale);
    }

    
    #region Overload Play Methods

    public void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return;

        if (!_pools.TryGetValue(prefab, out var pool))
        {
            pool = CreatePool(prefab);
            _pools[prefab] = pool;
        }

        var instance = pool.Get();
        instance.transform.SetParent(transform, false);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.gameObject.SetActive(true);
        instance.Clear(true);
        instance.Play();
    }
    private void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (prefab == null) return;
        if (!_pools.TryGetValue(prefab, out var pool))
        {
            pool = CreatePool(prefab);
            _pools[prefab] = pool;
        }

        var instance = pool.Get();
        instance.transform.SetParent(transform, false);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.transform.localScale = scale; 
        instance.gameObject.SetActive(true);
        instance.Clear(true);
        instance.Play();
    }

    #endregion
  

    private ObjectPool<ParticleSystem> CreatePool(ParticleSystem prefab)
    {
        ObjectPool<ParticleSystem> pool = null;
        pool = new ObjectPool<ParticleSystem>(
            createFunc: () =>
            {
                var inst = Instantiate(prefab);
                inst.gameObject.SetActive(false);
                var main = inst.main;
                main.stopAction = ParticleSystemStopAction.Callback;
                var callbackComp = inst.gameObject.AddComponent<ParticleCallback>();
                callbackComp.Initialize(inst, pool);
                return inst;
            },
            actionOnGet: ps =>
            {
                ps.gameObject.SetActive(true);
            },
            actionOnRelease: ps =>
            {
                ps.gameObject.SetActive(false);
            },
            actionOnDestroy: ps =>
            {
                Destroy(ps.gameObject);
            },
            collectionCheck: false,
            defaultCapacity: defaultPoolSize,
            maxSize: maxPoolSize
        );
        return pool;
    }
}

public class ParticleCallback : MonoBehaviour
{
    private ParticleSystem _ps;
    private ObjectPool<ParticleSystem> _pool;

    public void Initialize(ParticleSystem ps, ObjectPool<ParticleSystem> associatedPool)
    {
        _ps = ps;
        _pool = associatedPool;
    }

    private void OnParticleSystemStopped()
    {
        if (_pool != null)
        {
            _pool.Release(_ps);
        }
        else
        {
            _ps.gameObject.SetActive(false);
        }
    }
}
