using System.Linq;
using inRiver.Remoting.Objects;
using System.Xml.Linq;
using Exjobb.Shared.Constants;
using System.IO;

namespace Exjobb
{
    public class MessageHandler : IMessageHandler
    {
        private const string filePath = @"C:\temp\";
        private const string fileType = ".xml";

        public void SendDeleteMessage(Entity entity)
        {
            CreateAndSendMessage(entity, Operation.Delete);
        }

        public void SendLinkMessage(Entity entity)
        {
            CreateAndSendMessage(entity, Operation.Link);
        }

        public void SendUnlinkMessage(Entity entity)
        {
            CreateAndSendMessage(entity, Operation.Delete);
        }

        public void SendUpdateMessage(Entity entity)
        {
            CreateAndSendMessage(entity, Operation.Update);
        }

        private void CreateAndSendMessage(Entity entity, string operation)
        {
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
                fileName = (string)entity.GetField(
                    entity.EntityType.Id == Product.EntityTypeId ? Product.IdFieldId : Item.IdFieldId)
                    .Data;
            }
            else
            {
                fileName = (string)entity.DisplayName.Data;
            }
            Directory.CreateDirectory(filePath);
            doc.Save(filePath + fileName + fileType);
        }
    }
}
