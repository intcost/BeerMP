using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200005E RID: 94
	[ManagerCreate(10)]
	internal class NetStoreManager : MonoBehaviour
	{
		// Token: 0x0600025A RID: 602 RVA: 0x0000F120 File Offset: 0x0000D320
		private void Start()
		{
			this.store = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault((GameObject x) => x.name == "STORE");
			this.store.GetPlayMaker("LOD").enabled = false;
			this.syncInventory = NetEvent<NetStoreManager>.Register("SyncStoreInventory", new NetEventHandler(this.OnSyncInventory));
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				this.SyncInventory(user);
				for (int j = 0; j < this.products.Count; j++)
				{
					this.products[j].SyncInitial(user);
				}
				this.register.SyncInitial(user);
			};
			Transform transform = this.store.transform.Find("LOD");
			transform.gameObject.SetActive(true);
			PlayMakerFSM[] array = transform.Find("ActivateStore").GetComponentsInChildren<PlayMakerFSM>(true).Where((PlayMakerFSM x) => x.FsmName == "Buy")
				.ToArray<PlayMakerFSM>();
			for (int i = 0; i < array.Length; i++)
			{
				this.products.Add(new NetStoreManager.StoreProduct(array[i]));
			}
			Transform transform2 = this.store.transform.Find("Inventory");
			this.boughtInventory = transform2.GetComponents<PlayMakerHashTableProxy>().FirstOrDefault((PlayMakerHashTableProxy x) => x.referenceName == "Bought");
			Transform transform3 = this.store.transform.Find("StoreCashRegister/Register");
			this.register = new NetStoreManager.CashRegister(transform3.GetPlayMaker("Data"));
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000F298 File Offset: 0x0000D498
		private void SyncInventory(ulong target)
		{
			using (Packet packet = new Packet())
			{
				object[] array = new object[this.boughtInventory._hashTable.Count];
				this.boughtInventory._hashTable.Keys.CopyTo(array, 0);
				int i;
				for (i = 0; i < this.boughtInventory._hashTable.Count; i++)
				{
					packet.Write(array[i].ToString(), -1);
					packet.Write((int)this.boughtInventory._hashTable[array[i]], -1);
				}
				packet.Write(i, 0);
				this.syncInventory.Send(packet, target, true);
			}
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000F358 File Offset: 0x0000D558
		private void OnSyncInventory(ulong sender, Packet packet)
		{
			int num = packet.ReadInt(true);
			for (int i = 0; i < num; i++)
			{
				string text = packet.ReadString(true);
				int num2 = packet.ReadInt(true);
				this.boughtInventory._hashTable[text] = num2;
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000F3A0 File Offset: 0x0000D5A0
		public NetStoreManager()
		{
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000F3B4 File Offset: 0x0000D5B4
		[CompilerGenerated]
		private void <Start>b__7_1(ulong user)
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			this.SyncInventory(user);
			for (int i = 0; i < this.products.Count; i++)
			{
				this.products[i].SyncInitial(user);
			}
			this.register.SyncInitial(user);
		}

		// Token: 0x0400023D RID: 573
		public GameObject store;

		// Token: 0x0400023E RID: 574
		public List<NetStoreManager.StoreProduct> products = new List<NetStoreManager.StoreProduct>();

		// Token: 0x0400023F RID: 575
		public NetStoreManager.CashRegister register;

		// Token: 0x04000240 RID: 576
		public PlayMakerHashTableProxy boughtInventory;

		// Token: 0x04000241 RID: 577
		public NetEvent<NetStoreManager> syncInventory;

		// Token: 0x020000FD RID: 253
		internal class StoreProduct
		{
			// Token: 0x0600051E RID: 1310 RVA: 0x0001D520 File Offset: 0x0001B720
			public StoreProduct(PlayMakerFSM fsm)
			{
				this.fsm = fsm;
				fsm.Initialize();
				this.Quantity = fsm.FsmVariables.FindFsmInt("Quantity");
				this.Bought = fsm.FsmVariables.FindFsmInt("Bought");
				this.Name = fsm.name;
				this.Purchase = fsm.AddEvent("MP_PURCHASE");
				if (fsm.HasState("Check inventory"))
				{
					fsm.AddGlobalTransition(this.Purchase, "Check inventory");
					fsm.InsertAction("Check inventory", delegate
					{
						using (Packet packet = new Packet())
						{
							if (this.doSync)
							{
								this.purchase.Send(packet, true);
							}
							this.doSync = true;
						}
					}, 2);
				}
				else
				{
					fsm.AddGlobalTransition(this.Purchase, "Play anim");
					fsm.InsertAction("Play anim", delegate
					{
						using (Packet packet2 = new Packet(1))
						{
							if (this.doSync)
							{
								this.purchase.Send(packet2, true);
							}
							this.doSync = true;
						}
					}, 2);
				}
				if (fsm.HasState("Check if 0"))
				{
					this.Depurchase = fsm.AddEvent("MP_DEPURCHASE");
					fsm.AddGlobalTransition(this.Depurchase, "Check if 0");
					fsm.InsertAction("Check if 0", delegate
					{
						using (Packet packet3 = new Packet(1))
						{
							if (this.doSync)
							{
								this.depurchase.Send(packet3, true);
							}
							this.doSync = true;
						}
					}, 2);
				}
				this.purchase = NetEvent<NetStoreManager.StoreProduct>.Register("Purchase" + this.Name, new NetEventHandler(this.OnPurchase));
				this.depurchase = NetEvent<NetStoreManager.StoreProduct>.Register("Depurchase" + this.Name, new NetEventHandler(this.OnDepurchase));
				this.syncInitial = NetEvent<NetStoreManager.StoreProduct>.Register("SyncInitial" + this.Name, new NetEventHandler(this.OnSyncInitial));
			}

			// Token: 0x0600051F RID: 1311 RVA: 0x0001D6AD File Offset: 0x0001B8AD
			private void OnSyncInitial(ulong sender, Packet packet)
			{
				this.Quantity.Value = packet.ReadInt(true);
				this.Bought.Value = packet.ReadInt(true);
			}

			// Token: 0x06000520 RID: 1312 RVA: 0x0001D6D3 File Offset: 0x0001B8D3
			private void OnPurchase(ulong sender, Packet packet)
			{
				if (sender == BeerMPGlobals.UserID)
				{
					return;
				}
				this.doSync = false;
				this.fsm.SendEvent(this.Purchase.Name);
			}

			// Token: 0x06000521 RID: 1313 RVA: 0x0001D6FB File Offset: 0x0001B8FB
			private void OnDepurchase(ulong sender, Packet packet)
			{
				if (sender == BeerMPGlobals.UserID)
				{
					return;
				}
				this.doSync = false;
				this.fsm.SendEvent(this.Depurchase.Name);
			}

			// Token: 0x06000522 RID: 1314 RVA: 0x0001D724 File Offset: 0x0001B924
			public void SyncInitial(ulong target)
			{
				using (Packet packet = new Packet(1))
				{
					if (this.Quantity == null || this.Bought == null)
					{
						if (this.Quantity == null)
						{
							Console.LogWarning("Quanitity is null on StoreProduct '" + this.Name + "'", true);
						}
						if (this.Bought == null)
						{
							Console.LogWarning("Bought is null on StoreProduct '" + this.Name + "'", true);
						}
					}
					else
					{
						packet.Write(this.Quantity.Value, -1);
						packet.Write(this.Bought.Value, -1);
						this.syncInitial.Send(packet, target, true);
					}
				}
			}

			// Token: 0x06000523 RID: 1315 RVA: 0x0001D7E0 File Offset: 0x0001B9E0
			[CompilerGenerated]
			private void <.ctor>b__10_0()
			{
				using (Packet packet = new Packet())
				{
					if (this.doSync)
					{
						this.purchase.Send(packet, true);
					}
					this.doSync = true;
				}
			}

			// Token: 0x06000524 RID: 1316 RVA: 0x0001D82C File Offset: 0x0001BA2C
			[CompilerGenerated]
			private void <.ctor>b__10_1()
			{
				using (Packet packet = new Packet(1))
				{
					if (this.doSync)
					{
						this.purchase.Send(packet, true);
					}
					this.doSync = true;
				}
			}

			// Token: 0x06000525 RID: 1317 RVA: 0x0001D878 File Offset: 0x0001BA78
			[CompilerGenerated]
			private void <.ctor>b__10_2()
			{
				using (Packet packet = new Packet(1))
				{
					if (this.doSync)
					{
						this.depurchase.Send(packet, true);
					}
					this.doSync = true;
				}
			}

			// Token: 0x040004CE RID: 1230
			public PlayMakerFSM fsm;

			// Token: 0x040004CF RID: 1231
			public FsmInt Quantity;

			// Token: 0x040004D0 RID: 1232
			public FsmInt Bought;

			// Token: 0x040004D1 RID: 1233
			public string Name;

			// Token: 0x040004D2 RID: 1234
			public FsmEvent Purchase;

			// Token: 0x040004D3 RID: 1235
			public FsmEvent Depurchase;

			// Token: 0x040004D4 RID: 1236
			public NetEvent<NetStoreManager.StoreProduct> purchase;

			// Token: 0x040004D5 RID: 1237
			public NetEvent<NetStoreManager.StoreProduct> depurchase;

			// Token: 0x040004D6 RID: 1238
			public NetEvent<NetStoreManager.StoreProduct> syncInitial;

			// Token: 0x040004D7 RID: 1239
			public bool doSync = true;
		}

		// Token: 0x020000FE RID: 254
		internal class CashRegister
		{
			// Token: 0x06000526 RID: 1318 RVA: 0x0001D8C4 File Offset: 0x0001BAC4
			public CashRegister(PlayMakerFSM fsm)
			{
				this.fsm = fsm;
				fsm.Initialize();
				this.interact = fsm.AddEvent("MP_INTERACT");
				fsm.AddGlobalTransition(this.interact, "Check money");
				fsm.InsertAction("Check money", delegate
				{
					using (Packet packet = new Packet())
					{
						if (this.doSync)
						{
							this.useRegister.Send(packet, true);
						}
						this.doSync = true;
					}
				}, 0);
				this.intVars = fsm.FsmVariables.IntVariables;
				this.useRegister = NetEvent<NetStoreManager.CashRegister>.Register("UseStoreRegister", new NetEventHandler(this.OnUseRegister));
				this.syncInitial = NetEvent<NetStoreManager.CashRegister>.Register("SyncStoreRegisterInitial", new NetEventHandler(this.OnSyncInitial));
			}

			// Token: 0x06000527 RID: 1319 RVA: 0x0001D96E File Offset: 0x0001BB6E
			private void OnUseRegister(ulong sender, Packet packet)
			{
				if (sender == BeerMPGlobals.UserID)
				{
					return;
				}
				this.doSync = false;
				this.fsm.SendEvent(this.interact.Name);
			}

			// Token: 0x06000528 RID: 1320 RVA: 0x0001D998 File Offset: 0x0001BB98
			public void SyncInitial(ulong target)
			{
				using (Packet packet = new Packet())
				{
					int num = 0;
					for (int i = 0; i < this.intVars.Length; i++)
					{
						packet.Write(this.intVars[i].Name, -1);
						packet.Write(this.intVars[i].Value, -1);
						num++;
					}
					packet.Write(num, 0);
					this.syncInitial.Send(packet, target, true);
				}
			}

			// Token: 0x06000529 RID: 1321 RVA: 0x0001DA20 File Offset: 0x0001BC20
			private void OnSyncInitial(ulong sender, Packet packet)
			{
				int num = packet.ReadInt(true);
				for (int i = 0; i < num; i++)
				{
					string name = packet.ReadString(true);
					int num2 = packet.ReadInt(true);
					if (this.intVars.Any((FsmInt x) => x.Name == name))
					{
						this.intVars.FirstOrDefault((FsmInt x) => x.Name == name).Value = num2;
					}
				}
			}

			// Token: 0x0600052A RID: 1322 RVA: 0x0001DA94 File Offset: 0x0001BC94
			[CompilerGenerated]
			private void <.ctor>b__6_0()
			{
				using (Packet packet = new Packet())
				{
					if (this.doSync)
					{
						this.useRegister.Send(packet, true);
					}
					this.doSync = true;
				}
			}

			// Token: 0x040004D8 RID: 1240
			public PlayMakerFSM fsm;

			// Token: 0x040004D9 RID: 1241
			public FsmEvent interact;

			// Token: 0x040004DA RID: 1242
			public NetEvent<NetStoreManager.CashRegister> useRegister;

			// Token: 0x040004DB RID: 1243
			public NetEvent<NetStoreManager.CashRegister> syncInitial;

			// Token: 0x040004DC RID: 1244
			public bool doSync = true;

			// Token: 0x040004DD RID: 1245
			public FsmInt[] intVars;

			// Token: 0x0200021B RID: 539
			[CompilerGenerated]
			private sealed class <>c__DisplayClass9_0
			{
				// Token: 0x06000953 RID: 2387 RVA: 0x000214EF File Offset: 0x0001F6EF
				public <>c__DisplayClass9_0()
				{
				}

				// Token: 0x06000954 RID: 2388 RVA: 0x000214F7 File Offset: 0x0001F6F7
				internal bool <OnSyncInitial>b__0(FsmInt x)
				{
					return x.Name == this.name;
				}

				// Token: 0x06000955 RID: 2389 RVA: 0x0002150A File Offset: 0x0001F70A
				internal bool <OnSyncInitial>b__1(FsmInt x)
				{
					return x.Name == this.name;
				}

				// Token: 0x040005D5 RID: 1493
				public string name;
			}
		}

		// Token: 0x020000FF RID: 255
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600052B RID: 1323 RVA: 0x0001DAE0 File Offset: 0x0001BCE0
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600052C RID: 1324 RVA: 0x0001DAEC File Offset: 0x0001BCEC
			public <>c()
			{
			}

			// Token: 0x0600052D RID: 1325 RVA: 0x0001DAF4 File Offset: 0x0001BCF4
			internal bool <Start>b__7_0(GameObject x)
			{
				return x.name == "STORE";
			}

			// Token: 0x0600052E RID: 1326 RVA: 0x0001DB06 File Offset: 0x0001BD06
			internal bool <Start>b__7_2(PlayMakerFSM x)
			{
				return x.FsmName == "Buy";
			}

			// Token: 0x0600052F RID: 1327 RVA: 0x0001DB18 File Offset: 0x0001BD18
			internal bool <Start>b__7_3(PlayMakerHashTableProxy x)
			{
				return x.referenceName == "Bought";
			}

			// Token: 0x040004DE RID: 1246
			public static readonly NetStoreManager.<>c <>9 = new NetStoreManager.<>c();

			// Token: 0x040004DF RID: 1247
			public static Func<GameObject, bool> <>9__7_0;

			// Token: 0x040004E0 RID: 1248
			public static Func<PlayMakerFSM, bool> <>9__7_2;

			// Token: 0x040004E1 RID: 1249
			public static Func<PlayMakerHashTableProxy, bool> <>9__7_3;
		}
	}
}
