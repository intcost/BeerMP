using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.PlayerManagers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000078 RID: 120
	[ManagerCreate(10)]
	internal class PlayerGrabbingManager : MonoBehaviour
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000372 RID: 882 RVA: 0x0001A1D1 File Offset: 0x000183D1
		public static Rigidbody GrabbedRigidbody
		{
			get
			{
				return PlayerGrabbingManager.handItem_rb;
			}
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0001A1D8 File Offset: 0x000183D8
		private void Start()
		{
			base.StartCoroutine(this.init());
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0001A1E7 File Offset: 0x000183E7
		private IEnumerator init()
		{
			while (!PlayerGrabbingManager.itemPivot.Value)
			{
				yield return null;
			}
			PlayerGrabbingManager.handFSM = PlayerGrabbingManager.itemPivot.Value.transform.parent.GetChild(2).GetComponent<PlayMakerFSM>();
			PlayerGrabbingManager.handItem = PlayerGrabbingManager.handFSM.FsmVariables.FindFsmGameObject("RaycastHitObject");
			PlayerGrabbingManager.handFSM.InsertAction("State 1", new Action(this.OnItemGrabbed), -1);
			PlayerGrabbingManager.handFSM.InsertAction("Wait", new Action(this.OnItemDropped), -1);
			PlayerGrabbingManager.handFSM.InsertAction("Look for object", new Action(this.OnItemDropped), -1);
			yield break;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0001A1F8 File Offset: 0x000183F8
		private void OnItemGrabbed()
		{
			if (PlayerGrabbingManager.handItem.Value == null)
			{
				return;
			}
			PlayerGrabbingManager.handItem_rb = PlayerGrabbingManager.handItem.Value.GetComponent<Rigidbody>();
			if (PlayerGrabbingManager.handItem_rb == null)
			{
				return;
			}
			MPItem mpitem = PlayerGrabbingManager.handItem_rb.GetComponent<MPItem>();
			if (mpitem == null)
			{
				mpitem = PlayerGrabbingManager.handItem_rb.gameObject.AddComponent<MPItem>();
			}
			mpitem.doUpdate = true;
			int rigidbodyHash = NetRigidbodyManager.GetRigidbodyHash(PlayerGrabbingManager.handItem_rb);
			if (rigidbodyHash == 0)
			{
				Console.LogError("Rigidbody " + PlayerGrabbingManager.handItem_rb.name + " is not registered!", false);
			}
			else
			{
				Console.Log(string.Format("Grabbed hash {0}", rigidbodyHash), false);
			}
			NetRigidbodyManager.RequestOwnership(PlayerGrabbingManager.handItem_rb);
			this.SendGrabItemEvent(true, rigidbodyHash);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0001A2BC File Offset: 0x000184BC
		private void OnItemDropped()
		{
			this.SendGrabItemEvent(false, (PlayerGrabbingManager.handItem_rb == null) ? 0 : NetRigidbodyManager.GetRigidbodyHash(PlayerGrabbingManager.handItem_rb));
			MPItem component = PlayerGrabbingManager.handItem_rb.GetComponent<MPItem>();
			component.doUpdate = true;
			component.UpdateOwner();
			PlayerGrabbingManager.handItem_rb = null;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0001A2FC File Offset: 0x000184FC
		private void SendGrabItemEvent(bool grab, int hash)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(grab, -1);
				packet.Write(hash, -1);
				NetEvent<NetPlayer>.Send("GrabItem" + BeerMPGlobals.UserID.ToString(), packet, true);
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0001A35C File Offset: 0x0001855C
		public PlayerGrabbingManager()
		{
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0001A364 File Offset: 0x00018564
		// Note: this type is marked as 'beforefieldinit'.
		static PlayerGrabbingManager()
		{
		}

		// Token: 0x04000376 RID: 886
		private static FsmGameObject itemPivot = FsmVariables.GlobalVariables.FindFsmGameObject("ItemPivot");

		// Token: 0x04000377 RID: 887
		internal static PlayMakerFSM handFSM;

		// Token: 0x04000378 RID: 888
		private static FsmGameObject handItem;

		// Token: 0x04000379 RID: 889
		private static Rigidbody handItem_rb;

		// Token: 0x02000150 RID: 336
		[CompilerGenerated]
		private sealed class <init>d__7 : IEnumerator<object>, IDisposable, IEnumerator
		{
			// Token: 0x06000636 RID: 1590 RVA: 0x000211A8 File Offset: 0x0001F3A8
			[DebuggerHidden]
			public <init>d__7(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x06000637 RID: 1591 RVA: 0x000211B7 File Offset: 0x0001F3B7
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06000638 RID: 1592 RVA: 0x000211BC File Offset: 0x0001F3BC
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayerGrabbingManager playerGrabbingManager = this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
				}
				if (PlayerGrabbingManager.itemPivot.Value)
				{
					PlayerGrabbingManager.handFSM = PlayerGrabbingManager.itemPivot.Value.transform.parent.GetChild(2).GetComponent<PlayMakerFSM>();
					PlayerGrabbingManager.handItem = PlayerGrabbingManager.handFSM.FsmVariables.FindFsmGameObject("RaycastHitObject");
					PlayerGrabbingManager.handFSM.InsertAction("State 1", new Action(playerGrabbingManager.OnItemGrabbed), -1);
					PlayerGrabbingManager.handFSM.InsertAction("Wait", new Action(playerGrabbingManager.OnItemDropped), -1);
					PlayerGrabbingManager.handFSM.InsertAction("Look for object", new Action(playerGrabbingManager.OnItemDropped), -1);
					return false;
				}
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x17000040 RID: 64
			// (get) Token: 0x06000639 RID: 1593 RVA: 0x000212A3 File Offset: 0x0001F4A3
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600063A RID: 1594 RVA: 0x000212AB File Offset: 0x0001F4AB
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000041 RID: 65
			// (get) Token: 0x0600063B RID: 1595 RVA: 0x000212B2 File Offset: 0x0001F4B2
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x040005C0 RID: 1472
			private int <>1__state;

			// Token: 0x040005C1 RID: 1473
			private object <>2__current;

			// Token: 0x040005C2 RID: 1474
			public PlayerGrabbingManager <>4__this;
		}
	}
}
