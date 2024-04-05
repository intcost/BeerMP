using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000062 RID: 98
	internal class FsmStarter
	{
		// Token: 0x06000296 RID: 662 RVA: 0x00011370 File Offset: 0x0000F570
		public FsmStarter(FsmNetVehicle fsmVeh, PlayMakerFSM fsm)
		{
			FsmStarter <>4__this = this;
			this.fsmVeh = fsmVeh;
			this.fsm = fsm;
			fsm.Initialize();
			this.PlugHeat = fsm.FsmVariables.FindFsmFloat("PlugHeat");
			this.StartTime = fsm.FsmVariables.FindFsmFloat("StartTime");
			if (fsm.HasState("Starting engine"))
			{
				this.startingengine = fsm.AddEvent("MP_STARTINGENGINE");
				fsm.AddGlobalTransition(this.startingengine, "Starting engine");
				FsmState state = fsm.GetState("Starting engine");
				fsm.InsertAction("Starting engine", delegate
				{
					<>4__this.current = <>4__this.startingengine;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet = new Packet(1))
						{
							packet.Write((<>4__this.PlugHeat != null) ? <>4__this.PlugHeat.Value : (-1f), -1);
							packet.Write(<>4__this.StartTime.Value, -1);
							<>4__this.Startingengine.Send(packet, true);
						}
					}
					if (fsm.HasState("ACC / Glowplug"))
					{
						fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
					}
					if (fsm.HasState("Check clutch"))
					{
						FsmStateAction fsmStateAction = fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
						if (fsmStateAction != null)
						{
							fsmStateAction.Enabled = true;
						}
					}
				}, state.Actions.Length - 3);
			}
			if (fsm.HasState("Start engine"))
			{
				this.startengine = fsm.AddEvent("MP_STARTENGINE");
				fsm.AddGlobalTransition(this.startengine, "Start engine");
				fsm.InsertAction("Start engine", delegate
				{
					<>4__this.current = <>4__this.startengine;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet2 = new Packet(1))
						{
							<>4__this.Startengine.Send(packet2, true);
						}
					}
					<>4__this.doSync = true;
				}, 0);
			}
			if (fsm.HasState("Motor running"))
			{
				this.motorrunning = fsm.AddEvent("MP_MOTORRUNNING");
				fsm.AddGlobalTransition(this.motorrunning, "Motor running");
				fsm.InsertAction("Motor running", delegate
				{
					<>4__this.current = <>4__this.motorrunning;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet3 = new Packet(1))
						{
							<>4__this.Motorrunning.Send(packet3, true);
						}
					}
					if (fsmVeh.ignition != null)
					{
						fsmVeh.ignition.user = 0UL;
					}
					if (fsm.HasState("ACC / Glowplug"))
					{
						fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
					}
					if (fsm.HasState("Check clutch"))
					{
						FsmStateAction fsmStateAction2 = fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
						if (fsmStateAction2 != null)
						{
							fsmStateAction2.Enabled = true;
						}
					}
					<>4__this.doSync = true;
				}, 0);
			}
			if (fsm.HasState("Wait"))
			{
				this.wait = fsm.AddEvent("MP_WAIT");
				fsm.AddGlobalTransition(this.wait, "Wait");
				fsm.InsertAction("Wait", delegate
				{
					<>4__this.current = <>4__this.wait;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet4 = new Packet(1))
						{
							<>4__this.Wait.Send(packet4, true);
						}
					}
					<>4__this.doSync = true;
				}, 0);
			}
			if (fsm.HasState("ACC / Glowplug"))
			{
				this.accglowplug = fsm.AddEvent("MP_ACCGLOWPLUG");
				fsm.AddGlobalTransition(this.accglowplug, "ACC / Glowplug");
				fsm.InsertAction("ACC / Glowplug", delegate
				{
					<>4__this.current = <>4__this.accglowplug;
					if (<>4__this.user == 0UL)
					{
						<>4__this.user = BeerMPGlobals.UserID;
					}
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet5 = new Packet(1))
						{
							packet5.Write((<>4__this.PlugHeat != null) ? <>4__this.PlugHeat.Value : (-1f), -1);
							<>4__this.ACCGlowplug.Send(packet5, true);
							return;
						}
					}
					fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = false);
				}, 0);
			}
			if (fsm.HasState("Broken starter"))
			{
				this.brokenstarter = fsm.AddEvent("MP_BROKENSTARTER");
				fsm.AddGlobalTransition(this.startingengine, "Broken starter");
				FsmState state2 = fsm.GetState("Broken starter");
				fsm.InsertAction("Broken starter", delegate
				{
					<>4__this.current = <>4__this.brokenstarter;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet6 = new Packet(1))
						{
							<>4__this.Brokenstarter.Send(packet6, true);
						}
					}
					if (fsm.HasState("ACC / Glowplug"))
					{
						fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
					}
					if (fsm.HasState("Check clutch"))
					{
						FsmStateAction fsmStateAction3 = fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
						if (fsmStateAction3 != null)
						{
							fsmStateAction3.Enabled = true;
						}
					}
					<>4__this.doSync = true;
				}, state2.Actions.Length - 1);
			}
			if (fsm.HasState("Wait for start"))
			{
				this.waitforstart = fsm.AddEvent("MP_WAITFORSTART");
				fsm.AddGlobalTransition(this.waitforstart, "Wait for start");
				fsm.InsertAction("Wait for start", delegate
				{
					<>4__this.current = <>4__this.waitforstart;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet7 = new Packet(1))
						{
							<>4__this.Waitforstart.Send(packet7, true);
						}
					}
					if (fsm.HasState("Check clutch"))
					{
						FsmStateAction fsmStateAction4 = fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
						if (fsmStateAction4 != null)
						{
							fsmStateAction4.Enabled = true;
						}
					}
					<>4__this.user = 0UL;
					<>4__this.doSync = true;
				}, -1);
			}
			if (fsm.HasState("State 1"))
			{
				this.state1 = fsm.AddEvent("MP_STATE1");
				fsm.AddGlobalTransition(this.state1, "State 1");
				fsm.InsertAction("State 1", delegate
				{
					<>4__this.current = <>4__this.state1;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet8 = new Packet(1))
						{
							<>4__this.State1.Send(packet8, true);
						}
					}
					<>4__this.doSync = true;
					if (fsm.HasState("ACC / Glowplug"))
					{
						fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
					}
					if (fsm.HasState("Check clutch"))
					{
						FsmStateAction fsmStateAction5 = fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
						if (fsmStateAction5 != null)
						{
							fsmStateAction5.Enabled = true;
						}
					}
				}, 0);
			}
			if (fsm.HasState("Check clutch"))
			{
				this.checkclutch = fsm.AddEvent("MP_CHECKCLUTCH");
				fsm.AddGlobalTransition(this.checkclutch, "Check clutch");
				fsm.InsertAction("Check clutch", delegate
				{
					<>4__this.current = <>4__this.checkclutch;
					if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
					{
						using (Packet packet9 = new Packet(1))
						{
							<>4__this.Checkclutch.Send(packet9, true);
							goto IL_9E;
						}
					}
					FsmStateAction fsmStateAction6 = fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
					if (fsmStateAction6 != null)
					{
						fsmStateAction6.Enabled = false;
					}
					IL_9E:
					<>4__this.doSync = true;
				}, 0);
			}
			this.Startingengine = NetEvent<FsmStarter>.Register(string.Format("Startingengine{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnStartingengine));
			this.Startengine = NetEvent<FsmStarter>.Register(string.Format("Startengine{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnStartengine));
			this.Motorrunning = NetEvent<FsmStarter>.Register(string.Format("Motorrunning{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnMotorrunning));
			this.Wait = NetEvent<FsmStarter>.Register(string.Format("Wait{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnWait));
			this.ACCGlowplug = NetEvent<FsmStarter>.Register(string.Format("ACCGlowplug{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnACCGlowplug));
			this.Brokenstarter = NetEvent<FsmStarter>.Register(string.Format("Brokenstarter{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnBrokenstarter));
			this.Waitforstart = NetEvent<FsmStarter>.Register(string.Format("Waitforstart{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnWaitforstart));
			this.State1 = NetEvent<FsmStarter>.Register(string.Format("State1{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnState1));
			this.Checkclutch = NetEvent<FsmStarter>.Register(string.Format("Checkclutch{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnCheckclutch));
			BeerMPGlobals.OnMemberReady += delegate(ulong player)
			{
				if (<>4__this.current != null)
				{
					<>4__this.doSync = true;
					fsm.SendEvent(<>4__this.current.Name);
				}
			};
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00011966 File Offset: 0x0000FB66
		private void OnCheckclutch(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.checkclutch.Name);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0001198E File Offset: 0x0000FB8E
		private void OnState1(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.state1.Name);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x000119B6 File Offset: 0x0000FBB6
		private void OnWaitforstart(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.waitforstart.Name);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x000119DE File Offset: 0x0000FBDE
		private void OnBrokenstarter(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.brokenstarter.Name);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00011A08 File Offset: 0x0000FC08
		private void OnACCGlowplug(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			float num = packet.ReadFloat(true);
			if (num > 0f)
			{
				this.PlugHeat.Value = num;
			}
			this.fsm.SendEvent(this.accglowplug.Name);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00011A57 File Offset: 0x0000FC57
		private void OnWait(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.wait.Name);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00011A80 File Offset: 0x0000FC80
		private void OnMotorrunning(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			if (this.fsmVeh.ignition != null)
			{
				this.fsmVeh.ignition.user = 0UL;
			}
			this.user = 0UL;
			this.fsm.SendEvent(this.motorrunning.Name);
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00011ADA File Offset: 0x0000FCDA
		private void OnStartengine(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.startengine.Name);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00011B04 File Offset: 0x0000FD04
		private void OnStartingengine(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			float num = packet.ReadFloat(true);
			float num2 = packet.ReadFloat(true);
			this.fsm.SendEvent(this.startingengine.Name);
			if (num > 0f)
			{
				this.PlugHeat.Value = num;
			}
			this.StartTime.Value = num2;
		}

		// Token: 0x04000273 RID: 627
		public FsmNetVehicle fsmVeh;

		// Token: 0x04000274 RID: 628
		public PlayMakerFSM fsm;

		// Token: 0x04000275 RID: 629
		public FsmFloat PlugHeat;

		// Token: 0x04000276 RID: 630
		public FsmFloat StartTime;

		// Token: 0x04000277 RID: 631
		internal ulong user;

		// Token: 0x04000278 RID: 632
		private bool doSync = true;

		// Token: 0x04000279 RID: 633
		private NetEvent<FsmStarter> Startingengine;

		// Token: 0x0400027A RID: 634
		private NetEvent<FsmStarter> Startengine;

		// Token: 0x0400027B RID: 635
		private NetEvent<FsmStarter> Motorrunning;

		// Token: 0x0400027C RID: 636
		private NetEvent<FsmStarter> Wait;

		// Token: 0x0400027D RID: 637
		private NetEvent<FsmStarter> ACCGlowplug;

		// Token: 0x0400027E RID: 638
		private NetEvent<FsmStarter> Brokenstarter;

		// Token: 0x0400027F RID: 639
		private NetEvent<FsmStarter> Waitforstart;

		// Token: 0x04000280 RID: 640
		private NetEvent<FsmStarter> State1;

		// Token: 0x04000281 RID: 641
		private NetEvent<FsmStarter> Checkclutch;

		// Token: 0x04000282 RID: 642
		private FsmEvent startingengine;

		// Token: 0x04000283 RID: 643
		private FsmEvent startengine;

		// Token: 0x04000284 RID: 644
		private FsmEvent motorrunning;

		// Token: 0x04000285 RID: 645
		private FsmEvent wait;

		// Token: 0x04000286 RID: 646
		private FsmEvent accglowplug;

		// Token: 0x04000287 RID: 647
		private FsmEvent brokenstarter;

		// Token: 0x04000288 RID: 648
		private FsmEvent waitforstart;

		// Token: 0x04000289 RID: 649
		private FsmEvent state1;

		// Token: 0x0400028A RID: 650
		private FsmEvent checkclutch;

		// Token: 0x0400028B RID: 651
		private FsmEvent current;

		// Token: 0x0200010E RID: 270
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000556 RID: 1366 RVA: 0x0001E5A8 File Offset: 0x0001C7A8
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000557 RID: 1367 RVA: 0x0001E5B4 File Offset: 0x0001C7B4
			public <>c()
			{
			}

			// Token: 0x06000558 RID: 1368 RVA: 0x0001E5BC File Offset: 0x0001C7BC
			internal bool <.ctor>b__25_9(FsmStateAction x)
			{
				return x.GetType() == typeof(BoolTest);
			}

			// Token: 0x06000559 RID: 1369 RVA: 0x0001E5D0 File Offset: 0x0001C7D0
			internal bool <.ctor>b__25_10(FsmStateAction x)
			{
				return x.Enabled = true;
			}

			// Token: 0x0600055A RID: 1370 RVA: 0x0001E5E7 File Offset: 0x0001C7E7
			internal bool <.ctor>b__25_11(FsmStateAction x)
			{
				return x.GetType() == typeof(SendRandomEvent);
			}

			// Token: 0x0600055B RID: 1371 RVA: 0x0001E5FB File Offset: 0x0001C7FB
			internal bool <.ctor>b__25_12(FsmStateAction x)
			{
				return x.GetType() == typeof(BoolTest);
			}

			// Token: 0x0600055C RID: 1372 RVA: 0x0001E610 File Offset: 0x0001C810
			internal bool <.ctor>b__25_13(FsmStateAction x)
			{
				return x.Enabled = true;
			}

			// Token: 0x0600055D RID: 1373 RVA: 0x0001E627 File Offset: 0x0001C827
			internal bool <.ctor>b__25_14(FsmStateAction x)
			{
				return x.GetType() == typeof(SendRandomEvent);
			}

			// Token: 0x0600055E RID: 1374 RVA: 0x0001E63B File Offset: 0x0001C83B
			internal bool <.ctor>b__25_15(FsmStateAction x)
			{
				return x.GetType() == typeof(BoolTest);
			}

			// Token: 0x0600055F RID: 1375 RVA: 0x0001E650 File Offset: 0x0001C850
			internal bool <.ctor>b__25_16(FsmStateAction x)
			{
				return x.Enabled = false;
			}

			// Token: 0x06000560 RID: 1376 RVA: 0x0001E667 File Offset: 0x0001C867
			internal bool <.ctor>b__25_18(FsmStateAction x)
			{
				return x.GetType() == typeof(BoolTest);
			}

			// Token: 0x06000561 RID: 1377 RVA: 0x0001E67C File Offset: 0x0001C87C
			internal bool <.ctor>b__25_19(FsmStateAction x)
			{
				return x.Enabled = true;
			}

			// Token: 0x06000562 RID: 1378 RVA: 0x0001E693 File Offset: 0x0001C893
			internal bool <.ctor>b__25_20(FsmStateAction x)
			{
				return x.GetType() == typeof(SendRandomEvent);
			}

			// Token: 0x06000563 RID: 1379 RVA: 0x0001E6A7 File Offset: 0x0001C8A7
			internal bool <.ctor>b__25_21(FsmStateAction x)
			{
				return x.GetType() == typeof(SendRandomEvent);
			}

			// Token: 0x06000564 RID: 1380 RVA: 0x0001E6BB File Offset: 0x0001C8BB
			internal bool <.ctor>b__25_22(FsmStateAction x)
			{
				return x.GetType() == typeof(BoolTest);
			}

			// Token: 0x06000565 RID: 1381 RVA: 0x0001E6D0 File Offset: 0x0001C8D0
			internal bool <.ctor>b__25_23(FsmStateAction x)
			{
				return x.Enabled = true;
			}

			// Token: 0x06000566 RID: 1382 RVA: 0x0001E6E7 File Offset: 0x0001C8E7
			internal bool <.ctor>b__25_24(FsmStateAction x)
			{
				return x.GetType() == typeof(SendRandomEvent);
			}

			// Token: 0x06000567 RID: 1383 RVA: 0x0001E6FB File Offset: 0x0001C8FB
			internal bool <.ctor>b__25_25(FsmStateAction x)
			{
				return x.GetType() == typeof(SendRandomEvent);
			}

			// Token: 0x0400051C RID: 1308
			public static readonly FsmStarter.<>c <>9 = new FsmStarter.<>c();

			// Token: 0x0400051D RID: 1309
			public static Func<FsmStateAction, bool> <>9__25_9;

			// Token: 0x0400051E RID: 1310
			public static Func<FsmStateAction, bool> <>9__25_10;

			// Token: 0x0400051F RID: 1311
			public static Func<FsmStateAction, bool> <>9__25_11;

			// Token: 0x04000520 RID: 1312
			public static Func<FsmStateAction, bool> <>9__25_12;

			// Token: 0x04000521 RID: 1313
			public static Func<FsmStateAction, bool> <>9__25_13;

			// Token: 0x04000522 RID: 1314
			public static Func<FsmStateAction, bool> <>9__25_14;

			// Token: 0x04000523 RID: 1315
			public static Func<FsmStateAction, bool> <>9__25_15;

			// Token: 0x04000524 RID: 1316
			public static Func<FsmStateAction, bool> <>9__25_16;

			// Token: 0x04000525 RID: 1317
			public static Func<FsmStateAction, bool> <>9__25_18;

			// Token: 0x04000526 RID: 1318
			public static Func<FsmStateAction, bool> <>9__25_19;

			// Token: 0x04000527 RID: 1319
			public static Func<FsmStateAction, bool> <>9__25_20;

			// Token: 0x04000528 RID: 1320
			public static Func<FsmStateAction, bool> <>9__25_21;

			// Token: 0x04000529 RID: 1321
			public static Func<FsmStateAction, bool> <>9__25_22;

			// Token: 0x0400052A RID: 1322
			public static Func<FsmStateAction, bool> <>9__25_23;

			// Token: 0x0400052B RID: 1323
			public static Func<FsmStateAction, bool> <>9__25_24;

			// Token: 0x0400052C RID: 1324
			public static Func<FsmStateAction, bool> <>9__25_25;
		}

		// Token: 0x0200010F RID: 271
		[CompilerGenerated]
		private sealed class <>c__DisplayClass25_0
		{
			// Token: 0x06000568 RID: 1384 RVA: 0x0001E70F File Offset: 0x0001C90F
			public <>c__DisplayClass25_0()
			{
			}

			// Token: 0x06000569 RID: 1385 RVA: 0x0001E718 File Offset: 0x0001C918
			internal void <.ctor>b__8()
			{
				this.<>4__this.current = this.<>4__this.startingengine;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						packet.Write((this.<>4__this.PlugHeat != null) ? this.<>4__this.PlugHeat.Value : (-1f), -1);
						packet.Write(this.<>4__this.StartTime.Value, -1);
						this.<>4__this.Startingengine.Send(packet, true);
					}
				}
				if (this.fsm.HasState("ACC / Glowplug"))
				{
					this.fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
				}
				if (this.fsm.HasState("Check clutch"))
				{
					FsmStateAction fsmStateAction = this.fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
					if (fsmStateAction != null)
					{
						fsmStateAction.Enabled = true;
					}
				}
			}

			// Token: 0x0600056A RID: 1386 RVA: 0x0001E89C File Offset: 0x0001CA9C
			internal void <.ctor>b__0()
			{
				this.<>4__this.current = this.<>4__this.startengine;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Startengine.Send(packet, true);
					}
				}
				this.<>4__this.doSync = true;
			}

			// Token: 0x0600056B RID: 1387 RVA: 0x0001E920 File Offset: 0x0001CB20
			internal void <.ctor>b__1()
			{
				this.<>4__this.current = this.<>4__this.motorrunning;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Motorrunning.Send(packet, true);
					}
				}
				if (this.fsmVeh.ignition != null)
				{
					this.fsmVeh.ignition.user = 0UL;
				}
				if (this.fsm.HasState("ACC / Glowplug"))
				{
					this.fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
				}
				if (this.fsm.HasState("Check clutch"))
				{
					FsmStateAction fsmStateAction = this.fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
					if (fsmStateAction != null)
					{
						fsmStateAction.Enabled = true;
					}
				}
				this.<>4__this.doSync = true;
			}

			// Token: 0x0600056C RID: 1388 RVA: 0x0001EA90 File Offset: 0x0001CC90
			internal void <.ctor>b__2()
			{
				this.<>4__this.current = this.<>4__this.wait;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Wait.Send(packet, true);
					}
				}
				this.<>4__this.doSync = true;
			}

			// Token: 0x0600056D RID: 1389 RVA: 0x0001EB14 File Offset: 0x0001CD14
			internal void <.ctor>b__3()
			{
				this.<>4__this.current = this.<>4__this.accglowplug;
				if (this.<>4__this.user == 0UL)
				{
					this.<>4__this.user = BeerMPGlobals.UserID;
				}
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						packet.Write((this.<>4__this.PlugHeat != null) ? this.<>4__this.PlugHeat.Value : (-1f), -1);
						this.<>4__this.ACCGlowplug.Send(packet, true);
						return;
					}
				}
				this.fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = false);
			}

			// Token: 0x0600056E RID: 1390 RVA: 0x0001EC38 File Offset: 0x0001CE38
			internal void <.ctor>b__17()
			{
				this.<>4__this.current = this.<>4__this.brokenstarter;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Brokenstarter.Send(packet, true);
					}
				}
				if (this.fsm.HasState("ACC / Glowplug"))
				{
					this.fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
				}
				if (this.fsm.HasState("Check clutch"))
				{
					FsmStateAction fsmStateAction = this.fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
					if (fsmStateAction != null)
					{
						fsmStateAction.Enabled = true;
					}
				}
				this.<>4__this.doSync = true;
			}

			// Token: 0x0600056F RID: 1391 RVA: 0x0001ED88 File Offset: 0x0001CF88
			internal void <.ctor>b__4()
			{
				this.<>4__this.current = this.<>4__this.waitforstart;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Waitforstart.Send(packet, true);
					}
				}
				if (this.fsm.HasState("Check clutch"))
				{
					FsmStateAction fsmStateAction = this.fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
					if (fsmStateAction != null)
					{
						fsmStateAction.Enabled = true;
					}
				}
				this.<>4__this.user = 0UL;
				this.<>4__this.doSync = true;
			}

			// Token: 0x06000570 RID: 1392 RVA: 0x0001EE70 File Offset: 0x0001D070
			internal void <.ctor>b__5()
			{
				this.<>4__this.current = this.<>4__this.state1;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.State1.Send(packet, true);
					}
				}
				this.<>4__this.doSync = true;
				if (this.fsm.HasState("ACC / Glowplug"))
				{
					this.fsm.GetState("ACC / Glowplug").Actions.Where((FsmStateAction x) => x.GetType() == typeof(BoolTest)).ToArray<FsmStateAction>().Select((FsmStateAction x) => x.Enabled = true);
				}
				if (this.fsm.HasState("Check clutch"))
				{
					FsmStateAction fsmStateAction = this.fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
					if (fsmStateAction != null)
					{
						fsmStateAction.Enabled = true;
					}
				}
			}

			// Token: 0x06000571 RID: 1393 RVA: 0x0001EFC0 File Offset: 0x0001D1C0
			internal void <.ctor>b__6()
			{
				this.<>4__this.current = this.<>4__this.checkclutch;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Checkclutch.Send(packet, true);
						goto IL_9E;
					}
				}
				FsmStateAction fsmStateAction = this.fsm.GetState("Check clutch").Actions.FirstOrDefault((FsmStateAction x) => x.GetType() == typeof(SendRandomEvent));
				if (fsmStateAction != null)
				{
					fsmStateAction.Enabled = false;
				}
				IL_9E:
				this.<>4__this.doSync = true;
			}

			// Token: 0x06000572 RID: 1394 RVA: 0x0001F088 File Offset: 0x0001D288
			internal void <.ctor>b__7(ulong player)
			{
				if (this.<>4__this.current != null)
				{
					this.<>4__this.doSync = true;
					this.fsm.SendEvent(this.<>4__this.current.Name);
				}
			}

			// Token: 0x0400052D RID: 1325
			public FsmStarter <>4__this;

			// Token: 0x0400052E RID: 1326
			public PlayMakerFSM fsm;

			// Token: 0x0400052F RID: 1327
			public FsmNetVehicle fsmVeh;
		}
	}
}
