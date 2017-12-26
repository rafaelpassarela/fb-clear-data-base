using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Schemas
{
    public class FbSchemaProcedure : IFbSchemaItem
    {
        public string Name { get; set; }
        public int Inputs { get; set; } = -1;
        public int Outputs { get; set; } = -1;
        public string Source { get; set; }
        public List<FbSchemaProcedureParam> Params { get; set; } = new List<FbSchemaProcedureParam>();

        public string GetCreateSQL()
        {
            var sql = $@"SET TERM ^ ;

CREATE OR ALTER procedure {Name} {GetInputParams()} 
{GetOutputParams()}
AS
{Source}^

SET TERM ; ^

";
            return sql;
        }

        private string GetInputParams()
        {
            if (Inputs <= 0)
                return "";

            var param = "";
            foreach (var item in Params.Where(x => x.InOut == 1).OrderBy(y => y.Position))
            {
                param += (!string.IsNullOrEmpty(param) ? ", \r\n" : "") + item.GetCreateSQL();
            }
            return $"({param})";
        }

        private string GetOutputParams()
        {
            if (Outputs <= 0)
                return "";

            var param = "";
            foreach (var item in Params.Where(x => x.InOut == 2).OrderBy(y => y.Position))
            {
                param += (!string.IsNullOrEmpty(param) ? ", \r\n" : "") + item.GetCreateSQL();
            }
            return $"returns ({param})";
        }

        public string GetMasterName()
        {
            return "";
        }

        public string GetName()
        {
            return Name;
        }

        public void ProcessDataRow(DataRow row)
        {
            Name = row["PROCEDURE_NAME"].ToString();
            Source = row["SOURCE"].ToString();

            if (int.TryParse(row["INPUTS"].ToString(), out int input))
            {
                Inputs = input;
            }

            if (int.TryParse(row["OUTPUTS"].ToString(), out int output))
            {
                Outputs = output;
            }
        }
    }
}
