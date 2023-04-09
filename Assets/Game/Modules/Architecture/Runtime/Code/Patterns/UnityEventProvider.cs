using System;
using System.Collections;
using GeekyHouse.Architecture.Patterns;
using UnityEngine;

namespace GeekyHouse.Architecture.Events
{
	public class UnityEventProvider : SingletonMonoBehaviour<UnityEventProvider>
	{
		public static event Action OnUpdate
		{
			add => Instance._OnUpdate += value;
			remove => Instance._OnUpdate -= value;
		}

		public static event Action OnSecondTick
		{
			add => Instance._OnSecondTick += value;
			remove => Instance._OnSecondTick -= value;
		}

		public static event Action OnFixedUpdate
		{
			add => Instance._OnFixedUpdate += value;
			remove => Instance._OnFixedUpdate -= value;
		}

		public static event Action OnLateUpdate
		{
			add => Instance._OnLateUpdate += value;
			remove => Instance._OnLateUpdate -= value;
		}

		public static event Action OnNextUpdate
		{
			add => Instance._OnNextUpdate += value;
			remove => Instance._OnNextUpdate -= value;
		}

		public static event Action<bool> OnApplicationFocusChanged
		{
			add => Instance._OnApplicationFocusChanged += value;
			remove => Instance._OnApplicationFocusChanged -= value;
		}

		public static void CoroutineStart(IEnumerator coroutine)
		{
			Instance.StartCoroutine(coroutine);
		}

		public static void CoroutineStop(IEnumerator coroutine)
		{
			Instance.StopCoroutine(coroutine);
		}

		public static void CoroutineStopAll()
		{
			Instance.StopAllCoroutines();
		}

		private event Action _OnUpdate;

		private event Action _OnSecondTick;

		private event Action _OnFixedUpdate;

		private event Action _OnLateUpdate;

		private event Action _OnNextUpdate;

		private event Action<bool> _OnApplicationFocusChanged;

		private float _nextSecond;

		private void Update()
		{
			_OnUpdate?.Invoke();

			Action next = _OnNextUpdate;
			_OnNextUpdate = null;
			next?.Invoke();

			if (_nextSecond < Time.time)
			{
				_nextSecond += 1;
				_OnSecondTick?.Invoke();
			}
		}

		private void FixedUpdate()
		{
			_OnFixedUpdate?.Invoke();
		}

		private void LateUpdate()
		{
			_OnLateUpdate?.Invoke();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			_OnApplicationFocusChanged?.Invoke(hasFocus);
		}
	}
}