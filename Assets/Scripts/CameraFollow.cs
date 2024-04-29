
using System;
using UnityEngine;
using Cinemachine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera topDownCamera;
    [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;

    private void OnEnable()
    {
        CameraSwitcher.Register(topDownCamera);
        CameraSwitcher.Register(thirdPersonCamera);
        CameraSwitcher.SwitchCamera(topDownCamera);
    }

    private void OnDisable()
    {
        CameraSwitcher.OnRegister(topDownCamera);
        CameraSwitcher.OnRegister(thirdPersonCamera);
    }

    private void LateUpdate()
    {
        
      
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            print("F");
            if (CameraSwitcher.IsActiveCamera(thirdPersonCamera))
            {
                CameraSwitcher.SwitchCamera(topDownCamera);
            }
            else if(CameraSwitcher.IsActiveCamera(topDownCamera))
            {
                CameraSwitcher.SwitchCamera(thirdPersonCamera);
            }

        }


        
    }
}
