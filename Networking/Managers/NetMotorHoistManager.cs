using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000076 RID: 118
	[ManagerCreate(10)]
	internal class NetMotorHoistManager : MonoBehaviour
	{
		// Token: 0x06000368 RID: 872 RVA: 0x00019CEC File Offset: 0x00017EEC
		private void Start()
		{
			NetEvent<NetMotorHoistManager>.Register("Init", new NetEventHandler(this.OnInitSync));
			NetEvent<NetMotorHoistManager>.Register("BeginMove", new NetEventHandler(this.OnBeginMovement));
			NetEvent<NetMotorHoistManager>.Register("EndMove", new NetEventHandler(this.OnEndMovement));
			Transform transform = GameObject.Find("ITEMS").transform.Find("motor hoist(itemx)");
			this.motorHoistArm = transform.Find("motorhoist_arm");
			this.handle = transform.Find("Pump/HandlePivot").GetComponent<Animation>();
			this.usageFsm = transform.Find("Pump/Trigger").GetPlayMaker("Usage");
			this.angle = this.usageFsm.FsmVariables.FindFsmFloat("Angle");
			Action<bool> beginMove = delegate(bool isUp)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(isUp, -1);
					packet.Write(this.angle.Value, -1);
					NetEvent<NetMotorHoistManager>.Send("BeginMove", packet, true);
				}
			};
			this.usageFsm.InsertAction("Up", delegate
			{
				beginMove(true);
			}, -1);
			this.usageFsm.InsertAction("Down", delegate
			{
				beginMove(false);
			}, -1);
			this.usageFsm.InsertAction("State 1", delegate
			{
				using (Packet packet2 = new Packet(1))
				{
					packet2.Write(this.angle.Value, -1);
					NetEvent<NetMotorHoistManager>.Send("EndMove", packet2, true);
				}
			}, -1);
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				using (Packet packet3 = new Packet(1))
				{
					packet3.Write(this.angle.Value, -1);
					NetEvent<NetMotorHoistManager>.Send("Init", packet3, user, true);
				}
			};
			BeerMPGlobals.OnMemberExit += delegate(ulong user)
			{
				if (this.hoistOwner == user)
				{
					this.OnEndMovement(user, this.angle.Value);
				}
			};
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00019E64 File Offset: 0x00018064
		private void OnInitSync(ulong sender, Packet packet)
		{
			float num = packet.ReadFloat(true);
			this.angle.Value = num;
			this.motorHoistArm.localEulerAngles = Vector3.right * num;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00019E9C File Offset: 0x0001809C
		private void OnBeginMovement(ulong sender, Packet packet)
		{
			bool flag = packet.ReadBool(true);
			float num = packet.ReadFloat(true);
			this.angle.Value = num;
			this.motorHoistArm.localEulerAngles = Vector3.right * num;
			this.handle.Play("motor_hoist_pump_down", PlayMode.StopAll);
			this.usageFsm.enabled = false;
			this.hoistOwner = sender;
			MasterAudio.PlaySound3DAndForget("HouseFoley", this.usageFsm.transform, false, 1f, null, 0f, "carjack1");
			if (flag)
			{
				this.isHoistMoving = true;
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00019F36 File Offset: 0x00018136
		private void OnEndMovement(ulong sender, Packet packet)
		{
			this.OnEndMovement(sender, packet.ReadFloat(true));
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00019F48 File Offset: 0x00018148
		private void OnEndMovement(ulong sender, float ang)
		{
			if (sender != this.hoistOwner && this.hoistOwner != 0UL)
			{
				return;
			}
			this.angle.Value = ang;
			this.motorHoistArm.localEulerAngles = Vector3.right * ang;
			this.handle.Play("motor_hoist_pump_up", PlayMode.StopAll);
			this.usageFsm.enabled = true;
			this.hoistOwner = 0UL;
			this.isHoistMoving = false;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00019FB8 File Offset: 0x000181B8
		private void Update()
		{
			if (this.isHoistMoving)
			{
				float num = this.angle.Value + 0.07f;
				this.angle.Value = num;
				this.motorHoistArm.localEulerAngles = Vector3.right * num;
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0001A001 File Offset: 0x00018201
		public NetMotorHoistManager()
		{
		}

		// Token: 0x0400036D RID: 877
		private FsmFloat angle;

		// Token: 0x0400036E RID: 878
		private Transform motorHoistArm;

		// Token: 0x0400036F RID: 879
		private Animation handle;

		// Token: 0x04000370 RID: 880
		private PlayMakerFSM usageFsm;

		// Token: 0x04000371 RID: 881
		private bool isHoistMoving;

		// Token: 0x04000372 RID: 882
		private ulong hoistOwner;

		// Token: 0x0200014D RID: 333
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_0
		{
			// Token: 0x06000626 RID: 1574 RVA: 0x00020F0A File Offset: 0x0001F10A
			public <>c__DisplayClass6_0()
			{
			}

			// Token: 0x06000627 RID: 1575 RVA: 0x00020F14 File Offset: 0x0001F114
			internal void <Start>b__0(bool isUp)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(isUp, -1);
					packet.Write(this.<>4__this.angle.Value, -1);
					NetEvent<NetMotorHoistManager>.Send("BeginMove", packet, true);
				}
			}

			// Token: 0x06000628 RID: 1576 RVA: 0x00020F70 File Offset: 0x0001F170
			internal void <Start>b__1()
			{
				this.beginMove(true);
			}

			// Token: 0x06000629 RID: 1577 RVA: 0x00020F7E File Offset: 0x0001F17E
			internal void <Start>b__2()
			{
				this.beginMove(false);
			}

			// Token: 0x0600062A RID: 1578 RVA: 0x00020F8C File Offset: 0x0001F18C
			internal void <Start>b__3()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.<>4__this.angle.Value, -1);
					NetEvent<NetMotorHoistManager>.Send("EndMove", packet, true);
				}
			}

			// Token: 0x0600062B RID: 1579 RVA: 0x00020FE0 File Offset: 0x0001F1E0
			internal void <Start>b__4(ulong user)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.<>4__this.angle.Value, -1);
					NetEvent<NetMotorHoistManager>.Send("Init", packet, user, true);
				}
			}

			// Token: 0x0600062C RID: 1580 RVA: 0x00021034 File Offset: 0x0001F234
			internal void <Start>b__5(ulong user)
			{
				if (this.<>4__this.hoistOwner == user)
				{
					this.<>4__this.OnEndMovement(user, this.<>4__this.angle.Value);
				}
			}

			// Token: 0x040005B9 RID: 1465
			public NetMotorHoistManager <>4__this;

			// Token: 0x040005BA RID: 1466
			public Action<bool> beginMove;
		}
	}
}
