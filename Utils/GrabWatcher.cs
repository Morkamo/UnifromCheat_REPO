using System;
using HarmonyLib;
using UnityEngine;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO.Utils
{
    public class GrabWatcher : MonoBehaviour
    {
        private bool _lastGrabbed;
        
        public static event Action<Rigidbody> OnGrabbedObject;

        private void Update()
        {
            if (PlayerController.instance == null || 
                PlayerController.instance.playerAvatarScript == null || 
                PlayerController.instance.playerAvatarScript.physGrabber == null)
                return;

            var grabber = PlayerController.instance.playerAvatarScript.physGrabber;
            bool currentGrabbed = PhysGrabberRefs.grabbed(grabber);

            if (currentGrabbed != _lastGrabbed)
            {
                _lastGrabbed = currentGrabbed;

                Rigidbody rb = currentGrabbed 
                    ? PhysGrabberRefs.grabbedObject(grabber) 
                    : null;

                OnGrabbedObject?.Invoke(rb);
            }
        }
    }
}