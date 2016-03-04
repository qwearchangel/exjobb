using inRiver.Integration.Interface;
using System;
using System.Linq;
using inRiver.Remoting.Objects;
using inRiver.Remoting;
using Exjobb.Shared.Constants;
using inRiver.Integration.Reporting;
using inRiver.Integration.Export;
using inRiver.Remoting.Query;

namespace Exjobb
{
    public class Listener : ServerListener, IChannelListener, IEntityListener, IOutboundConnector
    {
        private IDataHandler _dataHandler;
        private IMessageHandler _messageHandler;
        private IResourceHandler _resourceHandler;

        public Listener() :
            this(
            new DataHandler(),
            new MessageHandler(),
            new ResourceHandler())
        {
        }

        public Listener(
            IDataHandler dataHandler,
            IMessageHandler messageHandler,
            IResourceHandler resourceHandler)
        {
            _dataHandler = dataHandler;
            _messageHandler = messageHandler;
            _resourceHandler = resourceHandler;
        }

        private bool _isStarted;

        public new bool IsStarted { get { return _isStarted && base.IsStarted; } }

        public new void Start()
        {
            base.Start();
            _isStarted = true;
            ReportManager.Instance.Write(Id,"Connector started");
        }

        public new void Stop()
        {
            base.Stop();
            _isStarted = false;
            ReportManager.Instance.Write(Id, "Connector stopped");
        }

        public override void InitConfigurationSettings()
        {
        }

        // Channel -------------------------------------------------------------------

        public void Publish(int channelId)
        {
        }

        public void UnPublish(int channelId)
        {
        }

        public void Synchronize(int channelId)
        {
        }

        public void ChannelEntityAdded(int channelId, int entityId)
        {
        }

        public void ChannelEntityUpdated(int channelId, int entityId, string data)
        {
            var fieldTypeIds = data.Split(',');
            var updatedEntity = RemoteManager.DataService.GetEntity(entityId, LoadLevel.DataOnly);

            Entity entity = null;
            if (updatedEntity.EntityType.Id != Resource.EntityTypeId)
            {
                entity = _dataHandler.UpdateEntity(updatedEntity, fieldTypeIds);
            }

            if (entity == null)
            {
                return;
            }

            _messageHandler.SendUpdateMessage(entity);
        }

        public void ChannelEntityDeleted(int channelId, Entity deletedEntity)
        {
            _messageHandler.SendDeleteMessage(deletedEntity);
        }

        public void ChannelEntityFieldSetUpdated(int channelId, int entityId, string fieldSetId)
        {
        }

        public void ChannelEntitySpecificationFieldAdded(int channelId, int entityId, string fieldName)
        {
        }

        public void ChannelEntitySpecificationFieldUpdated(int channelId, int entityId, string fieldName)
        {
        }

        public void ChannelLinkAdded(int channelId, int sourceEntityId, int targetEntityId, string linkTypeId, int? linkEntityId)
        {
            var targetEntity = RemoteManager.DataService.GetEntity(targetEntityId, LoadLevel.DataOnly);

            if (targetEntity.EntityType.Id == Resource.EntityTypeId)
            {
                _resourceHandler.ExportResource(targetEntity);
            }

            _messageHandler.SendLinkMessage(targetEntity);
        }

        public void ChannelLinkDeleted(int channelId, int sourceEntityId, int targetEntityId, string linkTypeId, int? linkEntityId)
        {
            var entity = RemoteManager.DataService.GetEntity(targetEntityId, LoadLevel.DataOnly);
            _messageHandler.SendUnlinkMessage(entity);
        }

        public void ChannelLinkUpdated(int channelId, int sourceEntityId, int targetEntityId, string linkTypeId, int? linkEntityId)
        {
        }

        public void AssortmentCopiedInChannel(int channelId, int assortmentId, int targetId, string targetType)
        {
        }

        public new void OnMessage(string xml)
        {
        }

        //Entity ----------------------------------------------------------------------

        public void EntityCreated(int entityId)
        {
            var entity = RemoteManager.DataService.GetEntity(entityId, LoadLevel.DataOnly);

            if (entity.EntityType.Id == Product.EntityTypeId)
            {
                LinkProductToNode(entity);
                return;
            }
            if (entity.EntityType.Id == Item.EntityTypeId)
            {
                LinkItemToProduct(entity);
                return;
            }
            if (entity.EntityType.Id == Resource.EntityTypeId)
            {
                LinkResourceToProductOrItem(entity);
                return;
            }
        }

