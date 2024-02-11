using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
   public sealed class Timer : MonoBehaviour
    {
        public event Action<int, float> OnTimerStarted;
        public event Action<int, float> OnTimerChanged;
        public event Action OnTimerCompleted;

        [SerializeField]
        private float _time = 3;

        [SerializeField]
        private int _timeStepCount = 3;

        private float _timeStep => _time / _timeStepCount;

        public void StartTimer()
        {
            StopAllCoroutines();
            StartCoroutine(nameof(TimerCoroutine));
        }

        public void StartTimer(float time, int timestepCount)
        {
            _time = time;
            _timeStepCount = timestepCount;
            StartTimer();
        }

        private IEnumerator TimerCoroutine()
        {
            OnTimerStarted?.Invoke(_timeStepCount, _timeStep);
            for (int i = _timeStepCount; i > 0; i--)
            {
                yield return new WaitForSeconds(_timeStep);
                OnTimerChanged?.Invoke(i - 1, _timeStep);
            }

            OnTimerCompleted?.Invoke();
        }

    }
}
