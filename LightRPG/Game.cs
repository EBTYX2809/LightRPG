using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine; // Подключаем двигло игры

namespace LightRPG // Добавить стоимость предметов и магазин
{
    public partial class Game : Form
    {
        private Player _player;
        private Monster _currentMonster;
        public Game()
        {
            InitializeComponent();

            _player = new Player(100, 100, 20, 0, 1);
            MoveTo(TheWorld.LocationByID(TheWorld.LOCATION_ID_HOME));
            _player.AddItemToInventory(TheWorld.ItemByID(TheWorld.ITEM_ID_KNIFE));
            UpdateInventoryListInUI();
            UpdateWeaponListInUI();

            lblHP.Text = _player.CurrentHP.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.Experience.ToString();
            lblLevel.Text = _player.Level.ToString();

            // Block for change background color
            panel = new Panel
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(panel);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Here is moving functions
        private void MoveTo(Location newLocation)
        {
            _player.CurrentLocation = newLocation;

            // Show/hide available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null && IsPlayerCanMoveToBridge());
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            // Show/hide available action buttons
            btnSay.Visible = (newLocation.CharacterLivingHere != null);
            cboPlayerLine.Visible = (newLocation.CharacterLivingHere != null);

            btnUseWeapon.Visible = (newLocation.MonsterLivingHere != null);
            btnUsePotion.Visible = (newLocation.MonsterLivingHere != null);
            cboWeapons.Visible = (newLocation.MonsterLivingHere != null);
            cboPotions.Visible = (newLocation.MonsterLivingHere != null);

            // Display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;
            rtbLocation.Text += TheWorld.LocationByID(newLocation.ID).ShowLocation("default");

            SpawnMonster(_player.CurrentLocation); // Auto spawn monster if location has it.
        }

        // Workaround solution that need to lock East button for move if player hasn't sword
        public bool IsPlayerCanMoveToBridge()
        {
            if (_player.CurrentLocation == TheWorld.LocationByID(TheWorld.LOCATION_ID_GUARD_POST))
            {
                if (_player.Inventory.Any(ii => ii.ID == TheWorld.ITEM_ID_SWORD) && firstDialogueWithGuarder == false)
                {
                    return true;
                }
                else { return false; }
            }
            else { return true; }
        }
        
        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }
        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }
        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }
        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Updaters for UI
        private void ScrollToBottomOfMessages()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }
        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 190;
            dgvInventory.Columns[1].Name = "Count";
            dgvInventory.Columns[1].Width = 45;

            dgvInventory.Rows.Clear();

            foreach (Item inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Count > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Name, inventoryItem.Count.ToString() });
                }
            }
        }
        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 190;
            dgvQuests.Columns[1].Name = "Done?";
            dgvQuests.Columns[1].Width = 45;

            dgvQuests.Rows.Clear();

            foreach (var playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Name, playerQuest.IsCompleted.ToString() });
            }
        }
        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (var inventoryItem in _player.Inventory)
            {
                if (inventoryItem is Weapon)
                {
                    if (inventoryItem.Count > 0)
                    {
                        weapons.Add((Weapon)inventoryItem);
                    }
                }
            }
            cboWeapons.DataSource = weapons;
            cboWeapons.DisplayMember = "Name";
            cboWeapons.ValueMember = "ID";

            //cboWeapons.SelectedIndex = 0;
        }
        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (var inventoryItem in _player.Inventory)
            {
                if (inventoryItem is HealingPotion)
                {
                    if (inventoryItem.Count > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                // The player doesn't have any potions, so hide the potion combobox and "Use" button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Quest functions
        public void LevelUpCheck()
        {
            if (_player.Experience >= 30)
            {
                _player.Level++;
                _player.Experience = 0;
                _player.MaximumHP += 20;
                _player.CurrentHP = _player.MaximumHP;

                rtbMessages.Text += "You leveled up.\n";

                lblHP.Text = _player.CurrentHP.ToString();
                lblGold.Text = _player.Gold.ToString();
                lblExperience.Text = _player.Experience.ToString();
                lblLevel.Text = _player.Level.ToString();
            }
            else { return; }
        }
        
        // Compare inventory items with required for complete quests items
        public void CompleteQuestCheck()
        {
            foreach (var InvItem in _player.Inventory)
            {
                foreach (var QstItem in _player.Quests)
                {
                    if (InvItem.ID == QstItem.QuestCompletionItems.First().Key.ID && InvItem.Count >= QstItem.QuestCompletionItems.First().Value)
                    {
                        QstItem.IsCompleted = true;
                    }
                }
            }
            UpdateQuestListInUI();
        }
        public void FinishQuest()
        {            
            _player.Quests.Remove(_player.CurrentLocation.CharacterLivingHere.QuestAvailable);
            UpdateQuestListInUI();

            for (int i = 0; i <= _player.CurrentLocation.CharacterLivingHere.QuestAvailable.QuestCompletionItems.First().Value; i++)
            {
                _player.RemoveItemFromInventory(_player.CurrentLocation.CharacterLivingHere.QuestAvailable.QuestCompletionItems.First().Key);
            }

            _player.AddItemToInventory(_player.CurrentLocation.CharacterLivingHere.QuestAvailable.RewardItem);
            UpdateInventoryListInUI();
            UpdateWeaponListInUI();
            UpdatePotionListInUI();

            _player.Gold += _player.CurrentLocation.CharacterLivingHere.QuestAvailable.RewardGold;
            _player.Experience += _player.CurrentLocation.CharacterLivingHere.QuestAvailable.RewardExperience;
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.Experience.ToString();

            rtbMessages.Text += "\tYou completed the quest \n" + _player.CurrentLocation.CharacterLivingHere.QuestAvailable.Name + Environment.NewLine;
            rtbMessages.Text += "Your reward: " + _player.CurrentLocation.CharacterLivingHere.QuestAvailable.RewardItem.Name + Environment.NewLine;
            rtbMessages.Text += "Gold: " + _player.CurrentLocation.CharacterLivingHere.QuestAvailable.RewardGold.ToString() + Environment.NewLine;
            rtbMessages.Text += "Experience: " + _player.CurrentLocation.CharacterLivingHere.QuestAvailable.RewardExperience.ToString() + Environment.NewLine;
          
            LevelUpCheck();
            ScrollToBottomOfMessages();
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        // Here is dialogue functions
        bool FirstClick = true; // Need to start dialogue from Say button
        // Filling a dictionary to store player answers with further options and filling a dictionary to store characters answers
        private Dictionary<string, List<string>> dialogueOptionsWithGuider = new Dictionary<string, List<string>>
            {
            { "Hello", new List<string> { "Hi. What is this place?", "Good Bye." } },
            { "Hi. What is this place?", new List<string> { "West", "North", "East", "South" } },
            { "West", new List<string> { "Okey, thank you, good bye." } },
            { "North", new List<string> { "Okey, thank you, good bye." } },
            { "East", new List<string> { "Okey, thank you, good bye." } },
            { "South", new List<string> { "Okey, thank you, good bye." } }
            }; 
        private Dictionary<string, string> GuiderResponses = new Dictionary<string, string> 
            {
            { "Hi. What is this place?", "This is just town square. I can tell you more about locations from all directions of the world." },
            { "West", "On West we have a good farm. But I heard a farmer complains that there are rats in the fields. Maybe you could help him with it." },
            { "North", "On north living a strange alchemist. He’s not very sociable, but helps out town be healthy. If you want you can try to talk with him, but be careful." },
            { "East", "On east is a dangerous zone, not everyone is allowed there. Of course, you can talk to the sullen guard, but I’m not sure that he will let you through easily." },
            { "South", "Oh, this is where you came from." },
            { "Good Bye.", "Oh, yeah. Good Bye." },
            { "Okey, thank you, good bye.", "Oh, yeah. Good Bye. And... keep this potion for good luck." }
            }; 
        private Dictionary<string, List<string>> dialogueOptionsWithFarmer = new Dictionary<string, List<string>>
            {
            { "Hey, welcome to my farm. How can I help?", new List<string> { "Hi. What are you growing here?", "Hello. I heard you complain about rats in the fields.", "Nothing. Good Bye." } },
            { "Hello. I heard you complain about rats in the fields.", new List<string> { "Yeah, i will kill them." } },
            }; 
        private Dictionary<string, string> FarmerResponses = new Dictionary<string, string>
            {
            { "Hi. What are you growing here?", "Basically the bare necessities like corn, wheat, and livestock for slaughter - pigs, sheep and cows." },
            { "Hello. I heard you complain about rats in the fields.", "Yeah, they are already sick of me, they are devouring the most mature shoots. Could you help me with them?" },
            { "Nothing. Good Bye.", "Sure. Good bye." },
            { "Yeah, i will kill them.", "Oh good. Will waiting for it." }
            }; 
        private Dictionary<string, List<string>> dialogueOptionsWithAlchemist = new Dictionary<string, List<string>>
            {
            { "...", new List<string> { "Hello. Can you tell me about your ground a little?", "Good Bye." } },
            { "Hello. Can you tell me about your ground a little?", new List<string> { "I'm loocking for a ways to earn extra money. Maybe you can give some work for me?" } },
            { "I'm loocking for a ways to earn extra money. Maybe you can give some work for me?", new List<string> { "Oh, okey i will do this." } },
            }; 
        private Dictionary<string, string> AlchemistResponses = new Dictionary<string, string>
            {
            { "Hello. Can you tell me about your ground a little?", "why?" },
            { "Good Bye.", "*unintelligible muttering*" },
            { "I'm loocking for a ways to earn extra money. Maybe you can give some work for me?", "huh"},
            { "Oh, okey i will do this.", "*follows with an attentive gaze*"},
            }; 
        private Dictionary<string, List<string>> dialogueOptionsWithGuarder = new Dictionary<string, List<string>>
            {
            { "Stop. You shall not pass.", new List<string> { "Why?"} },
            { "Why?", new List<string> { "Okey, i will get something for self-defense. See you soon.", "If so, good bye." } },
            }; 
        private Dictionary<string, string> GuarderResponses = new Dictionary<string, string>
            {
            { "Why?", "Because there is a threat zone With spiders, they kill peoples that can't self-defense" },        
            { "If so, good bye.", "Bye." },
            { "Okey, i will get something for self-defense. See you soon.", "Better keep away from here." }
            }; 
        private HashSet<string> chosenVariants; // Quick fix to dialogue options with guider

        // Function adding combobox dialogue options fro player choise
        private void UpdateComboBoxOptions(List<string> options)
        {
            cboPlayerLine.Items.Clear();
            cboPlayerLine.Items.AddRange(options.ToArray());
        }

        // Quick fix to dialogue options with guider
        private List<string> GetNextVariantOptions()
        {
            var directions = new List<string> { "West", "North", "East", "South" };
            var availableDirections = directions.Except(chosenVariants).ToList();

            if (availableDirections.Count == 0)
            {
                availableDirections.Add("Okey, thank you, good bye.");
            }

            return availableDirections;
        }

        // My function to delete dialogue options that we which we will no longer need (for example, to receive quests).
        private void RemoveDialogueOptions(string tempKey, ref Dictionary<string, List<string>> dialogueOptions, ref Dictionary<string, string> characterResponses)
        {
            // Delete dialogue line after choosed phrase giving a quest.            
            bool IsMapContainPlayerActualLine = true;

            while (IsMapContainPlayerActualLine == true)
            {
                foreach (var obj in dialogueOptions) // Looping through all objects in dialogueOptions
                {
                    // If find corresponding playerActualLine(that rewriting to tempKey) answer in List<string> actual obj                    
                    if (obj.Value.Find(a => a == tempKey) != null)
                    {
                        obj.Value.Remove(tempKey); // Remove playerActualLine(that rewriting to tempKey) в obj.Value(list further player options)

                        // If main block don't find value in actual obj(that is, there are no answers), so obj.Key also deleting.
                        if (obj.Value.Count() == 0)
                        {
                            tempKey = obj.Key;
                            dialogueOptions.Remove(obj.Key);
                        }

                        // If main block return true, that is, obj.Key has some other dialog options, so there is no need to delete obj.Key.                                
                        else
                        {
                            IsMapContainPlayerActualLine = false;
                        }

                        break;
                    }
                }
            }

            // Delete character responses for corresponding player phrase;
            characterResponses.Remove(tempKey);
        }

        // Workaround solution that need to lock East button for move if player hasn't sword
        public bool firstDialogueWithGuarder = true;

        // When we press Say button first time dialogue initialized
        private void InitializeDialogue()
        {
            // Show/hide available movement buttons
            btnNorth.Visible = false;
            btnEast.Visible = false;
            btnSouth.Visible = false;
            btnWest.Visible = false;

            // Rendering location when player talk with character            
            rtbLocation.Text = TheWorld.LocationByID(_player.CurrentLocation.ID).Name + Environment.NewLine;
            rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).Description + Environment.NewLine;
            rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).ShowLocation("dialogue");

            // Display information about character
            rtbMessages.Text += TheWorld.CharacterByID(_player.CurrentLocation.CharacterLivingHere.ID).Name + Environment.NewLine;
            rtbMessages.Text += TheWorld.CharacterByID(_player.CurrentLocation.CharacterLivingHere.ID).Description + Environment.NewLine;
            ScrollToBottomOfMessages();

            // Start dialogue depending on which character we communicate with
            if (_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_GUIDER))
            {
                rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                rtbMessages.Text += "Hello" + Environment.NewLine;
                ScrollToBottomOfMessages();
                UpdateComboBoxOptions(dialogueOptionsWithGuider["Hello"]);

                chosenVariants = new HashSet<string>();
            }
            else if (_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_FARMER))
            {                
                // If player has corresponding quest need delete options that exit on it
                if (_player.Quests.Find(q => q.ID == TheWorld.QUEST_ID_CLEAR_FARMERS_FIELD) != null)
                {
                    RemoveDialogueOptions("Yeah, i will kill them.", ref dialogueOptionsWithFarmer, ref FarmerResponses); 
                }

                // If quest completed we add new answer line for finish it                
                if (_player.Quests.Find(q => q.ID == TheWorld.QUEST_ID_CLEAR_FARMERS_FIELD)?.IsCompleted == true)
                {
                    dialogueOptionsWithFarmer["Hey, welcome to my farm. How can I help?"].Add("I got rid of the rats that ate your seeds.");
                    dialogueOptionsWithFarmer.Add("I got rid of the rats that ate your seeds.", new List<string> { "Thanks, good bye." } );
                    FarmerResponses.Add("I got rid of the rats that ate your seeds.", "Oh, thank you so much. This is what I can give you in gratitude");
                }

                rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                rtbMessages.Text += "Hey, welcome to my farm. How can I help?" + Environment.NewLine;
                ScrollToBottomOfMessages();
                UpdateComboBoxOptions(dialogueOptionsWithFarmer["Hey, welcome to my farm. How can I help?"]);
            }
            else if (_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_ALCHEMIST))
            {
                // If player has corresponding quest need delete options that exit on it
                if (_player.Quests.Find(q => q.ID == TheWorld.QUEST_ID_CLEAR_ALCHEMIST_GARDEN) != null)
                {
                    RemoveDialogueOptions("Oh, okey i will do this.", ref dialogueOptionsWithAlchemist, ref AlchemistResponses);
                }

                // If quest completed we add new answer line for finish it                
                if (_player.Quests.Find(q => q.ID == TheWorld.QUEST_ID_CLEAR_ALCHEMIST_GARDEN)?.IsCompleted == true)
                {
                    dialogueOptionsWithAlchemist["..."].Add("I killed snakes.");
                    dialogueOptionsWithAlchemist.Add("I killed snakes.", new List<string> { "Bye." });
                    AlchemistResponses.Add("I killed snakes.", "ty, thats your reward");
                }

                rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                rtbMessages.Text += "..." + Environment.NewLine;
                ScrollToBottomOfMessages();
                UpdateComboBoxOptions(dialogueOptionsWithAlchemist["..."]);
            }
            else if (_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_GUARDER))
            {        
                // Add quest answer line if player has sword, and guard check this first time
                if (_player.Inventory.Find(q => q.ID == TheWorld.ITEM_ID_SWORD) != null && firstDialogueWithGuarder == true)
                {                                        
                    dialogueOptionsWithGuarder["Why?"].Add("Oh, i can self-defense. I have a sword and healing potion just in case.");
                    dialogueOptionsWithGuarder.Add("Oh, i can self-defense. I have a sword and healing potion just in case.", new List<string> { "Oh good, i want additional reward." });
                    GuarderResponses.Add("Oh, i can self-defense. I have a sword and healing potion just in case.", "Good. Then for you the passage is open.");
                    GuarderResponses.Add("Oh good, i want additional reward.", "Be careful.");                    
                }

                // If player has corresponding quest need delete options that exit on it
                if (_player.Quests.Find(q => q.ID == TheWorld.QUEST_ID_CLEAR_SPIDER_FIELD) != null)
                {
                    RemoveDialogueOptions("Oh good, i want additional reward.", ref dialogueOptionsWithGuarder, ref GuarderResponses);
                }

                // If quest completed we add new answer line for finish it                
                if (_player.Quests.Find(q => q.ID == TheWorld.QUEST_ID_CLEAR_SPIDER_FIELD)?.IsCompleted == true)
                {
                    dialogueOptionsWithGuarder["Stop. You shall not pass."].Add("I killed spiders.");
                    dialogueOptionsWithGuarder.Add("I killed spiders.", new List<string> { "Oh, it's nice to receive this. Good bye." });
                    GuarderResponses.Add("I killed spiders.", "Thank you, keep it, you deserve it.");
                }

                rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                rtbMessages.Text += "Stop. You shall not pass." + Environment.NewLine;
                ScrollToBottomOfMessages();
                UpdateComboBoxOptions(dialogueOptionsWithGuarder["Stop. You shall not pass."]);
            }
        }

        //Main Dialohue function
        private async void Dialogue()
        {        
            // Get choosed answer            
            string playerActualLine = cboPlayerLine.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(playerActualLine))
            {
                MessageBox.Show("Please, choose answer.");
                return;
            }

            // Display player choose            
            rtbMessages.Text += "You: ";
            rtbMessages.Text += playerActualLine + Environment.NewLine;
            ScrollToBottomOfMessages();
            
            // Determining the character's next message depending on the player's response.
            if (GuiderResponses.ContainsKey(playerActualLine)
                ||FarmerResponses.ContainsKey(playerActualLine)
                ||AlchemistResponses.ContainsKey(playerActualLine)
                ||GuarderResponses.ContainsKey(playerActualLine))
            {
                rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                if (GuiderResponses.ContainsKey(playerActualLine))
                {
                    rtbMessages.Text += GuiderResponses[playerActualLine] + Environment.NewLine;
                }
                if(FarmerResponses.ContainsKey(playerActualLine))
                {
                    rtbMessages.Text += FarmerResponses[playerActualLine] + Environment.NewLine;
                }
                if (AlchemistResponses.ContainsKey(playerActualLine))
                {
                    rtbMessages.Text += AlchemistResponses[playerActualLine] + Environment.NewLine;
                }
                if (GuarderResponses.ContainsKey(playerActualLine))
                {
                    rtbMessages.Text += GuarderResponses[playerActualLine] + Environment.NewLine;
                }
                ScrollToBottomOfMessages();
            }

            // Finish dialogue after farewell            
            if (playerActualLine == "Good Bye." 
                || playerActualLine == "Okey, thank you, good bye." 
                || playerActualLine == "Nothing. Good Bye."
                || playerActualLine == "Yeah, i will kill them."
                || playerActualLine == "Oh, okey i will do this."
                || playerActualLine == "Okey, i will get something for self-defense. See you soon."
                || playerActualLine == "If so, good bye."
                || playerActualLine == "Oh good, i want additional reward."
                || playerActualLine == "Thanks, good bye."
                || playerActualLine == "Bye."
                || playerActualLine == "Oh, it's nice to receive this. Good bye.")
            {
                cboPlayerLine.Items.Clear();
                FirstClick = true;
                rtbLocation.Text = TheWorld.LocationByID(_player.CurrentLocation.ID).Name + Environment.NewLine;
                rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).Description + Environment.NewLine;
                rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).ShowLocation("default");

                // Implementation giving quests after corresponding player answer                
                if (playerActualLine == "Yeah, i will kill them." 
                   || playerActualLine == "Oh, okey i will do this."
                   || playerActualLine == "Oh good, i want additional reward.")
                {                    
                    _player.Quests.Add(_player.CurrentLocation.CharacterLivingHere.QuestAvailable);
                    UpdateQuestListInUI();
                    rtbMessages.Text += "\tYou received a quest \n" + _player.CurrentLocation.CharacterLivingHere.QuestAvailable.Name + Environment.NewLine;
                    rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.QuestAvailable.Description + Environment.NewLine;

                    if(playerActualLine == "Oh good, i want additional reward.")
                    {
                        firstDialogueWithGuarder = false;
                    }

                    ScrollToBottomOfMessages();
                }

                // Deleting answers line, which we choosed after completed quest(in order not to dupe the reward)                
                if (playerActualLine == "Thanks, good bye."
                    || playerActualLine == "Bye."
                    || playerActualLine == "Oh, it's nice to receive this. Good bye."
                    || playerActualLine == "Okey, thank you, good bye.")
                {
                    if (playerActualLine == "Thanks, good bye.")
                    {
                        RemoveDialogueOptions("Thanks, good bye.", ref dialogueOptionsWithFarmer, ref FarmerResponses);
                    }
                    if (playerActualLine == "Bye.")
                    {
                        RemoveDialogueOptions("Bye.", ref dialogueOptionsWithAlchemist, ref AlchemistResponses);
                    }
                    if (playerActualLine == "Oh, it's nice to receive this. Good bye.")
                    {
                        RemoveDialogueOptions("Oh, it's nice to receive this. Good bye.", ref dialogueOptionsWithGuarder, ref GuarderResponses);
                        _player.AddItemToInventory(TheWorld.ItemByID(TheWorld.ITEM_ID_METHAMPHETAMINE));
                        UpdateInventoryListInUI();
                        UpdatePotionListInUI();
                        rtbMessages.Text += "Your thoughts: Oh shit, it's meth. May be i can try it on a location with monsters.\n";
                    }
                    if(playerActualLine == "Okey, thank you, good bye.")
                    {
                        _player.AddItemToInventory(TheWorld.ItemByID(TheWorld.ITEM_ID_HEALING_POTION));
                        UpdateInventoryListInUI();                        
                        UpdatePotionListInUI();
                    }
                }

                rtbMessages.Text += "" + Environment.NewLine; // Indentation after end of dialogue
                ScrollToBottomOfMessages();

                // Show/hide available movement buttons
                btnNorth.Visible = (_player.CurrentLocation.LocationToNorth != null);
                btnEast.Visible = (_player.CurrentLocation.LocationToEast != null && IsPlayerCanMoveToBridge());
                btnSouth.Visible = (_player.CurrentLocation.LocationToSouth != null);
                btnWest.Visible = (_player.CurrentLocation.LocationToWest != null);

                return;
            }

            else if (_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_GUIDER))
            {
                if (dialogueOptionsWithGuider.ContainsKey(playerActualLine))
                {
                    if (playerActualLine == "Hi. What is this place?")
                    {
                        UpdateComboBoxOptions(dialogueOptionsWithGuider[playerActualLine]);
                    }
                    else if (new[] { "West", "North", "East", "South" }.Contains(playerActualLine))
                    {
                        chosenVariants.Add(playerActualLine);
                        UpdateComboBoxOptions(GetNextVariantOptions());
                    }
                }
            }
            else if(_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_FARMER))
            {
                if (dialogueOptionsWithFarmer.ContainsKey(playerActualLine))
                {
                    if (playerActualLine == "Hello. I heard you complain about rats in the fields.")
                    {
                        UpdateComboBoxOptions(dialogueOptionsWithFarmer[playerActualLine]);
                    }
                    else if(playerActualLine == "I got rid of the rats that ate your seeds.")
                    {
                        UpdateComboBoxOptions(dialogueOptionsWithFarmer[playerActualLine]);
                        FinishQuest();
                    }
                }                
            }
            else if (_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_ALCHEMIST))
            {
                if (dialogueOptionsWithAlchemist.ContainsKey(playerActualLine))
                {
                    if (playerActualLine == "Hello. Can you tell me about your ground a little?")
                    {
                        UpdateComboBoxOptions(dialogueOptionsWithAlchemist[playerActualLine]);
                    }
                    else if (playerActualLine == "I'm loocking for a ways to earn extra money. Maybe you can give some work for me?")
                    {                       
                        int delay = 1500;
                        // Первая задержка и вывод сообщения
                        await Task.Delay(delay);
                        rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                        rtbMessages.Text += "lucky bastard" + Environment.NewLine;
                        ScrollToBottomOfMessages();

                        // Вторая задержка и вывод следующего сообщения
                        await Task.Delay(delay);
                        rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                        rtbMessages.Text += "fucking snakes are poisoning my plants, if you kill them I'll give you some gold" + Environment.NewLine;
                        ScrollToBottomOfMessages();
                        UpdateComboBoxOptions(dialogueOptionsWithAlchemist[playerActualLine]);
                    }
                    else if (playerActualLine == "I killed snakes.")
                    {
                        UpdateComboBoxOptions(dialogueOptionsWithAlchemist[playerActualLine]);
                        FinishQuest();
                    }
                }
            }
            else if (_player.CurrentLocation.CharacterLivingHere == TheWorld.CharacterByID(TheWorld.CHARACTER_ID_GUARDER))
            {
                if (dialogueOptionsWithGuarder.ContainsKey(playerActualLine))
                {
                    if (playerActualLine == "Why?")
                    {
                        UpdateComboBoxOptions(dialogueOptionsWithGuarder[playerActualLine]);
                    }
                    else if (playerActualLine == "Oh, i can self-defense. I have a sword and healing potion just in case.")
                    {
                        int delay = 1000;
                        // Первая задержка и вывод сообщения
                        await Task.Delay(delay);
                        rtbMessages.Text += _player.CurrentLocation.CharacterLivingHere.Name + ": ";
                        rtbMessages.Text += "And... If you kill some spiders for safety another peoples i can give you a little reward." + Environment.NewLine;
                        ScrollToBottomOfMessages();

                        UpdateComboBoxOptions(dialogueOptionsWithGuarder[playerActualLine]);
                    }
                    else if (playerActualLine == "I killed spiders.")
                    {
                        UpdateComboBoxOptions(dialogueOptionsWithGuarder[playerActualLine]);
                        FinishQuest();
                    }
                }
            }

            cboPlayerLine.SelectedIndex = -1;
        }

        private void btnSay_Click(object sender, EventArgs e)
        {
            if (FirstClick)
            {
                FirstClick = false;
                InitializeDialogue();
                return;
            }
           Dialogue();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Here is simple figth system              

        public void SpawnMonster(Location loc)
        {            
            if (loc.MonsterLivingHere != null)
            {
                // Make a new monster, using the values from the standard monster in the World.Monster list
                Monster standardMonster = TheWorld.MonsterByID(loc.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.MaximumHP, standardMonster.CurrentHP, standardMonster.ID, standardMonster.Name,
                    standardMonster.MaximumDamage, standardMonster.RewardExperience, standardMonster.RewardGold);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }
            }
            else
            {
                _currentMonster = null;
            }
        }

        // If we death this function works
        public void PossibleDeath()
        {
            if (_player.CurrentHP <= 0)
            {
                // Display message
                rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;

                // Move player to "Home"
                MoveTo(TheWorld.LocationByID(TheWorld.LOCATION_ID_HOME));

                _player.CurrentHP = 50;
                _player.Gold = 5;
                _player.Experience = 0;
                _player.Level = 1;

                lblHP.Text = _player.CurrentHP.ToString();
                lblGold.Text = _player.Gold.ToString();
                lblExperience.Text = _player.Experience.ToString();
                lblLevel.Text = _player.Level.ToString();
            }
        }

        // Function player turn for attack
        public bool PlayerAttack()
        {
            rtbLocation.Text = TheWorld.LocationByID(_player.CurrentLocation.ID).Name + Environment.NewLine;
            rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).Description + Environment.NewLine;
            rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).ShowLocation("fight");

            // Create a temp object weapon from selected player's weapon  
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;
            
            int damageToMonster = Engine.RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            _currentMonster.CurrentHP -= damageToMonster;
            rtbMessages.Text += "You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " damage." + Environment.NewLine;            

            if (_currentMonster.CurrentHP <= 0)
            {
                // Monster is dead                
                rtbMessages.Text += "You defeated the " + _currentMonster.Name + Environment.NewLine;

                // Give player experience and gold for killing the monster
                _player.Experience += _currentMonster.RewardExperience;
                rtbMessages.Text += "You receive " + _currentMonster.RewardExperience.ToString() + " experience points" + Environment.NewLine;
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "You receive " + _currentMonster.RewardGold.ToString() + " gold" + Environment.NewLine;

                // Create a temp list that will store the loot dropped from the monster
                List<Item> lootedItems = new List<Item>();

                // Generate loot depending on drop percentage and add to temp list
                foreach (var loot in _currentMonster.LootTable)
                {
                    if (Engine.RandomNumberGenerator.NumberBetween(1, 100) <= loot.DropPercentage)
                    {
                        lootedItems.Add(loot.Details);
                    }
                }

                // If player unlucky give him default item
                if (lootedItems.Count == 0)
                {
                    foreach (var loot in _currentMonster.LootTable)
                    {
                        if (loot.IsDefaultItem)
                        {
                            lootedItems.Add(loot.Details);
                        }
                    }
                }

                // And add looted items to inventory
                foreach (var item in lootedItems)
                {
                    _player.AddItemToInventory(item);

                    rtbMessages.Text += "You loot" + " 1 " + item.Name + Environment.NewLine;
                }

                // Refresh player information and inventory controls
                lblHP.Text = _player.CurrentHP.ToString();
                lblGold.Text = _player.Gold.ToString();
                lblExperience.Text = _player.Experience.ToString();
                lblLevel.Text = _player.Level.ToString();

                UpdateInventoryListInUI();
                rtbMessages.Text += Environment.NewLine;
                ScrollToBottomOfMessages(); 

                return false;
            }
            
            return true;
        }

        // Function monster turn for attack
        public void MonsterAttack()
        {
            // Determine the amount of damage the monster does to the player
            int damageToPlayer = Engine.RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);
            
            rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;                            
            
            _player.CurrentHP -= damageToPlayer;
            
            lblHP.Text = _player.CurrentHP.ToString();

            rtbMessages.Text += Environment.NewLine;
            PossibleDeath();
            ScrollToBottomOfMessages();
        }

        public async void Healing()
        {
            // Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;


            if (potion.ID == TheWorld.ITEM_ID_HEALING_POTION)
            {
                // Add healing amount to the player's current hit points
                _player.CurrentHP = (_player.CurrentHP + potion.AmountToHeal);

                // CurrentHitPoints cannot exceed player's MaximumHitPoints
                if (_player.CurrentHP > _player.MaximumHP)
                {
                    _player.CurrentHP = _player.MaximumHP;
                }
            }
            else if(potion.ID == TheWorld.ITEM_ID_METHAMPHETAMINE) // Fun function in end game when player complete guarder quest
            {
                btnNorth.Enabled = false;
                btnEast.Enabled = false;
                btnSouth.Enabled = false;
                btnWest.Enabled = false;

                btnSay.Enabled = false;
                btnUseWeapon.Enabled = false;
                btnUsePotion.Enabled = false;

                int a = 0;
                while (a<=50)
                {                 
                    _player.CurrentLocation.ID = TheWorld.LOCATION_ID_HOME;
                    for (int i = 1; i <= 9; i++)
                    {
                        rtbLocation.Text = TheWorld.LocationByID(_player.CurrentLocation.ID).Name + Environment.NewLine;
                        rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).Description + Environment.NewLine;
                        rtbLocation.Text += TheWorld.LocationByID(_player.CurrentLocation.ID).ShowLocation("default");
                        _player.CurrentLocation.ID++;
                        rtbMessages.Text += Engine.RandomNumberGenerator.NumberBetween(1000000, 999999999).ToString();

                        _player.AddItemToInventory(TheWorld.ItemByID(Engine.RandomNumberGenerator.NumberBetween(1, 10)));
                        _player.Quests.Add(TheWorld.QuestByID(Engine.RandomNumberGenerator.NumberBetween(1, 3)));
                        UpdateInventoryListInUI();
                        UpdateQuestListInUI();

                        _player.CurrentHP *= (_player.CurrentHP + potion.AmountToHeal);
                        _player.Experience *= 1000;
                        _player.Gold -= 100;
                        _player.Level = a;

                        lblHP.Text = _player.CurrentHP.ToString();
                        lblGold.Text = _player.Gold.ToString();
                        lblExperience.Text = _player.Experience.ToString();
                        lblLevel.Text = _player.Level.ToString();

                        btnNorth.Visible = (i == 1);
                        btnEast.Visible = (i == 3);
                        btnSouth.Visible = (i == 5);
                        btnWest.Visible = (i == 1);

                        cboPlayerLine.Visible = (i == 7);
                        cboWeapons.Visible = (i == 8);
                        cboPotions.Visible = (i == 5);

                        btnSay.Visible = (i == 1);
                        btnUseWeapon.Visible = (i == 2);
                        btnUsePotion.Visible = (i == 6);

                        dgvInventory.Visible = (i == 4);
                        dgvQuests.Visible = (i == 8);

                        ChangeColorsAsync();

                        await Task.Delay(50);
                    }                                                          

                    await Task.Delay(1);
                    a++;
                    ScrollToBottomOfMessages();
                }

                MessageBox.Show("You overdosed from methamphetamine. Game Over.");
                Application.Exit();
            }

            // Remove the potion from the player's inventory
            foreach (var ii in _player.Inventory)
            {
                if (ii.ID == potion.ID)
                {
                    ii.Count--;
                    break;
                }
            }

            // Display message
            rtbMessages.Text += "You drink a " + potion.Name + Environment.NewLine;
            ScrollToBottomOfMessages();

            lblHP.Text = _player.CurrentHP.ToString();
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }
        
        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            bool IsMonsterAlive = PlayerAttack();
            if (IsMonsterAlive)
            {
                MonsterAttack();
            }
            else
            {
                SpawnMonster(_player.CurrentLocation);
            }
            CompleteQuestCheck();
            LevelUpCheck();
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            Healing();
            MonsterAttack();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Color generator for background after meth(written entirely by chat gpt)
        private Panel panel;

        private int colorIndex = 0;

        private void ChangeColorsAsync()
        {
            panel.BackColor = GetRainbowColor(colorIndex);
            colorIndex = (colorIndex + 10) % 360;
        }

        private Color GetRainbowColor(int hue)
        {
            return ColorFromHSV(hue, 1.0, 1.0);
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int h = (int)(hue / 60) % 6;
            double f = hue / 60 - h;
            double p = value * (1 - saturation);
            double q = value * (1 - f * saturation);
            double t = value * (1 - (1 - f) * saturation);

            double r = 0, g = 0, b = 0;

            switch (h)
            {
                case 0:
                    r = value;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = value;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = value;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = value;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = value;
                    break;
                case 5:
                    r = value;
                    g = p;
                    b = q;
                    break;
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        // Idk what further functions do, but i can't delete it.
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void rtbLocation_TextChanged(object sender, EventArgs e)
        {
        }

        private void cboPlayerLine_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void dgvQuests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}