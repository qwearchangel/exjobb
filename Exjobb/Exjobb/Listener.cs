﻿using inRiver.Integration.Interface;
using System;
using System.Linq;
using inRiver.Remoting.Objects;
using inRiver.Remoting;
using Exjobb.Shared.Constants;
using inRiver.Integration.Reporting;
using inRiver.Integration.Export;
using inRiver.Remoting.Query;
using inRiver.Integration.Configuration;

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
            base.InitConfigurationSettings();
            ConfigurationManager.Instance.SetConnectorSetting(Id, Setting.ImageExportSettingKey, @"C:\temp\image");
            ConfigurationManager.Instance.SetConnectorSetting(Id, Setting.XmlExportSettingKey, @"C:\temp\");
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
            var updatedEntity = RemoteManager.DataService.GetEntity(entityId, LoadLevel.DataAndLinks);

            if (updatedEntity == null)
            {
                return;
            }

            Entity entity = null;
            if (updatedEntity.EntityType.Id != Resource.EntityTypeId)
            {
                entity = _dataHandler.UpdateEntity(updatedEntity, fieldTypeIds);
            }

            if (entity == null)
            {
                return;
            }
            string filePath = ConfigurationManager.Instance.GetSetting(Id, Setting.XmlExportSettingKey);
            _messageHandler.SendUpdateMessage(entity, filePath);
        }

        public void ChannelEntityDeleted(int channelId, Entity deletedEntity)
        {
            string filePath = ConfigurationManager.Instance.GetSetting(Id, Setting.XmlExportSettingKey);
            if (deletedEntity.EntityType.Id == Resource.EntityTypeId)
            {
                string imagePath = ConfigurationManager.Instance.GetSetting(Id, Setting.ImageExportSettingKey);
                _resourceHandler.DeleteResource(deletedEntity, imagePath);
                return;
            }
            _messageHandler.SendDeleteMessage(deletedEntity, filePath);
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
            var targetEntity = RemoteManager.DataService.GetEntity(targetEntityId, LoadLevel.DataAndLinks);

            if (targetEntity.EntityType.Id == Resource.EntityTypeId)
            {
                string imagePath = ConfigurationManager.Instance.GetSetting(Id, Setting.ImageExportSettingKey);
                _resourceHandler.ExportResource(targetEntity, imagePath);
                return;
            }

            string filePath = ConfigurationManager.Instance.GetSetting(Id, Setting.XmlExportSettingKey);
            _messageHandler.SendLinkMessage(targetEntity, filePath);
        }

        public void ChannelLinkDeleted(int channelId, int sourceEntityId, int targetEntityId, string linkTypeId, int? linkEntityId)
        {
            var entity = RemoteManager.DataService.GetEntity(targetEntityId, LoadLevel.DataAndLinks);
            string filePath = ConfigurationManager.Instance.GetSetting(Id, Setting.XmlExportSettingKey);
            _messageHandler.SendUnlinkMessage(entity, filePath);
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
                LinkResourceToItem(entity);
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

            var productId = (entity.GetField(Item.IdFieldId).Data.ToString()).Substring(0, 6);
            if (string.IsNullOrWhiteSpace(productId))
            {
                return;
            }

            var productList = RemoteManager.DataService.Search(new Criteria
            {
                FieldTypeId = Product.IdFieldId,
                Operator = Operator.Equal,
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

            if (!nodeList.Any())
            {
                nodeList.Add(CreateAndLinkNode(nodeName));
            }

            Link link = new Link
            {
                LinkType = new LinkType { Id = ChannelNode.ProductLinkType },
                Source = nodeList[0],
                Target = entity
            };
            RemoteManager.DataService.AddLinkLast(link);
        }

        private void LinkResourceToItem(Entity entity)
        {
            if (!entity.Fields.Any())
            {
                return;
            }

            var resourceName = (entity.GetField(Resource.FileNameFieldId).Data).ToString().Substring(0, 10);

            var resouceId = new string(resourceName.Where(Char.IsDigit).ToArray());

            var itemId = resouceId.Remove(resouceId.Length -2) ;

            if (itemId.Length != 8)
            {
                return;
            }

            var itemEntities = RemoteManager.DataService.Search(new Criteria
            {
                FieldTypeId = Item.IdFieldId,
                Operator = Operator.Equal,
                Value = itemId
            },
            LoadLevel.Shallow);

            if (!itemEntities.Any())
            {
                ReportManager.Instance.WriteError(Id, $"No item with id: {itemId} was found");
                return;
            }
            var sourceEntity = itemEntities[0];
            var linkType = new LinkType { Id = Item.ResourceLinkTypeId };

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

        private Entity CreateAndLinkNode(string nodeName)
        {
            var nodeEntityType = RemoteManager.ModelService.GetEntityType(ChannelNode.EntityTypeId);

            var node = Entity.CreateEntity(nodeEntityType);
            var nodeNameField = node.Fields.Find(f => f.FieldType.Id == ChannelNode.IdFieldId);
            nodeNameField.Data = nodeName;

            node = RemoteManager.DataService.AddEntity(node);

            var channelId = ConfigurationManager.Instance.GetSetting(Id, Setting.ChannelIdSettingKey);

            var channel = RemoteManager.DataService.GetEntity(int.Parse(channelId), LoadLevel.Shallow);

            var linkType = new LinkType { Id = ChannelNode.ChannelLinkType };
            var link = new Link
            {
                LinkType = linkType,
                Source = channel,
                Target = node
            };

            RemoteManager.DataService.AddLinkLast(link);

            return node;
        }
    }
}
