﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Messages;

namespace TCC
{
    public delegate void Scythe(int time);
    public delegate void Gamble(int time);

    public static class Warrior
    {
        static readonly uint[] GambleIDs = { 100800, 100801, 100802, 100803 };
        public static int CurrentEdge { get; set; }

        public static event Scythe Scythed;
        public static event Gamble GambleBuff;
        public static event Gamble GambleCooldown;

        public static void CheckGamble(S_ABNORMALITY_BEGIN sAbnormalityBegin)
        {
            if (GambleIDs.Contains(sAbnormalityBegin.id))
            {
                GambleBuff.Invoke(sAbnormalityBegin.duration);
            }
        }

        public static void CheckScytheAndGamble(S_START_COOLTIME_SKILL sk)
        {
            if (SkillsDatabase.SkillIdToName(sk.SkillId, Class.Warrior).Contains("Scythe I"))
            {
                Scythed?.Invoke((int)sk.Cooldown);
            }
            else if(SkillsDatabase.SkillIdToName(sk.SkillId, Class.Warrior).Contains("Deadly Gamble"))
            {
                GambleCooldown?.Invoke((int)sk.Cooldown);
            }

        }
    }
}
