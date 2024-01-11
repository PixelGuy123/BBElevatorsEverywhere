using BepInEx;
using HarmonyLib;

namespace BBElevatorsEverywhere.Plugin
{
	[BepInPlugin(ModInfo.GUID, ModInfo.Name, ModInfo.Version)]
	public class BasePlugin : BaseUnityPlugin
	{
		void Awake()
		{
			i = this;

			Harmony harmony = new(ModInfo.GUID);
			harmony.PatchAll();
		}

		public static BasePlugin i;
	}


	internal static class ModInfo
	{
		internal const string GUID = "pixelguy.pixelmodding.baldiplus.bbelvseverywhere";
		internal const string Name = "BB+ Elevators & Field Trips everywhere";
		internal const string Version = "1.0.0";
	}
}
