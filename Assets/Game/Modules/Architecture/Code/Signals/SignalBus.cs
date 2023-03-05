using System;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Architecture.Signals
{
    internal interface ISignalContainer
    {
    }
    
    internal class SignalContainer<T> : ISignalContainer
    {
        private readonly HashSet<Action<T>> _handlers = new HashSet<Action<T>>();
        
        public void Subscribe(Action<T>act)
        {
            _handlers.Add(act);
        }
        
        public void Unsubscribe(Action<T>act)
        {
            _handlers.Remove(act);
        }
        
        public void Invoke(T signal)
        {
            if (_handlers.Count==0 && SignalBus.ThrowNoHandlersException)
            {
                throw new ArgumentException($"No handlers for signal: {signal}");
            }
        
            foreach (var handler in _handlers)
            {
                handler(signal);
            }
        }
    }
    
    public partial class SignalBus
    {
        private static SignalBus _global;
        
        public static SignalBus Global => _global ?? (_global = new SignalBus());
        
        public static bool ThrowNoHandlersException = true;
        
        private readonly Dictionary<Type, ISignalContainer> _signals = new Dictionary<Type, ISignalContainer>();
        
        public void Subscribe<T>(Action<T>handler)
        {
            var container = GetContainer<T>();
            
            container.Subscribe(handler);
        }
        
        public void Unsubscribe<T>(Action<T>handler)
        {
            var container = GetContainer<T>();
            
            container.Unsubscribe(handler);
        }
        
        public void Invoke<T>(T signal)
        {
            var container = GetContainer<T>();
            
            container.Invoke(signal);
        }
        
        private bool HaveContainer<T>()
        {
            return _signals.ContainsKey(typeof(T));
        }
        
        private SignalContainer<T> GetContainer<T>()
        {
            var type = typeof(T);
            if (!_signals.TryGetValue(type, out var iContainer))
                _signals.Add(type, iContainer = new SignalContainer<T>());
            return iContainer as SignalContainer<T>;
        }
    }
}