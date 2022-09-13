using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using Kingmaker.UI.RestCamp;
using DG.Tweening;

namespace CampUseFromStashPatch
{
    static class Main
    {
        public static bool Load(UnityModManager.ModEntry modEntry)
        {
//            modEntry.OnUpdate = OnUpdate;
//            modEntry.OnGUI = OnGUI;
//            modEntry.OnHideGUI = OnHideGUI;

            Harmony harmony2 = new Harmony(modEntry.Info.Id + modEntry.Info.Author);
            harmony2.PatchAll(Assembly.GetExecutingAssembly());

            //MethodInfo original;
            //MethodInfo prefix;
            //MethodInfo postfix;

            //original = typeof(CampRations).GetMethod("Init");
            //prefix = typeof(ControlUseFromStashDefault).GetMethod("Prefix");
            //postfix = typeof(ControlUseFromStashDefault).GetMethod("Postfix");
            //harmony2.Patch(original, prefix: new HarmonyMethod(prefix), postfix: new HarmonyMethod(postfix));

            //original = typeof(CampRations).GetMethod("UseFromStashToggleChanged");
            //prefix = typeof(ControlUseFromStashDefault).GetMethod("UseFromStashToggleChangedPrefix");
            //harmony2.Patch(original, prefix: new HarmonyMethod(prefix));

            Debug.Log("CampUseFromStashPatch properly loaded");

            return true;
        }

        [HarmonyPatch(typeof(CampRations))]
        static class ControlUseFromStashDefault
        {
            private static bool isInited = false;
            private static bool lastValue = true;

            [HarmonyPrefix]
            [HarmonyPatch("Init")]
            public static void Prefix(CampRations __instance, ref bool ___m_isInit)
            {
                isInited = ___m_isInit;
            }

            [HarmonyPostfix]
            [HarmonyPatch("Init")]
            public static void Postfix(CampRations __instance)
            {
                if (!isInited)
                {
                    //Debug.Log("Init-Postfix: " + lastValue);
                    isInited = true;

                    __instance.UseFromStashToggle.isOn = lastValue;

                    //MethodInfo dynMethod = typeof(CampRations).GetMethod("UseFromStashToggleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                    //dynMethod.Invoke(__instance, new object[] { __instance.UseFromStashToggle.isOn });
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch("UseFromStashToggleChanged")]
            public static void UseFromStashToggleChangedPrefix(bool state)
            {
                //Debug.Log("UseFromStashToggleChanged: state = " + state);
                lastValue = state;
                //Debug.Log("UseFromStashToggleChanged: lastValue = " + lastValue);
            }
        }

        [HarmonyPatch(typeof(CampRations), "SetupRationsView")]
        static class UseFromStashBackGroundAlphaBugFix
        {
            public static void Postfix(CampRations __instance)
            {
                Tweener t;
                if (__instance.Calculator.UseFromStash)
                {
                    t = __instance.HaveRationsCanvasGroup.DOFade(1f, 0.2f).SetUpdate(UpdateType.Late);
                    __instance.HaveRationsBg.gameObject.SetActive(true);
                    t.PlayForward();
                }
                else
                {
                    t = __instance.HaveRationsCanvasGroup.DOFade(0.5f, 0.2f).SetUpdate(UpdateType.Late);
                    __instance.HaveRationsBg.gameObject.SetActive(false);
                    t.PlayForward();
                }
            }
        }
    }
}