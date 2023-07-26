using System;
using System.Linq;
using DG.Tweening;
using Kaleb.TweenAnimator;
using Self.Architecture.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Self.Story
{
    public class ReplicaWidget : MonoBehaviour
    {
        // new workflow
        [SerializeField] PlayableDirector[] animations;

        [Space] public Button button;
        public TMP_Text text;
        public float textDuration;

        [Space] public RectTransform leftContainer;
        public RectTransform rightContainer;
        public RectTransform characterRoot;
        public Image characterImage;

        public event Action OnShowComplete;
        public event Action OnHideComplete;
        public event Action OnClick;

        private ReplicaNode _replica;
        private Tween _tween;
        private bool _isHiding;
        private bool _isShowing;



        public virtual void Show(ReplicaNode node)
        {
            _replica = node;
            _isShowing = true;
            SetupCharacter();
            text.SetText(string.Empty);
            _tween = DOTween.Sequence()
                .Join(text.DOText(_replica.localized, textDuration))
                .InsertCallback(textDuration, HandleCompleteShow);
            _tween.Play();
            PlayAnimation("Anim_Show");

            if (button != null)
                button.onClick.AddListener(HandleClick);
        }

        private void SetupCharacter()
        {
            var character = _replica.character;
            characterImage.sprite = character.characterIcon;

            characterRoot.gameObject.SetActive(characterImage.sprite != null);
            characterRoot.FitInside(character.isMainCharacter ? leftContainer : rightContainer);
        }

        public virtual void Hide()
        {
            _isHiding = true;
            var hideAnim = PlayAnimation("Anim_Hide");

            hideAnim.stopped += HandleCompleteHide;

            if (button != null)
                button.onClick.RemoveListener(HandleClick);
        }

        protected virtual void HandleCompleteShow()
        {
            _isShowing = false;

            _tween = null;
            text.SetText(_replica.localized);
            OnShowComplete?.Invoke();
        }

        protected virtual void HandleCompleteHide(PlayableDirector hideAnim)
        {
            _isHiding = false;

            hideAnim.stopped -= HandleCompleteHide;
            OnHideComplete?.Invoke();
        }

        private void HandleClick()
        {
            if (_isHiding)
            {
                return;
            }

            if (_isShowing)
            {
                _tween?.Kill();

                foreach (var anim in animations)
                {
                    anim.stopped -= HandleCompleteHide;
                }

                StopAnimation("Anim_Show");
                PlayAnimation("Anim_ShowInstant");

                HandleCompleteShow();

                return;
            }

            OnClick?.Invoke();
        }

        protected PlayableDirector PlayAnimation(string name)
        {
            var animToPlay = animations.FirstOrDefault(a => a.name == name);

            if(animToPlay != null)
            {
                animToPlay.Play();
                return animToPlay;
            }
            else
            {
                throw new Exception($"[{GetType().Name}.{nameof(PlayAnimation)}] Could not find animation: '{name}'");
            }
        }

        protected void StopAnimation(string name)
        {
            var animToPlay = animations.FirstOrDefault(a => a.name == name);

            if (animToPlay != null)
            {
                animToPlay.Stop();
            }
            else
            {
                throw new Exception($"[{GetType().Name}.{nameof(PlayAnimation)}] Could not find animation: '{name}'");
            }
        }
    }
}
