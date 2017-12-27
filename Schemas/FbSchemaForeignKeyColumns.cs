using System;
using System.Data;

namespace Schemas
{
    public class FbSchemaForeignKeyColumns : IFbSchemaItem
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedColumnName { get; set; }
        public int Position { get; set; }
        
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
            ColumnName = row["COLUMN_NAME"].ToString();
            ReferencedColumnName = row["REFERENCED_COLUMN_NAME"].ToString();
            ReferencedTableName = row["REFERENCED_TABLE_NAME"].ToString();

            if (int.TryParse(row["ORDINAL_POSITION"].ToString(), out var pos))
            {
                Position = pos;
            }
        }
    }
}
