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

using System.Collections;
using UnityEngine;

/// <summary>
/// This class will continuously check the mesh of a MeshFilter component
/// and if it has changed to a new mesh, it will create barycentric
/// coordinates and set this to the per vertex color information of the
/// mesh.
///
/// We only need barycentric coordinates for our wireframe shader.
/// </summary>
/// <remarks>
/// Using this will increase the number of vertices by 3. Avoid using
/// this class for anything other than debugging.
///
/// The alternative is to use geometry shaders, however, they aren't
/// well supported for non Multi-Pass rendering modes.
/// </remarks>
[RequireComponent(typeof(MeshFilter))]
public class SetMeshBarycentricCoordinates : MonoBehaviour
{
    MeshFilter _meshFilter;
    Mesh _mesh;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();

        StartCoroutine(CheckMeshData());
    }

    private IEnumerator CheckMeshData()
    {
        yield return null;

        if (_meshFilter.mesh == null ||
            _mesh == _meshFilter.mesh ||
            _meshFilter.mesh.vertexCount == 0)
        {
            yield return new WaitForSeconds(1);
        }

        // we have a new mesh with data that we need to populate
        CreateBarycentricCoordinates();
    }

    private void CreateBarycentricCoordinates()
    {
        // calculate the barycentric coordinate per vertex, and set
        // provide these in the color data of the mesh.
        var mesh = _meshFilter.mesh;

        var vertices = mesh.vertices;
        var triangles = mesh.GetTriangles(0);

        var c = new Color[triangles.Length];
        var v = new Vector3[triangles.Length];
        var idx = new int[triangles.Length];
        for (var i = 0; i < triangles.Length; i++)
        {
            c[i] = new Color(
                i % 3 == 0 ? 1.0f : 0.0f,
                i % 3 == 1 ? 1.0f : 0.0f,
                i % 3 == 2 ? 1.0f : 0.0f);
            v[i] = vertices[triangles[i]];
            idx[i] = i;
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(v);
        mesh.SetColors(c);
        mesh.SetIndices(idx, MeshTopology.Triangles, 0, true, 0);

        _mesh = mesh;
        _meshFilter.mesh = _mesh;
        _meshFilter.mesh.RecalculateNormals();
    }
}
