// Copyright (c) Meta Platforms, Inc. and affiliates.


using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Visual of finger tip poke tool.
/// </summary>
namespace OculusSampleFramework
{
    [MetaCodeSample("StarterSample.Core-HandsInteraction")]
    public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
    {
        [SerializeField] private MeshRenderer _sphereMeshRenderer = null;

        public InteractableTool InteractableTool { get; set; }

        public bool EnableState
        {
            get { return _sphereMeshRenderer.enabled; }
            set { _sphereMeshRenderer.enabled = value; }
        }

        public bool ToolActivateState { get; set; }

        public float SphereRadius { get; private set; }

        private void Awake()
        {
            Assert.IsNotNull(_sphereMeshRenderer);
            SphereRadius = _sphereMeshRenderer.transform.localScale.z * 0.5f;
        }

        public void SetFocusedInteractable(Interactable interactable)
        {
            // nothing to see here
        }
    }
}
