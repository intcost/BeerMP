using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000060 RID: 96
	[ManagerCreate(10)]
	internal class NetCreateItemsManager : MonoBehaviour
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600027C RID: 636 RVA: 0x00010038 File Offset: 0x0000E238
		public static PlayMakerFSM[] Beercases
		{
			get
			{
				object[] array = NetCreateItemsManager.beerDB.arrayList.ToArray();
				PlayMakerFSM[] array2 = new PlayMakerFSM[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = (array[i] as GameObject).GetPlayMaker("Use");
				}
				return array2;
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00010084 File Offset: 0x0000E284
		private void Start()
		{
			GameObject gameObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault((GameObject x) => x.name == "Spawner" && x.transform.root == x.transform);
			this.createItems = gameObject.transform.Find("CreateItems").gameObject;
			PlayMakerFSM[] array = this.createItems.GetComponents<PlayMakerFSM>();
			for (int i = 0; i < array.Length; i++)
			{
				NetCreateItemsManager.creators.Add(new NetCreateItemsManager.Creator(array[i]));
			}
			this.createSpraycans = gameObject.transform.Find("CreateSpraycans").gameObject;
			array = this.createSpraycans.GetComponents<PlayMakerFSM>();
			for (int j = 0; j < array.Length; j++)
			{
				NetCreateItemsManager.creators.Add(new NetCreateItemsManager.Creator(array[j]));
			}
			this.createMooseMeat = gameObject.transform.Find("CreateMooseMeat").gameObject;
			array = this.createMooseMeat.GetComponents<PlayMakerFSM>();
			for (int k = 0; k < array.Length; k++)
			{
				NetCreateItemsManager.creators.Add(new NetCreateItemsManager.Creator(array[k]));
			}
			this.createShoppingbag = gameObject.transform.Find("CreateShoppingbag").gameObject;
			array = this.createShoppingbag.GetComponents<PlayMakerFSM>();
			for (int l = 0; l < array.Length; l++)
			{
				NetCreateItemsManager.creators.Add(new NetCreateItemsManager.Creator(array[l]));
			}
			NetCreateItemsManager.beerDB = gameObject.transform.Find("BeerDB").GetComponent<PlayMakerArrayListProxy>();
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				for (int m = 0; m < NetCreateItemsManager.creators.Count; m++)
				{
					NetCreateItemsManager.creators[m].SyncInitial(user);
				}
			};
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00010229 File Offset: 0x0000E429
		public NetCreateItemsManager()
		{
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00010231 File Offset: 0x0000E431
		// Note: this type is marked as 'beforefieldinit'.
		static NetCreateItemsManager()
		{
		}

		// Token: 0x04000267 RID: 615
		public GameObject createItems;

		// Token: 0x04000268 RID: 616
		public GameObject createSpraycans;

		// Token: 0x04000269 RID: 617
		public GameObject createMooseMeat;

		// Token: 0x0400026A RID: 618
		public GameObject createShoppingbag;

		// Token: 0x0400026B RID: 619
		public static List<NetCreateItemsManager.Creator> creators = new List<NetCreateItemsManager.Creator>();

		// Token: 0x0400026C RID: 620
		private static PlayMakerArrayListProxy beerDB;

		// Token: 0x02000106 RID: 262
		public class Item
		{
			// Token: 0x0600053E RID: 1342 RVA: 0x0001DC54 File Offset: 0x0001BE54
			public Item(PlayMakerFSM fsm, NetCreateItemsManager.Creator creator, string id)
			{
				this.fsm = fsm;
				this.ID = id;
				this.hash = this.ID.GetHashCode();
				this.rb = fsm.gameObject.GetComponent<Rigidbody>();
				this.orb = NetRigidbodyManager.AddRigidbody(this.rb, this.hash);
				this.creator = creator;
				this.Owner = fsm.FsmVariables.FindFsmGameObject("Owner");
				this.CheckPart();
				this.CheckShoppingbag();
				this.CheckBeercase();
			}

			// Token: 0x0600053F RID: 1343 RVA: 0x0001DCE4 File Offset: 0x0001BEE4
			private void CheckBeercase()
			{
				string text = this.rb.gameObject.name.ToLower();
				if (text.Contains("beer") && text.Contains("case"))
				{
					NetItemsManager.SetupBeercaseFSM(this.fsm, this);
				}
			}

			// Token: 0x06000540 RID: 1344 RVA: 0x0001DD30 File Offset: 0x0001BF30
			private void CheckPart()
			{
				PlayMakerFSM playMaker = this.fsm.gameObject.GetPlayMaker("Removal");
				if (playMaker != null)
				{
					NetPartManager.SetupRemovalPlaymaker(playMaker, this.hash);
					this.orb.Removal_Rigidbody = playMaker.FsmVariables.FindFsmObject("Rigidbody");
					this.orb.remove = playMaker;
				}
				PlayMakerFSM playMaker2 = this.fsm.gameObject.GetPlayMaker("Screw");
				if (playMaker2 != null && !NetPartManager.AddBolt(playMaker2, this.hash))
				{
					Console.LogError(string.Format("Bolt of hash {0} ({1}) doesn't have stage variable", this.hash, this.ID), false);
				}
			}

			// Token: 0x06000541 RID: 1345 RVA: 0x0001DDE0 File Offset: 0x0001BFE0
			private void CheckShoppingbag()
			{
				FsmGameObject fsmGameObject = this.fsm.FsmVariables.FindFsmGameObject("ProductSpawner");
				if (fsmGameObject == null)
				{
					return;
				}
				this.ProductSpawner = fsmGameObject.Value.GetPlayMaker("Logic");
				FsmGameObject currentBag = this.ProductSpawner.FsmVariables.FindFsmGameObject("CurrentBag");
				PlayMakerArrayListProxy[] components = this.fsm.gameObject.GetComponents<PlayMakerArrayListProxy>();
				this.Items = components.FirstOrDefault((PlayMakerArrayListProxy x) => x.referenceName == "Items");
				this.Spraycans = components.FirstOrDefault((PlayMakerArrayListProxy x) => x.referenceName == "Spraycans");
				this.fsm.InsertAction("Spawn one", delegate
				{
					if (this.doSync)
					{
						NetCreateItemsManager.Creator.doSync = true;
						using (Packet packet = new Packet())
						{
							int num = 0;
							int num2 = 0;
							for (int i = 0; i < this.Items._arrayList.Count; i++)
							{
								packet.Write((int)this.Items._arrayList[i], -1);
								num++;
							}
							for (int j = 0; j < this.Spraycans._arrayList.Count; j++)
							{
								packet.Write((int)this.Spraycans._arrayList[j], -1);
								num2++;
							}
							packet.Write(num2, 0);
							packet.Write(num, 0);
							this.ShoppingSpawnOne.Send(packet, true);
						}
					}
					this.doSync = true;
				}, 0);
				this.spawnAll = this.fsm.AddEvent("MP_SPAWNALL");
				this.fsm.AddGlobalTransition(this.spawnAll, "Spawn all");
				this.fsm.InsertAction("Spawn all", delegate
				{
					currentBag.Value = this.Owner.Value;
					if (this.doSync)
					{
						using (Packet packet2 = new Packet())
						{
							this.ShoppingSpawnAll.Send(packet2, true);
						}
					}
					this.doSync = true;
				}, 0);
				this.ShoppingSpawnOne = NetEvent<NetCreateItemsManager.Item>.Register(this.ID + "SpawnOne", new NetEventHandler(this.OnShoppingSpawnOne));
				this.ShoppingSpawnAll = NetEvent<NetCreateItemsManager.Item>.Register(this.ID + "SpawnAll", new NetEventHandler(this.OnShoppingSpawnAll));
			}

			// Token: 0x06000542 RID: 1346 RVA: 0x0001DF60 File Offset: 0x0001C160
			private void OnShoppingSpawnOne(ulong sender, Packet packet)
			{
				if (sender == BeerMPGlobals.UserID)
				{
					return;
				}
				int num = packet.ReadInt(true);
				int num2 = packet.ReadInt(true);
				for (int i = 0; i < num; i++)
				{
					this.Items._arrayList[i] = packet.ReadInt(true);
				}
				for (int j = 0; j < num2; j++)
				{
					this.Spraycans._arrayList[j] = packet.ReadInt(true);
				}
			}

			// Token: 0x06000543 RID: 1347 RVA: 0x0001DFD8 File Offset: 0x0001C1D8
			private void OnShoppingSpawnAll(ulong sender, Packet packet)
			{
				if (sender == BeerMPGlobals.UserID)
				{
					return;
				}
				this.doSync = false;
				this.fsm.SendEvent(this.spawnAll.Name);
			}

			// Token: 0x040004EA RID: 1258
			public NetRigidbodyManager.OwnedRigidbody orb;

			// Token: 0x040004EB RID: 1259
			public PlayMakerFSM fsm;

			// Token: 0x040004EC RID: 1260
			public NetCreateItemsManager.Creator creator;

			// Token: 0x040004ED RID: 1261
			public int hash;

			// Token: 0x040004EE RID: 1262
			public Rigidbody rb;

			// Token: 0x040004EF RID: 1263
			public string ID;

			// Token: 0x040004F0 RID: 1264
			public FsmGameObject Owner;

			// Token: 0x040004F1 RID: 1265
			public PlayMakerFSM ProductSpawner;

			// Token: 0x040004F2 RID: 1266
			public PlayMakerArrayListProxy Items;

			// Token: 0x040004F3 RID: 1267
			public PlayMakerArrayListProxy Spraycans;

			// Token: 0x040004F4 RID: 1268
			private bool doSync = true;

			// Token: 0x040004F5 RID: 1269
			private FsmEvent spawnAll;

			// Token: 0x040004F6 RID: 1270
			private NetEvent<NetCreateItemsManager.Item> ShoppingSpawnOne;

			// Token: 0x040004F7 RID: 1271
			private NetEvent<NetCreateItemsManager.Item> ShoppingSpawnAll;

			// Token: 0x0200021C RID: 540
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x06000956 RID: 2390 RVA: 0x0002151D File Offset: 0x0001F71D
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x06000957 RID: 2391 RVA: 0x00021529 File Offset: 0x0001F729
				public <>c()
				{
				}

				// Token: 0x06000958 RID: 2392 RVA: 0x00021531 File Offset: 0x0001F731
				internal bool <CheckShoppingbag>b__14_0(PlayMakerArrayListProxy x)
				{
					return x.referenceName == "Items";
				}

				// Token: 0x06000959 RID: 2393 RVA: 0x00021543 File Offset: 0x0001F743
				internal bool <CheckShoppingbag>b__14_1(PlayMakerArrayListProxy x)
				{
					return x.referenceName == "Spraycans";
				}

				// Token: 0x040005D6 RID: 1494
				public static readonly NetCreateItemsManager.Item.<>c <>9 = new NetCreateItemsManager.Item.<>c();

				// Token: 0x040005D7 RID: 1495
				public static Func<PlayMakerArrayListProxy, bool> <>9__14_0;

				// Token: 0x040005D8 RID: 1496
				public static Func<PlayMakerArrayListProxy, bool> <>9__14_1;
			}

			// Token: 0x0200021D RID: 541
			[CompilerGenerated]
			private sealed class <>c__DisplayClass14_0
			{
				// Token: 0x0600095A RID: 2394 RVA: 0x00021555 File Offset: 0x0001F755
				public <>c__DisplayClass14_0()
				{
				}

				// Token: 0x0600095B RID: 2395 RVA: 0x00021560 File Offset: 0x0001F760
				internal void <CheckShoppingbag>b__2()
				{
					if (this.<>4__this.doSync)
					{
						NetCreateItemsManager.Creator.doSync = true;
						using (Packet packet = new Packet())
						{
							int num = 0;
							int num2 = 0;
							for (int i = 0; i < this.<>4__this.Items._arrayList.Count; i++)
							{
								packet.Write((int)this.<>4__this.Items._arrayList[i], -1);
								num++;
							}
							for (int j = 0; j < this.<>4__this.Spraycans._arrayList.Count; j++)
							{
								packet.Write((int)this.<>4__this.Spraycans._arrayList[j], -1);
								num2++;
							}
							packet.Write(num2, 0);
							packet.Write(num, 0);
							this.<>4__this.ShoppingSpawnOne.Send(packet, true);
						}
					}
					this.<>4__this.doSync = true;
				}

				// Token: 0x0600095C RID: 2396 RVA: 0x00021668 File Offset: 0x0001F868
				internal void <CheckShoppingbag>b__3()
				{
					this.currentBag.Value = this.<>4__this.Owner.Value;
					if (this.<>4__this.doSync)
					{
						using (Packet packet = new Packet())
						{
							this.<>4__this.ShoppingSpawnAll.Send(packet, true);
						}
					}
					this.<>4__this.doSync = true;
				}

				// Token: 0x040005D9 RID: 1497
				public NetCreateItemsManager.Item <>4__this;

				// Token: 0x040005DA RID: 1498
				public FsmGameObject currentBag;
			}
		}

		// Token: 0x02000107 RID: 263
		public class Creator
		{
			// Token: 0x06000544 RID: 1348 RVA: 0x0001E000 File Offset: 0x0001C200
			public Creator(PlayMakerFSM fsm)
			{
				NetCreateItemsManager.Creator <>4__this = this;
				this.name = fsm.FsmName;
				this.fsm = fsm;
				fsm.Initialize();
				this.Condition = fsm.FsmVariables.FindFsmFloat("Condition");
				this.ObjectNumberInt = fsm.FsmVariables.FindFsmInt("ObjectNumberInt");
				this.New = fsm.FsmVariables.FindFsmGameObject("New");
				this.newItemID = fsm.FsmVariables.FindFsmString("ID");
				this.CreateItem = fsm.AddEvent("MP_CREATEITEM");
				fsm.AddGlobalTransition(this.CreateItem, fsm.HasState("Create") ? "Create" : "Add ID");
				this.SpawnItem = fsm.AddEvent("MP_SPAWNITEM");
				fsm.AddGlobalTransition(this.SpawnItem, fsm.HasState("Spawn") ? "Spawn" : "Create product");
				this.spawnInitItem = NetEvent<NetCreateItemsManager.Creator>.Register("SpawnInit" + this.name, new NetEventHandler(this.OnSpawnInitItem));
				this.spawnItem = NetEvent<NetCreateItemsManager.Creator>.Register("Spawn" + this.name, delegate(ulong sender, Packet packet)
				{
					if (sender == BeerMPGlobals.UserID)
					{
						return;
					}
					NetCreateItemsManager.Creator.doSync = false;
					fsm.SendEvent(<>4__this.SpawnItem.Name);
				});
				fsm.Initialize();
				if (fsm.HasState("Spawn"))
				{
					fsm.InsertAction("Spawn", delegate
					{
						<>4__this.OnCreateItem();
					}, -1);
				}
				if (fsm.HasState("Create product"))
				{
					fsm.InsertAction("Create product", delegate
					{
						<>4__this.OnCreateItem();
					}, -1);
				}
				if (fsm.HasState("Create"))
				{
					fsm.InsertAction("Create", delegate
					{
						<>4__this.OnCreateItem();
					}, 5);
				}
				if (fsm.HasState("Add ID"))
				{
					fsm.InsertAction("Add ID", delegate
					{
						<>4__this.OnCreateItem();
					}, 5);
				}
			}

			// Token: 0x06000545 RID: 1349 RVA: 0x0001E268 File Offset: 0x0001C468
			private void OnCreateItem()
			{
				this.items.Add(new NetCreateItemsManager.Item(this.New.Value.GetPlayMaker("Use"), this, this.newItemID.Value));
				if (NetCreateItemsManager.Creator.doSync)
				{
					using (Packet packet = new Packet(1))
					{
						this.spawnItem.Send(packet, true);
					}
				}
				NetCreateItemsManager.Creator.doSync = false;
			}

			// Token: 0x06000546 RID: 1350 RVA: 0x0001E2E4 File Offset: 0x0001C4E4
			private void OnSpawnInitItem(ulong sender, Packet packet)
			{
				if (sender == BeerMPGlobals.UserID)
				{
					return;
				}
				int num = packet.ReadInt(true);
				if (this.ObjectNumberInt.Value == num || num <= 0)
				{
					return;
				}
				this.ObjectNumberInt.Value = num;
				this.fsm.SendEvent(this.CreateItem.Name);
			}

			// Token: 0x06000547 RID: 1351 RVA: 0x0001E338 File Offset: 0x0001C538
			public void SyncInitial(ulong target)
			{
				if (this.ObjectNumberInt.Value <= 0)
				{
					return;
				}
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.ObjectNumberInt.Value, -1);
					this.spawnInitItem.Send(packet, target, true);
				}
			}

			// Token: 0x040004F8 RID: 1272
			public string name;

			// Token: 0x040004F9 RID: 1273
			public PlayMakerFSM fsm;

			// Token: 0x040004FA RID: 1274
			public FsmString newItemID;

			// Token: 0x040004FB RID: 1275
			public FsmFloat Condition;

			// Token: 0x040004FC RID: 1276
			public FsmInt ObjectNumberInt;

			// Token: 0x040004FD RID: 1277
			public FsmGameObject New;

			// Token: 0x040004FE RID: 1278
			public FsmEvent SpawnItem;

			// Token: 0x040004FF RID: 1279
			public FsmEvent CreateItem;

			// Token: 0x04000500 RID: 1280
			public List<NetCreateItemsManager.Item> items = new List<NetCreateItemsManager.Item>();

			// Token: 0x04000501 RID: 1281
			internal static bool doSync;

			// Token: 0x04000502 RID: 1282
			private NetEvent<NetCreateItemsManager.Creator> spawnInitItem;

			// Token: 0x04000503 RID: 1283
			private NetEvent<NetCreateItemsManager.Creator> spawnItem;

			// Token: 0x0200021E RID: 542
			[CompilerGenerated]
			private sealed class <>c__DisplayClass12_0
			{
				// Token: 0x0600095D RID: 2397 RVA: 0x000216E0 File Offset: 0x0001F8E0
				public <>c__DisplayClass12_0()
				{
				}

				// Token: 0x0600095E RID: 2398 RVA: 0x000216E8 File Offset: 0x0001F8E8
				internal void <.ctor>b__0(ulong sender, Packet packet)
				{
					if (sender == BeerMPGlobals.UserID)
					{
						return;
					}
					NetCreateItemsManager.Creator.doSync = false;
					this.fsm.SendEvent(this.<>4__this.SpawnItem.Name);
				}

				// Token: 0x0600095F RID: 2399 RVA: 0x00021714 File Offset: 0x0001F914
				internal void <.ctor>b__1()
				{
					this.<>4__this.OnCreateItem();
				}

				// Token: 0x06000960 RID: 2400 RVA: 0x00021721 File Offset: 0x0001F921
				internal void <.ctor>b__2()
				{
					this.<>4__this.OnCreateItem();
				}

				// Token: 0x06000961 RID: 2401 RVA: 0x0002172E File Offset: 0x0001F92E
				internal void <.ctor>b__3()
				{
					this.<>4__this.OnCreateItem();
				}

				// Token: 0x06000962 RID: 2402 RVA: 0x0002173B File Offset: 0x0001F93B
				internal void <.ctor>b__4()
				{
					this.<>4__this.OnCreateItem();
				}

				// Token: 0x040005DB RID: 1499
				public PlayMakerFSM fsm;

				// Token: 0x040005DC RID: 1500
				public NetCreateItemsManager.Creator <>4__this;
			}
		}

		// Token: 0x02000108 RID: 264
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000548 RID: 1352 RVA: 0x0001E398 File Offset: 0x0001C598
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000549 RID: 1353 RVA: 0x0001E3A4 File Offset: 0x0001C5A4
			public <>c()
			{
			}

			// Token: 0x0600054A RID: 1354 RVA: 0x0001E3AC File Offset: 0x0001C5AC
			internal bool <Start>b__10_0(GameObject x)
			{
				return x.name == "Spawner" && x.transform.root == x.transform;
			}

			// Token: 0x0600054B RID: 1355 RVA: 0x0001E3D8 File Offset: 0x0001C5D8
			internal void <Start>b__10_1(ulong user)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				for (int i = 0; i < NetCreateItemsManager.creators.Count; i++)
				{
					NetCreateItemsManager.creators[i].SyncInitial(user);
				}
			}

			// Token: 0x04000504 RID: 1284
			public static readonly NetCreateItemsManager.<>c <>9 = new NetCreateItemsManager.<>c();

			// Token: 0x04000505 RID: 1285
			public static Func<GameObject, bool> <>9__10_0;

			// Token: 0x04000506 RID: 1286
			public static Action<ulong> <>9__10_1;
		}
	}
}
