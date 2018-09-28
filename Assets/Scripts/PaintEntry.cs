using System;
using System.Collections.Generic;
using SurfaceEdit.Brushes;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class PaintEntry : IDisposable
    {
        public readonly BrushSnapshot brushSnapshot;
        
        public IReadOnlyCollection<Vector3> BrushPositions { get; private set; }
        private List<Vector3> brushPositions;

        public Mesh Mesh => mesh.Value;
        private Lazy<Mesh> mesh;

        public event Action<List<Vector3>> BrushPositionAdded;

        public PaintEntry(BrushSnapshot brushSnapshot, List<Vector3> brushPositions)
        {
            Assert.ArgumentNotNull (brushSnapshot, nameof (brushSnapshot));
            Assert.ArgumentNotNull (brushPositions, nameof (brushPositions));
            Assert.ArgumentTrue (brushPositions.Count >= 1, nameof (brushPositions) + ".Lenght is less then 1");

            this.brushSnapshot = brushSnapshot;
            this.brushPositions = brushPositions;

            BrushPositions = this.brushPositions.AsReadOnly ();

            mesh = new Lazy<Mesh> (() => ConstructMesh ());
        }

        public void AddBrushPositions(List<Vector3> brushPositionsToAdd)
        {
            Assert.ArgumentNotNull (brushPositionsToAdd, nameof (brushPositionsToAdd));

            if ( brushPositionsToAdd.Count == 0 )
                return;

            brushPositions.AddRange (brushPositionsToAdd);

            GameObject.Destroy (mesh.Value);
            mesh = new Lazy<Mesh> (() => ConstructMesh ());

            BrushPositionAdded?.Invoke (brushPositionsToAdd);
        }

        private Mesh ConstructMesh()
        {
            var mesh = new Mesh ();

            var indices = new int[brushPositions.Count];
            for ( int i = 0; i < indices.Length; i++ )
                indices[i] = i;

            mesh.SetVertices (brushPositions);
            mesh.bounds = new Bounds (Vector3.zero, new Vector3 (10000, 10000)); // To disable camera frustrum culling
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
    public sealed class PaintEntryAddition
    {
        public IReadOnlyCollection<Vector3> NewBrushPosition { get; private set; }

        public PaintEntryAddition (List<Vector3> newBrushPosition)
        {
            NewBrushPosition = newBrushPosition.AsReadOnly();
        }
    }
}
