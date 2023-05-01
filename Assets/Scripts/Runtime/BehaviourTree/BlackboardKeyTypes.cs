using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.BehaviourTree {

    [Serializable]
    public class BooleanKey : BlackboardKey<bool> { }
    
    [Serializable]
    public class GameObjectKey : BlackboardKey<GameObject> { }
    
    [Serializable]
    public class RigidBodyKey : BlackboardKey<Rigidbody2D> { }

    [Serializable]
    public class ColliderKey : BlackboardKey<Collider2D> { }
    
    [Serializable]
    public class ColorKey : BlackboardKey<Color> { }
    
    [Serializable]
    public class FloatKey : BlackboardKey<float> { }
    
    [Serializable]
    public class GameObjectListKey : BlackboardKey<List<GameObject>> { }
    
    [Serializable]
    public class IntKey : BlackboardKey<int> { }
    
    [Serializable]
    public class LayerMaskKey : BlackboardKey<LayerMask> { }
    
    [Serializable]
    public class ObjectKey : BlackboardKey<Object> { }
    
    [Serializable]
    public class ObjectListKey : BlackboardKey<List<Object>> { }
    
    [Serializable]
    public class StringKey : BlackboardKey<string> { }
    
    [Serializable]
    public class TransformKey : BlackboardKey<Transform> { }
    
    [Serializable]
    public class TransformListKey : BlackboardKey<List<Transform>> { }
    
    [Serializable]   
    public class Vector2Key : BlackboardKey<Vector2> { }
    
}
