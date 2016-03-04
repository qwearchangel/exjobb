using inRiver.Remoting.Objects;

namespace Exjobb
{
    public interface IDataHandler
    {
        Entity UpdateEntity(Entity entity, string[] fieldTypeIds);
    }
}
