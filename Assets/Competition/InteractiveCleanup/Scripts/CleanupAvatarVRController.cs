using SIGVerse.Common;
using SIGVerse.Human.IK;
using SIGVerse.Human.VR;
using SIGVerse.SIGVerseRosBridge;
using SIGVerse.ToyotaHSR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine.XR.Management;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace SIGVerse.Competition.InteractiveCleanup
{
	public interface IAvatarMotionHandler : IEventSystemHandler
	{
		void OnAvatarPointByLeft();
		void OnAvatarPointByRight();
		void OnAvatarPressA();
		void OnAvatarPressX();
	}

	public class CleanupAvatarVRController : MonoBehaviour
	{
		public Laser laserLeft;
		public Laser laserRight;

		public GameObject cameraRig;
		public GameObject eyeAnchor;

		public Animator   avatarAnimator;

		public List<GameObject> avatarMotionDestinations;

		public GameObject initialPositionMarker;

		public CapsuleCollider rootCapsuleCollider;

		public GameObject rosBridgeScripts;

		//----------------------------------------

		private List<CapsuleCollider> capsuleColliders;

		private SimpleHumanVRController simpleHumanVRController;
		private SimpleIK simpleIK;
		private List<CleanupAvatarVRHandController> cleanupAvatarVRHandControllers;

		private XRLoader activeLoader;


		void Awake()
		{
			this.capsuleColliders      = this.GetComponentsInChildren<CapsuleCollider>().ToList();
			this.capsuleColliders.Remove(this.rootCapsuleCollider);

			this.simpleHumanVRController        = this.GetComponentInChildren<SimpleHumanVRController>();
			this.simpleIK                       = this.GetComponentInChildren<SimpleIK>();
			this.cleanupAvatarVRHandControllers = this.GetComponentsInChildren<CleanupAvatarVRHandController>().ToList();

			ExecutionMode executionMode = (ExecutionMode)Enum.ToObject(typeof(ExecutionMode), CleanupConfig.Instance.configFileInfo.executionMode);

			switch (executionMode)
			{
				// For the competition. Read generated data.
				case ExecutionMode.Competition:
				{
					this.cameraRig.SetActive(false);
					this.avatarAnimator.enabled = false;

					this.rootCapsuleCollider.enabled = false;
					foreach(CapsuleCollider capsuleCollider in this.capsuleColliders){ capsuleCollider.enabled = true; }

					this.simpleHumanVRController.enabled = false;
					this.simpleIK               .enabled = false;
					foreach(CleanupAvatarVRHandController cleanupAvatarVRHandController in this.cleanupAvatarVRHandControllers){ cleanupAvatarVRHandController.enabled = false; }

					this.initialPositionMarker.SetActive(false);

					// Add for SteamVR START
					Rigidbody[] rigidbodies = this.GetComponentsInChildren<Rigidbody>(true);
					foreach(Rigidbody rigidbody in rigidbodies){ rigidbody.useGravity = false; }
					// Add for SteamVR END

					this.enabled = false;

					break;
				}
				// For data generation. 
				case ExecutionMode.DataGeneration:
				{
					StartCoroutine(this.InitializeHumanForDataGeneration());

					break;
				}
				default:
				{
					throw new Exception("Illegal Execution mode. mode=" + CleanupConfig.Instance.configFileInfo.executionMode);
				}
			}
		}

		private IEnumerator InitializeHumanForDataGeneration()
		{
			yield return this.InitializeXRForDataGeneration();

			this.EnableScriptsForDataGeneration();
		}

		private IEnumerator InitializeXRForDataGeneration()
		{
			// Initialize XR System
			XRManagerSettings xrManagerSettings = XRGeneralSettings.Instance.Manager;

			if (xrManagerSettings == null) { SIGVerseLogger.Error("xrManagerSettings == null"); yield break; }

			if(xrManagerSettings.activeLoader == null)
			{
				yield return xrManagerSettings.InitializeLoader();
			}

			this.activeLoader = xrManagerSettings.activeLoader;

			if (this.activeLoader == null)
			{
				Debug.LogError("Initializing XR Failed.");
				yield break;
			}

			xrManagerSettings.activeLoader.Start();

			SteamVR_Actions.sigverse.Activate(SteamVR_Input_Sources.Any);
		}

		private void EnableScriptsForDataGeneration()
		{
			this.cameraRig.SetActive(true);
			this.avatarAnimator.enabled = true;

			this.rootCapsuleCollider.enabled = true;
			foreach(CapsuleCollider capsuleCollider in this.capsuleColliders){ capsuleCollider.enabled = false; }

			this.simpleHumanVRController.enabled = true;
			this.simpleIK               .enabled = true;
			foreach(CleanupAvatarVRHandController cleanupAvatarVRHandController in this.cleanupAvatarVRHandControllers){ cleanupAvatarVRHandController.enabled = true; }

			this.initialPositionMarker.SetActive(true);


			// Add for SteamVR START
			this.GetComponent<Player>().enabled = true;

			this.cameraRig.GetComponent<SIGVerse.Human.IK.AnchorPostureCalculator>().enabled = true;

			SteamVR_Behaviour_Pose[] steamVrBehaviourPoses = this.cameraRig.GetComponentsInChildren<SteamVR_Behaviour_Pose>();
			foreach(SteamVR_Behaviour_Pose steamVrBehaviourPose in steamVrBehaviourPoses){ steamVrBehaviourPose.enabled = true;}

			Hand[] hands = this.cameraRig.GetComponentsInChildren<Hand>(true);
			foreach(Hand hand in hands){ hand.enabled = true; }

			this.eyeAnchor.GetComponent<Camera>().enabled = true;
			this.eyeAnchor.GetComponent<SteamVR_CameraHelper>().enabled = true;
			// Add for SteamVR END
		}


//#if SIGVERSE_USING_OCULUS_RIFT

		// Update is called once per frame
		void Update()
		{
			// Enable/Disable Laser of Left hand
			if (SteamVR_Actions.sigverse_SqueezeMiddle.GetAxis(SteamVR_Input_Sources.LeftHand) > 0.95)
			{
				if(!this.laserLeft.gameObject.activeInHierarchy)
				{
					this.laserLeft.Activate(); 
				}
			}
			else
			{
				if(this.laserLeft.gameObject.activeInHierarchy)
				{
					this.laserLeft.Deactivate();
				}
			}

			// Enable/Disable Laser of Right hand
			if (SteamVR_Actions.sigverse_SqueezeMiddle.GetAxis(SteamVR_Input_Sources.RightHand) > 0.95)
			{
				if(!this.laserRight.gameObject.activeInHierarchy)
				{
					this.laserRight.Activate();
				}
			}
			else
			{
				if(this.laserRight.gameObject.activeInHierarchy)
				{
					this.laserRight.Deactivate();
				}
			}


			if (this.laserLeft.gameObject.activeInHierarchy && SteamVR_Actions.sigverse_PressIndex.GetStateDown(SteamVR_Input_Sources.LeftHand))
			{
				string selectedTargetName = this.laserLeft.Point(true);

				foreach (GameObject avatarMotionDestination in this.avatarMotionDestinations)
				{
					ExecuteEvents.Execute<IAvatarMotionHandler>
					(
						target: avatarMotionDestination,
						eventData: null,
						functor: (reciever, eventData) => reciever.OnAvatarPointByLeft()
					);
				}

				Debug.Log("selectedTargetName=" + selectedTargetName);
			}


			if (this.laserRight.gameObject.activeInHierarchy && SteamVR_Actions.sigverse_PressIndex.GetStateDown(SteamVR_Input_Sources.RightHand))
			{
				string selectedTargetName = this.laserRight.Point(true);

				foreach (GameObject avatarMotionDestination in this.avatarMotionDestinations)
				{
					ExecuteEvents.Execute<IAvatarMotionHandler>
					(
						target: avatarMotionDestination,
						eventData: null,
						functor: (reciever, eventData) => reciever.OnAvatarPointByRight()
					);
				}

				Debug.Log("selectedTargetName=" + selectedTargetName);
			}

			if(SteamVR_Actions.sigverse_PressNearButton.GetStateDown(SteamVR_Input_Sources.RightHand))
			{
				foreach (GameObject avatarMotionDestination in this.avatarMotionDestinations)
				{
					ExecuteEvents.Execute<IAvatarMotionHandler>
					(
						target: avatarMotionDestination,
						eventData: null,
						functor: (reciever, eventData) => reciever.OnAvatarPressA()
					);
				}

				Debug.Log("Pressed A button");
			}

			if(SteamVR_Actions.sigverse_PressNearButton.GetStateDown(SteamVR_Input_Sources.LeftHand))
			{
				foreach (GameObject avatarMotionDestination in this.avatarMotionDestinations)
				{
					ExecuteEvents.Execute<IAvatarMotionHandler>
					(
						target: avatarMotionDestination,
						eventData: null,
						functor: (reciever, eventData) => reciever.OnAvatarPressX()
					);
				}

				Debug.Log("Pressed X button");
			}
		}
		void OnDestroy()
		{
			// It is mandatory to perform this termination process.
			if(this.activeLoader != null)
			{
				this.activeLoader.Stop();
				XRGeneralSettings.Instance.Manager.DeinitializeLoader();
			}
		}
//#endif
	}
}
