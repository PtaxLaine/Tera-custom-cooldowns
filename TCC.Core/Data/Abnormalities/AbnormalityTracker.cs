﻿using System;
using System.Collections.Generic;
using TCC.Data.Skills;
using TCC.Utilities;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class AbnormalityTracker
    {
        protected static readonly List<ulong> MarkedTargets = new List<ulong>();
        public static event Action<ulong> MarkingRefreshed = null!;
        public static event Action MarkingExpired = null!;
        public static event Action<Skill, uint> PrecooldownStarted = null!;

        public virtual void CheckAbnormality(S_ABNORMALITY_BEGIN p) { }
        public virtual void CheckAbnormality(S_ABNORMALITY_REFRESH p) { }
        public virtual void CheckAbnormality(S_ABNORMALITY_END p) { }

        protected static bool CheckByIconName(uint id, string iconName)
        {
            if (!Game.DB.AbnormalityDatabase.Abnormalities.TryGetValue(id, out var ab)) return false;
            if (ab.Infinity) return false;
            return ab.IconName == iconName;
        }

        protected static void InvokeMarkingExpired() => MarkingExpired?.Invoke();
        protected static void InvokeMarkingRefreshed(ulong duration) => MarkingRefreshed?.Invoke(duration);
        public static void CheckMarkingOnDespawn(ulong target)
        {
            if (!MarkedTargets.Contains(target)) return;
            MarkedTargets.Remove(target);
            if (MarkedTargets.Count == 0) InvokeMarkingExpired();
        }

        public static void ClearMarkedTargets()
        {
            App.BaseDispatcher.Invoke(() => MarkedTargets.Clear());
        }
        protected static void StartPrecooldown(Skill sk, uint duration)
        {
            PrecooldownStarted?.Invoke(sk, duration);
            //SkillManager.AddSkillDirectly(sk, duration, CooldownType.Skill, CooldownMode.Pre); 
        }
        public AbnormalityTracker()
        {
            ClearMarkedTargets();
        }

        protected static bool IsViewModelAvailable<T>(out T? vm) where T : BaseClassLayoutVM
        {
            vm = TccUtils.CurrentClassVM<T>();
            return vm != null;
        }
    }
}