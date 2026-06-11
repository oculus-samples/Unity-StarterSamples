// Copyright (c) Meta Platforms, Inc. and affiliates.


namespace OculusSampleFramework
{
    /// <summary>
    /// The visual abstraction of an interactable tool.
    /// </summary>
    public interface InteractableToolView
    {
        InteractableTool InteractableTool { get; }
        void SetFocusedInteractable(Interactable interactable);

        bool EnableState { get; set; }

        // Useful if you want to tool to glow in case it interacts with an object.
        bool ToolActivateState { get; set; }
    }
}
