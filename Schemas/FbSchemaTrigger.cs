using System.Data;

namespace Schemas
{
    public class FbSchemaTrigger : IFbSchemaItem
    {
        public string TableName { get; set; }
        public string Name { get; set; }
        public bool Inactive { get; set; }
        public bool System { get; set; }
        public int TriggerType { get; set; } = -1;
        public int Position { get; set; } = -1;
        public string Source { get; set; }

        public string GetCreateSQL()
        {
            var activeState = (Inactive) ? "INACTIVE" : "ACTIVE";
            var sql = $@"SET TERM ^ ;

CREATE OR ALTER TRIGGER {Name} FOR {TableName}
{activeState} {TriggerTypeToString()} POSITION {Position}
{Source}^

SET TERM ; ^

";
            return sql;
        }

        public string GetName()
        {
            return Name;
        }

        private string TriggerTypeToString()
        {
            switch (TriggerType)
            {
                case 1:
                    return "BEFORE INSERT";
                case 2:
                    return "AFTER INSERT";
                case 3:
                    return "BEFORE UPDATE";
                case 4:
                    return "AFTER UPDATE";
                case 5:
                    return "BEFORE DELETE";
                case 6:
                    return "AFTER DELETE";
                default:
                    return $"UNDEF {TriggerType}";
            }
        }

        public void ProcessDataRow(DataRow row)
        {
            Name = row["TRIGGER_NAME"].ToString();
            TableName = row["TABLE_NAME"].ToString();
            Inactive = row["IS_INACTIVE"].ToString() == "1";
            System = row["IS_SYSTEM_TRIGGER"].ToString() == "1";
            Source = row["SOURCE"].ToString();

            if (int.TryParse(row["TRIGGER_TYPE"].ToString(), out int type))
            {
                TriggerType = type;
            }

            if (int.TryParse(row["SEQUENCE"].ToString(), out int seq))
            {
                Position = seq;
            }
        }

        public string GetMasterName()
        {
            return TableName;
        }
    }
}
