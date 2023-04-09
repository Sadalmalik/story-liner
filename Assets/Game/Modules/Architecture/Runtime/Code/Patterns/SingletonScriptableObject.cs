using UnityEngine;

namespace GeekyHouse.Architecture.Patterns
{
	public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<T>(typeof(T).Name);
					_instance.Initialize();
				}
				return _instance;
			}
		}
        
		protected virtual void Initialize()
		{
			
		}
	}
}