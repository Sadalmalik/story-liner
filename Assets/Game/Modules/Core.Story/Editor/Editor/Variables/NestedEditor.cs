using UnityEditor;
using UnityEngine;

namespace Self.Story.Editors
{
	public abstract class NestedEditor : Editor
	{
		private SerializedObject m_ParentObject;

		protected virtual void OnEnable()
		{
			var parentObjectPath = AssetDatabase.GetAssetPath(serializedObject.targetObject);
			var parentObject     = AssetDatabase.LoadAssetAtPath(parentObjectPath, typeof(ScriptableObject));
			m_ParentObject = parentObject != null ? new SerializedObject(parentObject) : null;
		}

		protected void ApplyModifiedProperties()
		{
			serializedObject.ApplyModifiedProperties();
			m_ParentObject?.ApplyModifiedProperties();
		}
	}
}