using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000073 RID: 115
	[ManagerCreate(10)]
	internal class NetOvenManager : MonoBehaviour
	{
		// Token: 0x06000358 RID: 856 RVA: 0x000193B0 File Offset: 0x000175B0
		private void Start()
		{
			NetEvent<NetOvenManager>.Register("KnobTurn", new NetEventHandler(this.OnKnobTurn));
			NetEvent<NetOvenManager>.Register("SimSync", new NetEventHandler(this.OnSimSync));
			this.knobData = new FsmFloat[4];
			this.knobRot = new FsmFloat[4];
			this.hotplateTemps = new FsmFloat[4];
			this.knobMesh = new Transform[4];
			Action<ulong>[] knobSyncs = new Action<ulong>[4];
			Transform transform = GameObject.Find("YARD").transform.Find("Building/KITCHEN/OvenStove");
			for (int i = 0; i < 4; i++)
			{
				PlayMakerFSM playMaker = transform.Find("KnobPower" + (i + 1).ToString()).GetPlayMaker("Screw");
				FsmFloat data = playMaker.FsmVariables.FindFsmFloat("Data");
				FsmFloat fsmFloat = playMaker.FsmVariables.FindFsmFloat("Rot");
				Transform mesh = playMaker.FsmVariables.FindFsmGameObject("Mesh").Value.transform;
				this.knobData[i] = data;
				this.knobRot[i] = fsmFloat;
				this.knobMesh[i] = mesh;
				Action<ulong> a = delegate(ulong target)
				{
					using (Packet packet = new Packet(1))
					{
						int num = 0;
						for (int k = 0; k < 4; k++)
						{
							if (this.knobMesh[k] == mesh)
							{
								num = k;
								break;
							}
						}
						packet.Write(num, -1);
						packet.Write(data.Value, -1);
						if (target == 0UL)
						{
							NetEvent<NetOvenManager>.Send("KnobTurn", packet, true);
						}
						else
						{
							NetEvent<NetOvenManager>.Send("KnobTurn", packet, target, true);
						}
					}
				};
				playMaker.InsertAction("State 1", delegate
				{
					a(0UL);
				}, -1);
				knobSyncs[i] = a;
			}
			PlayMakerFSM playMaker2 = GameObject.Find("YARD").transform.Find("Building/KITCHEN/OvenStove/Simulation").GetPlayMaker("Data");
			for (int j = 0; j < 4; j++)
			{
				this.hotplateTemps[j] = playMaker2.FsmVariables.FindFsmFloat(string.Format("HotPlate{0}Heat", j + 1));
			}
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				if (BeerMPGlobals.IsHost)
				{
					for (int l = 0; l < 4; l++)
					{
						knobSyncs[l](user);
					}
					this.SyncSim(user);
				}
			};
		}

		// Token: 0x06000359 RID: 857 RVA: 0x000195C5 File Offset: 0x000177C5
		private void Update()
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			this.stoveSimulationSyncTime += Time.deltaTime;
			if (this.stoveSimulationSyncTime >= 10f)
			{
				this.stoveSimulationSyncTime = 0f;
				this.SyncSim(0UL);
			}
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00019604 File Offset: 0x00017804
		private void SyncSim(ulong target = 0UL)
		{
			using (Packet packet = new Packet(1))
			{
				for (int i = 0; i < 4; i++)
				{
					packet.Write(this.hotplateTemps[i].Value, -1);
				}
				if (target == 0UL)
				{
					NetEvent<NetOvenManager>.Send("SimSync", packet, true);
				}
				else
				{
					NetEvent<NetOvenManager>.Send("SimSync", packet, target, true);
				}
			}
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00019674 File Offset: 0x00017874
		private void OnSimSync(ulong sender, Packet packet)
		{
			for (int i = 0; i < this.hotplateTemps.Length; i++)
			{
				this.hotplateTemps[i].Value = packet.ReadFloat(true);
			}
		}

		// Token: 0x0600035C RID: 860 RVA: 0x000196A8 File Offset: 0x000178A8
		private void OnKnobTurn(ulong sender, Packet packet)
		{
			int num = packet.ReadInt(true);
			float num2 = packet.ReadFloat(true);
			if (num > 4)
			{
				return;
			}
			this.knobData[num].Value = (this.knobRot[num].Value = num2);
			this.knobMesh[num].localEulerAngles = Vector3.up * num2;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00019700 File Offset: 0x00017900
		public NetOvenManager()
		{
		}

		// Token: 0x04000355 RID: 853
		private FsmFloat[] knobData;

		// Token: 0x04000356 RID: 854
		private FsmFloat[] knobRot;

		// Token: 0x04000357 RID: 855
		private FsmFloat[] hotplateTemps;

		// Token: 0x04000358 RID: 856
		private Transform[] knobMesh;

		// Token: 0x04000359 RID: 857
		private const string KnobTurnEvent = "KnobTurn";

		// Token: 0x0400035A RID: 858
		private const string SimSyncEvent = "SimSync";

		// Token: 0x0400035B RID: 859
		private float stoveSimulationSyncTime = 10f;

		// Token: 0x02000149 RID: 329
		[CompilerGenerated]
		private sealed class <>c__DisplayClass7_0
		{
			// Token: 0x06000616 RID: 1558 RVA: 0x00020BE3 File Offset: 0x0001EDE3
			public <>c__DisplayClass7_0()
			{
			}

			// Token: 0x06000617 RID: 1559 RVA: 0x00020BEC File Offset: 0x0001EDEC
			internal void <Start>b__0(ulong user)
			{
				if (BeerMPGlobals.IsHost)
				{
					for (int i = 0; i < 4; i++)
					{
						this.knobSyncs[i](user);
					}
					this.<>4__this.SyncSim(user);
				}
			}

			// Token: 0x040005AE RID: 1454
			public NetOvenManager <>4__this;

			// Token: 0x040005AF RID: 1455
			public Action<ulong>[] knobSyncs;
		}

		// Token: 0x0200014A RID: 330
		[CompilerGenerated]
		private sealed class <>c__DisplayClass7_1
		{
			// Token: 0x06000618 RID: 1560 RVA: 0x00020C26 File Offset: 0x0001EE26
			public <>c__DisplayClass7_1()
			{
			}

			// Token: 0x06000619 RID: 1561 RVA: 0x00020C30 File Offset: 0x0001EE30
			internal void <Start>b__1(ulong target)
			{
				using (Packet packet = new Packet(1))
				{
					int num = 0;
					for (int i = 0; i < 4; i++)
					{
						if (this.CS$<>8__locals1.<>4__this.knobMesh[i] == this.mesh)
						{
							num = i;
							break;
						}
					}
					packet.Write(num, -1);
					packet.Write(this.data.Value, -1);
					if (target == 0UL)
					{
						NetEvent<NetOvenManager>.Send("KnobTurn", packet, true);
					}
					else
					{
						NetEvent<NetOvenManager>.Send("KnobTurn", packet, target, true);
					}
				}
			}

			// Token: 0x0600061A RID: 1562 RVA: 0x00020CCC File Offset: 0x0001EECC
			internal void <Start>b__2()
			{
				this.a(0UL);
			}

			// Token: 0x040005B0 RID: 1456
			public Transform mesh;

			// Token: 0x040005B1 RID: 1457
			public FsmFloat data;

			// Token: 0x040005B2 RID: 1458
			public Action<ulong> a;

			// Token: 0x040005B3 RID: 1459
			public NetOvenManager.<>c__DisplayClass7_0 CS$<>8__locals1;
		}
	}
}
