using System.Linq;
using inRiver.Remoting.Objects;
using System.Xml.Linq;
using Exjobb.Shared.Constants;
using System.IO;
using inRiver.Remoting;
using inRiver.Integration.Configuration;

namespace Exjobb
{
    public class MessageHandler : IMessageHandler
    {
        private const string fileType = ".xml";

        public void SendDeleteMessage(Entity entity, string filePath)
        {
            CreateAndSendMessage(entity, Operation.Delete, filePath);
        }

        public void SendLinkMessage(Entity entity, string filePath)
        {
            CreateAndSendMessage(entity, Operation.Link, filePath);
        }

        public void SendUnlinkMessage(Entity entity, string filePath)
        {
            CreateAndSendMessage(entity, Operation.Delete, filePath);
        }

        public void SendUpdateMessage(Entity entity, string filePath)
        {
            CreateAndSendMessage(entity, Operation.Update, filePath);
        }

        private void CreateAndSendMessage(Entity entity, string operation, string filePath)
        {
            if (entity.EntityType.Id == ChannelNode.EntityTypeId)
            {
                checkOutboundLinks(entity, operation, filePath);
                return;
            }

            if (entity.EntityType.Id == Item.EntityTypeId)
            {
                var itemSizeField = entity.GetField(Item.ItemSizeFieldId);
                if (itemSizeField != null)
                {
                    itemSizeField.Data = GetStringCvlFieldValue(itemSizeField);
                }
            }

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                        new XElement("Data",
                        new XElement("DataType", entity.EntityType.Id),
                        new XElement("Operation", operation),
                        new XElement("Fields",
                        entity.Fields.Select(field => new XElement(field.FieldType.Id, field.Data)))
                        )
                    );

            string fileName;
            if (entity.DisplayName  == null)
            {
                fileName = entity.GetField(
                    entity.EntityType.Id == Product.EntityTypeId ? Product.IdFieldId : Item.IdFieldId)
                    .Data.ToString();
            }
            else
            {
                fileName = entity.DisplayName.Data.ToString();
            }

            Directory.CreateDirectory(filePath);
            doc.Save(filePath + fileName + fileType);

            checkOutboundLinks(entity, operation, filePath);
        }

        private void checkOutboundLinks(Entity entity, string operation, string filePath)
        {
            if (entity.OutboundLinks.Any())
            {
                foreach (var link in entity.OutboundLinks)
                {
                    var targetEntity = RemoteManager.DataService.GetEntity(link.Target.Id, LoadLevel.DataOnly);
                    if (targetEntity.EntityType.Id != Resource.EntityTypeId)
                    {
                        CreateAndSendMessage(targetEntity, operation, filePath);
                    }
                }
            }
        }

        private string GetStringCvlFieldValue(Field cvlField)
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
