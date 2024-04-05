using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000069 RID: 105
	internal class FsmVehicleDoor
	{
		// Token: 0x060002D9 RID: 729 RVA: 0x000150D0 File Offset: 0x000132D0
		public FsmVehicleDoor(PlayMakerFSM fsm, bool isHayosikoSidedoor = false)
		{
			this.fsm = fsm;
			this.hash = fsm.transform.GetGameobjectHashString().GetHashCode();
			this.owner = BeerMPGlobals.HostID;
			fsm.Initialize();
			if (isHayosikoSidedoor)
			{
				this.SetupFSMSidedoor();
			}
			else
			{
				this.SetupFSM();
			}
			if (FsmVehicleDoor.doorRotationEvent == null)
			{
				FsmVehicleDoor.doorRotationEvent = NetEvent<FsmVehicleDoor>.Register("Rot", new NetEventHandler(FsmVehicleDoor.OnDoorRotation));
			}
			if (FsmVehicleDoor.doorToggleEvent == null)
			{
				FsmVehicleDoor.doorToggleEvent = NetEvent<FsmVehicleDoor>.Register("Toggle", new NetEventHandler(FsmVehicleDoor.OnDoorToggle));
			}
			if (FsmVehicleDoor.requestOwnershipEvent == null)
			{
				FsmVehicleDoor.requestOwnershipEvent = NetEvent<FsmVehicleDoor>.Register("SetOwner", new NetEventHandler(FsmVehicleDoor.OnSetOwner));
			}
			if (FsmVehicleDoor.initSync == null)
			{
				FsmVehicleDoor.initSync = NetEvent<FsmVehicleDoor>.Register("Init", delegate(ulong s, Packet p)
				{
					while (p.UnreadLength() > 0)
					{
						int hash = p.ReadInt(true);
						bool flag = p.ReadBool(true);
						FsmVehicleDoor fsmVehicleDoor = FsmVehicleDoor.doors.FirstOrDefault((FsmVehicleDoor d) => d.hash == hash);
						if (fsmVehicleDoor == null)
						{
							Console.LogError(string.Format("Failed to init sync fsm car door: the hash {0} was not found", hash), false);
							return;
						}
						fsmVehicleDoor.owner = s;
						if (flag)
						{
							fsmVehicleDoor.updatingFsm = true;
							fsmVehicleDoor.fsm.SendEvent("MP_OPEN");
						}
					}
				});
				BeerMPGlobals.OnMemberReady += delegate(ulong user)
				{
					using (Packet packet = new Packet(0))
					{
						for (int i = 0; i < FsmVehicleDoor.doors.Count; i++)
						{
							if (FsmVehicleDoor.doors[i] == null)
							{
								FsmVehicleDoor.doors.RemoveAt(i);
								i--;
							}
							else if (FsmVehicleDoor.doors[i].owner == BeerMPGlobals.UserID && FsmVehicleDoor.doors[i].doorOpen != null)
							{
								packet.Write(FsmVehicleDoor.doors[i].hash, -1);
								packet.Write(FsmVehicleDoor.doors[i].doorOpen.Value, -1);
							}
						}
						FsmVehicleDoor.initSync.Send(packet, user, true);
					}
				};
			}
			FsmVehicleDoor.doors.Add(this);
			NetManager.sceneLoaded = (Action<GameScene>)Delegate.Combine(NetManager.sceneLoaded, new Action<GameScene>(delegate(GameScene a)
			{
				if (FsmVehicleDoor.doors.Contains(this))
				{
					FsmVehicleDoor.doors.Remove(this);
				}
			}));
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00015220 File Offset: 0x00013420
		private static void OnDoorRotation(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			FsmVehicleDoor fsmVehicleDoor = FsmVehicleDoor.doors.FirstOrDefault((FsmVehicleDoor d) => d.hash == hash);
			if (fsmVehicleDoor == null)
			{
				Console.LogError(string.Format("Failed to rotate fsm car door: the hash {0} was not found", hash), false);
				return;
			}
			if (!fsmVehicleDoor.doorOpen.Value)
			{
				return;
			}
			float num = packet.ReadFloat(true);
			Vector3 localEulerAngles = fsmVehicleDoor.door.Value.transform.localEulerAngles;
			localEulerAngles[fsmVehicleDoor.axis] = num;
			fsmVehicleDoor.door.Value.transform.localEulerAngles = localEulerAngles;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x000152C8 File Offset: 0x000134C8
		private static void OnDoorToggle(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			FsmVehicleDoor fsmVehicleDoor = FsmVehicleDoor.doors.FirstOrDefault((FsmVehicleDoor d) => d.hash == hash);
			if (fsmVehicleDoor == null)
			{
				Console.LogError(string.Format("Failed to toggle fsm car door: the hash {0} was not found", hash), false);
				return;
			}
			fsmVehicleDoor.updatingFsm = true;
			bool flag = packet.ReadBool(true);
			fsmVehicleDoor.fsm.SendEvent(flag ? "MP_OPEN" : "MP_CLOSE");
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00015348 File Offset: 0x00013548
		private static void OnSetOwner(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			FsmVehicleDoor fsmVehicleDoor = FsmVehicleDoor.doors.FirstOrDefault((FsmVehicleDoor d) => d.hash == hash);
			if (fsmVehicleDoor == null)
			{
				Console.LogError(string.Format("Failed to set fsm car door ownership: the hash {0} was not found", hash), false);
				return;
			}
			fsmVehicleDoor.owner = sender;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x000153A8 File Offset: 0x000135A8
		public void FixedUpdate()
		{
			if (this.doorOpen != null && this.owner == BeerMPGlobals.UserID && this.doorOpen.Value)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.hash, -1);
					packet.Write(this.door.Value.transform.localEulerAngles[this.axis], -1);
					FsmVehicleDoor.doorRotationEvent.Send(packet, true);
				}
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00015440 File Offset: 0x00013640
		private void SetupFSMSidedoor()
		{
			string text = "Open door";
			string text2 = "Close door";
			FsmEvent fsmEvent = this.fsm.AddEvent("MP_OPEN");
			this.fsm.AddGlobalTransition(fsmEvent, text);
			FsmEvent fsmEvent2 = this.fsm.AddEvent("MP_CLOSE");
			this.fsm.AddGlobalTransition(fsmEvent2, text2);
			this.fsm.InsertAction(text, new Action(this.OpenDoor), 0);
			this.fsm.InsertAction(text2, new Action(this.CloseDoor), 0);
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000154C8 File Offset: 0x000136C8
		private void SetupFSM()
		{
			try
			{
				bool flag = false;
				this.doorOpen = this.fsm.FsmVariables.FindFsmBool("Open");
				this.door = this.fsm.FsmVariables.FindFsmGameObject("Door");
				if (this.door == null)
				{
					flag = true;
					this.door = this.fsm.FsmVariables.FindFsmGameObject("Bootlid");
					if (this.door == null)
					{
						this.door = this.fsm.gameObject;
						flag = false;
					}
				}
				string text = (flag ? "Open hood" : "Open door");
				string text2 = (flag ? (this.fsm.HasState("Drop") ? "Drop" : "State 2") : "Sound");
				GetRotation getRotation = this.fsm.GetState(text).Actions.First((FsmStateAction a) => a is GetRotation) as GetRotation;
				if (getRotation.xAngle != null && !getRotation.xAngle.IsNone)
				{
					this.axis = 0;
				}
				else if (getRotation.yAngle != null && !getRotation.yAngle.IsNone)
				{
					this.axis = 1;
				}
				else if (getRotation.zAngle != null && !getRotation.zAngle.IsNone)
				{
					this.axis = 2;
				}
				FsmEvent fsmEvent = this.fsm.AddEvent("MP_OPEN");
				this.fsm.AddGlobalTransition(fsmEvent, text);
				FsmEvent fsmEvent2 = this.fsm.AddEvent("MP_CLOSE");
				this.fsm.AddGlobalTransition(fsmEvent2, text2);
				this.fsm.InsertAction(text, new Action(this.OpenDoor), 0);
				if (!flag)
				{
					this.fsm.InsertAction(this.fsm.HasState("Open door 2") ? "Open door 2" : "Open door 3", new Action(this.RequestOwnership), 0);
				}
				this.fsm.InsertAction(text2, new Action(this.CloseDoor), 0);
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("Failed to setup door {0} ({1}): {2}, {3}, {4}", new object[]
				{
					this.hash,
					this.fsm.transform.GetGameobjectHashString(),
					ex.GetType(),
					ex.Message,
					ex.StackTrace
				}), false);
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00015740 File Offset: 0x00013940
		private void RequestOwnership()
		{
			if (BeerMPGlobals.UserID == this.owner)
			{
				return;
			}
			this.owner = BeerMPGlobals.UserID;
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				FsmVehicleDoor.requestOwnershipEvent.Send(packet, true);
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x000157A4 File Offset: 0x000139A4
		private void CloseDoor()
		{
			if (this.updatingFsm)
			{
				this.updatingFsm = false;
				return;
			}
			this.RequestOwnership();
			this.SendDoorToggleEvent(false);
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x000157C3 File Offset: 0x000139C3
		private void OpenDoor()
		{
			if (this.updatingFsm)
			{
				this.updatingFsm = false;
				return;
			}
			this.RequestOwnership();
			this.SendDoorToggleEvent(true);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x000157E4 File Offset: 0x000139E4
		private void SendDoorToggleEvent(bool open)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write(open, -1);
				FsmVehicleDoor.doorToggleEvent.Send(packet, true);
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00015838 File Offset: 0x00013A38
		// Note: this type is marked as 'beforefieldinit'.
		static FsmVehicleDoor()
		{
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00015844 File Offset: 0x00013A44
		[CompilerGenerated]
		private void <.ctor>b__14_2(GameScene a)
		{
			if (FsmVehicleDoor.doors.Contains(this))
			{
				FsmVehicleDoor.doors.Remove(this);
			}
		}

		// Token: 0x040002E5 RID: 741
		private PlayMakerFSM fsm;

		// Token: 0x040002E6 RID: 742
		private FsmBool doorOpen;

		// Token: 0x040002E7 RID: 743
		private FsmGameObject door;

		// Token: 0x040002E8 RID: 744
		private int hash;

		// Token: 0x040002E9 RID: 745
		private int axis = -1;

		// Token: 0x040002EA RID: 746
		private ulong owner;

		// Token: 0x040002EB RID: 747
		private bool updatingFsm;

		// Token: 0x040002EC RID: 748
		private const string openDoorFsmEvent = "MP_OPEN";

		// Token: 0x040002ED RID: 749
		private const string closeDoorFsmEvent = "MP_CLOSE";

		// Token: 0x040002EE RID: 750
		private static NetEvent<FsmVehicleDoor> doorRotationEvent;

		// Token: 0x040002EF RID: 751
		private static NetEvent<FsmVehicleDoor> doorToggleEvent;

		// Token: 0x040002F0 RID: 752
		private static NetEvent<FsmVehicleDoor> requestOwnershipEvent;

		// Token: 0x040002F1 RID: 753
		private static NetEvent<FsmVehicleDoor> initSync;

		// Token: 0x040002F2 RID: 754
		private static List<FsmVehicleDoor> doors = new List<FsmVehicleDoor>();

		// Token: 0x02000120 RID: 288
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060005A7 RID: 1447 RVA: 0x0001F8A4 File Offset: 0x0001DAA4
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060005A8 RID: 1448 RVA: 0x0001F8B0 File Offset: 0x0001DAB0
			public <>c()
			{
			}

			// Token: 0x060005A9 RID: 1449 RVA: 0x0001F8B8 File Offset: 0x0001DAB8
			internal void <.ctor>b__14_0(ulong s, Packet p)
			{
				while (p.UnreadLength() > 0)
				{
					FsmVehicleDoor.<>c__DisplayClass14_0 CS$<>8__locals1 = new FsmVehicleDoor.<>c__DisplayClass14_0();
					CS$<>8__locals1.hash = p.ReadInt(true);
					bool flag = p.ReadBool(true);
					FsmVehicleDoor fsmVehicleDoor = FsmVehicleDoor.doors.FirstOrDefault((FsmVehicleDoor d) => d.hash == CS$<>8__locals1.hash);
					if (fsmVehicleDoor == null)
					{
						Console.LogError(string.Format("Failed to init sync fsm car door: the hash {0} was not found", CS$<>8__locals1.hash), false);
						return;
					}
					fsmVehicleDoor.owner = s;
					if (flag)
					{
						fsmVehicleDoor.updatingFsm = true;
						fsmVehicleDoor.fsm.SendEvent("MP_OPEN");
					}
				}
			}

			// Token: 0x060005AA RID: 1450 RVA: 0x0001F944 File Offset: 0x0001DB44
			internal void <.ctor>b__14_1(ulong user)
			{
				using (Packet packet = new Packet(0))
				{
					for (int i = 0; i < FsmVehicleDoor.doors.Count; i++)
					{
						if (FsmVehicleDoor.doors[i] == null)
						{
							FsmVehicleDoor.doors.RemoveAt(i);
							i--;
						}
						else if (FsmVehicleDoor.doors[i].owner == BeerMPGlobals.UserID && FsmVehicleDoor.doors[i].doorOpen != null)
						{
							packet.Write(FsmVehicleDoor.doors[i].hash, -1);
							packet.Write(FsmVehicleDoor.doors[i].doorOpen.Value, -1);
						}
					}
					FsmVehicleDoor.initSync.Send(packet, user, true);
				}
			}

			// Token: 0x060005AB RID: 1451 RVA: 0x0001FA14 File Offset: 0x0001DC14
			internal bool <SetupFSM>b__20_0(FsmStateAction a)
			{
				return a is GetRotation;
			}

			// Token: 0x04000557 RID: 1367
			public static readonly FsmVehicleDoor.<>c <>9 = new FsmVehicleDoor.<>c();

			// Token: 0x04000558 RID: 1368
			public static NetEventHandler <>9__14_0;

			// Token: 0x04000559 RID: 1369
			public static Action<ulong> <>9__14_1;

			// Token: 0x0400055A RID: 1370
			public static Func<FsmStateAction, bool> <>9__20_0;
		}

		// Token: 0x02000121 RID: 289
		[CompilerGenerated]
		private sealed class <>c__DisplayClass14_0
		{
			// Token: 0x060005AC RID: 1452 RVA: 0x0001FA1F File Offset: 0x0001DC1F
			public <>c__DisplayClass14_0()
			{
			}

			// Token: 0x060005AD RID: 1453 RVA: 0x0001FA27 File Offset: 0x0001DC27
			internal bool <.ctor>b__3(FsmVehicleDoor d)
			{
				return d.hash == this.hash;
			}

			// Token: 0x0400055B RID: 1371
			public int hash;
		}

		// Token: 0x02000122 RID: 290
		[CompilerGenerated]
		private sealed class <>c__DisplayClass15_0
		{
			// Token: 0x060005AE RID: 1454 RVA: 0x0001FA37 File Offset: 0x0001DC37
			public <>c__DisplayClass15_0()
			{
			}

			// Token: 0x060005AF RID: 1455 RVA: 0x0001FA3F File Offset: 0x0001DC3F
			internal bool <OnDoorRotation>b__0(FsmVehicleDoor d)
			{
				return d.hash == this.hash;
			}

			// Token: 0x0400055C RID: 1372
			public int hash;
		}

		// Token: 0x02000123 RID: 291
		[CompilerGenerated]
		private sealed class <>c__DisplayClass16_0
		{
			// Token: 0x060005B0 RID: 1456 RVA: 0x0001FA4F File Offset: 0x0001DC4F
			public <>c__DisplayClass16_0()
			{
			}

			// Token: 0x060005B1 RID: 1457 RVA: 0x0001FA57 File Offset: 0x0001DC57
			internal bool <OnDoorToggle>b__0(FsmVehicleDoor d)
			{
				return d.hash == this.hash;
			}

			// Token: 0x0400055D RID: 1373
			public int hash;
		}

		// Token: 0x02000124 RID: 292
		[CompilerGenerated]
		private sealed class <>c__DisplayClass17_0
		{
			// Token: 0x060005B2 RID: 1458 RVA: 0x0001FA67 File Offset: 0x0001DC67
			public <>c__DisplayClass17_0()
			{
			}

			// Token: 0x060005B3 RID: 1459 RVA: 0x0001FA6F File Offset: 0x0001DC6F
			internal bool <OnSetOwner>b__0(FsmVehicleDoor d)
			{
				return d.hash == this.hash;
			}

			// Token: 0x0400055E RID: 1374
			public int hash;
		}
	}
}
