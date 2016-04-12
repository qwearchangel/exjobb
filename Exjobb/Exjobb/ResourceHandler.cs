using inRiver.Remoting.Objects;
using inRiver.Remoting;
using Exjobb.Shared.Constants;
using inRiver.Remoting.Image;
using System.IO;

namespace Exjobb
{
    public class ResourceHandler : IResourceHandler
    {
        private const string fileType = ".jpg";

        public void DeleteResource(Entity resource, string imagePath)
        {
            var fileName = resource.GetField(Resource.FileNameFieldId).Data.ToString();
            var path = Path.Combine(imagePath, fileName);
            File.Delete(path + fileType);
        }

        public void ExportResource(Entity resource, string imagePath)
        {
            var resourceFile = RemoteManager.UtilityService.GetFile((int)resource.GetField(Resource.FileIdFieldId).Data, ImageConfiguration.Original);
            var fileName = resource.GetField(Resource.FileNameFieldId).Data.ToString();
            var path = Path.Combine(imagePath, fileName);
            Directory.CreateDirectory(imagePath);
            File.WriteAllBytes(path + fileType, resourceFile);
        }
    }
}