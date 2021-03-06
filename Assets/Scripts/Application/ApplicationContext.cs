﻿using System.IO;
using UnityEngine;

namespace SurfaceEdit
{
    public class ApplicationContext : ObjectChangedNotifier
    {
        public UndoRedoManager UndoRedoManager { get; private set; }
        public Channels Channels { get; private set; }
        public TextureResolution TextureResolution { get; private set; }
        public ImmutableTextureResolution ChunkResolution { get; private set; }
        public Vector2Int ChunksCountVector { get; private set; }
        public int ChunksCountInt { get; private set; }

        public string CacheDirectory { get; private set; }
        public string ResourcesCacheDirectory { get; private set; }
        public TextureExtension CacheTextureResolution { get; private set; }
        public string CacheTextureResolutionString { get; private set; }

        private ImmutableTextureResolution initialChunkResolution;

        public ApplicationContext (
            UndoRedoManager undoRedoManager,
            Channels channels,
            TextureResolution textureResolution,
            ImmutableTextureResolution chunkResolution,
            string cacheDirectory,
            TextureExtension cacheTextureExtension)
        {
            Assert.ArgumentNotNull (undoRedoManager, nameof (undoRedoManager));
            Assert.ArgumentNotNull (channels, nameof (channels));
            Assert.ArgumentNotNull (textureResolution, nameof (textureResolution));
            Assert.ArgumentNotNull (chunkResolution, nameof (chunkResolution));
            Assert.ArgumentNotNullOrEmptry (cacheDirectory, nameof (cacheDirectory));

            UndoRedoManager = undoRedoManager;
            Channels = channels;
            TextureResolution = textureResolution;

            CacheDirectory = cacheDirectory;
            ResourcesCacheDirectory = Path.Combine (CacheDirectory, "Resources");
            CacheTextureResolution = cacheTextureExtension;
            CacheTextureResolutionString = CacheTextureResolution.ToString ();

            initialChunkResolution = new ImmutableTextureResolution(chunkResolution.AsEnum);

            CalculateChunkProperties ();

            Channels.Changed += (s, e) => NotifyChanged ();
            TextureResolution.Changed += (s, e) =>
            {
                CalculateChunkProperties ();
                NotifyChanged ();
            };
        }
        private void CalculateChunkProperties()
        {
            if ( initialChunkResolution.AsInt > TextureResolution.AsInt )
                ChunkResolution = new ImmutableTextureResolution (TextureResolution.AsEnum);
            else
                ChunkResolution = new ImmutableTextureResolution (initialChunkResolution.AsEnum);

            ChunksCountInt = TextureResolution.AsInt / ChunkResolution.AsInt;
            ChunksCountVector = new Vector2Int (ChunksCountInt, ChunksCountInt);
        }
    }
}
