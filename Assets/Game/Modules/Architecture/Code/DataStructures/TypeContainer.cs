using System;
using System.Collections.Generic;

namespace Self.Architecture.DataStructures
{
	public class TypeContainer<TKey, TValue> where TValue : class
	{
		private Dictionary<Type, TValue> _controllers;

		public event Action<TValue> OnAdded;

		public void AddController<XKey, XValue>()
			where XKey : TKey
			where XValue : TValue, new()
		{
			var value = new XValue();
			_controllers.Add(typeof(XKey), value);
			OnAdded?.Invoke(value);
		}

		public void Add<XKey, XValue>(XValue controller)
			where XKey : TKey
			where XValue : TValue
		{
			_controllers.Add(typeof(XKey), controller);
			OnAdded?.Invoke(controller);
		}

		public TValue Get(Type type)
		{
			return _controllers[type];
		}

		public TValue Get<XKey>(Type type)
			where XKey : TKey
		{
			return _controllers[typeof(XKey)];
		}

		public void RemoveController<XKey>()
			where XKey : TKey
		{
			_controllers.Remove(typeof(XKey));
		}
	}
}