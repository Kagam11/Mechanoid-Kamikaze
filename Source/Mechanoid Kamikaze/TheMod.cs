using Force.DeepCloner;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Mechanoid_Kamikaze
{
    public class Verb_SelfExplode : Verb_CastAbility
    {
        // 直接允许无目标释放
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return true;
        }

        //public override void OrderForceTarget(LocalTargetInfo target)
        //{
        //    base.OrderForceTarget(CasterPawn);
        //}

        protected override bool TryCastShot()
        {
            var pawn = CasterPawn;
            if (pawn == null || !pawn.Spawned || pawn.Map == null) return false;

            var settings = SelfDestructMod.settings;
            float bodySize = Mathf.Max(0.01f, pawn.BodySize);
            float radius = settings.baseRadius + settings.radiusPerBodySize * bodySize; // 半径随体型线性增长
            int damage = Mathf.RoundToInt(settings.baseDamage + settings.damagePerBodySize * bodySize); // 伤害随体型线性增长

            GenExplosion.DoExplosion(
                center: pawn.Position,
                map: pawn.Map,
                radius: radius,
                damType: DamageDefOf.Bomb,
                instigator: pawn,
                damAmount: damage,
                armorPenetration: 1.0f
            );

            // 确保自杀
            if (!pawn.Dead)
            {
                var dinfo = new DamageInfo(DamageDefOf.Bomb, 99999f, armorPenetration: 1.0f, instigator: pawn);
                pawn.Kill(dinfo);
            }

            return true;
        }

    }

    [StaticConstructorOnStartup]
    public static class Patching
    {
        static Patching()
        {
            var settings = SelfDestructMod.settings;
            var defs = DefDatabase<PawnKindDef>.AllDefsListForReading;
            var mechs = defs.FindAll(d => d?.race?.race != null && (d?.race?.race?.IsMechanoid ?? false));
            foreach (var mech in mechs)
            {
                if (mech.abilities == null) mech.abilities = new List<AbilityDef>();
                if (!mech.abilities.Contains(KamikazeAbilityDefOf.Mech_SelfExplode))
                {
                    var size = mech.race.race.baseBodySize;
                    var radius = settings.baseRadius + settings.radiusPerBodySize * size;

                    var ability = KamikazeAbilityDefOf.Mech_SelfExplode.DeepClone();
                    ability.verbProperties.range = radius;
                    // make a copy of the ability def to avoid modifying the original
                    mech.abilities.Add(ability);
                    //Log.Message($"[Mechanoid Kamikaze] Added Mech_SelfDestruct to {mech.defName}.");
                }
            }
            //Log.Message("[Mechanoid Kamikaze] Loaded.");
        }
    }

    [DefOf]
    public static class KamikazeAbilityDefOf
    {
        public static AbilityDef Mech_SelfExplode;
        static KamikazeAbilityDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AbilityDefOf));
        }
    }
}