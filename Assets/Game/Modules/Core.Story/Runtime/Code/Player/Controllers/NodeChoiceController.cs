using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;
using static Self.Story.ChoiceNode;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace Self.Story
{
	public class NodeChoiceController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ChoiceNode);

		[SerializeField] private Button button;
		[Inject] private StoryController _StoryController;
        [SerializeField] private PlayableDirector[] animations;
        [SerializeField] private float textDuration;

        public ChoiceNode    Node { get; private set; }
        // TODO: rework this View to become
        // ChoicesView
        // TextView
        // CharacterView
		public ChoiceWidget View { get; private set; }

		private int _selectedChoice;
        private bool _isPlayingSameNode;
        private bool _isHiding;
        private bool _isShowing;
        private Tween _tween;



        public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			View = signal.view.choiceWidget;


            for (int i = 0; i < View.choiceContainers.Count; i++)
            {
                var index = i;

                View.choiceContainers[i].button.onClick.AddListener(() => HandleSelect(index));
            }
		}

        public string Enter(BaseNode node, BaseNode previousNode = null)
        {
			Node = node as ChoiceNode;
            View.InitText(Node.localized, textDuration, HandleShowComplete);
            View.InitCharacter(Node.character);
            View.InitChoices(Node.choices);

            var changeAnimation = GetChangeAnimation(node, previousNode);

            PlayAnimation(changeAnimation);

            return null;
        }

        private string GetChangeAnimation(BaseNode node, BaseNode previousNode)
        {
            var prevName = previousNode == null ? string.Empty : previousNode.GetType().Name;
            var nextName = node.GetType().Name;

            return $"Anim_{prevName}To{nextName}";
        }

        private PlayableDirector PlayAnimation(string name)
        {
            var animToPlay = animations.FirstOrDefault(a => a.name == name);

            if (animToPlay != null)
            {
                animToPlay.Play();
                return animToPlay;
            }
            else
            {
                throw new Exception($"[{GetType().Name}.{nameof(PlayAnimation)}] Could not find animation: '{name}'");
            }
        }

        private void StopAnimation(string name)
        {
            var animToPlay = animations.FirstOrDefault(a => a.name == name);

            if (animToPlay != null)
            {
                animToPlay.Stop();
            }
            else
            {
                throw new Exception($"[{GetType().Name}.{nameof(StopAnimation)}] Could not find animation: '{name}'");
            }
        }

        private void HandleSelect(int index)
		{
			_selectedChoice = index;
			View.Hide();
		}

		private void HandleShowComplete()
		{
			
		}
		
		private void HandleHideComplete()
		{
			_StoryController.SetNode(Node.nextNodes[_selectedChoice]);
		}

        public void Show(ChoiceNode node)
        {
            base.Show(node as ReplicaNode);

        }

        protected override void HandleCompleteHide(PlayableDirector hideAnim)
        {
            foreach (var choice in choices)
                choice.Hide();

            base.HandleCompleteHide(hideAnim);
        }
    }
}