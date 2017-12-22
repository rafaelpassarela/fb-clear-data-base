using System.Data;

namespace Schemas
{
    public abstract class FbSchemaBaseItem
    {
        public string TableName { get; set; }
        public string Name { get; set; }

        public abstract void ProcessDataRow(DataRow row);
    }
}
