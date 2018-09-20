using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilify.Brushes;
using UnityEngine;

namespace Tilify
{ 
    public sealed class PaintEntry
    {
        public readonly BrushSnapshot brushSnapshot;

        public IReadOnlyCollection<Vector2> BrushPositions => Array.AsReadOnly(brushPositions);
        private Vector2[] brushPositions;

        public PaintEntry(BrushSnapshot brushSnapshot, Vector2[] brushPositions)
        {
            Assert.ArgumentNotNull (brushSnapshot, nameof (brushSnapshot));
            Assert.ArgumentNotNull (brushPositions, nameof (brushPositions));
            Assert.ArgumentTrue (brushPositions.Length >= 1, nameof (brushPositions) + ".Lenght is less then 1");

            this.brushSnapshot = brushSnapshot;
            this.brushPositions = brushPositions;
        }
    }
}
