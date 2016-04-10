using inRiver.Server.Extension;
using inRiver.Remoting.Objects;
using Exjobb.Shared.Constants;

namespace Exjobb.Extension
{
    public class ServerExtension : IServerExtension
    {
        public void OnAdd(Entity entity, CancelUpdateArgument arg)
        {
            CheckIdLenght(entity, arg);
        }

        public void OnCreateVersion(Entity entity, CancelUpdateArgument arg)
        {
        }

        public void OnDelete(Entity entity, CancelUpdateArgument arg)
        {
        }

        public void OnLink(Link link, CancelUpdateArgument arg)
        {
        }

        public void OnLinkUpdate(Link link, CancelUpdateArgument arg)
        {
        }

        public void OnLock(Entity entity, CancelUpdateArgument arg)
        {
        }

        public void OnUnlink(Link link, CancelUpdateArgument arg)
        {
        }

        public void OnUnlock(Entity entity, CancelUpdateArgument arg)
        {
        }

        public void OnUpdate(Entity entity, CancelUpdateArgument arg)
        {
            CheckIdLenght(entity, arg);
        }

        private void CheckIdLenght(Entity entity, CancelUpdateArgument arg)
        {
            if (entity.EntityType.Id == Product.EntityTypeId)
            {
                var id = (int)entity.GetField(Product.IdFieldId).Data;
                if (id.ToString().Length != 6)
                {
                    arg.Cancel = true;
                    arg.Message = "Product Id needs to be 6 letters long! ex: 123456";
                    return;
                }
            }
            if (entity.EntityType.Id == Item.EntityTypeId)
            {
                var id = (int)entity.GetField(Product.IdFieldId).Data;
                if (id.ToString().Length != 8)
                {
                    arg.Cancel = true;
                    arg.Message = "Item Id needs to be 8 letters long! ex: 12345601";
                    return;
                }
            }
            if (entity.EntityType.Id == Resource.FileNameFieldId)
            {
                var name = (string)entity.GetField(Product.IdFieldId).Data;
                if (name.Length != 10)
                {
                    arg.Cancel = true;
                    arg.Message = "Resource name needs to be 10 letters long! ex: 1234560101";
                    return;
                }
            }
        }
    }
}
