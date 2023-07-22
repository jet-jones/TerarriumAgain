using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AdvancedPlant : MonoBehaviour
{
    private struct Edge {
        public int a;
        public int b;

        public Edge(int a, int b) {
            this.a = a;
            this.b = b;
        }
    }

    private struct Branch
    {
        public int trunkIndex;
        public List<Vector3> points;
    }

    [SerializeField] private int branchMinLength = 1;
    [SerializeField] private int branchMaxLength = 4;

    private List<Vector3> trunkPoints;
    private List<Branch> branches;
    
    [SerializeField] private Vector2 wind;
    [SerializeField] private float directionChangeStrength;
    [SerializeField] private float upStrength;
    [SerializeField] private int seed;
    [SerializeField][Range(0,1)] private float branchChance;
    [SerializeField] private int length;
    [FormerlySerializedAs("offset")] [SerializeField] private float distanceBetweenPoints;

    private Vector3 GetRandomDirection(Vector3 lastDirection) {
        var randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Abs(randomDirection.y);
        randomDirection += Vector3.up * upStrength + lastDirection * directionChangeStrength;
        return randomDirection.normalized;
    }
    
    private void OnDrawGizmos()
    {
        Random.InitState(seed);
        trunkPoints = new List<Vector3>();
        branches = new List<Branch>();

        var offset = transform.position;
        for (var i = 0; i < length; i++) {
            var lastOffset = offset;
            trunkPoints.Add(offset);
            var direction = Vector3.Normalize(lastOffset - offset);
            offset += GetRandomDirection(direction) * distanceBetweenPoints;
        }
        
        for (var i = 0; i < length; i++) {
            if (Random.Range(0.0f, 1.0f) < branchChance) {
                var branchPointPos = trunkPoints[i];
                var newBranch = new Branch();
                newBranch.trunkIndex = i;
                newBranch.points = new List<Vector3>();

                branchPointPos += GetRandomDirection(Vector2.up) * distanceBetweenPoints;
                newBranch.points.Add(branchPointPos);
                var branchLength = Random.Range(branchMinLength, branchMaxLength) - 1;
                var lastBranchPos = trunkPoints[i];
                for (var j = 0; j < branchLength; j++) {
                    var direction = Vector3.Normalize(lastBranchPos - branchPointPos);
                    lastBranchPos = branchPointPos;
                    branchPointPos += GetRandomDirection(direction) * distanceBetweenPoints;
                    newBranch.points.Add(branchPointPos);
                }
                
                branches.Add(newBranch);
            }
        }

        for (var i = 0; i < trunkPoints.Count; i++) {
            var point = trunkPoints[i];
            Gizmos.DrawSphere(point, 0.05f);
            if (i < trunkPoints.Count - 1) {
                var nextPoint = trunkPoints[i + 1];
                Gizmos.DrawLine(point, nextPoint);
            }
        }

        Gizmos.color = Color.red;
        foreach (var branch in branches) {
            Gizmos.DrawLine(trunkPoints[branch.trunkIndex], branch.points[0]);
            for (var i = 0; i < branch.points.Count; i++) {
                var point = branch.points[i];
                Gizmos.DrawSphere(point, 0.05f);
                if (i < branch.points.Count - 1) {
                    var nextPoint = branch.points[i + 1];
                    Gizmos.DrawLine(point, nextPoint);
                }
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
