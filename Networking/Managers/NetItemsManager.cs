using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200005F RID: 95
	[ManagerCreate(10)]
	internal class NetItemsManager : MonoBehaviour
	{
		// Token: 0x0600025F RID: 607 RVA: 0x0000F404 File Offset: 0x0000D604
		private void Start()
		{
			Action action = delegate
			{
				NetEvent<NetItemsManager>.Register("SpannerTop", new NetEventHandler(this.OnSpannerSetOpen));
				NetEvent<NetItemsManager>.Register("CamGear", new NetEventHandler(NetItemsManager.OnCamshaftGearAdjust));
				NetEvent<NetItemsManager>.Register("BeercaseD", new NetEventHandler(this.OnBeercaseSubtractBottle));
				NetEvent<NetItemsManager>.Register("JerryCan", new NetEventHandler(this.OnJerryCanSync));
				NetEvent<NetItemsManager>.Register("JCanLid", new NetEventHandler(this.OnJerryCanLid));
				NetEvent<NetItemsManager>.Register("Distributor", new NetEventHandler(this.OnDistributorHandRotate));
				NetEvent<NetItemsManager>.Register("AlternatorTune", new NetEventHandler(this.OnAlternatorHandRotate));
				NetEvent<NetItemsManager>.Register("CarFluids", new NetEventHandler(FsmNetVehicle.OnCarFluidsAndFields));
				NetEvent<NetItemsManager>.Register("TrailerDetach", new NetEventHandler(this.OnTrailerDetach));
				NetEvent<NetItemsManager>.Register("WoodCar", new NetEventHandler(this.OnWoodCarrierSpawnLog));
				Transform transform = GameObject.Find("ITEMS").transform;
				NetItemsManager.spannerSetTop = transform.Find("spanner set(itemx)/Pivot/top").GetComponent<PlayMakerFSM>();
				this.InjectSpannerSetTop(NetItemsManager.spannerSetTop, delegate
				{
					this.SpannerSetOpen(false);
				}, out NetItemsManager.spannerSetOpen);
				NetItemsManager.ratchetSetTop = NetItemsManager.GetDatabaseObject("Database/DatabaseOrders/Ratchet Set").transform.Find("Hinge/Pivot/top").GetComponent<PlayMakerFSM>();
				this.InjectSpannerSetTop(NetItemsManager.ratchetSetTop, delegate
				{
					this.SpannerSetOpen(true);
				}, out NetItemsManager.ratchetSetOpen);
				NetItemsManager.camshaftGear = NetItemsManager.GetDatabaseObject("Database/DatabaseMotor/CamshaftGear").GetPlayMaker("BoltCheck");
				NetItemsManager.camshaftGearMesh = NetItemsManager.camshaftGear.transform.Find("camshaft_gear_mesh");
				NetItemsManager.camshaftGearAngle = NetItemsManager.camshaftGear.FsmVariables.FindFsmFloat("Angle");
				NetItemsManager.camshaftGearRotateAmount = NetItemsManager.camshaftGear.FsmVariables.FindFsmFloat("RotateAmount");
				Transform transform2 = transform.Find("gasoline(itemx)");
				NetItemsManager.gasolineCanFluid = transform2.Find("FluidTrigger").GetPlayMaker("Data").FsmVariables.FindFsmFloat("Fluid");
				this.SetupJerryCanLidFsm(transform2.Find("Open"), false);
				Transform transform3 = transform.Find("diesel(itemx)");
				NetItemsManager.dieselCanFluid = transform3.Find("FluidTrigger").GetPlayMaker("Data").FsmVariables.FindFsmFloat("Fluid");
				this.SetupJerryCanLidFsm(transform3.Find("Open"), true);
				NetItemsManager.distributorHandRotate = NetItemsManager.GetDatabaseObject("Database/DatabaseMotor/Distributor").GetPlayMaker("HandRotate");
				NetItemsManager.distributorRotation = NetItemsManager.distributorHandRotate.FsmVariables.FindFsmFloat("Rotation");
				NetItemsManager.distributorRotationPivot = NetItemsManager.distributorHandRotate.transform.Find("Pivot");
				NetItemsManager.distributorHandRotate.InsertAction("Wait", new Action(this.DistributorHandRotate), 0);
				NetItemsManager.alternatorHandRotate = NetItemsManager.GetDatabaseObject("Database/DatabaseMotor/Alternator").transform.Find("Pivot").GetPlayMaker("HandRotate");
				NetItemsManager.alternatorRotation = NetItemsManager.alternatorHandRotate.FsmVariables.FindFsmFloat("Rotation");
				NetItemsManager.alternatorHandRotate.InsertAction("Wait", new Action(this.AlternatorHandRotate), 0);
				NetItemsManager.trailerDetach = GameObject.Find("KEKMET(350-400psi)").transform.Find("Trailer/Remove").GetComponent<PlayMakerFSM>();
				FsmEvent fsmEvent = NetItemsManager.trailerDetach.AddEvent("MP_DETACH");
				NetItemsManager.trailerDetach.AddGlobalTransition(fsmEvent, "Close door");
				NetItemsManager.trailerDetach.InsertAction("Close door", new Action(this.SendTrailerDetached), 0);
				for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
				{
					GameObject gameObject = ObjectsLoader.ObjectsInGame[i];
					int hash = gameObject.transform.GetGameobjectHashString().GetHashCode();
					if (gameObject.name == "wood carrier(itemx)")
					{
						gameObject.SetActive(false);
					}
				}
			};
			if (ObjectsLoader.IsGameLoaded)
			{
				action();
				return;
			}
			ObjectsLoader.gameLoaded += action;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000F43C File Offset: 0x0000D63C
		private void OnWoodCarrierSpawnLog(ulong sender, Packet packet)
		{
			int num = packet.ReadInt(true);
			if (!NetItemsManager.woodCarriers.ContainsKey(num))
			{
				return;
			}
			NetItemsManager.woodCarriers[num].SendEvent("PICKWOOD");
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000F474 File Offset: 0x0000D674
		private void SendTrailerDetached()
		{
			using (Packet packet = new Packet(1))
			{
				NetEvent<NetItemsManager>.Send("TrailerDetach", packet, true);
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000F4B0 File Offset: 0x0000D6B0
		private void OnTrailerDetach(ulong sender, Packet packet)
		{
			NetItemsManager.trailerDetach.Fsm.Event(NetItemsManager.trailerDetach.FsmEvents.FirstOrDefault((FsmEvent e) => e.Name == "MP_DETACH"));
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000F4F0 File Offset: 0x0000D6F0
		public static GameObject GetDatabaseObject(string databasePath)
		{
			GameObject gameObject = GameObject.Find(databasePath);
			if (gameObject == null)
			{
				Console.Log("Database '" + databasePath + "' could not be found", true);
				return null;
			}
			PlayMakerFSM component = gameObject.GetComponent<PlayMakerFSM>();
			if (component == null)
			{
				Console.Log("Database '" + databasePath + "' doesn't have an fsm", true);
				return null;
			}
			FsmGameObject fsmGameObject = component.FsmVariables.FindFsmGameObject("ThisPart");
			if (fsmGameObject == null)
			{
				Console.Log("Database '" + databasePath + "' doesn't have a this part variable", true);
				return null;
			}
			return fsmGameObject.Value;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000F57F File Offset: 0x0000D77F
		private void Update()
		{
			this.DoRegularSync(ref this.jerryCanSyncTime, new Action(this.SyncJerryCans), 30f, true);
			this.DoRegularSync(ref this.carFluidsSyncTime, new Action(FsmNetVehicle.SendCarFluidsAndFields), 30f, true);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000F5BD File Offset: 0x0000D7BD
		private void DoRegularSync(ref float time, Action doSync, float resetTime = 30f, bool onlyHost = true)
		{
			if (onlyHost && !BeerMPGlobals.IsHost)
			{
				return;
			}
			time -= Time.deltaTime;
			if (time < 0f)
			{
				time = resetTime;
				doSync();
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000F5E8 File Offset: 0x0000D7E8
		private void SpannerSetOpen(bool isRatchet)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(isRatchet, -1);
				packet.Write(isRatchet ? NetItemsManager.ratchetSetOpen.Value : NetItemsManager.spannerSetOpen.Value, -1);
				NetEvent<NetItemsManager>.Send("SpannerTop", packet, true);
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000F64C File Offset: 0x0000D84C
		private void OnSpannerSetOpen(ulong sender, Packet packet)
		{
			if (sender == BeerMPGlobals.UserID)
			{
				return;
			}
			bool flag = packet.ReadBool(true);
			if ((flag ? NetItemsManager.ratchetSetOpen : NetItemsManager.spannerSetOpen).Value != packet.ReadBool(true))
			{
				return;
			}
			(flag ? NetItemsManager.ratchetSetTop : NetItemsManager.spannerSetTop).SendEvent("MP_TOGGLE");
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000F6A4 File Offset: 0x0000D8A4
		private void InjectSpannerSetTop(PlayMakerFSM fsm, Action topToggled, out FsmBool isOpen)
		{
			FsmEvent fsmEvent = fsm.AddEvent("MP_TOGGLE");
			fsm.AddGlobalTransition(fsmEvent, "Bool test");
			isOpen = fsm.FsmVariables.FindFsmBool("Open");
			fsm.InsertAction("Bool test", topToggled, 0);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000F6E8 File Offset: 0x0000D8E8
		internal static void CamshaftGearAdjustEvent()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(NetItemsManager.camshaftGearAngle.Value, -1);
				NetEvent<NetItemsManager>.Send("CamGear", packet, true);
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000F738 File Offset: 0x0000D938
		private static void OnCamshaftGearAdjust(ulong sender, Packet p)
		{
			float num = p.ReadFloat(true);
			NetItemsManager.camshaftGearAngle.Value = num - NetItemsManager.camshaftGearRotateAmount.Value;
			NetItemsManager.camshaftGearMesh.localEulerAngles = Vector3.right * NetItemsManager.camshaftGearAngle.Value;
			NetItemsManager.camshaftGear.SendEvent("ADJUST");
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000F790 File Offset: 0x0000D990
		public static void SetupBeercaseFSM(PlayMakerFSM fsm, NetCreateItemsManager.Item item)
		{
			int hash = item.ID.GetHashCode();
			NetItemsManager.beercases.Add(item);
			fsm.InsertAction("Remove bottle", delegate
			{
				NetItemsManager.BeercaseSubtractBottleEvent(hash);
			}, 0);
			fsm.InsertAction("NPC Drink", delegate
			{
				NetItemsManager.BeercaseSubtractBottleEvent(hash);
			}, 0);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000F7F0 File Offset: 0x0000D9F0
		private static void BeercaseSubtractBottleEvent(int hash)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(hash, -1);
				NetEvent<NetItemsManager>.Send("BeercaseD", packet, true);
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000F834 File Offset: 0x0000DA34
		private void OnBeercaseSubtractBottle(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			NetCreateItemsManager.Item item = NetItemsManager.beercases.FirstOrDefault((NetCreateItemsManager.Item i) => i.ID.GetHashCode() == hash);
			if (item == null)
			{
				Console.LogError(string.Format("Beercase of hash {0} does not exist", hash), false);
				return;
			}
			item.fsm.SendEvent("SUSKI");
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000F89C File Offset: 0x0000DA9C
		private void SyncJerryCans()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(NetItemsManager.gasolineCanFluid.Value, -1);
				packet.Write(NetItemsManager.dieselCanFluid.Value, -1);
				NetEvent<NetItemsManager>.Send("JerryCan", packet, true);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000F8FC File Offset: 0x0000DAFC
		private void OnJerryCanSync(ulong sender, Packet packet)
		{
			NetItemsManager.gasolineCanFluid.Value = packet.ReadFloat(true);
			NetItemsManager.dieselCanFluid.Value = packet.ReadFloat(true);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000F920 File Offset: 0x0000DB20
		private void SetupJerryCanLidFsm(Transform lid, bool isDiesel)
		{
			PlayMakerFSM playMaker = lid.GetPlayMaker("Use");
			playMaker.Initialize();
			if (isDiesel)
			{
				NetItemsManager.dieselCanLid = playMaker;
			}
			else
			{
				NetItemsManager.gasolineCanLid = playMaker;
			}
			FsmBool fsmBool = playMaker.FsmVariables.FindFsmBool("Open");
			if (isDiesel)
			{
				NetItemsManager.dieselCanOpen = fsmBool;
			}
			else
			{
				NetItemsManager.gasolineCanOpen = fsmBool;
			}
			FsmBool fsmBool2 = playMaker.FsmVariables.FindFsmBool("Closed");
			if (isDiesel)
			{
				NetItemsManager.dieselCanClose = fsmBool2;
			}
			else
			{
				NetItemsManager.gasolineCanClose = fsmBool2;
			}
			playMaker.InsertAction("State 2", delegate
			{
				this.JerryCanLidToggle(isDiesel);
			}, 0);
			FsmEvent fsmEvent = playMaker.AddEvent("MP_TOGGLE");
			playMaker.AddGlobalTransition(fsmEvent, "State 2");
			playMaker.Initialize();
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000F9F0 File Offset: 0x0000DBF0
		private void JerryCanLidToggle(bool isDiesel)
		{
			PlayMakerFSM playMakerFSM = (isDiesel ? NetItemsManager.dieselCanLid : NetItemsManager.gasolineCanLid);
			if (this.updatingFsms.Contains(playMakerFSM))
			{
				this.updatingFsms.Remove(playMakerFSM);
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write(isDiesel, -1);
				packet.Write(!(isDiesel ? NetItemsManager.dieselCanOpen : NetItemsManager.gasolineCanOpen).Value, -1);
				NetEvent<NetItemsManager>.Send("JCanLid", packet, true);
			}
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000FA80 File Offset: 0x0000DC80
		private void OnJerryCanLid(ulong sender, Packet packet)
		{
			bool flag = packet.ReadBool(true);
			bool flag2 = packet.ReadBool(true);
			(flag ? NetItemsManager.dieselCanOpen : NetItemsManager.gasolineCanOpen).Value = !flag2;
			(flag ? NetItemsManager.dieselCanClose : NetItemsManager.gasolineCanClose).Value = flag2;
			PlayMakerFSM playMakerFSM = (flag ? NetItemsManager.dieselCanLid : NetItemsManager.gasolineCanLid);
			this.updatingFsms.Add(playMakerFSM);
			playMakerFSM.SendEvent("MP_TOGGLE");
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000FAF0 File Offset: 0x0000DCF0
		private void DistributorHandRotate()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(NetItemsManager.distributorRotationPivot.localEulerAngles.z, -1);
				NetEvent<NetItemsManager>.Send("Distributor", packet, true);
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000FB44 File Offset: 0x0000DD44
		private void OnDistributorHandRotate(ulong sender, Packet p)
		{
			float num = p.ReadFloat(true);
			NetItemsManager.distributorRotationPivot.localEulerAngles = Vector3.forward * num;
			NetItemsManager.distributorRotation.Value = num;
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000FB7C File Offset: 0x0000DD7C
		private void AlternatorHandRotate()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(NetItemsManager.alternatorHandRotate.transform.localEulerAngles.x, -1);
				NetEvent<NetItemsManager>.Send("AlternatorTune", packet, true);
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000FBD4 File Offset: 0x0000DDD4
		private void OnAlternatorHandRotate(ulong sender, Packet p)
		{
			float num = p.ReadFloat(true);
			NetItemsManager.alternatorHandRotate.transform.localEulerAngles = Vector3.right * num;
			NetItemsManager.alternatorRotation.Value = num;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000FC0E File Offset: 0x0000DE0E
		public NetItemsManager()
		{
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000FC37 File Offset: 0x0000DE37
		// Note: this type is marked as 'beforefieldinit'.
		static NetItemsManager()
		{
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000FC44 File Offset: 0x0000DE44
		[CompilerGenerated]
		private void <Start>b__37_0()
		{
			NetEvent<NetItemsManager>.Register("SpannerTop", new NetEventHandler(this.OnSpannerSetOpen));
			NetEvent<NetItemsManager>.Register("CamGear", new NetEventHandler(NetItemsManager.OnCamshaftGearAdjust));
			NetEvent<NetItemsManager>.Register("BeercaseD", new NetEventHandler(this.OnBeercaseSubtractBottle));
			NetEvent<NetItemsManager>.Register("JerryCan", new NetEventHandler(this.OnJerryCanSync));
			NetEvent<NetItemsManager>.Register("JCanLid", new NetEventHandler(this.OnJerryCanLid));
			NetEvent<NetItemsManager>.Register("Distributor", new NetEventHandler(this.OnDistributorHandRotate));
			NetEvent<NetItemsManager>.Register("AlternatorTune", new NetEventHandler(this.OnAlternatorHandRotate));
			NetEvent<NetItemsManager>.Register("CarFluids", new NetEventHandler(FsmNetVehicle.OnCarFluidsAndFields));
			NetEvent<NetItemsManager>.Register("TrailerDetach", new NetEventHandler(this.OnTrailerDetach));
			NetEvent<NetItemsManager>.Register("WoodCar", new NetEventHandler(this.OnWoodCarrierSpawnLog));
			Transform transform = GameObject.Find("ITEMS").transform;
			NetItemsManager.spannerSetTop = transform.Find("spanner set(itemx)/Pivot/top").GetComponent<PlayMakerFSM>();
			this.InjectSpannerSetTop(NetItemsManager.spannerSetTop, delegate
			{
				this.SpannerSetOpen(false);
			}, out NetItemsManager.spannerSetOpen);
			NetItemsManager.ratchetSetTop = NetItemsManager.GetDatabaseObject("Database/DatabaseOrders/Ratchet Set").transform.Find("Hinge/Pivot/top").GetComponent<PlayMakerFSM>();
			this.InjectSpannerSetTop(NetItemsManager.ratchetSetTop, delegate
			{
				this.SpannerSetOpen(true);
			}, out NetItemsManager.ratchetSetOpen);
			NetItemsManager.camshaftGear = NetItemsManager.GetDatabaseObject("Database/DatabaseMotor/CamshaftGear").GetPlayMaker("BoltCheck");
			NetItemsManager.camshaftGearMesh = NetItemsManager.camshaftGear.transform.Find("camshaft_gear_mesh");
			NetItemsManager.camshaftGearAngle = NetItemsManager.camshaftGear.FsmVariables.FindFsmFloat("Angle");
			NetItemsManager.camshaftGearRotateAmount = NetItemsManager.camshaftGear.FsmVariables.FindFsmFloat("RotateAmount");
			Transform transform2 = transform.Find("gasoline(itemx)");
			NetItemsManager.gasolineCanFluid = transform2.Find("FluidTrigger").GetPlayMaker("Data").FsmVariables.FindFsmFloat("Fluid");
			this.SetupJerryCanLidFsm(transform2.Find("Open"), false);
			Transform transform3 = transform.Find("diesel(itemx)");
			NetItemsManager.dieselCanFluid = transform3.Find("FluidTrigger").GetPlayMaker("Data").FsmVariables.FindFsmFloat("Fluid");
			this.SetupJerryCanLidFsm(transform3.Find("Open"), true);
			NetItemsManager.distributorHandRotate = NetItemsManager.GetDatabaseObject("Database/DatabaseMotor/Distributor").GetPlayMaker("HandRotate");
			NetItemsManager.distributorRotation = NetItemsManager.distributorHandRotate.FsmVariables.FindFsmFloat("Rotation");
			NetItemsManager.distributorRotationPivot = NetItemsManager.distributorHandRotate.transform.Find("Pivot");
			NetItemsManager.distributorHandRotate.InsertAction("Wait", new Action(this.DistributorHandRotate), 0);
			NetItemsManager.alternatorHandRotate = NetItemsManager.GetDatabaseObject("Database/DatabaseMotor/Alternator").transform.Find("Pivot").GetPlayMaker("HandRotate");
			NetItemsManager.alternatorRotation = NetItemsManager.alternatorHandRotate.FsmVariables.FindFsmFloat("Rotation");
			NetItemsManager.alternatorHandRotate.InsertAction("Wait", new Action(this.AlternatorHandRotate), 0);
			NetItemsManager.trailerDetach = GameObject.Find("KEKMET(350-400psi)").transform.Find("Trailer/Remove").GetComponent<PlayMakerFSM>();
			FsmEvent fsmEvent = NetItemsManager.trailerDetach.AddEvent("MP_DETACH");
			NetItemsManager.trailerDetach.AddGlobalTransition(fsmEvent, "Close door");
			NetItemsManager.trailerDetach.InsertAction("Close door", new Action(this.SendTrailerDetached), 0);
			for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
			{
				GameObject gameObject = ObjectsLoader.ObjectsInGame[i];
				int hash = gameObject.transform.GetGameobjectHashString().GetHashCode();
				if (gameObject.name == "wood carrier(itemx)")
				{
					gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00010026 File Offset: 0x0000E226
		[CompilerGenerated]
		private void <Start>b__37_1()
		{
			this.SpannerSetOpen(false);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0001002F File Offset: 0x0000E22F
		[CompilerGenerated]
		private void <Start>b__37_2()
		{
			this.SpannerSetOpen(true);
		}

		// Token: 0x04000242 RID: 578
		private static FsmBool spannerSetOpen;

		// Token: 0x04000243 RID: 579
		private static FsmBool ratchetSetOpen;

		// Token: 0x04000244 RID: 580
		private static FsmBool gasolineCanOpen;

		// Token: 0x04000245 RID: 581
		private static FsmBool dieselCanOpen;

		// Token: 0x04000246 RID: 582
		private static FsmBool gasolineCanClose;

		// Token: 0x04000247 RID: 583
		private static FsmBool dieselCanClose;

		// Token: 0x04000248 RID: 584
		private static PlayMakerFSM spannerSetTop;

		// Token: 0x04000249 RID: 585
		private static PlayMakerFSM ratchetSetTop;

		// Token: 0x0400024A RID: 586
		private static PlayMakerFSM camshaftGear;

		// Token: 0x0400024B RID: 587
		private static PlayMakerFSM dieselCanLid;

		// Token: 0x0400024C RID: 588
		private static PlayMakerFSM gasolineCanLid;

		// Token: 0x0400024D RID: 589
		private static PlayMakerFSM distributorHandRotate;

		// Token: 0x0400024E RID: 590
		private static PlayMakerFSM alternatorHandRotate;

		// Token: 0x0400024F RID: 591
		private static PlayMakerFSM trailerDetach;

		// Token: 0x04000250 RID: 592
		private static List<NetCreateItemsManager.Item> beercases = new List<NetCreateItemsManager.Item>();

		// Token: 0x04000251 RID: 593
		private static FsmFloat camshaftGearAngle;

		// Token: 0x04000252 RID: 594
		private static FsmFloat camshaftGearRotateAmount;

		// Token: 0x04000253 RID: 595
		private static FsmFloat gasolineCanFluid;

		// Token: 0x04000254 RID: 596
		private static FsmFloat dieselCanFluid;

		// Token: 0x04000255 RID: 597
		private static FsmFloat distributorRotation;

		// Token: 0x04000256 RID: 598
		private static FsmFloat alternatorRotation;

		// Token: 0x04000257 RID: 599
		private static Transform camshaftGearMesh;

		// Token: 0x04000258 RID: 600
		private static Transform distributorRotationPivot;

		// Token: 0x04000259 RID: 601
		private static Dictionary<int, PlayMakerFSM> woodCarriers;

		// Token: 0x0400025A RID: 602
		private List<PlayMakerFSM> updatingFsms = new List<PlayMakerFSM>();

		// Token: 0x0400025B RID: 603
		public float jerryCanSyncTime = 31f;

		// Token: 0x0400025C RID: 604
		public float carFluidsSyncTime = 32f;

		// Token: 0x0400025D RID: 605
		internal const string spannerSetOpenEvent = "SpannerTop";

		// Token: 0x0400025E RID: 606
		internal const string camshaftGearEvent = "CamGear";

		// Token: 0x0400025F RID: 607
		internal const string beercaseDrinkEvent = "BeercaseD";

		// Token: 0x04000260 RID: 608
		internal const string jerryCanSyncEvent = "JerryCan";

		// Token: 0x04000261 RID: 609
		internal const string jerryCanLidEvent = "JCanLid";

		// Token: 0x04000262 RID: 610
		internal const string distributorRotateEvent = "Distributor";

		// Token: 0x04000263 RID: 611
		internal const string alternatorRotateEvent = "AlternatorTune";

		// Token: 0x04000264 RID: 612
		internal const string carFluidsSync = "CarFluids";

		// Token: 0x04000265 RID: 613
		internal const string trailerDetachEvent = "TrailerDetach";

		// Token: 0x04000266 RID: 614
		internal const string woodCarrierEvent = "WoodCar";

		// Token: 0x02000100 RID: 256
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000530 RID: 1328 RVA: 0x0001DB2A File Offset: 0x0001BD2A
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000531 RID: 1329 RVA: 0x0001DB36 File Offset: 0x0001BD36
			public <>c()
			{
			}

			// Token: 0x06000532 RID: 1330 RVA: 0x0001DB3E File Offset: 0x0001BD3E
			internal bool <OnTrailerDetach>b__40_0(FsmEvent e)
			{
				return e.Name == "MP_DETACH";
			}

			// Token: 0x040004E2 RID: 1250
			public static readonly NetItemsManager.<>c <>9 = new NetItemsManager.<>c();

			// Token: 0x040004E3 RID: 1251
			public static Func<FsmEvent, bool> <>9__40_0;
		}

		// Token: 0x02000101 RID: 257
		[CompilerGenerated]
		private sealed class <>c__DisplayClass37_0
		{
			// Token: 0x06000533 RID: 1331 RVA: 0x0001DB50 File Offset: 0x0001BD50
			public <>c__DisplayClass37_0()
			{
			}

			// Token: 0x06000534 RID: 1332 RVA: 0x0001DB58 File Offset: 0x0001BD58
			internal void <Start>b__4()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.hash, -1);
					NetEvent<NetItemsManager>.Send("WoodCar", packet, true);
				}
			}

			// Token: 0x040004E4 RID: 1252
			public int hash;
		}

		// Token: 0x02000102 RID: 258
		[CompilerGenerated]
		private sealed class <>c__DisplayClass37_1
		{
			// Token: 0x06000535 RID: 1333 RVA: 0x0001DBA4 File Offset: 0x0001BDA4
			public <>c__DisplayClass37_1()
			{
			}

			// Token: 0x06000536 RID: 1334 RVA: 0x0001DBAC File Offset: 0x0001BDAC
			internal void <Start>b__3()
			{
				Rigidbody component = this.newWood.Value.GetComponent<Rigidbody>();
				NetRigidbodyManager.AddRigidbody(component, (component.transform.GetGameobjectHashString() + NetJobManager.logsCount++.ToString()).GetHashCode());
			}

			// Token: 0x040004E5 RID: 1253
			public FsmGameObject newWood;
		}

		// Token: 0x02000103 RID: 259
		[CompilerGenerated]
		private sealed class <>c__DisplayClass49_0
		{
			// Token: 0x06000537 RID: 1335 RVA: 0x0001DBF9 File Offset: 0x0001BDF9
			public <>c__DisplayClass49_0()
			{
			}

			// Token: 0x06000538 RID: 1336 RVA: 0x0001DC01 File Offset: 0x0001BE01
			internal void <SetupBeercaseFSM>b__0()
			{
				NetItemsManager.BeercaseSubtractBottleEvent(this.hash);
			}

			// Token: 0x06000539 RID: 1337 RVA: 0x0001DC0E File Offset: 0x0001BE0E
			internal void <SetupBeercaseFSM>b__1()
			{
				NetItemsManager.BeercaseSubtractBottleEvent(this.hash);
			}

			// Token: 0x040004E6 RID: 1254
			public int hash;
		}

		// Token: 0x02000104 RID: 260
		[CompilerGenerated]
		private sealed class <>c__DisplayClass51_0
		{
			// Token: 0x0600053A RID: 1338 RVA: 0x0001DC1B File Offset: 0x0001BE1B
			public <>c__DisplayClass51_0()
			{
			}

			// Token: 0x0600053B RID: 1339 RVA: 0x0001DC23 File Offset: 0x0001BE23
			internal bool <OnBeercaseSubtractBottle>b__0(NetCreateItemsManager.Item i)
			{
				return i.ID.GetHashCode() == this.hash;
			}

			// Token: 0x040004E7 RID: 1255
			public int hash;
		}

		// Token: 0x02000105 RID: 261
		[CompilerGenerated]
		private sealed class <>c__DisplayClass54_0
		{
			// Token: 0x0600053C RID: 1340 RVA: 0x0001DC38 File Offset: 0x0001BE38
			public <>c__DisplayClass54_0()
			{
			}

			// Token: 0x0600053D RID: 1341 RVA: 0x0001DC40 File Offset: 0x0001BE40
			internal void <SetupJerryCanLidFsm>b__0()
			{
				this.<>4__this.JerryCanLidToggle(this.isDiesel);
			}

			// Token: 0x040004E8 RID: 1256
			public NetItemsManager <>4__this;

			// Token: 0x040004E9 RID: 1257
			public bool isDiesel;
		}
	}
}
