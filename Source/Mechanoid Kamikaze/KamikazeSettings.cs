using UnityEngine;
using Verse;

namespace Mechanoid_Kamikaze
{
    public class SelfDestructSettings : ModSettings
    {
        public float baseRadius = 1.5f;
        public float radiusPerBodySize = 1.2f;
        public float baseDamage = 30f;
        public float damagePerBodySize = 45f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref baseRadius, "baseRadius", 1.5f);
            Scribe_Values.Look(ref radiusPerBodySize, "radiusPerBodySize", 1.2f);
            Scribe_Values.Look(ref baseDamage, "baseDamage", 30f);
            Scribe_Values.Look(ref damagePerBodySize, "damagePerBodySize", 45f);
            base.ExposeData();
        }
    }
    public class SelfDestructMod : Mod
    {
        public static SelfDestructSettings settings;

        // 输入框缓存
        private string baseRadiusBuffer;
        private string radiusPerBodySizeBuffer;
        private string baseDamageBuffer;
        private string damagePerBodySizeBuffer;

        // 默认值常量
        private const float DefaultBaseRadius = 1.5f;
        private const float DefaultRadiusPerBodySize = 1.2f;
        private const float DefaultBaseDamage = 30f;
        private const float DefaultDamagePerBodySize = 45f;

        public SelfDestructMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<SelfDestructSettings>();
            SyncBuffersFromSettings();
        }

        public override string SettingsCategory() => "KamikazeTitle".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);

            //Text.Font = GameFont.Medium;
            //list.Label("KamikazeTitle".Translate());
            //Text.Font = GameFont.Small;
            list.GapLine();

            list.Label("KamikazeDesc".Translate());

            // 基础半径
            list.Label("BasicRadius".Translate());
            list.TextFieldNumeric(ref settings.baseRadius, ref baseRadiusBuffer, 0.1f, 100f);

            // 半径系数
            list.Label("RadiusPerSize".Translate());
            list.TextFieldNumeric(ref settings.radiusPerBodySize, ref radiusPerBodySizeBuffer, 0f, 100f);

            list.Gap();

            // 基础伤害
            list.Label("BasicDamage".Translate());
            list.TextFieldNumeric(ref settings.baseDamage, ref baseDamageBuffer, 1f, 10000f);

            // 伤害系数
            list.Label("DamagePerSize".Translate());
            list.TextFieldNumeric(ref settings.damagePerBodySize, ref damagePerBodySizeBuffer, 0f, 10000f);

            list.GapLine();

            // 恢复默认值按钮
            if (list.ButtonText("Reset".Translate()))
            {
                ResetToDefaults();
            }

            list.End();
        }

        private void ResetToDefaults()
        {
            settings.baseRadius = DefaultBaseRadius;
            settings.radiusPerBodySize = DefaultRadiusPerBodySize;
            settings.baseDamage = DefaultBaseDamage;
            settings.damagePerBodySize = DefaultDamagePerBodySize;

            SyncBuffersFromSettings();
        }

        private void SyncBuffersFromSettings()
        {
            baseRadiusBuffer = settings.baseRadius.ToString();
            radiusPerBodySizeBuffer = settings.radiusPerBodySize.ToString();
            baseDamageBuffer = settings.baseDamage.ToString();
            damagePerBodySizeBuffer = settings.damagePerBodySize.ToString();
        }
    }

}
