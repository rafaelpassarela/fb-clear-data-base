using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemas
{
    public abstract class FbSchemaBaseCollection
    {
        public List<FbSchemaBaseItem> Items { get; set; } = new List<FbSchemaBaseItem>();

        public FbSchemaBaseItem GetNewItem()
        {
            var item = DoGetNewItem();
            Items.Add(item);

            return item;
        }

        protected abstract FbSchemaBaseItem DoGetNewItem();

    }
}
