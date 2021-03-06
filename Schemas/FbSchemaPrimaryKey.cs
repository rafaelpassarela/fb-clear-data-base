﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemas
{
    public class FbSchemaPrimaryKey : IFbSchemaItem
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int Postition { get; set; }

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
            Name = row["PK_NAME"].ToString();
            TableName = row["TABLE_NAME"].ToString();
            ColumnName = row["COLUMN_NAME"].ToString();

            if (int.TryParse(row["ORDINAL_POSITION"].ToString(), out var pos))
            {
                Postition = pos;
            }
        }
    }
}
