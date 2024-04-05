using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000070 RID: 112
	[ManagerCreate(10)]
	internal class NetGameWorldManager : MonoBehaviour
	{
		// Token: 0x0600032B RID: 811 RVA: 0x000178AC File Offset: 0x00015AAC
		private void OnMemberReady(ulong userId)
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			this.SendTimeUpdate(userId);
			this.SendWeatherUpdate(userId);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x000178C4 File Offset: 0x00015AC4
		private void Start()
		{
			NetGameWorldManager.Instance = this;
			GameObject gameObject = GameObject.Find("MAP/SUN/Pivot/SUN");
			this.sunFSM = gameObject.GetPlayMaker("Color");
			this.day = FsmVariables.GlobalVariables.FindFsmInt("GlobalDay");
			this.time = this.sunFSM.FsmVariables.FindFsmInt("Time");
			this.minutes = this.sunFSM.FsmVariables.FindFsmFloat("Minutes");
			this.sunFSM.InsertAction("State 3", new NetGameWorldManager.TimeUpdateAction(), 0);
			PlayMakerFSM playMaker = GameObject.Find("YARD").transform.Find("Building/Dynamics/SuomiClock/Clock").GetPlayMaker("Time");
			playMaker.Initialize();
			FsmState state = playMaker.GetState("Set time");
			FsmStateAction[] actions = state.Actions;
			SetRotation setRotation = actions[1] as SetRotation;
			GameObject needle = setRotation.gameObject.GameObject.Value;
			actions[1] = new PlayMakerUtilities.PM_Hook(delegate
			{
				float num2 = this.minutes.Value % 60f;
				num2 = 60f - num2;
				num2 = num2 / 60f * 360f;
				needle.transform.localEulerAngles = Vector3.up * num2;
			}, false);
			state.Actions = actions;
			Action <>9__1;
			for (int i = 0; i < 12; i++)
			{
				FsmState fsmState = this.sunFSM.FsmStates[i];
				FsmStateAction[] actions2 = fsmState.Actions;
				for (int j = 0; j < actions2.Length; j++)
				{
					SetFloatValue setFloatValue = actions2[j] as SetFloatValue;
					if (setFloatValue != null && setFloatValue.floatVariable == this.minutes)
					{
						FsmStateAction[] array = actions2;
						int num = j;
						Action action;
						if ((action = <>9__1) == null)
						{
							action = (<>9__1 = delegate
							{
								if (!NetGameWorldManager.UpdatingMinutes)
								{
									this.minutes.Value = 0f;
								}
								NetGameWorldManager.UpdatingMinutes = false;
							});
						}
						array[num] = new PlayMakerUtilities.PM_Hook(action, false);
					}
				}
				fsmState.Actions = actions2;
			}
			GameObject gameObject2 = GameObject.Find("MAP/CloudSystem/Clouds");
			this.weatherFSM = gameObject2.GetPlayMaker("Weather");
			this.offset = this.weatherFSM.FsmVariables.FindFsmFloat("Offset");
			this.posX = this.weatherFSM.FsmVariables.FindFsmFloat("PosX");
			this.posZ = this.weatherFSM.FsmVariables.FindFsmFloat("PosZ");
			this.rotation = this.weatherFSM.FsmVariables.FindFsmFloat("Rotation");
			this.x = this.weatherFSM.FsmVariables.FindFsmFloat("X");
			this.z = this.weatherFSM.FsmVariables.FindFsmFloat("Z");
			this.weatherCloudID = this.weatherFSM.FsmVariables.FindFsmInt("WeatherCloudID");
			this.weatherType = this.weatherFSM.FsmVariables.FindFsmInt("WeatherType");
			this.rain = this.weatherFSM.FsmVariables.FindFsmBool("Rain");
			this.updateWeather = this.weatherFSM.AddEvent("MP_UpdateWeather");
			this.weatherFSM.AddGlobalTransition(this.updateWeather, "Set cloud");
			this.weatherFSM.InsertAction("Move clouds", new NetGameWorldManager.WeatherUpdateAction(), 0);
			BeerMPGlobals.OnMemberReady += new Action<ulong>(this.OnMemberReady);
			this.TimeChange = NetEvent<NetGameWorldManager>.Register("TimeChange", new NetEventHandler(this.OnTimeChange));
			this.WeatherChange = NetEvent<NetGameWorldManager>.Register("WeatherChange", new NetEventHandler(this.OnWeatherChange));
			if (!BeerMPGlobals.IsHost)
			{
				FsmStateAction[] actions3 = this.weatherFSM.GetState("Load game").Actions;
				for (int k = 0; k < actions3.Length; k++)
				{
					actions3[k].Enabled = false;
				}
				FsmStateAction[] actions4 = this.sunFSM.GetState("Load").Actions;
				for (int l = 0; l < actions4.Length; l++)
				{
					actions4[l].Enabled = false;
				}
			}
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00017C84 File Offset: 0x00015E84
		private void OnTimeChange(ulong userId, Packet packet)
		{
			int num = packet.ReadInt(true);
			int num2 = packet.ReadInt(true);
			float num3 = packet.ReadFloat(true);
			Console.Log(string.Format("NetGameTimeManager: received Time Update! new day is {0} new time is {1}", num, num2), true);
			this.day.Value = num;
			this.time.Value = num2;
			this.minutes.Value = num3;
			NetGameWorldManager.UpdatingMinutes = true;
			NetGameWorldManager.IsLocal = false;
			this.sunFSM.SendEvent("WAKEUP");
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00017D08 File Offset: 0x00015F08
		private void SendTimeUpdate()
		{
			using (Packet packet = new Packet())
			{
				packet.Write(this.day.Value, -1);
				packet.Write(this.time.Value, -1);
				packet.Write(this.minutes.Value, -1);
				NetEvent<NetGameWorldManager>.Send("TimeChange", packet, true);
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00017D7C File Offset: 0x00015F7C
		private void SendTimeUpdate(ulong userId)
		{
			using (Packet packet = new Packet())
			{
				packet.Write(this.day.Value, -1);
				packet.Write(this.time.Value, -1);
				packet.Write(this.minutes.Value, -1);
				NetEvent<NetGameWorldManager>.Send("TimeChange", packet, userId, true);
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00017DF0 File Offset: 0x00015FF0
		private void OnWeatherChange(ulong userId, Packet packet)
		{
			float num = packet.ReadFloat(true);
			float num2 = packet.ReadFloat(true);
			float num3 = packet.ReadFloat(true);
			float num4 = packet.ReadFloat(true);
			float num5 = packet.ReadFloat(true);
			float num6 = packet.ReadFloat(true);
			this.weatherCloudID.Value = packet.ReadInt(true);
			this.weatherType.Value = packet.ReadInt(true);
			this.weatherFSM.Fsm.Event(this.updateWeather);
			this.offset.Value = num;
			this.posX.Value = num2;
			this.posZ.Value = num3;
			this.rotation.Value = num4;
			this.weatherFSM.transform.eulerAngles = Vector3.up * num4;
			this.x.Value = num5;
			this.z.Value = num6;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00017ED0 File Offset: 0x000160D0
		private void SendWeatherUpdate()
		{
			using (Packet packet = new Packet())
			{
				packet.Write(this.offset.Value, -1);
				packet.Write(this.posX.Value, -1);
				packet.Write(this.posZ.Value, -1);
				packet.Write(this.rotation.Value, -1);
				packet.Write(this.x.Value, -1);
				packet.Write(this.z.Value, -1);
				packet.Write(this.weatherCloudID.Value, -1);
				packet.Write(this.weatherType.Value, -1);
				NetEvent<NetGameWorldManager>.Send("WeatherChange", packet, true);
			}
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00017F9C File Offset: 0x0001619C
		private void SendWeatherUpdate(ulong userId)
		{
			using (Packet packet = new Packet())
			{
				packet.Write(this.offset.Value, -1);
				packet.Write(this.posX.Value, -1);
				packet.Write(this.posZ.Value, -1);
				packet.Write(this.rotation.Value, -1);
				packet.Write(this.x.Value, -1);
				packet.Write(this.z.Value, -1);
				packet.Write(this.weatherCloudID.Value, -1);
				packet.Write(this.weatherType.Value, -1);
				NetEvent<NetGameWorldManager>.Send("WeatherChange", packet, userId, true);
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00018068 File Offset: 0x00016268
		public NetGameWorldManager()
		{
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00018070 File Offset: 0x00016270
		// Note: this type is marked as 'beforefieldinit'.
		static NetGameWorldManager()
		{
		}

		// Token: 0x04000327 RID: 807
		public static NetGameWorldManager Instance;

		// Token: 0x04000328 RID: 808
		private PlayMakerFSM sunFSM;

		// Token: 0x04000329 RID: 809
		private FsmInt day;

		// Token: 0x0400032A RID: 810
		private FsmInt time;

		// Token: 0x0400032B RID: 811
		private FsmFloat minutes;

		// Token: 0x0400032C RID: 812
		private PlayMakerFSM weatherFSM;

		// Token: 0x0400032D RID: 813
		private FsmFloat offset;

		// Token: 0x0400032E RID: 814
		private FsmFloat posX;

		// Token: 0x0400032F RID: 815
		private FsmFloat posZ;

		// Token: 0x04000330 RID: 816
		private FsmFloat rotation;

		// Token: 0x04000331 RID: 817
		private FsmFloat x;

		// Token: 0x04000332 RID: 818
		private FsmFloat z;

		// Token: 0x04000333 RID: 819
		private FsmInt weatherCloudID;

		// Token: 0x04000334 RID: 820
		private FsmInt weatherType;

		// Token: 0x04000335 RID: 821
		private FsmBool rain;

		// Token: 0x04000336 RID: 822
		private FsmEvent updateWeather;

		// Token: 0x04000337 RID: 823
		private static bool IsLocal = true;

		// Token: 0x04000338 RID: 824
		private static bool UpdatingMinutes = false;

		// Token: 0x04000339 RID: 825
		private NetEvent<NetGameWorldManager> TimeChange;

		// Token: 0x0400033A RID: 826
		private NetEvent<NetGameWorldManager> WeatherChange;

		// Token: 0x0200013F RID: 319
		public class TimeUpdateAction : FsmStateAction
		{
			// Token: 0x060005F0 RID: 1520 RVA: 0x000204A9 File Offset: 0x0001E6A9
			public override void OnEnter()
			{
				if (NetGameWorldManager.IsLocal)
				{
					NetGameWorldManager.Instance.SendTimeUpdate();
				}
				NetGameWorldManager.IsLocal = true;
				base.Finish();
			}

			// Token: 0x060005F1 RID: 1521 RVA: 0x000204C8 File Offset: 0x0001E6C8
			public TimeUpdateAction()
			{
			}
		}

		// Token: 0x02000140 RID: 320
		public class WeatherUpdateAction : FsmStateAction
		{
			// Token: 0x060005F2 RID: 1522 RVA: 0x000204D0 File Offset: 0x0001E6D0
			public override void OnEnter()
			{
				if (BeerMPGlobals.IsHost)
				{
					NetGameWorldManager.Instance.SendWeatherUpdate();
				}
				base.Finish();
			}

			// Token: 0x060005F3 RID: 1523 RVA: 0x000204E9 File Offset: 0x0001E6E9
			public WeatherUpdateAction()
			{
			}
		}

		// Token: 0x02000141 RID: 321
		[CompilerGenerated]
		private sealed class <>c__DisplayClass21_0
		{
			// Token: 0x060005F4 RID: 1524 RVA: 0x000204F1 File Offset: 0x0001E6F1
			public <>c__DisplayClass21_0()
			{
			}

			// Token: 0x060005F5 RID: 1525 RVA: 0x000204FC File Offset: 0x0001E6FC
			internal void <Start>b__0()
			{
				float num = this.<>4__this.minutes.Value % 60f;
				num = 60f - num;
				num = num / 60f * 360f;
				this.needle.transform.localEulerAngles = Vector3.up * num;
			}

			// Token: 0x060005F6 RID: 1526 RVA: 0x00020551 File Offset: 0x0001E751
			internal void <Start>b__1()
			{
				if (!NetGameWorldManager.UpdatingMinutes)
				{
					this.<>4__this.minutes.Value = 0f;
				}
				NetGameWorldManager.UpdatingMinutes = false;
			}

			// Token: 0x04000591 RID: 1425
			public NetGameWorldManager <>4__this;

			// Token: 0x04000592 RID: 1426
			public GameObject needle;

			// Token: 0x04000593 RID: 1427
			public Action <>9__1;
		}
	}
}
