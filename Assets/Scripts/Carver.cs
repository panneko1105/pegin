using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using UnityEngine;
using UnityEngine.Rendering;




public class Carver : MonoBehaviour
{
    private const float Precision = 1024.0f;
    
    private static readonly Clipper clipper = new Clipper(Clipper.ioStrictlySimple);
   
    [SerializeField] private bool attachRigidbodyOnCreateCollider;
    [SerializeField] private bool makeColliderTriggerOnCreateCollider;
    [SerializeField] private bool notflame;
    [SerializeField] private bool maskBox;
    [SerializeField] private bool GoalObj;
    private readonly List<List<IntPoint>> outlines = new List<List<IntPoint>>();
    private Mesh pathMesh;
    private (Renderer, (Material, int)[])[] renderers = new (Renderer, (Material, int)[])[0];
    public PolygonCollider2D Collider2D { get; private set; }

    public Material _material;
    public Mesh comn;

    [SerializeField] private Material Flamematerial;

    private List<Vector3> v = new List<Vector3>();

    private void Start()
    {
        this.FitColliderIntoMeshes();
        if (notflame)
        {
            this.GetComponent<Renderer>().sharedMaterial = _material;
        }
         Destroy(transform.parent.GetComponent<MeshFilter>());
         Destroy(transform.parent.GetComponent<MeshRenderer>());

        Mesh mesh2 = Instantiate(comn);
        
        GetComponent<MeshFilter>().sharedMesh = mesh2;
    }

    public void Change()
    {
        this.GetComponent<Renderer>().sharedMaterial = _material;
    }

    public Material GetMaterial()
    {
        return Flamematerial;
    }
   

    // PolygonCollider2Dを現在の3Dモデルの見た目に合わせて更新する
    // まずStartで一度実行されるが、後で再度実行すれば欠損部分が復活することになる
    // その他、モデルの三次元的な回転などによりモデルのアウトラインが変化した
    // 場合にもこのメソッドでアウトラインを更新するべきだが、実行速度は
    // 大して考慮していないため、あまり頻繁に行うのはおすすめできない
    // なお、モデルの「Read/Write Enabled」をオンにしておかないと失敗すると思われる
    public void FitColliderIntoMeshes()
    {
        
        // 3Dオブジェクトを使っているということなのでオブジェクトが三次元的に
        // 回転している可能性があるが、2Dコライダーはtransform.forwardが
        // ワールドZ+を向いていた方が好都合なため、親オブジェクトを追加して
        // そこにコライダーを付けることにした
        // 併せてそれにアタッチされるRenderingHelperが、CommandBufferを使って
        // 独自にレンダリングを行うことになる
        if (this.Collider2D == null)
        {
            this.Collider2D = this.CreateCollider();
        }

        // MeshFilter、またはSkinnedMeshRendererからメッシュを集めて
        // ワールドZ方向に押し潰し、三角形の集合を得てコライダーの原型とする
        // 裏向きの三角形も向きを反転した上で列挙しているが、ポリゴンの裏面は
        // 考慮しなくてもいいのなら、裏は除外してもいいかもしれない
        //this.renderers = this.GetComponentsInChildren<Renderer>().Select(
        //    r => (r, r.materials.Select((m, i) => (m, i)).OrderBy(pair => pair.m.renderQueue).ToArray())).ToArray();
        var meshes = this.GatherMeshes();
        var triangles = this.GetTrianglesFromMesh(meshes);
        DeleteMeshes(meshes);

        // Clipperを使ってアウトラインを作る
        // Clipperは右手系の慣習に従うようなので、入力三角形は巡回方向を逆転させて反時計回りを表とする
        // また、計算上のロバスト性のためClipperは座標を整数として扱うそうなので、まずUnity上の座標値を
        // Precision倍したものをClipperに与え、得られたアウトラインからコライダー形状をセットする際には
        // 逆にPrecisionで割るようにした
        clipper.Clear();
        var sourcePaths = GetPathsFromTriangles(triangles, Precision);
        clipper.AddPaths(sourcePaths, PolyType.ptSubject, true);
       
        if (clipper.Execute(ClipType.ctUnion, this.outlines, PolyFillType.pftPositive))
        {
            // また、アウトライン生成後にTriangulatorを使ってアウトラインをメッシュ化しておく
            // これは見た目をくり抜くためのマスクとして使われる
            // まずpathMeshを新しいoutlinesに合わせて更新、引き続きコライダーオブジェクトに
            // メッシュレンダラーを付け、それにpathMeshをセットしてマスキングを行わせる
            this.UpdatePathMesh();
            this.UpdateMask();
        }
        else
        {
            Debug.LogError($"Path generation for {this.name} failed.");
        }
    }

