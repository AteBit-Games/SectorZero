/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections.Generic;
using Runtime.AI.Interfaces;
using Runtime.Player;
using UnityEngine;

namespace Runtime.AI.Sight
{
    public class SentinelSightVisualizer : MonoBehaviour
    {
        private Sentinel _sight;
        private Mesh _mesh;
        private Vector3[] _vertices;
        private List<Vector3> _viewVertex;
        private RaycastHit2D _hit;
        private const float MESH_RES = 2;
        private int[] _triangles;
        private int _stepCount;
        private int _activated;
        private PlayerController _player;

        private void Awake()
        {
            _player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            _sight = transform.parent.GetComponent<Sentinel>();
            _mesh = GetComponent<MeshFilter>().mesh;
        }
    
        private void LateUpdate()
        {
            if(_sight.IsActivated) UpdateMesh();
            else _mesh.Clear();
        }
    
        private void UpdateMesh()
        {
            if(_sight.IsActivated) ShootRays();
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
                
                //if the player is crouching take into account the obstacle mask
                if (_player.isSneaking)
                {
                    _hit = Physics2D.Raycast(_sight.transform.position, dir, _sight.ViewRadius, _sight.WallMask | _sight.ObstacleMask | _sight.PlayerMask);
                }
                else
                {
                    _hit = Physics2D.Raycast(_sight.transform.position, dir, _sight.ViewRadius, _sight.PlayerMask | _sight.WallMask);
                }
                
                if (_hit.collider != null)
                {
                    bool playerHit = (_sight.PlayerMask.value & 1 << _hit.transform.gameObject.layer) > 0 && _hit.collider.CompareTag("Player");
                    if (playerHit)
                    {
                        var player = _hit.collider.GetComponent<PlayerController>();
                        if(player == null) player = _hit.collider.GetComponentInParent<PlayerController>();
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
                else
                {
                    _viewVertex.Add(transform.position + dir.normalized * _sight.ViewRadius);
                }
            }

            if (spottedPlayer == null)
            {
                //_sight.gameObject.GetComponent<ISightHandler>().OnSightExit();
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
