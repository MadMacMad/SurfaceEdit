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
    public sealed class TextureResolution : ObjectChangedNotifier
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

        public void IncreaseResolution ()
        {
            var currentIndex = (int)AsEnum;
            var nextIndex = currentIndex * 2;
            if ( nextIndex <= (int)TextureResolutionEnum.x8192 )
            {
                var newResolution = (TextureResolutionEnum)nextIndex;
                SetResolution (newResolution);
            }
        }
        public void DecreaseResolution()
        {
            var currentResolutionIndex = (int)AsEnum;
            var previousIndex = currentResolutionIndex / 2;
            if ( previousIndex >= (int)TextureResolutionEnum.x2 )
            {
                var newResolution = (TextureResolutionEnum)previousIndex;
                SetResolution (newResolution);
            }
        }

        private void SetResolution(TextureResolutionEnum resolution)
        {
            AsEnum = resolution;
            AsVector = TextureResolutionEnumToVector (resolution);
            AsInt = (int)resolution;
            NotifyChanged ();
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
