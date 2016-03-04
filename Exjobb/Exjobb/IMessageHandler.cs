using inRiver.Remoting.Objects;

namespace Exjobb
{
    public interface IMessageHandler
    {
        void SendUpdateMessage(Entity entity);

        void SendUnlinkMessage(Entity entity);

        void SendLinkMessage(Entity entity);

        void SendDeleteMessage(Entity entity);
    }
}
