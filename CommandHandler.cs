using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking;
using BeerMP.Networking.Managers;
using BeerMP.Networking.PlayerManagers;
using Steamworks;
using UnityEngine;

namespace BeerMP
{
	// Token: 0x0200003E RID: 62
	internal static class CommandHandler
	{
		// Token: 0x0600013B RID: 315 RVA: 0x00006BAC File Offset: 0x00004DAC
		public static void Execute(string command, params string[] args)
		{
			int num = -1;
			for (int i = 0; i < CommandHandler.commands.Length; i++)
			{
				if (CommandHandler.commands[i].name == command)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				Console.LogError("Unknown command '" + command + "'", true);
				return;
			}
			if (args.Length < CommandHandler.commands[num].argCountMin || args.Length > CommandHandler.commands[num].argCountMax)
			{
				Console.LogError("Invalid syntax. Please use: " + CommandHandler.commands[num].usage, true);
				return;
			}
			if (CommandHandler.commands[num].isHostOnly && !BeerMPGlobals.IsHost)
			{
				Console.LogError("This command can only be executed by the lobby host!", true);
				return;
			}
			Action<string[]> handler = CommandHandler.commands[num].handler;
			if (handler == null)
			{
				return;
			}
			handler(args);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00006C90 File Offset: 0x00004E90
		// Note: this type is marked as 'beforefieldinit'.
		static CommandHandler()
		{
		}

		// Token: 0x0400012D RID: 301
		private static CommandHandler.Command[] commands = new CommandHandler.Command[]
		{
			new CommandHandler.Command("/tp", "/tp <player>", 1, false, delegate(string[] args)
			{
				CSteamID csteamID = default(CSteamID);
				bool flag = false;
				foreach (KeyValuePair<CSteamID, string> keyValuePair in NetManager.playerNames)
				{
					if (keyValuePair.Value.ToLower() == args[0].ToLower())
					{
						csteamID = keyValuePair.Key;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Console.Log("Player " + args[0] + " can't be found", true);
					return;
				}
				Transform transform = NetManager.GetPlayerComponentById<NetPlayer>(csteamID).player.transform;
				LocalNetPlayer.Instance.player.Value.transform.position = transform.position;
				Console.Log("Teleported to " + args[0], true);
			}),
			new CommandHandler.Command("/rbhash", "/rbhash <hash>", 1, false, delegate(string[] args)
			{
				int num;
				if (int.TryParse(args[0], out num))
				{
					NetRigidbodyManager.OwnedRigidbody ownedRigidbody = NetRigidbodyManager.GetOwnedRigidbody(num);
					Console.Log(string.Format("The object of hash {0} is {1}, full path: {2}. DEBUG: {3}, {4}, {5}, {6}, {7}, {8}", new object[]
					{
						num,
						ownedRigidbody.transform.name,
						ownedRigidbody.transform.GetGameobjectHashString(),
						ownedRigidbody.remove == null,
						ownedRigidbody.Removal_Part == null,
						ownedRigidbody.Removal_Rigidbody == null,
						ownedRigidbody.rigidbody == null,
						ownedRigidbody.rigidbodyPart == null,
						ownedRigidbody.transform == null
					}), true);
					return;
				}
				Console.LogError("The argument is not a number!", true);
			}),
			new CommandHandler.Command("/sethand", "/sethand <player> <left = true|right = false> <true|false>", 3, false, delegate(string[] args)
			{
				bool flag2;
				if (!bool.TryParse(args[1], out flag2))
				{
					Console.LogError("The hand type argument is not a bool!", true);
				}
				bool flag3;
				if (!bool.TryParse(args[2], out flag3))
				{
					Console.LogError("The value argument is not a bool!", true);
				}
				CSteamID csteamID2 = default(CSteamID);
				bool flag4 = false;
				foreach (KeyValuePair<CSteamID, string> keyValuePair2 in NetManager.playerNames)
				{
					if (keyValuePair2.Value.ToLower() == args[0].ToLower())
					{
						csteamID2 = keyValuePair2.Key;
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					Console.Log("Player " + args[0] + " can't be found", true);
					return;
				}
				PlayerAnimationManager playerAnimationManager = NetManager.GetPlayerComponentById<NetPlayer>(csteamID2).playerAnimationManager;
				if (flag2)
				{
					playerAnimationManager.leftHandOn = flag3;
				}
				else
				{
					playerAnimationManager.rightHandOn = flag3;
				}
				Console.Log(string.Format("Set {0} hand of {1} to {2}", flag2 ? "left" : "right", args[0], flag3), true);
			}),
			new CommandHandler.Command("/resetgrab", "/resetgrab", 0, false, delegate(string[] args)
			{
				PlayerGrabbingManager.handFSM.SendEvent("FINISHED");
				Console.Log("Successfully attempted to reset grabbing fsm", true);
			}),
			new CommandHandler.Command("/satprof", "/satprof", 0, false, delegate(string[] args)
			{
				SatsumaProfiler.Instance.PrintToFile();
				Console.Log("Saved last 10 seconds of satsuma behaviour to satsuma_profiler.txt", true);
			}),
			new CommandHandler.Command("/bprofiler", "/bprofiler <start|stop>", 1, false, delegate(string[] args)
			{
				string text = args[0].ToLower();
				if (text == "start")
				{
					BProfiler.Start();
					return;
				}
				if (!(text == "stop"))
				{
					Console.LogError("Invalid argument, please use either start or stop", true);
					return;
				}
				BProfiler.Stop("BeerMP_Profiler_results.txt");
			}),
			new CommandHandler.Command("/pvc", "/pvc <enable true/false|pushtotalk true/false|changekey|mastervolume 0-10>", 1, 2, false, delegate(string[] args)
			{
				if (args[0].ToLower() != "changekey" && args.Length < 2)
				{
					Console.LogError("Invalid syntax. Please use: /pvc <enable true/false|pushtotalk true/false|changekey|mastervolume 0-10>", true);
					return;
				}
				string text2 = args[0].ToLower();
				if (text2 == "enable")
				{
					bool flag5 = bool.Parse(args[1]);
					ProximityVoiceChat.SetEnabled(flag5);
					Console.Log((flag5 ? "Enabled" : "Disabled") + " voice chat", true);
					return;
				}
				if (text2 == "pushtotalk")
				{
					bool flag6 = bool.Parse(args[1]);
					ProximityVoiceChat.SetPushToTalk(flag6);
					Console.Log((flag6 ? "Enabled" : "Disabled") + " push to talk", true);
					return;
				}
				if (text2 == "changekey")
				{
					ProximityVoiceChat.ChangePTT_Keybing();
					Console.Log("Press the key you want to assign Push to talk to", true);
					return;
				}
				if (!(text2 == "mastervolume"))
				{
					return;
				}
				float? num2 = new float?(float.Parse(args[1]));
				if (num2 != null)
				{
					ProximityVoiceChat.SetMasterVolume(num2.Value);
					Console.Log(string.Format("Changed voice chat volume to {0}", num2.Value), true);
					return;
				}
				Console.LogError("The value " + args[1] + " is not a number!", true);
			}),
			new CommandHandler.Command("/vehlist", "/vehlist", 0, false, delegate(string[] args)
			{
				for (int i = 0; i < NetVehicleManager.vehicles.Count; i++)
				{
					Console.Log(string.Format("{0} = {1}", i, NetVehicleManager.vehicles[i].transform.name), true);
				}
			}),
			new CommandHandler.Command("/resetdm", "/resetdm <vehicle index>", 1, false, delegate(string[] args)
			{
				int num3 = int.Parse(args[0]);
				NetVehicleManager.vehicles[num3].driver = 0UL;
				Console.Log("Successfully resetted " + NetVehicleManager.vehicles[num3].transform.name + " driving mode", true);
			})
		};

		// Token: 0x020000E0 RID: 224
		internal struct Command
		{
			// Token: 0x060004A4 RID: 1188 RVA: 0x0001B984 File Offset: 0x00019B84
			public Command(string name, string usage, int argCount, bool isHostOnly, Action<string[]> handler)
			{
				this.name = name;
				this.usage = usage;
				this.argCountMax = argCount;
				this.argCountMin = argCount;
				this.isHostOnly = isHostOnly;
				this.handler = handler;
			}

			// Token: 0x060004A5 RID: 1189 RVA: 0x0001B9BF File Offset: 0x00019BBF
			public Command(string name, string usage, int argCountMin, int argCountMax, bool isHostOnly, Action<string[]> handler)
			{
				this.name = name;
				this.usage = usage;
				this.argCountMin = argCountMin;
				this.argCountMax = argCountMax;
				this.isHostOnly = isHostOnly;
				this.handler = handler;
			}

			// Token: 0x04000454 RID: 1108
			public string name;

			// Token: 0x04000455 RID: 1109
			public string usage;

			// Token: 0x04000456 RID: 1110
			public bool isHostOnly;

			// Token: 0x04000457 RID: 1111
			public int argCountMin;

			// Token: 0x04000458 RID: 1112
			public int argCountMax;

			// Token: 0x04000459 RID: 1113
			public Action<string[]> handler;
		}

		// Token: 0x020000E1 RID: 225
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004A6 RID: 1190 RVA: 0x0001B9EE File Offset: 0x00019BEE
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004A7 RID: 1191 RVA: 0x0001B9FA File Offset: 0x00019BFA
			public <>c()
			{
			}

			// Token: 0x060004A8 RID: 1192 RVA: 0x0001BA04 File Offset: 0x00019C04
			internal void <.cctor>b__3_0(string[] args)
			{
				CSteamID csteamID = default(CSteamID);
				bool flag = false;
				foreach (KeyValuePair<CSteamID, string> keyValuePair in NetManager.playerNames)
				{
					if (keyValuePair.Value.ToLower() == args[0].ToLower())
					{
						csteamID = keyValuePair.Key;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Console.Log("Player " + args[0] + " can't be found", true);
					return;
				}
				Transform transform = NetManager.GetPlayerComponentById<NetPlayer>(csteamID).player.transform;
				LocalNetPlayer.Instance.player.Value.transform.position = transform.position;
				Console.Log("Teleported to " + args[0], true);
			}

			// Token: 0x060004A9 RID: 1193 RVA: 0x0001BAE0 File Offset: 0x00019CE0
			internal void <.cctor>b__3_1(string[] args)
			{
				int num;
				if (int.TryParse(args[0], out num))
				{
					NetRigidbodyManager.OwnedRigidbody ownedRigidbody = NetRigidbodyManager.GetOwnedRigidbody(num);
					Console.Log(string.Format("The object of hash {0} is {1}, full path: {2}. DEBUG: {3}, {4}, {5}, {6}, {7}, {8}", new object[]
					{
						num,
						ownedRigidbody.transform.name,
						ownedRigidbody.transform.GetGameobjectHashString(),
						ownedRigidbody.remove == null,
						ownedRigidbody.Removal_Part == null,
						ownedRigidbody.Removal_Rigidbody == null,
						ownedRigidbody.rigidbody == null,
						ownedRigidbody.rigidbodyPart == null,
						ownedRigidbody.transform == null
					}), true);
					return;
				}
				Console.LogError("The argument is not a number!", true);
			}

			// Token: 0x060004AA RID: 1194 RVA: 0x0001BBC0 File Offset: 0x00019DC0
			internal void <.cctor>b__3_2(string[] args)
			{
				bool flag;
				if (!bool.TryParse(args[1], out flag))
				{
					Console.LogError("The hand type argument is not a bool!", true);
				}
				bool flag2;
				if (!bool.TryParse(args[2], out flag2))
				{
					Console.LogError("The value argument is not a bool!", true);
				}
				CSteamID csteamID = default(CSteamID);
				bool flag3 = false;
				foreach (KeyValuePair<CSteamID, string> keyValuePair in NetManager.playerNames)
				{
					if (keyValuePair.Value.ToLower() == args[0].ToLower())
					{
						csteamID = keyValuePair.Key;
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					Console.Log("Player " + args[0] + " can't be found", true);
					return;
				}
				PlayerAnimationManager playerAnimationManager = NetManager.GetPlayerComponentById<NetPlayer>(csteamID).playerAnimationManager;
				if (flag)
				{
					playerAnimationManager.leftHandOn = flag2;
				}
				else
				{
					playerAnimationManager.rightHandOn = flag2;
				}
				Console.Log(string.Format("Set {0} hand of {1} to {2}", flag ? "left" : "right", args[0], flag2), true);
			}

			// Token: 0x060004AB RID: 1195 RVA: 0x0001BCD4 File Offset: 0x00019ED4
			internal void <.cctor>b__3_3(string[] args)
			{
				PlayerGrabbingManager.handFSM.SendEvent("FINISHED");
				Console.Log("Successfully attempted to reset grabbing fsm", true);
			}

			// Token: 0x060004AC RID: 1196 RVA: 0x0001BCF0 File Offset: 0x00019EF0
			internal void <.cctor>b__3_4(string[] args)
			{
				SatsumaProfiler.Instance.PrintToFile();
				Console.Log("Saved last 10 seconds of satsuma behaviour to satsuma_profiler.txt", true);
			}

			// Token: 0x060004AD RID: 1197 RVA: 0x0001BD08 File Offset: 0x00019F08
			internal void <.cctor>b__3_5(string[] args)
			{
				string text = args[0].ToLower();
				if (text == "start")
				{
					BProfiler.Start();
					return;
				}
				if (!(text == "stop"))
				{
					Console.LogError("Invalid argument, please use either start or stop", true);
					return;
				}
				BProfiler.Stop("BeerMP_Profiler_results.txt");
			}

			// Token: 0x060004AE RID: 1198 RVA: 0x0001BD58 File Offset: 0x00019F58
			internal void <.cctor>b__3_6(string[] args)
			{
				if (args[0].ToLower() != "changekey" && args.Length < 2)
				{
					Console.LogError("Invalid syntax. Please use: /pvc <enable true/false|pushtotalk true/false|changekey|mastervolume 0-10>", true);
					return;
				}
				string text = args[0].ToLower();
				if (text == "enable")
				{
					bool flag = bool.Parse(args[1]);
					ProximityVoiceChat.SetEnabled(flag);
					Console.Log((flag ? "Enabled" : "Disabled") + " voice chat", true);
					return;
				}
				if (text == "pushtotalk")
				{
					bool flag2 = bool.Parse(args[1]);
					ProximityVoiceChat.SetPushToTalk(flag2);
					Console.Log((flag2 ? "Enabled" : "Disabled") + " push to talk", true);
					return;
				}
				if (text == "changekey")
				{
					ProximityVoiceChat.ChangePTT_Keybing();
					Console.Log("Press the key you want to assign Push to talk to", true);
					return;
				}
				if (!(text == "mastervolume"))
				{
					return;
				}
				float? num = new float?(float.Parse(args[1]));
				if (num != null)
				{
					ProximityVoiceChat.SetMasterVolume(num.Value);
					Console.Log(string.Format("Changed voice chat volume to {0}", num.Value), true);
					return;
				}
				Console.LogError("The value " + args[1] + " is not a number!", true);
			}

			// Token: 0x060004AF RID: 1199 RVA: 0x0001BE90 File Offset: 0x0001A090
			internal void <.cctor>b__3_7(string[] args)
			{
				for (int i = 0; i < NetVehicleManager.vehicles.Count; i++)
				{
					Console.Log(string.Format("{0} = {1}", i, NetVehicleManager.vehicles[i].transform.name), true);
				}
			}

			// Token: 0x060004B0 RID: 1200 RVA: 0x0001BEE0 File Offset: 0x0001A0E0
			internal void <.cctor>b__3_8(string[] args)
			{
				int num = int.Parse(args[0]);
				NetVehicleManager.vehicles[num].driver = 0UL;
				Console.Log("Successfully resetted " + NetVehicleManager.vehicles[num].transform.name + " driving mode", true);
			}

			// Token: 0x0400045A RID: 1114
			public static readonly CommandHandler.<>c <>9 = new CommandHandler.<>c();
		}
	}
}
