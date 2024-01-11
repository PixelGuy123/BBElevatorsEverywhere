using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace BBElevatorsEverywhere.Patches
{
	[HarmonyPatch(typeof(LevelGenerator), "Generate", MethodType.Enumerator)]
	internal class ElevatorsEverywhere
	{
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ElevatorsIsSpawningEverywehre(IEnumerable<CodeInstruction> instructions, ILGenerator gen) =>
		
		new CodeMatcher(instructions, gen).MatchForward(false,
			// Elevators Everywhere
				new CodeMatch(OpCodes.Ldc_I4_0), // Gets the first local variable assignment
				new CodeMatch(OpCodes.Ldloc_2),
				new CodeMatch(OpCodes.Ldflda, AccessTools.Field(typeof(LevelBuilder), "levelSize")),
				new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(IntVector2), "x")),
				new CodeMatch(OpCodes.Ldc_I4_1),
				new CodeMatch(OpCodes.Sub),
				new CodeMatch(OpCodes.Ldloc_S, name: "V_87"), // Idk how to check local variables, so i'll just get by the name in the decompiler
				new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(IntVector2), "x")),
				new CodeMatch(OpCodes.Mul),
				new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Mathf), "Max", [typeof(int), typeof(int)])))
			.RemoveInstructions(10) // Remove the entire assignment
			.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0)) // Put a single 0 as assignment
			.RemoveInstructions(14) // Removes the second Assignment
			.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2),
				new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(LevelBuilder), "levelSize")),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IntVector2), "x")))
			.Advance(1) // Skips that one instruction
			.RemoveInstructions(10) // Removes the third assignment
			.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0)) // assignment to 0
			.Advance(1) // Skips the one instruction again
			.RemoveInstructions(14) // Removes the last assignment
			.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2),
				new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(LevelBuilder), "levelSize")),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IntVector2), "z")))
			// A bugfix to stop these elevators from crashing

			.MatchForward(true, // Change the last codematch
				new CodeMatch(OpCodes.Ldloc_2), // Moves an if block to another if block inside
				new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LevelBuilder), "ld")),
				new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LevelObject), "elevatorPre")),
				new CodeMatch(OpCodes.Ldloc_S, name: "V_84"), // Local variable
				new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Directions), "GetOpposite", [typeof(Direction)])),
				new CodeMatch(OpCodes.Ldelem_Ref),
				new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Elevator), "TileGroup")),
				new CodeMatch(OpCodes.Ldloc_S, name: "V_95")
				/*, Removed due to the line below >:(
				new CodeMatch(OpCodes.Ldloc_S), //name: "V_84" removed to avoid exceptions
				new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Directions), "ToIntVector2", [typeof(Direction)])),
				new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(IntVector2), "op_Addition", [typeof(IntVector2), typeof(IntVector2)])),
				new CodeMatch(OpCodes.Ldloc_2),
				new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LevelBuilder), "ec")),
				new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(TileGroup), "Fits", [typeof(IntVector2), typeof(EnvironmentController)])),
				new CodeMatch(OpCodes.Brfalse_S, name: "IL_25BE")
				*/
				)
			.Advance(7)
			.SetJumpTo(OpCodes.Brfalse_S, 3130, out _) // Should in theory, jump to the end of the for loop, making the second if inside the first
			// Field trip stuff
			.MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(LevelBuilder), "CreateTripEntrance")))
			.Advance(-34) // Goes to an if block near it
			.RemoveInstructions(4) // Removes one of the if conditions

			.InstructionEnumeration();
		
	}

	[HarmonyPatch(typeof(LevelBuilder))]
	internal class EdgeTilesPatch
	{
		[HarmonyPatch("edgeTilesNorth")]
		[HarmonyPatch("edgeTilesEast")]
		[HarmonyPatch("edgeTilesSouth")]
		[HarmonyPatch("edgeTilesWest")]
		[HarmonyPrefix]
		private static bool NuhUh() => false;

		[HarmonyPatch("edgeTilesNorth")]
		[HarmonyPatch("edgeTilesEast")]
		[HarmonyPatch("edgeTilesSouth")]
		[HarmonyPatch("edgeTilesWest")]
		[HarmonyPostfix]
		private static void Alltiles(ref TileController[] __result, EnvironmentController ___ec, List<RoomController> ___specialRooms)
		{
			List<TileController> tiles = ___ec.mainHall.GetNewTileList();
			___specialRooms.ForEach(x => tiles.AddRange(x.GetNewTileList().Where(z => !___ec.GetTileNeighbors(z.position).All(s => s != null)))); // Basically only tiles that are next to any null tile
			__result = [.. tiles];
		}
	}
}
