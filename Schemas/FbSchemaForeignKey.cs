using System;
using System.Data;

namespace Schemas
{
    public class FbSchemaForeignKey : IFbSchemaItem
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string ReferencedTable { get; set; }
        public string UpdateRule { get; set; }
        public string DeleteRule { get; set; }

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
            ReferencedTable = row["REFERENCED_TABLE_NAME"].ToString();
            UpdateRule = row["UPDATE_RULE"].ToString();
            DeleteRule = row["DELETE_RULE"].ToString();
        }

        public string GetDeleteRule()
        {
            if (string.IsNullOrEmpty(DeleteRule) || DeleteRule == "RESTRICT")
            {
                return "";
            }

            return $"ON DELETE {DeleteRule}";
        }

        public string GetUpdateRule()
        {
            if (string.IsNullOrEmpty(UpdateRule) || UpdateRule == "RESTRICT")
            {
                return "";
            }

            return $"ON UPDATE {UpdateRule}";
        }
    }
}
