using AndrewDowsett.CommonObservers;
using AndrewDowsett.Utility;
using System;
using UnityEngine;

namespace AndrewDowsett.MovementSystems
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController3D : MonoBehaviour, IUpdateObserver
    {
        public static FirstPersonController3D Instance { get; private set; }

        public float CurrentPitch { get { return currentPitch; } }

        [Header("Camera Reference")]
        public Camera cam;

        [Header("Movement Settings")]
        public float movementSpeed = 5.0f;
        public float sprintMultiplier = 2.0f;
        public float jumpStrength = 5.0f;

        [Header("Mouse Settings")]
        public float mouseSensitivityX = 5.0f;
        public float mouseSensitivityY = 5.0f;

        private Vector3 lastForwardDirection;
        private CharacterController cc;
        private Vector3 currentMovementVector;
        private float currentYaw;
        private float currentPitch;
        private float currentYVelocity;
        private bool hasControl = true;
        private ulong controlledClientId;

        void Start()
        {
            Instance = this;
            cc = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            TakeControl(0);
            UpdateManager.RegisterObserver(this);
        }

        public void TakeControl(ulong clientId)
        {
            if (controlledClientId == clientId) return;
            controlledClientId = clientId;
            hasControl = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void RemoveControl(ulong clientId)
        {
            controlledClientId = 0;
            hasControl = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ObservedUpdate(float deltaTime)
        {
            if (!hasControl) return;

            currentYaw += Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
            currentPitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
            bool isSprinting = false; //Input.GetButton("Sprint");
            currentPitch = Math.Clamp(currentPitch, -80f, 80f);
            transform.rotation = Quaternion.Euler(0.0f, currentYaw, 0.0f);
            cam.transform.localRotation = Quaternion.Euler(currentPitch, 0.0f, 0.0f);

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 forward = cam.transform.forward.IgnoreAxis(EAxis.Y).normalized;
            Vector3 right = cam.transform.right.IgnoreAxis(EAxis.Y).normalized;

            Vector3 movementVector = horizontalInput * right + verticalInput * forward;
            movementVector.Normalize();

            cc.Move(movementVector * (deltaTime * movementSpeed * (isSprinting ? sprintMultiplier : 1.0f)));
            cc.Move(Vector3.up * (currentYVelocity * deltaTime));

            if (cc.isGrounded)
                currentYVelocity = 0.0f;
            else
                currentYVelocity += Physics.gravity.y * deltaTime;

            lastForwardDirection = cam.transform.forward;

            if (Input.GetButtonDown("Jump") && currentYVelocity <= 0.0f && currentYVelocity > -1.0f)
                currentYVelocity = jumpStrength;
        }
    }
}