    // Clipperを使ってオブジェクトをくり抜く
    public static void Carve(Carver subject, IEnumerable<Carver> withCarvers)
    {
        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        if (withCarvers == null)
        {
            throw new ArgumentNullException(nameof(withCarvers));
        }

        var solution = new List<List<IntPoint>>();
        clipper.Clear();
        clipper.AddPaths(subject.outlines, PolyType.ptSubject, true);
        var worldToSubject = subject.Collider2D.transform.worldToLocalMatrix;

        void TransformPath(List<List<IntPoint>> input, List<List<IntPoint>> output, Matrix4x4 matrix)
        {
            output.Clear();
            output.AddRange(
                input.Select(
                    path => path.Select(
                        point =>
                        {
                            var p = matrix.MultiplyPoint3x4(new Vector3(point.X / Precision, point.Y / Precision, 0.0f));
                            return new IntPoint(p.x * Precision, p.y * Precision);
                        }).ToList()));
        }

        var transformedPath = new List<List<IntPoint>>();
        foreach (var withCarver in withCarvers.Where(c => (c != null) && (c != subject)))
        {
            TransformPath(
                withCarver.outlines,
                transformedPath,
                worldToSubject * withCarver.Collider2D.transform.localToWorldMatrix);
            clipper.AddPaths(transformedPath, PolyType.ptClip, true);
        }

        if (clipper.Execute(ClipType.ctDifference, solution, PolyFillType.pftNonZero))
        {
            subject.outlines.Clear();
            subject.outlines.AddRange(solution);
            subject.UpdatePathMesh();
            subject.UpdateMask();
        }
        else
        {
            Debug.LogError($"Path generation for {subject.name} failed.");
        }
    }


    private void UpdatePathMesh()
    {
        var pathCount = this.outlines.Count;
        this.Collider2D.pathCount = pathCount;
        if (this.pathMesh != null)
        {
            Destroy(this.pathMesh);
        }

        var points = this.outlines.Select(
            path => (Clipper.Orientation(path) ? ((IEnumerable<IntPoint>)path).Reverse() : path)
                .Select(p => new Vector2(p.X / Precision, p.Y / Precision)).ToArray()
        ).ToArray();
        for (var i = 0; i < pathCount; i++)
        {
            this.Collider2D.SetPath(i, points[i]);
        }

        var pathMeshVertices = points.SelectMany(path => path).Select(p => (Vector3)p).ToArray();
        var pathMeshIndexOffsets = new int[pathCount];
        for (var i = 1; i < pathCount; i++)
        {
            pathMeshIndexOffsets[i] = pathMeshIndexOffsets[i - 1] + this.outlines[i - 1].Count;
        }

        var pathMeshIndices = points.Zip(pathMeshIndexOffsets, (path, indexOffset) => (path, indexOffset))
            .SelectMany(
                pathAndOffset => new Triangulator(pathAndOffset.path).Triangulate()
                    .Select(i => i + pathAndOffset.indexOffset)).ToArray();
        var mesh = new Mesh
        {
            vertices = pathMeshVertices,
            triangles = pathMeshIndices
        };
        mesh.RecalculateBounds();
        this.pathMesh = mesh;
    }

