using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FiringRange
{
	public class BaseDummy : MonoBehaviour, IDummy
	{
		[Header("Dummy Settings")] [SerializeField]
		protected float activeDuration = 3;

		[SerializeField] protected int scoreValue = 10;

		public bool IsActive { get; protected set; }

		protected FiringRangeController firingRangeController;

		private CancellationTokenSource cts;

		public virtual void Initialize(FiringRangeController _controller)
		{
			firingRangeController = _controller;
			Deactivate();
		}

		public virtual void Activate()
		{
			if (IsActive) { return; }
			IsActive = true;
			gameObject.SetActive(true);
			OnActivated();
			cts = new CancellationTokenSource();
			autoDeactivate(cts.Token).Forget();
		}

		public virtual void Deactivate()
		{
			cts?.Cancel();
			IsActive = false;
			gameObject.SetActive(false);
			OnDeactivated();
		}

		public virtual void OnHit()
		{
			firingRangeController.AddScore(scoreValue);
			Deactivate();
		}

		private async UniTaskVoid autoDeactivate(CancellationToken _token)
		{
			try { await UniTask.Delay(TimeSpan.FromSeconds(activeDuration), cancellationToken: _token); }
			catch (OperationCanceledException) { }
		}
		
		//hooks for moving dummy
		protected virtual void OnActivated() { }
		protected virtual void OnDeactivated() { }

		private void OnDestroy()
		{
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
		}
	}
}