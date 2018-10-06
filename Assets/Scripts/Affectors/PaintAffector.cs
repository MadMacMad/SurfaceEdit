using System.Collections.Generic;
using System.Linq;
using SurfaceEdit.Commands;
using UnityEngine;

namespace SurfaceEdit.Affectors
{
    public class PaintAffector : Affector
    {
        private static readonly float distanceBetweenBrushes = .00001f;

        private RendererStation rendererStation;

        private PaintEntry temporaryPaintEntry;

        public PaintAffector(ApplicationContext context, Channels affectedChannels) : base (context, affectedChannels)
        {
            rendererStation = new RendererStation (Settings.rendererStationLayerID);
        }

        public void PaintTemporary(PaintEntry entry) => Paint_Internal (entry, true);
        public void PaintFinal (PaintEntry entry) => Paint_Internal (entry, false);

        private void Paint_Internal(PaintEntry entry, bool isTemporary)
        {
            Assert.ArgumentNotNull (entry, nameof (entry));

            var material = entry.brushSnapshot.material;

            var go = new GameObject ("PaintEntry");

            var renderer = go.AddComponent<MeshRenderer> ();
            renderer.material = material;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            renderer.receiveShadows = false;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.allowOcclusionWhenDynamic = false;

            var meshFilter = go.AddComponent<MeshFilter> ();
            meshFilter.mesh = entry.Mesh;

            var objectWidth = entry.BrushPositions.Count * distanceBetweenBrushes;

            if ( isTemporary )
            {
                rendererStation.UseItTemporary (go, objectWidth);
                var chunksToRender = CalculateChunksToRender (entry.brushSnapshot.percentageSize, entry.BrushPositions);
                var renderContext = CalculateRenderContext (chunksToRender);
                NotifyNeedRender (renderContext);

                entry.BrushPositionAdded += l =>
                {
                    meshFilter.mesh = entry.Mesh;
                    var newChunksToRender = CalculateChunksToRender (entry.brushSnapshot.percentageSize, l);
                    var newRenderContext = CalculateRenderContext (newChunksToRender);
                    NotifyNeedRender (newRenderContext);
                };
            }
            else
            {
                var chunksToRender = CalculateChunksToRender (entry.brushSnapshot.percentageSize, entry.BrushPositions);
                var renderContext = CalculateRenderContext (chunksToRender);
                var command = new PaintCommand (rendererStation, go, objectWidth, () => NotifyNeedRender(renderContext));
                undoRedoManager.Do (command);
            }
        }

        private RenderContext CalculateRenderContext (ChunksToRender chunksToRender)
        {
            if ( AffectedChannels.List.Contains (Channel.Mask) )
                return new RenderContext (Context.Channels.ToImmutable (), RenderCovering.Part, chunksToRender);
            else
                return new RenderContext (AffectedChannels.ToImmutable (), RenderCovering.Part, chunksToRender);
        }

        private ChunksToRender CalculateChunksToRender(Vector2 brushPercentageSize, IEnumerable<Vector3> brushPositions)
        {
            var result = new ChunksToRender (Context);
            
            var chunkResolution = Context.ChunkResolution.AsInt;
            var textureResolution = Context.TextureResolution.AsInt;

            var isMultiTile = brushPercentageSize.x > chunkResolution / (float)textureResolution;

            foreach (var position in brushPositions)
            {
                var bottomLeft  = new Vector2Int ((int)(position.x                           * textureResolution / chunkResolution ),
                                                  (int)(position.y                           * textureResolution / chunkResolution ));

                var bottomRight = new Vector2Int ((int)((position.x + brushPercentageSize.x) * textureResolution / chunkResolution ),
                                                  (int)( position.y                          * textureResolution / chunkResolution ));

                var topLeft     = new Vector2Int ((int)( position.x                          * textureResolution / chunkResolution),
                                                  (int)((position.y + brushPercentageSize.y) * textureResolution / chunkResolution ));

                var topRight    = new Vector2Int ((int)((position.x + brushPercentageSize.x) * textureResolution / chunkResolution ),
                                                  (int)((position.y + brushPercentageSize.y) * textureResolution / chunkResolution ));

                if ( isMultiTile )
                {
                    for ( int x = bottomLeft.x; x <= topRight.x; x++ )
                        for ( int y = bottomLeft.y; y <= topRight.y; y++ )
                            result.AddChunkPosition (new Vector2Int (x, y));
                }
                else
                {
                    result.AddChunkPosition (bottomLeft);
                    result.AddChunkPosition (bottomRight);
                    result.AddChunkPosition (topLeft);
                    result.AddChunkPosition (topRight);
                }
            }

            return result;
        }

        public void Reset()
        {
            rendererStation.Reset ();
        }
        
        public override void Dispose ()
        {
            rendererStation.Dispose ();
        }

        protected override void Affect (ProviderTexture texture, Vector2Int pixelPosition, Vector2Int pixelSize)
            => rendererStation.Render (texture, pixelPosition, pixelSize);
        
    }
}
