using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200006F RID: 111
	[ManagerCreate(20000)]
	internal class NetDoorManager : MonoBehaviour
	{
		// Token: 0x06000327 RID: 807 RVA: 0x00017740 File Offset: 0x00015940
		private void Start()
		{
			NetDoorManager.Instance = this;
			GameObject[] array = Resources.FindObjectsOfTypeAll<GameObject>().Where((GameObject x) => x.name.StartsWith("Door")).ToArray<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				NetDoorManager.Door.Create(array[i]);
			}
			NetDoorManager.Door.Create(GameObject.Find("YARD").transform.Find("Building/KITCHEN/Fridge/Pivot/Handle").gameObject);
			NetEvent<NetDoorManager>.Register("ToggleDoor", new NetEventHandler(this.OnToggleDoor));
			BeerMPGlobals.OnMemberReady += delegate(ulong sender)
			{
				if (BeerMPGlobals.IsHost)
				{
					foreach (NetDoorManager.Door door in this.doors)
					{
						door.SyncStateInitial(sender);
					}
				}
			};
		}

		// Token: 0x06000328 RID: 808 RVA: 0x000177F0 File Offset: 0x000159F0
		private void OnToggleDoor(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			Vector3 doorPos = packet.ReadVector3(true);
			bool flag = packet.ReadBool(true);
			this.doors.First((NetDoorManager.Door x) => x.doorPos == doorPos).Toggle(flag);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0001783E File Offset: 0x00015A3E
		public NetDoorManager()
		{
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00017854 File Offset: 0x00015A54
		[CompilerGenerated]
		private void <Start>b__2_1(ulong sender)
		{
			if (BeerMPGlobals.IsHost)
			{
				foreach (NetDoorManager.Door door in this.doors)
				{
					door.SyncStateInitial(sender);
				}
			}
		}

		// Token: 0x04000325 RID: 805
		public static NetDoorManager Instance;

		// Token: 0x04000326 RID: 806
		public List<NetDoorManager.Door> doors = new List<NetDoorManager.Door>();

		// Token: 0x0200013B RID: 315
		public class Door
		{
			// Token: 0x060005E5 RID: 1509 RVA: 0x000201EC File Offset: 0x0001E3EC
			public static void Create(GameObject doorObject)
			{
				Transform transform = doorObject.transform.Find("Pivot/Handle");
				if (!transform)
				{
					transform = doorObject.transform.Find("Handle");
				}
				if (!transform)
				{
					transform = doorObject.transform;
				}
				if (transform.name != "Handle")
				{
					return;
				}
				Rigidbody[] componentsInChildren = doorObject.GetComponentsInChildren<Rigidbody>();
				if (componentsInChildren.Length != 0)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						global::UnityEngine.Object.Destroy(componentsInChildren[i]);
					}
				}
				PlayMakerFSM playMaker = transform.gameObject.GetPlayMaker("Use");
				if (playMaker == null)
				{
					return;
				}
				playMaker.Initialize();
				if (!playMaker.FsmEvents.Any((FsmEvent x) => x.Name == "OPENDOOR"))
				{
					return;
				}
				NetDoorManager.Door door = new NetDoorManager.Door();
				FsmEvent fsmEvent = playMaker.AddEvent("MP_TOGGLEDOOR");
				playMaker.AddGlobalTransition(fsmEvent, "Check position");
				playMaker.InsertAction("Check position", new NetDoorManager.DoorAction
				{
					door = door
				}, 0);
				NetDoorManager.Instance.doors.Add(door);
				door.doorPos = doorObject.transform.position;
				door.fsm = playMaker;
				door.fsmEvent = fsmEvent;
				door.doorOpen = playMaker.FsmVariables.GetFsmBool("DoorOpen");
			}

			// Token: 0x060005E6 RID: 1510 RVA: 0x0002033C File Offset: 0x0001E53C
			public void Toggle(bool open = false)
			{
				if (this.doorOpen.Value != open)
				{
					return;
				}
				this.doSync = false;
				this.fsm.Fsm.Event(this.fsmEvent);
			}

			// Token: 0x060005E7 RID: 1511 RVA: 0x0002036C File Offset: 0x0001E56C
			public void SyncStateInitial(ulong userId)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.doorPos, -1);
					packet.Write(!this.doorOpen.Value, -1);
					NetEvent<NetDoorManager>.Send("ToggleDoor", packet, userId, true);
				}
			}

			// Token: 0x060005E8 RID: 1512 RVA: 0x000203CC File Offset: 0x0001E5CC
			public Door()
			{
			}

			// Token: 0x04000588 RID: 1416
			public Vector3 doorPos;

			// Token: 0x04000589 RID: 1417
			public PlayMakerFSM fsm;

			// Token: 0x0400058A RID: 1418
			public FsmBool doorOpen;

			// Token: 0x0400058B RID: 1419
			public FsmEvent fsmEvent;

			// Token: 0x0400058C RID: 1420
			internal bool doSync = true;

			// Token: 0x0200021F RID: 543
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x06000963 RID: 2403 RVA: 0x00021748 File Offset: 0x0001F948
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x06000964 RID: 2404 RVA: 0x00021754 File Offset: 0x0001F954
				public <>c()
				{
				}

				// Token: 0x06000965 RID: 2405 RVA: 0x0002175C File Offset: 0x0001F95C
				internal bool <Create>b__0_0(FsmEvent x)
				{
					return x.Name == "OPENDOOR";
				}

				// Token: 0x040005DD RID: 1501
				public static readonly NetDoorManager.Door.<>c <>9 = new NetDoorManager.Door.<>c();

				// Token: 0x040005DE RID: 1502
				public static Func<FsmEvent, bool> <>9__0_0;
			}
		}

		// Token: 0x0200013C RID: 316
		public class DoorAction : FsmStateAction
		{
			// Token: 0x060005E9 RID: 1513 RVA: 0x000203DC File Offset: 0x0001E5DC
			public override void OnEnter()
			{
				if (this.door.doSync)
				{
					using (Packet packet = new Packet(1))
					{
						packet.Write(this.door.doorPos, -1);
						packet.Write(this.door.doorOpen.Value, -1);
						NetEvent<NetDoorManager>.Send("ToggleDoor", packet, true);
					}
				}
				this.door.doSync = true;
				base.Finish();
			}

			// Token: 0x060005EA RID: 1514 RVA: 0x00020460 File Offset: 0x0001E660
			public DoorAction()
			{
			}

			// Token: 0x0400058D RID: 1421
			public NetDoorManager.Door door;
		}

		// Token: 0x0200013D RID: 317
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060005EB RID: 1515 RVA: 0x00020468 File Offset: 0x0001E668
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060005EC RID: 1516 RVA: 0x00020474 File Offset: 0x0001E674
			public <>c()
			{
			}

			// Token: 0x060005ED RID: 1517 RVA: 0x0002047C File Offset: 0x0001E67C
			internal bool <Start>b__2_0(GameObject x)
			{
				return x.name.StartsWith("Door");
			}

			// Token: 0x0400058E RID: 1422
			public static readonly NetDoorManager.<>c <>9 = new NetDoorManager.<>c();

			// Token: 0x0400058F RID: 1423
			public static Func<GameObject, bool> <>9__2_0;
		}

		// Token: 0x0200013E RID: 318
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x060005EE RID: 1518 RVA: 0x0002048E File Offset: 0x0001E68E
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x060005EF RID: 1519 RVA: 0x00020496 File Offset: 0x0001E696
			internal bool <OnToggleDoor>b__0(NetDoorManager.Door x)
			{
				return x.doorPos == this.doorPos;
			}

			// Token: 0x04000590 RID: 1424
			public Vector3 doorPos;
		}
	}
}
