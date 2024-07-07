using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player : Subject
    {
        public int Gold { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public Location CurrentLocation { get; set; }
        public List<Item> Inventory { get; set; }
        public List<Quest> Quests { get; set; }
        public Player(int maximumHP, int currentHP, int gold, int experience, int level) : base (maximumHP, currentHP)
        {
            Gold = gold;
            Experience = experience;    
            Level = level;
            Inventory = new List<Item>();
            Quests = new List<Quest>();
        }

        public void AddItemToInventory(Item item)
        {
            if (Inventory.Find(dublicate => dublicate.ID == item.ID && dublicate.Count >= 1) != null)
            {
                item.Count++;
            }
            else {
                Inventory.Add(item);
                item.Count++;
            }
        }

        public void RemoveItemFromInventory(Item item)
        {
            if (Inventory.Find(dublicate => dublicate.ID == item.ID && dublicate.Count > 1) != null)
            {
                item.Count--;
            }
            else
            {
                Inventory.Remove(item);
                item.Count--;
            }
        }
    }
}