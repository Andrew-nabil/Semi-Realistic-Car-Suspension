
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public static class CameraSwitcher
{
   private static List<CinemachineVirtualCamera> _cameras = new List<CinemachineVirtualCamera>();
   public static CinemachineVirtualCamera activeCamera = null;

   public static bool IsActiveCamera(CinemachineVirtualCamera camera)
   {
      return camera == activeCamera;
   }
   public static void SwitchCamera(CinemachineVirtualCamera camera)
   {
      camera.Priority = 10;
      activeCamera = camera;
      foreach (CinemachineVirtualCamera c in _cameras)
      {
         if (c != camera && camera.Priority != 0)
         {
            c.Priority = 0;
         }
      }
   }

   public static void Register(CinemachineVirtualCamera camera)
   {
      _cameras.Add(camera);
   }

   public static void OnRegister(CinemachineVirtualCamera camera)
   {
      _cameras.Remove(camera);
   }
}
