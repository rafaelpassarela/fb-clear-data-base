using LocalizationHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemas
{
    public class FbSchemaProcedureParam : IFbSchemaItem
    {
        public string Name { get; set; }
        public string MasterName { get; set; }
        public string DataType { get; set; }
        public int Position { get; set; }
        // PARAMETER_DIRECTION = 1 (In) 2 (Out)
        public int InOut { get; set; }
        // for strings only (char, varchar...)
        public int Size { get; set; }
        public string CharSet { get; set; } = "";
        // for double (numeric, double precision) = 15,4
        public int Precision { get; set; } // 15
        public int Scale { get; set; } // 4

        public string GetCreateSQL()
        {
            switch (DataType.ToUpper())
            {
                case "DATE":
                case "INTEGER":
                case "SMALLINT":
                case "TIMESTAMP":
                case "DOUBLE PRECISION":
                    return $"{Name} {DataType}";

                case "NUMERIC":
                case "DECIMAL":
                    return $"{Name} {DataType}({Precision},{Scale})";
                case "CHAR":
                case "VARCHAR":
                    return $"{Name} {DataType}({Size}){((!string.IsNullOrEmpty(CharSet)) ? " character set " + CharSet : "")}";
                default:
                    throw new Exception($"{strings.error}> {strings.notFound} [{DataType}]");
            }
        }

        public string GetMasterName()
        {
            return MasterName;
        }

        public string GetName()
        {
            return Name;
        }

        public void ProcessDataRow(DataRow row)
        {
            Name = row["PARAMETER_NAME"].ToString();
            MasterName = row["PROCEDURE_NAME"].ToString();
            DataType = row["PARAMETER_DATA_TYPE"].ToString();

            if (int.TryParse(row["ORDINAL_POSITION"].ToString(), out int pos))
            {
                Position = pos;
            }

            if (int.TryParse(row["PARAMETER_DIRECTION"].ToString(), out int dir))
            {
                InOut = dir;
            }

            if (int.TryParse(row["PARAMETER_SIZE"].ToString(), out int size))
            {
                Size = size;
            }

            CharSet = row["CHARACTER_SET_NAME"].ToString();

            if (int.TryParse(row["NUMERIC_PRECISION"].ToString(), out int prec))
            {
                Precision = prec;
            }

            if (int.TryParse(row["NUMERIC_SCALE"].ToString(), out int scale))
            {
                Scale = scale;
            }
        }
    }
}
