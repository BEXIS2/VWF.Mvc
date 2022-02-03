using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vaiona.Utils.Cfg
{
    public class JsonSettings
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public Entry[] Entry { get; set; }
    }

    public class Entry
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public Item[] Item { get; set; }
    }

    public class Item
    {
        public Attribute[] Attribute { get; set; }
    }

    public class Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}