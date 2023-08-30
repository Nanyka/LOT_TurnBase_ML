using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class TestTween : MonoBehaviour
    {
        [Header("Camera")] [SerializeField] ShakeSettings cameraShakeSettings;
        [SerializeField] private Camera camera;
        [SerializeField] private float cameraShakeMagnitude;
        [SerializeField] private float cameraShakeDuration;
        [SerializeField] private float cameraShakeFrequency;

        [Header("Twinkle effect")] [SerializeField]
        private Transform[] targetTransform;

        [SerializeField] private TweenSettings<float> uiElementSettings;

        [Header("Open menu effect")] 
        [SerializeField] private RectTransform[] targetIcon;
        [SerializeField] private TweenSettings<float> menuAnimationSettings;

        [Header("Text appear effect")] 
        [SerializeField] private RectTransform targetTextContainer;
        [SerializeField] private TweenSettings<float> textAppearSettings;

        private Tween _currentTween;
        private int _currentTargetIndex = 0;
        private bool _isMenuOpen;

        private void Start()
        {
            camera = Camera.main;
        }

        public void OnClickTweenButton()
        {
            // Shake effect on camera
            // Tween.ShakeLocalPosition(camera.transform, cameraShakeSettings);

            // Twinkle effect
            // _currentTween = Tween.LocalScale(targetTransform[_currentTargetIndex], uiElementSettings);
        }

        public void OnSetWindowOpened()
        {
            _isMenuOpen = !_isMenuOpen;
            var sequence = Sequence.Create();
            foreach (var icon in targetIcon)
                sequence.Chain(Tween.UIAnchoredPositionY(icon, menuAnimationSettings.WithDirection(toEndValue: _isMenuOpen)));
        }

        public void OnTextAppear()
        {
            Tween.LocalScale(targetTextContainer, textAppearSettings.WithDirection(toEndValue: true));
        }

        public void OnChangeTarget()
        {
            _currentTween.Complete();
            _currentTween.Stop();
            _currentTargetIndex = (_currentTargetIndex + 1) % targetTransform.Length;
            // OnClickTweenButton();
            // OnSetWindowOpened();
        }
    }
}