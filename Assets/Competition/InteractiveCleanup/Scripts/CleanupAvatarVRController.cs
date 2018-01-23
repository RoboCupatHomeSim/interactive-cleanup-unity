using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

		public GameObject ovrCameraRig;
		public Animator   avatarAnimator;

		public List<GameObject> avatarMotionDestinations;

		void Awake()
		{
			ExecutionMode executionMode = (ExecutionMode)Enum.ToObject(typeof(ExecutionMode), CleanupConfig.Instance.configFileInfo.executionMode);

			switch (executionMode)
			{
				// For the competition. Read generated data.
				case ExecutionMode.Competition:
				{
					this.ovrCameraRig.SetActive(false);
					this.avatarAnimator.enabled = false;

					CleanupAvatarVRHandController[] cleanupAvatarVRHandControllers = this.GetComponentsInChildren<CleanupAvatarVRHandController>();

					foreach(CleanupAvatarVRHandController cleanupAvatarVRHandController in cleanupAvatarVRHandControllers)
					{
						cleanupAvatarVRHandController.enabled = false;
					}

					this.enabled = false;

					break;
				}
				// For data generation. 
				case ExecutionMode.DataGeneration:
				{
					this.ovrCameraRig.SetActive(true);
					this.avatarAnimator.enabled = true;

					break;
				}
				default:
				{
					throw new Exception("Illegal Execution mode. mode=" + CleanupConfig.Instance.configFileInfo.executionMode);
				}
			}
		}
		
		// Use this for initialization
		void Start()
		{
		}

#if SIGVERSE_USING_OCULUS_RIFT

		// Update is called once per frame
		void Update()
		{
			// Enable/Disable Laser of Right hand
			if (OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.95)
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

			// Enable/Disable Laser of Left hand
			if (OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) > 0.95)
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


			if (this.laserLeft.gameObject.activeInHierarchy && OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
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


			if (this.laserRight.gameObject.activeInHierarchy && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
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

			if(OVRInput.GetDown(OVRInput.RawButton.A))
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

			if(OVRInput.GetDown(OVRInput.RawButton.X))
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
#endif
	}
}
