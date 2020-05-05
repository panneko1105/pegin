using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpriteJigsaw
{
    public class SpriteJigsaw : MonoBehaviour
    {
        private static readonly int MainTexProperty = Shader.PropertyToID("_MainTex");

        [SerializeField] private bool attachRigidbody2D = true;

        private Sprite sourceSprite;
        private Material spriteMaterial;
        private Mesh shapeMesh;
        private Vector2[] shapeOutlinePathPositions;
        private PolygonCollider2D shapeCollider;

        private float sum = 0f;

        private List<Vector2> vew = new List<Vector2>();
        private List<Vector3> vew2 = new List<Vector3>();
        private bool push;

        public GameObject gameobject;


        // このオブジェクトをワールド座標系のパスで切断し、新しいオブジェクトを返す
        // 切断に成功した場合、このオブジェクトは破壊される
        public List<SpriteJigsaw> Cut(IList<Vector2> cuttingPathPositions)
        {
            Debug.Log($"Cut {this.name}...");
            Debug.Assert(cuttingPathPositions != null);
            Debug.Assert(this.shapeCollider != null);

            // 始点・終点の点検
            var cuttingPathSigns = GetPathCornerSigns(cuttingPathPositions, this.shapeCollider);
            if (cuttingPathSigns.First())
            {
                Debug.LogError("First point is inside collider!");
                return null;
            }
            if (cuttingPathSigns.Last())
            {
                Debug.LogError("Last point is inside collider!");
                return null;
            }

            // 自己交差の点検
            var cuttingPathCornerCount = cuttingPathPositions.Count;
            var cuttingPath = Enumerable.Range(0, cuttingPathCornerCount).ToList();
            var cuttingPathEdges = CreateEdgeListFromPath(cuttingPath, false);
            if (CheckWhetherPathHasSelfIntersection(cuttingPathEdges, cuttingPathPositions))
            {
                Debug.LogError("Path has self intersections!");
                return null;
            }

            // 切断パスをローカル空間に移す
            var worldToLocal = this.transform.worldToLocalMatrix;
            var cuttingPathLocalPositions = cuttingPathPositions.Select(p => (Vector2)worldToLocal.MultiplyPoint3x4(p)).ToList();

            // 切断後のアウトラインを求める
            var cutOutlines = CreateCutOutlines(
                this.shapeOutlinePathPositions,
                cuttingPathLocalPositions,
                cuttingPathSigns);


            // 新しいオブジェクトを作り...
            var result = cutOutlines.Select(
                (loop, i) =>
                {
                    var newName = $"{this.name}-{i}";
                    var loopArray = loop.ToArray();
                    var indices = new Triangulator(loopArray).Triangulate().Select(j => (ushort)j).ToArray();
                    var newSprite = Instantiate(this.sourceSprite);
                    var spriteSize = newSprite.rect.size;
                    var spritePivot = newSprite.pivot;
                    var spritePixelsPerUnit = new Vector2(newSprite.pixelsPerUnit, newSprite.pixelsPerUnit);
                    var spriteSpaceLoopArray = loopArray.Select(
                        p => Vector2.Min(
                            Vector2.Max((p * spritePixelsPerUnit) + spritePivot, Vector2.zero),
                            spriteSize)).ToArray();
                    newSprite.OverrideGeometry(spriteSpaceLoopArray, indices);
                    var newMesh = new Mesh
                    {
                        name = newName,
                        vertices = newSprite.vertices.Select(v => (Vector3)v).ToArray(),
                        uv = newSprite.uv,
                        triangles = newSprite.triangles.Select(j => (int)j).ToArray()
                    };
                    var newObject = Instantiate(this);
                    newObject.name = newName;
                    newObject.sourceSprite = newSprite;
                    newObject.shapeMesh = newMesh;
                    var newMeshFilter = newObject.GetComponent<MeshFilter>();
                    newMeshFilter.sharedMesh = newMesh;
                    var newRenderer = newObject.GetComponent<MeshRenderer>();
                    var materialProperties = new MaterialPropertyBlock();
                    newRenderer.GetPropertyBlock(materialProperties);
                    materialProperties.SetTexture(MainTexProperty, newSprite.texture);
                    newRenderer.SetPropertyBlock(materialProperties);
                    var newCollider = newObject.GetComponent<PolygonCollider2D>();
                    newCollider.points = loopArray;
                    newObject.shapeCollider = newCollider;
                    newObject.shapeOutlinePathPositions = loopArray;
                    return newObject;
                }).ToList();

            // 自分自身は破壊する
            Destroy(this.gameObject);
            return result;
        }

        // 切断後のアウトラインを表す頂点座標ループ群を作る
        private static List<List<Vector2>> CreateCutOutlines(
            IList<Vector2> shapeOutlinePathPositions,
            IList<Vector2> cuttingPathLocalPositions,
            IList<bool> cuttingPathSigns)
        {
            // アウトラインパス、切断パスをともにVertexチェーンに変換する
            var shapeOutlineFirstVertex = CreateVerticesFromPath(shapeOutlinePathPositions, true);
            var cuttingPathFirstVertex = CreateVerticesFromPath(cuttingPathLocalPositions, false, cuttingPathSigns);

            // 後で頂点を総ざらいするときのため、パス作成に関与している頂点をこれに覚えておく
            var vertexBag = new HashSet<Vertex>();
            {
                var intersectionsForVertex = new Dictionary<Vertex, List<Intersection>>();

                // 切断パスをたどっていき...
                var cuttingVertex = cuttingPathFirstVertex;
                while (cuttingVertex.To != null)
                {
                    vertexBag.Add(cuttingVertex);
                    var intersections = new List<Intersection>();
                    intersectionsForVertex.Add(cuttingVertex, intersections);

                    // アウトラインパスをたどっていき...
                    var outlineVertex = shapeOutlineFirstVertex;
                    do
                    {
                        vertexBag.Add(outlineVertex);
                        if (Intersection.CreateIntersection(cuttingVertex, outlineVertex, out var intersection))
                        {
                            // 交点が見つかれば、生成された交点情報を覚えておく
                            // このとき同時にアウトラインパスに切れ目が挿入されている
                            intersections.Add(intersection);
                            vertexBag.Add(intersection.OutlineIn);
                            vertexBag.Add(intersection.OutlineOut);
                            outlineVertex = intersection.OutlineOut;
                        }

                        outlineVertex = outlineVertex.To;
                    } while (outlineVertex != shapeOutlineFirstVertex);

                    cuttingVertex = cuttingVertex.To;
                }

                // アウトラインパスに一通り切れ目を入れた後で、もう一度切断パスをたどっていき...
                cuttingVertex = cuttingPathFirstVertex;
                while (cuttingVertex.To != null)
                {
                    var segmentTerminalVertex = cuttingVertex.To;
                    var intersections = intersectionsForVertex[cuttingVertex];
                    var intersectionCount = intersections.Count;
                    var currentVertexSign = cuttingVertex.Sign == Vertex.VertexSign.Inside;
                    var nextVertexSign = cuttingVertex.To.Sign == Vertex.VertexSign.Inside;

                    // この頂点と同じ位置に頂点を追加してペアを作り...
                    var otherCuttingVertex = new Vertex(cuttingVertex.Position);
                    vertexBag.Add(otherCuttingVertex);
                    cuttingVertex.Other = otherCuttingVertex;
                    otherCuttingVertex.Other = cuttingVertex;

                    // 頂点が形状の内側かを調べ...
                    if (currentVertexSign)
                    {
                        // 内側なら、起点が形状内であることを許さないルールにより
                        // 必ず一つ前の頂点が存在するはず
                        Debug.Assert(cuttingVertex.From != null);

                        // 内側なら自身の双子頂点と一つ前の双子頂点の間に逆向きの接続を作る
                        cuttingVertex.From.Other.From = otherCuttingVertex;
                        otherCuttingVertex.To = cuttingVertex.From.Other;
                    }
                    else
                    {
                        // 外側なら、ここまでの処理の過程でこの頂点よりも前の頂点との接続は切ってあるはず
                        Debug.Assert(cuttingVertex.From == null);

                        // 次の頂点への接続は不要なので切断、この頂点ペアは廃棄する
                        cuttingVertex.To.From = null;
                        cuttingVertex.To = null;
                        vertexBag.Remove(cuttingVertex);
                        vertexBag.Remove(otherCuttingVertex);
                    }

                    if (intersectionCount <= 0)
                    {
                        // この頂点から次の頂点まで交点がないなら、両頂点の内外符号は同じはず
                        Debug.Assert(currentVertexSign == nextVertexSign);
                    }
                    else
                    {
                        // この頂点から次の頂点までの交点が偶数なら両頂点の内外符号は同じ、奇数なら異なるはず
                        Debug.Assert((intersectionCount % 2) == 0 == currentVertexSign == nextVertexSign);

                        // 起点に近い順に交点情報を並べて...
                        intersections = intersections.OrderBy(i => i.T).ToList();

                        // 頂点をつなぎ変えていく
                        var intersectionPointerVertex = cuttingVertex;
                        var sign = currentVertexSign;
                        for (var i = 0; i < intersectionCount; i++)
                        {
                            var intersection = intersections[i];
                            if (sign)
                            {
                                // 交差部分の双子頂点に対して、出る側と入る側を接続する
                                intersectionPointerVertex.To = intersection.OutlineOut;
                                intersection.OutlineOut.From = intersectionPointerVertex;
                                intersectionPointerVertex.Other.From = intersection.OutlineIn;
                                intersection.OutlineIn.To = intersectionPointerVertex.Other;
                            }

                            // 次の交差点へ
                            intersectionPointerVertex = intersection.OutlineIn;
                            intersectionPointerVertex.Other = intersection.OutlineOut;
                            sign = !sign;
                        }

                        // 最後の交差点とセグメント末端を接続する
                        Debug.Assert(sign == nextVertexSign);
                        if (sign)
                        {
                            intersectionPointerVertex.To = segmentTerminalVertex;
                            segmentTerminalVertex.From = intersectionPointerVertex;
                        }
                        else
                        {
                            segmentTerminalVertex.From = null;
                        }
                    }

                    cuttingVertex = segmentTerminalVertex;
                }
            }

            // 出来上がった頂点群から新しいアウトラインを作る
            var result = new List<List<Vector2>>();
            while (vertexBag.Count > 0)
            {
                // 頂点を一つ取り出し...
                var firstVertex = vertexBag.First();
                vertexBag.Remove(firstVertex);
                if (firstVertex.To == firstVertex)
                {
                    continue;
                }

                var loop = new List<Vector2>();
                var vertex = firstVertex;
                var nextVertex = vertex.To;
                do
                {
                    // 頂点をたどって輪を作る
                    Debug.Assert(vertex.From != null);
                    Debug.Assert(vertex.To != null);
                    loop.Add(vertex.Position);
                    Debug.Assert((vertex == firstVertex) || vertexBag.Contains(vertex));
                    vertexBag.Remove(vertex);
                    vertex = nextVertex;
                    nextVertex = vertex.To;
                } while (vertex != firstVertex);

                result.Add(loop);
                
            }


            return result;
        }

        private void Start()
        {
            if (this.shapeMesh != null)
            {
                return;
            }

            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                return;
            }

            this.sourceSprite = spriteRenderer.sprite;
            if (this.sourceSprite == null)
            {
                return;
            }

            // SpriteRendererをMeshRendererに差し替える
            this.shapeMesh = new Mesh
            {
                name = this.name,
                vertices = this.sourceSprite.vertices.Select(v => (Vector3)v).ToArray(),
                uv = this.sourceSprite.uv,
                triangles = this.sourceSprite.triangles.Select(i => (int)i).ToArray()
            };
            this.spriteMaterial = spriteRenderer.sharedMaterial;
            DestroyImmediate(spriteRenderer);
            var meshFilter = this.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = this.shapeMesh;
            var meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = this.spriteMaterial;
            var materialProperties = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(materialProperties);
            materialProperties.SetTexture(MainTexProperty, this.sourceSprite.texture);
            meshRenderer.SetPropertyBlock(materialProperties);

            // メッシュの外周を求め、その形のPolygonCollider2Dを付ける
            this.shapeCollider = this.gameObject.AddComponent<PolygonCollider2D>();
            var meshOutlines = GetOutlines(this.shapeMesh);
            Debug.Assert(meshOutlines.Length == 1);
            var meshOutline = meshOutlines[0];
            var meshVertices = this.shapeMesh.vertices;
            this.shapeOutlinePathPositions = meshOutline.Select(i => (Vector2)meshVertices[i]).ToArray();
            this.shapeCollider.points = this.shapeOutlinePathPositions;


            // 必要に応じてRigidbody2Dも付ける
            if (this.attachRigidbody2D)
            {
                this.gameObject.AddComponent<Rigidbody2D>();
            }

        }

        void Update()
        {
            bool fg = Input.GetMouseButtonDown(0);


            if (fg)
            {
                sum = 0f;
                push = true;
            }

            else
            {
                if (push)
                {
                    float mouse_x_delta = Input.GetAxis("Mouse X");
                    float mouse_y_delta = Input.GetAxis("Mouse Y");

                    sum += Mathf.Abs(mouse_x_delta) + Mathf.Abs(mouse_y_delta);

                    if (sum > 1.5f)
                    {
                        sum = 0f;
                        // Vector3でマウス位置座標を取得する
                        Vector3 position = Input.mousePosition;
                        // Z軸修正
                        position.z = 10f;
                        // マウス位置座標をスクリーン座標からワールド座標に変換する
                        Vector3 screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(position);

                        vew.Add(new Vector2(screenToWorldPointPosition.x, screenToWorldPointPosition.y));
                    }
                }
            }


            if (Input.GetMouseButtonUp(0))
            {
               Cut(vew);
               //vew.clear();
               push = false;

                //for (int i = 0; i < composite.GetPathCount(); ++i)
                //{
                //    int pathPointCount = composite.GetPathPointCount(i);
                //    Vector2[] points = new Vector2[pathPointCount];
                //    col.GetPath(i, points);
                //    vew2.Add(points);
                //}

                //gameobject.GetComponent<DrawMesh>().CreateMesh(vew2);
                //vew2.Clear();
            }

            if (Input.GetKeyDown("return"))
            {
                Mesh myMesh = GetComponent<MeshFilter>().mesh;
                for (int i = 0; i < myMesh.vertices.Length; i++)
                {
                    Debug.Log(myMesh.vertices[i]);
                }
            }
        }

        // パスに自己交差部分があるかを検査し、交差が見つかればtrueを返す
        private static bool CheckWhetherPathHasSelfIntersection(IList<Edge> pathEdges, IList<Vector2> pathPositions)
        {
            Debug.Assert(pathEdges != null);
            return pathEdges.Where((e0, i) => pathEdges.Skip(i + 2).Any(e1 => GetIntersection(e0, e1, pathPositions, out _, out _))).Any();
        }

        // パス座標を基に、連結リスト状につながったVertex群を作る
        // 入力される座標リストは連結順に並んでいることが前提
        private static Vertex CreateVerticesFromPath(
            IList<Vector2> sortedPathPositions,
            bool closed,
            IList<bool> signs = null)
        {
            Debug.Assert(sortedPathPositions != null);
            var firstV = new Vertex(sortedPathPositions.First());
            var vertexList = new List<Vertex>();
            var v = firstV;
            vertexList.Add(v);
            foreach (var p in sortedPathPositions.Skip(1))
            {
                var nextV = new Vertex(p);
                v.To = nextV;
                nextV.From = v;
                v = nextV;
                vertexList.Add(v);
            }

            if (closed)
            {
                firstV.From = v;
                v.To = firstV;
            }

            if (signs != null)
            {
                Debug.Assert(signs.Count == vertexList.Count);
                var count = signs.Count;
                for (var i = 0; i < count; i++)
                {
                    vertexList[i].Sign = signs[i] ? Vertex.VertexSign.Inside : Vertex.VertexSign.Outside;
                }
            }


            return firstV;
        }

        // Edgeを引数とするGetIntersection
        private static bool GetIntersection(Edge e0, Edge e1, IList<Vector2> pathPositions, out float t0, out float t1)
        {
            Debug.Assert(pathPositions != null);
            return GetIntersection(pathPositions[e0.From], pathPositions[e0.To], pathPositions[e1.From], pathPositions[e1.To], out t0, out t1);
        }

        // 二つの線分の交点の、線分上の内分点を得る
        // t0、t1がいずれも0以上1以下ならtrueを返す
        // 線分が平行な場合は交差なしと見なしfalseを返す
        private static bool GetIntersection(
            Vector2 p00,
            Vector2 p01,
            Vector2 p10,
            Vector2 p11,
            out float t0,
            out float t1)
        {
            var vc = p10 - p00;
            var v0 = p01 - p00;
            var v1 = p11 - p10;
            var cc = CrossZ(v0, v1);
            if (Mathf.Approximately(cc, 0.0f))
            {
                t0 = 0.0f;
                t1 = 0.0f;
                return false;
            }

            var c0 = CrossZ(vc, v0);
            var c1 = CrossZ(vc, v1);
            t0 = c1 / cc;
            t1 = c0 / cc;
            return (t0 >= 0.0f) && (t0 <= 1.0f) && (t1 >= 0.0f) && (t1 <= 1.0f);
        }

        // a、bを3次元ベクトルと見なし、外積のZ成分を返す
        private static float CrossZ(Vector2 a, Vector2 b)
        {
            return (a.x * b.y) - (a.y * b.x);
        }

        // パス上の各点が形状の内側にあるかを調べて返す
        private static List<bool> GetPathCornerSigns(IList<Vector2> pathPositions, Collider2D collider)
        {
            Debug.Assert(pathPositions != null);
            Debug.Assert(collider != null);
            return pathPositions.Select(collider.OverlapPoint).ToList();
        }

        // パスのインデックスリストから辺のリストを得る
        private static List<Edge> CreateEdgeListFromPath(IList<int> path, bool closed)
        {
            Debug.Assert(path != null);
            var pointCount = path.Count;
            var points = closed ? path : path.Take(pointCount - 1);
            return points.Select((index, i) => new Edge(index, path[(i + 1) % pointCount])).ToList();
        }

        // メッシュ境界をたどる頂点のインデックスを得る経路の配列を得る
        // 外周は時計回り、内周は反時計回り
        private static int[][] GetOutlines(Mesh mesh)
        {
            Debug.Assert(mesh != null);
            var indices = mesh.triangles;
            var triangleCount = indices.Length / 3;
            var edges = Enumerable.Range(0, triangleCount).SelectMany(
                i =>
                {
                    var o = i * 3;
                    return Enumerable.Range(0, 3).Select(j => new Edge(indices[o + j], indices[o + ((j + 1) % 3)]));
                }).ToList();
            var outlineEdges = edges.Where(e => !edges.Contains(e.Inverse())).ToList();
            var result = new List<int[]>();
            while (outlineEdges.Any())
            {
                var outlineIndices = new List<int>();
                var e0 = outlineEdges.First();
                var firstIndex = e0.From;
                var e1 = outlineEdges.First(e => e.From == e0.To);
                while (true)
                {
                    outlineIndices.Add(e0.From);
                    outlineEdges.Remove(e0);
                    e0 = e1;
                    if (e0.To == firstIndex)
                    {
                        outlineIndices.Add(e0.From);
                        outlineEdges.Remove(e0);
                        break;
                    }

                    e1 = outlineEdges.First(e => e.From == e0.To);
                }

                result.Add(outlineIndices.ToArray());
            }

            return result.ToArray();
        }

        private struct Edge
        {
            public readonly int From;
            public readonly int To;

            public Edge(int from, int to)
            {
                this.From = from;
                this.To = to;
            }

            public Edge Inverse()
            {
                return new Edge(this.To, this.From);
            }
        }

        private class Vertex
        {
            public enum VertexSign
            {
                None,
                Inside,
                Outside
            }

            public readonly Vector2 Position;
            public VertexSign Sign;
            public Vertex From;
            public Vertex To;
            public Vertex Other;

            public Vertex(Vector2 position)
            {
                this.Position = position;
                this.Sign = VertexSign.None;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{(this.From == null ? "null" : this.From.Position.ToString())}->[{(this.Sign == VertexSign.Inside ? "+" : this.Sign == VertexSign.Outside ? "-" : "0")}]{this.Position.ToString()}->{(this.To == null ? "null" : this.To.Position.ToString())}";
            }
        }

        private struct Intersection
        {
            public readonly Vertex OutlineIn;
            public readonly Vertex OutlineOut;
            public readonly float T;

            private Intersection(Vertex outlineIn, Vertex outlineOut, float t)
            {
                this.OutlineIn = outlineIn;
                this.OutlineOut = outlineOut;
                this.T = t;
            }

            public static bool CreateIntersection(Vertex pathFrom, Vertex outlineFrom, out Intersection intersection)
            {
                var p00 = pathFrom.Position;
                var p01 = pathFrom.To.Position;
                var p10 = outlineFrom.Position;
                var p11 = outlineFrom.To.Position;
                if (GetIntersection(p00, p01, p10, p11, out var t0, out _))
                {
                    var position = Vector2.Lerp(p00, p01, t0);
                    var outlineIn = new Vertex(position);
                    var outlineOut = new Vertex(position);
                    outlineIn.From = outlineFrom;
                    outlineIn.To = outlineOut;
                    outlineOut.From = outlineIn;
                    outlineOut.To = outlineFrom.To;
                    outlineFrom.To.From = outlineOut;
                    outlineFrom.To = outlineIn;
                    intersection = new Intersection(outlineIn, outlineOut, t0);
                    return true;
                }
                intersection = new Intersection();
                return false;
            }
        }
    }

    // runevisionさんによるTriangulator(http://wiki.unity3d.com/index.php/Triangulator)
    public class Triangulator
    {
        private readonly List<Vector2> m_points = new List<Vector2>();

        public Triangulator(Vector2[] points)
        {
            this.m_points = new List<Vector2>(points);
        }

        public int[] Triangulate()
        {
            var indices = new List<int>();
            var n = this.m_points.Count;
            if (n < 3)
            {
                return indices.ToArray();
            }
            var V = new int[n];
            if (this.Area() > 0)
            {
                for (var v = 0; v < n; v++)
                {
                    V[v] = v;
                }
            }
            else
            {
                for (var v = 0; v < n; v++)
                {
                    V[v] = n - 1 - v;
                }
            }
            var nv = n;
            var count = 2 * nv;
            for (var v = nv - 1; nv > 2;)
            {
                if (count-- <= 0)
                {
                    return indices.ToArray();
                }
                var u = v;
                if (nv <= u)
                {
                    u = 0;
                }
                v = u + 1;
                if (nv <= v)
                {
                    v = 0;
                }
                var w = v + 1;
                if (nv <= w)
                {
                    w = 0;
                }
                if (this.Snip(u, v, w, nv, V))
                {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);
                    for (s = v, t = v + 1; t < nv; s++, t++)
                    {
                        V[s] = V[t];
                    }
                    nv--;
                    count = 2 * nv;
                }
            }
            indices.Reverse();
            return indices.ToArray();
        }

        private float Area()
        {
            var n = this.m_points.Count;
            var A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                var pval = this.m_points[p];
                var qval = this.m_points[q];
                A += (pval.x * qval.y) - (qval.x * pval.y);
            }
            return A * 0.5f;
        }

        private bool Snip(int u, int v, int w, int n, int[] V)
        {
            int p;
            var A = this.m_points[V[u]];
            var B = this.m_points[V[v]];
            var C = this.m_points[V[w]];
            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            {
                return false;
            }
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                {
                    continue;
                }
                var P = this.m_points[V[p]];
                if (this.InsideTriangle(A, B, C, P))
                {
                    return false;
                }
            }
            return true;
        }

        private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;
            ax = C.x - B.x;
            ay = C.y - B.y;
            bx = A.x - C.x;
            by = A.y - C.y;
            cx = B.x - A.x;
            cy = B.y - A.y;
            apx = P.x - A.x;
            apy = P.y - A.y;
            bpx = P.x - B.x;
            bpy = P.y - B.y;
            cpx = P.x - C.x;
            cpy = P.y - C.y;
            aCROSSbp = (ax * bpy) - (ay * bpx);
            cCROSSap = (cx * apy) - (cy * apx);
            bCROSScp = (bx * cpy) - (by * cpx);
            return (aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f);
        }
    }
}