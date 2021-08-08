using UnityEngine;
using SIGVerse.ToyotaHSR;
using SIGVerse.Common;
using System.Linq;
using System;

namespace SIGVerse.Competition.InteractiveCleanup
{
	public class CleanupHsrInit : MonoBehaviour
	{
		public GameObject rosBridgeScripts;

		private void Awake()
		{
			ExecutionMode executionMode = (ExecutionMode)Enum.ToObject(typeof(ExecutionMode), CleanupConfig.Instance.configFileInfo.executionMode);

			if (executionMode==ExecutionMode.DataGeneration && CleanupConfig.Instance.configFileInfo.reduceLoadInDataGen)
			{
				this.ReduceLoad();
			}
		}

		private void ReduceLoad()
		{
			this.rosBridgeScripts.GetComponentInChildren<HSRPubXtionDepthController>().sendingInterval *= 1000;
			this.rosBridgeScripts.GetComponentInChildren<HSRPubXtionRGBController>()  .sendingInterval *= 1000;
			this.rosBridgeScripts.GetComponentInChildren<HSRPubStereoRGBController>() .sendingInterval *= 1000;
			this.rosBridgeScripts.GetComponentsInChildren<HSRPubWideRGBController>().ToList().ForEach(x => x.sendingInterval *= 1000);
		}
	}
}