    private void UpdateMask()
    {
        var meshFilter = this.Collider2D.GetComponent<MeshFilter>();
        var meshRenderer = this.Collider2D.GetComponent<MeshRenderer>();
        if (this.pathMesh == null)
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
        else
        {
            if (meshFilter == null)
            {
                meshFilter = this.Collider2D.gameObject.AddComponent<MeshFilter>();
            }

            if (meshRenderer == null)
            {
                meshRenderer = this.Collider2D.gameObject.AddComponent<MeshRenderer>();
            }

            meshFilter.sharedMesh = this.pathMesh;
            meshRenderer.enabled = true;
        }
    }

    private static List<List<IntPoint>> GetPathsFromTriangles(Triangle[] triangles, float precision)
    {
        return triangles.Select(
                t =>
                {
                    return t.Vertices.Reverse().Select(p => new IntPoint(p.x * precision, p.y * precision))
                        .ToList();
                })
            .ToList();
    }

   
    private static void DeleteMeshes((Transform, Mesh)[] meshes)
    {
        foreach (var (_, mesh) in meshes)
        {
            Destroy(mesh);
        }
    }

    private static IEnumerable<(Transform, Vector3)> EnumerateScaledTransformsInParent(Transform fromTransform)
    {
        do
        {
            var localScale = fromTransform.localScale;
            if (localScale != Vector3.one)
            {
                yield return (fromTransform, localScale);
            }

            fromTransform = fromTransform.parent;
        } while (fromTransform != null);
    }

    // 自身の階層下からコライダーの原型とするメッシュを収集する
    // オリジナルのメッシュをそのまま返すのではなく、念のため複製を返すことにした
    // SkinnedMeshRendererについてもBakeMeshで現在の形をキャプチャーして返すようにしたが、
    // BakeMeshはモデルが拡大縮小されていると正しく見た目通りの形をキャプチャーしてくれないようで
    // 苦肉の策として一時的になるべくスケールが等倍になるようにしてからキャプチャーし
    // 後で元に戻すことにした
    private (Transform, Mesh)[] GatherMeshes()
    {
        var meshFilters = this.GetComponentsInChildren<MeshFilter>();
        var skinnedMeshRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderers.Select(smr => smr.transform.lossyScale).Distinct().Skip(1).Any())
        {
            Debug.LogWarning($"{this.gameObject.name} has complexly scaled transform hierarchy! It may cause wrong mesh generation.");
        }

        var scaledTransforms = EnumerateScaledTransformsInParent(this.transform).ToArray();
        foreach (var (scaledTransform, _) in scaledTransforms)
        {
            scaledTransform.localScale = Vector3.one;
        }

        var result = meshFilters.Select(mf => (mf.transform, Instantiate(mf.sharedMesh))).Concat(
            skinnedMeshRenderers.Select(
                smr =>
                {
                    var mesh = new Mesh();
                    smr.BakeMesh(mesh);
                    return (smr.transform, mesh);
                })).ToArray();
        foreach (var (scaledTransform, localScale) in scaledTransforms)
        {
            scaledTransform.localScale = localScale;
        }

