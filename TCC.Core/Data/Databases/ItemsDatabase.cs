﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.Data.Databases
{
    public class ItemsDatabase : DatabaseBase
    {
        protected override string FolderName => "items";
        protected override string Extension => "tsv";

        public readonly Dictionary<uint, Item> Items;
        public ItemsDatabase(string lang) : base(lang)
        {
            Items = new Dictionary<uint, Item>();
        }

        public string GetItemName(uint id)
        {
            return Items.TryGetValue(id, out var item) ? item.Name : "Unknown";
        }
        public bool TryGetItemSkill(uint itemId, out Skill sk)
        {
            sk = new Skill(0, Class.None, string.Empty, string.Empty);
            if (!Items.TryGetValue(itemId, out var item)) return false;
            sk = new Skill(itemId, Class.Common, item.Name, "") { IconName = item.IconName };
            return true;

        }

        public override void Load()
        {
            Items.Clear();
            //var f = File.OpenText(FullPath);
            var lines = File.ReadAllLines(FullPath);
            foreach (var line in lines)
            {
                //var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                var id = uint.Parse(s[0]); //Convert.ToUInt32(s[0]);
                var grad = uint.Parse(s[1]);// Convert.ToUInt32(s[1]);
                var name = s[2];
                var expId = uint.Parse(s[3]); // Convert.ToUInt32(s[3]);
                var cd = uint.Parse(s[4]); //Convert.ToUInt32(s[4]);
                var icon = s[5];

                var item = new Item(id, name, (RareGrade)grad, expId, cd, icon);
                Items[id] =  item;
            }

            AddOverride(new Item(149644, "Harrowhold Rejuvenation Potion", RareGrade.Uncommon, 0, 30, "icon_items.potion1_tex"));
            AddOverride(new Item(139520, "Minify", RareGrade.Common, 0, 3, "icon_items.icon_janggoe_item_tex_minus"));
        }

        private void AddOverride(Item item)
        {
            Items[item.Id] = item;
        }

        public IEnumerable<Item> ItemSkills
        {
            get
            {
                //var ret = new List<Item>();
                //foreach (var item in Items.Values)
                //{
                //    var iconName = "unknown";
                //    if (item.IconName.ToString() != "")
                //    {
                //        iconName = item.IconName.ToString();
                //        iconName = iconName.Replace(".", "/");
                //    }
                //    if (File.Exists(Path.GetDirectoryName(typeof(App).Assembly.Location)+ "/resources/images/" + iconName+ ".png")) ret.Add(item);
                //}

                //return ret;
                return Items.Values.Where(x => x.Cooldown > 0).ToList();
            }
        }
    }
}
