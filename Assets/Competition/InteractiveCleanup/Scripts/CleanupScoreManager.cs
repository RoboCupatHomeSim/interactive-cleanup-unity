using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using SIGVerse.Common;
using SIGVerse.ToyotaHSR;
using UnityEngine.EventSystems;

namespace SIGVerse.Competition.InteractiveCleanup
{
	public static class Score
	{
		public const int MaxScore = +999;
		public const int MinScore = -999;

		public enum Type
		{
			ObjectGraspedSuccess,
			ObjectGraspedFailure,
			CleanupSuccess,
			CleanupFailure,
			AskedCorrectOrNot,
			PointItAgain,
			HsrCollisionEnter,
			ObjectCollisionEnter,
		}

		public static int GetScore(Type scoreType, params object[] args)
		{
			switch(scoreType)
			{
				case Score.Type.ObjectGraspedSuccess: { return +50; }
				case Score.Type.ObjectGraspedFailure: { return -10; }
				case Score.Type.CleanupSuccess      : { return +50; }
				case Score.Type.CleanupFailure      : { return -10; }
				case Score.Type.AskedCorrectOrNot   : { return -10; }
				case Score.Type.PointItAgain        : { return -10; }
				case Score.Type.HsrCollisionEnter   : { return 0; }
//				case Score.Type.HsrCollisionEnter   : { return GetHsrCollisionScore   ((Collision)args[0], (float)args[1]); }
				case Score.Type.ObjectCollisionEnter: { return 0; }
//				case Score.Type.ObjectCollisionEnter: { return GetObjectCollisionScore((SIGVerse.Competition.CollisionType)args[0], (Collision)args[1], (float)args[2]); }
			}

			throw new Exception("Illegal score type. Type = " + (int)scoreType + ", method name=(" + System.Reflection.MethodBase.GetCurrentMethod().Name + ")");
		}

		public static float GetObjectCollisionVeloticyThreshold()
		{
			return 1.0f;
		}

		private static int GetHsrCollisionScore(Collision collision, float collisionVelocity)
		{
			return Mathf.Clamp(Mathf.FloorToInt(Mathf.Log10(100 * collisionVelocity) * -20), -50, -5);
		}

		private static int GetObjectCollisionScore(SIGVerse.Competition.CollisionType collisionType, Collision collision, float collisionVelocity)
		{
			if(collisionType==CollisionType.WithHsrBase)
			{
				return -5;
			}
			else
			{
				return Mathf.Clamp(Mathf.FloorToInt((collisionVelocity - 1.0f) * -10), -50, -1);
			}
		}

		public static float GetHsrCollisionVeloticyThreshold()
		{
			return 0.0f;
		}
	}

	public class CleanupScoreManager : MonoBehaviour, IRobotCollisionHandler, ITransferredCollisionHandler
	{
		private const float DefaultTimeScale = 1.0f;

		public List<GameObject> scoreNotificationDestinations;

		public List<string> timeIsUpDestinationTags;

		//---------------------------------------------------
		private int timeLimit = 0;

		private GameObject mainMenu;
		private PanelMainController panelMainController;

		private List<GameObject> timeIsUpDestinations;

		private float timeLeft;

		private int score;


		void Awake()
		{
			this.timeLimit = CleanupConfig.Instance.configFileInfo.sessionTimeLimit;

			this.mainMenu = GameObject.FindGameObjectWithTag("MainMenu");

			this.panelMainController = this.mainMenu.GetComponent<PanelMainController>();


			this.timeIsUpDestinations = new List<GameObject>();

			foreach (string timeIsUpDestinationTag in this.timeIsUpDestinationTags)
			{
				GameObject[] timeIsUpDestinationArray = GameObject.FindGameObjectsWithTag(timeIsUpDestinationTag);

				foreach(GameObject timeIsUpDestination in timeIsUpDestinationArray)
				{
					this.timeIsUpDestinations.Add(timeIsUpDestination);
				}
			}
		}

		// Use this for initialization
		void Start()
		{
			this.UpdateScoreText(0, CleanupConfig.Instance.GetTotalScore());

			this.score = 0;

			this.timeLeft = (float)timeLimit;

			this.panelMainController.SetTimeLeft(this.timeLeft);

			Time.timeScale = 0.0f;
		}

		// Update is called once per frame
		void Update()
		{
			this.timeLeft = Mathf.Max(0.0f, this.timeLeft-Time.deltaTime);

			this.panelMainController.SetTimeLeft(this.timeLeft);

			if(this.timeLeft == 0.0f)
			{
				foreach(GameObject timeIsUpDestination in this.timeIsUpDestinations)
				{
					ExecuteEvents.Execute<ITimeIsUpHandler>
					(
						target: timeIsUpDestination,
						eventData: null,
						functor: (reciever, eventData) => reciever.OnTimeIsUp()
					);
				}
			}
		}

		public void AddScore(Score.Type scoreType, params object[] args)
		{
			int additionalScore = Score.GetScore(scoreType, args);

			this.score = Mathf.Clamp(this.score + additionalScore, Score.MinScore, Score.MaxScore);

			this.UpdateScoreText(this.score);

			SIGVerseLogger.Info("Score add [" + additionalScore + "], Challenge " + CleanupConfig.Instance.numberOfTrials + " Score=" + this.score);

			// Send the Score Notification
			ScoreStatus scoreStatus = new ScoreStatus(additionalScore, this.score, CleanupConfig.Instance.GetTotalScore());

			foreach(GameObject scoreNotificationDestination in this.scoreNotificationDestinations)
			{
				ExecuteEvents.Execute<IScoreHandler>
				(
					target: scoreNotificationDestination,
					eventData: null,
					functor: (reciever, eventData) => reciever.OnScoreChange(scoreStatus)
				);
			}
		}

		public void TaskStart()
		{
			this.UpdateScoreText(this.score);

			Time.timeScale = CleanupScoreManager.DefaultTimeScale;
		}

		public void TaskEnd()
		{
			Time.timeScale = 0.0f;

			CleanupConfig.Instance.AddScore(this.score);

			this.UpdateScoreText(this.score, CleanupConfig.Instance.GetTotalScore());

			SIGVerseLogger.Info("Total Score=" + CleanupConfig.Instance.GetTotalScore().ToString());

			CleanupConfig.Instance.RecordScoreInFile();
		}


		public void ResetTimeLeftText()
		{
			this.timeLeft = (float)timeLimit;
			this.panelMainController.SetTimeLeft(this.timeLeft);
		}

		private void UpdateScoreText(float score)
		{
			ExecuteEvents.Execute<IPanelScoreHandler>
			(
				target: this.mainMenu,
				eventData: null,
				functor: (reciever, eventData) => reciever.OnScoreChange(score)
			);
		}

		private void UpdateScoreText(float score, float total)
		{
			ExecuteEvents.Execute<IPanelScoreHandler>
			(
				target: this.mainMenu,
				eventData: null,
				functor: (reciever, eventData) => reciever.OnScoreChange(score, total)
			);
		}


		public void OnRobotCollisionEnter(Collision collision, float collisionVelocity, float effectScale)
		{
			this.AddScore(Score.Type.HsrCollisionEnter, collision, collisionVelocity);
		}

		public void OnTransferredCollisionEnter(SIGVerse.Competition.CollisionType collisionType, Collision collision, float collisionVelocity, float effectScale)
		{
			this.AddScore(Score.Type.ObjectCollisionEnter, collisionType, collision, collisionVelocity);
		}
	}
}
