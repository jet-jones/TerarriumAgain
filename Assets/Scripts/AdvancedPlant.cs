using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] float radius = 1.0f;
    [SerializeField] float branchRadius = 1.0f;
    [SerializeField] int sides = 12;
    [SerializeField] private int seed;
    [SerializeField] private int branchMinLength = 1;
    [SerializeField] private int branchMaxLength = 4;
    [SerializeField] private Vector2 wind;
    [SerializeField] private float directionChangeStrength;
    [SerializeField] private float upStrength;
    [SerializeField][Range(0,1)] private float branchChance;
    [SerializeField] private int length;
    [SerializeField] private float distanceBetweenPoints;

    private List<Vector3> _trunkPoints;
    private List<Branch> _branches;
    private Vector3 GetRandomDirection(Vector3 lastDirection) {
        var randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Abs(randomDirection.y);
        randomDirection += Vector3.up * upStrength + lastDirection * directionChangeStrength;
        return randomDirection.normalized;
    }

    public Vector3 GetGrowPoint() {
        var randomBranch = _branches[1];
        //var randomPoint = randomBranch.points[Random.Range(0, randomBranch.points.Count)];
        return  randomBranch.points[0];
    }

    private void OnDrawGizmosSelected() {
        if (_branches != null) {
            foreach (var branch in _branches) {
                for (var i = 1; i < branch.points.Count; i++) {
                    Gizmos.DrawLine(branch.points[i], branch.points[i - 1]);
                }
            }   
        }
    }

    private void Start() {
        var mesh = new Mesh {name = "Plant"};
        mesh.Clear();
        
        var ringCount = 0;
        var vertices = new List<Vector3>();
        var triangleIndices = new List<int>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        
        //Random.InitState(seed);
        _trunkPoints = new List<Vector3>();
        _branches = new List<Branch>();

        var offset = transform.position;
        var lastDirection = Vector3.up;
        for (var i = 0; i < length; i++) {
            _trunkPoints.Add(offset);
            var lastOffset = offset;
            offset += GetRandomDirection(lastDirection) * distanceBetweenPoints;
            lastDirection = Vector3.Normalize(offset - lastOffset);
        }
        
        for (var i = 0; i < length; i++) {
            if (Random.Range(0.0f, 1.0f) < branchChance) {
                var branchPointPos = _trunkPoints[i];
                var newBranch = new Branch();
                newBranch.trunkIndex = i;
                newBranch.points = new List<Vector3>();

                newBranch.points.Add(branchPointPos);
                
                branchPointPos += GetRandomDirection(Vector2.up) * distanceBetweenPoints;
                newBranch.points.Add(branchPointPos);
                var branchLength = Random.Range(branchMinLength, branchMaxLength) - 1;
                var lastBranchPos = _trunkPoints[i];
                for (var j = 0; j < branchLength; j++) {
                    var direction = Vector3.Normalize(branchPointPos - lastBranchPos);
                    lastBranchPos = branchPointPos;
                    branchPointPos += GetRandomDirection(direction) * distanceBetweenPoints;
                    newBranch.points.Add(branchPointPos);
                }
                
                _branches.Add(newBranch);
            }
        }

        var lastNormal = Vector3.up;
        for (var j = 0; j < _trunkPoints.Count; j++) {
            var pointWorld = _trunkPoints[j];
            var point = pointWorld - transform.position;
            //Gizmos.DrawSphere(pointWorld, 0.05f);
            if (j < _trunkPoints.Count - 1) {
                //Gizmos.color = Color.white;
                //Gizmos.DrawLine(pointWorld, _trunkPoints[j + 1]);

                var nextPoint = _trunkPoints[j + 1] - transform.position;
                var direction = Vector3.Normalize(point - nextPoint);
                var normal = Quaternion.LookRotation(direction, lastNormal) * Vector3.up;
                lastNormal = normal;
                var biTangent = Vector3.Cross(direction, normal);
                
                /*
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pointWorld, pointWorld + normal * 0.2f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(pointWorld, pointWorld + direction * 0.2f);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(pointWorld, pointWorld + biTangent * 0.2f);

                Gizmos.color = Color.white;
                */
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

                    if (ringCount > 0) {
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
                    normals.Add((vertexPosition - point).normalized );
                    uvs.Add(new Vector2(ringRadius / radius,j / (float)_trunkPoints.Count));
                }

                ringCount++;
            }
        }
        
        //Gizmos.color = Color.red;
        foreach (var branch in _branches) {
            //Gizmos.DrawLine(_trunkPoints[branch.trunkIndex], branch.points[0]);
            ringCount = 0;
            
            float trunkT = 1.0f - (branch.trunkIndex / (float)_trunkPoints.Count);
            
            for (var j = 0; j < branch.points.Count; j++) {
                var pointWorld = branch.points[j];
                var point = branch.points[j] - transform.position;
                //Gizmos.DrawSphere(pointWorld, 0.05f);
                    
                Vector3 direction;
                if (j > 0) {
                    var lastPoint = branch.points[j - 1];
                    direction = Vector3.Normalize(lastPoint - pointWorld);
                    //Gizmos.DrawLine(pointWorld, lastPoint);
                } else {
                    var nextPoint = branch.points[j + 1];
                    direction = Vector3.Normalize(pointWorld - nextPoint);
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
                    float ringRadius = trunkT * radius * branchT;
                    radialOffset *= ringRadius;

                    if (ringCount > 0) {
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
                    normals.Add((vertexPosition - point).normalized);
                    int totalPoints = _trunkPoints.Count + branch.points.Count;
                    uvs.Add(new Vector2(ringRadius / radius,(branch.trunkIndex + j) / (float)totalPoints));
                }

                ringCount++;
            }
        }
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
