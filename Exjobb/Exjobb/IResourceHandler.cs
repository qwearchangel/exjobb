using inRiver.Remoting.Objects;

namespace Exjobb
{
    public interface IResourceHandler
    {
        void ExportResource(Entity resource, string imagePath);
        void DeleteResource(Entity resource, string imagePath);
    }
}