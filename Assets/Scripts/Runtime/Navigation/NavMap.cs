using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Navigation
{
    public class NavMap : MonoBehaviour
    {
        // ===================================== Public Variables =====================================
        
        [Header("NAVMAP SETTINGS")]
        [Tooltip("The Shape used. Changing this will also change the Collider2D component type.")]
        public NavObstacle.ShapeType shapeType = NavObstacle.ShapeType.Polygon;
        [Tooltip("All NavObstacles in the selected layers will be monitored and added/removed automatically.")]
        public LayerMask obstaclesMask = -1;
        [Tooltip("The frame interval to auto update obstacle changes (enabled/disable or transform change). Set 0 to disable auto-regenerate.")]
        public int autoRegenerateInterval = 60;
        [Tooltip("The radius from the edges to offset the agents.")]
        public float radiusOffset = 0.1f;
        [Tooltip("Inverts the master polygon.")]
        public bool invertMasterPolygon;
        
        //Fires when the map is generated
        public static event Action<NavMap> OnMapGenerated;
        
        // ===================================== Private Variables =====================================
        
        private List<NavObstacle> _navObstacles;
        private PolyMap _map;
        private readonly List<PathNode> _nodes = new();
        private PathNode[] _tempNodes;
        
        private readonly Queue<PathRequest> _pathRequests = new();
        private PathRequest _currentRequest;
        private bool _isProcessingPath;
        private PathNode _startNode;
        private PathNode _endNode;
        private bool _regenerateFlag;
        
        // ===================================== Public Interface =====================================
        
        private Collider2D _masterCollider;
        private Collider2D MasterCollider {
            get
            {
                if (_masterCollider != null) return _masterCollider;
                var colliders = GetComponents<Collider2D>();
                return _masterCollider = colliders.FirstOrDefault();
            }
        }
        
        private static NavMap current;
        public static NavMap Current {
            get
            {
                if (current == null || !Application.isPlaying) 
                {
                    current = FindObjectOfType<NavMap>();
                }

                return current;
            }
        }
        
        public int NodesCount => _nodes.Count;

        // ===================================== Unity Methods =====================================
        
        private void Awake() {
            if(current == null) current = this;
            _regenerateFlag = false;
            _isProcessingPath = false;
            
            _navObstacles = FindObjectsOfType<NavObstacle>().Where(o => obstaclesMask == (obstaclesMask | 1 << o.gameObject.layer)).ToList();
            NavObstacle.OnObstacleStateChange += MonitorObstacle;
            if(MasterCollider != null) 
            {
                MasterCollider.enabled = false;
                GenerateMap();
            }
        }

        public void LateUpdate() 
        {
            if(Time.frameCount % autoRegenerateInterval != 0) return;

            foreach (var obstacle in _navObstacles.Where(obstacle => obstacle.transform.hasChanged))
            {
                obstacle.transform.hasChanged = false;
                _regenerateFlag = true;
            }

            if(_regenerateFlag) 
            {
                _regenerateFlag = false;
                GenerateMap(false);
            }
        }
        
        // ====================================== Helpers ======================================

        private void MonitorObstacle(NavObstacle obstacle, bool active) 
        {
            if (obstaclesMask == (obstaclesMask | 1 << obstacle.gameObject.layer)) 
            {
                if (active) AddObstacle(obstacle); 
                else RemoveObstacle(obstacle);
            }
        }
        
        public void AddObstacle(NavObstacle navObstacle) 
        {
            if(!_navObstacles.Contains(navObstacle)) 
            {
                _navObstacles.Add(navObstacle);
                _regenerateFlag = true;
            }
        }

        public void RemoveObstacle(NavObstacle navObstacle) 
        {
            if(_navObstacles.Contains(navObstacle))
            {
                _navObstacles.Remove(navObstacle);
                _regenerateFlag = true;
            }
        }

        public void GenerateMap(bool generateMaster = true) 
        {
            CreatePolyMap(generateMaster);
            CreateNodes();
            LinkNodes(_nodes);
            OnMapGenerated?.Invoke(this);
        }
        
        // ====================================== Pathfinding ======================================
        
        public void FindPath(Vector2 start, Vector2 end, Action<Vector2[]> callback) 
        {
            if(CheckLos(start, end)) 
            {
                callback(new[] {start, end});
                return;
            }

            _pathRequests.Enqueue(new PathRequest(start, end, callback));
            TryNextFindPath();
        }

        private void TryNextFindPath()
        {
            if (_isProcessingPath || _pathRequests.Count <= 0) return;

            _isProcessingPath = true;
            _currentRequest = _pathRequests.Dequeue();

            if(!PointIsValid(_currentRequest.start)) 
            {
                _currentRequest.start = GetCloserEdgePoint(_currentRequest.start);
            }
            
            _startNode = new PathNode(_currentRequest.start);
            _endNode = new PathNode(_currentRequest.end);

            _nodes.Add(_startNode);
            LinkStart(_startNode, _nodes);

            _nodes.Add(_endNode);
            LinkEnd(_endNode, _nodes);

            AStar.CalculatePath(_startNode, _endNode, _nodes, RequestDone);
        }


        public void RequestDone(Vector2[] path) 
        {
            for(int i = 0; i < _endNode.links.Count; i++) 
            {
                _nodes[_endNode.links[i]].links.Remove(_nodes.IndexOf(_endNode));
            }
            _nodes.Remove(_endNode);
            _nodes.Remove(_startNode);

            _isProcessingPath = false;
            _currentRequest.callback(path);
            TryNextFindPath();
        }

        private void CreatePolyMap(bool generateMaster) 
        {
            var masterPolys = new List<Polygon>();
            var obstaclePolys = new List<Polygon>();

            //create a polygon object for each obstacle
            foreach (var obstacle in _navObstacles)
            {
                if (obstacle == null) continue;
                
                var rad = radiusOffset + obstacle.extraOffset;
                for(var p = 0; p < obstacle.GetPathCount(); p++) 
                {
                    var points = obstacle.GetPathPoints(p);
                    var transformed = TransformPoints(ref points, obstacle.transform);
                    var inflated = InflatePolygon(ref transformed, rad);
                    obstaclePolys.Add(new Polygon(inflated));
                }
            }

            if(generateMaster)
            {
                if(MasterCollider is PolygonCollider2D polygonCollider2D)
                {
                    for(int i = 0; i < polygonCollider2D.pathCount; ++i) 
                    {
                        var points = polygonCollider2D.GetPath(i);

                        if(invertMasterPolygon) Array.Reverse(points);

                        var transformed = TransformPoints(ref points, polygonCollider2D.transform);
                        var inflated = InflatePolygon(ref transformed, Mathf.Max(0.01f, radiusOffset));
                        masterPolys.Add(new Polygon(inflated));
                    }
                }
                else if (MasterCollider is BoxCollider2D boxCollider2D)
                {
                    var offset = boxCollider2D.offset;
                    var size = boxCollider2D.size;
                        
                    var tl = offset + new Vector2(-size.x, size.y) / 2;
                    var tr = offset + new Vector2(size.x, size.y) / 2;
                    var br = offset + new Vector2(size.x, -size.y) / 2;
                    var bl = offset + new Vector2(-size.x, -size.y) / 2;
                        
                    var points = new[] { tl, bl, br, tr };
                    var transformed = TransformPoints(ref points, MasterCollider.transform);
                    var inflated = InflatePolygon(ref transformed, Mathf.Max(0.01f, radiusOffset));
                    masterPolys.Add(new Polygon(inflated));
                }
            } 
            else
            {
                if (_map != null) 
                {
                    masterPolys = _map.masterPolygons.ToList();
                }
            }

            _map = new PolyMap(masterPolys.ToArray(), obstaclePolys.ToArray());
        }

        public void CreateNodes()
        {
            _nodes.Clear();

            foreach (var poly in _map.AllPolygons)
            {
                var points = poly.points.ToArray();
                var inflatedPoints = InflatePolygon(ref points, 0.05f);
                
                for(int i = 0; i < inflatedPoints.Length; i++)
                {
                    if(PointIsConcave(inflatedPoints, i) || !PointIsValid(inflatedPoints[i])) continue;
                    _nodes.Add(new PathNode(inflatedPoints[i]));
                }
            }
        }

        public void LinkNodes(List<PathNode> nodeList) 
        {
            for(int i = 0; i < nodeList.Count; i++) 
            {
                nodeList[i].links.Clear();

                for(int j = 0; j < nodeList.Count; j++) 
                {
                    if(nodeList[i] == nodeList[j] || j > i) continue;
                    
                    if(CheckLos(nodeList[i].pos, nodeList[j].pos)) 
                    {
                        nodeList[i].links.Add(j);
                        nodeList[j].links.Add(i);
                    }
                }
            }
        }

        private void LinkStart(PathNode start, IReadOnlyList<PathNode> toNodes) 
        {
            for(int i = 0; i < toNodes.Count; i++) 
            {
                if(CheckLos(start.pos, toNodes[i].pos)) start.links.Add(i);
            }
        }
        
        private void LinkEnd(PathNode end, List<PathNode> toNodes) 
        {
            for(int i = 0; i < toNodes.Count; i++)
            {
                if(CheckLos(end.pos, toNodes[i].pos)) 
                {
                    end.links.Add(i);
                    toNodes[i].links.Add(toNodes.IndexOf(end));
                }
            }
        }
        
        public bool CheckLos(Vector2 posA, Vector2 posB) 
        {
            if((posA - posB ).magnitude < Mathf.Epsilon) return true;

            foreach (var poly in _map.AllPolygons)
            {
                for(int j = 0; j < poly.points.Length; j++) 
                {
                    if(SegmentsCross(posA, posB, poly.points[j], poly.points[( j + 1 ) % poly.points.Length])) return false;
                }
            }
            return true;
        }
        
        public bool PointIsValid(Vector2 point) 
        {
            for (int i = 0; i < _map.AllPolygons.Length; i++) 
            {
                if(i == 0 ? !PointInsidePolygon(_map.AllPolygons[i].points, point) : PointInsidePolygon(_map.AllPolygons[i].points, point)) return false;
            }
            return true;
        }

        private static Vector2[] TransformPoints(ref Vector2[] points, Transform t) 
        {
            for(int i = 0; i < points.Length; i++) points[i] = t.TransformPoint(points[i]);
            return points;
        }
        
        public static Vector2[] InflatePolygon(ref Vector2[] points, float dist)
        {
            var enlargedPoints = new Vector2[points.Length];
            for(var j = 0; j < points.Length; j++)
            {
                var i = (j - 1);
                if (i < 0) i += points.Length;
                var k = ( j + 1 ) % points.Length;
                
                var v1 = new Vector2(points[j].x - points[i].x, points[j].y - points[i].y).normalized;
                v1 *= dist;
                var n1 = new Vector2(-v1.y, v1.x);

                var pij1 = new Vector2(points[i].x + n1.x, points[i].y + n1.y);
                var pij2 = new Vector2(points[j].x + n1.x, points[j].y + n1.y);

                var v2 = new Vector2(points[k].x - points[j].x, points[k].y - points[j].y).normalized;
                v2 *= dist;
                var n2 = new Vector2(-v2.y, v2.x);

                var pjk1 = new Vector2(points[j].x + n2.x, points[j].y + n2.y);
                var pjk2 = new Vector2(points[k].x + n2.x, points[k].y + n2.y);

                FindIntersection(pij1, pij2, pjk1, pjk2, out _, out bool _, out Vector2 poi, out Vector2 _, out Vector2 _);
                enlargedPoints[j] = poi;
            }
            return enlargedPoints.ToArray();
        }
        
        public static void FindIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out bool linesIntersect, out bool segmentsIntersect, out Vector2 intersection, out Vector2 closeP1, out Vector2 closeP2) 
        {
            var dx12 = p2.x - p1.x;
            var dy12 = p2.y - p1.y;
            var dx34 = p4.x - p3.x;
            var dy34 = p4.y - p3.y;
            
            var denominator = ( dy12 * dx34 - dx12 * dy34 );

            var t1 = ((p1.x - p3.x ) * dy34 + ( p3.y - p1.y) * dx34) / denominator;
            if(float.IsInfinity(t1)) 
            {
                linesIntersect = false;
                segmentsIntersect = false;
                intersection = new Vector2(float.NaN, float.NaN);
                closeP1 = new Vector2(float.NaN, float.NaN);
                closeP2 = new Vector2(float.NaN, float.NaN);
                return;
            }
            linesIntersect = true;

            var t2 = ( ( p3.x - p1.x ) * dy12 + ( p1.y - p3.y ) * dx12 ) / -denominator;
            intersection = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);
            segmentsIntersect = ( t1 is >= 0 and <= 1 && t2 is >= 0 and <= 1 );

            t1 = t1 switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => t1
            };
            t2 = t2 switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => t2
            };

            closeP1 = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);
            closeP2 = new Vector2(p3.x + dx34 * t2, p3.y + dy34 * t2);
        }
        
        public static bool SegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d) 
        {
            bool c1 = (d.y - a.y) * (c.x - a.x ) > (c.y - a.y) * (d.x - a.x);
            bool c2 = (d.y - b.y) * (c.x - b.x ) > (c.y - b.y) * (d.x - b.x);
            bool c3 = (c.y - a.y) * (b.x - a.x ) > (b.y - a.y) * (c.x - a.x);
            bool c4 = (d.y - a.y) * (b.x - a.x ) > (b.y - a.y) * (d.x - a.x);
            return c1 != c2 && c3 != c4;
        }
        
        public static bool PointIsConcave(Vector2[] points, int pointIndex) 
        {
            Vector2 point = points[pointIndex];
            Vector2 next = points[( pointIndex + 1 ) % points.Length];
            Vector2 previous = points[pointIndex == 0 ? points.Length - 1 : pointIndex - 1];
            Vector2 left = new Vector2(point.x - previous.x, point.y - previous.y);
            Vector2 right = new Vector2(next.x - point.x, next.y - point.y);
            return (left.x * right.y) - (left.y * right.x) > 0;
        }
        
        public static bool PointInsidePolygon(Vector2[] polyPoints, Vector2 point) 
        {
            float xMin = float.PositiveInfinity;
            for(int i = 0; i < polyPoints.Length; i++) xMin = Mathf.Min(xMin, polyPoints[i].x);

            Vector2 origin = new Vector2(xMin - 0.1f, point.y);
            int intersections = 0;

            for(int i = 0; i < polyPoints.Length; i++)
            {
                Vector2 pA = polyPoints[i];
                Vector2 pB = polyPoints[( i + 1 ) % polyPoints.Length];

                if(SegmentsCross(origin, point, pA, pB)) intersections++;
            }
            
            return ( intersections & 1 ) == 1;
        }
        
        public Vector2 GetCloserEdgePoint(Vector2 point) 
        {
            var possiblePoints = new List<Vector2>();
            var closerVertex = Vector2.zero;
            var closerVertexDist = Mathf.Infinity;

            foreach (var poly in _map.AllPolygons)
            {
                var points = poly.points.ToArray();
                var inflatedPoints = InflatePolygon(ref points, 0.01f);

                for(int i = 0; i < inflatedPoints.Length; i++) 
                {
                    Vector2 a = inflatedPoints[i];
                    Vector2 b = inflatedPoints[( i + 1 ) % inflatedPoints.Length];

                    Vector2 originalA = poly.points[i];
                    Vector2 originalB = poly.points[( i + 1 ) % poly.points.Length];

                    Vector2 proj = (Vector2)Vector3.Project(( point - a ), ( b - a )) + a;

                    if(SegmentsCross(point, proj, originalA, originalB) && PointIsValid(proj)) possiblePoints.Add(proj);

                    float dist = ( point - inflatedPoints[i] ).sqrMagnitude;
                    if(dist < closerVertexDist && PointIsValid(inflatedPoints[i])) 
                    {
                        closerVertexDist = dist;
                        closerVertex = inflatedPoints[i];
                    }
                }
            }

            possiblePoints.Add(closerVertex);

            var closerDist = Mathf.Infinity;
            var index = 0;
            for(int i = 0; i < possiblePoints.Count; i++) 
            {
                var dist = (point - possiblePoints[i]).sqrMagnitude;
                if (dist < closerDist) 
                {
                    closerDist = dist;
                    index = i;
                }
            }
            
            Debug.DrawLine(point, possiblePoints[index]);
            return possiblePoints[index];
        }
        
        public class PolyMap
        {
            public readonly Polygon[] masterPolygons;
            public readonly Polygon[] obstaclePolygons;
            public Polygon[] AllPolygons { get; private set; }

            public PolyMap(Polygon[] masterPolys, params Polygon[] obstaclePolys) 
            {
                masterPolygons = masterPolys;
                obstaclePolygons = obstaclePolys;
                var temp = new List<Polygon>();
                temp.AddRange(masterPolys);
                temp.AddRange(obstaclePolys);
                AllPolygons = temp.ToArray();
            }

            public PolyMap(Polygon masterPoly, params Polygon[] obstaclePolys) 
            {
                masterPolygons = new[] { masterPoly };
                obstaclePolygons = obstaclePolys;
                var temp = new List<Polygon> { masterPoly };
                temp.AddRange(obstaclePolys);
                AllPolygons = temp.ToArray();
            }
        }
        
        public struct Polygon
        {
            public readonly Vector2[] points;
            public Polygon(Vector2[] points)
            {
                this.points = points;
            }
        }
        
        public class PathNode : IHeapItem<PathNode>
        {
            public Vector2 pos;
            public readonly List<int> links;
            public float gCost;
            public float hCost;
            public PathNode parent;

            public PathNode(Vector2 pos) 
            {
                this.pos = pos;
                links = new List<int>();
                gCost = 1f; 
                hCost = 0f;
                parent = null;
            }

            public float FCost => gCost + hCost;

            int IHeapItem<PathNode>.HeapIndex { get; set; }

            int IComparable<PathNode>.CompareTo(PathNode other) 
            {
                int compare = FCost.CompareTo(other.FCost);
                if(compare == 0) compare = hCost.CompareTo(other.hCost);
                return -compare;
            }
        }
        
        private struct PathRequest
        {
            public Vector2 start;
            public readonly Vector2 end;
            public readonly Action<Vector2[]> callback;

            public PathRequest(Vector2 start, Vector2 end, Action<Vector2[]> callback) 
            {
                this.start = start;
                this.end = end;
                this.callback = callback;
            }
        }
        
        // ================================= Editor Specific =================================
        
