using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.PlayerManagers;
using Steamworks;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200006D RID: 109
	[ManagerCreate(10)]
	public class NetVehicleManager : MonoBehaviour
	{
		// Token: 0x0600030F RID: 783 RVA: 0x000164E3 File Offset: 0x000146E3
		public static void RegisterNetVehicle(NetVehicle netVeh)
		{
			NetVehicleManager.vehicles.Add(netVeh);
			Console.Log(string.Format("registered NetVehicle with hash {0} ({1})", netVeh.hash, netVeh.transform.name), false);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00016518 File Offset: 0x00014718
		private void Start()
		{
			NetVehicleManager.vehicles.Clear();
			Action action = delegate
			{
				NetVehicleManager.InitialSyncEvent = NetEvent<NetVehicleManager>.Register("InitialSync", new NetEventHandler(this.OnInitialSync));
				NetVehicleManager.SoundUpdateEvent = NetEvent<NetVehicleManager>.Register("SoundUpdate", new NetEventHandler(this.OnSoundUpdate));
				NetVehicleManager.DrivingModeEvent = NetEvent<NetVehicleManager>.Register("DrivingMode", new NetEventHandler(this.OnDrivingMode));
				NetVehicleManager.PassengerModeEvent = NetEvent<NetVehicleManager>.Register("PassengerMode", new NetEventHandler(this.OnPassengerMode));
				NetVehicleManager.FlatbedPassengerModeEvent = NetEvent<NetVehicleManager>.Register("FlatbedPassengerMode", new NetEventHandler(this.OnFlatbedPassengerMode));
				NetVehicleManager.InputUpdateEvent = NetEvent<NetVehicleManager>.Register("InputUpdate", new NetEventHandler(this.OnInputUpdate));
				NetVehicleManager.DrivetrainUpdateEvent = NetEvent<NetVehicleManager>.Register("DrivetrainUpdate", new NetEventHandler(this.OnDrivetrainUpdate));
				PlayMakerFSM[] array = Resources.FindObjectsOfTypeAll<PlayMakerFSM>().Where((PlayMakerFSM x) => x.FsmName == "GearIndicator").ToArray<PlayMakerFSM>();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].transform.GetComponent<Drivetrain>() == null)
					{
						return;
					}
					FsmNetVehicle fsmNetVehicle = new FsmNetVehicle(array[i].transform);
					NetVehicleManager.vanillaVehicles.Add(fsmNetVehicle);
				}
				FsmNetVehicle.DoFlatbedPassengerSeats(out this.flatbed, out this.flatbedHash, new Action<bool>(this.EnterFlatbedPassenger));
				BeerMPGlobals.OnMemberReady += new Action<ulong>(this.OnMemberReady);
				BeerMPGlobals.OnMemberExit += new Action<ulong>(this.OnMemberExit);
			};
			if (ObjectsLoader.IsGameLoaded)
			{
				action();
				return;
			}
			ObjectsLoader.gameLoaded += action;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0001655C File Offset: 0x0001475C
		private void OnFlatbedPassengerMode(ulong sender, Packet packet)
		{
			bool flag = packet.ReadBool(true);
			NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)sender).SetPassengerMode(flag, this.flatbed, false);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0001658C File Offset: 0x0001478C
		private void EnterFlatbedPassenger(bool enter)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(enter, -1);
				LocalNetPlayer.Instance.inCar = enter;
				NetVehicleManager.FlatbedPassengerModeEvent.Send(packet, true);
			}
		}

		// Token: 0x06000313 RID: 787 RVA: 0x000165DC File Offset: 0x000147DC
		private void OnInitialSync(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			NetVehicle netVehicle = NetVehicleManager.vehicles.FirstOrDefault((NetVehicle x) => x.hash == hash);
			if (netVehicle == null)
			{
				Console.LogError(string.Format("InitialSync: vehicle with hash {0} not found.", hash), true);
				return;
			}
			netVehicle.OnInitialSync(packet);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001663C File Offset: 0x0001483C
		private void OnDrivetrainUpdate(ulong sender, Packet packet)
		{
			while (packet.UnreadLength() > 0)
			{
				int hash = packet.ReadInt(true);
				float num = packet.ReadFloat(true);
				int num2 = packet.ReadInt(true);
				NetVehicle netVehicle = NetVehicleManager.vehicles.FirstOrDefault((NetVehicle x) => x.hash == hash);
				if (netVehicle == null)
				{
					Console.LogError(string.Format("Received input update packet for unknown vehicle with hash {0}", hash), false);
					return;
				}
				if (netVehicle.owner == sender && netVehicle.acc != null)
				{
					netVehicle.SetDrivetrain(num, num2);
				}
			}
		}

		// Token: 0x06000315 RID: 789 RVA: 0x000166D0 File Offset: 0x000148D0
		private void OnMemberExit(ulong player)
		{
			for (int i = 0; i < NetVehicleManager.vehicles.Count; i++)
			{
				NetVehicle netVehicle = NetVehicleManager.vehicles[i];
				if (netVehicle != null && netVehicle.driver == player)
				{
					netVehicle.driver = 0UL;
					netVehicle.owner = BeerMPGlobals.HostID;
					if (BeerMPGlobals.IsHost)
					{
						NetRigidbodyManager.RequestOwnership(netVehicle.rigidbody);
					}
				}
				int num = 0;
				for (;;)
				{
					int num2 = num;
					int? num3 = ((netVehicle != null) ? new int?(netVehicle.seatsUsed.Count) : null);
					if (!((num2 < num3.GetValueOrDefault()) & (num3 != null)))
					{
						break;
					}
					if (netVehicle.seatsUsed[num] == player)
					{
						netVehicle.seatsUsed[num] = 0UL;
					}
					num++;
				}
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00016790 File Offset: 0x00014990
		private void OnMemberReady(ulong player)
		{
			using (Packet packet = new Packet(1))
			{
				for (int i = 0; i < NetVehicleManager.vehicles.Count; i++)
				{
					NetVehicle netVehicle = NetVehicleManager.vehicles[i];
					ulong? num = ((netVehicle != null) ? new ulong?(netVehicle.owner) : null);
					ulong userID = BeerMPGlobals.UserID;
					if ((num.GetValueOrDefault() == userID) & (num != null))
					{
						if (netVehicle.audioController != null)
						{
							netVehicle.audioController.WriteUpdate(packet, netVehicle.hash, true);
						}
						netVehicle.SendInitialSync(player);
					}
				}
				NetVehicleManager.SoundUpdateEvent.Send(packet, player, true);
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00016848 File Offset: 0x00014A48
		private void OnInputUpdate(ulong sender, Packet packet)
		{
			while (packet.UnreadLength() > 0)
			{
				int hash = packet.ReadInt(true);
				float num = packet.ReadFloat(true);
				float num2 = packet.ReadFloat(true);
				float num3 = packet.ReadFloat(true);
				float num4 = packet.ReadFloat(true);
				float num5 = packet.ReadFloat(true);
				NetVehicle netVehicle = NetVehicleManager.vehicles.FirstOrDefault((NetVehicle x) => x.hash == hash);
				if (netVehicle == null)
				{
					Console.LogError(string.Format("Received input update packet for unknown vehicle with hash {0}", hash), false);
					return;
				}
				if (netVehicle.owner == sender && netVehicle.acc != null)
				{
					netVehicle.SetAxisController(num, num2, num3, num4, num5);
				}
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00016904 File Offset: 0x00014B04
		private void LateUpdate()
		{
			using (Packet packet = new Packet(1))
			{
				using (Packet packet2 = new Packet(1))
				{
					using (Packet packet3 = new Packet(1))
					{
						bool flag = false;
						bool flag2 = false;
						bool flag3 = false;
						for (int i = 0; i < NetVehicleManager.vehicles.Count; i++)
						{
							NetVehicle netVehicle = NetVehicleManager.vehicles[i];
							netVehicle.Update();
							if (netVehicle.audioController != null)
							{
								netVehicle.audioController.Update();
							}
							if (netVehicle.owner == BeerMPGlobals.UserID)
							{
								if (netVehicle.audioController != null)
								{
									flag |= netVehicle.audioController.WriteUpdate(packet, netVehicle.hash, false);
								}
								if (netVehicle.acc != null)
								{
									flag2 = true;
									netVehicle.WriteAxisControllerUpdate(packet2);
								}
								if (netVehicle.drivetrain != null)
								{
									flag3 = true;
									netVehicle.WriteDrivetrainUpdate(packet3);
								}
							}
						}
						if (flag)
						{
							NetVehicleManager.SoundUpdateEvent.Send(packet, true);
						}
						if (flag2)
						{
							NetVehicleManager.InputUpdateEvent.Send(packet2, true);
						}
						if (flag3)
						{
							NetVehicleManager.DrivetrainUpdateEvent.Send(packet3, true);
						}
						for (int j = 0; j < NetVehicleManager.vanillaVehicles.Count; j++)
						{
							for (int k = 0; k < NetVehicleManager.vanillaVehicles[j].dashboardLevers.Count; k++)
							{
								NetVehicleManager.vanillaVehicles[j].dashboardLevers[k].Update();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00016AD0 File Offset: 0x00014CD0
		private void FixedUpdate()
		{
			for (int i = 0; i < NetVehicleManager.vanillaVehicles.Count; i++)
			{
				for (int j = 0; j < NetVehicleManager.vanillaVehicles[i].vehicleDoors.Count; j++)
				{
					NetVehicleManager.vanillaVehicles[i].vehicleDoors[j].FixedUpdate();
				}
			}
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00016B30 File Offset: 0x00014D30
		private void OnPassengerMode(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			int num = packet.ReadInt(true);
			bool flag = packet.ReadBool(true);
			if (hash == this.flatbedHash)
			{
				NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)sender).SetPassengerMode(flag, this.flatbed, false);
				return;
			}
			NetVehicle netVehicle = NetVehicleManager.vehicles.FirstOrDefault((NetVehicle x) => x.hash == hash);
			if (netVehicle == null)
			{
				Console.LogError(string.Format("Received passenger mode packet for unknown vehicle with hash {0}", hash), false);
				return;
			}
			netVehicle.seatsUsed[num] = (flag ? sender : 0UL);
			NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)sender).SetPassengerMode(flag, netVehicle.transform, true);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00016BEC File Offset: 0x00014DEC
		private void OnDrivingMode(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			bool flag = packet.ReadBool(true);
			NetVehicle netVehicle = NetVehicleManager.vehicles.FirstOrDefault((NetVehicle x) => x.hash == hash);
			if (netVehicle == null)
			{
				Console.LogError(string.Format("Received driving mode packet for unknown vehicle with hash {0}", hash), false);
				return;
			}
			netVehicle.DrivingMode(sender, flag);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00016C54 File Offset: 0x00014E54
		private void OnSoundUpdate(ulong sender, Packet packet)
		{
			while (packet.UnreadLength() > 0)
			{
				if (packet.ReadByte(true) != 7)
				{
					this.SoundUpdateReadError(0);
					return;
				}
				int hash = packet.ReadInt(true);
				NetVehicle netVehicle = NetVehicleManager.vehicles.FirstOrDefault((NetVehicle x) => x.hash == hash);
				if (netVehicle == null)
				{
					Console.LogError(string.Format("OnSoundUpdate can't find car with hash {0}", hash), false);
					return;
				}
				byte b = packet.ReadByte(true);
				while (b == 15)
				{
					int num = packet.ReadInt(true);
					bool? flag = null;
					float? num2 = null;
					float? num3 = null;
					float? num4 = null;
					byte b2 = packet.ReadByte(true);
					if (b2 == 31)
					{
						flag = new bool?(packet.ReadBool(true));
						if (flag.Value)
						{
							num4 = new float?(packet.ReadFloat(true));
						}
						b2 = packet.ReadByte(true);
					}
					if (b2 == 47)
					{
						num2 = new float?(packet.ReadFloat(true));
						b2 = packet.ReadByte(true);
					}
					if (b2 == 63)
					{
						num3 = new float?(packet.ReadFloat(true));
						b2 = packet.ReadByte(true);
					}
					if (b2 != 255)
					{
						this.SoundUpdateReadError(1);
						return;
					}
					b = packet.ReadByte(true);
					netVehicle.audioController.sources[num].OnUpdate(flag, num4, num2, num3);
				}
				if (b != 247)
				{
					this.SoundUpdateReadError(2);
					return;
				}
			}
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00016DC8 File Offset: 0x00014FC8
		private void SoundUpdateReadError(int code)
		{
			Console.LogError("Error code " + code.ToString() + " when reading OnSoundUpdate packet", false);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00016DE6 File Offset: 0x00014FE6
		public NetVehicleManager()
		{
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00016DEE File Offset: 0x00014FEE
		// Note: this type is marked as 'beforefieldinit'.
		static NetVehicleManager()
		{
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00016E04 File Offset: 0x00015004
		[CompilerGenerated]
		private void <Start>b__12_0()
		{
			NetVehicleManager.InitialSyncEvent = NetEvent<NetVehicleManager>.Register("InitialSync", new NetEventHandler(this.OnInitialSync));
			NetVehicleManager.SoundUpdateEvent = NetEvent<NetVehicleManager>.Register("SoundUpdate", new NetEventHandler(this.OnSoundUpdate));
			NetVehicleManager.DrivingModeEvent = NetEvent<NetVehicleManager>.Register("DrivingMode", new NetEventHandler(this.OnDrivingMode));
			NetVehicleManager.PassengerModeEvent = NetEvent<NetVehicleManager>.Register("PassengerMode", new NetEventHandler(this.OnPassengerMode));
			NetVehicleManager.FlatbedPassengerModeEvent = NetEvent<NetVehicleManager>.Register("FlatbedPassengerMode", new NetEventHandler(this.OnFlatbedPassengerMode));
			NetVehicleManager.InputUpdateEvent = NetEvent<NetVehicleManager>.Register("InputUpdate", new NetEventHandler(this.OnInputUpdate));
			NetVehicleManager.DrivetrainUpdateEvent = NetEvent<NetVehicleManager>.Register("DrivetrainUpdate", new NetEventHandler(this.OnDrivetrainUpdate));
			PlayMakerFSM[] array = Resources.FindObjectsOfTypeAll<PlayMakerFSM>().Where((PlayMakerFSM x) => x.FsmName == "GearIndicator").ToArray<PlayMakerFSM>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.GetComponent<Drivetrain>() == null)
				{
					return;
				}
				FsmNetVehicle fsmNetVehicle = new FsmNetVehicle(array[i].transform);
				NetVehicleManager.vanillaVehicles.Add(fsmNetVehicle);
			}
			FsmNetVehicle.DoFlatbedPassengerSeats(out this.flatbed, out this.flatbedHash, new Action<bool>(this.EnterFlatbedPassenger));
			BeerMPGlobals.OnMemberReady += new Action<ulong>(this.OnMemberReady);
			BeerMPGlobals.OnMemberExit += new Action<ulong>(this.OnMemberExit);
		}

		// Token: 0x04000317 RID: 791
		internal static List<NetVehicle> vehicles = new List<NetVehicle>();

		// Token: 0x04000318 RID: 792
		internal static List<FsmNetVehicle> vanillaVehicles = new List<FsmNetVehicle>();

		// Token: 0x04000319 RID: 793
		internal static NetEvent<NetVehicleManager> InitialSyncEvent;

		// Token: 0x0400031A RID: 794
		internal static NetEvent<NetVehicleManager> SoundUpdateEvent;

		// Token: 0x0400031B RID: 795
		internal static NetEvent<NetVehicleManager> DrivingModeEvent;

		// Token: 0x0400031C RID: 796
		internal static NetEvent<NetVehicleManager> PassengerModeEvent;

		// Token: 0x0400031D RID: 797
		internal static NetEvent<NetVehicleManager> FlatbedPassengerModeEvent;

		// Token: 0x0400031E RID: 798
		internal static NetEvent<NetVehicleManager> InputUpdateEvent;

		// Token: 0x0400031F RID: 799
		internal static NetEvent<NetVehicleManager> DrivetrainUpdateEvent;

		// Token: 0x04000320 RID: 800
		private Transform flatbed;

		// Token: 0x04000321 RID: 801
		internal int flatbedHash;

		// Token: 0x02000129 RID: 297
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060005C2 RID: 1474 RVA: 0x0001FE20 File Offset: 0x0001E020
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060005C3 RID: 1475 RVA: 0x0001FE2C File Offset: 0x0001E02C
			public <>c()
			{
			}

			// Token: 0x060005C4 RID: 1476 RVA: 0x0001FE34 File Offset: 0x0001E034
			internal bool <Start>b__12_1(PlayMakerFSM x)
			{
				return x.FsmName == "GearIndicator";
			}

			// Token: 0x0400056B RID: 1387
			public static readonly NetVehicleManager.<>c <>9 = new NetVehicleManager.<>c();

			// Token: 0x0400056C RID: 1388
			public static Func<PlayMakerFSM, bool> <>9__12_1;
		}

		// Token: 0x0200012A RID: 298
		[CompilerGenerated]
		private sealed class <>c__DisplayClass15_0
		{
			// Token: 0x060005C5 RID: 1477 RVA: 0x0001FE46 File Offset: 0x0001E046
			public <>c__DisplayClass15_0()
			{
			}

			// Token: 0x060005C6 RID: 1478 RVA: 0x0001FE4E File Offset: 0x0001E04E
			internal bool <OnInitialSync>b__0(NetVehicle x)
			{
				return x.hash == this.hash;
			}

			// Token: 0x0400056D RID: 1389
			public int hash;
		}

		// Token: 0x0200012B RID: 299
		[CompilerGenerated]
		private sealed class <>c__DisplayClass16_0
		{
			// Token: 0x060005C7 RID: 1479 RVA: 0x0001FE5E File Offset: 0x0001E05E
			public <>c__DisplayClass16_0()
			{
			}

			// Token: 0x060005C8 RID: 1480 RVA: 0x0001FE66 File Offset: 0x0001E066
			internal bool <OnDrivetrainUpdate>b__0(NetVehicle x)
			{
				return x.hash == this.hash;
			}

			// Token: 0x0400056E RID: 1390
			public int hash;
		}

		// Token: 0x0200012C RID: 300
		[CompilerGenerated]
		private sealed class <>c__DisplayClass19_0
		{
			// Token: 0x060005C9 RID: 1481 RVA: 0x0001FE76 File Offset: 0x0001E076
			public <>c__DisplayClass19_0()
			{
			}

			// Token: 0x060005CA RID: 1482 RVA: 0x0001FE7E File Offset: 0x0001E07E
			internal bool <OnInputUpdate>b__0(NetVehicle x)
			{
				return x.hash == this.hash;
			}

			// Token: 0x0400056F RID: 1391
			public int hash;
		}

		// Token: 0x0200012D RID: 301
		[CompilerGenerated]
		private sealed class <>c__DisplayClass22_0
		{
			// Token: 0x060005CB RID: 1483 RVA: 0x0001FE8E File Offset: 0x0001E08E
			public <>c__DisplayClass22_0()
			{
			}

			// Token: 0x060005CC RID: 1484 RVA: 0x0001FE96 File Offset: 0x0001E096
			internal bool <OnPassengerMode>b__0(NetVehicle x)
			{
				return x.hash == this.hash;
			}

			// Token: 0x04000570 RID: 1392
			public int hash;
		}

		// Token: 0x0200012E RID: 302
		[CompilerGenerated]
		private sealed class <>c__DisplayClass23_0
		{
			// Token: 0x060005CD RID: 1485 RVA: 0x0001FEA6 File Offset: 0x0001E0A6
			public <>c__DisplayClass23_0()
			{
			}

			// Token: 0x060005CE RID: 1486 RVA: 0x0001FEAE File Offset: 0x0001E0AE
			internal bool <OnDrivingMode>b__0(NetVehicle x)
			{
				return x.hash == this.hash;
			}

			// Token: 0x04000571 RID: 1393
			public int hash;
		}

		// Token: 0x0200012F RID: 303
		[CompilerGenerated]
		private sealed class <>c__DisplayClass24_0
		{
			// Token: 0x060005CF RID: 1487 RVA: 0x0001FEBE File Offset: 0x0001E0BE
			public <>c__DisplayClass24_0()
			{
			}

			// Token: 0x060005D0 RID: 1488 RVA: 0x0001FEC6 File Offset: 0x0001E0C6
			internal bool <OnSoundUpdate>b__0(NetVehicle x)
			{
				return x.hash == this.hash;
			}

			// Token: 0x04000572 RID: 1394
			public int hash;
		}
	}
}
