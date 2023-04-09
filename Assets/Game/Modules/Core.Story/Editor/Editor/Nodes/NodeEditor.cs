using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
	[CustomEditor(typeof(BaseNode), true)]
	public class NodeEditor : Editor
	{
		private const string NODE_GUI_ROOT_NAME     = "node-editor-container";

		protected SerializedProperty m_NextNodesProperty;

		protected VisualElement m_Root;
		protected VisualElement m_NodeGuiContainer;
		protected NodeView      m_NodeView;


		protected virtual void OnEnable()
		{
			m_NextNodesProperty   = serializedObject.FindProperty(nameof(BaseNode.nextNodes));
		}

		public override void OnInspectorGUI()
		{
			GUI.enabled = false;

			base.OnInspectorGUI();

			GUI.enabled = true;
		}

		protected virtual void CreateNodeGUI(VisualElement root)
		{
			root.Add(
				new Label($"Implement NodeEditor.CreateNodeGUI for {serializedObject.targetObject.GetType().Name}"));
		}

		// basically use this 
		// to display node actions
		public void CreateInspectorGUI(VisualElement root, NodeView nodeView)
		{
			m_Root     = root;
			m_NodeView = nodeView;

			m_NodeGuiContainer = m_Root.Q(NODE_GUI_ROOT_NAME);

			CreateNodeGUI(m_NodeGuiContainer);
		}

		protected VisualElement CreateContainerFromTemplate(string templatePath, string mainContainerName)
		{
			var template           = Resources.Load<VisualTreeAsset>(templatePath);
			var temporaryContainer = new VisualElement();

			template.CloneTree(temporaryContainer);

			return temporaryContainer.Q(mainContainerName);
		}

		protected void TryAddStyleSheet(string styleSheetPath)
		{
			var nodeStyleSheet = Resources.Load<StyleSheet>(styleSheetPath);

			if (!m_NodeGuiContainer.styleSheets.Contains(nodeStyleSheet))
				m_NodeGuiContainer.styleSheets.Add(nodeStyleSheet);
		}
	}
}