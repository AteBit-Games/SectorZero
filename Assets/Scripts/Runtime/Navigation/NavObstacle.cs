using UnityEngine;

namespace Runtime.Navigation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Navigation/NavObstacle")]
    public class NavObstacle : MonoBehaviour
    {
        public enum ShapeType
        {
            Polygon,
            Box
        }
        
        //Fires when the type of the obstacle is changed
        public static event System.Action<NavObstacle, bool> OnObstacleStateChange;
        
        [Tooltip("The Shape used. Changing this will also change the Collider2D component type.")]
        public ShapeType shapeType = ShapeType.Polygon;
        [Tooltip("Added extra offset radius.")]
        public float extraOffset;
        [Tooltip("Inverts the polygon.")]
        public bool invertPolygon;

        private Collider2D _collider;
        private Collider2D MyCollider => _collider != null ? _collider : _collider = GetComponent<Collider2D>();

        public int GetPathCount() 
        {
            return MyCollider switch
            {
                BoxCollider2D => 1,
                PolygonCollider2D polygonCollider2D => polygonCollider2D.pathCount,
                _ => 0
            };
        }
        
        public Vector2[] GetPathPoints(int index) 
        {
            Vector2[] points = null;

            if (MyCollider is BoxCollider2D boxCollider2D)
            {
                var size = boxCollider2D.size;
                var offset = boxCollider2D.offset;
                
                var tl = offset + ( new Vector2(-size.x, size.y) / 2 );
                var tr = offset + ( new Vector2(size.x, size.y) / 2 );
                var br = offset + ( new Vector2(size.x, -size.y) / 2 );
                var bl = offset + ( new Vector2(-size.x, -size.y) / 2 );
                points = new[] { tl, tr, br, bl };
            }

            if (MyCollider is PolygonCollider2D polygonCollider2D)
            {
                points = polygonCollider2D.GetPath(index);
            }

            if (invertPolygon && points != null) System.Array.Reverse(points); 
            return points;
        }

        private void Reset() 
        {
            if(MyCollider == null)
            {
                gameObject.AddComponent<PolygonCollider2D>();
                invertPolygon = true;
            }
            else
            {
                shapeType = MyCollider switch
                {
                    PolygonCollider2D => ShapeType.Polygon,
                    BoxCollider2D => ShapeType.Box,
                    _ => shapeType
                };
            }
            
            MyCollider.isTrigger = true;
            if(GetComponent<SpriteRenderer>() != null)
            {
                invertPolygon = true;
            }
        }

        private void OnEnable()
        {
            OnObstacleStateChange?.Invoke(this, true);
        }

        private void OnDisable()
        {
            OnObstacleStateChange?.Invoke(this, false);
        }

        private void Awake() {
            transform.hasChanged = false;
        }
    }
}