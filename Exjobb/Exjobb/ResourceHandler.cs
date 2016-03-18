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
            File.Delete(imagePath + (string)resource.GetField(Resource.FileNameFieldId).Data + fileType);
        }

        public void ExportResource(Entity resource, string imagePath)
        {
            var resourceFile = RemoteManager.UtilityService.GetFile((int)resource.GetField(Resource.FileIdFieldId).Data, ImageConfiguration.Original);
            var filePath = RemoteManager.UtilityService.GetServerSetting(Setting.ImageExportSettingKey);
            Directory.CreateDirectory(filePath);
            File.WriteAllBytes(imagePath + (string)resource.GetField(Resource.FileNameFieldId).Data + fileType, resourceFile);
        }
    }
}