using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using Steamworks;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000061 RID: 97
	[ManagerCreate(10)]
	public class NetRigidbodyManager : MonoBehaviour
	{
		// Token: 0x06000280 RID: 640 RVA: 0x00010240 File Offset: 0x0000E440
		private void Start()
		{
			Action action = delegate
			{
				string text = "";
				List<PlayMakerFSM> list = new List<PlayMakerFSM>();
				for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
				{
					if ((ObjectsLoader.ObjectsInGame[i].activeInHierarchy || !ObjectsLoader.ObjectsInGame[i].activeSelf || !(ObjectsLoader.ObjectsInGame[i].transform.parent == null)) && (!(ObjectsLoader.ObjectsInGame[i].name == "Ax") || ObjectsLoader.ObjectsInGame[i].layer != 20))
					{
						if (ObjectsLoader.ObjectsInGame[i].name == "wiring mess(itemx)")
						{
							NetPartManager.wiringMess = ObjectsLoader.ObjectsInGame[i].transform;
						}
						string text2 = ObjectsLoader.ObjectsInGame[i].transform.GetGameobjectHashString();
						PlayMakerFSM playMaker = ObjectsLoader.ObjectsInGame[i].GetPlayMaker("Use");
						if (playMaker != null)
						{
							FsmString fsmString = playMaker.FsmVariables.FindFsmString("ID");
							if (fsmString != null)
							{
								text2 = fsmString.Value;
							}
						}
						int hashCode = text2.GetHashCode();
						PlayMakerFSM[] components = ObjectsLoader.ObjectsInGame[i].GetComponents<PlayMakerFSM>();
						PlayMakerFSM playMakerFSM = null;
						FsmObject fsmObject = null;
						bool flag = false;
						bool flag2 = false;
						foreach (PlayMakerFSM playMakerFSM2 in components)
						{
							playMakerFSM2.Initialize();
							if (playMakerFSM2.FsmName == "Removal")
							{
								playMakerFSM = playMakerFSM2;
								fsmObject = playMakerFSM2.FsmVariables.FindFsmObject("Rigidbody");
								NetPartManager.SetupRemovalPlaymaker(playMakerFSM2, hashCode);
								if (playMakerFSM2.FsmVariables.FindFsmGameObject("db_ThisPart") != null && playMakerFSM2.GetComponent<Rigidbody>() == null)
								{
									list.Add(playMakerFSM2);
									flag2 = true;
									break;
								}
							}
							else if (playMakerFSM2.FsmName == "Assembly" || playMakerFSM2.FsmName == "Assemble")
							{
								int playmakerHash = playMakerFSM2.GetPlaymakerHash();
								NetPartManager.AddAssembleFsm(playmakerHash, playMakerFSM2);
								NetPartManager.SetupAssemblePlaymaker(playMakerFSM2, playmakerHash);
							}
							else if (playMakerFSM2.FsmName == "Screw" && (ObjectsLoader.ObjectsInGame[i].layer == 12 || ObjectsLoader.ObjectsInGame[i].layer == 19))
							{
								if (!NetPartManager.AddBolt(playMakerFSM2, hashCode))
								{
									Console.LogError(string.Format("Bolt of hash {0} ({1}) doesn't have stage variable", hashCode, text2), false);
								}
								flag = true;
								break;
							}
						}
						if (!flag && !flag2)
						{
							Rigidbody component = ObjectsLoader.ObjectsInGame[i].GetComponent<Rigidbody>();
							if (!(component == null) || !(playMakerFSM == null))
							{
								if (component != null && component.transform.name == "SATSUMA(557kg, 248)")
								{
									new SatsumaProfiler(component);
								}
								NetRigidbodyManager.OwnedRigidbody ownedRigidbody = new NetRigidbodyManager.OwnedRigidbody
								{
									hash = hashCode,
									OwnerID = BeerMPGlobals.HostID,
									rigidbody = component,
									remove = playMakerFSM,
									Removal_Rigidbody = fsmObject,
									transform = ObjectsLoader.ObjectsInGame[i].transform
								};
								NetRigidbodyManager.rigidbodyHashes.Add(hashCode);
								NetRigidbodyManager.ownedRigidbodies.Add(ownedRigidbody);
								if (ObjectsLoader.ObjectsInGame[i].layer == 19)
								{
									ObjectsLoader.ObjectsInGame[i].AddComponent<MPItem>().RB = ownedRigidbody;
								}
								text += string.Format("{0} - {1} - {2}\n", hashCode, ObjectsLoader.ObjectsInGame[i].name, text2);
							}
						}
					}
				}
				File.WriteAllText("hashesDebug.txt", text);
				for (int k = 0; k < list.Count; k++)
				{
					try
					{
						PlayMakerFSM component2 = list[k].FsmVariables.FindFsmGameObject("db_ThisPart").Value.GetComponent<PlayMakerFSM>();
						FsmGameObject fsmGameObject = list[k].FsmVariables.FindFsmGameObject("Part");
						FsmGameObject fsmGameObject2 = component2.FsmVariables.FindFsmGameObject("ThisPart");
						if (fsmGameObject2 == null)
						{
							fsmGameObject2 = component2.FsmVariables.FindFsmGameObject("SpawnThis");
						}
						int hashCode2 = list[k].transform.GetGameobjectHashString().GetHashCode();
						Rigidbody rb = fsmGameObject2.Value.GetComponent<Rigidbody>();
						int num = NetRigidbodyManager.ownedRigidbodies.FindIndex((NetRigidbodyManager.OwnedRigidbody orb) => orb.Rigidbody == rb);
						NetRigidbodyManager.OwnedRigidbody ownedRigidbody2 = new NetRigidbodyManager.OwnedRigidbody
						{
							hash = hashCode2,
							OwnerID = BeerMPGlobals.HostID,
							rigidbody = rb,
							remove = list[k],
							Removal_Part = fsmGameObject,
							rigidbodyPart = rb,
							transform = list[k].transform
						};
						if (num == -1)
						{
							NetRigidbodyManager.rigidbodyHashes.Add(hashCode2);
							NetRigidbodyManager.ownedRigidbodies.Add(ownedRigidbody2);
						}
						else
						{
							NetRigidbodyManager.rigidbodyHashes[num] = hashCode2;
							NetRigidbodyManager.ownedRigidbodies[num] = ownedRigidbody2;
						}
					}
					catch (Exception ex)
					{
						Console.LogError(string.Format("xxxxx removal creation error: {0}, {1}, {2}", ex.GetType(), ex.Message, ex.StackTrace), false);
					}
				}
				NetEvent<NetRigidbodyManager>.Register("RigidbodyUpdate", new NetEventHandler(this.OnRigidbodyUpdate));
				NetEvent<NetRigidbodyManager>.Register("InitRigidbodyUpdate", new NetEventHandler(this.OnInitRigidbodyUpdate));
				NetEvent<NetRigidbodyManager>.Register("RequestOwnership", new NetEventHandler(this.OnRequestOwnership));
				NetEvent<NetRigidbodyManager>.Register("SetOwnership", delegate(ulong sender, Packet p)
				{
					ulong num2 = (ulong)p.ReadLong(true);
					this.OnRequestOwnership(num2, p);
				});
				BeerMPGlobals.OnMemberReady += delegate(ulong user)
				{
					this.InitSyncRb(user);
				};
			};
			if (ObjectsLoader.IsGameLoaded)
			{
				action();
				return;
			}
			ObjectsLoader.gameLoaded += action;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00010278 File Offset: 0x0000E478
		public static NetRigidbodyManager.OwnedRigidbody AddRigidbody(Rigidbody rb, int hash)
		{
			NetRigidbodyManager.OwnedRigidbody ownedRigidbody = new NetRigidbodyManager.OwnedRigidbody
			{
				hash = hash,
				OwnerID = BeerMPGlobals.HostID,
				rigidbody = rb,
				remove = null,
				Removal_Rigidbody = null,
				transform = rb.transform
			};
			NetRigidbodyManager.rigidbodyHashes.Add(hash);
			NetRigidbodyManager.ownedRigidbodies.Add(ownedRigidbody);
			if (rb.gameObject.layer == 19)
			{
				rb.gameObject.AddComponent<MPItem>().RB = ownedRigidbody;
			}
			return ownedRigidbody;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000102F7 File Offset: 0x0000E4F7
		private void OnDestroy()
		{
			NetRigidbodyManager.rigidbodyHashes.Clear();
			NetRigidbodyManager.unknownHashes.Clear();
			NetRigidbodyManager.ownedRigidbodies.Clear();
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00010318 File Offset: 0x0000E518
		private void FixedUpdate()
		{
			bool flag;
			this.HandleIncomingUpdates(out flag);
			ulong num = 0UL;
			using (Packet packet = new Packet(1))
			{
				this.syncUpdateTime += Time.fixedDeltaTime;
				if (this.syncUpdateTime > 5f)
				{
					this.syncUpdateTime = 0f;
				}
				int num2 = 0;
				for (int i = 0; i < NetRigidbodyManager.rigidbodyHashes.Count; i++)
				{
					if (NetRigidbodyManager.ownedRigidbodies[i].Rigidbody && NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.transform.name == "SATSUMA(557kg, 248)")
					{
						num = NetRigidbodyManager.ownedRigidbodies[i].OwnerID;
					}
					if (NetRigidbodyManager.ownedRigidbodies[i].OwnerID == BeerMPGlobals.UserID)
					{
						if (!NetRigidbodyManager.ownedRigidbodies[i].Rigidbody)
						{
							if (NetRigidbodyManager.ownedRigidbodies[i].Removal_Rigidbody == null && NetRigidbodyManager.ownedRigidbodies[i].Removal_Part == null)
							{
								NetRigidbodyManager.rigidbodyHashes.RemoveAt(i);
								NetRigidbodyManager.ownedRigidbodies.RemoveAt(i--);
							}
						}
						else if (((double)NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.velocity.sqrMagnitude > 0.0001 || !(NetRigidbodyManager.ownedRigidbodies[i].Rigidbody != PlayerGrabbingManager.GrabbedRigidbody)) && (!(NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.transform.parent != null) || NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.transform.root.gameObject.layer != NetRigidbodyManager.datunLayer))
						{
							NetRigidbodyManager.ownedRigidbodies[i].cachedPosition = NetRigidbodyManager.ownedRigidbodies[i].transform.position;
							NetRigidbodyManager.ownedRigidbodies[i].cachedEulerAngles = NetRigidbodyManager.ownedRigidbodies[i].transform.eulerAngles;
							this.WriteRigidbody(packet, i);
							num2++;
						}
					}
				}
				SatsumaProfiler instance = SatsumaProfiler.Instance;
				if (instance != null)
				{
					instance.Update(flag, num);
				}
				if (num2 > 0)
				{
					packet.Write(num2, 0);
					NetEvent<NetRigidbodyManager>.Send("RigidbodyUpdate", packet, true);
				}
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0001059C File Offset: 0x0000E79C
		private void HandleIncomingUpdates(out bool receivedSatsuma)
		{
			receivedSatsuma = false;
			for (int i = 0; i < NetRigidbodyManager.receivedUpdates.Count; i++)
			{
				for (int j = 0; j < NetRigidbodyManager.receivedUpdates[i].Length; j++)
				{
					NetRigidbodyManager.RBUpdate rbupdate = NetRigidbodyManager.receivedUpdates[i][j];
					if (rbupdate.orb != null && rbupdate.orb.Rigidbody)
					{
						if (rbupdate.orb.assemble != null)
						{
							Console.LogWarning("Received update for rigidbody " + rbupdate.orb.transform.name + " which is already assembled, skipping", false);
						}
						else
						{
							if (rbupdate.orb.Rigidbody.transform.name == "SATSUMA(557kg, 248)")
							{
								receivedSatsuma = true;
							}
							rbupdate.orb.Rigidbody.transform.position = rbupdate.pos;
							rbupdate.orb.cachedPosition = rbupdate.pos;
							rbupdate.orb.Rigidbody.transform.eulerAngles = rbupdate.rot;
							rbupdate.orb.cachedEulerAngles = rbupdate.rot;
							rbupdate.orb.Rigidbody.velocity = rbupdate.vel;
							rbupdate.orb.Rigidbody.angularVelocity = rbupdate.ang;
						}
					}
				}
			}
			NetRigidbodyManager.receivedUpdates.Clear();
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00010708 File Offset: 0x0000E908
		private void InitSyncRb(ulong target)
		{
			using (Packet packet = new Packet(1))
			{
				Console.Log("Init sync rb", false);
				int num = 0;
				for (int i = 0; i < NetRigidbodyManager.rigidbodyHashes.Count; i++)
				{
					if (NetRigidbodyManager.ownedRigidbodies[i].OwnerID == BeerMPGlobals.UserID)
					{
						if (!NetRigidbodyManager.ownedRigidbodies[i].Rigidbody)
						{
							if (NetRigidbodyManager.ownedRigidbodies[i].Removal_Rigidbody == null && NetRigidbodyManager.ownedRigidbodies[i].Removal_Part == null)
							{
								NetRigidbodyManager.rigidbodyHashes.RemoveAt(i);
								NetRigidbodyManager.ownedRigidbodies.RemoveAt(i--);
							}
						}
						else if (NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.transform.root.gameObject.layer != NetRigidbodyManager.datunLayer)
						{
							this.WriteRigidbody(packet, i);
							num++;
						}
					}
				}
				Console.Log(string.Format("Init sync rb: {0}", num), false);
				if (num > 0)
				{
					packet.Write(num, 0);
					NetEvent<NetRigidbodyManager>.Send("InitRigidbodyUpdate", packet, target, true);
				}
			}
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00010844 File Offset: 0x0000EA44
		private void WriteRigidbody(Packet _p, int i)
		{
			_p.Write(NetRigidbodyManager.rigidbodyHashes[i], -1);
			_p.Write(NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.transform.position, -1);
			_p.Write(NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.transform.eulerAngles, -1);
			_p.Write(NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.velocity, -1);
			_p.Write(NetRigidbodyManager.ownedRigidbodies[i].Rigidbody.angularVelocity, -1);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000108DD File Offset: 0x0000EADD
		private void Update()
		{
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000108E0 File Offset: 0x0000EAE0
		private void OnRigidbodyUpdate(ulong userId, Packet packet)
		{
			int num = packet.ReadInt(true);
			NetRigidbodyManager.RBUpdate[] array = new NetRigidbodyManager.RBUpdate[num];
			for (int i = 0; i < num; i++)
			{
				try
				{
					int num2 = packet.ReadInt(true);
					int num3 = NetRigidbodyManager.rigidbodyHashes.IndexOf(num2);
					Vector3 vector = packet.ReadVector3(true);
					Vector3 vector2 = packet.ReadVector3(true);
					Vector3 vector3 = packet.ReadVector3(true);
					Vector3 vector4 = packet.ReadVector3(true);
					if (num3 == -1)
					{
						if (!NetRigidbodyManager.unknownHashes.Contains(num2))
						{
							Console.LogError(string.Format("Recieved an update for rigidbody with hash {0} but it doesn't seem to exist", num2), false);
							NetRigidbodyManager.unknownHashes.Add(num2);
						}
					}
					else if (NetRigidbodyManager.ownedRigidbodies[num3].OwnerID != userId)
					{
						Console.LogWarning(string.Concat(new string[]
						{
							"Received update from ",
							NetManager.playerNames[(CSteamID)userId],
							" for rigidbody ",
							NetRigidbodyManager.ownedRigidbodies[num3].Rigidbody.name,
							" but he's not its owner, skipping"
						}), false);
					}
					else
					{
						array[i].orb = NetRigidbodyManager.ownedRigidbodies[num3];
						array[i].pos = vector;
						array[i].rot = vector2;
						array[i].vel = vector3;
						array[i].ang = vector4;
					}
				}
				catch (Exception ex)
				{
					Console.LogError(string.Format("OnRigidbodyUpdate: {0}, {1}, {2}", ex.GetType(), ex.Message, ex.StackTrace), false);
				}
			}
			NetRigidbodyManager.receivedUpdates.Add(array);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00010A90 File Offset: 0x0000EC90
		private void OnInitRigidbodyUpdate(ulong sender, Packet packet)
		{
			this.OnRigidbodyUpdate(sender, packet);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00010A9C File Offset: 0x0000EC9C
		private void OnRequestOwnership(ulong userId, Packet packet)
		{
			userId = (ulong)packet.ReadLong(true);
			int num = packet.ReadInt(true);
			int num2 = NetRigidbodyManager.rigidbodyHashes.IndexOf(num);
			if (num2 == -1)
			{
				Console.LogError(string.Format("Recieved an ownership request for rigidbody with hash {0} but it doesn't seem to exist", num), false);
				return;
			}
			NetRigidbodyManager.ownedRigidbodies[num2].OwnerID = userId;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00010AF4 File Offset: 0x0000ECF4
		public static Rigidbody GetRigidbody(int hash)
		{
			int num = NetRigidbodyManager.rigidbodyHashes.IndexOf(hash);
			Rigidbody rigidbody;
			if (num == -1)
			{
				Console.LogError(string.Format("GetRigidbody: rigidbody with hash {0} doesn't seem to exist", hash), false);
				rigidbody = null;
			}
			else
			{
				rigidbody = NetRigidbodyManager.ownedRigidbodies[num].Rigidbody;
			}
			return rigidbody;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00010B40 File Offset: 0x0000ED40
		public static NetRigidbodyManager.OwnedRigidbody GetOwnedRigidbody(int hash)
		{
			int num = NetRigidbodyManager.rigidbodyHashes.IndexOf(hash);
			NetRigidbodyManager.OwnedRigidbody ownedRigidbody;
			if (num == -1)
			{
				Console.LogError(string.Format("GetOwnedRigidbody: rigidbody with hash {0} doesn't seem to exist", hash), false);
				ownedRigidbody = null;
			}
			else
			{
				ownedRigidbody = NetRigidbodyManager.ownedRigidbodies[num];
			}
			return ownedRigidbody;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00010B88 File Offset: 0x0000ED88
		public static int GetRigidbodyHash(Rigidbody rb)
		{
			int num = NetRigidbodyManager.ownedRigidbodies.FindIndex((NetRigidbodyManager.OwnedRigidbody or) => or.Rigidbody == rb);
			if (num == -1)
			{
				return 0;
			}
			return NetRigidbodyManager.rigidbodyHashes[num];
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00010BD0 File Offset: 0x0000EDD0
		public static void RequestOwnership(Rigidbody rigidbody)
		{
			int num = NetRigidbodyManager.ownedRigidbodies.FindIndex((NetRigidbodyManager.OwnedRigidbody x) => x.Rigidbody == rigidbody);
			if (num == -1)
			{
				Console.LogError("Request ownership failed: Didn't find rigidbody " + rigidbody.gameObject.name + " in orb list", false);
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write((long)BeerMPGlobals.UserID, -1);
				packet.Write(NetRigidbodyManager.rigidbodyHashes[num], -1);
				NetEvent<NetRigidbodyManager>.Send("RequestOwnership", packet, true);
				NetRigidbodyManager.ownedRigidbodies[num].OwnerID = BeerMPGlobals.UserID;
			}
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00010C90 File Offset: 0x0000EE90
		internal static void RequestOwnership(NetRigidbodyManager.OwnedRigidbody orb)
		{
			int num = NetRigidbodyManager.ownedRigidbodies.IndexOf(orb);
			if (num == -1)
			{
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write((long)BeerMPGlobals.UserID, -1);
				packet.Write(NetRigidbodyManager.rigidbodyHashes[num], -1);
				NetEvent<NetRigidbodyManager>.Send("RequestOwnership", packet, true);
				orb.OwnerID = BeerMPGlobals.UserID;
			}
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00010D08 File Offset: 0x0000EF08
		internal static void RequestOwnership(NetRigidbodyManager.OwnedRigidbody orb, ulong owner)
		{
			int num = NetRigidbodyManager.ownedRigidbodies.IndexOf(orb);
			if (num == -1)
			{
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write((long)owner, -1);
				packet.Write(NetRigidbodyManager.rigidbodyHashes[num], -1);
				NetEvent<NetRigidbodyManager>.Send("RequestOwnership", packet, true);
				orb.OwnerID = owner;
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00010D78 File Offset: 0x0000EF78
		public NetRigidbodyManager()
		{
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00010D80 File Offset: 0x0000EF80
		// Note: this type is marked as 'beforefieldinit'.
		static NetRigidbodyManager()
		{
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00010DBC File Offset: 0x0000EFBC
		[CompilerGenerated]
		private void <Start>b__7_0()
		{
			string text = "";
			List<PlayMakerFSM> list = new List<PlayMakerFSM>();
			for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
			{
				if ((ObjectsLoader.ObjectsInGame[i].activeInHierarchy || !ObjectsLoader.ObjectsInGame[i].activeSelf || !(ObjectsLoader.ObjectsInGame[i].transform.parent == null)) && (!(ObjectsLoader.ObjectsInGame[i].name == "Ax") || ObjectsLoader.ObjectsInGame[i].layer != 20))
				{
					if (ObjectsLoader.ObjectsInGame[i].name == "wiring mess(itemx)")
					{
						NetPartManager.wiringMess = ObjectsLoader.ObjectsInGame[i].transform;
					}
					string text2 = ObjectsLoader.ObjectsInGame[i].transform.GetGameobjectHashString();
					PlayMakerFSM playMaker = ObjectsLoader.ObjectsInGame[i].GetPlayMaker("Use");
					if (playMaker != null)
					{
						FsmString fsmString = playMaker.FsmVariables.FindFsmString("ID");
						if (fsmString != null)
						{
							text2 = fsmString.Value;
						}
					}
					int hashCode = text2.GetHashCode();
					PlayMakerFSM[] components = ObjectsLoader.ObjectsInGame[i].GetComponents<PlayMakerFSM>();
					PlayMakerFSM playMakerFSM = null;
					FsmObject fsmObject = null;
					bool flag = false;
					bool flag2 = false;
					foreach (PlayMakerFSM playMakerFSM2 in components)
					{
						playMakerFSM2.Initialize();
						if (playMakerFSM2.FsmName == "Removal")
						{
							playMakerFSM = playMakerFSM2;
							fsmObject = playMakerFSM2.FsmVariables.FindFsmObject("Rigidbody");
							NetPartManager.SetupRemovalPlaymaker(playMakerFSM2, hashCode);
							if (playMakerFSM2.FsmVariables.FindFsmGameObject("db_ThisPart") != null && playMakerFSM2.GetComponent<Rigidbody>() == null)
							{
								list.Add(playMakerFSM2);
								flag2 = true;
								break;
							}
						}
						else if (playMakerFSM2.FsmName == "Assembly" || playMakerFSM2.FsmName == "Assemble")
						{
							int playmakerHash = playMakerFSM2.GetPlaymakerHash();
							NetPartManager.AddAssembleFsm(playmakerHash, playMakerFSM2);
							NetPartManager.SetupAssemblePlaymaker(playMakerFSM2, playmakerHash);
						}
						else if (playMakerFSM2.FsmName == "Screw" && (ObjectsLoader.ObjectsInGame[i].layer == 12 || ObjectsLoader.ObjectsInGame[i].layer == 19))
						{
							if (!NetPartManager.AddBolt(playMakerFSM2, hashCode))
							{
								Console.LogError(string.Format("Bolt of hash {0} ({1}) doesn't have stage variable", hashCode, text2), false);
							}
							flag = true;
							break;
						}
					}
					if (!flag && !flag2)
					{
						Rigidbody component = ObjectsLoader.ObjectsInGame[i].GetComponent<Rigidbody>();
						if (!(component == null) || !(playMakerFSM == null))
						{
							if (component != null && component.transform.name == "SATSUMA(557kg, 248)")
							{
								new SatsumaProfiler(component);
							}
							NetRigidbodyManager.OwnedRigidbody ownedRigidbody = new NetRigidbodyManager.OwnedRigidbody
							{
								hash = hashCode,
								OwnerID = BeerMPGlobals.HostID,
								rigidbody = component,
								remove = playMakerFSM,
								Removal_Rigidbody = fsmObject,
								transform = ObjectsLoader.ObjectsInGame[i].transform
							};
							NetRigidbodyManager.rigidbodyHashes.Add(hashCode);
							NetRigidbodyManager.ownedRigidbodies.Add(ownedRigidbody);
							if (ObjectsLoader.ObjectsInGame[i].layer == 19)
							{
								ObjectsLoader.ObjectsInGame[i].AddComponent<MPItem>().RB = ownedRigidbody;
							}
							text += string.Format("{0} - {1} - {2}\n", hashCode, ObjectsLoader.ObjectsInGame[i].name, text2);
						}
					}
				}
			}
			File.WriteAllText("hashesDebug.txt", text);
			for (int k = 0; k < list.Count; k++)
			{
				try
				{
					PlayMakerFSM component2 = list[k].FsmVariables.FindFsmGameObject("db_ThisPart").Value.GetComponent<PlayMakerFSM>();
					FsmGameObject fsmGameObject = list[k].FsmVariables.FindFsmGameObject("Part");
					FsmGameObject fsmGameObject2 = component2.FsmVariables.FindFsmGameObject("ThisPart");
					if (fsmGameObject2 == null)
					{
						fsmGameObject2 = component2.FsmVariables.FindFsmGameObject("SpawnThis");
					}
					int hashCode2 = list[k].transform.GetGameobjectHashString().GetHashCode();
					Rigidbody rb = fsmGameObject2.Value.GetComponent<Rigidbody>();
					int num = NetRigidbodyManager.ownedRigidbodies.FindIndex((NetRigidbodyManager.OwnedRigidbody orb) => orb.Rigidbody == rb);
					NetRigidbodyManager.OwnedRigidbody ownedRigidbody2 = new NetRigidbodyManager.OwnedRigidbody
					{
						hash = hashCode2,
						OwnerID = BeerMPGlobals.HostID,
						rigidbody = rb,
						remove = list[k],
						Removal_Part = fsmGameObject,
						rigidbodyPart = rb,
						transform = list[k].transform
					};
					if (num == -1)
					{
						NetRigidbodyManager.rigidbodyHashes.Add(hashCode2);
						NetRigidbodyManager.ownedRigidbodies.Add(ownedRigidbody2);
					}
					else
					{
						NetRigidbodyManager.rigidbodyHashes[num] = hashCode2;
						NetRigidbodyManager.ownedRigidbodies[num] = ownedRigidbody2;
					}
				}
				catch (Exception ex)
				{
					Console.LogError(string.Format("xxxxx removal creation error: {0}, {1}, {2}", ex.GetType(), ex.Message, ex.StackTrace), false);
				}
			}
			NetEvent<NetRigidbodyManager>.Register("RigidbodyUpdate", new NetEventHandler(this.OnRigidbodyUpdate));
			NetEvent<NetRigidbodyManager>.Register("InitRigidbodyUpdate", new NetEventHandler(this.OnInitRigidbodyUpdate));
			NetEvent<NetRigidbodyManager>.Register("RequestOwnership", new NetEventHandler(this.OnRequestOwnership));
			NetEvent<NetRigidbodyManager>.Register("SetOwnership", delegate(ulong sender, Packet p)
			{
				ulong num2 = (ulong)p.ReadLong(true);
				this.OnRequestOwnership(num2, p);
			});
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				this.InitSyncRb(user);
			};
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00011348 File Offset: 0x0000F548
		[CompilerGenerated]
		private void <Start>b__7_1(ulong sender, Packet p)
		{
			ulong num = (ulong)p.ReadLong(true);
			this.OnRequestOwnership(num, p);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00011365 File Offset: 0x0000F565
		[CompilerGenerated]
		private void <Start>b__7_2(ulong user)
		{
			this.InitSyncRb(user);
		}

		// Token: 0x0400026D RID: 621
		private static List<NetRigidbodyManager.RBUpdate[]> receivedUpdates = new List<NetRigidbodyManager.RBUpdate[]>();

		// Token: 0x0400026E RID: 622
		internal static List<int> rigidbodyHashes = new List<int>();

		// Token: 0x0400026F RID: 623
		internal static List<int> unknownHashes = new List<int>();

		// Token: 0x04000270 RID: 624
		internal static List<NetRigidbodyManager.OwnedRigidbody> ownedRigidbodies = new List<NetRigidbodyManager.OwnedRigidbody>();

		// Token: 0x04000271 RID: 625
		private static readonly int datunLayer = LayerMask.NameToLayer("Datsun");

		// Token: 0x04000272 RID: 626
		private float syncUpdateTime;

		// Token: 0x02000109 RID: 265
		public class OwnedRigidbody
		{
			// Token: 0x1700003D RID: 61
			// (get) Token: 0x0600054C RID: 1356 RVA: 0x0001E413 File Offset: 0x0001C613
			// (set) Token: 0x0600054D RID: 1357 RVA: 0x0001E41B File Offset: 0x0001C61B
			public ulong OwnerID
			{
				[CompilerGenerated]
				get
				{
					return this.<OwnerID>k__BackingField;
				}
				[CompilerGenerated]
				internal set
				{
					this.<OwnerID>k__BackingField = value;
				}
			}

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001E424 File Offset: 0x0001C624
			public Rigidbody Rigidbody
			{
				get
				{
					if (this.rigidbody != null)
					{
						return this.rigidbody;
					}
					if (this.remove != null && this.Removal_Rigidbody != null)
					{
						Rigidbody rigidbody = this.Removal_Rigidbody.Value as Rigidbody;
						if (rigidbody != null)
						{
							return rigidbody;
						}
						if (this.Removal_Rigidbody_Cache)
						{
							return this.Removal_Rigidbody_Cache;
						}
						if (this.remove.enabled)
						{
							return null;
						}
						if (Time.time - this.lastRBcheckTime > 0.5f)
						{
							this.lastRBcheckTime = Time.time;
							rigidbody = (this.Removal_Rigidbody_Cache = this.remove.transform.GetComponent<Rigidbody>());
							this.Removal_Rigidbody.Value = rigidbody;
							return rigidbody;
						}
						return null;
					}
					else
					{
						if (!(this.remove != null) || this.Removal_Part == null)
						{
							return null;
						}
						if (this.rigidbodyPart)
						{
							return this.rigidbodyPart;
						}
						if (this.Removal_Part.Value != null)
						{
							this.lastRBcheckTime = Time.time;
							this.rigidbodyPart = this.Removal_Part.Value.GetComponent<Rigidbody>();
							return this.rigidbodyPart;
						}
						return null;
					}
				}
			}

			// Token: 0x0600054F RID: 1359 RVA: 0x0001E54F File Offset: 0x0001C74F
			public OwnedRigidbody()
			{
			}

			// Token: 0x04000507 RID: 1287
			[CompilerGenerated]
			private ulong <OwnerID>k__BackingField;

			// Token: 0x04000508 RID: 1288
			internal int hash;

			// Token: 0x04000509 RID: 1289
			internal Rigidbody rigidbody;

			// Token: 0x0400050A RID: 1290
			internal Rigidbody rigidbodyPart;

			// Token: 0x0400050B RID: 1291
			internal Vector3 cachedPosition;

			// Token: 0x0400050C RID: 1292
			internal Vector3 cachedEulerAngles;

			// Token: 0x0400050D RID: 1293
			public Transform transform;

			// Token: 0x0400050E RID: 1294
			private float lastRBcheckTime;

			// Token: 0x0400050F RID: 1295
			internal FsmObject Removal_Rigidbody;

			// Token: 0x04000510 RID: 1296
			private Rigidbody Removal_Rigidbody_Cache;

			// Token: 0x04000511 RID: 1297
			internal FsmGameObject Removal_Part;

			// Token: 0x04000512 RID: 1298
			internal PlayMakerFSM assemble;

			// Token: 0x04000513 RID: 1299
			internal PlayMakerFSM remove;
		}

		// Token: 0x0200010A RID: 266
		private struct RBUpdate
		{
			// Token: 0x04000514 RID: 1300
			public NetRigidbodyManager.OwnedRigidbody orb;

			// Token: 0x04000515 RID: 1301
			public Vector3 pos;

			// Token: 0x04000516 RID: 1302
			public Vector3 rot;

			// Token: 0x04000517 RID: 1303
			public Vector3 vel;

			// Token: 0x04000518 RID: 1304
			public Vector3 ang;
		}

		// Token: 0x0200010B RID: 267
		[CompilerGenerated]
		private sealed class <>c__DisplayClass21_0
		{
			// Token: 0x06000550 RID: 1360 RVA: 0x0001E557 File Offset: 0x0001C757
			public <>c__DisplayClass21_0()
			{
			}

			// Token: 0x06000551 RID: 1361 RVA: 0x0001E55F File Offset: 0x0001C75F
			internal bool <GetRigidbodyHash>b__0(NetRigidbodyManager.OwnedRigidbody or)
			{
				return or.Rigidbody == this.rb;
			}

			// Token: 0x04000519 RID: 1305
			public Rigidbody rb;
		}

		// Token: 0x0200010C RID: 268
		[CompilerGenerated]
		private sealed class <>c__DisplayClass22_0
		{
			// Token: 0x06000552 RID: 1362 RVA: 0x0001E572 File Offset: 0x0001C772
			public <>c__DisplayClass22_0()
			{
			}

			// Token: 0x06000553 RID: 1363 RVA: 0x0001E57A File Offset: 0x0001C77A
			internal bool <RequestOwnership>b__0(NetRigidbodyManager.OwnedRigidbody x)
			{
				return x.Rigidbody == this.rigidbody;
			}

			// Token: 0x0400051A RID: 1306
			public Rigidbody rigidbody;
		}

		// Token: 0x0200010D RID: 269
		[CompilerGenerated]
		private sealed class <>c__DisplayClass7_0
		{
			// Token: 0x06000554 RID: 1364 RVA: 0x0001E58D File Offset: 0x0001C78D
			public <>c__DisplayClass7_0()
			{
			}

			// Token: 0x06000555 RID: 1365 RVA: 0x0001E595 File Offset: 0x0001C795
			internal bool <Start>b__3(NetRigidbodyManager.OwnedRigidbody orb)
			{
				return orb.Rigidbody == this.rb;
			}

			// Token: 0x0400051B RID: 1307
			public Rigidbody rb;
		}
	}
}
