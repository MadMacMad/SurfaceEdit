﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class RenderContext
    {
        public ChunksToRender ChunksToRender { get; private set; }
        public ImmutableChannels ChannelsToRender { get; private set; }
        public RenderCovering Covering { get; private set; }

        public RenderContext (ImmutableChannels channelsToRender, RenderCovering renderCovering = RenderCovering.Full, ChunksToRender chunksToRender = null)
        {
            Assert.ArgumentNotNull (channelsToRender, nameof (channelsToRender));

            if ( renderCovering == RenderCovering.Part )
                Assert.ArgumentNotNull (chunksToRender, nameof (chunksToRender));

            ChunksToRender = chunksToRender;
            ChannelsToRender = channelsToRender;
            Covering = renderCovering;
        }
    }

    public sealed class ChunksToRender
    {
        public ProgramContext Context { get; private set; }
        
        public IReadOnlyCollection<Vector2Int> PixelPositions { get; private set; }
        private List<Vector2Int> pixelPositions;

        private List<Vector2Int> chunkPositions;

        private Vector2Int chunkResolution;

        private Vector2 textureResolution;

        public ChunksToRender (ProgramContext context)
        {
            Assert.ArgumentNotNull (context, nameof (context));

            Context = context;

            chunkPositions = new List<Vector2Int> ();

            pixelPositions = new List<Vector2Int> ();
            PixelPositions = pixelPositions.AsReadOnly ();

            chunkResolution = Context.ChunkResolution.AsVector;
            textureResolution = Context.TextureResolution.AsVector;

            context.TextureResolution.Changed += (s, e) =>
            {
                var lastTextureResolution = textureResolution;
                textureResolution = Context.TextureResolution.AsVector;

                var newChunkPositions = new List<Vector2Int> ();

                if ( lastTextureResolution.x < textureResolution.x )
                {
                    foreach ( var position in chunkPositions )
                    {
                        var newPosition = position * 2;
                        newChunkPositions.Add (newPosition);
                        newChunkPositions.Add (new Vector2Int (newPosition.x + 1, newPosition.y));
                        newChunkPositions.Add (new Vector2Int (newPosition.x, newPosition.y + 1));
                        newChunkPositions.Add (new Vector2Int (newPosition.x + 1, newPosition.y + 1));
                    }
                }
                else
                {
                    foreach(var position in chunkPositions)
                    {
                        var newPosition = new Vector2Int(Mathf.FloorToInt(position.x / 2), Mathf.FloorToInt (position.y / 2));
                        if (!newChunkPositions.Contains(newPosition))
                            newChunkPositions.Add (newPosition);
                    }
                }
                
                chunkPositions = newChunkPositions;
                pixelPositions.Clear ();
                foreach (var position in chunkPositions)
                    pixelPositions.Add (position * chunkResolution);
            };
        }
        
        public void AddChunkPosition (Vector2Int chunkPosition)
        {
            if ( chunkPositions.Contains (chunkPosition) )
                return;
            if ( chunkPosition.x < 0 || chunkPosition.y < 0 || chunkPosition.x >= Context.ChunksCountInt || chunkPosition.y > Context.ChunksCountInt )
                return;

            chunkPositions.Add (chunkPosition);

            pixelPositions.Add (chunkPosition * chunkResolution);
        }
    }
    public enum RenderCovering
    {
        Full,
        Part
    }
}