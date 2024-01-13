using HarmonyLib;
using UnityEngine;

namespace BBElevatorsEverywhere.Extensions
{
	public static class MatchDebugExtensions
	{
		public static CodeMatcher LogAll(this CodeMatcher m, bool beginFromStart = false, int count = 0)
		{
			var clone = m.Clone();

			if (beginFromStart)
				clone.Start();

			if (count <= 0)
				count = clone.Remaining;

			int i = 0;

			while (clone.IsValid && i++ <= count)
			{
				Debug.Log($"{clone.Pos}: {clone.Opcode} | {clone.Operand}");
				clone.Advance(1);
			}

			return m;

		}
	}
}
