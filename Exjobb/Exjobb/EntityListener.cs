using inRiver.Integration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inRiver.Remoting.Objects;
using inRiver.Remoting;
using Exjobb.Shared.Constants;

namespace Exjobb
{
    public class EntityListener : IEntityListener
    {
        private readonly IProductHandler _productHandler;

        public EntityListener(IProductHandler productHandler)
        {
            _productHandler = productHandler;
        }


        public void EntityCommentAdded(int entityId, int commentId)
        {
            throw new NotImplementedException();
        }

        public void EntityCreated(int entityId)
        {
            var entity = RemoteManager.DataService.GetEntity(entityId, LoadLevel.Shallow);
            if (entity.EntityType.Id == Product.EntityTypeId)
            {
                _productHandler.CreateOrLinkNode(entity);
            }
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
