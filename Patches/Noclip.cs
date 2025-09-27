using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnifromCheat_REPO.Patches
{
    public class Noclip : MonoBehaviour
    {
        public static bool isActive;
        public NoclipComponent noclipComponent;
        private Rigidbody rb;
        private TextMeshProUGUI noclipText;

        private void Start()
        {
            if (PlayerController.instance == null) 
                return;

            var player = PlayerController.instance;
            if (player == null) 
                return;

            rb = player.rb;
            noclipText = GameObject.Find("NoclipText")?.GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (!Core.isNoclipEnabled)
            {
                ResetNoclip();
                return;
            }

            if (Keyboard.current.leftAltKey.wasPressedThisFrame)
            {
                isActive = !isActive;
                UpdateNoclipState();
            }
        }

        private void UpdateNoclipState()
        {
            if (isActive)
            {
                if (noclipText != null)
                    noclipText.SetText("<b>Noclip = ON</b>");

                if (noclipComponent == null && PlayerController.instance != null)
                    noclipComponent = PlayerController.instance.gameObject.AddComponent<NoclipComponent>();

                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
            else
            {
                ResetNoclip();
            }
        }

        private void ResetNoclip()
        {
            isActive = false;

            if (noclipText != null)
                noclipText.SetText("<b>Noclip = OFF</b>");

            if (noclipComponent != null)
            {
                Destroy(noclipComponent);
                noclipComponent = null;
            }

            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
            }
        }
    }

    public class NoclipComponent : MonoBehaviour
    {
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        private void Update()
        {
            if (!Noclip.isActive) return;

            Vector3 input = Vector3.zero;

            if (Keyboard.current.wKey.isPressed) input += transform.forward;
            if (Keyboard.current.sKey.isPressed) input -= transform.forward;
            if (Keyboard.current.aKey.isPressed) input -= transform.right;
            if (Keyboard.current.dKey.isPressed) input += transform.right;
            if (Keyboard.current.spaceKey.isPressed) input += transform.up;
            if (Keyboard.current.ctrlKey.isPressed) input -= transform.up;

            transform.position += input * (Core.noclipSpeed * Time.deltaTime * 5);

            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            if (PlayerController.instance != null && PlayerController.instance.CollisionController != null)
            {
                PlayerController.instance.CollisionController.Grounded = true;
            }
        }

        private void OnDestroy()
        {
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
            }
        }
    }
}
