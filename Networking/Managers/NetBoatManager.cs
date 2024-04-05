using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.PlayerManagers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Steamworks;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000059 RID: 89
	[ManagerCreate(10)]
	internal class NetBoatManager : MonoBehaviour
	{
		// Token: 0x06000216 RID: 534 RVA: 0x0000C6FD File Offset: 0x0000A8FD
		private void Start()
		{
			NetBoatManager.instance = this;
			ObjectsLoader.gameLoaded += delegate
			{
				this.boat = GameObject.Find("BOAT").GetComponent<Rigidbody>();
				this.allRBS = this.boat.GetComponentsInChildren<Rigidbody>(true);
				PlayMakerFSM[] componentsInChildren = this.boat.GetComponentsInChildren<PlayMakerFSM>(true);
				this.RemoveDeath(componentsInChildren);
				this.DoItemCollider();
				PlayMakerFSM drivingMode = componentsInChildren.FirstOrDefault(delegate(PlayMakerFSM fsm)
				{
					if (fsm.FsmName != "PlayerTrigger" || fsm.gameObject.name != "DriveTrigger")
					{
						return false;
					}
					fsm.Initialize();
					FsmState state = fsm.GetState("Press return");
					if (state == null)
					{
						return false;
					}
					SetStringValue setStringValue = state.Actions.FirstOrDefault((FsmStateAction a) => a is SetStringValue) as SetStringValue;
					return setStringValue != null && setStringValue.stringValue.Value.Contains("DRIVING");
				});
				if (drivingMode != null)
				{
					drivingMode.Initialize();
					drivingMode.InsertAction("Press return", delegate
					{
						if (this.driver != 0UL)
						{
							drivingMode.SendEvent("FINISHED");
						}
					}, 0);
					drivingMode.InsertAction("Player in car", new Action(this.SendEnterDrivingMode), -1);
					drivingMode.InsertAction("Create player", new Action(this.SendExitDrivingMode), -1);
					Console.Log("Init driving mode for BOAT", false);
				}
				BoxCollider component = this.boat.transform.Find("GFX/Triggers/PlayerTrigger").GetComponent<BoxCollider>();
				component.center = Vector3.forward * -1.38f;
				component.size = new Vector3(0.8f, 0.4f, 3.7f);
				this.AddPassengerSeat(this.boat, this.boat.transform, new Vector3(0f, 0f, 0.1f), new Vector3(1f, 0f, 0f));
				this.AddPassengerSeat(this.boat, this.boat.transform, new Vector3(0f, 0f, -1.1f), new Vector3(1f, 0f, 0f));
				this.drivingModeEvent = NetEvent<NetBoatManager>.Register("DrivingMode", delegate(ulong s, Packet p)
				{
					bool flag = p.ReadBool(true);
					this.DrivingMode(s, flag);
				});
				this.passengerModeEvent = NetEvent<NetBoatManager>.Register("PassengerMode", delegate(ulong s, Packet p)
				{
					bool flag2 = p.ReadBool(true);
					NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)s).SetPassengerMode(flag2, this.boat.transform, false);
				});
			};
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000C720 File Offset: 0x0000A920
		private void DoItemCollider()
		{
			SphereCollider sphereCollider = new GameObject("ItemCollider")
			{
				transform = 
				{
					parent = this.boat.transform,
					localPosition = default(Vector3)
				}
			}.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = 3f;
			this.itemCollider = sphereCollider;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000C780 File Offset: 0x0000A980
		private void RemoveDeath(PlayMakerFSM[] fsms)
		{
			for (int i = 0; i < fsms.Length; i++)
			{
				if (fsms[i].FsmName == "Death" && fsms[i].gameObject.name == "DriverHeadPivot")
				{
					Transform transform = fsms[i].transform;
					transform.GetComponent<Rigidbody>().isKinematic = true;
					transform.transform.localPosition = Vector3.zero;
					transform.transform.localEulerAngles = Vector3.zero;
					global::UnityEngine.Object.Destroy(fsms[i]);
					global::UnityEngine.Object.Destroy(transform.parent.GetComponentInChildren<ConfigurableJoint>());
					Console.Log("Successfully removed death fsm from driving mode of " + base.transform.name, false);
					return;
				}
			}
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000C838 File Offset: 0x0000AA38
		public void SendEnterDrivingMode()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(true, -1);
				this.driver = (this.owner = BeerMPGlobals.UserID);
				LocalNetPlayer.Instance.inCar = true;
				this.drivingModeEvent.Send(packet, true);
				NetRigidbodyManager.RequestOwnership(this.boat);
				for (int i = 0; i < this.allRBS.Length; i++)
				{
					NetRigidbodyManager.RequestOwnership(this.allRBS[i]);
				}
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000C8C8 File Offset: 0x0000AAC8
		public void SendExitDrivingMode()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(false, -1);
				this.driver = 0UL;
				LocalNetPlayer.Instance.inCar = false;
				this.drivingModeEvent.Send(packet, true);
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000C920 File Offset: 0x0000AB20
		internal void DrivingMode(ulong player, bool enter)
		{
			this.driver = (enter ? player : 0UL);
			if (enter)
			{
				this.owner = player;
			}
			NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)player).SetPassengerMode(enter, this.boat.transform, false);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000C958 File Offset: 0x0000AB58
		private void AddPassengerSeat(Rigidbody rb, Transform parent, Vector3 triggerOffset, Vector3 headPivotOffset)
		{
			GameObject gameObject = GameObject.Find("NPC_CARS").transform.Find("Amikset/KYLAJANI/LOD/PlayerFunctions").gameObject;
			int num = 0;
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject);
			gameObject2.name = string.Format("MPPlayerFunctions_{0}", num);
			Transform transform = gameObject2.transform.Find("DriverHeadPivot");
			transform.GetComponent<Rigidbody>().isKinematic = true;
			transform.transform.localPosition = Vector3.zero;
			global::UnityEngine.Object.Destroy(transform.GetPlayMaker("Death"));
			Transform child = gameObject2.transform.GetChild(1);
			child.gameObject.SetActive(false);
			child.transform.localPosition = headPivotOffset;
			global::UnityEngine.Object.Destroy(child.GetComponent<ConfigurableJoint>());
			child.gameObject.SetActive(true);
			global::UnityEngine.Object.Destroy(gameObject2.transform.GetChild(0).gameObject);
			gameObject2.transform.SetParent(parent, false);
			gameObject2.transform.Find("PlayerTrigger/DriveTrigger").localPosition = triggerOffset;
			Transform transform2 = gameObject2.transform.Find("PlayerTrigger");
			transform2.localPosition = Vector3.zero;
			global::UnityEngine.Object.Destroy(transform2.GetComponent<PlayMakerFSM>());
			global::UnityEngine.Object.Destroy(transform2.GetComponent<BoxCollider>());
			PlayMakerFSM component = gameObject2.transform.Find("PlayerTrigger/DriveTrigger").GetComponent<PlayMakerFSM>();
			component.name = "PassengerTrigger";
			component.transform.parent.name = "PlayerOffset";
			component.Initialize();
			component.GetComponent<CapsuleCollider>().radius = 0.2f;
			component.InsertAction("Reset view", delegate
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(true, -1);
					LocalNetPlayer.Instance.inCar = true;
					this.passengerModeEvent.Send(packet, true);
				}
			}, -1);
			component.InsertAction("Create player", delegate
			{
				using (Packet packet2 = new Packet(1))
				{
					packet2.Write(false, -1);
					LocalNetPlayer.Instance.inCar = false;
					this.passengerModeEvent.Send(packet2, true);
				}
			}, -1);
			(component.FsmStates.First((FsmState s) => s.Name == "Check speed").Actions[0] as GetVelocity).gameObject = new FsmOwnerDefault
			{
				GameObject = rb.transform.gameObject,
				OwnerOption = OwnerDefaultOption.SpecifyGameObject
			};
			(component.FsmStates.First((FsmState s) => s.Name == "Player in car").Actions[3] as SetStringValue).stringValue = "Passenger_" + rb.transform.name;
			gameObject2.SetActive(true);
			gameObject2.transform.GetChild(1).gameObject.SetActive(true);
			component.gameObject.SetActive(true);
			Transform transform3 = gameObject2.transform;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000CBDD File Offset: 0x0000ADDD
		private void Update()
		{
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000CBDF File Offset: 0x0000ADDF
		public NetBoatManager()
		{
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000CBF4 File Offset: 0x0000ADF4
		[CompilerGenerated]
		private void <Start>b__9_0()
		{
			this.boat = GameObject.Find("BOAT").GetComponent<Rigidbody>();
			this.allRBS = this.boat.GetComponentsInChildren<Rigidbody>(true);
			PlayMakerFSM[] componentsInChildren = this.boat.GetComponentsInChildren<PlayMakerFSM>(true);
			this.RemoveDeath(componentsInChildren);
			this.DoItemCollider();
			PlayMakerFSM drivingMode = componentsInChildren.FirstOrDefault(delegate(PlayMakerFSM fsm)
			{
				if (fsm.FsmName != "PlayerTrigger" || fsm.gameObject.name != "DriveTrigger")
				{
					return false;
				}
				fsm.Initialize();
				FsmState state = fsm.GetState("Press return");
				if (state == null)
				{
					return false;
				}
				SetStringValue setStringValue = state.Actions.FirstOrDefault((FsmStateAction a) => a is SetStringValue) as SetStringValue;
				return setStringValue != null && setStringValue.stringValue.Value.Contains("DRIVING");
			});
			if (drivingMode != null)
			{
				drivingMode.Initialize();
				drivingMode.InsertAction("Press return", delegate
				{
					if (this.driver != 0UL)
					{
						drivingMode.SendEvent("FINISHED");
					}
				}, 0);
				drivingMode.InsertAction("Player in car", new Action(this.SendEnterDrivingMode), -1);
				drivingMode.InsertAction("Create player", new Action(this.SendExitDrivingMode), -1);
				Console.Log("Init driving mode for BOAT", false);
			}
			BoxCollider component = this.boat.transform.Find("GFX/Triggers/PlayerTrigger").GetComponent<BoxCollider>();
			component.center = Vector3.forward * -1.38f;
			component.size = new Vector3(0.8f, 0.4f, 3.7f);
			this.AddPassengerSeat(this.boat, this.boat.transform, new Vector3(0f, 0f, 0.1f), new Vector3(1f, 0f, 0f));
			this.AddPassengerSeat(this.boat, this.boat.transform, new Vector3(0f, 0f, -1.1f), new Vector3(1f, 0f, 0f));
			this.drivingModeEvent = NetEvent<NetBoatManager>.Register("DrivingMode", delegate(ulong s, Packet p)
			{
				bool flag = p.ReadBool(true);
				this.DrivingMode(s, flag);
			});
			this.passengerModeEvent = NetEvent<NetBoatManager>.Register("PassengerMode", delegate(ulong s, Packet p)
			{
				bool flag2 = p.ReadBool(true);
				NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)s).SetPassengerMode(flag2, this.boat.transform, false);
			});
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000CDF4 File Offset: 0x0000AFF4
		[CompilerGenerated]
		private void <Start>b__9_3(ulong s, Packet p)
		{
			bool flag = p.ReadBool(true);
			this.DrivingMode(s, flag);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000CE14 File Offset: 0x0000B014
		[CompilerGenerated]
		private void <Start>b__9_4(ulong s, Packet p)
		{
			bool flag = p.ReadBool(true);
			NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)s).SetPassengerMode(flag, this.boat.transform, false);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000CE48 File Offset: 0x0000B048
		[CompilerGenerated]
		private void <AddPassengerSeat>b__15_0()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(true, -1);
				LocalNetPlayer.Instance.inCar = true;
				this.passengerModeEvent.Send(packet, true);
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000CE98 File Offset: 0x0000B098
		[CompilerGenerated]
		private void <AddPassengerSeat>b__15_1()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(false, -1);
				LocalNetPlayer.Instance.inCar = false;
				this.passengerModeEvent.Send(packet, true);
			}
		}

		// Token: 0x040001F7 RID: 503
		internal static NetBoatManager instance;

		// Token: 0x040001F8 RID: 504
		internal SphereCollider itemCollider;

		// Token: 0x040001F9 RID: 505
		internal Rigidbody boat;

		// Token: 0x040001FA RID: 506
		private Rigidbody[] allRBS;

		// Token: 0x040001FB RID: 507
		private ConfigurableJoint[] passengerModes;

		// Token: 0x040001FC RID: 508
		private NetEvent<NetBoatManager> drivingModeEvent;

		// Token: 0x040001FD RID: 509
		private NetEvent<NetBoatManager> passengerModeEvent;

		// Token: 0x040001FE RID: 510
		public ulong driver;

		// Token: 0x040001FF RID: 511
		public ulong owner = BeerMPGlobals.HostID;

		// Token: 0x020000ED RID: 237
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004E2 RID: 1250 RVA: 0x0001C988 File Offset: 0x0001AB88
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004E3 RID: 1251 RVA: 0x0001C994 File Offset: 0x0001AB94
			public <>c()
			{
			}

			// Token: 0x060004E4 RID: 1252 RVA: 0x0001C99C File Offset: 0x0001AB9C
			internal bool <Start>b__9_1(PlayMakerFSM fsm)
			{
				if (fsm.FsmName != "PlayerTrigger" || fsm.gameObject.name != "DriveTrigger")
				{
					return false;
				}
				fsm.Initialize();
				FsmState state = fsm.GetState("Press return");
				if (state == null)
				{
					return false;
				}
				SetStringValue setStringValue = state.Actions.FirstOrDefault((FsmStateAction a) => a is SetStringValue) as SetStringValue;
				return setStringValue != null && setStringValue.stringValue.Value.Contains("DRIVING");
			}

			// Token: 0x060004E5 RID: 1253 RVA: 0x0001CA35 File Offset: 0x0001AC35
			internal bool <Start>b__9_5(FsmStateAction a)
			{
				return a is SetStringValue;
			}

			// Token: 0x060004E6 RID: 1254 RVA: 0x0001CA40 File Offset: 0x0001AC40
			internal bool <AddPassengerSeat>b__15_2(FsmState s)
			{
				return s.Name == "Check speed";
			}

			// Token: 0x060004E7 RID: 1255 RVA: 0x0001CA52 File Offset: 0x0001AC52
			internal bool <AddPassengerSeat>b__15_3(FsmState s)
			{
				return s.Name == "Player in car";
			}

			// Token: 0x0400048E RID: 1166
			public static readonly NetBoatManager.<>c <>9 = new NetBoatManager.<>c();

			// Token: 0x0400048F RID: 1167
			public static Func<FsmStateAction, bool> <>9__9_5;

			// Token: 0x04000490 RID: 1168
			public static Func<PlayMakerFSM, bool> <>9__9_1;

			// Token: 0x04000491 RID: 1169
			public static Func<FsmState, bool> <>9__15_2;

			// Token: 0x04000492 RID: 1170
			public static Func<FsmState, bool> <>9__15_3;
		}

		// Token: 0x020000EE RID: 238
		[CompilerGenerated]
		private sealed class <>c__DisplayClass9_0
		{
			// Token: 0x060004E8 RID: 1256 RVA: 0x0001CA64 File Offset: 0x0001AC64
			public <>c__DisplayClass9_0()
			{
			}

			// Token: 0x060004E9 RID: 1257 RVA: 0x0001CA6C File Offset: 0x0001AC6C
			internal void <Start>b__2()
			{
				if (this.<>4__this.driver != 0UL)
				{
					this.drivingMode.SendEvent("FINISHED");
				}
			}

			// Token: 0x04000493 RID: 1171
			public PlayMakerFSM drivingMode;

			// Token: 0x04000494 RID: 1172
			public NetBoatManager <>4__this;
		}
	}
}
