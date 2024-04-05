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
	// Token: 0x02000064 RID: 100
	internal class FsmNetVehicle
	{
		// Token: 0x060002A6 RID: 678 RVA: 0x00011FF4 File Offset: 0x000101F4
		public FsmNetVehicle(Transform transform)
		{
			this.transform = transform;
			this.netVehicle = new NetVehicle(transform);
			PlayMakerFSM[] componentsInChildren = transform.GetComponentsInChildren<PlayMakerFSM>(true);
			this.DoItemCollider();
			this.DoFsms();
			this.DoDrivingMode(componentsInChildren);
			this.DoPassengerSeats();
			this.DoDriverPivots();
			this.DoFluidsAndFields(componentsInChildren);
			this.DoDoors(componentsInChildren);
			this.DoDashboard(componentsInChildren);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00012090 File Offset: 0x00010290
		private void DoItemCollider()
		{
			int num = Array.IndexOf<string>(FsmNetVehicle.carNames, this.transform.name);
			if (num == -1)
			{
				Console.LogWarning(string.Format("Car {0} ({1}) not in car names list", this.netVehicle.hash, this.transform.name), false);
				return;
			}
			SphereCollider sphereCollider = new GameObject("ItemCollider")
			{
				transform = 
				{
					parent = this.transform,
					localPosition = FsmNetVehicle.itemColliderCenter[num]
				}
			}.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = FsmNetVehicle.itemColliderRadius[num];
			this.netVehicle.itemCollider = sphereCollider;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0001213C File Offset: 0x0001033C
		private void DoDrivingMode(PlayMakerFSM[] fsms)
		{
			for (int i = 0; i < fsms.Length; i++)
			{
				if (fsms[i].FsmName == "Death" && fsms[i].gameObject.name == "DriverHeadPivot")
				{
					Transform transform = fsms[i].transform;
					transform.GetComponent<Rigidbody>().isKinematic = true;
					global::UnityEngine.Object.Destroy(fsms[i]);
					ConfigurableJoint configurableJoint = transform.GetComponent<ConfigurableJoint>();
					if (configurableJoint == null)
					{
						configurableJoint = transform.parent.GetComponentInChildren<ConfigurableJoint>();
					}
					configurableJoint.transform.localPosition = configurableJoint.connectedAnchor;
					configurableJoint.transform.localEulerAngles = Vector3.zero;
					global::UnityEngine.Object.Destroy(configurableJoint);
					Console.Log("Successfully removed death fsm from driving mode of " + this.transform.name, false);
					return;
				}
			}
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0001220C File Offset: 0x0001040C
		private void DoDashboard(PlayMakerFSM[] fsms)
		{
			foreach (PlayMakerFSM playMakerFSM in fsms)
			{
				playMakerFSM.Initialize();
				if (playMakerFSM.FsmName == "Knob")
				{
					this.dashboardKnobs.Add(new FsmDashboardKnob(playMakerFSM));
				}
				else if (playMakerFSM.transform.name == "TurnSignals" && playMakerFSM.FsmName == "Usage")
				{
					this.turnSignals.Add(new FsmTurnSignals(playMakerFSM));
				}
				else if (!(playMakerFSM.FsmName != "Use"))
				{
					if (!playMakerFSM.HasState("Test") && !playMakerFSM.HasState("Test 2"))
					{
						if (playMakerFSM.HasState("INCREASE") && playMakerFSM.HasState("DECREASE"))
						{
							Console.Log("Added dashboard lever for car " + this.transform.name + ": " + playMakerFSM.transform.name, false);
							this.dashboardLevers.Add(new FsmDashboardLever(playMakerFSM));
						}
					}
					else if (!(playMakerFSM.transform.name == "Ignition"))
					{
						Console.Log("Added dashboard button for car " + this.transform.name + ": " + playMakerFSM.transform.name, false);
						this.dashboardButtons.Add(new FsmDashboardButton(playMakerFSM));
					}
				}
			}
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00012380 File Offset: 0x00010580
		private void DoDoors(PlayMakerFSM[] fsms)
		{
			if (this.transform.name == "SATSUMA(557kg, 248)")
			{
				this.vehicleDoors.Add(new FsmVehicleDoor(NetItemsManager.GetDatabaseObject("Database/DatabaseBody/Door_Left").GetPlayMaker("Use"), false));
				this.vehicleDoors.Add(new FsmVehicleDoor(NetItemsManager.GetDatabaseObject("Database/DatabaseBody/Door_Right").GetPlayMaker("Use"), false));
				this.vehicleDoors.Add(new FsmVehicleDoor(NetItemsManager.GetDatabaseObject("Database/DatabaseBody/Bootlid").transform.Find("Handles").GetPlayMaker("Use"), false));
				return;
			}
			foreach (PlayMakerFSM playMakerFSM in fsms)
			{
				if (!(playMakerFSM.FsmName != "Use") && (!(playMakerFSM.transform.name != "Handle") || !(playMakerFSM.transform.name != "Handles") || playMakerFSM.transform.name.ToLower().Contains("door") || playMakerFSM.transform.name.ToLower().Contains("bootlid")))
				{
					this.vehicleDoors.Add(new FsmVehicleDoor(playMakerFSM, false));
				}
			}
			if (this.transform.name == "HAYOSIKO(1500kg, 250)")
			{
				this.vehicleDoors.Add(new FsmVehicleDoor(this.transform.Find("SideDoor/door/Collider").GetComponent<PlayMakerFSM>(), true));
			}
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00012504 File Offset: 0x00010704
		private void DoFluidsAndFields(PlayMakerFSM[] fsms)
		{
			if (this.transform.name.ToUpper().Contains("SATSUMA"))
			{
				this.fuelLevel = this.GetDatabaseFsmFloat("Database/DatabaseMechanics/FuelTank", "FuelLevel");
				this.engineTemp = PlayMakerGlobals.Instance.Variables.FindFsmFloat("EngineTemp");
				this.oilLevel = this.GetDatabaseFsmFloat("Database/DatabaseMotor/Oilpan", "Oil");
				this.oilContamination = this.GetDatabaseFsmFloat("Database/DatabaseMotor/Oilpan", "OilContamination");
				this.oilGrade = this.GetDatabaseFsmFloat("Database/DatabaseMotor/Oilpan", "OilGrade");
				this.coolant1Level = this.GetDatabaseFsmFloat("Database/DatabaseMechanics/Radiator", "Water");
				this.coolant2Level = this.GetDatabaseFsmFloat("Database/DatabaseOrders/Racing Radiator", "Water");
				this.brake1Level = this.GetDatabaseFsmFloat("Database/DatabaseMechanics/BrakeMasterCylinder", "BrakeFluidF");
				this.brake2Level = this.GetDatabaseFsmFloat("Database/DatabaseMechanics/BrakeMasterCylinder", "BrakeFluidR");
				this.clutchLevel = this.GetDatabaseFsmFloat("Database/DatabaseMechanics/ClutchMasterCylinder", "ClutchFluid");
				Console.Log(string.Format("Init fluids and fields for Satsuma, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", new object[] { this.fuelLevel, this.engineTemp, this.oilLevel, this.oilContamination, this.oilGrade, this.coolant1Level, this.coolant2Level, this.brake1Level, this.brake2Level, this.clutchLevel }), false);
				return;
			}
			for (int i = 0; i < fsms.Length; i++)
			{
				if (fsms[i].transform.name == "FuelTank")
				{
					this.fuelLevel = fsms[i].FsmVariables.FindFsmFloat("FuelLevel");
					this.oilLevel = fsms[i].FsmVariables.FindFsmFloat("FuelOil");
				}
				else if (fsms[i].FsmName == "Cooling")
				{
					this.engineTemp = fsms[i].FsmVariables.FloatVariables.FirstOrDefault((FsmFloat f) => f.Name.Contains("EngineTemp"));
				}
			}
			Console.Log(string.Format("Init fluids and fields for {0}, {1}, {2}, {3}", new object[]
			{
				this.transform.name,
				this.fuelLevel,
				this.engineTemp,
				this.oilLevel
			}), false);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0001276C File Offset: 0x0001096C
		internal static void SendCarFluidsAndFields()
		{
			using (Packet packet = new Packet(1))
			{
				for (int i = 0; i < NetVehicleManager.vanillaVehicles.Count; i++)
				{
					FsmNetVehicle fsmNetVehicle = NetVehicleManager.vanillaVehicles[i];
					packet.Write(fsmNetVehicle.netVehicle.hash, -1);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.fuelLevel, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.oilLevel, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.oilContamination, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.oilGrade, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.coolant1Level, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.coolant2Level, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.brake1Level, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.brake2Level, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.clutchLevel, packet);
					FsmNetVehicle.WriteNullableFloat(fsmNetVehicle.engineTemp, packet);
				}
				NetEvent<NetItemsManager>.Send("CarFluids", packet, true);
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0001285C File Offset: 0x00010A5C
		internal static void OnCarFluidsAndFields(ulong sender, Packet p)
		{
			while (p.UnreadLength() > 0)
			{
				int hash = p.ReadInt(true);
				FsmNetVehicle fsmNetVehicle = NetVehicleManager.vanillaVehicles.FirstOrDefault((FsmNetVehicle v) => v.netVehicle.hash == hash);
				if (fsmNetVehicle == null)
				{
					Console.LogError(string.Format("OnCarFluidsAndFields vehicle of hash {0} cannot be found", hash), false);
					for (int i = 0; i < 10; i++)
					{
						p.ReadFloat(true);
					}
				}
				else
				{
					float num;
					if (FsmNetVehicle.ReadNullableFloat(p, out num))
					{
						fsmNetVehicle.fuelLevel.Value = num;
					}
					float num2;
					if (FsmNetVehicle.ReadNullableFloat(p, out num2))
					{
						fsmNetVehicle.oilLevel.Value = num2;
					}
					float num3;
					if (FsmNetVehicle.ReadNullableFloat(p, out num3))
					{
						fsmNetVehicle.oilContamination.Value = num3;
					}
					float num4;
					if (FsmNetVehicle.ReadNullableFloat(p, out num4))
					{
						fsmNetVehicle.oilGrade.Value = num4;
					}
					float num5;
					if (FsmNetVehicle.ReadNullableFloat(p, out num5))
					{
						fsmNetVehicle.coolant1Level.Value = num5;
					}
					float num6;
					if (FsmNetVehicle.ReadNullableFloat(p, out num6))
					{
						fsmNetVehicle.coolant2Level.Value = num6;
					}
					float num7;
					if (FsmNetVehicle.ReadNullableFloat(p, out num7))
					{
						fsmNetVehicle.brake1Level.Value = num7;
					}
					float num8;
					if (FsmNetVehicle.ReadNullableFloat(p, out num8))
					{
						fsmNetVehicle.brake2Level.Value = num8;
					}
					float num9;
					if (FsmNetVehicle.ReadNullableFloat(p, out num9))
					{
						fsmNetVehicle.clutchLevel.Value = num9;
					}
					float num10;
					if (FsmNetVehicle.ReadNullableFloat(p, out num10))
					{
						fsmNetVehicle.engineTemp.Value = num10;
					}
				}
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000129C4 File Offset: 0x00010BC4
		private static bool ReadNullableFloat(Packet p, out float f)
		{
			f = p.ReadFloat(true);
			return !float.IsNaN(f);
		}

		// Token: 0x060002AF RID: 687 RVA: 0x000129D9 File Offset: 0x00010BD9
		private static void WriteNullableFloat(FsmFloat f, Packet p)
		{
			p.Write((f == null) ? float.NaN : f.Value, -1);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x000129F4 File Offset: 0x00010BF4
		private FsmFloat GetDatabaseFsmFloat(string databasePath, string variableName)
		{
			GameObject gameObject = GameObject.Find(databasePath);
			if (gameObject == null)
			{
				Console.Log("NV: Database '" + databasePath + "' could not be found", true);
				return null;
			}
			PlayMakerFSM component = gameObject.GetComponent<PlayMakerFSM>();
			if (component == null)
			{
				Console.Log("NV: Database '" + databasePath + "' doesn't have an fsm", true);
				return null;
			}
			FsmFloat fsmFloat = component.FsmVariables.FindFsmFloat(variableName);
			if (fsmFloat == null)
			{
				Console.Log(string.Concat(new string[] { "NV: Database '", databasePath, "' doesn't have a ", variableName, " variable" }), true);
				return null;
			}
			return fsmFloat;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00012A98 File Offset: 0x00010C98
		private void DoDriverPivots()
		{
			int num = Array.IndexOf<string>(FsmNetVehicle.carNames, this.transform.name);
			if (num == -1)
			{
				return;
			}
			NetVehicle netVehicle = this.netVehicle;
			NetVehicleDriverPivots netVehicleDriverPivots = new NetVehicleDriverPivots();
			netVehicleDriverPivots.throttlePedal = this.MakeDriverPivot(FsmNetVehicle.throttlePedals[num]);
			netVehicleDriverPivots.brakePedal = this.MakeDriverPivot(FsmNetVehicle.brakePedals[num]);
			netVehicleDriverPivots.clutchPedal = this.MakeDriverPivot(FsmNetVehicle.clutchPedals[num]);
			netVehicleDriverPivots.steeringWheel = this.MakeDriverPivot(FsmNetVehicle.steeringWheels[num]);
			NetVehicleDriverPivots netVehicleDriverPivots2 = netVehicleDriverPivots;
			Transform[] array;
			if (num != 4)
			{
				(array = new Transform[1])[0] = this.MakeDriverPivot(FsmNetVehicle.gearSticks[num]);
			}
			else
			{
				Transform[] array2 = new Transform[2];
				array2[0] = this.MakeDriverPivot(FsmNetVehicle.gearSticks[num]);
				array = array2;
				array2[1] = this.MakeDriverPivot(FsmNetVehicle.gearSticks[num].GetAltPivot());
			}
			netVehicleDriverPivots2.gearSticks = array;
			netVehicleDriverPivots.driverParent = this.MakeDriverPivot(FsmNetVehicle.drivingModes[num]);
			netVehicle.driverPivots = netVehicleDriverPivots;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00012BA0 File Offset: 0x00010DA0
		private Transform MakeDriverPivot(FsmNetVehicle.Pivot pivot)
		{
			if (pivot.path == "")
			{
				return null;
			}
			Transform transform = ((pivot.path == null) ? this.transform : this.transform.Find(pivot.path));
			if (transform == null)
			{
				return null;
			}
			Transform transform2 = new GameObject("BeerMP_DriverPivot").transform;
			transform2.parent = transform;
			transform2.localPosition = pivot.position;
			transform2.localEulerAngles = pivot.eulerAngles;
			return transform2;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00012C1C File Offset: 0x00010E1C
		private void DoFsms()
		{
			PlayMakerFSM[] componentsInChildren = this.transform.GetComponentsInChildren<PlayMakerFSM>(true);
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
					if (this.netVehicle.driverSeatTaken)
					{
						drivingMode.SendEvent("FINISHED");
					}
				}, 0);
				drivingMode.InsertAction("Reset view", new Action(this.netVehicle.SendEnterDrivingMode), -1);
				drivingMode.InsertAction("Create player", new Action(this.netVehicle.SendExitDrivingMode), -1);
				Console.Log("Init driving mode for " + this.transform.name, false);
			}
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00012D08 File Offset: 0x00010F08
		private void DoPassengerSeats()
		{
			int num = Array.IndexOf<string>(FsmNetVehicle.carNames, this.transform.name);
			if (num == -1)
			{
				Console.LogWarning(string.Format("no passenger seats for car with hash {0} ({1})", this.netVehicle.hash, this.transform.name), false);
				return;
			}
			Vector3[][] array = FsmNetVehicle.carPassengerSeats[num];
			if (array.Length == 0)
			{
				Console.LogWarning(string.Format("no passenger seats for car with hash {0} ({1})", this.netVehicle.hash, this.transform.name), false);
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				this.netVehicle.AddPassengerSeat(array[i][0], array[i][1]);
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00012DC0 File Offset: 0x00010FC0
		public static void DoFlatbedPassengerSeats(out Transform _flatbed, out int _hash, Action<bool> enterPassenger)
		{
			Transform transform = GameObject.Find("FLATBED").transform;
			_flatbed = transform;
			int hashCode = transform.gameObject.name.GetHashCode();
			_hash = hashCode;
			Transform transform2 = transform.Find("Bed");
			Transform transform3 = NetVehicle.AddPassengerSeat(null, transform.GetComponent<Rigidbody>(), transform2, new Vector3(0f, 0.5f, 2f), default(Vector3)).Find("PlayerOffset/PassengerTrigger");
			global::UnityEngine.Object.Destroy(transform3.GetComponent<CapsuleCollider>());
			BoxCollider boxCollider = transform3.gameObject.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;
			boxCollider.size = new Vector3(2f, 1f, 4.8f);
			Transform transform4 = global::UnityEngine.Object.Instantiate<Transform>(GameObject.Find("RCO_RUSCKO12(270)").transform.Find("LOD/PlayerTrigger"));
			for (int i = 0; i < transform4.childCount; i++)
			{
				global::UnityEngine.Object.Destroy(transform4.GetChild(i));
			}
			transform4.transform.parent = transform3;
			transform4.transform.localPosition = (transform4.transform.localEulerAngles = Vector3.zero);
			transform4.GetComponent<BoxCollider>().size = new Vector3(2f, 1f, 4.8f);
			PlayMakerFSM component = transform3.GetComponent<PlayMakerFSM>();
			component.Initialize();
			component.InsertAction("Reset view", delegate
			{
				enterPassenger(true);
			}, -1);
			component.InsertAction("Create player", delegate
			{
				enterPassenger(false);
			}, -1);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00012F4C File Offset: 0x0001114C
		// Note: this type is marked as 'beforefieldinit'.
		static FsmNetVehicle()
		{
			FsmNetVehicle.Pivot[] array = new FsmNetVehicle.Pivot[7];
			int num = 0;
			FsmNetVehicle.Pivot pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals 1/throttle",
				position = new Vector3(0f, -0.2f, -0.26f),
				eulerAngles = new Vector3(310f, 0f, 180f)
			};
			array[num] = pivot;
			int num2 = 1;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals/Throttle",
				position = default(Vector3),
				eulerAngles = new Vector3(320f, 0f, 180f)
			};
			array[num2] = pivot;
			int num3 = 2;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals 2/throttle",
				position = new Vector3(0f, -0.2f, 0f),
				eulerAngles = new Vector3(294f, 0f, 180f)
			};
			array[num3] = pivot;
			int num4 = 3;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals/throttle",
				position = new Vector3(0f, 0.02f, 0.1f),
				eulerAngles = new Vector3(340f, 0f, 180f)
			};
			array[num4] = pivot;
			int num5 = 4;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "Dashboard/Pedals/pedal_throttle",
				position = new Vector3(0f, -0.24f, -0.15f),
				eulerAngles = new Vector3(330f, 0f, 180f)
			};
			array[num5] = pivot;
			int num6 = 5;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/ThrottleFoot/Pivot/tractor_pedal_speed",
				position = new Vector3(-0.21f, -0.03f, 0.04f),
				eulerAngles = new Vector3(-45f, 90f, 90f)
			};
			array[num6] = pivot;
			int num7 = 6;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "MESH",
				position = new Vector3(-0.05999999f, 0.13f, -0.18f),
				eulerAngles = new Vector3(350.0001f, 90.00019f, 100.0002f)
			};
			array[num7] = pivot;
			FsmNetVehicle.throttlePedals = array;
			FsmNetVehicle.Pivot[] array2 = new FsmNetVehicle.Pivot[7];
			int num8 = 0;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals 1/brake",
				position = new Vector3(0f, -0.1f, -0.26f),
				eulerAngles = new Vector3(350f, 0f, 180f)
			};
			array2[num8] = pivot;
			int num9 = 1;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals/Brake",
				position = default(Vector3),
				eulerAngles = new Vector3(330f, 0f, 180f)
			};
			array2[num9] = pivot;
			int num10 = 2;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals 2/brake",
				position = new Vector3(0f, -0.38f, -0.3f),
				eulerAngles = new Vector3(312f, 0f, 180f)
			};
			array2[num10] = pivot;
			int num11 = 3;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals/brake",
				position = new Vector3(0f, -0.05f, 0.26f),
				eulerAngles = new Vector3(340f, 0f, 180f)
			};
			array2[num11] = pivot;
			int num12 = 4;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "Dashboard/Pedals/pedal_brake",
				position = new Vector3(0f, -0.25f, -0.41f),
				eulerAngles = new Vector3(330f, 0f, 180f)
			};
			array2[num12] = pivot;
			int num13 = 5;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Brake/Pivot/tractor_pedal_brake",
				position = new Vector3(-0.14f, -0.1f, -0.04f),
				eulerAngles = new Vector3(-10f, 30f, 90f)
			};
			array2[num13] = pivot;
			int num14 = 6;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "MESH",
				position = new Vector3(-0.05999999f, -0.13f, -0.18f),
				eulerAngles = new Vector3(350.0001f, 80.00032f, 100.0003f)
			};
			array2[num14] = pivot;
			FsmNetVehicle.brakePedals = array2;
			FsmNetVehicle.Pivot[] array3 = new FsmNetVehicle.Pivot[7];
			int num15 = 0;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals 1/clutch",
				position = new Vector3(0f, -0.1f, -0.26f),
				eulerAngles = new Vector3(350f, 0f, 180f)
			};
			array3[num15] = pivot;
			int num16 = 1;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals/Clutch",
				position = new Vector3(0f, -0.21f, -0.36f),
				eulerAngles = new Vector3(330f, 0f, 180f)
			};
			array3[num16] = pivot;
			int num17 = 2;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "",
				position = default(Vector3),
				eulerAngles = default(Vector3)
			};
			array3[num17] = pivot;
			int num18 = 3;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Pedals/clutch",
				position = new Vector3(0f, -0.05f, 0.15f),
				eulerAngles = new Vector3(340f, 0f, 180f)
			};
			array3[num18] = pivot;
			int num19 = 4;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "Dashboard/Pedals/pedal_clutch",
				position = new Vector3(0f, -0.25f, -0.41f),
				eulerAngles = new Vector3(330f, 0f, 180f)
			};
			array3[num19] = pivot;
			int num20 = 5;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Clutch/Pivot/tractor_pedal_clutch",
				position = new Vector3(-0.17f, 0f, -0.11f),
				eulerAngles = new Vector3(0f, 30f, 90f)
			};
			array3[num20] = pivot;
			int num21 = 6;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "",
				position = default(Vector3),
				eulerAngles = default(Vector3)
			};
			array3[num21] = pivot;
			FsmNetVehicle.clutchPedals = array3;
			FsmNetVehicle.Pivot[] array4 = new FsmNetVehicle.Pivot[7];
			int num22 = 0;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Steering/VanSteeringPivot",
				position = new Vector3(0f, -0.2f, -0.09f),
				eulerAngles = default(Vector3)
			};
			array4[num22] = pivot;
			int num23 = 1;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Steering/TruckSteeringPivot",
				position = new Vector3(0f, 0.05f, 0.22f),
				eulerAngles = new Vector3(0f, 90f, 0f)
			};
			array4[num23] = pivot;
			int num24 = 2;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Steering/MuscleSteeringPivot",
				position = new Vector3(0.2f, 0f, 0.05f),
				eulerAngles = new Vector3(0f, 90f, 90f)
			};
			array4[num24] = pivot;
			int num25 = 3;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Steering/RusckoSteeringPivot",
				position = new Vector3(0f, -0.22f, -0.1f),
				eulerAngles = new Vector3(0f, 350f, 70f)
			};
			array4[num25] = pivot;
			int num26 = 4;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "Dashboard/Steering/CarSteeringPivot",
				position = new Vector3(0f, 0.22f, 0.82f),
				eulerAngles = new Vector3(10f, 190f, 0f)
			};
			array4[num26] = pivot;
			int num27 = 5;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Steering/TractorSteeringPivot/valmet_steering",
				position = new Vector3(0.2f, 0f, 0f),
				eulerAngles = new Vector3(0f, 0f, 90f)
			};
			array4[num27] = pivot;
			int num28 = 6;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Suspension/Steering/SteeringPivot/Column",
				position = new Vector3(0.03f, -0.3f, 0.41f),
				eulerAngles = new Vector3(3.585849E-05f, 320.0001f, 80.00005f)
			};
			array4[num28] = pivot;
			FsmNetVehicle.steeringWheels = array4;
			FsmNetVehicle.Pivot[] array5 = new FsmNetVehicle.Pivot[7];
			int num29 = 0;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/GearShifter/lever",
				position = new Vector3(-0.02f, 0.21f, -0.02f),
				eulerAngles = default(Vector3)
			};
			array5[num29] = pivot;
			int num30 = 1;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/GearLever/Pivot/Lever",
				position = new Vector3(0f, -0.05f, 0.34f),
				eulerAngles = new Vector3(310f, 0f, 190f)
			};
			array5[num30] = pivot;
			int num31 = 2;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/GearShifter/Pivot/muscle_gear_lever",
				position = new Vector3(-0.07f, 0.08f, 0.06f),
				eulerAngles = new Vector3(30f, 80f, 110f)
			};
			array5[num31] = pivot;
			int num32 = 3;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/GearLever/Vibration/Pivot/lever",
				position = new Vector3(-0.01f, -0.14f, 0.39f),
				eulerAngles = new Vector3(280f, 0f, 180f)
			};
			array5[num32] = pivot;
			int num33 = 4;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "Dashboard/gear stick(xxxxx)/GearLever/Pivot/Lever/gear_stick",
				position = new Vector3(-0.05f, 0.2f, 0.2f),
				eulerAngles = new Vector3(340f, 120f, 70f),
				path_alt = "Dashboard/center console gt(xxxxx)/GearLever/Pivot/Lever/gear_stick",
				position_alt = new Vector3(-0.05f, 0.2f, 0.2f),
				eulerAngles_alt = new Vector3(340f, 120f, 70f)
			};
			array5[num33] = pivot;
			int num34 = 5;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Dashboard/Gear/Lever/tractor_lever_gear",
				position = new Vector3(0.12f, 0.14f, 0.21f),
				eulerAngles = new Vector3(0f, 90f, 90f)
			};
			array5[num34] = pivot;
			int num35 = 6;
			pivot = new FsmNetVehicle.Pivot
			{
				path = "LOD/Suspension/Steering/SteeringPivot/Throttle",
				position = new Vector3(0.06999999f, 0.01999998f, 0f),
				eulerAngles = new Vector3(0.0001466356f, 10.00063f, 340.0001f)
			};
			array5[num35] = pivot;
			FsmNetVehicle.gearSticks = array5;
			FsmNetVehicle.Pivot[] array6 = new FsmNetVehicle.Pivot[7];
			int num36 = 0;
			pivot = new FsmNetVehicle.Pivot
			{
				position = new Vector3(-0.4f, 0.93f, 0.99f),
				eulerAngles = default(Vector3)
			};
			array6[num36] = pivot;
			int num37 = 1;
			pivot = new FsmNetVehicle.Pivot
			{
				position = new Vector3(-0.75f, 1.84f, 2.74f),
				eulerAngles = default(Vector3)
			};
			array6[num37] = pivot;
			int num38 = 2;
			pivot = new FsmNetVehicle.Pivot
			{
				position = new Vector3(-0.4f, 0.53f, -0.05f),
				eulerAngles = default(Vector3)
			};
			array6[num38] = pivot;
			int num39 = 3;
			pivot = new FsmNetVehicle.Pivot
			{
				position = new Vector3(-0.29f, 0.37f, -0.08f),
				eulerAngles = default(Vector3)
			};
			array6[num39] = pivot;
			int num40 = 4;
			pivot = new FsmNetVehicle.Pivot
			{
				position = new Vector3(-0.25f, 0.2f, 0f),
				eulerAngles = default(Vector3)
			};
			array6[num40] = pivot;
			int num41 = 5;
			pivot = new FsmNetVehicle.Pivot
			{
				position = new Vector3(0f, 1.31f, -0.6f),
				eulerAngles = default(Vector3)
			};
			array6[num41] = pivot;
			int num42 = 6;
			pivot = new FsmNetVehicle.Pivot
			{
				position = new Vector3(0.02f, 0.66f, -0.44f),
				eulerAngles = default(Vector3)
			};
			array6[num42] = pivot;
			FsmNetVehicle.drivingModes = array6;
		}

		// Token: 0x0400029D RID: 669
		public Transform transform;

		// Token: 0x0400029E RID: 670
		public NetVehicle netVehicle;

		// Token: 0x0400029F RID: 671
		public List<FsmVehicleDoor> vehicleDoors = new List<FsmVehicleDoor>();

		// Token: 0x040002A0 RID: 672
		public List<FsmDashboardButton> dashboardButtons = new List<FsmDashboardButton>();

		// Token: 0x040002A1 RID: 673
		public List<FsmDashboardLever> dashboardLevers = new List<FsmDashboardLever>();

		// Token: 0x040002A2 RID: 674
		public List<FsmDashboardKnob> dashboardKnobs = new List<FsmDashboardKnob>();

		// Token: 0x040002A3 RID: 675
		public List<FsmTurnSignals> turnSignals = new List<FsmTurnSignals>();

		// Token: 0x040002A4 RID: 676
		internal static string[] carNames = new string[] { "HAYOSIKO(1500kg, 250)", "GIFU(750/450psi)", "FERNDALE(1630kg)", "RCO_RUSCKO12(270)", "SATSUMA(557kg, 248)", "KEKMET(350-400psi)", "JONNEZ ES(Clone)" };

		// Token: 0x040002A5 RID: 677
		internal static Vector3[] itemColliderCenter = new Vector3[]
		{
			default(Vector3),
			new Vector3(0f, 2f, 3f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -0.3f),
			default(Vector3),
			new Vector3(0f, 1f, -2.2f),
			new Vector3(0f, 0.4f, -0.3f)
		};

		// Token: 0x040002A6 RID: 678
		internal static float[] itemColliderRadius = new float[] { 2.4f, 1.7f, 2f, 2f, 2f, 5.5f, 0.5f };

		// Token: 0x040002A7 RID: 679
		internal static Vector3[][][] carPassengerSeats = new Vector3[][][]
		{
			new Vector3[][]
			{
				new Vector3[]
				{
					new Vector3(0.4364214f, 0.8763284f, 0.7790703f),
					new Vector3(0.3336831f, 1.290549f, 0.7909793f)
				},
				new Vector3[]
				{
					new Vector3(0.0364214f, 0.8763284f, 0.7790703f),
					new Vector3(0.3336831f, 1.290549f, 0.7909793f)
				}
			},
			new Vector3[][] { new Vector3[]
			{
				new Vector3(0.696f, 1.759f, 2.831f),
				new Vector3(0.6999686f, 1.290549f, 0.7909793f)
			} },
			new Vector3[][]
			{
				new Vector3[]
				{
					new Vector3(0.513f, 0.669f, -0.259f),
					new Vector3(0.3940828f, 0.8053533f, -0.2172538f)
				},
				new Vector3[]
				{
					new Vector3(-0.513f, 0.669f, -0.9589999f),
					new Vector3(-0.3940828f, 0.8053533f, -1.1f)
				},
				new Vector3[]
				{
					new Vector3(0f, 0.669f, -0.9589999f),
					new Vector3(0f, 0.8053533f, -1.1f)
				},
				new Vector3[]
				{
					new Vector3(0.513f, 0.669f, -0.9589999f),
					new Vector3(0.3940828f, 0.8053533f, -1.1f)
				}
			},
			new Vector3[][] { new Vector3[]
			{
				new Vector3(0.26f, 0.325f, -0.087f),
				new Vector3(0.2239742f, 0.8545685f, -0.3627806f)
			} },
			new Vector3[][]
			{
				new Vector3[]
				{
					new Vector3(0.282f, 0.30727938f, -0.0671216f),
					new Vector3(0.3000017f, 0.5315849f, 0.01975246f)
				},
				new Vector3[]
				{
					new Vector3(-0.282f, 0.30727938f, -0.5671216f),
					new Vector3(-0.3000017f, 0.5315849f, -0.48024753f)
				},
				new Vector3[]
				{
					new Vector3(0.282f, 0.30727938f, -0.5671216f),
					new Vector3(0.3000017f, 0.5315849f, -0.48024753f)
				}
			},
			new Vector3[0][],
			new Vector3[0][]
		};

		// Token: 0x040002A8 RID: 680
		internal static readonly FsmNetVehicle.Pivot[] throttlePedals;

		// Token: 0x040002A9 RID: 681
		internal static readonly FsmNetVehicle.Pivot[] brakePedals;

		// Token: 0x040002AA RID: 682
		internal static readonly FsmNetVehicle.Pivot[] clutchPedals;

		// Token: 0x040002AB RID: 683
		internal static readonly FsmNetVehicle.Pivot[] steeringWheels;

		// Token: 0x040002AC RID: 684
		internal static readonly FsmNetVehicle.Pivot[] gearSticks;

		// Token: 0x040002AD RID: 685
		internal static readonly FsmNetVehicle.Pivot[] drivingModes;

		// Token: 0x040002AE RID: 686
		public FsmFloat fuelLevel;

		// Token: 0x040002AF RID: 687
		public FsmFloat engineTemp;

		// Token: 0x040002B0 RID: 688
		public FsmFloat oilLevel;

		// Token: 0x040002B1 RID: 689
		public FsmFloat oilContamination;

		// Token: 0x040002B2 RID: 690
		public FsmFloat oilGrade;

		// Token: 0x040002B3 RID: 691
		public FsmFloat coolant1Level;

		// Token: 0x040002B4 RID: 692
		public FsmFloat coolant2Level;

		// Token: 0x040002B5 RID: 693
		public FsmFloat brake1Level;

		// Token: 0x040002B6 RID: 694
		public FsmFloat brake2Level;

		// Token: 0x040002B7 RID: 695
		public FsmFloat clutchLevel;

		// Token: 0x040002B8 RID: 696
		public FsmIgnition ignition;

		// Token: 0x040002B9 RID: 697
		public FsmStarter starter;

		// Token: 0x02000111 RID: 273
		internal struct Pivot
		{
			// Token: 0x0600057A RID: 1402 RVA: 0x0001F3DC File Offset: 0x0001D5DC
			public FsmNetVehicle.Pivot GetAltPivot()
			{
				return new FsmNetVehicle.Pivot
				{
					path = this.path_alt,
					position = this.position_alt,
					eulerAngles = this.eulerAngles_alt
				};
			}

			// Token: 0x04000532 RID: 1330
			public string path;

			// Token: 0x04000533 RID: 1331
			public Vector3 position;

			// Token: 0x04000534 RID: 1332
			public Vector3 eulerAngles;

			// Token: 0x04000535 RID: 1333
			public string path_alt;

			// Token: 0x04000536 RID: 1334
			public Vector3 position_alt;

			// Token: 0x04000537 RID: 1335
			public Vector3 eulerAngles_alt;
		}

		// Token: 0x02000112 RID: 274
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600057B RID: 1403 RVA: 0x0001F419 File Offset: 0x0001D619
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600057C RID: 1404 RVA: 0x0001F425 File Offset: 0x0001D625
			public <>c()
			{
			}

			// Token: 0x0600057D RID: 1405 RVA: 0x0001F42D File Offset: 0x0001D62D
			internal bool <DoFluidsAndFields>b__33_0(FsmFloat f)
			{
				return f.Name.Contains("EngineTemp");
			}

			// Token: 0x0600057E RID: 1406 RVA: 0x0001F440 File Offset: 0x0001D640
			internal bool <DoFsms>b__43_0(PlayMakerFSM fsm)
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

			// Token: 0x0600057F RID: 1407 RVA: 0x0001F4D9 File Offset: 0x0001D6D9
			internal bool <DoFsms>b__43_2(FsmStateAction a)
			{
				return a is SetStringValue;
			}

			// Token: 0x04000538 RID: 1336
			public static readonly FsmNetVehicle.<>c <>9 = new FsmNetVehicle.<>c();

			// Token: 0x04000539 RID: 1337
			public static Func<FsmFloat, bool> <>9__33_0;

			// Token: 0x0400053A RID: 1338
			public static Func<FsmStateAction, bool> <>9__43_2;

			// Token: 0x0400053B RID: 1339
			public static Func<PlayMakerFSM, bool> <>9__43_0;
		}

		// Token: 0x02000113 RID: 275
		[CompilerGenerated]
		private sealed class <>c__DisplayClass35_0
		{
			// Token: 0x06000580 RID: 1408 RVA: 0x0001F4E4 File Offset: 0x0001D6E4
			public <>c__DisplayClass35_0()
			{
			}

			// Token: 0x06000581 RID: 1409 RVA: 0x0001F4EC File Offset: 0x0001D6EC
			internal bool <OnCarFluidsAndFields>b__0(FsmNetVehicle v)
			{
				return v.netVehicle.hash == this.hash;
			}

			// Token: 0x0400053C RID: 1340
			public int hash;
		}

		// Token: 0x02000114 RID: 276
		[CompilerGenerated]
		private sealed class <>c__DisplayClass43_0
		{
			// Token: 0x06000582 RID: 1410 RVA: 0x0001F501 File Offset: 0x0001D701
			public <>c__DisplayClass43_0()
			{
			}

			// Token: 0x06000583 RID: 1411 RVA: 0x0001F509 File Offset: 0x0001D709
			internal void <DoFsms>b__1()
			{
				if (this.<>4__this.netVehicle.driverSeatTaken)
				{
					this.drivingMode.SendEvent("FINISHED");
				}
			}

			// Token: 0x0400053D RID: 1341
			public FsmNetVehicle <>4__this;

			// Token: 0x0400053E RID: 1342
			public PlayMakerFSM drivingMode;
		}

		// Token: 0x02000115 RID: 277
		[CompilerGenerated]
		private sealed class <>c__DisplayClass45_0
		{
			// Token: 0x06000584 RID: 1412 RVA: 0x0001F52D File Offset: 0x0001D72D
			public <>c__DisplayClass45_0()
			{
			}

			// Token: 0x06000585 RID: 1413 RVA: 0x0001F535 File Offset: 0x0001D735
			internal void <DoFlatbedPassengerSeats>b__0()
			{
				this.enterPassenger(true);
			}

			// Token: 0x06000586 RID: 1414 RVA: 0x0001F543 File Offset: 0x0001D743
			internal void <DoFlatbedPassengerSeats>b__1()
			{
				this.enterPassenger(false);
			}

			// Token: 0x0400053F RID: 1343
			public Action<bool> enterPassenger;
		}
	}
}
