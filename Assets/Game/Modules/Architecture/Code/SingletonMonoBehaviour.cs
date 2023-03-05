using UnityEngine;

namespace GeekyHouse.Architecture.Patterns
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject o = new GameObject("UnityEventProvider");
					_instance = o.AddComponent<T>();
					DontDestroyOnLoad(o);
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