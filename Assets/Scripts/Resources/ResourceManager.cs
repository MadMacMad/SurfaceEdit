using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class ResourceManager
    {
        public static readonly ExtensionFilter SupportedExtensions = new ExtensionFilter("Supported Extensions", "png");

        public ApplicationContext Context { get; private set; }

        public event Action<Resource> ResourceAdded;
        public event Action<Resource> ResourceDeleted;

        public IReadOnlyCollection<Resource> Resources { get; private set; }
        private List<Resource> resources = new List<Resource> ();

        public ResourceManager(ApplicationContext context)
        {
            Assert.ArgumentNotNull (context, nameof(context));

            Context = context;
            Resources = resources.AsReadOnly ();
        }

        public void DeleteResource (Resource resource)
        {
            if ( resource == null )
                return;

            if ( !resources.Contains (resource) )
                return;

            resources.Remove (resource);
            ResourceDeleted?.Invoke (resource);
            resource.Dispose ();
        }
        
        public void Register(Resource resource)
        {
            Assert.ArgumentNotNull (resource, nameof (resource));

            if ( resources.Contains (resource) )
            {
                Debug.LogWarning ($"{nameof (ResourceManager)} already contains this {nameof (resource)}");
                return;
            }

            Assert.ArgumentTrue (ReferenceEquals (resource.Context, Context),
                                 $"{nameof (resource)}.{nameof (resource.Context)} is not {nameof (ResourceManager)}.{nameof (Context)}");

            resources.Add (resource);
            ResourceAdded?.Invoke (resource);
        }

        public List<SResourceType> GetResources<SResourceType> (string name) where SResourceType : Resource
        {
            Assert.ArgumentNotNullOrEmptry (name, nameof (name));

            return resources.Where (s => s.Name == name).OfType<SResourceType>().ToList();
        }

        public ResourceImportResult TryImport<SResourceType> (string pathToFile, out SResourceType resource) where SResourceType : Resource
        {
            resource = null;

            var result = TryImport_Internal (pathToFile, out Resource internalResource);

            if ( !result.IsSuccessfull )
                return result;

            if ( internalResource is SResourceType )
            {
                resources.Add (internalResource);
                ResourceAdded?.Invoke (internalResource);
                resource = internalResource as SResourceType;
                return result;
            }
            else
            {
                result = new ResourceImportResult (false, $"Resource successfully loaded, but the type of the loaded resource ({internalResource.GetType().Name})  does not match the specified type ({typeof(SResourceType).Name})");
                internalResource?.Dispose ();
                return result;
            }
        }
        public ResourceImportResult TryImport (string pathToFile, out Resource resource)
        {
            var result = TryImport_Internal (pathToFile, out resource);

            if ( result.IsSuccessfull )
            {
                resources.Add (resource);
                ResourceAdded?.Invoke (resource);
            }

            return result;
        }
        private ResourceImportResult TryImport_Internal (string pathToFile, out Resource resource)
        {
            resource = null;

            if ( string.IsNullOrEmpty (pathToFile) )
                return new ResourceImportResult(false, "Given path is null or emptry");

            if ( !Path.HasExtension (pathToFile) )
                return new ResourceImportResult (false, $"Given path({pathToFile}) does not has any extension");

            if ( !File.Exists (pathToFile) )
                return new ResourceImportResult (false, $"File with given path({pathToFile}) does not exists");

            try
            {
                var extension = Path.GetExtension (pathToFile);
                var name = Path.GetFileNameWithoutExtension (pathToFile);
                
                switch ( extension )
                {
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                        var texture = TextureUtility.LoadTexture2DFromDisk (pathToFile);

                        if ( texture.width != texture.height )
                        {
                            var result = new ResourceImportResult (false, $"Selected texture({pathToFile}) is not square");
                            GameObject.DestroyImmediate (texture);
                            return result;
                        }

                        if ( !Mathf.IsPowerOfTwo (texture.width) )
                        {
                            var result = new ResourceImportResult (false, $"Selected texture({pathToFile}) has not power of 2 size");
                            GameObject.DestroyImmediate (texture);
                            return result;
                        }

                        var textureMinSize = 256;

                        if ( texture.width < textureMinSize )
                        {
                            var result = new ResourceImportResult (false, $"Selected texture({pathToFile}) is too small({texture.width}). Min size is {textureMinSize}");
                            GameObject.DestroyImmediate (texture);
                            return result;
                        }

                        resource = new Texture2DResource (name, pathToFile, texture, Context);
                        break;
                    default:
                        return new ResourceImportResult (false, "Unsupported extension: " + extension);
                }

                return new ResourceImportResult (true);
            }
            catch ( Exception e )
            {
                return new ResourceImportResult (false, "An exception was thrown when loading a resource! " + e.Message);
            }
        }
    }

    public sealed class ResourceImportResult
    {
        public readonly bool IsSuccessfull;
        public readonly string ErrorMessage;

        public ResourceImportResult (bool isSuccessfull, string errorMessage = "")
        {
            IsSuccessfull = isSuccessfull;
            ErrorMessage = errorMessage;
        }
    }
}
