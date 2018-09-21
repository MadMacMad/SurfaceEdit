using System;
using System.Collections.Generic;
using Tilify.Brushes;
using UnityEngine;

namespace Tilify
{
    public sealed class PaintEntry : IDisposable
    {
        public readonly BrushSnapshot brushSnapshot;

        public IReadOnlyList<Vector3> BrushPositions => brushPositions.AsReadOnly ();
        private List<Vector3> brushPositions;

        public Mesh Mesh => mesh.Value;
        private Lazy<Mesh> mesh;

        public PaintEntry(BrushSnapshot brushSnapshot, List<Vector3> brushPositions)
        {
            Assert.ArgumentNotNull (brushSnapshot, nameof (brushSnapshot));
            Assert.ArgumentNotNull (brushPositions, nameof (brushPositions));
            Assert.ArgumentTrue (brushPositions.Count >= 1, nameof (brushPositions) + ".Lenght is less then 1");

            this.brushSnapshot = brushSnapshot;
            this.brushPositions = brushPositions;

            mesh = new Lazy<Mesh> (() => ConstructMesh ());
        }

        private Mesh ConstructMesh()
        {
            var mesh = new Mesh ();

            var indices = new int[brushPositions.Count];
            for ( int i = 0; i < indices.Length; i++ )
                indices[i] = i;

            mesh.SetVertices (brushPositions);
            mesh.SetIndices (indices, MeshTopology.Points, 0, false);

            return mesh;
        }

        public void Dispose()
        {
            if (mesh.IsValueCreated)
                GameObject.Destroy (mesh.Value);

            brushSnapshot.Dispose ();
        }
    }
}
