using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemas
{
    public class FbSchemaBaseCollection<T> where T : IFbSchemaItem
    {
        public List<IFbSchemaItem> Items { get; set; } = new List<IFbSchemaItem>();

        public IFbSchemaItem GetNewItem()
        {
            var item = (T)Activator.CreateInstance(typeof(T));
            Items.Add(item);

            return item;
        }
    }
}
