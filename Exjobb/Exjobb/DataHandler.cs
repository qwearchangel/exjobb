using inRiver.Remoting.Objects;
using System.Collections.Generic;

namespace Exjobb
{
    public class DataHandler : IDataHandler
    {
        public DataHandler()
        {
        }

        public Entity UpdateEntity(Entity storedEntity, string[] fieldTypeIds)
        {
            var updatedFieldsEntity = new Entity
            {
                EntityType = storedEntity.EntityType,
                Fields = GetUpdatedFields(storedEntity, fieldTypeIds),
            };
            updatedFieldsEntity.DisplayName = storedEntity.DisplayName;
                        
            return updatedFieldsEntity;
        }

        private List<Field> GetUpdatedFields(Entity storedEntity, string[] fieldTypeIds)
        {
            var fieldList = new List<Field>();
            foreach (var fieldTypeId in fieldTypeIds)
            {
                fieldList.Add(new Field { FieldType = new FieldType { Id = fieldTypeId }, Data = storedEntity.GetField(fieldTypeId).Data });
            }

            return fieldList;
        }
    }
}
