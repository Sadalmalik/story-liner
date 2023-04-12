using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    [InspectedType(typeof(ConditionNode))]
    [CustomEditor(typeof(ConditionNode))]
    public class ConditionNodeEditor : NodeEditor
    {
		private const string CONDITIONVIEW_TEMPLATE_PATH	= "Styles/NodeEditorStyles/ConditionNodeView";
		private const string CONDITIONVIEW_STYLE_PATH		= "Styles/NodeEditorStyles/ConditionNodeStyle";

		private VisualElement m_ConditionContainer;
		private VisualElement m_PortsContainer;



		protected override void CreateNodeGUI(VisualElement root)
        {
			m_ConditionContainer = CreateContainerFromTemplate(CONDITIONVIEW_TEMPLATE_PATH, "condition-container");
			m_PortsContainer = m_ConditionContainer.Q<VisualElement>("ports-container");

			var m_textField = m_ConditionContainer.Q<TextField>("condition-text");
			var condNode = m_NodeView.Node as ConditionNode;
			m_textField.SetValueWithoutNotify(condNode.rawCondition);
			
			root.Add(m_ConditionContainer);
			TryAddStyleSheet(CONDITIONVIEW_STYLE_PATH);

			CreateChoicesContainers();
		}

		private void CreateChoicesContainers()
		{
			m_NodeView.OutputPorts.Clear();
			m_NodeView.outputContainer.Clear();

			for (int i = 0; i < 2; i++)
			{
				AddPort(i);
			}
		}

		private void AddPort(int atIndex)
		{
			var newPort = m_NodeView.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);

			newPort.portColor = atIndex == 0 ? Color.green : Color.red;
			newPort.AddToClassList("right-port");

			m_NodeView.OutputPorts.Add(newPort);

			m_PortsContainer.Add(newPort);
		}
	}
}