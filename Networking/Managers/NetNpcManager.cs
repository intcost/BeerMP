using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200005B RID: 91
	[ManagerCreate(10)]
	internal class NetNpcManager : MonoBehaviour
	{
		// Token: 0x06000226 RID: 550 RVA: 0x0000CF89 File Offset: 0x0000B189
		private void Start()
		{
			ObjectsLoader.gameLoaded += delegate
			{
				Transform transform = GameObject.Find("HUMANS/Randomizer/Walkers").transform;
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					int hashCode = child.GetGameobjectHashString().GetHashCode();
					NetNpcManager.walkers.Add(hashCode, child);
				}
				Transform transform2 = GameObject.Find("KILJUGUY").transform.Find("HikerPivot");
				NetNpcManager.walkers.Add(transform2.GetGameobjectHashString().GetHashCode(), transform2);
				this.drunkHiker2Fsm = transform2.Find("JokkeHiker2").GetPlayMaker("Logic");
				this.drunkHiker2Fsm.Initialize();
				this.drunkHiker2CarEvents = new FsmEvent[NetNpcManager.jokkeHiker_eventNames.Length];
				for (int j = 0; j < NetNpcManager.jokkeHiker_eventNames.Length; j++)
				{
					this.drunkHiker2CarEvents[j] = this.drunkHiker2Fsm.AddEvent(NetNpcManager.jokkeHiker_eventNames[j]);
					this.drunkHiker2Fsm.AddGlobalTransition(this.drunkHiker2CarEvents[j], NetNpcManager.jokkeHiker_stateNames[j]);
					int _i = j;
					this.drunkHiker2Fsm.InsertAction(NetNpcManager.jokkeHiker_stateNames[j], delegate
					{
						this.JokkeEnterCar(_i);
					}, 0);
				}
				this.drunkHiker2EnterCar = NetEvent<NetNpcManager>.Register("DH2Car", new NetEventHandler(this.OnJokkeEnterCar));
				Transform transform3 = GameObject.Find("TRAFFIC").transform;
				this.vehiclesHighway = transform3.Find("VehiclesHighway").gameObject;
				this.policeFsm = transform3.Find("Police").GetComponent<PlayMakerFSM>();
				if (BeerMPGlobals.IsHost)
				{
					this.policeFsm.InsertAction("State 3", delegate
					{
						this.PoliceSpawn(0);
					}, 0);
					this.policeFsm.InsertAction("State 4", delegate
					{
						this.PoliceSpawn(1);
					}, 0);
					this.policeFsm.InsertAction("State 5", delegate
					{
						this.PoliceSpawn(2);
					}, 0);
					this.policeFsm.InsertAction("State 6", delegate
					{
						this.PoliceSpawn(3);
					}, 0);
				}
				else
				{
					this.policeEvents = new FsmEvent[]
					{
						this.policeFsm.AddEvent("MP_SPAWN0"),
						this.policeFsm.AddEvent("MP_SPAWN1"),
						this.policeFsm.AddEvent("MP_SPAWN2"),
						this.policeFsm.AddEvent("MP_SPAWN3")
					};
					this.policeFsm.AddGlobalTransition(this.policeEvents[0], "State 3");
					this.policeFsm.AddGlobalTransition(this.policeEvents[1], "State 4");
					this.policeFsm.AddGlobalTransition(this.policeEvents[2], "State 5");
					this.policeFsm.AddGlobalTransition(this.policeEvents[3], "State 6");
					this.policeFsm.GetState("Cop1").Actions[2].Enabled = false;
					this.policeFsm.GetState("State 1").Actions[0].Enabled = false;
				}
				this.walkerSync = NetEvent<NetNpcManager>.Register("Walk", new NetEventHandler(this.OnWalkerNPCSync));
				this.highwayUpdateEvent = NetEvent<NetNpcManager>.Register("HighwayUpdate", new NetEventHandler(this.OnHighwayUpdate));
				this.policeUpdateEvent = NetEvent<NetNpcManager>.Register("PoliceUpdate", new NetEventHandler(this.OnPoliceUpdate));
			};
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000CFA8 File Offset: 0x0000B1A8
		private void OnJokkeEnterCar(ulong s, Packet p)
		{
			int num = (int)p.ReadByte(true);
			this.receivedDrunkHiker2 = num;
			this.drunkHiker2Fsm.Fsm.Event(this.drunkHiker2CarEvents[num]);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000CFDC File Offset: 0x0000B1DC
		private void JokkeEnterCar(int index)
		{
			if (this.receivedDrunkHiker2 != -1)
			{
				this.receivedDrunkHiker2 = -1;
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write((byte)index, -1);
				this.drunkHiker2EnterCar.Send(packet, true);
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000D034 File Offset: 0x0000B234
		private void OnPoliceUpdate(ulong s, Packet p)
		{
			int num = (int)p.ReadByte(true);
			this.policeFsm.Fsm.Event(this.policeEvents[num]);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000D064 File Offset: 0x0000B264
		private void PoliceSpawn(int index)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write((byte)index, -1);
				this.policeUpdateEvent.Send(packet, true);
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000D0AC File Offset: 0x0000B2AC
		private void OnHighwayUpdate(ulong s, Packet p)
		{
			bool flag = p.ReadBool(true);
			this.highwayOn = flag;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000D0C8 File Offset: 0x0000B2C8
		private void CheckHighway()
		{
			if (BeerMPGlobals.IsHost)
			{
				if (this.highwayOn == this.vehiclesHighway.activeSelf)
				{
					return;
				}
				this.highwayOn = this.vehiclesHighway.activeSelf;
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.highwayOn, -1);
					this.highwayUpdateEvent.Send(packet, true);
					return;
				}
			}
			if (this.highwayOn != this.vehiclesHighway.activeSelf)
			{
				this.vehiclesHighway.SetActive(this.highwayOn);
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000D164 File Offset: 0x0000B364
		private void OnWalkerNPCSync(ulong s, Packet p)
		{
			while (p.UnreadLength() > 0)
			{
				int num = p.ReadInt(true);
				Vector3 vector = p.ReadVector3(true);
				Vector3 vector2 = p.ReadVector3(true);
				if (NetNpcManager.walkers.ContainsKey(num))
				{
					NetNpcManager.walkers[num].position = vector;
					NetNpcManager.walkers[num].eulerAngles = vector2;
				}
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000D1C4 File Offset: 0x0000B3C4
		private void UpdateWalkers()
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			using (Packet packet = new Packet(1))
			{
				foreach (KeyValuePair<int, Transform> keyValuePair in NetNpcManager.walkers)
				{
					packet.Write(keyValuePair.Key, -1);
					packet.Write(keyValuePair.Value.position, -1);
					packet.Write(keyValuePair.Value.eulerAngles, -1);
				}
				this.walkerSync.Send(packet, true);
			}
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000D278 File Offset: 0x0000B478
		private void Update()
		{
			this.UpdateWalkers();
			this.CheckHighway();
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000D286 File Offset: 0x0000B486
		public NetNpcManager()
		{
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000D29C File Offset: 0x0000B49C
		// Note: this type is marked as 'beforefieldinit'.
		static NetNpcManager()
		{
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000D30C File Offset: 0x0000B50C
		[CompilerGenerated]
		private void <Start>b__15_0()
		{
			Transform transform = GameObject.Find("HUMANS/Randomizer/Walkers").transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				int hashCode = child.GetGameobjectHashString().GetHashCode();
				NetNpcManager.walkers.Add(hashCode, child);
			}
			Transform transform2 = GameObject.Find("KILJUGUY").transform.Find("HikerPivot");
			NetNpcManager.walkers.Add(transform2.GetGameobjectHashString().GetHashCode(), transform2);
			this.drunkHiker2Fsm = transform2.Find("JokkeHiker2").GetPlayMaker("Logic");
			this.drunkHiker2Fsm.Initialize();
			this.drunkHiker2CarEvents = new FsmEvent[NetNpcManager.jokkeHiker_eventNames.Length];
			for (int j = 0; j < NetNpcManager.jokkeHiker_eventNames.Length; j++)
			{
				this.drunkHiker2CarEvents[j] = this.drunkHiker2Fsm.AddEvent(NetNpcManager.jokkeHiker_eventNames[j]);
				this.drunkHiker2Fsm.AddGlobalTransition(this.drunkHiker2CarEvents[j], NetNpcManager.jokkeHiker_stateNames[j]);
				int _i = j;
				this.drunkHiker2Fsm.InsertAction(NetNpcManager.jokkeHiker_stateNames[j], delegate
				{
					this.JokkeEnterCar(_i);
				}, 0);
			}
			this.drunkHiker2EnterCar = NetEvent<NetNpcManager>.Register("DH2Car", new NetEventHandler(this.OnJokkeEnterCar));
			Transform transform3 = GameObject.Find("TRAFFIC").transform;
			this.vehiclesHighway = transform3.Find("VehiclesHighway").gameObject;
			this.policeFsm = transform3.Find("Police").GetComponent<PlayMakerFSM>();
			if (BeerMPGlobals.IsHost)
			{
				this.policeFsm.InsertAction("State 3", delegate
				{
					this.PoliceSpawn(0);
				}, 0);
				this.policeFsm.InsertAction("State 4", delegate
				{
					this.PoliceSpawn(1);
				}, 0);
				this.policeFsm.InsertAction("State 5", delegate
				{
					this.PoliceSpawn(2);
				}, 0);
				this.policeFsm.InsertAction("State 6", delegate
				{
					this.PoliceSpawn(3);
				}, 0);
			}
			else
			{
				this.policeEvents = new FsmEvent[]
				{
					this.policeFsm.AddEvent("MP_SPAWN0"),
					this.policeFsm.AddEvent("MP_SPAWN1"),
					this.policeFsm.AddEvent("MP_SPAWN2"),
					this.policeFsm.AddEvent("MP_SPAWN3")
				};
				this.policeFsm.AddGlobalTransition(this.policeEvents[0], "State 3");
				this.policeFsm.AddGlobalTransition(this.policeEvents[1], "State 4");
				this.policeFsm.AddGlobalTransition(this.policeEvents[2], "State 5");
				this.policeFsm.AddGlobalTransition(this.policeEvents[3], "State 6");
				this.policeFsm.GetState("Cop1").Actions[2].Enabled = false;
				this.policeFsm.GetState("State 1").Actions[0].Enabled = false;
			}
			this.walkerSync = NetEvent<NetNpcManager>.Register("Walk", new NetEventHandler(this.OnWalkerNPCSync));
			this.highwayUpdateEvent = NetEvent<NetNpcManager>.Register("HighwayUpdate", new NetEventHandler(this.OnHighwayUpdate));
			this.policeUpdateEvent = NetEvent<NetNpcManager>.Register("PoliceUpdate", new NetEventHandler(this.OnPoliceUpdate));
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000D66F File Offset: 0x0000B86F
		[CompilerGenerated]
		private void <Start>b__15_1()
		{
			this.PoliceSpawn(0);
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000D678 File Offset: 0x0000B878
		[CompilerGenerated]
		private void <Start>b__15_2()
		{
			this.PoliceSpawn(1);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000D681 File Offset: 0x0000B881
		[CompilerGenerated]
		private void <Start>b__15_3()
		{
			this.PoliceSpawn(2);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000D68A File Offset: 0x0000B88A
		[CompilerGenerated]
		private void <Start>b__15_4()
		{
			this.PoliceSpawn(3);
		}

		// Token: 0x0400021C RID: 540
		private static Dictionary<int, Transform> walkers = new Dictionary<int, Transform>();

		// Token: 0x0400021D RID: 541
		private GameObject vehiclesHighway;

		// Token: 0x0400021E RID: 542
		private PlayMakerFSM policeFsm;

		// Token: 0x0400021F RID: 543
		private PlayMakerFSM drunkHiker2Fsm;

		// Token: 0x04000220 RID: 544
		private FsmEvent[] policeEvents;

		// Token: 0x04000221 RID: 545
		private FsmEvent[] drunkHiker2CarEvents;

		// Token: 0x04000222 RID: 546
		private bool highwayOn;

		// Token: 0x04000223 RID: 547
		private int policeIndex = -1;

		// Token: 0x04000224 RID: 548
		private int receivedDrunkHiker2 = -1;

		// Token: 0x04000225 RID: 549
		private static readonly string[] jokkeHiker_eventNames = new string[] { "MP_SATSUMA", "MP_MUSCLE", "MP_VAN", "MP_RUSCKO" };

		// Token: 0x04000226 RID: 550
		private static readonly string[] jokkeHiker_stateNames = new string[] { "Satsuma", "Muscle", "Van", "Ruscko" };

		// Token: 0x04000227 RID: 551
		private NetEvent<NetNpcManager> walkerSync;

		// Token: 0x04000228 RID: 552
		private NetEvent<NetNpcManager> highwayUpdateEvent;

		// Token: 0x04000229 RID: 553
		private NetEvent<NetNpcManager> policeUpdateEvent;

		// Token: 0x0400022A RID: 554
		private NetEvent<NetNpcManager> drunkHiker2EnterCar;

		// Token: 0x020000F0 RID: 240
		[CompilerGenerated]
		private sealed class <>c__DisplayClass15_0
		{
			// Token: 0x060004ED RID: 1261 RVA: 0x0001CAA1 File Offset: 0x0001ACA1
			public <>c__DisplayClass15_0()
			{
			}

			// Token: 0x060004EE RID: 1262 RVA: 0x0001CAA9 File Offset: 0x0001ACA9
			internal void <Start>b__5()
			{
				this.<>4__this.JokkeEnterCar(this._i);
			}

			// Token: 0x04000497 RID: 1175
			public int _i;

			// Token: 0x04000498 RID: 1176
			public NetNpcManager <>4__this;
		}
	}
}
