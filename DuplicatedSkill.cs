using BepInEx;
using HarmonyLib;
using GameDataEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Configuration;
using I2.Loc;
using System.Reflection;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UseItem;

namespace Remove_hitcap
{
    [BepInPlugin(GUID, "DuplicateItem", version)]
    [BepInProcess("ChronoArk.exe")]
    public class DuplicateItemPlugin : BaseUnityPlugin
    {

        public const string GUID = "catrice.DuplicateItem";
        public const string version = "1.0.0";


        private static readonly Harmony harmony = new Harmony(GUID);

        private static BepInEx.Logging.ManualLogSource logger;

        private static ConfigEntry<bool> removeHitcapForEnemies;

        void Awake()
        {
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        [HarmonyPatch(typeof(SkillBookCharacter_Rare), "Use")]
        class SkillBookHook
        {
            static bool Prefix(SkillBookCharacter_Rare __instance, ref bool __result)
            {

				List<Skill> list = new List<Skill>();
                List<BattleAlly> battleallys = PlayData.Battleallys;
                BattleTeam tempBattleTeam = PlayData.TempBattleTeam;
                for (int i = 0; i < PlayData.TSavedata.Party.Count * 2; i++)
                {
                    bool flag = false;
                    if (!flag)
                    {
                        list.Add(Skill.TempSkill(PlayData.GetMySkills(PlayData.TSavedata.Party[i].KeyData, true).Random<GDESkillData>().KeyID, battleallys[i], tempBattleTeam));
                    }
                }
                if (list.Count == 0)
                {
                    EffectView.SimpleTextout(FieldSystem.instance.TopWindow.transform, ScriptLocalization.System.CantRareSkill, 1f, false, 1f);
                    __result = false;
                    return false;
                }
                foreach (Skill skill in list)
                {
                    if (!SaveManager.IsUnlock(skill.MySkill.KeyID, SaveManager.NowData.unlockList.SkillPreView))
                    {
                        SaveManager.NowData.unlockList.SkillPreView.Add(skill.MySkill.KeyID);
                    }
                }
                DarkTonic.MasterAudio.MasterAudio.PlaySound("BookFlip", 1f, null, 0f, null, null, false, false);
                FieldSystem.DelayInput(BattleSystem.I_OtherSkillSelect(list, new SkillButton.SkillClickDel(__instance.SkillAdd), ScriptLocalization.System_Item.SkillAdd, false, true, true, true, true));
                __result = true;
                return false;
			}
        }




    }
}
