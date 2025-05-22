using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using SpaceEngineers.Game.World;
using Torch;
using Torch.API;
using HarmonyLib;
using System.Linq;

namespace Auto_Respawn_Anywhere
{
	public class Auto_Respawn_Anywhere : TorchPluginBase
	{
		public override void Init(ITorchBase torch)
		{
			new Harmony("AutorespawnPatch").Patch(AccessTools.Method(typeof(MySpaceRespawnComponent), "RespawnRequest_Implementation"), null, null, new HarmonyMethod(typeof(Auto_Respawn_Anywhere), nameof(CustomRespawnRequestPatch)));
		}

		// Removes the "Not Ready" nonsense from spawn points when autorespawn is enabled
		public static IEnumerable<CodeInstruction> CustomRespawnRequestPatch(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo respawn = AccessTools.Field(typeof(MySpaceRespawnComponent.MyRespawnPointInfo), "MedicalRoomId");
			List<CodeInstruction> list = instructions.ToList();
			int index = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if ((list[i].opcode == OpCodes.Ldloc_S && ((LocalBuilder)list[i].operand).LocalIndex == 8) &&
					(list[i + 1].opcode == OpCodes.Ldfld && (FieldInfo)list[i + 1].operand == respawn))
				{
					index = i;
					break;
				}
			}

			list[index] = new CodeInstruction(OpCodes.Ldc_I4_0);
			list[index + 1] = new CodeInstruction(OpCodes.Conv_I8);
			return list;
		}
	}
}
