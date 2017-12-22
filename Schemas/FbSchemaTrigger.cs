using System.Data;

namespace Schemas
{
    class FbSchemaTrigger : FbSchemaBaseItem
    {
        public bool Inactive { get; set; }
        public bool System { get; set; }
        public int TriggerType { get; set; } = -1;
        public int Sequence { get; set; } = -1;
        public string Source { get; set; }

        public override void ProcessDataRow(DataRow row)
        {
            Name = row["TRIGGER_NAME"].ToString();
            TableName = row["TABLE_NAME"].ToString();
            Inactive = row["IS_INACTIVE"].ToString() == "1";
            System = row["IS_SYSTEM_TRIGGER"].ToString() == "1";
            Source= row["SOURCE"].ToString();

            if (int.TryParse(row["TRIGGER_TYPE"].ToString(), out int type))
            {
                TriggerType = type;
            }

            if (int.TryParse(row["SEQUENCE"].ToString(), out int seq))
            {
                Sequence = seq;
            }
        }
    }
}
