﻿using TCC.Data.Skills;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class ReaperAbnormalityTracker : AbnormalityTracker
    {
        private const int ShadowReapingId = 10151010;
        private const int ShadowStepId = 10151000;
        private const int DeathSpiralId = 10151131;
        private const int AssassinateId = 10151192;

        private readonly Skill _shadowStep;
        private readonly Skill _deathSpiral;

        public ReaperAbnormalityTracker()
        {
            Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.chainbrandish_tex", Game.Me.Class, out _deathSpiral);
            Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.instantleap_tex", Game.Me.Class, out _shadowStep);

        }
        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckShadowReaping(p);
            CheckShadowStep(p);
            CheckDeathSpiral(p);
            CheckAssassinate(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckShadowReaping(p);
            CheckAssassinate(p);

        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckShadowReaping(p);
            CheckAssassinate(p);

        }
        private void CheckDeathSpiral(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != DeathSpiralId) return;
            StartPrecooldown(_deathSpiral, p.Duration);
        }
        private void CheckShadowStep(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != ShadowStepId) return;
            StartPrecooldown(_shadowStep, p.Duration);
        }


        private static void CheckAssassinate(S_ABNORMALITY_BEGIN p)
        {
            if (AssassinateId != p.AbnormalityId) return;
            if (!IsViewModelAvailable<ReaperLayoutVM>(out var vm)) return;

            vm.ShroudedEscape.StartEffect(p.Duration);
        }
        private static void CheckAssassinate(S_ABNORMALITY_REFRESH p)
        {
            if (AssassinateId != p.AbnormalityId) return;
            if (!IsViewModelAvailable<ReaperLayoutVM>(out var vm)) return;

            vm.ShroudedEscape.RefreshEffect(p.Duration);
        }
        private static void CheckAssassinate(S_ABNORMALITY_END p)
        {
            if (AssassinateId != p.AbnormalityId) return;
            if (!IsViewModelAvailable<ReaperLayoutVM>(out var vm)) return;

            vm.ShroudedEscape.StopEffect();
        }
        private static void CheckShadowReaping(S_ABNORMALITY_BEGIN p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            if (!IsViewModelAvailable<ReaperLayoutVM>(out var vm)) return;

            vm.ShadowReaping.StartEffect(p.Duration);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_REFRESH p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            if (!IsViewModelAvailable<ReaperLayoutVM>(out var vm)) return;

            vm.ShadowReaping.RefreshEffect(p.Duration);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_END p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            if (!IsViewModelAvailable<ReaperLayoutVM>(out var vm)) return;

            vm.ShadowReaping.StopEffect();
        }

    }
}
