using System.IO;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class CacheManager
    {
        public ApplicationContext Context { get; private set; }
        public string CacheDirectory { get; private set; }

        public CacheManager(ApplicationContext context, string cacheDirectory)
        {
            Assert.ArgumentNotNull (context, nameof (context));
            Assert.ArgumentNotNullOrEmptry (cacheDirectory, nameof (cacheDirectory));

            Assert.ArgumentTrue (cacheDirectory.IsDirectory(), nameof(cacheDirectory) + " is not directory!");

            Context = context;
            CacheDirectory = cacheDirectory;

            if ( !Directory.Exists (CacheDirectory) )
                Directory.CreateDirectory (cacheDirectory);
        }

        public void Save(string name, Texture2D texture, TextureExtension extension = TextureExtension.Png)
        {

        }
    }

    public enum TextureExtension
    {
        Png,
        Jpg
    }
}
