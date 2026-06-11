// Copyright (c) Meta Platforms, Inc. and affiliates.


using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;

namespace OculusSampleFramework
{
    /// <summary>
    /// In case someone wants to know about all interactables in a scene,
    /// this registry is the easiest way to access that information.
    /// </summary>
    [MetaCodeSample("StarterSample.Core-HandsInteraction")]
    public class InteractableRegistry : MonoBehaviour
    {
        public static HashSet<Interactable> _interactables = new HashSet<Interactable>();

        public static HashSet<Interactable> Interactables
        {
            get { return _interactables; }
        }

        public static void RegisterInteractable(Interactable interactable)
        {
            Interactables.Add(interactable);
        }

        public static void UnregisterInteractable(Interactable interactable)
        {
            Interactables.Remove(interactable);
        }
    }
}
