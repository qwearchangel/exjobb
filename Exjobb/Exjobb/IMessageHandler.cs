using inRiver.Remoting.Objects;

namespace Exjobb
{
    public interface IMessageHandler
    {
        void SendUpdateMessage(Entity entity, string filePath);

        void SendUnlinkMessage(Entity entity, string filePath);

        void SendLinkMessage(Entity entity, string filePath);

        void SendDeleteMessage(Entity entity, string filePath);
    }
}
