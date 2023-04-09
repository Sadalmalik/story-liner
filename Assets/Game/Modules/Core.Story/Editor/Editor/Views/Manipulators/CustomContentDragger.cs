using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    public class CustomContentDragger : ContentDragger
    {
        public const float c_MacPanSpeed = 20f;

        public CustomContentDragger() { }

        protected override void RegisterCallbacksOnTarget()
        {
            GraphView graphView = base.target as GraphView;
            if (graphView == null)
            {
                throw new InvalidOperationException("Manipulator can only be added to a GraphView");
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
                base.target.RegisterCallback<WheelEvent>(OnWheel);
            else if (Application.platform == RuntimePlatform.WindowsEditor)
                base.RegisterCallbacksOnTarget();
        }

        /// <summary>
        ///   <para>Called to unregister event callbacks from the target element.</para>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
                base.target.UnregisterCallback<WheelEvent>(OnWheel);
            else if (Application.platform == RuntimePlatform.WindowsEditor)
                base.RegisterCallbacksOnTarget();

            base.UnregisterCallbacksFromTarget();
        }

        private void OnWheel(WheelEvent evt)
        {
            if (evt.altKey)
                return;

            GraphView graphView = base.target as GraphView;

            if (graphView == null)
                return;

            Vector2 vector = -evt.mouseDelta * c_MacPanSpeed;
            Vector3 scale = graphView.contentViewContainer.transform.scale;

            graphView.viewTransform.position += Vector3.Scale(vector, scale);

            evt.StopPropagation();
        }
    }
}