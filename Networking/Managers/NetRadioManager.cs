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
	// Token: 0x0200005D RID: 93
	[ManagerCreate(10)]
	public class NetRadioManager : MonoBehaviour
	{
		// Token: 0x06000251 RID: 593 RVA: 0x0000EAA3 File Offset: 0x0000CCA3
		private void Start()
		{
			ObjectsLoader.gameLoaded += delegate
			{
				for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
				{
					GameObject gameObject = ObjectsLoader.ObjectsInGame[i];
					if (!(gameObject.name != "RadioChannels"))
					{
						Transform transform = GameObject.Find("RADIO").transform.Find("Paikallisradio");
						transform.gameObject.SetActive(false);
						this.InitChannel1(gameObject.transform, transform);
						this.InitFolk(gameObject.transform);
						NetRadioManager.radioLoaded = true;
						Console.Log("Init radio done", true);
						this.newTrackEvent = NetEvent<NetRadioManager>.Register("NewTrack", new NetEventHandler(this.OnNewTrackSelected));
						return;
					}
				}
			};
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				for (int j = 0; j < this.radios.Count; j++)
				{
					this.NewTrackSelected(j, true, new ulong?(user));
				}
				Console.Log("init sync radio", true);
			};
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000EADC File Offset: 0x0000CCDC
		private void InitFolk(Transform radio)
		{
			PlayMakerFSM component = radio.Find("Folk").GetComponent<PlayMakerFSM>();
			component.Initialize();
			PlayMakerFSM playMaker = (component.GetState("State 1").Actions[1] as SendEvent).eventTarget.gameObject.GameObject.Value.GetPlayMaker("Kansanradio");
			playMaker.Initialize();
			this.radios.Add(component);
			this.radioSources.Add(component.GetComponent<AudioSource>());
			FsmState state = playMaker.GetState("Play advert 1");
			FsmEvent fsmEvent = component.AddEvent("MP_NEXTRACK");
			this.radioPlayNextTrack.Add(fsmEvent);
			component.AddGlobalTransition(fsmEvent, "State 1");
			FsmInt trackID = new FsmInt("MP_NextTrack");
			List<FsmInt> list = playMaker.FsmVariables.IntVariables.ToList<FsmInt>();
			list.Add(trackID);
			this.radioNextTrackIndex.Add(trackID);
			playMaker.FsmVariables.IntVariables = list.ToArray();
			playMaker.FsmGlobalTransitions[0].ToState = state.Name;
			if (BeerMPGlobals.IsHost)
			{
				(state.Actions[0] as ArrayListGetRandom).randomIndex = trackID;
				playMaker.InsertAction(state.Name, delegate
				{
					this.NewTrackSelected(1, false, null);
				}, 2);
			}
			else
			{
				FsmStateAction[] actions = state.Actions;
				ArrayListGetRandom oldAction = actions[0] as ArrayListGetRandom;
				PlayMakerArrayListProxy arrayList = playMaker.GetComponents<PlayMakerArrayListProxy>().FirstOrDefault((PlayMakerArrayListProxy al) => al.referenceName == oldAction.reference.Value);
				FsmVar targetVar = oldAction.randomItem;
				actions[0] = new PlayMakerUtilities.PM_Hook(delegate
				{
					object obj = arrayList.arrayList[trackID.Value];
					targetVar.SetValue(obj);
				}, false);
				actions[1].Enabled = false;
				state.Actions = actions;
				component.InsertAction("Play radio", delegate
				{
					this.SetSRCtime(1);
				}, 1);
			}
			this.audioTimes.Add(0f);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000ECEC File Offset: 0x0000CEEC
		private void InitChannel1(Transform radio, Transform pakaliradioidk)
		{
			PlayMakerFSM component = radio.Find("Channel1").GetComponent<PlayMakerFSM>();
			component.Initialize();
			this.radios.Add(component);
			this.radioSources.Add(component.GetComponent<AudioSource>());
			FsmState state = component.GetState("Channel1");
			FsmEvent fsmEvent = component.AddEvent("MP_NEXTRACK");
			this.radioPlayNextTrack.Add(fsmEvent);
			component.AddGlobalTransition(fsmEvent, state.Name);
			component.Fsm.Event(fsmEvent);
			FsmInt trackID = new FsmInt("MP_NextTrack");
			List<FsmInt> list = component.FsmVariables.IntVariables.ToList<FsmInt>();
			list.Add(trackID);
			this.radioNextTrackIndex.Add(trackID);
			component.FsmVariables.IntVariables = list.ToArray();
			if (BeerMPGlobals.IsHost)
			{
				(state.Actions[0] as ArrayListGetRandom).randomIndex = trackID;
				component.InsertAction(state.Name, delegate
				{
					this.NewTrackSelected(0, false, null);
				}, 2);
			}
			else
			{
				FsmStateAction[] actions = state.Actions;
				ArrayListGetRandom oldAction = actions[0] as ArrayListGetRandom;
				PlayMakerArrayListProxy arrayList = pakaliradioidk.GetComponents<PlayMakerArrayListProxy>().FirstOrDefault((PlayMakerArrayListProxy al) => al.referenceName == oldAction.reference.Value);
				FsmVar targetVar = oldAction.randomItem;
				actions[0] = new PlayMakerUtilities.PM_Hook(delegate
				{
					object obj = arrayList.arrayList[trackID.Value];
					targetVar.SetValue(obj);
				}, false);
				actions[1].Enabled = false;
				state.Actions = actions;
				component.InsertAction("Play radio 2", delegate
				{
					this.SetSRCtime(0);
				}, 2);
			}
			this.audioTimes.Add(0f);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000EEB6 File Offset: 0x0000D0B6
		private void SetSRCtime(int index)
		{
			this.radioSources[index].time = this.audioTimes[index];
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000EED8 File Offset: 0x0000D0D8
		private void OnNewTrackSelected(ulong u, Packet p)
		{
			int num = p.ReadInt(true);
			int num2 = p.ReadInt(true);
			float num3 = p.ReadFloat(true);
			this.audioTimes[num] = ((num3 < 0f) ? 0f : num3);
			this.radioNextTrackIndex[num].Value = num2;
			this.radios[num].Fsm.Event(this.radioPlayNextTrack[num]);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000EF50 File Offset: 0x0000D150
		private void NewTrackSelected(int index, bool includeTime = false, ulong? target = null)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(index, -1);
				packet.Write(this.radioNextTrackIndex[index].Value, -1);
				packet.Write(includeTime ? this.radioSources[index].time : (-1f), -1);
				if (target == null)
				{
					this.newTrackEvent.Send(packet, true);
				}
				else
				{
					this.newTrackEvent.Send(packet, target.Value, true);
				}
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000EFF0 File Offset: 0x0000D1F0
		public NetRadioManager()
		{
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000F030 File Offset: 0x0000D230
		[CompilerGenerated]
		private void <Start>b__7_0()
		{
			for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
			{
				GameObject gameObject = ObjectsLoader.ObjectsInGame[i];
				if (!(gameObject.name != "RadioChannels"))
				{
					Transform transform = GameObject.Find("RADIO").transform.Find("Paikallisradio");
					transform.gameObject.SetActive(false);
					this.InitChannel1(gameObject.transform, transform);
					this.InitFolk(gameObject.transform);
					NetRadioManager.radioLoaded = true;
					Console.Log("Init radio done", true);
					this.newTrackEvent = NetEvent<NetRadioManager>.Register("NewTrack", new NetEventHandler(this.OnNewTrackSelected));
					return;
				}
			}
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000F0DC File Offset: 0x0000D2DC
		[CompilerGenerated]
		private void <Start>b__7_1(ulong user)
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			for (int i = 0; i < this.radios.Count; i++)
			{
				this.NewTrackSelected(i, true, new ulong?(user));
			}
			Console.Log("init sync radio", true);
		}

		// Token: 0x04000236 RID: 566
		private List<AudioSource> radioSources = new List<AudioSource>();

		// Token: 0x04000237 RID: 567
		private List<PlayMakerFSM> radios = new List<PlayMakerFSM>();

		// Token: 0x04000238 RID: 568
		private List<FsmEvent> radioPlayNextTrack = new List<FsmEvent>();

		// Token: 0x04000239 RID: 569
		private List<FsmInt> radioNextTrackIndex = new List<FsmInt>();

		// Token: 0x0400023A RID: 570
		private List<float> audioTimes = new List<float>();

		// Token: 0x0400023B RID: 571
		private NetEvent<NetRadioManager> newTrackEvent;

		// Token: 0x0400023C RID: 572
		internal static bool radioLoaded;

		// Token: 0x020000F9 RID: 249
		[CompilerGenerated]
		private sealed class <>c__DisplayClass8_0
		{
			// Token: 0x06000512 RID: 1298 RVA: 0x0001D3E5 File Offset: 0x0001B5E5
			public <>c__DisplayClass8_0()
			{
			}

			// Token: 0x06000513 RID: 1299 RVA: 0x0001D3F0 File Offset: 0x0001B5F0
			internal void <InitFolk>b__0()
			{
				this.<>4__this.NewTrackSelected(1, false, null);
			}

			// Token: 0x06000514 RID: 1300 RVA: 0x0001D413 File Offset: 0x0001B613
			internal void <InitFolk>b__3()
			{
				this.<>4__this.SetSRCtime(1);
			}

			// Token: 0x040004C2 RID: 1218
			public NetRadioManager <>4__this;

			// Token: 0x040004C3 RID: 1219
			public FsmInt trackID;
		}

		// Token: 0x020000FA RID: 250
		[CompilerGenerated]
		private sealed class <>c__DisplayClass8_1
		{
			// Token: 0x06000515 RID: 1301 RVA: 0x0001D421 File Offset: 0x0001B621
			public <>c__DisplayClass8_1()
			{
			}

			// Token: 0x06000516 RID: 1302 RVA: 0x0001D429 File Offset: 0x0001B629
			internal bool <InitFolk>b__1(PlayMakerArrayListProxy al)
			{
				return al.referenceName == this.oldAction.reference.Value;
			}

			// Token: 0x06000517 RID: 1303 RVA: 0x0001D448 File Offset: 0x0001B648
			internal void <InitFolk>b__2()
			{
				object obj = this.arrayList.arrayList[this.CS$<>8__locals1.trackID.Value];
				this.targetVar.SetValue(obj);
			}

			// Token: 0x040004C4 RID: 1220
			public ArrayListGetRandom oldAction;

			// Token: 0x040004C5 RID: 1221
			public PlayMakerArrayListProxy arrayList;

			// Token: 0x040004C6 RID: 1222
			public FsmVar targetVar;

			// Token: 0x040004C7 RID: 1223
			public NetRadioManager.<>c__DisplayClass8_0 CS$<>8__locals1;
		}

		// Token: 0x020000FB RID: 251
		[CompilerGenerated]
		private sealed class <>c__DisplayClass9_0
		{
			// Token: 0x06000518 RID: 1304 RVA: 0x0001D482 File Offset: 0x0001B682
			public <>c__DisplayClass9_0()
			{
			}

			// Token: 0x06000519 RID: 1305 RVA: 0x0001D48C File Offset: 0x0001B68C
			internal void <InitChannel1>b__0()
			{
				this.<>4__this.NewTrackSelected(0, false, null);
			}

			// Token: 0x0600051A RID: 1306 RVA: 0x0001D4AF File Offset: 0x0001B6AF
			internal void <InitChannel1>b__3()
			{
				this.<>4__this.SetSRCtime(0);
			}

			// Token: 0x040004C8 RID: 1224
			public NetRadioManager <>4__this;

			// Token: 0x040004C9 RID: 1225
			public FsmInt trackID;
		}

		// Token: 0x020000FC RID: 252
		[CompilerGenerated]
		private sealed class <>c__DisplayClass9_1
		{
			// Token: 0x0600051B RID: 1307 RVA: 0x0001D4BD File Offset: 0x0001B6BD
			public <>c__DisplayClass9_1()
			{
			}

			// Token: 0x0600051C RID: 1308 RVA: 0x0001D4C5 File Offset: 0x0001B6C5
			internal bool <InitChannel1>b__1(PlayMakerArrayListProxy al)
			{
				return al.referenceName == this.oldAction.reference.Value;
			}

			// Token: 0x0600051D RID: 1309 RVA: 0x0001D4E4 File Offset: 0x0001B6E4
			internal void <InitChannel1>b__2()
			{
				object obj = this.arrayList.arrayList[this.CS$<>8__locals1.trackID.Value];
				this.targetVar.SetValue(obj);
			}

			// Token: 0x040004CA RID: 1226
			public ArrayListGetRandom oldAction;

			// Token: 0x040004CB RID: 1227
			public PlayMakerArrayListProxy arrayList;

			// Token: 0x040004CC RID: 1228
			public FsmVar targetVar;

			// Token: 0x040004CD RID: 1229
			public NetRadioManager.<>c__DisplayClass9_0 CS$<>8__locals1;
		}
	}
}
