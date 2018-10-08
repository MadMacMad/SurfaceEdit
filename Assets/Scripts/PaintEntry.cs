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
            
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            var indices = new int[brushPositions.Count];
            var brushSize = brushSnapshot.percentageSize;
            for ( int i = 0; i < indices.Length; i++ )
            {
                indices[i] = i;
                var brushPos = brushPositions[i];

                var minBrushX = brushPos.x;
                var maxBrushX = minBrushX + brushSize.x;

                var minBrushY = brushPos.y;
                var maxBrushY = minBrushY + brushSize.y;

                if ( minX > minBrushX )
                    minX = minBrushX;
                if ( minY > minBrushY )
                    minY = minBrushY;

                if ( maxX < maxBrushX )
                    maxX = maxBrushX;
                if ( maxY < maxBrushY )
                    maxY= maxBrushY;
            }

            mesh.SetVertices (brushPositions);
            mesh.SetIndices (indices, MeshTopology.Points, 0, false);
            var size = new Vector2 (maxX - minX, maxY - minY);
            mesh.bounds = new Bounds(new Vector3(minX + size.x / 2f, minY + size.y /2f), size);

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
