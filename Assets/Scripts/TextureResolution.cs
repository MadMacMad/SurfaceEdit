using UnityEngine;

namespace SurfaceEdit
{
    public sealed class ImmutableTextureResolution
    {
        public readonly int AsInt;
        public readonly TextureResolutionEnum AsEnum;
        public readonly Vector2Int AsVector;

        public ImmutableTextureResolution (TextureResolutionEnum resolution)
        {
            AsEnum = resolution;
            AsVector = TextureResolutionEnumToVector (resolution);
            AsInt = (int)resolution;
        }
        private Vector2Int TextureResolutionEnumToVector (TextureResolutionEnum resolution)
            => new Vector2Int ((int)resolution, (int)resolution);
    }
    public sealed class TextureResolution : PropertyChangedNotifier
    {
        public int AsInt { get; private set; }
        public TextureResolutionEnum AsEnum { get; private set; }
        public Vector2Int AsVector { get; private set; }

        public TextureResolution (TextureResolutionEnum resolution)
        {
            AsEnum = resolution;
            AsVector = TextureResolutionEnumToVector (resolution);
            AsInt = (int)resolution;
        }

        public void SetResolution (TextureResolutionEnum resolution)
        {
            if ( resolution != AsEnum )
            {
                AsEnum = resolution;
                AsVector = TextureResolutionEnumToVector (resolution);
                AsInt = (int)resolution;
                NotifyPropertyChanged (nameof (AsVector));
            }
        }

        public ImmutableTextureResolution ToImmutable ()
            => new ImmutableTextureResolution (AsEnum);

        private Vector2Int TextureResolutionEnumToVector (TextureResolutionEnum resolution)
            => new Vector2Int ((int)resolution, (int)resolution);
    }
    public enum TextureResolutionEnum : int
    {
        x2      = 2,
        x4      = 4,
        x8      = 8,
        x16     = 16,
        x32     = 32,
        x64     = 64,
        x128    = 128,
        x256    = 256,
        x512    = 512,
        x1024   = 1024,
        x2048   = 2048,
        x4096   = 4096,
        x8192   = 8192
    }
}
