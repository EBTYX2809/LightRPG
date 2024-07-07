using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Monster : Subject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaximumDamage { get; set; }
        public int RewardExperience { get; set; }
        public int RewardGold { get; set; }
        public List<LootItem> LootTable { get; set; }
        public Monster(int maximumHP, int currentHP, int id, string name, int maximumDamage, int rewardExperience, int rewardGold) :base(maximumHP, currentHP)
        {
            ID = id;
            Name = name;
            MaximumDamage = maximumDamage;
            RewardExperience = rewardExperience;
            RewardGold = rewardGold;
            LootTable = new List<LootItem>();
        }
    }
}
