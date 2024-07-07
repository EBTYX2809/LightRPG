using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Location
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] LocationOverview { get; set; }
        public string[] LocationOverviewDialogue { get; set; }
        public string[] LocationOverviewFight { get; set; }
        public Quest QuestAvailableHere { get; set; }
        public Monster MonsterLivingHere { get; set; }
        public Character CharacterLivingHere { get; set; }
        public Location LocationToNorth { get; set; }
        public Location LocationToEast { get; set; }
        public Location LocationToSouth { get; set; }
        public Location LocationToWest { get; set; }

        public Location(int id, string name, string description,
        string[] locationOverview, string[] locationOverviewDialogue = null, string[] locationOverviewFight = null,
        Monster monsterLivingHere = null, Character characterLivingHere = null)
        {
            ID = id;
            Name = name;
            Description = description;
            MonsterLivingHere = monsterLivingHere;
            CharacterLivingHere = characterLivingHere;

            LocationOverview = locationOverview;
        }

        public string ShowLocation(string situation)
        {
            StringBuilder sb = new StringBuilder();

            if (situation == "default")
            {
                foreach (var line in LocationOverview)
                {
                    sb.AppendLine(line);
                }
            }
            else if (situation == "dialogue")
            {
                foreach (var line in LocationOverviewDialogue)
                {
                    sb.AppendLine(line);
                }
            }
            else if(situation == "fight")
            {
                foreach (var line in LocationOverviewFight)
                {
                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }
    }
}
