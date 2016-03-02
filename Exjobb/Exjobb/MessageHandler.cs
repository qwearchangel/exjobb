using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inRiver.Remoting.Objects;
using System.Xml;
using System.Xml.Linq;

namespace Exjobb
{
    public class MessageHandler : IMessageHandler
    {
        private const string filePath = @"C:\temp\";
        private const string fileType = ".xml";

        public void SendDeleteMessage(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void SendLinkMessage(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void SendUnlinkMessage(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void SendUpdateMessage(Entity entity)
        {
            CreateAndSendMessage(entity, "update");
        }

        private void CreateAndSendMessage(Entity entity, string operation)
        {
            XDocument doc = new XDocument
                (
                new XDeclaration("1.0", "utf-8", null),
                new XElement(entity.EntityType.Id,
                entity.Fields.Select(field => new XElement(field.FieldType.Id, field.Data)))
                );

            doc.Save(filePath + (string)entity.DisplayName.Data + fileType);
        }
    }
}
