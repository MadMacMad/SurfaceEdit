using System;

namespace Tilify
{
    [Flags]
    public enum TextureChannel
    {
        Unknown     = 0,
        Albedo      = 1,
        Normal      = 2,
        Roughness   = 4,
        Metallic    = 8,
        Height      = 16,
        Mask        = 32,
        User1       = 64,
        User2       = 128,
        User3       = 256,
        User4       = 512,
        User5       = 1024,
        User6       = 2048,
        User7       = 4096,
        User8       = 8192
    }
}
