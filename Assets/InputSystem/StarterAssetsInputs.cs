using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{

		public static StarterAssetsInputs Instance;

        private void Awake() {
            Instance = this;
        }

        [Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		
		[Header("Game Systems Input Values")]
		public bool pauseGame;
		public bool inventory;
		public bool interact;
		public bool examine;
		public bool changeItem;
		public bool backButton;
		public bool turnTorch;

		[Header("Fight Settings")]
		public bool aimingGun;
		public bool shootGun = false;
		public bool reload = false;
		public bool scream = false;

		[Header("Movement Settings")]
		public bool analogMovement;
		public bool rightStickUp;
		public bool rightStickDown;


#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
        }

		public void OnExamine(InputValue value)
		{
			ExamineInput(value.isPressed);
        }

		public void OnPause(InputValue value) {
			PauseInput(value.isPressed);
        }

		public void OnInventory(InputValue value) {
			InventoryInput(value.isPressed);
		}

		public void OnAiming(InputValue value) {
			AimingGunInput(value.isPressed);
        }

		public void OnShoot(InputValue value) {
			ShootGunInput(value.isPressed);
		}

		public void OnBackButton(InputValue value) {
			BackButtonInput(value.isPressed);
		}

		public void OnTurnTorch(InputValue value) {
			TurnTorchInput(value.isPressed);
		}

		public void OnRighStickUp(InputValue value) {
			RightStickUp(value.isPressed);
		}

		public void OnRighStickDown(InputValue value) {
			RightStickDown(value.isPressed);
		}

		public void OnReload(InputValue value) {
			ReloadInput(value.isPressed);
		}

		public void OnScream(InputValue value) {
			ScreamInput(value.isPressed);
		}

		public void OnChangeItem(InputValue value) {
			ChangeItemInput(value.isPressed);
		}

	
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

		public void ExamineInput(bool newExamineState) 
		{
			examine = newExamineState;
		}

		public void PauseInput(bool newPauseState) {
			pauseGame = newPauseState;
        }

		public void InventoryInput(bool newInventoryState) {
			inventory = newInventoryState;
		}

		public void AimingGunInput(bool newAimingGunState) {
			aimingGun = newAimingGunState;
		}

		public void ShootGunInput(bool newShootGunState) {
			shootGun = newShootGunState;
		}

		public void BackButtonInput(bool newBackButtonState) {
			backButton = newBackButtonState;
		}

		public void TurnTorchInput(bool newTurnTorchState) {
			turnTorch = newTurnTorchState;
		}

		public void RightStickUp(bool newRightStickUpState) {
			rightStickUp = newRightStickUpState;
		}

		public void RightStickDown(bool newRightStickDownState) {
			rightStickDown = newRightStickDownState;
		}

		public void ReloadInput(bool newReloadState) {
			reload = newReloadState;
		}
		
		public void ScreamInput(bool newScreamState) {
			scream = newScreamState;
        }
		
		public void ChangeItemInput(bool newExamineState) {
			changeItem = newExamineState;
		}



        public void Reset() 
		{
			pauseGame = false;
			inventory = false;
			interact = false;
			examine = false;
			changeItem = false;
			aimingGun =  false;
			shootGun = false;
			backButton = false;
			turnTorch = false;
			jump = false;
			rightStickDown = false;
			rightStickUp = false;
			scream = false;
			reload = false;
		}


#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}