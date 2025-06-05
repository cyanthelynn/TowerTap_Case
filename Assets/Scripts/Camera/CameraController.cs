using Cinemachine;
using UnityEngine;

namespace TowerTap
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform camFollower;
        private Vector3 _camFollowerPos;
        private CinemachineVirtualCamera _gameCam;
        private void Start()
        {
            if (UnityEngine.Camera.main != null)
            {
                _gameCam = UnityEngine.Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
                UnityEngine.Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.Follow = camFollower;
            }
        }

        public void SetCameraHeight(Block lastBlock)
        {
            _camFollowerPos = camFollower.transform.position;
            _camFollowerPos.y = lastBlock.transform.localPosition.y;
            camFollower.position = _camFollowerPos;
        }

        public void SetPlayGameCamera(bool result)
        {
            _gameCam.gameObject.SetActive(result);
        }
    }
}
