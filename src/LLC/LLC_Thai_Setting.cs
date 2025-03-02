﻿using HarmonyLib;
#if ML
using Il2Cpp;
using Il2CppLocalSave;
using Il2CppMainUI;
using MelonLoader;
using Il2CppTMPro;
#elif BIE
using LocalSave;
using MainUI;
using BepInEx.Configuration;
using TMPro;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LimbusLocalize
{
    public static class LLC_Thai_Setting
    {
#if ML
        public static MelonPreferences_Entry<bool> IsUseChinese = LCB_LLCMod.LLC_Settings.CreateEntry("IsUseChinese", true, null, "是否使用汉化 ( true | false )");
#elif BIE
        public static ConfigEntry<bool> IsUseChinese = LCB_LLCMod.LLC_Settings.Bind("LLC Settings", "IsUseChinese", true, "是否使用汉化 ( true | false )");
#endif
        static bool _isusechinese;
        static Toggle Chinese_Setting;
        [HarmonyPatch(typeof(SettingsPanelGame), nameof(SettingsPanelGame.InitLanguage))]
        [HarmonyPrefix]
        private static bool InitLanguage(SettingsPanelGame __instance, LocalGameOptionData option)
        {
            if (!Chinese_Setting)
            {
                Toggle original = __instance._languageToggles[0];
                Transform parent = original.transform.parent;
                var _languageToggle = UnityEngine.Object.Instantiate(original, parent);
                var cntmp = _languageToggle.GetComponentInChildren<TextMeshProUGUI>(true);
                cntmp.font = LCB_Thai_Font.tmpthaifonts[1];
                cntmp.fontMaterial = LCB_Thai_Font.tmpthaifonts[1].material;
                cntmp.fontSize = 39;
                cntmp.fontSizeMax = 39;
                cntmp.text = "ภาษาไทย";
                Chinese_Setting = _languageToggle;
                
                parent.localPosition = new Vector3(parent.localPosition.x - 352f, parent.localPosition.y, parent.localPosition.z);
                while (__instance._languageToggles.Count > 3)
                    __instance._languageToggles.RemoveAt(__instance._languageToggles.Count - 1);
                __instance._languageToggles.Add(_languageToggle);
            }
            foreach (Toggle tg in __instance._languageToggles)
            {
                tg.onValueChanged.RemoveAllListeners();
                Action<bool> onValueChanged = (bool isOn) =>
                {
                    if (!isOn)
                        return;
                    __instance.OnClickLanguageToggleEx(__instance._languageToggles.IndexOf(tg));
                };
                tg.onValueChanged.AddListener(onValueChanged);
                tg.SetIsOnWithoutNotify(false);
            }
            LOCALIZE_LANGUAGE language = option.GetLanguage();
            if (_isusechinese = IsUseChinese.Value)
                Chinese_Setting.SetIsOnWithoutNotify(true);
            else if (language == LOCALIZE_LANGUAGE.KR)
                __instance._languageToggles[0].SetIsOnWithoutNotify(true);
            else if (language == LOCALIZE_LANGUAGE.EN)
                __instance._languageToggles[1].SetIsOnWithoutNotify(true);
            else if (language == LOCALIZE_LANGUAGE.JP)
                __instance._languageToggles[2].SetIsOnWithoutNotify(true);
            __instance._lang = language;
            return false;
        }
        [HarmonyPatch(typeof(SettingsPanelGame), nameof(SettingsPanelGame.ApplySetting))]
        [HarmonyPostfix]
        private static void ApplySetting()
        {
            IsUseChinese.Value = _isusechinese;
        }
        private static void OnClickLanguageToggleEx(this SettingsPanelGame __instance, int tgIdx)
        {
            if (tgIdx == 3)
            {
                _isusechinese = true;
                return;
            }
            _isusechinese = false;
            if (tgIdx == 0)
                __instance._lang = LOCALIZE_LANGUAGE.KR;
            else if (tgIdx == 1)
                __instance._lang = LOCALIZE_LANGUAGE.EN;
            else if (tgIdx == 2)
                __instance._lang = LOCALIZE_LANGUAGE.JP;
        }
    }
}