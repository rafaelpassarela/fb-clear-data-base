namespace Schemas
{
    class FbSchemaTrigger : FbSchemaBaseItem
    {
        public bool Inactive { get; set; }
        public bool System { get; set; }
        public int Type { get; set; }
        public int Sequence { get; set; }
        public string Source { get; set; }
    }
}
