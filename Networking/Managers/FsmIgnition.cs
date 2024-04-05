using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using Steamworks;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000063 RID: 99
	internal class FsmIgnition
	{
		// Token: 0x060002A0 RID: 672 RVA: 0x00011B68 File Offset: 0x0000FD68
		public FsmIgnition(FsmNetVehicle fsmVeh, PlayMakerFSM fsm)
		{
			FsmIgnition <>4__this = this;
			this.fsmVeh = fsmVeh;
			this.fsm = fsm;
			if (fsmVeh.starter != null)
			{
				this.starter = fsmVeh.starter;
			}
			fsm.Initialize();
			this.sound = fsm.AddEvent("MP_PUTKEYIN");
			fsm.AddGlobalTransition(this.sound, "Sound");
			fsm.InsertAction("Sound", delegate
			{
				<>4__this.current = <>4__this.sound;
				if (<>4__this.doSync)
				{
					using (Packet packet = new Packet(1))
					{
						if (<>4__this.user == 0UL)
						{
							<>4__this.user = BeerMPGlobals.UserID;
							if (<>4__this.starter != null)
							{
								<>4__this.starter.user = BeerMPGlobals.UserID;
							}
						}
						<>4__this.Sound.Send(packet, true);
					}
				}
			}, 0);
			this.accon = fsm.AddEvent("MP_ACCON");
			fsm.AddGlobalTransition(this.accon, "ACC on");
			FsmState state = fsm.GetState("ACC on");
			fsm.InsertAction("ACC on", delegate
			{
				<>4__this.current = <>4__this.accon;
				if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet2 = new Packet(1))
					{
						<>4__this.AccOn.Send(packet2, true);
						goto IL_6A;
					}
				}
				fsm.SendEvent("FINISHED");
				IL_6A:
				<>4__this.doSync = true;
			}, state.Actions.Length - 1);
			this.shutoff = fsm.AddEvent("MP_SHUTOFF");
			fsm.AddGlobalTransition(this.shutoff, "Shut off");
			fsm.InsertAction("Shut off", delegate
			{
				<>4__this.current = <>4__this.shutoff;
				if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet3 = new Packet(1))
					{
						<>4__this.Shutoff.Send(packet3, true);
					}
				}
				<>4__this.doSync = true;
			}, 0);
			this.motoroff = fsm.AddEvent("MP_MOTOROFF");
			fsm.AddGlobalTransition(this.motoroff, "Motor OFF");
			fsm.InsertAction("Motor OFF", delegate
			{
				<>4__this.current = <>4__this.motoroff;
				if (<>4__this.doSync)
				{
					using (Packet packet4 = new Packet(1))
					{
						<>4__this.MotorOFF.Send(packet4, true);
					}
				}
				<>4__this.user = 0UL;
				<>4__this.doSync = true;
			}, 0);
			this.motorstarting = fsm.AddEvent("MP_MOTORSTARTING");
			fsm.AddGlobalTransition(this.motorstarting, "Motor starting");
			FsmState state2 = fsm.GetState("Motor starting");
			fsm.InsertAction("Motor starting", delegate
			{
				<>4__this.current = <>4__this.motorstarting;
				if (<>4__this.doSync && <>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet5 = new Packet(1))
					{
						<>4__this.Motorstarting.Send(packet5, true);
						goto IL_75;
					}
				}
				fsm.SendEvent(<>4__this.waitbutton.Name);
				IL_75:
				<>4__this.doSync = true;
			}, state2.Actions.Length - 1);
			this.waitbutton = fsm.AddEvent("MP_WAITBUTTON");
			fsm.AddGlobalTransition(this.waitbutton, "Wait button");
			this.Sound = NetEvent<FsmIgnition>.Register(string.Format("Sound{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnSound));
			this.AccOn = NetEvent<FsmIgnition>.Register(string.Format("AccOn{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnAccOn));
			this.Shutoff = NetEvent<FsmIgnition>.Register(string.Format("Shutoff{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnShutoff));
			this.MotorOFF = NetEvent<FsmIgnition>.Register(string.Format("MotorOFF{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnMotorOFF));
			this.Motorstarting = NetEvent<FsmIgnition>.Register(string.Format("Motorstarting{0}", fsmVeh.netVehicle.hash), new NetEventHandler(this.OnMotorstarting));
			BeerMPGlobals.OnMemberReady += delegate(ulong player)
			{
				if (<>4__this.current != null)
				{
					<>4__this.doSync = true;
					fsm.SendEvent(<>4__this.current.Name);
				}
			};
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00011EA8 File Offset: 0x000100A8
		private void OnMotorstarting(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			if (this.starter != null && this.starter.user == 0UL)
			{
				this.starter.user = sender;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.motorstarting.Name);
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00011EFC File Offset: 0x000100FC
		private void OnMotorOFF(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.user = 0UL;
			this.doSync = false;
			this.fsm.SendEvent(this.motoroff.Name);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00011F2C File Offset: 0x0001012C
		private void OnShutoff(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.shutoff.Name);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00011F54 File Offset: 0x00010154
		private void OnAccOn(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			this.doSync = false;
			this.fsm.SendEvent(this.accon.Name);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00011F7C File Offset: 0x0001017C
		private void OnSound(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			if (this.user == 0UL)
			{
				Console.Log("FsmIgnition & FsmStarter: " + SteamFriends.GetFriendPersonaName((CSteamID)sender) + " is new User!", true);
				this.user = sender;
				if (this.starter != null)
				{
					this.starter.user = sender;
				}
			}
			this.doSync = false;
			this.fsm.SendEvent(this.sound.Name);
		}

		// Token: 0x0400028C RID: 652
		public FsmNetVehicle fsmVeh;

		// Token: 0x0400028D RID: 653
		public PlayMakerFSM fsm;

		// Token: 0x0400028E RID: 654
		public FsmStarter starter;

		// Token: 0x0400028F RID: 655
		internal ulong user;

		// Token: 0x04000290 RID: 656
		private bool doSync = true;

		// Token: 0x04000291 RID: 657
		private NetEvent<FsmIgnition> Sound;

		// Token: 0x04000292 RID: 658
		private NetEvent<FsmIgnition> AccOn;

		// Token: 0x04000293 RID: 659
		private NetEvent<FsmIgnition> Shutoff;

		// Token: 0x04000294 RID: 660
		private NetEvent<FsmIgnition> MotorOFF;

		// Token: 0x04000295 RID: 661
		private NetEvent<FsmIgnition> Motorstarting;

		// Token: 0x04000296 RID: 662
		private FsmEvent sound;

		// Token: 0x04000297 RID: 663
		private FsmEvent accon;

		// Token: 0x04000298 RID: 664
		private FsmEvent shutoff;

		// Token: 0x04000299 RID: 665
		private FsmEvent motoroff;

		// Token: 0x0400029A RID: 666
		private FsmEvent motorstarting;

		// Token: 0x0400029B RID: 667
		private FsmEvent waitbutton;

		// Token: 0x0400029C RID: 668
		private FsmEvent current;

		// Token: 0x02000110 RID: 272
		[CompilerGenerated]
		private sealed class <>c__DisplayClass17_0
		{
			// Token: 0x06000573 RID: 1395 RVA: 0x0001F0BE File Offset: 0x0001D2BE
			public <>c__DisplayClass17_0()
			{
			}

			// Token: 0x06000574 RID: 1396 RVA: 0x0001F0C8 File Offset: 0x0001D2C8
			internal void <.ctor>b__0()
			{
				this.<>4__this.current = this.<>4__this.sound;
				if (this.<>4__this.doSync)
				{
					using (Packet packet = new Packet(1))
					{
						if (this.<>4__this.user == 0UL)
						{
							this.<>4__this.user = BeerMPGlobals.UserID;
							if (this.<>4__this.starter != null)
							{
								this.<>4__this.starter.user = BeerMPGlobals.UserID;
							}
						}
						this.<>4__this.Sound.Send(packet, true);
					}
				}
			}

			// Token: 0x06000575 RID: 1397 RVA: 0x0001F16C File Offset: 0x0001D36C
			internal void <.ctor>b__1()
			{
				this.<>4__this.current = this.<>4__this.accon;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.AccOn.Send(packet, true);
						goto IL_6A;
					}
				}
				this.fsm.SendEvent("FINISHED");
				IL_6A:
				this.<>4__this.doSync = true;
			}

			// Token: 0x06000576 RID: 1398 RVA: 0x0001F200 File Offset: 0x0001D400
			internal void <.ctor>b__2()
			{
				this.<>4__this.current = this.<>4__this.shutoff;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Shutoff.Send(packet, true);
					}
				}
				this.<>4__this.doSync = true;
			}

			// Token: 0x06000577 RID: 1399 RVA: 0x0001F284 File Offset: 0x0001D484
			internal void <.ctor>b__3()
			{
				this.<>4__this.current = this.<>4__this.motoroff;
				if (this.<>4__this.doSync)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.MotorOFF.Send(packet, true);
					}
				}
				this.<>4__this.user = 0UL;
				this.<>4__this.doSync = true;
			}

			// Token: 0x06000578 RID: 1400 RVA: 0x0001F304 File Offset: 0x0001D504
			internal void <.ctor>b__4()
			{
				this.<>4__this.current = this.<>4__this.motorstarting;
				if (this.<>4__this.doSync && this.<>4__this.user == BeerMPGlobals.UserID)
				{
					using (Packet packet = new Packet(1))
					{
						this.<>4__this.Motorstarting.Send(packet, true);
						goto IL_75;
					}
				}
				this.fsm.SendEvent(this.<>4__this.waitbutton.Name);
				IL_75:
				this.<>4__this.doSync = true;
			}

			// Token: 0x06000579 RID: 1401 RVA: 0x0001F3A4 File Offset: 0x0001D5A4
			internal void <.ctor>b__5(ulong player)
			{
				if (this.<>4__this.current != null)
				{
					this.<>4__this.doSync = true;
					this.fsm.SendEvent(this.<>4__this.current.Name);
				}
			}

			// Token: 0x04000530 RID: 1328
			public FsmIgnition <>4__this;

			// Token: 0x04000531 RID: 1329
			public PlayMakerFSM fsm;
		}
	}
}
