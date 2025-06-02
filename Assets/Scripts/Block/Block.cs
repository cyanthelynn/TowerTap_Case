using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private ParticleSystem perfectEffect;
    private Rigidbody _rb;
    void Awake() => _rb = GetComponent<Rigidbody>();

    public void SetKinematic(bool result)
    {
        _rb.isKinematic = result;
    }

    public void PlayPerfectEffect() => perfectEffect.Play();

    public void SetDefaultRotation()
    {
        var transformRotation = transform.rotation;
        transformRotation.eulerAngles = Vector3.zero;
        transform.rotation = transformRotation;
    }

    public void SelfExplode()
    {   
        SetKinematic(false);
        _rb.AddExplosionForce(2000,transform.position,5,2,ForceMode.Impulse);
    }
}
