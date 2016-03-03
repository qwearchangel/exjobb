using System;
using inRiver.Integration.Interface;
using inRiver.Remoting.Objects;
using inRiver.Remoting;
using Exjobb.Shared.Constants;
using inRiver.Remoting.Image;
using System.IO;

namespace Exjobb
{
    public class ResourceHandler : IResourceHandler
    {
        private const string filePath = @"C:\temp\images\";
        private const string fileType = ".jpg";

        public void ExportResource(Entity resource)
        {
            var resourceFile = RemoteManager.UtilityService.GetFile((int)resource.GetField(Resource.FileIdFieldId).Data, ImageConfiguration.Original);
            //TODO: Fix resource export. Above is a byte array.
            Directory.CreateDirectory(filePath);
            File.Create(filePath + (string)resource.GetField(Resource.FileNameFieldId).Data + fileType);
        }
    }
}