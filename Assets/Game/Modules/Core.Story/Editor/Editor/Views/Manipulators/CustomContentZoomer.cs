using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    public class CustomContentZoomer : ContentZoomer
    {
        private const float c_MacZoomMultiplier = 0.15f;



        public CustomContentZoomer()
        {
            if(Application.platform == RuntimePlatform.OSXEditor)
                scaleStep = DefaultScaleStep * c_MacZoomMultiplier;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            GraphView graphView = base.target as GraphView;
            if (graphView == null)
            {
                throw new InvalidOperationException("Manipulator can only be added to a GraphView");
            }

            if(Application.platform == RuntimePlatform.OSXEditor)
                base.target.RegisterCallback<WheelEvent>(OnPinch);
            else if (Application.platform == RuntimePlatform.WindowsEditor)
                base.target.RegisterCallback<WheelEvent>(OnWheel);
        }

        /// <summary>
        ///   <para>Called to unregister event callbacks from the target element.</para>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
                base.target.UnregisterCallback<WheelEvent>(OnPinch);
            else if (Application.platform == RuntimePlatform.WindowsEditor)
                base.target.UnregisterCallback<WheelEvent>(OnWheel);
        }

        private void OnPinch(WheelEvent evt)
        {
            if (!evt.altKey)
                return;

            GraphView graphView = base.target as GraphView;

            if (graphView == null)
                return;

            IPanel panel = (evt.target as VisualElement)?.panel;

            if (panel.GetCapturingElement(PointerId.mousePointerId) == null)
            {
                Vector3 position = graphView.viewTransform.position;
                Vector3 scale = graphView.viewTransform.scale;
                Vector2 vector = base.target.ChangeCoordinatesTo(graphView.contentViewContainer, evt.localMousePosition);
                float x = vector.x + graphView.contentViewContainer.layout.x;
                float y = vector.y + graphView.contentViewContainer.layout.y;
                position += Vector3.Scale(new Vector3(x, y, 0f), scale);
                scale.y = (scale.x = CalculateNewZoom(scale.y, 0f - evt.mouseDelta.y, scaleStep, referenceScale, minScale, maxScale));
                scale.z = 1f;
                position -= Vector3.Scale(new Vector3(x, y, 0f), scale);
                graphView.UpdateViewTransform(position, scale);
                evt.StopPropagation();
            }
        }

        private void OnWheel(WheelEvent evt)
        {
            GraphView graphView = base.target as GraphView;
            if (graphView != null)
            {
                IPanel panel = (evt.target as VisualElement)?.panel;
                if (panel.GetCapturingElement(PointerId.mousePointerId) == null)
                {
                    Vector3 position = graphView.viewTransform.position;
                    Vector3 scale = graphView.viewTransform.scale;
                    Vector2 vector = base.target.ChangeCoordinatesTo(graphView.contentViewContainer, evt.localMousePosition);
                    float x = vector.x + graphView.contentViewContainer.layout.x;
                    float y = vector.y + graphView.contentViewContainer.layout.y;
                    position += Vector3.Scale(new Vector3(x, y, 0f), scale);
                    scale.y = (scale.x = CalculateNewZoom(scale.y, 0f - evt.delta.y, scaleStep, referenceScale, minScale, maxScale));
                    scale.z = 1f;
                    position -= Vector3.Scale(new Vector3(x, y, 0f), scale);
                    graphView.UpdateViewTransform(position, scale);
                    evt.StopPropagation();
                }
            }
        }

        private static float CalculateNewZoom(float currentZoom, float wheelDelta, float zoomStep, float referenceZoom, float minZoom, float maxZoom)
        {
            if (minZoom <= 0f)
            {
                Debug.LogError($"The minimum zoom ({minZoom}) must be greater than zero.");
                return currentZoom;
            }
            if (referenceZoom < minZoom)
            {
                Debug.LogError($"The reference zoom ({referenceZoom}) must be greater than or equal to the minimum zoom ({minZoom}).");
                return currentZoom;
            }
            if (referenceZoom > maxZoom)
            {
                Debug.LogError($"The reference zoom ({referenceZoom}) must be less than or equal to the maximum zoom ({maxZoom}).");
                return currentZoom;
            }
            if (zoomStep < 0f)
            {
                Debug.LogError($"The zoom step ({zoomStep}) must be greater than or equal to zero.");
                return currentZoom;
            }
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            if (Mathf.Approximately(wheelDelta, 0f))
            {
                return currentZoom;
            }
            double num = Math.Log(referenceZoom, 1f + zoomStep);
            double num2 = (double)referenceZoom - Math.Pow(1f + zoomStep, num);
            double num3 = Math.Log((double)minZoom - num2, 1f + zoomStep) - num;
            double num4 = Math.Log((double)maxZoom - num2, 1f + zoomStep) - num;
            double num5 = Math.Log((double)currentZoom - num2, 1f + zoomStep) - num;
            wheelDelta = Math.Sign(wheelDelta);
            num5 += (double)wheelDelta;
            if (num5 > num4 - 0.5)
            {
                return maxZoom;
            }
            if (num5 < num3 + 0.5)
            {
                return minZoom;
            }
            num5 = Math.Round(num5);
            return (float)(Math.Pow(1f + zoomStep, num5 + num) + num2);
        }
    }
}