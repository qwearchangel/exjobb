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

        public void ExportResource(Entity resource, string ImagePath)
        {
            var resourceFile = RemoteManager.UtilityService.GetFile((int)resource.GetField(Resource.FileIdFieldId).Data, ImageConfiguration.Original);
            var filePath = RemoteManager.UtilityService.GetServerSetting(Setting.ImageExportSettingKey);
            Directory.CreateDirectory(filePath);
            File.WriteAllBytes(ImagePath + (string)resource.GetField(Resource.FileNameFieldId).Data + fileType, resourceFile);
        }
    }
}