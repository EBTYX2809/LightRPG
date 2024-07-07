using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Item
    {
        public int ID { get; set; }
        public int Count { get; set; }
        public string Name { get; set; } 
        public Item(int id, string name)
        {
            ID = id;
            Name = name;
            Count = 0;
        }
    }
}