        return result;
    }

    // メッシュの頂点をワールド空間に移し、Z方向に圧縮したものをローカル空間に戻し、Triangle配列として返す
    private Triangle[] GetTrianglesFromMesh((Transform, Mesh)[] meshes)
    {
        var worldToLocal = this.Collider2D.transform.worldToLocalMatrix;
        var worldZ = this.Collider2D.transform.position.z;
        return meshes.SelectMany(
            pair =>
            {
                var (t, m) = pair;
                var localToWorld = t.localToWorldMatrix;
                var vertices = m.vertices;
                var indices = m.triangles;
                return Enumerable.Range(0, indices.Length / 3).Select(
                    i =>
                    {
                        var j = i * 3;
                        var pw0 = localToWorld.MultiplyPoint3x4(vertices[indices[j]]);
                        var pw1 = localToWorld.MultiplyPoint3x4(vertices[indices[j + 1]]);
                        var pw2 = localToWorld.MultiplyPoint3x4(vertices[indices[j + 2]]);
                        var crossZ = CrossZ(pw1 - pw0, pw2 - pw0);
                        return (pw0, pw1, pw2, crossZ);
                    }).Where(face => Mathf.Abs(face.crossZ) > 0.0f).Select(
                    face =>
                    {
                        face.pw0.z = worldZ;
                        face.pw1.z = worldZ;
                        face.pw2.z = worldZ;
                        if (face.crossZ < 0.0f)
                        {
                            return new Triangle
                            {
                                Vertex0 = worldToLocal.MultiplyPoint3x4(face.pw0),
                                Vertex1 = worldToLocal.MultiplyPoint3x4(face.pw1),
                                Vertex2 = worldToLocal.MultiplyPoint3x4(face.pw2)
                            };
                        }

                        return new Triangle
                        {
                            Vertex0 = worldToLocal.MultiplyPoint3x4(face.pw0),
                            Vertex1 = worldToLocal.MultiplyPoint3x4(face.pw2),
                            Vertex2 = worldToLocal.MultiplyPoint3x4(face.pw1)
                        };
                    });
            }).ToArray();
    }

    private static float CrossZ(Vector3 lhs, Vector3 rhs)
    {
        return (lhs.x * rhs.y) - (lhs.y * rhs.x);
    }

    // このオブジェクトに親オブジェクトを作ってPolygonCollider2Dを取り付け、
    // さらに描画処理を担当するRenderingHelperもアタッチする
    private PolygonCollider2D CreateCollider()
    {
        var siblingIndex = this.transform.GetSiblingIndex();
        var colliderObject = new GameObject(this.gameObject.name);
        colliderObject.transform.SetParent(this.transform.parent, false);
        colliderObject.transform.SetSiblingIndex(siblingIndex);
        colliderObject.transform.position = this.transform.position;
        colliderObject.transform.rotation = Quaternion.identity;
        this.transform.SetParent(colliderObject.transform);
        
        var collider = colliderObject.AddComponent<PolygonCollider2D>();

        //通常ブロックの場合
        if (this.attachRigidbodyOnCreateCollider)
        {
            //タグの設定（親に）
            colliderObject.tag = "block";
            var rb=colliderObject.AddComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            rb.mass = 100f;
        }
        if (this.maskBox)
        {
            //フレームの場合生成前に当たらないように
            colliderObject.tag = "Mask";
            collider.isTrigger = true;
            //レイヤーをblockに変更
            colliderObject.layer = 13;
            colliderObject.AddComponent<maskBoxMove>();
        }
        //フレームの場合
        if (this.makeColliderTriggerOnCreateCollider)
        {
            //フレームの場合生成前に当たらないように
            colliderObject.tag = "flame";
            colliderObject.AddComponent<FlameMove>();
            var dr = colliderObject.AddComponent<DrawMesh>();
            
            collider.isTrigger = this.makeColliderTriggerOnCreateCollider;
            //レイヤーをblockに変更
            colliderObject.layer = 14;
        }
        if (this.GoalObj)
        {
            colliderObject.tag = "Goal";
            colliderObject.AddComponent<Goal>();
            var dr = colliderObject.AddComponent<DrawMesh>();
            Debug.Log("生成したよ");
        }
        
        return collider;
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


    private struct Triangle
    {
        public Vector3 Vertex0;
        public Vector3 Vertex1;
        public Vector3 Vertex2;

        public IEnumerable<Vector3> Vertices
        {
            get
            {
                yield return this.Vertex0;
                yield return this.Vertex1;
                yield return this.Vertex2;
            }
        }
    }
}

