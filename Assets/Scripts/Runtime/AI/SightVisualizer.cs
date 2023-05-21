﻿using System.Collections.Generic;
using Runtime.AI.Interfaces;
using Runtime.BehaviourTree;
using Runtime.Player;
using UnityEngine;

namespace Runtime.AI
{
    public class SightVisualizer : MonoBehaviour
    {
        private BehaviourTreeOwner _sight;
        private Mesh _mesh;
        private Vector3[] _vertices;
        private List<Vector3> _viewVertex;
        private RaycastHit2D _hit;
        private const float MESH_RES = 2;
        private int[] _triangles;
        private int _stepCount;
        private int _activated;
        
        private Vector2 _lastSeenPosition;

        private void Awake()
        {
            //find the _sight component in siblings
            _sight = transform.parent.GetComponent<BehaviourTreeOwner>();
            _mesh = GetComponent<MeshFilter>().mesh;
        }
    
        private void LateUpdate()
        {
            UpdateMesh();
        }
    
        private void UpdateMesh()
        {
            ShootRays();
            if(_sight.debug) CreateMesh();
        }
    
        private void ShootRays()
        {
            _stepCount = Mathf.RoundToInt(_sight.ViewAngle * MESH_RES);
            float stepAngle = _sight.ViewAngle / _stepCount;

            _viewVertex = new List<Vector3>();
            _hit = new RaycastHit2D();

            GameObject spottedPlayer = null;
            for (int i = 0; i < _stepCount; i++)
            {
                float angle = _sight.transform.eulerAngles.y - _sight.ViewAngle / 2 + stepAngle * i + 90;
                Vector3 dir = _sight.DirFromAngle(angle) * Mathf.Sign(transform.parent.transform.localScale.x);
                _hit = Physics2D.Raycast(_sight.transform.position, dir, _sight.ViewRadius, _sight.ObstacleMask | _sight.PlayerMask);
                
                if (_hit.collider != null)
                {
                    bool playerHit = (_sight.PlayerMask.value & 1 << _hit.transform.gameObject.layer) > 0 && _hit.collider.CompareTag("Player");
                    if (playerHit)
                    {
                        var player = _hit.collider.GetComponent<PlayerController>();
                        if(player.isHiding) continue;
                        
                        //if the player is not hiding, call the OnSightEnter method
                        player.GetComponent<ISightEntity>().IsSeen = true;
                        _sight.gameObject.GetComponent<ISightHandler>().OnSightEnter();
                        spottedPlayer = _hit.transform.gameObject;
                    }
                    else if(_sight.debug) 
                    { 
                        _viewVertex.Add(transform.position + dir.normalized * (_hit.distance));
                    }
                }
                else if(_sight.debug) 
                {
                    _viewVertex.Add(transform.position + dir.normalized * _sight.ViewRadius);
                }
            }

            if (spottedPlayer == null)
            {
                _sight.gameObject.GetComponent<ISightHandler>().OnSightExit(_lastSeenPosition);
                _lastSeenPosition = Vector2.zero;
            }
            else
            {
                _lastSeenPosition = spottedPlayer.transform.position;
            }
        }

        //Creating a fov mesh
        private void CreateMesh()
        {
            int vertexCount = _viewVertex.Count + 1;
            _vertices = new Vector3[vertexCount];
            _triangles = new int[(vertexCount - 2) * 3];
            _vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                _vertices[i + 1] = transform.InverseTransformPoint(_viewVertex[i]);
                if (i < vertexCount - 2)
                {
                    _triangles[i * 3 + 2] = 0;
                    _triangles[i * 3 + 1] = i + 1;
                    _triangles[i * 3] = i + 2;
                }
            }
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.RecalculateNormals();
        }

    }
}
