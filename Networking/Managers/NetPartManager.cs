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
	// Token: 0x0200005C RID: 92
	[ManagerCreate(10)]
	internal class NetPartManager : MonoBehaviour
	{
		// Token: 0x06000237 RID: 567 RVA: 0x0000D694 File Offset: 0x0000B894
		private void Start()
		{
			NetEvent<NetPartManager>.Register("PartAssemble", new NetEventHandler(this.OnAssemble));
			NetEvent<NetPartManager>.Register("PartRemove", new NetEventHandler(this.OnRemove));
			NetEvent<NetPartManager>.Register("InitAssemble", new NetEventHandler(this.OnInitSyncAssemble));
			NetEvent<NetPartManager>.Register("Screw", new NetEventHandler(this.OnTightnessChange));
			NetEvent<NetPartManager>.Register("InitScrew", new NetEventHandler(this.OnInitBolts));
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				this.InitSyncAssemble(user);
				this.InitSyncBolts(user);
			};
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000D730 File Offset: 0x0000B930
		internal static void AddAssembleFsm(int hash, PlayMakerFSM fsm)
		{
			if (NetPartManager.assemblesHashes.Contains(hash))
			{
				Console.LogError(string.Format("<b>FATAL ERROR!</b> FSM Hash {0} of FSM '{1}' on path '{2}' already exists!", hash, fsm.FsmName, fsm.transform.GetPath()), false);
				return;
			}
			NetPartManager.assembles.Add(fsm);
			NetPartManager.assemblesHashes.Add(hash);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000D788 File Offset: 0x0000B988
		internal static bool AddBolt(PlayMakerFSM fsm, int hash)
		{
			NetPartManager.Bolt bolt = new NetPartManager.Bolt(fsm, hash, new Action<int, byte>(NetPartManager.TightnessChangeEvent));
			if (bolt.stage == null && bolt.alignment == null && bolt.rot == null)
			{
				return false;
			}
			NetPartManager.bolts.Add(bolt);
			return true;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000D7D0 File Offset: 0x0000B9D0
		private void OnInitSyncAssemble(ulong sender, Packet packet)
		{
			while (packet.UnreadLength() > 0)
			{
				int num = packet.ReadInt(true);
				int num2 = packet.ReadInt(true);
				int num3 = NetRigidbodyManager.rigidbodyHashes.IndexOf(num);
				if (num3 == -1)
				{
					Console.LogError(string.Format("NetRigidbodyManager.OnInitSyncAssemble(ulong sender, Packet packet): the item hash {0} does not exist", num), false);
				}
				else
				{
					NetRigidbodyManager.OwnedRigidbody ownedRigidbody = NetRigidbodyManager.ownedRigidbodies[num3];
					PlayMakerFSM playMakerFSM = null;
					if (num2 != 0)
					{
						int num4 = NetPartManager.assemblesHashes.IndexOf(num2);
						if (num4 == -1)
						{
							Console.LogError(string.Format("NetRigidbodyManager.OnInitSyncAssemble(ulong sender, Packet packet): the assemble hash {0} does not exist", num2), false);
							continue;
						}
						playMakerFSM = NetPartManager.assembles[num4];
					}
					Console.Log(string.Concat(new string[]
					{
						"InitAssemble: ",
						(playMakerFSM == null) ? "null" : playMakerFSM.transform.name,
						", ",
						(ownedRigidbody.assemble == null) ? "null" : ownedRigidbody.assemble.transform.name,
						", ",
						ownedRigidbody.transform.name
					}), false);
					if (playMakerFSM != ownedRigidbody.assemble)
					{
						if (playMakerFSM == null)
						{
							this.Remove(num);
						}
						else
						{
							this.Assemble(num2, num);
						}
					}
				}
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000D920 File Offset: 0x0000BB20
		private void InitSyncAssemble(ulong user)
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			using (Packet packet = new Packet(1))
			{
				for (int i = 0; i < NetRigidbodyManager.ownedRigidbodies.Count; i++)
				{
					Console.Log("Init assemble send: " + NetRigidbodyManager.ownedRigidbodies[i].transform.name, false);
					packet.Write(NetRigidbodyManager.rigidbodyHashes[i], -1);
					int num = 0;
					if (NetRigidbodyManager.ownedRigidbodies[i].assemble != null)
					{
						num = NetPartManager.assemblesHashes[NetPartManager.assembles.IndexOf(NetRigidbodyManager.ownedRigidbodies[i].assemble)];
					}
					packet.Write(num, -1);
				}
				NetEvent<NetPartManager>.Send("InitAssemble", packet, user, true);
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000DA00 File Offset: 0x0000BC00
		private void OnDestroy()
		{
			NetPartManager.assemblesHashes.Clear();
			NetPartManager.assembles.Clear();
			NetPartManager.bolts.Clear();
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000DA20 File Offset: 0x0000BC20
		private void OnAssemble(ulong user, Packet p)
		{
			int num = p.ReadInt(true);
			int num2 = p.ReadInt(true);
			this.Assemble(num, num2);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000DA48 File Offset: 0x0000BC48
		private void OnRemove(ulong user, Packet p)
		{
			int num = p.ReadInt(true);
			this.Remove(num);
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000DA64 File Offset: 0x0000BC64
		private static void SendAssembleEvent(int fsmHash, FsmGameObject part)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(fsmHash, -1);
				if (part != null)
				{
					if (part.Value == null)
					{
						Console.LogError("NetRigidbodyManager.SendAssembeEvent: Attached gameobject is null!", false);
						return;
					}
					int num = NetRigidbodyManager.ownedRigidbodies.FindIndex((NetRigidbodyManager.OwnedRigidbody r) => r.Rigidbody == part.Value.GetComponent<Rigidbody>());
					if (num == -1)
					{
						Console.LogError("NetRigidbodyManager.SendAssembeEvent: Attached gameobject '" + part.Value.transform.GetPath() + "' does not have a hash!", false);
						return;
					}
					NetRigidbodyManager.ownedRigidbodies[num].assemble = NetPartManager.assembles[NetPartManager.assemblesHashes.IndexOf(fsmHash)];
					packet.Write(NetRigidbodyManager.rigidbodyHashes[num], -1);
				}
				else
				{
					packet.Write(0, -1);
				}
				NetEvent<NetPartManager>.Send("PartAssemble", packet, true);
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000DB6C File Offset: 0x0000BD6C
		private static void SendRemoveEvent(int fsmHash)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(fsmHash, -1);
				NetEvent<NetPartManager>.Send("PartRemove", packet, true);
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000DBB0 File Offset: 0x0000BDB0
		internal static void SetupRemovalPlaymaker(PlayMakerFSM fsm, int hash)
		{
			try
			{
				fsm.Initialize();
				NetPartManager.removesHashes.Add(hash);
				NetPartManager.removes.Add(fsm);
				FsmTransition fsmTransition = fsm.FsmStates[0].Transitions.FirstOrDefault((FsmTransition t) => t.EventName.Contains("REMOV"));
				string text;
				if (fsmTransition == null)
				{
					text = "Remove part";
				}
				else
				{
					text = fsmTransition.ToState;
				}
				FsmEvent fsmEvent = fsm.FsmEvents.FirstOrDefault((FsmEvent e) => e.Name == "MP_REMOVE");
				if (fsmEvent == null)
				{
					FsmEvent[] array = new FsmEvent[fsm.FsmEvents.Length + 1];
					fsm.FsmEvents.CopyTo(array, 0);
					fsmEvent = new FsmEvent("MP_REMOVE");
					FsmEvent.AddFsmEvent(fsmEvent);
					array[array.Length - 1] = fsmEvent;
					fsm.Fsm.Events = array;
				}
				FsmTransition[] array2 = new FsmTransition[fsm.FsmGlobalTransitions.Length + 1];
				fsm.FsmGlobalTransitions.CopyTo(array2, 0);
				array2[array2.Length - 1] = new FsmTransition
				{
					FsmEvent = fsmEvent,
					ToState = text
				};
				fsm.Fsm.GlobalTransitions = array2;
				fsm.InsertAction(text, delegate
				{
					NetPartManager.RemoveEvent(hash);
				}, fsm.GetState(text).Actions.Length - 1);
				fsm.Initialize();
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("NetAttachmentManager.SetupPlaymaker(PlaymakerFSM): fsm {0} with hash {1} ({2}) failed with exception {3}", new object[]
				{
					fsm.FsmName,
					hash,
					fsm.transform.name,
					ex
				}), false);
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000DD80 File Offset: 0x0000BF80
		internal static void SetupAssemblePlaymaker(PlayMakerFSM fsm, int hash)
		{
			try
			{
				fsm.Initialize();
				string fsmName = fsm.FsmName;
				FsmTransition[] array2;
				if (!(fsmName == "Assembly"))
				{
					if (!(fsmName == "Assemble"))
					{
						goto IL_442;
					}
				}
				else
				{
					if (fsm.transform.name == "Insert")
					{
						FsmEvent[] array = new FsmEvent[fsm.FsmEvents.Length + 1];
						fsm.FsmEvents.CopyTo(array, 0);
						array[array.Length - 1] = new FsmEvent("MP_ASSEMBLE");
						FsmEvent.AddFsmEvent(array[array.Length - 1]);
						fsm.Fsm.Events = array;
						array2 = new FsmTransition[fsm.FsmGlobalTransitions.Length + 1];
						fsm.FsmGlobalTransitions.CopyTo(array2, 0);
						FsmTransition[] array3 = array2;
						int num = array2.Length - 1;
						FsmTransition fsmTransition = new FsmTransition();
						fsmTransition.FsmEvent = fsm.FsmEvents.First((FsmEvent e) => e.Name == "MP_ASSEMBLE");
						fsmTransition.ToState = "Add battery";
						array3[num] = fsmTransition;
						fsm.Fsm.GlobalTransitions = array2;
						fsm.InsertAction("Add battery", delegate
						{
							NetPartManager.BatteryToRadioOrFlashlightEvent(hash, fsm.FsmVariables.FindFsmGameObject("Part"));
						}, -1);
						goto IL_442;
					}
					if (fsm.transform.name == "TriggerCharger")
					{
						fsm.InsertAction("Init", delegate
						{
							NetPartManager.BatteryOnChargerEvent(hash, fsm.FsmVariables.FindFsmGameObject("Battery"));
						}, -1);
						goto IL_442;
					}
				}
				FsmTransition fsmTransition2 = fsm.FsmStates[0].Transitions.FirstOrDefault((FsmTransition t) => t.EventName.Contains("ASSEMBL"));
				bool flag = fsm.FsmGlobalTransitions.Any((FsmTransition t) => t.EventName == "RESETWIRING");
				string text;
				if (flag)
				{
					text = "Sound";
					NetPartManager.FixWiringFsm(fsm);
				}
				else if (fsmTransition2 == null)
				{
					FsmState fsmState = fsm.FsmStates.FirstOrDefault((FsmState s) => s.Name.ToLower().Contains("assemble"));
					if (fsmState == null)
					{
						Console.LogError(string.Format("NetAttachmentManager.SetupPlaymaker(PlaymakerFSM): fsm {0} with hash {1} ({2}) failed because there was no state 'assemble' nor event 'ASSEMBLE'", fsm.FsmName, hash, fsm.transform.name), false);
						return;
					}
					text = fsmState.Name;
				}
				else
				{
					text = fsmTransition2.ToState;
				}
				FsmEvent fsmEvent = fsm.FsmEvents.FirstOrDefault((FsmEvent e) => e.Name == "MP_ASSEMBLE");
				if (fsmEvent == null)
				{
					FsmEvent[] array4 = new FsmEvent[fsm.FsmEvents.Length + 1];
					fsm.FsmEvents.CopyTo(array4, 0);
					fsmEvent = new FsmEvent("MP_ASSEMBLE");
					FsmEvent.AddFsmEvent(fsmEvent);
					array4[array4.Length - 1] = fsmEvent;
					fsm.Fsm.Events = array4;
				}
				array2 = new FsmTransition[fsm.FsmGlobalTransitions.Length + 1];
				fsm.FsmGlobalTransitions.CopyTo(array2, 0);
				array2[array2.Length - 1] = new FsmTransition
				{
					FsmEvent = fsmEvent,
					ToState = text
				};
				fsm.Fsm.GlobalTransitions = array2;
				FsmState[] array5 = fsm.FsmStates.Where((FsmState s) => s.Name.ToLower().Contains("assemble")).ToArray<FsmState>();
				if (flag || array5.Length == 0)
				{
					fsm.InsertAction(text, delegate
					{
						NetPartManager.AssembleEvent(hash);
					}, -1);
				}
				else
				{
					Action <>9__9;
					for (int i = 0; i < array5.Length; i++)
					{
						PlayMakerFSM fsm2 = fsm;
						string name = array5[i].Name;
						Action action;
						if ((action = <>9__9) == null)
						{
							action = (<>9__9 = delegate
							{
								NetPartManager.AssembleEvent(hash);
							});
						}
						fsm2.InsertAction(name, action, -1);
					}
				}
				IL_442:
				fsm.Initialize();
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("NetAttachmentManager.SetupPlaymaker(PlaymakerFSM): fsm {0} with hash {1} ({2}) failed with exception {3}", new object[]
				{
					fsm.FsmName,
					hash,
					fsm.transform.name,
					ex
				}), false);
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000E248 File Offset: 0x0000C448
		private static void FixWiringFsm(PlayMakerFSM fsm)
		{
			FsmFloat dist = fsm.FsmVariables.FindFsmFloat("Distance");
			FsmFloat tol = fsm.FsmVariables.FindFsmFloat("Tolerance");
			FsmState state = fsm.GetState("State 1");
			state.Actions[state.Actions.ToList<FsmStateAction>().FindIndex((FsmStateAction a) => a is FloatCompare)] = new PlayMakerUtilities.PM_Hook(delegate
			{
				if (dist.Value < tol.Value && NetPartManager.wiringMess.transform.root.name == "PLAYER")
				{
					fsm.SendEvent("ASSEMBLE");
				}
			}, true);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000E2F4 File Offset: 0x0000C4F4
		private static void AssembleEvent(int fsmHash)
		{
			try
			{
				PlayMakerFSM playMakerFSM = NetPartManager.assembles[NetPartManager.assemblesHashes.IndexOf(fsmHash)];
				Console.Log(string.Format("Attach event triggered: {0}, {1}", fsmHash, playMakerFSM.transform.name), false);
				SatsumaProfiler.Instance.attached.Add(playMakerFSM.transform.name);
				FsmGameObject fsmGameObject = playMakerFSM.FsmVariables.FindFsmGameObject("Part");
				if (NetPartManager.updatingFsms.Contains(playMakerFSM))
				{
					NetPartManager.updatingFsms.Remove(playMakerFSM);
				}
				else
				{
					NetPartManager.SendAssembleEvent(fsmHash, fsmGameObject);
				}
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("Error in AssembleEvent: {0}, {1}, {2}", ex.GetType(), ex.Message, ex.StackTrace), false);
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000E3BC File Offset: 0x0000C5BC
		private static void RemoveEvent(int fsmHash)
		{
			try
			{
				int num = NetRigidbodyManager.rigidbodyHashes.IndexOf(fsmHash);
				NetRigidbodyManager.OwnedRigidbody ownedRigidbody;
				if (num == -1)
				{
					int num2 = NetPartManager.removesHashes.IndexOf(fsmHash);
					PlayMakerFSM playMakerFSM = NetPartManager.removes[num2];
					FsmObject fsmObject = playMakerFSM.FsmVariables.FindFsmObject("Rigidbody");
					ownedRigidbody = NetRigidbodyManager.AddRigidbody(fsmObject.Value as Rigidbody, fsmHash);
					ownedRigidbody.remove = playMakerFSM;
					ownedRigidbody.Removal_Rigidbody = fsmObject;
					num = NetRigidbodyManager.rigidbodyHashes.IndexOf(fsmHash);
				}
				else
				{
					ownedRigidbody = NetRigidbodyManager.ownedRigidbodies[num];
				}
				NetRigidbodyManager.RequestOwnership(ownedRigidbody);
				ownedRigidbody.assemble = null;
				PlayMakerFSM remove = ownedRigidbody.remove;
				SatsumaProfiler.Instance.detached.Add(remove.transform.name);
				if (NetPartManager.updatingFsms.Contains(remove))
				{
					NetPartManager.updatingFsms.Remove(remove);
				}
				else
				{
					NetPartManager.SendRemoveEvent(fsmHash);
				}
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("Error in RemoveEvent: {0}, {1}, {2}", ex.GetType(), ex.Message, ex.StackTrace), false);
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000E4CC File Offset: 0x0000C6CC
		private static void BatteryOnChargerEvent(int fsmHash, FsmGameObject battery)
		{
			try
			{
				Console.Log(string.Format("Battery attached to charger: {0}", battery.Value.transform.GetPath().GetHashCode()), false);
				PlayMakerFSM playMakerFSM = NetPartManager.assembles[NetPartManager.assemblesHashes.IndexOf(fsmHash)];
				if (NetPartManager.updatingFsms.Contains(playMakerFSM))
				{
					NetPartManager.updatingFsms.Remove(playMakerFSM);
				}
				else
				{
					NetPartManager.SendAssembleEvent(fsmHash, battery);
				}
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("Error in AssembleEvent: {0}, {1}, {2}", ex.GetType(), ex.Message, ex.StackTrace), false);
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000E574 File Offset: 0x0000C774
		private static void BatteryToRadioOrFlashlightEvent(int fsmHash, FsmGameObject battery)
		{
			try
			{
				PlayMakerFSM playMakerFSM = NetPartManager.assembles[NetPartManager.assemblesHashes.IndexOf(fsmHash)];
				Console.Log(string.Format("Battery added to radio or flashlight: {0}, {1}, batt: {2}", fsmHash, playMakerFSM.transform.name, battery.Value.transform.GetPath().GetHashCode()), false);
				if (NetPartManager.updatingFsms.Contains(playMakerFSM))
				{
					NetPartManager.updatingFsms.Remove(playMakerFSM);
				}
				else
				{
					NetPartManager.SendAssembleEvent(fsmHash, battery);
				}
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("Error in AssembleEvent: {0}, {1}, {2}", ex.GetType(), ex.Message, ex.StackTrace), false);
			}
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000E62C File Offset: 0x0000C82C
		private void Assemble(int fsmHash, int partHash)
		{
			int num = NetPartManager.assemblesHashes.IndexOf(fsmHash);
			if (num == -1)
			{
				return;
			}
			PlayMakerFSM playMakerFSM = NetPartManager.assembles[num];
			NetPartManager.updatingFsms.Add(playMakerFSM);
			if (partHash == 0)
			{
				if (!playMakerFSM.FsmGlobalTransitions.Any((FsmTransition t) => t.EventName == "RESETWIRING"))
				{
					Console.LogError(string.Format("Received assemble event for fsm {0} ({1}) but the part hash is 0 and the fsm doesn't look like a wiring fsm", fsmHash, playMakerFSM.transform.name), false);
				}
				playMakerFSM.Fsm.Event(playMakerFSM.FsmEvents.First((FsmEvent e) => e.Name == "MP_ASSEMBLE"));
				return;
			}
			num = NetRigidbodyManager.rigidbodyHashes.IndexOf(partHash);
			if (num == -1)
			{
				return;
			}
			NetRigidbodyManager.OwnedRigidbody ownedRigidbody = NetRigidbodyManager.ownedRigidbodies[num];
			ownedRigidbody.assemble = playMakerFSM;
			GameObject gameObject = ownedRigidbody.Rigidbody.gameObject;
			if (playMakerFSM.transform.name == "TriggerCharger")
			{
				playMakerFSM.FsmVariables.FindFsmGameObject("Battery").Value = gameObject;
				playMakerFSM.Fsm.Event(playMakerFSM.FsmEvents.First((FsmEvent e) => e.Name == "PLACEBATT"));
				return;
			}
			playMakerFSM.FsmVariables.FindFsmGameObject("Part").Value = gameObject;
			playMakerFSM.Fsm.Event(playMakerFSM.FsmEvents.First((FsmEvent e) => e.Name == "MP_ASSEMBLE"));
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000E7C8 File Offset: 0x0000C9C8
		private void Remove(int fsmHash)
		{
			int num = NetRigidbodyManager.rigidbodyHashes.IndexOf(fsmHash);
			NetRigidbodyManager.OwnedRigidbody ownedRigidbody;
			if (num == -1)
			{
				int num2 = NetPartManager.removesHashes.IndexOf(fsmHash);
				PlayMakerFSM playMakerFSM = NetPartManager.removes[num2];
				ownedRigidbody = NetRigidbodyManager.AddRigidbody(playMakerFSM.GetComponent<Rigidbody>(), fsmHash);
				ownedRigidbody.remove = playMakerFSM;
				ownedRigidbody.Removal_Rigidbody = playMakerFSM.FsmVariables.FindFsmObject("Rigidbody");
				num = NetRigidbodyManager.rigidbodyHashes.IndexOf(fsmHash);
			}
			else
			{
				ownedRigidbody = NetRigidbodyManager.ownedRigidbodies[num];
			}
			PlayMakerFSM remove = ownedRigidbody.remove;
			remove.Fsm.Event(remove.FsmEvents.First((FsmEvent e) => e.Name == "MP_REMOVE"));
			if (num == -1)
			{
				return;
			}
			NetRigidbodyManager.RequestOwnership(ownedRigidbody);
			ownedRigidbody.assemble = null;
			NetPartManager.updatingFsms.Add(remove);
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000E8A0 File Offset: 0x0000CAA0
		private static void TightnessChangeEvent(int boltHash, byte stage)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(boltHash, -1);
				packet.Write(stage, -1);
				NetEvent<NetPartManager>.Send("Screw", packet, true);
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000E8EC File Offset: 0x0000CAEC
		private void OnTightnessChange(ulong sender, Packet packet)
		{
			int boltHash = packet.ReadInt(true);
			byte b2 = packet.ReadByte(true);
			NetPartManager.Bolt bolt = NetPartManager.bolts.FirstOrDefault((NetPartManager.Bolt b) => b.hash == boltHash);
			if (bolt == null)
			{
				Console.LogError(string.Format("The bolt with hash {0} could not be found", boltHash), false);
				return;
			}
			bolt.SetTightness(b2);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000E954 File Offset: 0x0000CB54
		private void InitSyncBolts(ulong target)
		{
			if (!BeerMPGlobals.IsHost)
			{
				return;
			}
			using (Packet packet = new Packet(1))
			{
				for (int i = 0; i < NetPartManager.bolts.Count; i++)
				{
					packet.Write(NetPartManager.bolts[i].hash, -1);
					packet.Write(NetPartManager.bolts[i].ScrewSyncStage, -1);
				}
				NetEvent<NetPartManager>.Send("InitScrew", packet, target, true);
			}
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000E9DC File Offset: 0x0000CBDC
		private void OnInitBolts(ulong sender, Packet p)
		{
			while (p.UnreadLength() > 0)
			{
				int boltHash = p.ReadInt(true);
				byte b2 = p.ReadByte(true);
				NetPartManager.Bolt bolt = NetPartManager.bolts.FirstOrDefault((NetPartManager.Bolt b) => b.hash == boltHash);
				if (bolt == null)
				{
					Console.LogError(string.Format("The bolt with hash {0} could not be found", boltHash), false);
				}
				else
				{
					bolt.SetTightness(b2);
				}
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000EA4D File Offset: 0x0000CC4D
		public NetPartManager()
		{
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000EA55 File Offset: 0x0000CC55
		// Note: this type is marked as 'beforefieldinit'.
		static NetPartManager()
		{
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000EA93 File Offset: 0x0000CC93
		[CompilerGenerated]
		private void <Start>b__12_0(ulong user)
		{
			this.InitSyncAssemble(user);
			this.InitSyncBolts(user);
		}

		// Token: 0x0400022B RID: 555
		internal static List<int> assemblesHashes = new List<int>();

		// Token: 0x0400022C RID: 556
		internal static List<int> removesHashes = new List<int>();

		// Token: 0x0400022D RID: 557
		internal static List<PlayMakerFSM> removes = new List<PlayMakerFSM>();

		// Token: 0x0400022E RID: 558
		internal static List<PlayMakerFSM> assembles = new List<PlayMakerFSM>();

		// Token: 0x0400022F RID: 559
		internal static List<PlayMakerFSM> updatingFsms = new List<PlayMakerFSM>();

		// Token: 0x04000230 RID: 560
		internal static List<NetPartManager.Bolt> bolts = new List<NetPartManager.Bolt>();

		// Token: 0x04000231 RID: 561
		internal static Transform wiringMess;

		// Token: 0x04000232 RID: 562
		private const string assembleEvent = "PartAssemble";

		// Token: 0x04000233 RID: 563
		private const string removeEvent = "PartRemove";

		// Token: 0x04000234 RID: 564
		private const string screwEvent = "Screw";

		// Token: 0x04000235 RID: 565
		private const string camshaftGearEvent = "CamGear";

		// Token: 0x020000F1 RID: 241
		internal class Bolt
		{
			// Token: 0x1700003C RID: 60
			// (get) Token: 0x060004EF RID: 1263 RVA: 0x0001CABC File Offset: 0x0001ACBC
			public byte ScrewSyncStage
			{
				get
				{
					if (this.isTuneBolt)
					{
						return (byte)Mathf.RoundToInt(this.alignment.Value / this.maxAlignment * 255f);
					}
					if (this.isScrewableLid)
					{
						return (byte)Mathf.RoundToInt(this.rot.Value / 360f * 255f);
					}
					if (this.stage == null)
					{
						return (byte)this.floatstage.Value;
					}
					return (byte)this.stage.Value;
				}
			}

			// Token: 0x060004F0 RID: 1264 RVA: 0x0001CB38 File Offset: 0x0001AD38
			public Bolt(PlayMakerFSM screw, int hash, Action<int, byte> onTightnessChange)
			{
				if (NetPartManager.Bolt.raycastBolt == null)
				{
					NetPartManager.Bolt.raycastBolt = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/2Spanner/Raycast").GetPlayMaker("Raycast").FsmVariables.FindFsmGameObject("Bolt");
				}
				screw.Initialize();
				this.screw = screw;
				this.stage = screw.FsmVariables.IntVariables.FirstOrDefault((FsmInt i) => i.Name == "Stage");
				if (this.stage == null)
				{
					this.floatstage = screw.FsmVariables.FindFsmFloat("Tightness");
				}
				this.hash = hash;
				this.onTightnessChange = onTightnessChange;
				if (screw.HasState("Wait") && !screw.HasState("Wait 2") && !screw.HasState("Wait 3") && !screw.HasState("Wait 4"))
				{
					this.rotAmount = screw.FsmVariables.FindFsmFloat("ScrewAmount").Value;
					this.rot = screw.FsmVariables.FindFsmFloat("Rot");
					FsmEvent fsmEvent = screw.AddEvent("MP_UNSCREW");
					FsmEvent fsmEvent2 = screw.AddEvent("MP_SCREW");
					screw.AddGlobalTransition(fsmEvent, "Unscrew");
					screw.AddGlobalTransition(fsmEvent2, "Screw");
					screw.InsertAction("Wait", new Action(this.OnTightness), 0);
					this.isScrewableLid = true;
				}
				else if (screw.HasState("Wait") || screw.HasState("Wait 2") || screw.HasState("Wait 3") || screw.HasState("Wait 4"))
				{
					if (screw.HasState("Wait 3") || screw.HasState("Wait"))
					{
						screw.InsertAction(screw.HasState("Wait 3") ? "Wait 3" : "Wait", new Action(this.OnTightness), 0);
					}
					screw.InsertAction(screw.HasState("Wait 4") ? "Wait 4" : "Wait 2", new Action(this.OnTightness), 0);
				}
				else if (screw.HasState("Setup"))
				{
					this.alignment = screw.FsmVariables.FindFsmFloat("Alignment");
					this.maxAlignment = screw.FsmVariables.FindFsmFloat("Max").Value;
					screw.InsertAction("Setup", new Action(this.OnTightness), 0);
					FsmEvent fsmEvent3 = screw.AddEvent("MP_SETUP");
					screw.AddGlobalTransition(fsmEvent3, "Setup");
					this.isTuneBolt = true;
				}
				if (screw.gameObject.name.StartsWith("oil filter"))
				{
					FsmEvent fsmEvent4 = screw.FsmEvents.FirstOrDefault((FsmEvent e) => e.Name == "TIGHTEN");
					FsmEvent fsmEvent5 = screw.FsmEvents.FirstOrDefault((FsmEvent e) => e.Name == "UNTIGHTEN");
					if (fsmEvent4 == null || fsmEvent5 == null)
					{
						Console.LogError(string.Format("Init bolt with name oil filter but occured null: {0} {1}", fsmEvent4 == null, fsmEvent5 == null), false);
						return;
					}
					screw.AddGlobalTransition(fsmEvent4, "Screw");
					screw.AddGlobalTransition(fsmEvent5, "Unscrew");
				}
				if (screw.gameObject.name == "Pin")
				{
					this.isPin = true;
					FsmEvent fsmEvent6 = screw.FsmEvents.FirstOrDefault((FsmEvent e) => e.Name == "TIGHTEN");
					FsmEvent fsmEvent7 = screw.FsmEvents.FirstOrDefault((FsmEvent e) => e.Name == "UNTIGHTEN");
					if (fsmEvent6 == null || fsmEvent7 == null)
					{
						Console.LogError(string.Format("Init bolt with name Pin but occured null: {0} {1}", fsmEvent6 == null, fsmEvent7 == null), false);
						return;
					}
					screw.AddGlobalTransition(fsmEvent6, "1");
					screw.AddGlobalTransition(fsmEvent7, "0");
				}
				if (screw.transform.parent != null && screw.transform.parent.name == "MaskedCamshaftGear")
				{
					this.isCamshaftGear = true;
					screw.InsertAction("Rotate", new Action(NetItemsManager.CamshaftGearAdjustEvent), -1);
				}
				if (screw.transform.name.Contains("oil filter"))
				{
					Console.Log("Oil filter bolt, " + screw.transform.name + ", " + Environment.StackTrace, true);
				}
				screw.Initialize();
			}

			// Token: 0x060004F1 RID: 1265 RVA: 0x0001CFE8 File Offset: 0x0001B1E8
			private void OnTightness()
			{
				if (this.doSync && ((NetPartManager.Bolt.raycastBolt != null && NetPartManager.Bolt.raycastBolt.Value == this.screw.gameObject) || this.isPin || (!this.screw.HasState("Wait 3") && !this.screw.HasState("Wait 4"))) && this.ScrewSyncStage != this.lastScrewSyncStage)
				{
					this.lastScrewSyncStage = this.ScrewSyncStage;
					this.onTightnessChange(this.hash, this.ScrewSyncStage);
				}
				this.doSync = true;
			}

			// Token: 0x060004F2 RID: 1266 RVA: 0x0001D088 File Offset: 0x0001B288
			public void SetTightness(byte stage)
			{
				if (this.stage != null && (int)stage == this.stage.Value)
				{
					return;
				}
				this.doSync = false;
				if (this.isTuneBolt)
				{
					float num = (float)stage / 255f * this.maxAlignment;
					this.alignment.Value = num;
					this.screw.SendEvent("MP_SETUP");
					return;
				}
				if (this.isScrewableLid)
				{
					float num2 = (float)stage / 255f * 360f;
					bool flag = num2 > this.rot.Value;
					this.rot.Value = (flag ? (num2 - this.rotAmount) : (num2 + this.rotAmount));
					if (!flag && num2 < 5f)
					{
						this.rot.Value = this.rotAmount;
					}
					this.screw.SendEvent(flag ? "MP_SCREW" : "MP_UNSCREW");
					return;
				}
				bool flag2 = (int)stage > this.stage.Value;
				this.stage.Value = (int)(flag2 ? (stage - 1) : (stage + 1));
				this.screw.SendEvent(flag2 ? "TIGHTEN" : "UNTIGHTEN");
			}

			// Token: 0x04000499 RID: 1177
			internal PlayMakerFSM screw;

			// Token: 0x0400049A RID: 1178
			internal FsmInt stage;

			// Token: 0x0400049B RID: 1179
			internal FsmFloat floatstage;

			// Token: 0x0400049C RID: 1180
			internal int hash;

			// Token: 0x0400049D RID: 1181
			private bool isPin;

			// Token: 0x0400049E RID: 1182
			private bool isTuneBolt;

			// Token: 0x0400049F RID: 1183
			private bool isScrewableLid;

			// Token: 0x040004A0 RID: 1184
			private bool isCamshaftGear;

			// Token: 0x040004A1 RID: 1185
			private Action<int, byte> onTightnessChange;

			// Token: 0x040004A2 RID: 1186
			internal FsmFloat alignment;

			// Token: 0x040004A3 RID: 1187
			internal FsmFloat rot;

			// Token: 0x040004A4 RID: 1188
			private float maxAlignment;

			// Token: 0x040004A5 RID: 1189
			private float rotAmount;

			// Token: 0x040004A6 RID: 1190
			private byte lastScrewSyncStage;

			// Token: 0x040004A7 RID: 1191
			private bool doSync = true;

			// Token: 0x040004A8 RID: 1192
			private static FsmGameObject raycastBolt;

			// Token: 0x0200021A RID: 538
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x0600094C RID: 2380 RVA: 0x00021481 File Offset: 0x0001F681
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x0600094D RID: 2381 RVA: 0x0002148D File Offset: 0x0001F68D
				public <>c()
				{
				}

				// Token: 0x0600094E RID: 2382 RVA: 0x00021495 File Offset: 0x0001F695
				internal bool <.ctor>b__18_0(FsmInt i)
				{
					return i.Name == "Stage";
				}

				// Token: 0x0600094F RID: 2383 RVA: 0x000214A7 File Offset: 0x0001F6A7
				internal bool <.ctor>b__18_1(FsmEvent e)
				{
					return e.Name == "TIGHTEN";
				}

				// Token: 0x06000950 RID: 2384 RVA: 0x000214B9 File Offset: 0x0001F6B9
				internal bool <.ctor>b__18_2(FsmEvent e)
				{
					return e.Name == "UNTIGHTEN";
				}

				// Token: 0x06000951 RID: 2385 RVA: 0x000214CB File Offset: 0x0001F6CB
				internal bool <.ctor>b__18_3(FsmEvent e)
				{
					return e.Name == "TIGHTEN";
				}

				// Token: 0x06000952 RID: 2386 RVA: 0x000214DD File Offset: 0x0001F6DD
				internal bool <.ctor>b__18_4(FsmEvent e)
				{
					return e.Name == "UNTIGHTEN";
				}

				// Token: 0x040005CF RID: 1487
				public static readonly NetPartManager.Bolt.<>c <>9 = new NetPartManager.Bolt.<>c();

				// Token: 0x040005D0 RID: 1488
				public static Func<FsmInt, bool> <>9__18_0;

				// Token: 0x040005D1 RID: 1489
				public static Func<FsmEvent, bool> <>9__18_1;

				// Token: 0x040005D2 RID: 1490
				public static Func<FsmEvent, bool> <>9__18_2;

				// Token: 0x040005D3 RID: 1491
				public static Func<FsmEvent, bool> <>9__18_3;

				// Token: 0x040005D4 RID: 1492
				public static Func<FsmEvent, bool> <>9__18_4;
			}
		}

		// Token: 0x020000F2 RID: 242
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004F3 RID: 1267 RVA: 0x0001D1A4 File Offset: 0x0001B3A4
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004F4 RID: 1268 RVA: 0x0001D1B0 File Offset: 0x0001B3B0
			public <>c()
			{
			}

			// Token: 0x060004F5 RID: 1269 RVA: 0x0001D1B8 File Offset: 0x0001B3B8
			internal bool <SetupRemovalPlaymaker>b__22_0(FsmTransition t)
			{
				return t.EventName.Contains("REMOV");
			}

			// Token: 0x060004F6 RID: 1270 RVA: 0x0001D1CA File Offset: 0x0001B3CA
			internal bool <SetupRemovalPlaymaker>b__22_1(FsmEvent e)
			{
				return e.Name == "MP_REMOVE";
			}

			// Token: 0x060004F7 RID: 1271 RVA: 0x0001D1DC File Offset: 0x0001B3DC
			internal bool <SetupAssemblePlaymaker>b__23_7(FsmEvent e)
			{
				return e.Name == "MP_ASSEMBLE";
			}

			// Token: 0x060004F8 RID: 1272 RVA: 0x0001D1EE File Offset: 0x0001B3EE
			internal bool <SetupAssemblePlaymaker>b__23_1(FsmTransition t)
			{
				return t.EventName.Contains("ASSEMBL");
			}

			// Token: 0x060004F9 RID: 1273 RVA: 0x0001D200 File Offset: 0x0001B400
			internal bool <SetupAssemblePlaymaker>b__23_2(FsmTransition t)
			{
				return t.EventName == "RESETWIRING";
			}

			// Token: 0x060004FA RID: 1274 RVA: 0x0001D212 File Offset: 0x0001B412
			internal bool <SetupAssemblePlaymaker>b__23_8(FsmState s)
			{
				return s.Name.ToLower().Contains("assemble");
			}

			// Token: 0x060004FB RID: 1275 RVA: 0x0001D229 File Offset: 0x0001B429
			internal bool <SetupAssemblePlaymaker>b__23_3(FsmEvent e)
			{
				return e.Name == "MP_ASSEMBLE";
			}

			// Token: 0x060004FC RID: 1276 RVA: 0x0001D23B File Offset: 0x0001B43B
			internal bool <SetupAssemblePlaymaker>b__23_4(FsmState s)
			{
				return s.Name.ToLower().Contains("assemble");
			}

			// Token: 0x060004FD RID: 1277 RVA: 0x0001D252 File Offset: 0x0001B452
			internal bool <FixWiringFsm>b__24_0(FsmStateAction a)
			{
				return a is FloatCompare;
			}

			// Token: 0x060004FE RID: 1278 RVA: 0x0001D25D File Offset: 0x0001B45D
			internal bool <Assemble>b__29_0(FsmTransition t)
			{
				return t.EventName == "RESETWIRING";
			}

			// Token: 0x060004FF RID: 1279 RVA: 0x0001D26F File Offset: 0x0001B46F
			internal bool <Assemble>b__29_1(FsmEvent e)
			{
				return e.Name == "MP_ASSEMBLE";
			}

			// Token: 0x06000500 RID: 1280 RVA: 0x0001D281 File Offset: 0x0001B481
			internal bool <Assemble>b__29_2(FsmEvent e)
			{
				return e.Name == "PLACEBATT";
			}

			// Token: 0x06000501 RID: 1281 RVA: 0x0001D293 File Offset: 0x0001B493
			internal bool <Assemble>b__29_3(FsmEvent e)
			{
				return e.Name == "MP_ASSEMBLE";
			}

			// Token: 0x06000502 RID: 1282 RVA: 0x0001D2A5 File Offset: 0x0001B4A5
			internal bool <Remove>b__30_0(FsmEvent e)
			{
				return e.Name == "MP_REMOVE";
			}

			// Token: 0x040004A9 RID: 1193
			public static readonly NetPartManager.<>c <>9 = new NetPartManager.<>c();

			// Token: 0x040004AA RID: 1194
			public static Func<FsmTransition, bool> <>9__22_0;

			// Token: 0x040004AB RID: 1195
			public static Func<FsmEvent, bool> <>9__22_1;

			// Token: 0x040004AC RID: 1196
			public static Func<FsmEvent, bool> <>9__23_7;

			// Token: 0x040004AD RID: 1197
			public static Func<FsmTransition, bool> <>9__23_1;

			// Token: 0x040004AE RID: 1198
			public static Func<FsmTransition, bool> <>9__23_2;

			// Token: 0x040004AF RID: 1199
			public static Func<FsmState, bool> <>9__23_8;

			// Token: 0x040004B0 RID: 1200
			public static Func<FsmEvent, bool> <>9__23_3;

			// Token: 0x040004B1 RID: 1201
			public static Func<FsmState, bool> <>9__23_4;

			// Token: 0x040004B2 RID: 1202
			public static Predicate<FsmStateAction> <>9__24_0;

			// Token: 0x040004B3 RID: 1203
			public static Func<FsmTransition, bool> <>9__29_0;

			// Token: 0x040004B4 RID: 1204
			public static Func<FsmEvent, bool> <>9__29_1;

			// Token: 0x040004B5 RID: 1205
			public static Func<FsmEvent, bool> <>9__29_2;

			// Token: 0x040004B6 RID: 1206
			public static Func<FsmEvent, bool> <>9__29_3;

			// Token: 0x040004B7 RID: 1207
			public static Func<FsmEvent, bool> <>9__30_0;
		}

		// Token: 0x020000F3 RID: 243
		[CompilerGenerated]
		private sealed class <>c__DisplayClass20_0
		{
			// Token: 0x06000503 RID: 1283 RVA: 0x0001D2B7 File Offset: 0x0001B4B7
			public <>c__DisplayClass20_0()
			{
			}

			// Token: 0x06000504 RID: 1284 RVA: 0x0001D2BF File Offset: 0x0001B4BF
			internal bool <SendAssembleEvent>b__0(NetRigidbodyManager.OwnedRigidbody r)
			{
				return r.Rigidbody == this.part.Value.GetComponent<Rigidbody>();
			}

			// Token: 0x040004B8 RID: 1208
			public FsmGameObject part;
		}

		// Token: 0x020000F4 RID: 244
		[CompilerGenerated]
		private sealed class <>c__DisplayClass22_0
		{
			// Token: 0x06000505 RID: 1285 RVA: 0x0001D2DC File Offset: 0x0001B4DC
			public <>c__DisplayClass22_0()
			{
			}

			// Token: 0x06000506 RID: 1286 RVA: 0x0001D2E4 File Offset: 0x0001B4E4
			internal void <SetupRemovalPlaymaker>b__2()
			{
				NetPartManager.RemoveEvent(this.hash);
			}

			// Token: 0x040004B9 RID: 1209
			public int hash;
		}

		// Token: 0x020000F5 RID: 245
		[CompilerGenerated]
		private sealed class <>c__DisplayClass23_0
		{
			// Token: 0x06000507 RID: 1287 RVA: 0x0001D2F1 File Offset: 0x0001B4F1
			public <>c__DisplayClass23_0()
			{
			}

			// Token: 0x06000508 RID: 1288 RVA: 0x0001D2F9 File Offset: 0x0001B4F9
			internal void <SetupAssemblePlaymaker>b__6()
			{
				NetPartManager.BatteryToRadioOrFlashlightEvent(this.hash, this.fsm.FsmVariables.FindFsmGameObject("Part"));
			}

			// Token: 0x06000509 RID: 1289 RVA: 0x0001D31B File Offset: 0x0001B51B
			internal void <SetupAssemblePlaymaker>b__0()
			{
				NetPartManager.BatteryOnChargerEvent(this.hash, this.fsm.FsmVariables.FindFsmGameObject("Battery"));
			}

			// Token: 0x0600050A RID: 1290 RVA: 0x0001D33D File Offset: 0x0001B53D
			internal void <SetupAssemblePlaymaker>b__5()
			{
				NetPartManager.AssembleEvent(this.hash);
			}

			// Token: 0x0600050B RID: 1291 RVA: 0x0001D34A File Offset: 0x0001B54A
			internal void <SetupAssemblePlaymaker>b__9()
			{
				NetPartManager.AssembleEvent(this.hash);
			}

			// Token: 0x040004BA RID: 1210
			public int hash;

			// Token: 0x040004BB RID: 1211
			public PlayMakerFSM fsm;

			// Token: 0x040004BC RID: 1212
			public Action <>9__9;
		}

		// Token: 0x020000F6 RID: 246
		[CompilerGenerated]
		private sealed class <>c__DisplayClass24_0
		{
			// Token: 0x0600050C RID: 1292 RVA: 0x0001D357 File Offset: 0x0001B557
			public <>c__DisplayClass24_0()
			{
			}

			// Token: 0x0600050D RID: 1293 RVA: 0x0001D360 File Offset: 0x0001B560
			internal void <FixWiringFsm>b__1()
			{
				if (this.dist.Value < this.tol.Value && NetPartManager.wiringMess.transform.root.name == "PLAYER")
				{
					this.fsm.SendEvent("ASSEMBLE");
				}
			}

			// Token: 0x040004BD RID: 1213
			public FsmFloat dist;

			// Token: 0x040004BE RID: 1214
			public FsmFloat tol;

			// Token: 0x040004BF RID: 1215
			public PlayMakerFSM fsm;
		}

		// Token: 0x020000F7 RID: 247
		[CompilerGenerated]
		private sealed class <>c__DisplayClass32_0
		{
			// Token: 0x0600050E RID: 1294 RVA: 0x0001D3B5 File Offset: 0x0001B5B5
			public <>c__DisplayClass32_0()
			{
			}

			// Token: 0x0600050F RID: 1295 RVA: 0x0001D3BD File Offset: 0x0001B5BD
			internal bool <OnTightnessChange>b__0(NetPartManager.Bolt b)
			{
				return b.hash == this.boltHash;
			}

			// Token: 0x040004C0 RID: 1216
			public int boltHash;
		}

		// Token: 0x020000F8 RID: 248
		[CompilerGenerated]
		private sealed class <>c__DisplayClass34_0
		{
			// Token: 0x06000510 RID: 1296 RVA: 0x0001D3CD File Offset: 0x0001B5CD
			public <>c__DisplayClass34_0()
			{
			}

			// Token: 0x06000511 RID: 1297 RVA: 0x0001D3D5 File Offset: 0x0001B5D5
			internal bool <OnInitBolts>b__0(NetPartManager.Bolt b)
			{
				return b.hash == this.boltHash;
			}

			// Token: 0x040004C1 RID: 1217
			public int boltHash;
		}
	}
}
