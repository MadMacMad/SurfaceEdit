using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public static class LineSegmentDivider
    {
        public static List<Vector2> DivideLineSegmentIntoPoints (Vector2 startPosition, Vector2 endPosition, float distanceBetweenPoints)
        {
            if ( startPosition == endPosition )
                return new List<Vector2>() { startPosition };

            if ( distanceBetweenPoints < .01f )
                distanceBetweenPoints = .01f;

            var originVector = endPosition - startPosition;
            var normalizedVector = originVector.normalized;
            var pointOffset = normalizedVector * distanceBetweenPoints;

            var pointsCount = Mathf.FloorToInt(originVector.magnitude / distanceBetweenPoints);
            var result = new List<Vector2> ();

            var offsetPosition = startPosition;

            for(int i = 0; i < pointsCount; i++ )
            {
                result.Add(offsetPosition);
                offsetPosition += pointOffset;
            }

            return result;
        }
    }
}
