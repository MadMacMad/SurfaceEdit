using System;

namespace Tilify
{
    [Flags]
    public enum TextureChannel
    {
        Unknown     = 1,
        Albedo      = 2,
        Normal      = 4,
        Roughness   = 8,
        Metallic    = 16,
        Height      = 32,
        Mask        = 64,
        User1       = 128,
        User2       = 256,
        User3       = 512,
        User4       = 1024,
        User5       = 2048,
        User6       = 4096,
        User7       = 8192,
        User8       = 16384
    }
}
