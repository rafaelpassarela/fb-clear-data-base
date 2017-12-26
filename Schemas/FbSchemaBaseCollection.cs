using LocalizationHelper;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IFbSchemaItem GetItemByName(string name)
        {
            return Items.FirstOrDefault(x => x.GetName().ToUpper() == name.ToUpper());
        }

        public string GetCreateSQLForItem(string itemName)
        {
            var item = GetItemByName(itemName.Trim());
            return (item != null) ? item.GetCreateSQL() : $"{strings.error}> {strings.notFound} [{itemName}]";
        }
    }
}
