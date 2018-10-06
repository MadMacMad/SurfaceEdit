using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFB;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class SResources : Singleton<SResources>
    {
        public static readonly ExtensionFilter SupportedExtensions = new ExtensionFilter("Supported Extensions", "png");

        public event Action<SResource> ResourceAdded;
        public event Action<SResource> ResourceDeleted;

        public IReadOnlyCollection<SResource> Resources { get; private set; }
        private List<SResource> resources = new List<SResource> ();

        private SResources()
        {
            Resources = resources.AsReadOnly ();
        }

        public void DeleteResource (SResource resource)
        {
            if ( resource == null )
                return;

            if ( !resources.Contains (resource) )
                return;

            resources.Remove (resource);
            ResourceDeleted?.Invoke (resource);
            resource.Dispose ();
        }

        public SResourceLoadResult TryImport<SResourceType> (string pathToFile, out SResourceType resource) where SResourceType : SResource
        {
            resource = null;

            var result = TryImport_Internal (pathToFile, out SResource internalResource);

            if ( !result.IsSuccessfull )
                return result;

            if ( internalResource is SResourceType )
            {
                resources.Add (internalResource);
                ResourceAdded?.Invoke (internalResource);
                resource = internalResource as SResourceType;
            }
            else
                internalResource?.Dispose ();

            return result;
        }
        public SResourceLoadResult TryImport (string pathToFile, out SResource resource)
        {
            var result = TryImport_Internal (pathToFile, out resource);

            if ( result.IsSuccessfull )
            {
                resources.Add (resource);
                ResourceAdded?.Invoke (resource);
            }

            return result;
        }
        private SResourceLoadResult TryImport_Internal (string pathToFile, out SResource resource)
        {
            resource = null;

            if ( string.IsNullOrEmpty (pathToFile) )
                return new SResourceLoadResult(false, "Given path is null or emptry");

            if ( !Path.HasExtension (pathToFile) )
                return new SResourceLoadResult (false, "Given path does not has any extension");

            if ( !File.Exists (pathToFile) )
                return new SResourceLoadResult (false, "File with given path does not exists");

            try
            {
                var extension = Path.GetExtension (pathToFile);
                var name = Path.GetFileNameWithoutExtension (pathToFile);
                
                switch ( extension )
                {
                    case ".png":
                        var texture = TextureIOHelper.LoadTexture2DFromDisk (pathToFile);

                        if ( texture.width != texture.height )
                        {
                            var result = new SResourceLoadResult (false, $"Selected texture({pathToFile}) is not square");
                            GameObject.DestroyImmediate (texture);
                            return result;
                        }

                        if ( !Mathf.IsPowerOfTwo (texture.width) )
                        {
                            var result = new SResourceLoadResult (false, $"Selected texture({pathToFile}) has not power of 2 size");
                            GameObject.DestroyImmediate (texture);
                            return result;
                        }

                        var textureMinSize = 256;

                        if ( texture.width < textureMinSize )
                        {
                            var result = new SResourceLoadResult (false, $"Selected texture({pathToFile}) is too small({texture.width}). Min size is {textureMinSize}");
                            GameObject.DestroyImmediate (texture);
                            return result;
                        }

                        resource = new STexture2DResource (name, texture);
                        break;
                    default:
                        return new SResourceLoadResult (false, "Unsupported extension: " + extension);
                }

                return new SResourceLoadResult (true);
            }
            catch ( Exception e )
            {
                return new SResourceLoadResult (false, "An exception was thrown when loading a resource! " + e.Message);
            }
        }
    }

    public sealed class SResourceLoadResult
    {
        public readonly bool IsSuccessfull;
        public readonly string ErrorMessage;

        public SResourceLoadResult (bool isSuccessfull, string errorMessage = "")
        {
            IsSuccessfull = isSuccessfull;
            ErrorMessage = errorMessage;
        }
    }
}
