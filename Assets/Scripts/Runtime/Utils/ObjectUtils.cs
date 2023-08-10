/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Utils
{
    public static class ObjectUtils
    {
        public static bool AnyEquals(object a, object b) 
        {
            if (a is Object or null && b is Object or null) 
            {
                return (Object)a == (Object)b;
            }
            
            return a == b || Equals(a, b) || ReferenceEquals(a, b);
        }
        
        public static List<T> Shuffle<T>(this List<T> list) 
        {
            for(var i = list.Count - 1; i > 0; i--) 
            {
                var j = (int) Mathf.Floor(Random.value * ( i + 1 ));
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }
        
        public static T GetAddComponent<T>(this GameObject gameObject) where T : Component 
        {
            if ( gameObject == null ) return null;
            
            var result = gameObject.GetComponent<T>();
            if ( result == null ) 
            {
                result = gameObject.AddComponent<T>();
            }
            return result;
        }

        public static IEnumerable<GameObject> FindGameObjectsWithinLayerMask(LayerMask mask, GameObject exclude = null) 
        {
            return Object.FindObjectsOfType<GameObject>().Where(x => x != exclude && x.IsInLayerMask(mask));
        }
        
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask mask) 
        {
            return mask == (mask | (1 << gameObject.layer));
        }
    }
}
