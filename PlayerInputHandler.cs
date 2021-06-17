using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DexCode
{
    public class PlayerInputHandler : MonoBehaviour
    {

        public Vector2 CurrentMousePostion { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 NormalizedMoveInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool DuckInput { get; private set; }
        public bool InteractInput { get; private set; }
        public bool RunInput { get; private set; }


        private Vector3 mouseP;
        private int NormInputX;
        private int NormInputY;

        public void OnRunInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                RunInput = true;
            }
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();

            NormInputX = (int)(MovementInput * Vector2.right).normalized.x;
            NormInputY = (int)(MovementInput * Vector2.up).normalized.y;

            NormalizedMoveInput = new Vector2(NormInputX, NormInputY);
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                JumpInput = true;
            }
        }

        public void OnInteractInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                InteractInput = true;
            }
        }
        public void OnDuckInput(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                DuckInput = true;
            }
        }
        public void OnInventoryInput(InputAction.CallbackContext context)
        {

        }

        public void OnNextDialogueInput(InputAction.CallbackContext context)
        {

        }

        // TODO: Find better way to track mousePostion
        void MousePostion()
        {
            mouseP = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            CurrentMousePostion = new Vector2(mouseP.x, mouseP.y);
        }

        private void Update()
        {
            MousePostion();
        }
        //Resets
        public void UseDuckInput() => DuckInput = false;
        public void UseInteractInput() => InteractInput = false;
        public void UseJumpInput() => JumpInput = false;
        public void UseRunInput() => RunInput = false;
    }
}
