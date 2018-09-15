﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Surface : ObjectChangedRegistrator
    {
        public Vector2 WorldSize
        {
            get => worldSize;
            set => SetPropertyAndRegisterUndoRedo (v => worldSize = v, () => worldSize, value, true, t => t.ClampBoth (.1f));
        }
        private Vector2 worldSize;

        public IReadOnlyDictionary<TextureChannel, RenderTexture> Textures => textures;
        private Dictionary<TextureChannel, RenderTexture> textures = new Dictionary<TextureChannel, RenderTexture> ();

        private Dictionary<TextureChannel, TextureProvider> providers;

        public Surface(UndoRedoRegister undoRedoRegister, Dictionary<TextureChannel, TextureProvider> textureProviders, Vector2 worldSize) : base(undoRedoRegister)
        {
            providers = textureProviders;
            foreach(var pair in providers )
                textures.Add (pair.Key, pair.Value.Provide ());
        }
          
        public Dictionary<TextureChannel, RenderTexture> SelectTextures(List<TextureChannel> selectionList)
        {
            return selectionList
                .Where (s => textures.ContainsKey (s))
                .ToDictionary (s => s, s => textures[s]);
        }
        public void Reset(List<TextureChannel> selectionList)
        {
            foreach(var s in selectionList)
                providers[s].Override (textures[s]);
        }
        public void ResetAll()
        {
            foreach ( var pair in textures )
                providers[pair.Key].Override (textures[pair.Key]);
        }
    }
}
