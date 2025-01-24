using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Verse.AI;
using RimWorld.Planet;

namespace NzFaceLessManMod
{
    public class FacelessManMod : Mod
    {

        public FacelessManMod(ModContentPack content) : base(content)
        {
            #if DEBUG
            Log.Message("FacelessManMod: loaded");
            #endif
            var harmony = new Harmony("nightz.facelessman");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main()
        {
            Log.Message("FacelessManMod: StaticConstructorOnStartup");
            // var harmony = new Harmony("nightz.facelessman");
            // harmony.PatchAll(Assembly.GetExecutingAssembly());
            // #if DEBUG
            // Log.Message("FacelessManMod loaded");
            // #endif
        }
    }

}