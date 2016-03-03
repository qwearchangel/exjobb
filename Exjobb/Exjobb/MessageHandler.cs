using System;
using System.Linq;
using inRiver.Remoting.Objects;
using System.Xml.Linq;
using Exjobb.Shared.Constants;

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
            CreateAndSendMessage(entity, Operation.UnLink);
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
                        new XElement(entity.EntityType.Id,
                        new XElement("Operation", operation),
                        new XElement("Fields",
                        entity.Fields.Select(field => new XElement(field.FieldType.Id, field.Data)))
                        )
                    );

            string fileName;
            if (entity.DisplayName  == null)
            {
                fileName = (string)entity.GetField(Product.IdFieldId).Data;
            }
            else
            {
                fileName = (string)entity.DisplayName.Data;
            }

            doc.Save(filePath + fileName + fileType);
        }
    }
}
