using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ShadowCaster2D))]
[ExecuteInEditMode]
public class ShadowCaster2DController : MonoBehaviour
{
    [SerializeField] private bool setOnAwake;
    [HideInInspector, SerializeField] private ShadowCaster2D shadowCaster;
    [HideInInspector, SerializeField] private SpriteRenderer spriteRenderer;
    
    private readonly FieldInfo _shapePathField;
    private readonly FieldInfo _shapeHash;

    private ShadowCaster2DController()
    {
        _shapeHash = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    }
    
    private void Awake()
    {
        if (!setOnAwake) { return; }
        shadowCaster = GetComponent<ShadowCaster2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        var physicsShape = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, physicsShape);
        UpdateShadowFromPoints(physicsShape);
    }
    
    private void UpdateShadowFromPoints(List<Vector2> points)
    {
        _shapePathField.SetValue(shadowCaster, Vector2ToVector3(points));
        
        unchecked
        {
            var hashCode = (int)2166136261 ^ _shapePathField.GetHashCode();
            hashCode = hashCode * 16777619 ^ (points.GetHashCode());
            _shapeHash.SetValue(shadowCaster, hashCode);
        }
    }
    
    private Vector3[] Vector2ToVector3([NotNull] List<Vector2> vector2List)
    {
        if (vector2List == null) throw new ArgumentNullException(nameof(vector2List));
        var points = new Vector3[vector2List.Count];
 
        for (var i = 0; i < vector2List.Count; i++)
        {
            points[i] = vector2List[i];
        }
 
        return points;
    }
}
 