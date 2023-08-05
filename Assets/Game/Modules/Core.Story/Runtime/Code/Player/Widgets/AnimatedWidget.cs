using System.Linq;
using UnityEngine;

namespace Self.Story
{
	public class AnimatedWidget : MonoBehaviour
	{
		[SerializeField] protected Animator viewAnimator;

		string[] nodeStates;



		public virtual void Show(string state = null)
		{
			if(!string.IsNullOrEmpty(state))
			{
				foreach (var nodeState in nodeStates)
				{
					if(nodeState != state)
					{
						viewAnimator.SetBool(nodeState, false);
					}
				}

				viewAnimator.SetBool(state, true);
			}

			viewAnimator?.SetBool("IsVisible", true);
		}

		public virtual void Hide()
		{
			var parameters = viewAnimator.parameters;

			foreach (var p in parameters.Where(par => par.type == AnimatorControllerParameterType.Bool))
			{
				viewAnimator.SetBool(p.name, false);
			}
		}

		private void OnEnable()
		{
			nodeStates = new string[]
			{
				typeof(ReplicaNode).Name,
				typeof(ChoiceNode).Name
			};
		}
	}
}
