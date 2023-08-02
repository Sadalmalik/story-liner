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
	// just a container with links
	public class ReplicaWidget : MonoBehaviour
	{
		// new workflow

		public TMP_Text textContainer;

		[Space] public RectTransform leftContainer;
		public RectTransform rightContainer;
		public RectTransform characterRoot;
		public Image characterImage;

        protected Tween _tween;



        public void InitText(string text, float duration, TweenCallback callback)
        {
            _tween?.Kill();
            _tween = DOTween.Sequence()
                .Join(textContainer.DOText(text, duration))
                .InsertCallback(duration, callback);
            _tween.Play();
        }

        public void InitCharacter(Character character)
        {
            characterImage.sprite = character.characterIcon;

            characterRoot.gameObject.SetActive(characterImage.sprite != null);
            characterRoot.FitInside(character.isMainCharacter ? leftContainer : rightContainer);
        }
    }
}
