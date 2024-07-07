using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Character : Informer
    {
        public Quest QuestAvailable { get; set; }
        public Character(int id, string name, string description, Quest questAvailable = null) : base(id, name, description)
        {
            QuestAvailable = questAvailable;
        }

    }
}
