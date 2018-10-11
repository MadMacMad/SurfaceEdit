using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace SurfaceEdit
{
    public abstract class Resource : IDisposable
    {
        public static readonly Vector2Int ThumbnailSize = new Vector2Int (64, 64);
        
        public ResourceMetadata Metadata { get; private set; }

        /// <summary>
        /// An event that is triggered if everything goes really wrong.
        /// For example, if someone tries to get a texture from Texture2DResource that is not loaded,
        /// Texture2DResource tries to read the texture from disk, but there is no texture on disk!
        /// Thus, the resource is simply useless, because it does not contains a texture by itself and the cache is also empty.
        /// </summary>
        public event Action<Resource> FatalError;
        protected void NotifyFatalError ()
            => FatalError?.Invoke (this);

        public event Action<Resource> Deleted;
        
        public bool IsLoaded { get; private set; } = false;
        
        public Texture2D Thumbnail
        {
            get
            {
                if ( thumbnail == null )
                    thumbnail = LoadOrGenerateThumbnail ();
                return thumbnail;
            }
        }
        private Texture2D thumbnail;

        private List<object> chainKeys = new List<object> ();

        protected Resource (ResourceMetadata metadata)
        {
            Assert.ArgumentNotNull (metadata, nameof (metadata));

            Metadata = metadata;
        }

        public void Chain(object key)
        {
            if (chainKeys.Contains(key))
            {
                Debug.LogWarning ("Resource already chained with that key!");
                return;
            }
            chainKeys.Add (key);

            if ( chainKeys.Count == 1 )
            {
                LoadResource ();
                IsLoaded = true;
            }
        }
        public void UnChain(object key)
        {
            if ( chainKeys.Contains (key) )
                return;

            chainKeys.Remove (key);

            if ( chainKeys.Count == 0 )
            {
                UnloadResource ();
                IsLoaded = false;
            }
        }

        public void DeleteResource ()
        {
            if ( Directory.Exists (Metadata.ResourceCacheDirectory) )
                Directory.Delete (Metadata.ResourceCacheDirectory, true);
            Dispose ();
            Deleted?.Invoke (this);
        }
        public void Dispose ()
        {
            if ( thumbnail != null )
                GameObject.DestroyImmediate (thumbnail);

            UnloadResource ();
        }

        private Texture2D LoadOrGenerateThumbnail()
        {
            var thumbnailTexturePath = Path.Combine (Metadata.ResourceCacheDirectory, "thumbnail." + Metadata.TextureExtension);

            TextureUtility.TryLoadTexture2DFromDisk (thumbnailTexturePath, out var thumbnail);

            if (thumbnail == null)
                return GenerateThumbnail ();

            return thumbnail;
        }
        
        protected void Cache ()
        {
            Debug.Log (new StackTrace ().ToString ());
            try
            {
                if ( Directory.Exists (Metadata.ResourceCacheDirectory) )
                    Directory.Delete (Metadata.ResourceCacheDirectory, true);

                Directory.CreateDirectory (Metadata.ResourceCacheDirectory);

                var metadataJson = JsonConvert.SerializeObject (Metadata, Formatting.Indented);

                var metadataPath = Path.Combine (Metadata.ResourceCacheDirectory, "metadata.json");

                File.WriteAllText (metadataPath, metadataJson);

                var thumbnailPath = Path.Combine (Metadata.ResourceCacheDirectory, "thumbnail." + Metadata.TextureExtensionString);
                TextureUtility.SaveTexture2DToDisk (thumbnailPath, Thumbnail);

                CacheResourceSpecific ();
            }
            catch (Exception e)
            {
                Debug.LogError (e.Message);
            }
        }
        
        protected abstract void CacheResourceSpecific ();
        protected abstract Texture2D GenerateThumbnail ();
        protected abstract void LoadResource ();
        protected abstract void UnloadResource ();
    }

    public sealed class ResourceMetadata
    {
        [JsonIgnore]
        public ApplicationContext Context { get; private set; }
        [JsonIgnore]
        public string ResourceCacheDirectory { get; private set; }
        [JsonIgnore]
        public string TextureExtensionString { get; private set; }

        public string Name { get; private set; }
        public Guid Guid { get; private set; }
        [JsonConverter (typeof (StringEnumConverter))]
        public TextureExtension TextureExtension { get; private set; }
        [JsonConverter (typeof (StringEnumConverter))]
        public ResourceType ResourceType { get; private set; }

        public ResourceMetadata (ApplicationContext context, string name, TextureExtension textureExtension, ResourceType resourceType)
            : this(context, name, Guid.NewGuid(), textureExtension, resourceType) { }
        public ResourceMetadata (ApplicationContext context, string name, Guid guid, TextureExtension textureExtension, ResourceType resourceType)
        {
            Assert.ArgumentNotNull (context, nameof (context));
            Assert.ArgumentNotNullOrEmptry (name, nameof (name));

            Name = name;
            Guid = guid;
            TextureExtension = textureExtension;
            ResourceType = resourceType;

            Context = context;
            ResourceCacheDirectory = Path.Combine (context.ResourcesCacheDirectory, name + "_" + guid.ToString ());
            TextureExtensionString = textureExtension.ToString ();
        }

        public static bool Equals(ResourceMetadata left, ResourceMetadata right)
        {
            if ( left is null && right is null )
                return true;

            if ( left is null && !( right is null ) )
                return false;

            if ( right is null && !( left is null ) )
                return false;

            if ( ReferenceEquals(left, right))
                return true;

            return ReferenceEquals (left.Context, right.Context)
                && left.ResourceCacheDirectory == right.ResourceCacheDirectory
                && left.Name == right.Name
                && left.Guid == right.Guid
                && left.TextureExtension == right.TextureExtension
                && left.ResourceType == right.ResourceType;
        }

        public override bool Equals (object obj)
            => Equals (this, obj as ResourceMetadata);

        public override int GetHashCode ()
            => ( Context, ResourceCacheDirectory, Name, Guid, TextureExtension, ResourceType ).GetHashCode ();

        public static bool operator == (ResourceMetadata left, ResourceMetadata right)
            => Equals (left, right);

        public static bool operator != (ResourceMetadata left, ResourceMetadata right)
           => !Equals (left, right);
    }

    public enum ResourceType
    {
        Texture2D
    }
}
