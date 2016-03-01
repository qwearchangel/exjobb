using Exjobb.Shared.Constants;
using inRiver.Remoting;
using inRiver.Remoting.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exjobb
{
    public class DataHandler : IDataHandler
    {
        public Entity UpdateEntity(int entityId, string[] fieldTypeIds)
        {
            var entity = RemoteManager.DataService.GetEntity(entityId, LoadLevel.DataAndLinks);

            return null;
        }
    }
}
