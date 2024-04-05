using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200006E RID: 110
	[ManagerCreate(10)]
	internal class NetWaterSourceManager : MonoBehaviour
	{
		// Token: 0x06000321 RID: 801 RVA: 0x00016F90 File Offset: 0x00015190
		private void Start()
		{
			NetEvent<NetWaterSourceManager>.Register("Tap", new NetEventHandler(this.OnWaterTap));
			NetEvent<NetWaterSourceManager>.Register("Shower", new NetEventHandler(this.OnShower));
			NetEvent<NetWaterSourceManager>.Register("Well", new NetEventHandler(this.OnWaterWell));
			List<PlayMakerFSM> list = Resources.FindObjectsOfTypeAll<PlayMakerFSM>().Where((PlayMakerFSM x) => x.FsmName == "Use").ToList<PlayMakerFSM>();
			for (int i = 0; i < list.Count; i++)
			{
				PlayMakerFSM fsm = list[i];
				if (!(fsm.transform.parent == null))
				{
					if (fsm.transform.parent.name == "KitchenWaterTap")
					{
						FsmBool tapOn2 = fsm.FsmVariables.FindFsmBool("SwitchOn");
						FsmEvent fsmEvent = fsm.AddEvent("MP_ON");
						fsm.AddGlobalTransition(fsmEvent, "ON");
						FsmEvent fsmEvent2 = fsm.AddEvent("MP_OFF");
						fsm.AddGlobalTransition(fsmEvent2, "OFF");
						fsm.InsertAction("Position", delegate
						{
							using (Packet packet = new Packet(1))
							{
								packet.Write(fsm.transform.position.GetHashCode(), -1);
								packet.Write(!tapOn2.Value, -1);
								NetEvent<NetWaterSourceManager>.Send("Tap", packet, true);
							}
						}, 0);
						this.waterTaps.Add(new NetWaterSourceManager.WaterTap
						{
							fsm = fsm,
							tapOn = tapOn2
						});
					}
					else if (fsm.transform.parent.name == "Shower")
					{
						PlayMakerFSM playMaker = fsm.transform.parent.Find("Valve").GetPlayMaker("Switch");
						FsmBool showerSwitch = fsm.FsmVariables.FindFsmBool("ShowerSwitch");
						FsmEvent fsmEvent3 = fsm.AddEvent("MP_ON");
						fsm.AddGlobalTransition(fsmEvent3, "Shower");
						FsmEvent fsmEvent4 = fsm.AddEvent("MP_OFF");
						fsm.AddGlobalTransition(fsmEvent4, "State 1");
						fsm.InsertAction("Position", delegate
						{
							using (Packet packet2 = new Packet(1))
							{
								packet2.Write(fsm.transform.position.GetHashCode(), -1);
								packet2.Write(true, -1);
								packet2.Write(!showerSwitch.Value, -1);
								NetEvent<NetWaterSourceManager>.Send("Shower", packet2, true);
							}
						}, 0);
						FsmBool tapOn = playMaker.FsmVariables.FindFsmBool("Valve");
						fsmEvent3 = playMaker.AddEvent("MP_ON");
						playMaker.AddGlobalTransition(fsmEvent3, "ON");
						fsmEvent4 = playMaker.AddEvent("MP_OFF");
						playMaker.AddGlobalTransition(fsmEvent4, "OFF");
						playMaker.InsertAction("Position", delegate
						{
							using (Packet packet3 = new Packet(1))
							{
								packet3.Write(fsm.transform.position.GetHashCode(), -1);
								packet3.Write(!tapOn.Value, -1);
								packet3.Write(false, -1);
								NetEvent<NetWaterSourceManager>.Send("Shower", packet3, true);
							}
						}, 0);
						this.showers.Add(new NetWaterSourceManager.Shower
						{
							valve = playMaker,
							showerSwitch = fsm,
							tapOn = tapOn,
							showerOn = showerSwitch
						});
					}
					else if (fsm.transform.name == "Trigger")
					{
						bool flag = false;
						Transform transform = fsm.transform;
						while (transform.parent != null)
						{
							if (transform.name == "WaterWell")
							{
								flag = true;
								break;
							}
							transform = transform.parent;
						}
						if (flag)
						{
							FsmEvent fsmEvent5 = fsm.AddEvent("MP_USE");
							NetWaterSourceManager.WaterWell well = new NetWaterSourceManager.WaterWell
							{
								fsm = fsm
							};
							fsm.AddGlobalTransition(fsmEvent5, "Move lever");
							fsm.InsertAction("Move lever", delegate
							{
								if (well.receivedWellEvent)
								{
									well.receivedWellEvent = false;
									return;
								}
								using (Packet packet4 = new Packet(1))
								{
									packet4.Write(fsm.transform.position.GetHashCode(), -1);
									NetEvent<NetWaterSourceManager>.Send("Well", packet4, true);
								}
							}, 0);
							this.wells.Add(well);
						}
					}
				}
			}
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				for (int j = 0; j < this.waterTaps.Count; j++)
				{
					using (Packet packet5 = new Packet(1))
					{
						packet5.Write(this.waterTaps[j].fsm.transform.position.GetHashCode(), -1);
						packet5.Write(this.waterTaps[j].tapOn.Value, -1);
						NetEvent<NetWaterSourceManager>.Send("Tap", packet5, true);
					}
				}
				for (int k = 0; k < this.showers.Count; k++)
				{
					using (Packet packet6 = new Packet(1))
					{
						packet6.Write(this.showers[k].showerSwitch.transform.position.GetHashCode(), -1);
						packet6.Write(this.showers[k].tapOn.Value, -1);
						packet6.Write(this.showers[k].showerOn.Value, -1);
						NetEvent<NetWaterSourceManager>.Send("Shower", packet6, true);
					}
				}
			};
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00017428 File Offset: 0x00015628
		private void OnWaterTap(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			bool flag = packet.ReadBool(true);
			NetWaterSourceManager.WaterTap waterTap = this.waterTaps.FirstOrDefault((NetWaterSourceManager.WaterTap t) => t.fsm.transform.position.GetHashCode() == hash);
			if (waterTap != null)
			{
				bool flag2 = waterTap.tapOn.Value != flag;
				waterTap.tapOn.Value = flag;
				if (flag2)
				{
					waterTap.fsm.SendEvent(flag ? "MP_ON" : "MP_OFF");
				}
			}
		}

		// Token: 0x06000323 RID: 803 RVA: 0x000174A4 File Offset: 0x000156A4
		private void OnShower(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			bool flag = packet.ReadBool(true);
			bool flag2 = packet.ReadBool(true);
			NetWaterSourceManager.Shower shower = this.showers.FirstOrDefault((NetWaterSourceManager.Shower t) => t.showerSwitch.transform.position.GetHashCode() == hash);
			if (shower != null)
			{
				bool flag3 = shower.tapOn.Value != flag;
				shower.tapOn.Value = flag;
				if (flag3)
				{
					shower.valve.SendEvent(flag ? "MP_ON" : "MP_OFF");
				}
				bool flag4 = shower.showerOn.Value != flag2;
				shower.showerOn.Value = flag2;
				if (flag4)
				{
					shower.showerSwitch.SendEvent(flag2 ? "MP_ON" : "MP_OFF");
				}
			}
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00017564 File Offset: 0x00015764
		private void OnWaterWell(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			NetWaterSourceManager.WaterWell waterWell = this.wells.FirstOrDefault((NetWaterSourceManager.WaterWell f) => f.fsm.transform.position.GetHashCode() == hash);
			if (waterWell != null)
			{
				waterWell.receivedWellEvent = true;
				waterWell.fsm.SendEvent("MP_USE");
			}
		}

		// Token: 0x06000325 RID: 805 RVA: 0x000175B6 File Offset: 0x000157B6
		public NetWaterSourceManager()
		{
		}

		// Token: 0x06000326 RID: 806 RVA: 0x000175E0 File Offset: 0x000157E0
		[CompilerGenerated]
		private void <Start>b__6_1(ulong user)
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			for (int i = 0; i < this.waterTaps.Count; i++)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.waterTaps[i].fsm.transform.position.GetHashCode(), -1);
					packet.Write(this.waterTaps[i].tapOn.Value, -1);
					NetEvent<NetWaterSourceManager>.Send("Tap", packet, true);
				}
			}
			for (int j = 0; j < this.showers.Count; j++)
			{
				using (Packet packet2 = new Packet(1))
				{
					packet2.Write(this.showers[j].showerSwitch.transform.position.GetHashCode(), -1);
					packet2.Write(this.showers[j].tapOn.Value, -1);
					packet2.Write(this.showers[j].showerOn.Value, -1);
					NetEvent<NetWaterSourceManager>.Send("Shower", packet2, true);
				}
			}
		}

		// Token: 0x04000322 RID: 802
		private List<NetWaterSourceManager.WaterTap> waterTaps = new List<NetWaterSourceManager.WaterTap>();

		// Token: 0x04000323 RID: 803
		private List<NetWaterSourceManager.Shower> showers = new List<NetWaterSourceManager.Shower>();

		// Token: 0x04000324 RID: 804
		private List<NetWaterSourceManager.WaterWell> wells = new List<NetWaterSourceManager.WaterWell>();

		// Token: 0x02000130 RID: 304
		private class WaterTap
		{
			// Token: 0x060005D1 RID: 1489 RVA: 0x0001FED6 File Offset: 0x0001E0D6
			public WaterTap()
			{
			}

			// Token: 0x04000573 RID: 1395
			public PlayMakerFSM fsm;

			// Token: 0x04000574 RID: 1396
			public FsmBool tapOn;
		}

		// Token: 0x02000131 RID: 305
		private class Shower
		{
			// Token: 0x060005D2 RID: 1490 RVA: 0x0001FEDE File Offset: 0x0001E0DE
			public Shower()
			{
			}

			// Token: 0x04000575 RID: 1397
			public PlayMakerFSM valve;

			// Token: 0x04000576 RID: 1398
			public PlayMakerFSM showerSwitch;

			// Token: 0x04000577 RID: 1399
			public FsmBool tapOn;

			// Token: 0x04000578 RID: 1400
			public FsmBool showerOn;
		}

		// Token: 0x02000132 RID: 306
		private class WaterWell
		{
			// Token: 0x060005D3 RID: 1491 RVA: 0x0001FEE6 File Offset: 0x0001E0E6
			public WaterWell()
			{
			}

			// Token: 0x04000579 RID: 1401
			public PlayMakerFSM fsm;

			// Token: 0x0400057A RID: 1402
			public bool receivedWellEvent;
		}

		// Token: 0x02000133 RID: 307
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060005D4 RID: 1492 RVA: 0x0001FEEE File Offset: 0x0001E0EE
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060005D5 RID: 1493 RVA: 0x0001FEFA File Offset: 0x0001E0FA
			public <>c()
			{
			}

			// Token: 0x060005D6 RID: 1494 RVA: 0x0001FF02 File Offset: 0x0001E102
			internal bool <Start>b__6_0(PlayMakerFSM x)
			{
				return x.FsmName == "Use";
			}

			// Token: 0x0400057B RID: 1403
			public static readonly NetWaterSourceManager.<>c <>9 = new NetWaterSourceManager.<>c();

			// Token: 0x0400057C RID: 1404
			public static Func<PlayMakerFSM, bool> <>9__6_0;
		}

		// Token: 0x02000134 RID: 308
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_0
		{
			// Token: 0x060005D7 RID: 1495 RVA: 0x0001FF14 File Offset: 0x0001E114
			public <>c__DisplayClass6_0()
			{
			}

			// Token: 0x0400057D RID: 1405
			public PlayMakerFSM fsm;
		}

		// Token: 0x02000135 RID: 309
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_1
		{
			// Token: 0x060005D8 RID: 1496 RVA: 0x0001FF1C File Offset: 0x0001E11C
			public <>c__DisplayClass6_1()
			{
			}

			// Token: 0x060005D9 RID: 1497 RVA: 0x0001FF24 File Offset: 0x0001E124
			internal void <Start>b__2()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.CS$<>8__locals1.fsm.transform.position.GetHashCode(), -1);
					packet.Write(!this.tapOn.Value, -1);
					NetEvent<NetWaterSourceManager>.Send("Tap", packet, true);
				}
			}

			// Token: 0x0400057E RID: 1406
			public FsmBool tapOn;

			// Token: 0x0400057F RID: 1407
			public NetWaterSourceManager.<>c__DisplayClass6_0 CS$<>8__locals1;
		}

		// Token: 0x02000136 RID: 310
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_2
		{
			// Token: 0x060005DA RID: 1498 RVA: 0x0001FFA0 File Offset: 0x0001E1A0
			public <>c__DisplayClass6_2()
			{
			}

			// Token: 0x060005DB RID: 1499 RVA: 0x0001FFA8 File Offset: 0x0001E1A8
			internal void <Start>b__3()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.CS$<>8__locals2.fsm.transform.position.GetHashCode(), -1);
					packet.Write(true, -1);
					packet.Write(!this.showerSwitch.Value, -1);
					NetEvent<NetWaterSourceManager>.Send("Shower", packet, true);
				}
			}

			// Token: 0x060005DC RID: 1500 RVA: 0x0002002C File Offset: 0x0001E22C
			internal void <Start>b__4()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.CS$<>8__locals2.fsm.transform.position.GetHashCode(), -1);
					packet.Write(!this.tapOn.Value, -1);
					packet.Write(false, -1);
					NetEvent<NetWaterSourceManager>.Send("Shower", packet, true);
				}
			}

			// Token: 0x04000580 RID: 1408
			public FsmBool showerSwitch;

			// Token: 0x04000581 RID: 1409
			public FsmBool tapOn;

			// Token: 0x04000582 RID: 1410
			public NetWaterSourceManager.<>c__DisplayClass6_0 CS$<>8__locals2;
		}

		// Token: 0x02000137 RID: 311
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_3
		{
			// Token: 0x060005DD RID: 1501 RVA: 0x000200B0 File Offset: 0x0001E2B0
			public <>c__DisplayClass6_3()
			{
			}

			// Token: 0x060005DE RID: 1502 RVA: 0x000200B8 File Offset: 0x0001E2B8
			internal void <Start>b__5()
			{
				if (this.well.receivedWellEvent)
				{
					this.well.receivedWellEvent = false;
					return;
				}
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.CS$<>8__locals3.fsm.transform.position.GetHashCode(), -1);
					NetEvent<NetWaterSourceManager>.Send("Well", packet, true);
				}
			}

			// Token: 0x04000583 RID: 1411
			public NetWaterSourceManager.WaterWell well;

			// Token: 0x04000584 RID: 1412
			public NetWaterSourceManager.<>c__DisplayClass6_0 CS$<>8__locals3;
		}

		// Token: 0x02000138 RID: 312
		[CompilerGenerated]
		private sealed class <>c__DisplayClass7_0
		{
			// Token: 0x060005DF RID: 1503 RVA: 0x00020138 File Offset: 0x0001E338
			public <>c__DisplayClass7_0()
			{
			}

			// Token: 0x060005E0 RID: 1504 RVA: 0x00020140 File Offset: 0x0001E340
			internal bool <OnWaterTap>b__0(NetWaterSourceManager.WaterTap t)
			{
				return t.fsm.transform.position.GetHashCode() == this.hash;
			}

			// Token: 0x04000585 RID: 1413
			public int hash;
		}

		// Token: 0x02000139 RID: 313
		[CompilerGenerated]
		private sealed class <>c__DisplayClass8_0
		{
			// Token: 0x060005E1 RID: 1505 RVA: 0x00020173 File Offset: 0x0001E373
			public <>c__DisplayClass8_0()
			{
			}

			// Token: 0x060005E2 RID: 1506 RVA: 0x0002017C File Offset: 0x0001E37C
			internal bool <OnShower>b__0(NetWaterSourceManager.Shower t)
			{
				return t.showerSwitch.transform.position.GetHashCode() == this.hash;
			}

			// Token: 0x04000586 RID: 1414
			public int hash;
		}

		// Token: 0x0200013A RID: 314
		[CompilerGenerated]
		private sealed class <>c__DisplayClass9_0
		{
			// Token: 0x060005E3 RID: 1507 RVA: 0x000201AF File Offset: 0x0001E3AF
			public <>c__DisplayClass9_0()
			{
			}

			// Token: 0x060005E4 RID: 1508 RVA: 0x000201B8 File Offset: 0x0001E3B8
			internal bool <OnWaterWell>b__0(NetWaterSourceManager.WaterWell f)
			{
				return f.fsm.transform.position.GetHashCode() == this.hash;
			}

			// Token: 0x04000587 RID: 1415
			public int hash;
		}
	}
}
