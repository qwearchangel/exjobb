using inRiver.Integration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inRiver.Remoting.Objects;

namespace Exjobb
{
    class EntityListener : IEntityListener
    {
        public void EntityCommentAdded(int entityId, int commentId)
        {
            throw new NotImplementedException();
        }

        public void EntityCreated(int entityId)
        {
            
        }

        public void EntityDeleted(Entity deletedEntity)
        {
            throw new NotImplementedException();
        }

        public void EntityFieldSetUpdated(int entityId, string fieldSetId)
        {
            throw new NotImplementedException();
        }

        public void EntityLocked(int entityId)
        {
            throw new NotImplementedException();
        }

        public void EntitySpecificationFieldAdded(int entityId, string fieldName)
        {
            throw new NotImplementedException();
        }

        public void EntitySpecificationFieldUpdated(int entityId, string fieldName)
        {
            throw new NotImplementedException();
        }

        public void EntityUnlocked(int entityId)
        {
            throw new NotImplementedException();
        }

        public void EntityUpdated(int entityId, string[] fields)
        {
            throw new NotImplementedException();
        }
    }
}
