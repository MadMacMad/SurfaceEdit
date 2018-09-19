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
        public bool NeedPaint { get; private set; }

        public readonly BrushSnapshot brushSnapshot;

        public IReadOnlyList<Vector2> BrushPositions => brushPositions;
        private List<Vector2> brushPositions;

        public PaintEntry(BrushSnapshot brushSnapshot, Vector2 lastPercentagePosition, Vector2 newPercentagePosition)
        {
            lastPercentagePosition.Clamp01 ();
            newPercentagePosition.Clamp01 ();
            this.brushSnapshot = brushSnapshot;

            var maxBrushPercentageSizeDimension = Math.Max (brushSnapshot.percentageSize.x, brushSnapshot.percentageSize.y);

            brushPositions = LineSegmentDivider.DivideLineSegmentIntoPoints (
                lastPercentagePosition, newPercentagePosition,
                brushSnapshot.intervals * maxBrushPercentageSizeDimension);

            NeedPaint = brushPositions.Count != 0;
        }
    }
}
