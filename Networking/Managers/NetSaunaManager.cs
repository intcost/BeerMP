using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000075 RID: 117
	[ManagerCreate(10)]
	internal class NetSaunaManager : MonoBehaviour
	{
		// Token: 0x06000361 RID: 865 RVA: 0x00019830 File Offset: 0x00017A30
		private void Start()
		{
			NetEvent<NetSaunaManager>.Register("Knob", new NetEventHandler(this.OnKnobScrew));
			NetEvent<NetSaunaManager>.Register("SimSync", new NetEventHandler(this.OnSimSync));
			NetEvent<NetSaunaManager>.Register("Steam", new NetEventHandler(this.OnSteam));
			Transform transform = GameObject.Find("YARD").transform.Find("Building/SAUNA/Sauna");
			PlayMakerFSM playMaker = transform.Find("Simulation").GetPlayMaker("Time");
			this.simMaxHeat = playMaker.FsmVariables.FindFsmFloat("MaxHeat");
			this.simTimer = playMaker.FsmVariables.FindFsmFloat("Time");
			this.simMaxSaunaHeat = playMaker.FsmVariables.FindFsmFloat("MaxSaunaHeat");
			this.simSaunaHeat = playMaker.FsmVariables.FindFsmFloat("SaunaHeat");
			this.simStoveHeat = playMaker.FsmVariables.FindFsmFloat("StoveHeat");
			this.simCoolingSauna = playMaker.FsmVariables.FindFsmFloat("CoolingSauna");
			PlayMakerFSM playMaker2 = transform.Find("Kiuas/ButtonPower").GetPlayMaker("Screw");
			this.powerRot = playMaker2.FsmVariables.FindFsmFloat("Rot");
			this.maxHeat = playMaker2.FsmVariables.FindFsmFloat("MaxHeat");
			this.powerKnobMesh = playMaker2.FsmVariables.FindFsmGameObject("CapMesh").Value.transform;
			Action<ulong> a = delegate(ulong target)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(false, -1);
					packet.Write(this.powerRot.Value, -1);
					if (target == 0UL)
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet, true);
					}
					else
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet, target, true);
					}
				}
			};
			playMaker2.InsertAction("Wait", delegate
			{
				a(0UL);
			}, -1);
			PlayMakerFSM playMaker3 = transform.Find("Kiuas/ButtonTime").GetPlayMaker("Screw");
			this.timerRot = playMaker3.FsmVariables.FindFsmFloat("Timer");
			this.timerMath1 = playMaker3.FsmVariables.FindFsmFloat("Math1");
			this.timerKnobMesh = playMaker3.FsmVariables.FindFsmGameObject("CapMesh").Value.transform;
			Action<ulong> b = delegate(ulong target)
			{
				using (Packet packet2 = new Packet(1))
				{
					packet2.Write(true, -1);
					packet2.Write(this.timerRot.Value, -1);
					if (target == 0UL)
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet2, true);
					}
					else
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet2, target, true);
					}
				}
			};
			playMaker3.InsertAction("Wait", delegate
			{
				b(0UL);
			}, -1);
			this.stoveTrigger = transform.Find("Kiuas/StoveTrigger").GetPlayMaker("Steam");
			this.stoveTrigger.InsertAction("Calc blur", delegate
			{
				if (this.receivedSteamEvent)
				{
					this.receivedSteamEvent = false;
					return;
				}
				using (Packet packet3 = new Packet(1))
				{
					NetEvent<NetSaunaManager>.Send("Steam", packet3, true);
				}
			}, -1);
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				a(user);
				b(user);
				this.SyncSim(user);
			};
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00019AB3 File Offset: 0x00017CB3
		private void Update()
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			this.saunaSimSyncTime += Time.deltaTime;
			if (this.saunaSimSyncTime >= 10f)
			{
				this.saunaSimSyncTime = 0f;
				this.SyncSim(0UL);
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00019AF0 File Offset: 0x00017CF0
		private void SyncSim(ulong target = 0UL)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.simMaxSaunaHeat.Value, -1);
				packet.Write(this.simSaunaHeat.Value, -1);
				packet.Write(this.simStoveHeat.Value, -1);
				packet.Write(this.simCoolingSauna.Value, -1);
				if (target == 0UL)
				{
					NetEvent<NetSaunaManager>.Send("SimSync", packet, true);
				}
				else
				{
					NetEvent<NetSaunaManager>.Send("SimSync", packet, target, true);
				}
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00019B88 File Offset: 0x00017D88
		private void OnSteam(ulong sender, Packet packet)
		{
			this.receivedSteamEvent = true;
			this.stoveTrigger.SendEvent("GLOBALEVENT");
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00019BA4 File Offset: 0x00017DA4
		private void OnSimSync(ulong sender, Packet packet)
		{
			this.simMaxSaunaHeat.Value = packet.ReadFloat(true);
			this.simSaunaHeat.Value = packet.ReadFloat(true);
			this.simStoveHeat.Value = packet.ReadFloat(true);
			this.simCoolingSauna.Value = packet.ReadFloat(true);
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00019BFC File Offset: 0x00017DFC
		private void OnKnobScrew(ulong sender, Packet packet)
		{
			bool flag = packet.ReadBool(true);
			float num = packet.ReadFloat(true);
			if (flag)
			{
				this.timerRot.Value = num;
				this.timerMath1.Value = (this.simTimer.Value = num * 6f);
				this.timerKnobMesh.localEulerAngles = Vector3.up * num;
			}
			else
			{
				this.powerRot.Value = num;
				this.maxHeat.Value = (this.simMaxHeat.Value = num / 300f);
				this.powerKnobMesh.localEulerAngles = Vector3.up * num;
			}
			MasterAudio.PlaySound3DAndForget("HouseFoley", flag ? this.timerKnobMesh : this.powerKnobMesh, false, 1f, null, 0f, "sauna_stove_knob");
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00019CD6 File Offset: 0x00017ED6
		public NetSaunaManager()
		{
		}

		// Token: 0x0400035E RID: 862
		private FsmFloat powerRot;

		// Token: 0x0400035F RID: 863
		private FsmFloat maxHeat;

		// Token: 0x04000360 RID: 864
		private FsmFloat timerRot;

		// Token: 0x04000361 RID: 865
		private FsmFloat timerMath1;

		// Token: 0x04000362 RID: 866
		private FsmFloat simMaxHeat;

		// Token: 0x04000363 RID: 867
		private FsmFloat simTimer;

		// Token: 0x04000364 RID: 868
		private FsmFloat simMaxSaunaHeat;

		// Token: 0x04000365 RID: 869
		private FsmFloat simSaunaHeat;

		// Token: 0x04000366 RID: 870
		private FsmFloat simStoveHeat;

		// Token: 0x04000367 RID: 871
		private FsmFloat simCoolingSauna;

		// Token: 0x04000368 RID: 872
		private Transform powerKnobMesh;

		// Token: 0x04000369 RID: 873
		private Transform timerKnobMesh;

		// Token: 0x0400036A RID: 874
		private PlayMakerFSM stoveTrigger;

		// Token: 0x0400036B RID: 875
		private float saunaSimSyncTime = 10f;

		// Token: 0x0400036C RID: 876
		private bool receivedSteamEvent;

		// Token: 0x0200014C RID: 332
		[CompilerGenerated]
		private sealed class <>c__DisplayClass15_0
		{
			// Token: 0x0600061F RID: 1567 RVA: 0x00020D83 File Offset: 0x0001EF83
			public <>c__DisplayClass15_0()
			{
			}

			// Token: 0x06000620 RID: 1568 RVA: 0x00020D8C File Offset: 0x0001EF8C
			internal void <Start>b__0(ulong target)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(false, -1);
					packet.Write(this.<>4__this.powerRot.Value, -1);
					if (target == 0UL)
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet, true);
					}
					else
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet, target, true);
					}
				}
			}

			// Token: 0x06000621 RID: 1569 RVA: 0x00020DFC File Offset: 0x0001EFFC
			internal void <Start>b__1()
			{
				this.a(0UL);
			}

			// Token: 0x06000622 RID: 1570 RVA: 0x00020E0C File Offset: 0x0001F00C
			internal void <Start>b__2(ulong target)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(true, -1);
					packet.Write(this.<>4__this.timerRot.Value, -1);
					if (target == 0UL)
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet, true);
					}
					else
					{
						NetEvent<NetSaunaManager>.Send("Knob", packet, target, true);
					}
				}
			}

			// Token: 0x06000623 RID: 1571 RVA: 0x00020E7C File Offset: 0x0001F07C
			internal void <Start>b__3()
			{
				this.b(0UL);
			}

			// Token: 0x06000624 RID: 1572 RVA: 0x00020E8C File Offset: 0x0001F08C
			internal void <Start>b__4()
			{
				if (this.<>4__this.receivedSteamEvent)
				{
					this.<>4__this.receivedSteamEvent = false;
					return;
				}
				using (Packet packet = new Packet(1))
				{
					NetEvent<NetSaunaManager>.Send("Steam", packet, true);
				}
			}

			// Token: 0x06000625 RID: 1573 RVA: 0x00020EE4 File Offset: 0x0001F0E4
			internal void <Start>b__5(ulong user)
			{
				this.a(user);
				this.b(user);
				this.<>4__this.SyncSim(user);
			}

			// Token: 0x040005B6 RID: 1462
			public NetSaunaManager <>4__this;

			// Token: 0x040005B7 RID: 1463
			public Action<ulong> a;

			// Token: 0x040005B8 RID: 1464
			public Action<ulong> b;
		}
	}
}
