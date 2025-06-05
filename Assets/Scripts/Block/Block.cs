using UnityEngine;

namespace TowerTap
{
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
    }
}
