using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Subject
    {
        public int MaximumHP { get; set; }
        public int CurrentHP { get; set; }

        public Subject(int maximumHP, int currentHP) 
        {
            MaximumHP = maximumHP;
            CurrentHP = currentHP;
        }
    }
}
