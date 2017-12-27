using System;
using System.Data;

namespace Schemas
{
    public class FbSchemaIndexes : IFbSchemaItem
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public bool Active { get; set; }
        public bool Unique { get; set; }
        public bool Primary { get; set; }
        // Null = PK/FK , 0 = INDEX, 1 = DESCENDING INDEX
        public string IndexType { get; set; }

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
            Name = row["INDEX_NAME"].ToString();
            TableName= row["TABLE_NAME"].ToString();
            IndexType = row["INDEX_TYPE"].ToString();
            Active = row["IS_INACTIVE"].ToString() == "0";
            Unique = row["IS_UNIQUE"].ToString() == "1";

            var value = row["IS_PRIMARY"].ToString().ToUpper();
            Primary = value == "1" || value == "TRUE";
        }
    }
}
