using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class AdvancedPlant : MonoBehaviour
{
    private struct Branch
    {
        public int trunkIndex;
        public List<Vector3> points;
    }

    private const float Tau = 6.28318530718f;

    [SerializeField] private float windScale;
    [SerializeField] private float windIntensity;
    [SerializeField] private float windSpeed;

    [SerializeField] private float branchRadiusShrink = 0.5f;
    [SerializeField] float radius = 1.0f;
    [SerializeField] int sides = 12;
    [SerializeField] private int branchMinLength = 1;
    [SerializeField] private int branchMaxLength = 4;
    [SerializeField] private Vector2 wind;
    [SerializeField] private float directionChangeStrength;
    [SerializeField] private float upStrength;
    [SerializeField][Range(0,1)] private float branchChance;
    [SerializeField] private int length;
    [SerializeField] private float distanceBetweenPoints;

    private List<Vector3> _baseTrunkPoints;
    private List<Branch> _baseBranches;

    // This is affected by wind
    private List<Vector3> _trunkPoints;
    private List<Branch> _branches;

    private Mesh _mesh;

    public void DrawDebug() {
#if UNITY_EDITOR
        if (_branches != null) {
            //Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            
            Handles.color = Color.red;

            for (var i = 1; i < _trunkPoints.Count; i++) {
                Handles.DrawLine(transform.TransformPoint(_trunkPoints[i]), 
                    transform.TransformPoint(_trunkPoints[i - 1]), 2); //, 10
            }
            
            Handles.color = Color.yellow;
            foreach (var branch in _branches) {
                for (var i = 1; i < branch.points.Count; i++) {
                    Handles.DrawLine(transform.TransformPoint(branch.points[i]), 
                        transform.TransformPoint(branch.points[i - 1]), 2); //, 10
                }
            }
        }
#endif
    }
    
    private Vector3 GetRandomDirection(Vector3 lastDirection) {
        var randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Abs(randomDirection.y);
        randomDirection += Vector3.up * upStrength + lastDirection * directionChangeStrength;
        return randomDirection.normalized;
    }

    public Vector3 GetGrowPoint(int index) {
        Random.InitState(index);
        var result = Vector3.zero;
        
        if (_branches.Count > 0) {
            var randomBranch = _branches[Random.Range(0, _branches.Count)];
            if (randomBranch.points.Count > 0) {
                result = transform.TransformPoint(randomBranch.points[Random.Range(0, randomBranch.points.Count)]);
            }
        }
        
        Random.InitState(Environment.TickCount);
        return result;
    }
    private void GenerateMesh(List<Vector3> vertices, 
        List<Vector2> uvs = null, List<Vector3> normals = null, List<int> triangleIndices = null) {
        
        _trunkPoints = new List<Vector3>();
        _branches = new List<Branch>();

        for (var i = 0; i < _baseTrunkPoints.Count; i++) {
            var trunkPoint = _baseTrunkPoints[i];
            var moveT = i / (float)_baseTrunkPoints.Count;
            var xOffset = Mathf.PerlinNoise(i * windScale, Time.time * windSpeed) * windIntensity * moveT;
            var zOffset = Mathf.PerlinNoise(i * windScale + 10000, Time.time * windSpeed) * windIntensity * moveT;

            _trunkPoints.Add(trunkPoint + new Vector3(xOffset, 0, zOffset));
        }
        
        foreach (var branch in _baseBranches) {
            var newBranch = new Branch {
                points = new List<Vector3>(),
                trunkIndex = branch.trunkIndex
            };

            var baseTrunkPoint = _baseTrunkPoints[branch.trunkIndex];
            var trunkPoint = _trunkPoints[branch.trunkIndex];

            for (var i = 0; i < branch.points.Count; i++) {
                var moveT = i / (float)branch.points.Count;
                var movedPoint = branch.points[i];
                movedPoint -= baseTrunkPoint;
                movedPoint += trunkPoint;
                
                var xOffset = Mathf.PerlinNoise(i * windScale, Time.time * windSpeed) * windIntensity * moveT;
                var zOffset = Mathf.PerlinNoise(i * windScale + 10000, Time.time * windSpeed) * windIntensity * moveT;

                newBranch.points.Add(movedPoint  + new Vector3(xOffset, 0, zOffset));
            }
            
            _branches.Add(newBranch);
        }
        
        /// ============== MESH
        
        var ringCount = 0;
        
        var lastNormal = Vector3.up;
        for (var j = 0; j < _trunkPoints.Count; j++) {
            var point = _trunkPoints[j];
            
            Vector3 direction;
            if (j > 0) {
                var lastPoint = _trunkPoints[j - 1];
                direction = Vector3.Normalize(lastPoint - point);
            } else {
                var nextPoint = _trunkPoints[j + 1];
                direction = Vector3.Normalize(point - nextPoint);
            }
            
            var normal = Quaternion.LookRotation(direction, lastNormal) * Vector3.up;
            lastNormal = normal;
            var biTangent = Vector3.Cross(direction, normal);
            
            float trunkT = 1.0f - (j / (float)_trunkPoints.Count);
            
            var baseVert = vertices.Count;

            for (var i = 0; i < sides; i++) {
                var f = i / (float)sides;
                var angRad = f * Tau;

                var radialOffset = new Vector2(
                    Mathf.Cos(angRad),
                    Mathf.Sin(angRad)
                );
                float ringRadius = trunkT * radius;
                radialOffset *= ringRadius;

                if (triangleIndices != null && ringCount > 0) {
                    var currentVert = baseVert + i;
                    var nextVert = baseVert + ((i + 1) % sides);
                    
                    // these are the same verts but on the previous ring
                    var currentVertLast = currentVert - sides;
                    var nextVertLast = baseVert - sides + ((i + 1) % sides);
                    
                    triangleIndices.Add(currentVert);
                    triangleIndices.Add(nextVert);
                    triangleIndices.Add(currentVertLast);
                    
                    triangleIndices.Add(currentVertLast);
                    triangleIndices.Add(nextVert);
                    triangleIndices.Add(nextVertLast);
                }
                
                var vertexPosition = point + normal * radialOffset.x + biTangent * radialOffset.y;
                
                vertices.Add(vertexPosition);
                if (normals != null) normals.Add((vertexPosition - point).normalized );
                if (uvs != null ) uvs.Add(new Vector2(ringRadius / radius,j / (float)_trunkPoints.Count));
            }

            ringCount++;
        }
        
        foreach (var branch in _branches) {
            ringCount = 0;
            
            float trunkT = 1.0f - (branch.trunkIndex / (float)_trunkPoints.Count);
            
            for (var j = 0; j < branch.points.Count; j++) {
                var point = branch.points[j];
                    
                Vector3 direction;
                if (j > 0) {
                    var lastPoint = branch.points[j - 1];
                    direction = Vector3.Normalize(lastPoint - point);
                } else {
                    var nextPoint = branch.points[j + 1];
                    direction = Vector3.Normalize(point - nextPoint);
                }
                
                var normal = Quaternion.LookRotation(direction, lastNormal) * Vector3.up;
                lastNormal = normal;
                var biTangent = Vector3.Cross(direction, normal);
                
                var baseVert = vertices.Count;

                float branchT = 1.0f - (j / (float)branch.points.Count);
                
                for (var i = 0; i < sides; i++) {
                    var f = i / (float)sides;
                    var angRad = f * Tau;

                    var radialOffset = new Vector2(
                        Mathf.Cos(angRad),
                        Mathf.Sin(angRad)
                    );
                    float ringRadius = trunkT * radius * branchT * branchRadiusShrink;
                    radialOffset *= ringRadius;

                    if (triangleIndices != null && ringCount > 0) {
                        var currentVert = baseVert + i;
                        var nextVert = baseVert + ((i + 1) % sides);
                    
                        // these are the same verts but on the previous ring
                        var currentVertLast = currentVert - sides;
                        var nextVertLast = baseVert - sides + ((i + 1) % sides);
                    
                        triangleIndices.Add(currentVert);
                        triangleIndices.Add(nextVert);
                        triangleIndices.Add(currentVertLast);
                    
                        triangleIndices.Add(currentVertLast);
                        triangleIndices.Add(nextVert);
                        triangleIndices.Add(nextVertLast);
                    }
                
                    var vertexPosition = point + normal * radialOffset.x + biTangent * radialOffset.y;
                    //Gizmos.DrawSphere(vertexPosition, 0.01f);
                
                    vertices.Add(vertexPosition);
                    if (normals != null) normals.Add((vertexPosition - point).normalized);
                    int totalPoints = _trunkPoints.Count + branch.points.Count;
                    if (uvs != null) uvs.Add(new Vector2(ringRadius / radius,(branch.trunkIndex + j) / (float)totalPoints));
                }

                ringCount++;
            }
        }
    }
    
    public void UpdateMesh() {
        var vertices = new List<Vector3>();
        GenerateMesh(vertices);
        _mesh.SetVertices(vertices);
    }

    public void Generate(int seed) {
        Random.InitState(seed);
        _baseTrunkPoints = new List<Vector3>();
        _baseBranches = new List<Branch>();

        var offset = Vector3.zero;
        var lastDirection = Vector3.up;
        for (var i = 0; i < length; i++) {
            _baseTrunkPoints.Add(offset);
            var lastOffset = offset;
            offset += GetRandomDirection(lastDirection) * distanceBetweenPoints;
            lastDirection = Vector3.Normalize(offset - lastOffset);
        }
        
        for (var i = 0; i < length; i++) {
            if (Random.Range(0.0f, 1.0f) < branchChance) {
                var branchPointPos = _baseTrunkPoints[i];
                var newBranch = new Branch();
                newBranch.trunkIndex = i;
                newBranch.points = new List<Vector3>();

                newBranch.points.Add(branchPointPos);
                
                branchPointPos += GetRandomDirection(Vector2.up) * distanceBetweenPoints;
                newBranch.points.Add(branchPointPos);
                var branchLength = Random.Range(branchMinLength, branchMaxLength) - 1;
                var lastBranchPos = _baseTrunkPoints[i];
                for (var j = 0; j < branchLength; j++) {
                    var direction = Vector3.Normalize(branchPointPos - lastBranchPos);
                    lastBranchPos = branchPointPos;
                    branchPointPos += GetRandomDirection(direction) * distanceBetweenPoints;
                    newBranch.points.Add(branchPointPos);
                }
                
                _baseBranches.Add(newBranch);
            }
        }

        _mesh = new Mesh { name = "Plant" };
        _mesh.Clear();
        var vertices = new List<Vector3>();
        var triangleIndices = new List<int>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        GenerateMesh(vertices, uvs, normals, triangleIndices);
        
        _mesh.SetVertices(vertices);
        _mesh.SetTriangles(triangleIndices, 0);
        _mesh.SetNormals(normals);
        _mesh.SetUVs(0, uvs);
        GetComponent<MeshFilter>().sharedMesh = _mesh;
        Random.InitState(Environment.TickCount);
    }
}