        public void EntityUpdated(int entityId, string[] fields)
        {
        }

        public void EntityDeleted(Entity deletedEntity)
        {
        }

        public void EntityLocked(int entityId)
        {
        }

        public void EntityUnlocked(int entityId)
        {
        }

        public void EntityFieldSetUpdated(int entityId, string fieldSetId)
        {
        }

        public void EntityCommentAdded(int entityId, int commentId)
        {
        }

        public void EntitySpecificationFieldAdded(int entityId, string fieldName)
        {
        }

        public void EntitySpecificationFieldUpdated(int entityId, string fieldName)
        {
        }

        // Other ---------------------------------------

        private void LinkItemToProduct(Entity entity)
        {
            if (!entity.Fields.Any())
            {
                return;
            }

            var productId = ((string)entity.GetField(Item.IdFieldId).Data).Substring(0, 6);
            if (string.IsNullOrWhiteSpace(productId))
            {
                return;
            }

            var productList = RemoteManager.DataService.Search(new Criteria
            {
                FieldTypeId = Product.IdFieldId,
                Operator = Operator.BeginsWith,
                Value = productId
            },
            LoadLevel.Shallow);

            Link link = new Link
            {
                LinkType = new LinkType { Id = Product.ItemLinkTypeId },
                Source = productList[0],
                Target = entity
            };
            RemoteManager.DataService.AddLinkLast(link);
        }

        private void LinkProductToNode(Entity entity)
        {
            if (!entity.Fields.Any())
            {
                return;
            }

            var nodeName = GetStringFromCvlField(entity.GetField(Product.CategoryFieldId));
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                return;
            }

            var nodeList = RemoteManager.DataService.Search(new Criteria
            {
                FieldTypeId = ChannelNode.IdFieldId,
                Operator = Operator.Equal,
                Value = nodeName
            },
            LoadLevel.Shallow);

            Link link = new Link
            {
                LinkType = new LinkType { Id = ChannelNode.ProductLinkType },
                Source = nodeList[0],
                Target = entity
            };
            RemoteManager.DataService.AddLinkLast(link);
        }

        private void LinkResourceToProductOrItem(Entity entity)
        {
            if (!entity.Fields.Any())
            {
                return;
            }

            var resourceNameId = ((string)entity.GetField(Resource.FileNameFieldId).Data).Substring(0, 10);

            var resouceId = new string(resourceNameId.Where(Char.IsDigit).ToArray());

            var productOrItemId = resouceId.Remove(resouceId.Length -2) ;

            var sourceEntity = new Entity();
            var linkType = new LinkType();
            if (productOrItemId.Length == 6)
            {
                var productEntities = RemoteManager.DataService.Search(new Criteria
                {
                    FieldTypeId = Product.IdFieldId,
                    Operator = Operator.BeginsWith,
                    Value = productOrItemId
                },
                LoadLevel.Shallow);
                sourceEntity = productEntities[0];
                linkType = new LinkType { Id = Product.ResourceLinkTypeId };
            }
            else
            {
                var itemEntities = RemoteManager.DataService.Search(new Criteria
                {
                    FieldTypeId = Item.IdFieldId,
                    Operator = Operator.BeginsWith,
                    Value = productOrItemId
                },
                LoadLevel.Shallow);
                sourceEntity = itemEntities[0];
                linkType = new LinkType { Id = Item.ResourceLinkTypeId };
            }

            if (sourceEntity == null || linkType == null)
            {
                return;
            }

            var link = new Link
            {
                LinkType = linkType,
                Source = sourceEntity,
                Target = entity
            };

            RemoteManager.DataService.AddLinkLast(link);
        }

        private string GetStringFromCvlField(Field cvlField)
        {
            if (cvlField.IsEmpty())
            {
                return string.Empty;
            }

            CVLValue cvlValue = RemoteManager.ModelService.GetCVLValueByKey(cvlField.Data.ToString(), cvlField.FieldType.CVLId);
            if (cvlValue == null || cvlValue.Value == null)
            {
                return string.Empty;
            }

            return cvlValue.Value.ToString();
        }
    }
}
