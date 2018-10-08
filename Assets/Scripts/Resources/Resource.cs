using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class Resource : IDisposable
    {
        [JsonIgnore]
        public ApplicationContext Context { get; private set; }

        public string Name { get; private set; }
        public string Guid { get; private set; }
        [JsonIgnore]
        public abstract Texture2D PreviewTexture { get; protected set; }

        [JsonIgnore]
        private string cacheDirectory;

        private string type;

        public Resource (string name, string cacheDirectory, ApplicationContext context)
        {
            Assert.ArgumentNotNullOrEmptry (name, nameof (name));
            Assert.ArgumentNotNullOrEmptry (cacheDirectory, nameof (cacheDirectory));
            Assert.ArgumentNotNull (context, nameof (context));

            Name = name;
            Context = context;
            Guid = System.Guid.NewGuid ().ToString ();

            this.cacheDirectory = Path.Combine(cacheDirectory, $"{Name}_{Guid}");

            type = GetType ().Name;
        }

        public virtual void Dispose ()
        {
            GameObject.DestroyImmediate (PreviewTexture);
        }

        protected void Cache()
        {
            try
            {
                if ( !Directory.Exists (cacheDirectory) )
                    Directory.CreateDirectory (cacheDirectory);

                var json = JsonConvert.SerializeObject (this);

                var jsonPath = Path.Combine (cacheDirectory, "resource.json");

                File.WriteAllText (jsonPath, json);

                var previewTexturePath = Path.Combine (cacheDirectory, "previewTexture.tga");
                TextureUtility.SaveTexture2DToDisk (previewTexturePath, PreviewTexture);

                Cache_Child (cacheDirectory);
            }
            catch (Exception e)
            {
                Debug.LogError (e.Message);
            }
        }
        protected abstract void Cache_Child (string cacheDirectory);
    }
}
