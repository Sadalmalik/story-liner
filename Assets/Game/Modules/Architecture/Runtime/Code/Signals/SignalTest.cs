using UnityEditor;
using UnityEngine;

namespace Self.Architecture.Signals
{
	class SignalA
	{
		
	}

	class SignalB
	{
		
	}

	class SignalC
	{
		
	}

	public static class SignalTest
	{
		// For test in Unity
		// [MenuItem("[TEST]/Signals")]
		public static void TestSignals()
		{
			SignalBus.Global.Subscribe<SignalA>(HangleSignlalA);
			SignalBus.Global.Subscribe<SignalB>(HangleSignlalB);
			
			SignalBus.Global.Invoke(new SignalA());
			SignalBus.Global.Invoke(new SignalB());
			SignalBus.Global.Invoke(new SignalA());
			SignalBus.Global.Invoke(new SignalA());
			SignalBus.Global.Invoke(new SignalB());
			SignalBus.Global.Invoke(new SignalC());
			
			SignalBus.Global.Unsubscribe<SignalA>(HangleSignlalA);
			SignalBus.Global.Unsubscribe<SignalB>(HangleSignlalB);
			
			SignalBus.Global.Invoke(new SignalA());
			SignalBus.Global.Invoke(new SignalB());
		}
		
		private static void HangleSignlalA<SignalA>(SignalA signal)
		{
			Debug.Log("Hangle Signlal A");
		}
		
		private static void HangleSignlalB<SignalB>(SignalB signal)
		{
			Debug.Log("Hangle Signlal B");
		}
	}
}
