using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Quest : Informer
    {
        public int RewardExperience { get; set; }
        public int RewardGold { get; set; }
        public bool IsCompleted { get; set; }
        public Dictionary<Item, int> QuestCompletionItems { get; set; }
        public Item RewardItem { get; set; }
        public Quest(int id, string name, string description, int rewardExperience, int rewardGold, Item rewardItem) : base(id, name, description)
        {
            RewardExperience = rewardExperience;
            RewardGold = rewardGold;
            RewardItem = rewardItem;
            IsCompleted = false;
            QuestCompletionItems = new Dictionary<Item, int>();
        }
    }
}
