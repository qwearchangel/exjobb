using inRiver.Remoting.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