#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if(!Application.isPlaying) 
            {
                _navObstacles = FindObjectsOfType<NavObstacle>().Where(o => obstaclesMask == ( obstaclesMask | 1 << o.gameObject.layer )).ToList();
                CreatePolyMap(true);
            }

            if (_masterCollider is PolygonCollider2D polygonCollider2D)
            {
                for(int i = 0; i < polygonCollider2D.pathCount; ++i) 
                {
                    var points = polygonCollider2D.GetPath(i).ToArray();
                    points = TransformPoints(ref points, polygonCollider2D.transform);
                    
                    foreach (var unused in points)
                    {
                        DebugDrawPolygon(points, Color.green);
                    }
                }
            } 
            else if(_masterCollider is BoxCollider2D boxCollider2D)
            {
                var offset = boxCollider2D.offset;
                var size = boxCollider2D.size;
                
                var tl = offset + new Vector2(-size.x, size.y) / 2;
                var tr = offset + new Vector2(size.x, size.y) / 2;
                var br = offset + new Vector2(size.x, -size.y) / 2;
                var bl = offset + new Vector2(-size.x, -size.y) / 2;
                
                var points = new[] { tl, tr, br, bl };
                points = TransformPoints(ref points, boxCollider2D.transform);
                DebugDrawPolygon(points, Color.green);
            }
            
            foreach (var obstacle in _navObstacles.Where(obstacle => obstacle != null))
            {
                for(var i = 0; i < obstacle.GetPathCount(); i++) 
                {
                    var points = obstacle.GetPathPoints(i);
                    points = TransformPoints(ref points, obstacle.transform);
                    DebugDrawPolygon(points, Color.green);
                }
            }
            
            if(_map != null) 
            {
                foreach (Polygon pathPoly in _map.masterPolygons) 
                {
                    DebugDrawPolygon(pathPoly.points, Color.grey);
                }

                foreach(Polygon poly in _map.obstaclePolygons) 
                {
                    DebugDrawPolygon(poly.points, Color.grey);
                }
            }
            
        }

        private static void DebugDrawPolygon(IReadOnlyList<Vector2> points, Color color) 
        {
            for(int i = 0; i < points.Count; i++) 
            {
                Gizmos.color = color;
                Gizmos.DrawLine(points[i], points[(i + 1) % points.Count]);
                Gizmos.color = Color.white;
            }
        }

        private void Reset() {
            if(_masterCollider == null) 
            {
                _masterCollider = gameObject.AddComponent<PolygonCollider2D>();
            }
            _masterCollider.enabled = false;
        }

        [UnityEditor.MenuItem("Navigation/Create Nav Map", false, 600)]
        public static void Create() {
            var newNav = new GameObject("NavMap").AddComponent<NavMap>();
            UnityEditor.Selection.activeObject = newNav;
        }

        [UnityEditor.MenuItem("Navigation/Create Nav Obstacle", false, 600)]
        public static void CreatePolyNavObstacle() {
            var obs = new GameObject("NavObstacle").AddComponent<NavObstacle>();
            UnityEditor.Selection.activeObject = obs;
        }
        
#endif
    }
}
