using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;

namespace Self.Story.Editors
{
	public class CustomNodeAdapter
	{
		private static List<MethodInfo>            s_TypeAdapters;
		private static Dictionary<int, MethodInfo> s_NodeAdapterDictionary;

		public bool CanAdapt(object a, object b)
		{
			if (a == b)
				return false; // self connections are not permitted

			if (a == null || b == null)
				return false;

			return GetAdapter(a, b) != null;
		}

		public bool Connect(object a, object b)
		{
			MethodInfo mi = GetAdapter(a, b);
			if (mi == null)
			{
				UnityEngine.Debug.LogError("Attempt to connect 2 unadaptable types: " + a.GetType() + " -> " +
				                           b.GetType());
				return false;
			}

			object retVal = mi.Invoke(this, new[] {this, a, b});
			return (bool) retVal;
		}

		IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
		{
			return assembly.GetTypes()
				.Where(t => t.IsSealed && !t.IsGenericType && !t.IsNested)
				.SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				.Where(m => m.IsDefined(typeof(ExtensionAttribute), false) &&
				            m.GetParameters()[0].ParameterType == extendedType);
		}

		public MethodInfo GetAdapter(object a, object b)
		{
			if (a == null || b == null)
				return null;

			if (s_NodeAdapterDictionary == null)
			{
				s_NodeAdapterDictionary = new Dictionary<int, MethodInfo>();

				// add extension methods
				AppDomain currentDomain = AppDomain.CurrentDomain;
				foreach (Assembly assembly in currentDomain.GetAssemblies())
				{
					IEnumerable<MethodInfo> methods;

					try
					{
						methods = GetExtensionMethods(assembly, typeof(CustomNodeAdapter));
					}
					// Invalid DLLs might raise this exception, simply ignore it
					catch (ReflectionTypeLoadException)
					{
						continue;
					}

					foreach (MethodInfo method in methods)
					{
						ParameterInfo[] methodParams = method.GetParameters();
						if (methodParams.Length == 3)
						{
							string pa   = methodParams[1].ParameterType + methodParams[2].ParameterType.ToString();
							int    hash = pa.GetHashCode();
							if (s_NodeAdapterDictionary.ContainsKey(hash))
							{
								UnityEngine.Debug.Log("NodeAdapter: multiple extensions have the same signature:\n" +
								                      "1: " + method + "\n" +
								                      "2: " + s_NodeAdapterDictionary[hash]);
							}
							else
							{
								s_NodeAdapterDictionary.Add(hash, method);
							}
						}
					}
				}
			}

			string s = a.GetType().ToString() + b.GetType();

			MethodInfo methodInfo;
			return s_NodeAdapterDictionary.TryGetValue(s.GetHashCode(), out methodInfo) ? methodInfo : null;
		}

		public MethodInfo GetTypeAdapter(Type from, Type to)
		{
			if (s_TypeAdapters == null)
			{
				s_TypeAdapters = new List<MethodInfo>();
				AppDomain currentDomain = AppDomain.CurrentDomain;
				foreach (Assembly assembly in currentDomain.GetAssemblies())
				{
					try
					{
						foreach (Type temptype in assembly.GetTypes())
						{
							MethodInfo[] methodInfos = temptype.GetMethods(BindingFlags.Public | BindingFlags.Static);
							foreach (MethodInfo i in methodInfos)
							{
								object[] allAttrs = i.GetCustomAttributes(typeof(TypeAdapter), false);
								if (allAttrs.Any())
								{
									s_TypeAdapters.Add(i);
								}
							}
						}
					}
					// Invalid DLLs might raise this exception, simply ignore it
					catch (ReflectionTypeLoadException)
					{
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.Log(ex);
					}
				}
			}


			foreach (MethodInfo i in s_TypeAdapters)
			{
				if (i.ReturnType == to)
				{
					ParameterInfo[] allParams = i.GetParameters();
					if (allParams.Length == 1)
					{
						if (allParams[0].ParameterType == from || from.IsSubclassOf(allParams[0].ParameterType))
							return i;
					}
				}
			}

			return null;
		}
	}
}