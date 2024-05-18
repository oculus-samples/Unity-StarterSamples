/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CameraSnapBehaviour : MonoBehaviour
{
    private readonly float angleSnapThreshold = 20;

    [SerializeField] private GameObject cameraToSnapTo;

    public float distanceToCamera = 1.2f;
    public float snapDuration = .6f;
    public float fadeInDuration = .5f;

    private CanvasGroup canvas;
    private float canvasTargetAlpha;
    private bool snapStarted;
    private bool snapDistanceStarted;
    private readonly float snapDistanceStartThreshold = .01f;

    private Vector3 originalScale;
    private Vector3 currentVelocity;
    private Vector3 origCameraPosition;
    private Vector3 origCameraForward;

    private void Awake()
    {
        canvas = gameObject.GetComponent<CanvasGroup>();
        canvasTargetAlpha = canvas.alpha;

        // hide the canvas for a few initial frames
        canvas.alpha = 0;
    }

    private void Update()
    {
        Transform objectTransform = transform;
        Transform cameraTransform = cameraToSnapTo.transform;
        Vector3 objectPosition = objectTransform.position;
        Vector3 cameraPosition = cameraToSnapTo.transform.position;

        Vector3 objectDirection = (objectPosition - cameraPosition).normalized;
        Vector3 cameraDirection = cameraTransform.forward;

        // A quick way to check that the CameraRig.CenterCamera is initialized (not immediately true when running
        // in Unity Editor over Link)
        if (cameraTransform.position == Vector3.zero)
            return;

        // A one-time operation to set up the initial state of the current game object
        if (origCameraPosition == Vector3.zero)
        {
            origCameraPosition = cameraTransform.position;
            origCameraForward = cameraTransform.forward;
            originalScale = transform.localScale;
            transform.position = cameraPosition + cameraDirection * distanceToCamera;
            transform.rotation = Quaternion.LookRotation(transform.position - cameraToSnapTo.transform.position);
            transform.localScale = originalScale * distanceToCamera; // scale the game object based on the distance
                                                                     // so that it occupied the same FoV angle

            StartCoroutine(RevealCanvas(fadeInDuration));
            return;
        }

        // At start we keep the logo world locked and don't start snapping until the camera turns away from it
        snapStarted = snapStarted || Vector3.Angle(objectDirection, cameraDirection) > angleSnapThreshold;

        Vector3 newObjectPosition;
        if (!snapStarted)
        {
            // Before the camera snap started, we position the object based on the initial camera position
            newObjectPosition = origCameraPosition + origCameraForward * distanceToCamera;
        }
        else
        {
            Vector3 finalObjectPosition = cameraPosition + cameraDirection * distanceToCamera;
            newObjectPosition = Vector3.SmoothDamp(transform.position, finalObjectPosition,
                ref currentVelocity, snapDuration);

            // When the snap process only starts the distance to logo might be different from the distanceToCamera
            // To avoid logo jumps and to allow it to gradually change the distance we don't immediately
            // start resetting the distance, but are waiting for the moment the logo approaches the desired distance
            // for the first time. As soon as is happens, the component starts snapping the logo to the desired distance.
            snapDistanceStarted = snapDistanceStarted ||
                                  Math.Abs((newObjectPosition - cameraPosition).magnitude - distanceToCamera) < snapDistanceStartThreshold;

            if (snapDistanceStarted)
            {
                // make sure the distance from the camera is preserved
                newObjectPosition = cameraPosition + (newObjectPosition - cameraPosition).normalized * distanceToCamera;
            }
        }

        transform.position = newObjectPosition;
        transform.rotation = Quaternion.LookRotation(newObjectPosition - cameraToSnapTo.transform.position);
        transform.localScale = originalScale * distanceToCamera; // adjust the canvas scale based on the distance
    }

    private IEnumerator RevealCanvas(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            canvas.alpha = Mathf.Lerp(0, canvasTargetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvas.alpha = canvasTargetAlpha;
    }

    public async Task FadeoutCanvas(float duration)
    {
        float time = 0;
        float origAlpha = canvas.alpha;
        while (time < duration)
        {
            canvas.alpha = Mathf.Lerp(origAlpha, 0, time / duration);
            time += Time.deltaTime;
            await Task.Yield();
        }
        canvas.alpha = 0;
    }
}
