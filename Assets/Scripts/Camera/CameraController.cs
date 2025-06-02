using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
   [SerializeField] private Transform camFollower;
   private Vector3 _camFollowerPos;
   private CinemachineVirtualCamera _gameCam;
   private void Start()
   {
       if (Camera.main != null)
       {
           _gameCam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
           Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.Follow = camFollower;
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
