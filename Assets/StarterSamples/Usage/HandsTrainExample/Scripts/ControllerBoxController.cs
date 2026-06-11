// Copyright (c) Meta Platforms, Inc. and affiliates.


using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    [MetaCodeSample("StarterSample-HandsTrain")]
    public class ControllerBoxController : MonoBehaviour
    {
        [SerializeField] private TrainLocomotive _locomotive = null;
        [SerializeField] private CowController _cowController = null;

        private void Awake()
        {
            Assert.IsNotNull(_locomotive);
            Assert.IsNotNull(_cowController);
        }

        public void StartStopStateChanged(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                _locomotive.StartStopStateChanged();
            }
        }

        public void DecreaseSpeedStateChanged(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                _locomotive.DecreaseSpeedStateChanged();
            }
        }

        public void IncreaseSpeedStateChanged(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                _locomotive.IncreaseSpeedStateChanged();
            }
        }

        public void SmokeButtonStateChanged(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                _locomotive.SmokeButtonStateChanged();
            }
        }

        public void WhistleButtonStateChanged(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                _locomotive.WhistleButtonStateChanged();
            }
        }

        public void ReverseButtonStateChanged(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                _locomotive.ReverseButtonStateChanged();
            }
        }

        public void SwitchVisualization(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                HandsManager.Instance.SwitchVisualization();
            }
        }

        public void GoMoo(InteractableStateArgs obj)
        {
            if (obj.NewInteractableState == InteractableState.ActionState)
            {
                _cowController.GoMooCowGo();
            }
        }
    }
}
