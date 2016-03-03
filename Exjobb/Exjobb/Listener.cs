using inRiver.Integration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inRiver.Remoting.Objects;
using inRiver.Remoting;
using Exjobb.Shared.Constants;
using inRiver.Integration.Reporting;
using inRiver.Integration.Configuration;
using inRiver.Integration.Export;

namespace Exjobb
{
    public class Listener : ServerListener, IChannelListener, IOutboundConnector
    {
        private IDataHandler _dataHandler;
        private IMessageHandler _messageHandler;

        public Listener() :
            this(
            new DataHandler(),
            new MessageHandler())
        {

        }

        public Listener(
            IDataHandler dataHandler,
            IMessageHandler messageHandler)
        {
            _dataHandler = dataHandler;
            _messageHandler = messageHandler;
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
            // Here
        }

        public void ChannelLinkDeleted(int channelId, int sourceEntityId, int targetEntityId, string linkTypeId, int? linkEntityId)
        {

            // Here
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
    }
}
