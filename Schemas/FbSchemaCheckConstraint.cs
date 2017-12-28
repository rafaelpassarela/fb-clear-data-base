using System;
using System.Data;

namespace Schemas
{
    public class FbSchemaCheckConstraint : IFbSchemaItem
    {
        public string Name{ get; set; }
        public string TableName { get; set; }
        public string Source { get; set; }

        public string GetCreateSQL()
        {
            throw new NotImplementedException();
        }

        public string GetMasterName()
        {
            return TableName;
        }

        public string GetName()
        {
            return Name;
        }

        public void ProcessDataRow(DataRow row)
        {
            Name = row["CONSTRAINT_NAME"].ToString();
            TableName = row["TABLE_NAME"].ToString();
            Source = row["CHECK_CLAUSULE"].ToString();
        }
    }
}
