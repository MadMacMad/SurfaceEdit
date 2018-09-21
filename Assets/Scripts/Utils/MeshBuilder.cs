using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public static class MeshBuilder
    {
        public static MeshData BuildQuad (Vector2 size)
        {
            size.Clamp (new Vector2 (.1f, .1f), new Vector2 (float.MaxValue, float.MaxValue));

            var vertices = new List<Vector3> ();
            var triangles = new List<int> ();
            var uvs = new List<Vector2> ();

            vertices.Add (new Vector3 (0, 0));
            vertices.Add (new Vector3 (0, size.y));
            vertices.Add (size);
            vertices.Add (new Vector3 (size.x, 0));

            triangles.Add (0);
            triangles.Add (1);
            triangles.Add (2);

            triangles.Add (0);
            triangles.Add (2);
            triangles.Add (3);

            uvs.Add (new Vector2());
            uvs.Add (new Vector2(0, 1));
            uvs.Add (new Vector2(1, 1));
            uvs.Add (new Vector2(1, 0));

            var meshData = new MeshData (vertices, triangles, uvs);
            return meshData;
        }

        public static MeshData BuildPlane (Vector2 size, Vector2Int pointsCount)
        {
            size.Clamp (new Vector2 (.1f, .1f), new Vector2 (float.MaxValue, float.MaxValue));
            pointsCount.Clamp (new Vector2Int (1, 1), new Vector2Int (4096, 4096));

            var distanceBetweenPoints = size / pointsCount;
            var vertices = new List<Vector3> ();
            var triangles = new List<int> ();
            var uvs = new List<Vector2> ();

            for ( int y = 0; y < pointsCount.y; y++ )
                for ( int x = 0; x < pointsCount.x; x++ )
                {
                    vertices.Add (new Vector3 (x * distanceBetweenPoints.x, y * distanceBetweenPoints.y));
                    uvs.Add (new Vector2 (x / (float)pointsCount.x, y / (float)pointsCount.y));
                }

            var offset = pointsCount.x;

            for ( int y = 0; y < pointsCount.y - 1; y++ )
                for ( int x = 0; x < pointsCount.x - 1; x++ )
                {
                    var i = x + y * offset;

                    triangles.Add (i);
                    triangles.Add (i + offset);
                    triangles.Add (i + offset + 1);

                    triangles.Add (i);
                    triangles.Add (i + offset + 1);
                    triangles.Add (i + 1);
                }

            var meshData = new MeshData (vertices, triangles, uvs);
            return meshData;
        }
    }

    public class MeshData
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uvs;

        public MeshData (List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }

        public Mesh ConvertToMesh()
        {
            var mesh = new Mesh ();
            mesh.SetVertices (vertices);
            mesh.SetTriangles (triangles, 0);
            mesh.SetUVs (0, uvs);
            mesh.RecalculateNormals ();
            return mesh;
        }
    }
}
