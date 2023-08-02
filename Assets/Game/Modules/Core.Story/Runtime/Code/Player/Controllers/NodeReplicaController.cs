using System;
using System.Linq;
using DG.Tweening;
using Self.Architecture.IOC;
using Self.Architecture.Signals;
using Self.Architecture.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Self.Story
{
	public class NodeReplicaController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ReplicaNode);

		[SerializeField] private Button button;
        [SerializeField] private PlayableDirector[] animations;
        [SerializeField] private float textDuration;

        [Inject] private StoryController _StoryController;

		public ReplicaNode   Node { get; private set; }
        // TODO: rework this View to become
        // CharacterView
        // TextView
        public ReplicaWidget View { get; private set; }

        private bool _isPlayingSameNode;
        private bool _isHiding;
        private bool _isShowing;



        public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
            button.onClick.AddListener(HandleClick);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			View = signal.view.replicaWidget;
		}

		public string Enter(BaseNode node, BaseNode previousNode = null)
        {
            Node = node as ReplicaNode;

            View.InitText(Node.localized, textDuration, HandleShowComplete);
            View.InitCharacter(Node.character);

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

        private void HandleClick()
		{
            var nextNode = _StoryController.CurrentChapter.TryGetNode(Node.NextNode);
            var nodeType = nextNode.GetType();

            if(nodeType == GetTargetType())
            {
                // same node playing
                // check if we need to change anything
                // or just clear the text 
                var replicaNode = nextNode as ReplicaNode;

                var needCharacterChange = replicaNode.character.characterName != Node.character.characterName;
                var needEmotionChange = replicaNode.emotion?.emotion != Node.emotion?.emotion;

                if(needCharacterChange || needEmotionChange)
                {
                    PlayAnimation("Anim_Hide");
                }
                else
                {
                    _isPlayingSameNode = true;
                    _StoryController.SetNode(Node.NextNode);
                }
            }
		}

		private void HandleShowComplete()
		{
			
		}
		
		private void HandleHideComplete()
		{
			_StoryController.SetNode(Node.NextNode);
		}
    }
}
