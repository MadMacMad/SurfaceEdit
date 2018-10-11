using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class ResourceManager
    {
        public ApplicationContext Context { get; private set; }

        public event Action<Resource> ResourceAdded;
        public event Action<Resource> ResourceDeleted;

        public IReadOnlyCollection<Resource> Resources { get; private set; }
        private List<Resource> resources = new List<Resource>();

        public ResourceManager(ApplicationContext context)
        {
            Assert.ArgumentNotNull (context, nameof (context));

            Context = context;
            Resources = resources.AsReadOnly ();

            Setup ();
        }
        
        public List<T> GetResources<T> (string name) where T : Resource
        {
            Assert.ArgumentNotNullOrEmptry (name, nameof (name));

            return resources.Where (r => r.Metadata.Name == name && r.GetType() == typeof(T)).Cast<T>().ToList ();
        }
        public ResourceImportResult ImportResource(string path)
        {
            var result = ImportResource_Internal (path);
            if (result.IsSuccessfull)
                Register (result.Resource);
            return result;
        }
        
        private void Setup ()
        {
            if ( !Directory.Exists (Context.ResourcesCacheDirectory) )
                return;

            foreach ( var directory in Directory.GetDirectories (Context.ResourcesCacheDirectory) )
            {
                if ( TryLoadResourceMetadata (directory, out var metadata) )
                {
                    if ( metadata.ResourceType == ResourceType.Texture2D )
                        Register (Texture2DResource.Existing (metadata));
                }
                else
                {
                    Directory.Delete (directory, true);
                }
            }
        }
        private void Register(Resource resource)
        {
            resources.Add (resource);
            ResourceAdded?.Invoke (resource);
            resource.Deleted += OnResourceDeleted;
        }
        private void OnResourceDeleted(Resource resource)
        {
            resource.Deleted -= OnResourceDeleted;
            resources.Remove (resource);
            ResourceDeleted?.Invoke (resource);
        }
        private ResourceImportResult ImportResource_Internal(string path)
        {
            if ( string.IsNullOrEmpty (path) )
                return new ResourceImportResult (false, "Path is null or emptry");

            if ( !Path.HasExtension (path) )
                return new ResourceImportResult (false, $"Path({path}) does not has any extension");

            if ( !File.Exists (path) )
                return new ResourceImportResult (false, $"File at ({path}) does not exists");

            try
            {
                var extension = Path.GetExtension (path);
                var name = Path.GetFileNameWithoutExtension (path);

                if ( extension == ".png" || extension == ".jpeg" || extension == ".jpg" || extension == ".tga" )
                {
                    var texture = TextureUtility.LoadTexture2DFromDisk (path);

                    if ( texture.width != texture.height )
                    {
                        var result = new ResourceImportResult (false, $"Texture({path}) is not square ({texture.width}, {texture.height})");
                        GameObject.DestroyImmediate (texture);
                        return result;
                    }

                    if ( !Mathf.IsPowerOfTwo (texture.width) )
                    {
                        var result = new ResourceImportResult (false, $"Texture({path}) has not power of 2 size ({texture.width}, {texture.height})");
                        GameObject.DestroyImmediate (texture);
                        return result;
                    }

                    var textureMinSize = 256;

                    if ( texture.width < textureMinSize )
                    {
                        var result = new ResourceImportResult (false, $"Texture({path}) is too small({texture.width}, {texture.height})). Min size is {textureMinSize}");
                        GameObject.DestroyImmediate (texture);
                        return result;
                    }

                    var metadata = new ResourceMetadata (Context, name, Context.CacheTextureResolution, ResourceType.Texture2D);

                    var resource = Texture2DResource.New (metadata, texture);
                    return new ResourceImportResult (true, "", resource);
                }
                else
                {
                    return new ResourceImportResult (false, $"Extension {extension} is not supported.");
                }
            }
            catch ( Exception e )
            {
                return new ResourceImportResult (false, "An exception was thrown when importing a resource! " + e.Message);
            }
        }
        private bool TryLoadResourceMetadata (string directory, out ResourceMetadata resourceMetadata)
        {
            resourceMetadata = null;

            if ( Directory.GetFiles (directory).Length == 0 )
                return false;

            var metadataPath = Path.Combine (directory, "metadata.json");

            if ( !File.Exists (metadataPath) )
                return false;

            var resourceTypeString = "";
            var guidString = "";
            var name = "";
            var textureExtensionString = "";

            try
            {
                var json = File.ReadAllText (metadataPath);

                var jsonMetadata = JObject.Parse (json);

                resourceTypeString = jsonMetadata["ResourceType"].ToString();
                name = jsonMetadata["Name"].ToString ();
                guidString = jsonMetadata["Guid"].ToString();
                textureExtensionString = jsonMetadata["TextureExtension"].ToString();
            }
            catch(Exception e)
            {
                var a = e;
                return false; }

            if ( string.IsNullOrEmpty (resourceTypeString) ||
                 string.IsNullOrEmpty (guidString) ||
                 string.IsNullOrEmpty (name) ||
                 string.IsNullOrEmpty (textureExtensionString) )
                return false;

            if ( !Enum.TryParse<ResourceType> (resourceTypeString, out var resourceType) )
                return false;

            if ( !Guid.TryParse (guidString, out var guid) )
                return false;

            if ( !Enum.TryParse<TextureExtension> (textureExtensionString, out var textureExtension) )
                return false;

            if ( new DirectoryInfo (directory).Name != name + "_" + guidString )
                return false;

            resourceMetadata = new ResourceMetadata (Context, name, guid, textureExtension, resourceType);

            return true;
        }
    }
    public sealed class ResourceImportResult
    {
        public readonly Resource Resource;
        public readonly bool IsSuccessfull;
        public readonly string ErrorMessage;

        public ResourceImportResult (bool isSuccessfull, string errorMessage, Resource resource = null)
        {
            IsSuccessfull = isSuccessfull;
            Resource = resource;
            ErrorMessage = errorMessage;
        }
    }
}
