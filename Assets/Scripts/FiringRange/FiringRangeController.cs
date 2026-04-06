using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FiringRange;
using UnityEngine;
using UnityEngine.UI;

public class FiringRangeController : MonoBehaviour
{
	[Header("UI")] [SerializeField] private Button startButton;

	[Header("Range Settings")] [SerializeField]
	private float roundDuration = 60f;

	[SerializeField] private float minActivationDelay = 1;
	[SerializeField] private float maxActivationDelay = 3;
	[SerializeField] private int minDummiesForActivation = 1;
	[SerializeField] private int maxDummiesForActivation = 3;

	[Header("Dummies")] [SerializeField] private List<GameObject> dummies;
	
	private List<IDummy> allDummies = new();
	private int currentScore;
	private CancellationTokenSource cts;

	private void Awake()
	{
		startButton.onClick.AddListener(startRound);
		foreach (var _dummy in dummies)
		{
			if (_dummy.TryGetComponent(out IDummy _dummyObj))
			{
				allDummies.Add(_dummyObj);
				_dummyObj.Initialize(this);
			}
		}
	}

	public void AddScore(int _score)
	{
		currentScore += _score;
		HUDManager.Instance.UpdateScoreText(currentScore);
	}

	private void startRound() => startRoundAsync().Forget();

	private async UniTaskVoid startRoundAsync()
	{
		startButton.gameObject.SetActive(false);
		currentScore = 0;
		HUDManager.Instance.UpdateScoreText(currentScore);

		cts?.Cancel();
		cts = new CancellationTokenSource();
		foreach (var _dummy in allDummies) { _dummy.Deactivate(); }

		await UniTask.WhenAll(runTimer(cts.Token), runActivationLoop(cts.Token));
		startButton.gameObject.SetActive(true);
	}

	private async UniTask runTimer(CancellationToken _token)
	{
		float _remainingTime = roundDuration;
		while (_remainingTime > 0 && !_token.IsCancellationRequested)
		{
			HUDManager.Instance.UpdateTimeText(_remainingTime);
			await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _token);
			_remainingTime -= 0.1f;
		}
		HUDManager.Instance.UpdateTimeText(0);
		cts?.Cancel();
	}

	private async UniTask runActivationLoop(CancellationToken _token)
	{
		while (!_token.IsCancellationRequested)
		{
			float _delay = UnityEngine.Random.Range(minActivationDelay, maxActivationDelay);
			await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: _token);
			if (_token.IsCancellationRequested) { break; }

			int _count = UnityEngine.Random.Range(minDummiesForActivation, maxDummiesForActivation);
			activateRandomDummies(_count);
		}
	}

	private void activateRandomDummies(int _numToActivate)
	{
		List<IDummy> _inactive = new List<IDummy>();
		foreach (var _dummy in allDummies)
		{
			if (!_dummy.IsActive) { _inactive.Add(_dummy); }
		}
		if (_inactive.Count <= 0) { return; }
		
		//quick shuffle
		for (int _i = 0; _i < _inactive.Count; _i++)
		{
			IDummy _temp = _inactive[_i];
			int _rand =  UnityEngine.Random.Range(_i, _inactive.Count);
			_inactive[_i] = _inactive[_rand];
			_inactive[_rand] = _temp;
		}
		
		int _toActivate = Mathf.Min(_numToActivate, _inactive.Count);
		for (int _i = 0; _i < _toActivate; _i++)
		{
			_inactive[_i].Activate();
		}
	}

	private void OnDestroy()
	{
		cts?.Cancel();
		cts?.Dispose();
		cts = null;
	}
}