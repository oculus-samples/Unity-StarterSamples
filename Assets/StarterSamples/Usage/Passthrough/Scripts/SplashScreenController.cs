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

using System.Threading.Tasks;
using Meta.XR.Samples;
using UnityEngine;

[MetaCodeSample("StarterSample-Passthrough")]
public class SplashScreenController : MonoBehaviour
{
    private CameraSnapBehaviour cameraSnapBehaviour;
    private CameraFoVBehaviour cameraFoVBehavior;

    public float ImageDistance
    {
        get => cameraSnapBehaviour.distanceToCamera;
        set => cameraSnapBehaviour.distanceToCamera = value;
    }

    public float ImageFoV
    {
        get => cameraFoVBehavior.FoV;
        set => cameraFoVBehavior.FoV = value;
    }

    public float ImageFadeInDuration
    {
        get => cameraSnapBehaviour.fadeInDuration;
        set => cameraSnapBehaviour.fadeInDuration = value;
    }

    public Task FadeoutSplashImagePanel(float duration)
    {
        return cameraSnapBehaviour.FadeoutCanvas(duration);
    }

    private void Start()
    {
        cameraSnapBehaviour = gameObject.GetComponent<CameraSnapBehaviour>();
        cameraFoVBehavior = gameObject.GetComponent<CameraFoVBehaviour>();
    }
}
