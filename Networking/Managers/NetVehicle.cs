using System;
using System.Collections.Generic;
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
	// Token: 0x0200006B RID: 107
	public class NetVehicle
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x000158C2 File Offset: 0x00013AC2
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x000158CA File Offset: 0x00013ACA
		public int hash
		{
			[CompilerGenerated]
			get
			{
				return this.<hash>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<hash>k__BackingField = value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060002EA RID: 746 RVA: 0x000158D3 File Offset: 0x00013AD3
		// (set) Token: 0x060002EB RID: 747 RVA: 0x000158DB File Offset: 0x00013ADB
		public Transform transform
		{
			[CompilerGenerated]
			get
			{
				return this.<transform>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<transform>k__BackingField = value;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060002EC RID: 748 RVA: 0x000158E4 File Offset: 0x00013AE4
		// (set) Token: 0x060002ED RID: 749 RVA: 0x000158EC File Offset: 0x00013AEC
		public Rigidbody rigidbody
		{
			[CompilerGenerated]
			get
			{
				return this.<rigidbody>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<rigidbody>k__BackingField = value;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000158F5 File Offset: 0x00013AF5
		// (set) Token: 0x060002EF RID: 751 RVA: 0x000158FD File Offset: 0x00013AFD
		public AxisCarController acc
		{
			[CompilerGenerated]
			get
			{
				return this.<acc>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<acc>k__BackingField = value;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x00015906 File Offset: 0x00013B06
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x0001590E File Offset: 0x00013B0E
		public Drivetrain drivetrain
		{
			[CompilerGenerated]
			get
			{
				return this.<drivetrain>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<drivetrain>k__BackingField = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x00015917 File Offset: 0x00013B17
		// (set) Token: 0x060002F3 RID: 755 RVA: 0x0001591F File Offset: 0x00013B1F
		public SphereCollider itemCollider
		{
			get
			{
				return this._itemCollider;
			}
			set
			{
				this._itemCollider = value;
				value.enabled = false;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x0001592F File Offset: 0x00013B2F
		// (set) Token: 0x060002F5 RID: 757 RVA: 0x00015937 File Offset: 0x00013B37
		public ulong owner
		{
			get
			{
				return this._owner;
			}
			internal set
			{
				this._owner = value;
				if (this.audioController != null)
				{
					this.audioController.IsDrivenBySoundController = value == BeerMPGlobals.UserID;
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x0001595B File Offset: 0x00013B5B
		// (set) Token: 0x060002F7 RID: 759 RVA: 0x00015963 File Offset: 0x00013B63
		public ulong driver
		{
			[CompilerGenerated]
			get
			{
				return this.<driver>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<driver>k__BackingField = value;
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0001596C File Offset: 0x00013B6C
		public NetVehicle(Transform transform)
		{
			this.rigidbody = transform.GetComponent<Rigidbody>();
			this.allRBS = transform.GetComponentsInChildren<Rigidbody>(true);
			this.transform = transform;
			this.hash = transform.GetGameobjectHashString().GetHashCode();
			this.acc = transform.GetComponent<AxisCarController>();
			this.drivetrain = transform.GetComponent<Drivetrain>();
			SoundController component = transform.GetComponent<SoundController>();
			if (component != null)
			{
				this.audioController = new NetVehicleAudio(transform, component);
			}
			this.owner = BeerMPGlobals.HostID;
			NetVehicleManager.RegisterNetVehicle(this);
			if (NetVehicle.updateIdleThrottle == null)
			{
				NetVehicle.updateIdleThrottle = NetEvent<NetVehicle>.Register("IThrottle", new NetEventHandler(NetVehicle.OnIdleThrottleUpdate));
			}
			ConfigurableJoint componentInChildren = transform.GetComponentInChildren<ConfigurableJoint>();
			this.headJoints.Add(componentInChildren);
			NetRigidbodyManager.AddRigidbody(this.rigidbody, this.hash);
			NetVehicle.FindFlatbed();
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x00015A71 File Offset: 0x00013C71
		public bool driverSeatTaken
		{
			get
			{
				return this.driver > 0UL;
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00015A7D File Offset: 0x00013C7D
		private static void FindFlatbed()
		{
			if (NetVehicle.FLATBED == null)
			{
				NetVehicle.FLATBED = GameObject.Find("FLATBED").GetComponent<Rigidbody>();
			}
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00015AA0 File Offset: 0x00013CA0
		private static void OnIdleThrottleUpdate(ulong sender, Packet p)
		{
			int hash = p.ReadInt(true);
			NetVehicle netVehicle = NetVehicleManager.vehicles.FirstOrDefault((NetVehicle v) => v.hash == hash);
			if (netVehicle == null)
			{
				Console.LogError(string.Format("Received idle throttle update for vehicle of hash {0} but it doesn't exist", hash), false);
				return;
			}
			netVehicle.updatingLastIdleThrottle = true;
			netVehicle.drivetrain.idlethrottle = p.ReadFloat(true);
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00015B10 File Offset: 0x00013D10
		public void SendEnterDrivingMode()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write(true, -1);
				this.driver = (this.owner = BeerMPGlobals.UserID);
				LocalNetPlayer.Instance.inCar = true;
				NetVehicleManager.DrivingModeEvent.Send(packet, true);
				NetRigidbodyManager.RequestOwnership(this.rigidbody);
				for (int i = 0; i < this.allRBS.Length; i++)
				{
					NetRigidbodyManager.RequestOwnership(this.allRBS[i]);
				}
				if (this.itemCollider != null)
				{
					Collider[] array = Physics.OverlapSphere(this.itemCollider.transform.position, this.itemCollider.radius, this.itemMask);
					for (int j = 0; j < array.Length; j++)
					{
						if (!(array[j].transform.parent != null))
						{
							Rigidbody component = array[j].GetComponent<Rigidbody>();
							if (!NetPlayer.grabbedItems.Contains(component))
							{
								NetRigidbodyManager.RequestOwnership(component);
							}
						}
					}
				}
				if (this.transform.name == "KEKMET(350-400psi)")
				{
					NetVehicle.FindFlatbed();
					NetRigidbodyManager.RequestOwnership(NetVehicle.FLATBED);
				}
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00015C5C File Offset: 0x00013E5C
		public void SendExitDrivingMode()
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write(false, -1);
				this.driver = 0UL;
				LocalNetPlayer.Instance.inCar = false;
				NetVehicleManager.DrivingModeEvent.Send(packet, true);
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00015CC0 File Offset: 0x00013EC0
		internal void DrivingMode(ulong player, bool enter)
		{
			Console.Log(string.Concat(new string[]
			{
				SteamFriends.GetFriendPersonaName((CSteamID)player),
				" ",
				enter ? "entered" : "exited",
				" ",
				this.transform.name,
				" driving mode"
			}), false);
			this.driver = (enter ? player : 0UL);
			if (enter)
			{
				this.owner = player;
			}
			NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)player).SetInCar(enter, this);
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00015D50 File Offset: 0x00013F50
		internal void WriteAxisControllerUpdate(Packet p)
		{
			p.Write(this.hash, -1);
			p.Write(this.acc.brakeInput, -1);
			p.Write(this.acc.throttleInput, -1);
			p.Write(this.acc.steerInput, -1);
			p.Write(this.acc.handbrakeInput, -1);
			p.Write(this.acc.clutchInput, -1);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00015DC4 File Offset: 0x00013FC4
		internal void SetAxisController(float brake, float throttle, float steering, float handbrake, float clutch)
		{
			this.brakeInput = brake;
			this.throttleInput = throttle;
			this.steerInput = steering;
			this.handbrakeInput = handbrake;
			this.clutchInput = clutch;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00015DEB File Offset: 0x00013FEB
		internal void WriteDrivetrainUpdate(Packet p)
		{
			p.Write(this.hash, -1);
			p.Write(this.drivetrain.rpm, -1);
			p.Write(this.drivetrain.gear, -1);
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00015E1E File Offset: 0x0001401E
		internal void SetDrivetrain(float rpm, int gear)
		{
			this.drivetrain.gear = gear;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00015E2C File Offset: 0x0001402C
		internal void SendInitialSync(ulong target)
		{
			if (this.driver == BeerMPGlobals.UserID)
			{
				this.SendEnterDrivingMode();
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write(this.transform.position, -1);
				packet.Write(this.transform.rotation, -1);
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00015EA0 File Offset: 0x000140A0
		internal void OnInitialSync(Packet packet)
		{
			Vector3 vector = packet.ReadVector3(true);
			Quaternion quaternion = packet.ReadQuaternion(true);
			this.transform.position = vector;
			this.transform.rotation = quaternion;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00015ED8 File Offset: 0x000140D8
		internal void Update()
		{
			if (this.owner != BeerMPGlobals.UserID && this.owner != 0UL && this.acc != null)
			{
				this.acc.brakeInput = this.brakeInput;
				this.acc.throttleInput = this.throttleInput;
				this.acc.steerInput = this.steerInput;
				this.acc.handbrakeInput = this.handbrakeInput;
				this.acc.clutchInput = this.clutchInput;
			}
			for (int i = 0; i < this.headJoints.Count; i++)
			{
				if (!(this.headJoints[i] == null))
				{
					this.headJoints[i].breakForce = float.PositiveInfinity;
					this.headJoints[i].breakTorque = float.PositiveInfinity;
				}
			}
			if (NetVehicle.flatbedPassengerSeat != null)
			{
				NetVehicle.flatbedPassengerSeat.breakForce = float.PositiveInfinity;
				NetVehicle.flatbedPassengerSeat.breakTorque = float.PositiveInfinity;
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00015FDE File Offset: 0x000141DE
		public Transform AddPassengerSeat(Vector3 triggerOffset, Vector3 headPivotOffset)
		{
			return NetVehicle.AddPassengerSeat(this, this.rigidbody, this.transform, triggerOffset, headPivotOffset);
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00015FF4 File Offset: 0x000141F4
		public static Transform AddPassengerSeat(NetVehicle self, Rigidbody rb, Transform parent, Vector3 triggerOffset, Vector3 headPivotOffset)
		{
			GameObject gameObject = GameObject.Find("NPC_CARS").transform.Find("Amikset/KYLAJANI/LOD/PlayerFunctions").gameObject;
			int seatIndex = 0;
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject);
			if (self != null)
			{
				seatIndex = self.seatCount;
				self.seatsUsed.Add(0UL);
			}
			gameObject2.name = string.Format("MPPlayerFunctions_{0}", seatIndex);
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
			PlayMakerFSM dtFsm = gameObject2.transform.Find("PlayerTrigger/DriveTrigger").GetComponent<PlayMakerFSM>();
			dtFsm.name = "PassengerTrigger";
			dtFsm.transform.parent.name = "PlayerOffset";
			dtFsm.Initialize();
			dtFsm.GetComponent<CapsuleCollider>().radius = 0.2f;
			int hash = parent.GetGameobjectHashString().GetHashCode();
			if (self != null)
			{
				dtFsm.InsertAction("Press return", delegate
				{
					if (self.seatsUsed[seatIndex] != 0UL)
					{
						dtFsm.SendEvent("FINISHED");
					}
				}, 0);
			}
			if (self != null)
			{
				dtFsm.InsertAction("Reset view", delegate
				{
					using (Packet packet = new Packet(1))
					{
						packet.Write(hash, -1);
						packet.Write(seatIndex, -1);
						packet.Write(true, -1);
						LocalNetPlayer.Instance.inCar = true;
						NetVehicleManager.PassengerModeEvent.Send(packet, true);
					}
				}, -1);
				dtFsm.InsertAction("Create player", delegate
				{
					using (Packet packet2 = new Packet(1))
					{
						packet2.Write(hash, -1);
						packet2.Write(seatIndex, -1);
						packet2.Write(false, -1);
						LocalNetPlayer.Instance.inCar = false;
						NetVehicleManager.PassengerModeEvent.Send(packet2, true);
					}
				}, -1);
			}
			(dtFsm.FsmStates.First((FsmState s) => s.Name == "Check speed").Actions[0] as GetVelocity).gameObject = new FsmOwnerDefault
			{
				GameObject = rb.transform.gameObject,
				OwnerOption = OwnerDefaultOption.SpecifyGameObject
			};
			(dtFsm.FsmStates.First((FsmState s) => s.Name == "Player in car").Actions[3] as SetStringValue).stringValue = "Passenger_" + rb.transform.name;
			gameObject2.SetActive(true);
			gameObject2.transform.GetChild(1).gameObject.SetActive(true);
			dtFsm.gameObject.SetActive(true);
			if (self != null)
			{
				self.seatCount++;
			}
			return gameObject2.transform;
		}

		// Token: 0x040002F9 RID: 761
		[CompilerGenerated]
		private int <hash>k__BackingField;

		// Token: 0x040002FA RID: 762
		[CompilerGenerated]
		private Transform <transform>k__BackingField;

		// Token: 0x040002FB RID: 763
		[CompilerGenerated]
		private Rigidbody <rigidbody>k__BackingField;

		// Token: 0x040002FC RID: 764
		[CompilerGenerated]
		private AxisCarController <acc>k__BackingField;

		// Token: 0x040002FD RID: 765
		[CompilerGenerated]
		private Drivetrain <drivetrain>k__BackingField;

		// Token: 0x040002FE RID: 766
		private SphereCollider _itemCollider;

		// Token: 0x040002FF RID: 767
		private ulong _owner;

		// Token: 0x04000300 RID: 768
		public NetVehicleDriverPivots driverPivots;

		// Token: 0x04000301 RID: 769
		[CompilerGenerated]
		private ulong <driver>k__BackingField;

		// Token: 0x04000302 RID: 770
		public static Rigidbody FLATBED;

		// Token: 0x04000303 RID: 771
		private Rigidbody[] allRBS;

		// Token: 0x04000304 RID: 772
		private float brakeInput;

		// Token: 0x04000305 RID: 773
		private float throttleInput;

		// Token: 0x04000306 RID: 774
		private float steerInput;

		// Token: 0x04000307 RID: 775
		private float handbrakeInput;

		// Token: 0x04000308 RID: 776
		private float clutchInput;

		// Token: 0x04000309 RID: 777
		internal NetVehicleAudio audioController;

		// Token: 0x0400030A RID: 778
		private int seatCount;

		// Token: 0x0400030B RID: 779
		internal List<ulong> seatsUsed = new List<ulong>();

		// Token: 0x0400030C RID: 780
		private float lastIdleThrottle;

		// Token: 0x0400030D RID: 781
		private float lastIdleThrottleUpdate;

		// Token: 0x0400030E RID: 782
		private bool updatingLastIdleThrottle;

		// Token: 0x0400030F RID: 783
		private static NetEvent<NetVehicle> updateIdleThrottle;

		// Token: 0x04000310 RID: 784
		internal List<ConfigurableJoint> headJoints = new List<ConfigurableJoint>();

		// Token: 0x04000311 RID: 785
		private static ConfigurableJoint flatbedPassengerSeat;

		// Token: 0x04000312 RID: 786
		private int itemMask = LayerMask.GetMask(new string[] { "Parts" });

		// Token: 0x02000125 RID: 293
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060005B4 RID: 1460 RVA: 0x0001FA7F File Offset: 0x0001DC7F
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060005B5 RID: 1461 RVA: 0x0001FA8B File Offset: 0x0001DC8B
			public <>c()
			{
			}

			// Token: 0x060005B6 RID: 1462 RVA: 0x0001FA93 File Offset: 0x0001DC93
			internal bool <AddPassengerSeat>b__66_3(FsmState s)
			{
				return s.Name == "Check speed";
			}

			// Token: 0x060005B7 RID: 1463 RVA: 0x0001FAA5 File Offset: 0x0001DCA5
			internal bool <AddPassengerSeat>b__66_4(FsmState s)
			{
				return s.Name == "Player in car";
			}

			// Token: 0x0400055F RID: 1375
			public static readonly NetVehicle.<>c <>9 = new NetVehicle.<>c();

			// Token: 0x04000560 RID: 1376
			public static Func<FsmState, bool> <>9__66_3;

			// Token: 0x04000561 RID: 1377
			public static Func<FsmState, bool> <>9__66_4;
		}

		// Token: 0x02000126 RID: 294
		[CompilerGenerated]
		private sealed class <>c__DisplayClass54_0
		{
			// Token: 0x060005B8 RID: 1464 RVA: 0x0001FAB7 File Offset: 0x0001DCB7
			public <>c__DisplayClass54_0()
			{
			}

			// Token: 0x060005B9 RID: 1465 RVA: 0x0001FABF File Offset: 0x0001DCBF
			internal bool <OnIdleThrottleUpdate>b__0(NetVehicle v)
			{
				return v.hash == this.hash;
			}

			// Token: 0x04000562 RID: 1378
			public int hash;
		}

		// Token: 0x02000127 RID: 295
		[CompilerGenerated]
		private sealed class <>c__DisplayClass66_0
		{
			// Token: 0x060005BA RID: 1466 RVA: 0x0001FACF File Offset: 0x0001DCCF
			public <>c__DisplayClass66_0()
			{
			}

			// Token: 0x060005BB RID: 1467 RVA: 0x0001FAD7 File Offset: 0x0001DCD7
			internal void <AddPassengerSeat>b__0()
			{
				if (this.self.seatsUsed[this.seatIndex] != 0UL)
				{
					this.dtFsm.SendEvent("FINISHED");
				}
			}

			// Token: 0x060005BC RID: 1468 RVA: 0x0001FB04 File Offset: 0x0001DD04
			internal void <AddPassengerSeat>b__1()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.hash, -1);
					packet.Write(this.seatIndex, -1);
					packet.Write(true, -1);
					LocalNetPlayer.Instance.inCar = true;
					NetVehicleManager.PassengerModeEvent.Send(packet, true);
				}
			}

			// Token: 0x060005BD RID: 1469 RVA: 0x0001FB70 File Offset: 0x0001DD70
			internal void <AddPassengerSeat>b__2()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.hash, -1);
					packet.Write(this.seatIndex, -1);
					packet.Write(false, -1);
					LocalNetPlayer.Instance.inCar = false;
					NetVehicleManager.PassengerModeEvent.Send(packet, true);
				}
			}

			// Token: 0x04000563 RID: 1379
			public NetVehicle self;

			// Token: 0x04000564 RID: 1380
			public int seatIndex;

			// Token: 0x04000565 RID: 1381
			public PlayMakerFSM dtFsm;

			// Token: 0x04000566 RID: 1382
			public int hash;
		}
	}
}
