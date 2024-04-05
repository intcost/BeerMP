using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.PlayerManagers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Steamworks;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000071 RID: 113
	internal class NetManager : MonoBehaviour
	{
		// Token: 0x06000335 RID: 821 RVA: 0x0001807E File Offset: 0x0001627E
		public static T GetPlayerComponentById<T>(ulong id) where T : class
		{
			return NetManager.GetPlayerComponentById<T>((CSteamID)id);
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0001808B File Offset: 0x0001628B
		public static T GetPlayerComponentById<T>(CSteamID id) where T : class
		{
			return NetManager.userObjects[id].FirstOrDefault((Component x) => x.GetType() == typeof(T)) as T;
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000337 RID: 823 RVA: 0x000180C6 File Offset: 0x000162C6
		private static List<string> MscloaderLoadAssetsAssetNames
		{
			get
			{
				FieldInfo fieldInfo = NetManager.f_MscloaderLoadAssetsAssetNames;
				return ((fieldInfo != null) ? fieldInfo.GetValue(null) : null) as List<string>;
			}
		}

		// Token: 0x06000338 RID: 824 RVA: 0x000180E0 File Offset: 0x000162E0
		public static void AddBeerMPSyncs()
		{
			NetManager.SystemContainer = new GameObject("BeerMP_NetManagers");
			try
			{
				Type[] array = typeof(NetManager).Assembly.GetTypes();
				array = array.Where((Type x) => x.GetCustomAttributes(typeof(ManagerCreate), true).Length != 0 && typeof(MonoBehaviour).IsAssignableFrom(x)).OrderByDescending((Type x) => x.GetCustomAttributes(typeof(ManagerCreate), true).Sum((object x) => ((ManagerCreate)x).priority)).ToArray<Type>();
				for (int i = 0; i < array.Length; i++)
				{
					Console.Log("created '" + array[i].Name + "'.", false);
					NetManager.SystemContainer.AddComponent(array[i]);
				}
			}
			catch (Exception ex)
			{
				Console.LogError(ex, false);
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x000181B4 File Offset: 0x000163B4
		internal static void SendSavegame(CSteamID player)
		{
			using (Packet packet = new Packet(1, GameScene.MainMenu))
			{
				string text = Application.persistentDataPath + "/defaultES2File.txt";
				string text2 = Application.persistentDataPath + "/items.txt";
				bool flag = File.Exists(text) && File.Exists(text2);
				packet.Write(flag, -1);
				if (flag)
				{
					byte[] array = File.ReadAllBytes(text);
					packet.Write(array.Length, -1);
					packet.Write(array, -1);
					array = File.ReadAllBytes(text2);
					packet.Write(array.Length, -1);
					packet.Write(array, -1);
				}
				NetEvent<NetManager>.Send("Savegame", packet, player.m_SteamID, true);
			}
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0001826C File Offset: 0x0001646C
		internal static void RecieveSavegame(ulong userId, Packet packet)
		{
			if (NetManager.savegameRecieved)
			{
				return;
			}
			NetManager.savegameRecieved = true;
			string text = Application.persistentDataPath + "/defaultES2File.txt";
			string text2 = Application.persistentDataPath + "/items.txt";
			bool flag = (NetManager.hadSavegame = File.Exists(text) && File.Exists(text2));
			bool flag2 = packet.ReadBool(true);
			if (flag)
			{
				File.Copy(text, Application.persistentDataPath + "/defaultES2File.bak", true);
				File.Copy(text2, Application.persistentDataPath + "/items.bak", true);
			}
			if (flag2)
			{
				File.WriteAllBytes(text, packet.ReadBytes(packet.ReadInt(true), true));
				File.WriteAllBytes(text2, packet.ReadBytes(packet.ReadInt(true), true));
			}
			else
			{
				File.Delete(text);
				File.Delete(text2);
			}
			SceneLoader.LoadScene(GameScene.GAME);
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00018334 File Offset: 0x00016534
		internal static void OnLobbyCreate(CSteamID[] players, CSteamID userId)
		{
			NetManager.IsHost = true;
			NetManager.IsConnected = true;
			NetManager.HostID = SteamMatchmaking.GetLobbyOwner(SteamNet.currentLobby);
			NetManager.UserID = SteamUser.GetSteamID();
			NetManager.connectedPlayers = new List<CSteamID>();
			for (int i = 0; i < players.Length; i++)
			{
				NetManager.connectedPlayers.Add(players[i]);
			}
			SceneLoader.LoadScene(GameScene.GAME);
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001839C File Offset: 0x0001659C
		internal static void OnMemberConnect(CSteamID userId, params CSteamID[] players)
		{
			NetManager.connectedPlayers = new List<CSteamID>();
			for (int i = 0; i < players.Length; i++)
			{
				NetManager.connectedPlayers.Add(players[i]);
			}
			NetManager.loadingPlayers.Add(userId);
			NetManager.CreateUserObjects();
			if (NetManager.IsHost)
			{
				NetManager.SendSavegame(userId);
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x000183EF File Offset: 0x000165EF
		internal static void OnMemberDisconnect(CSteamID userId)
		{
			NetManager.DeleteUserObjects(userId);
			NetManager.connectedPlayers.Remove(userId);
			Console.Log(SteamFriends.GetFriendPersonaName(userId) + " left.", true);
			ActionContainer<ulong> onMemberExit = BeerMPGlobals.OnMemberExit;
			if (onMemberExit == null)
			{
				return;
			}
			onMemberExit.Invoke(userId.m_SteamID);
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00018430 File Offset: 0x00016630
		internal static void OnConnect(CSteamID[] players, CSteamID userId)
		{
			NetManager.IsConnected = true;
			NetManager.connectedPlayers = new List<CSteamID>();
			for (int i = 0; i < players.Length; i++)
			{
				NetManager.connectedPlayers.Add(players[i]);
			}
			NetManager.UserID = SteamUser.GetSteamID();
			NetManager.HostID = SteamMatchmaking.GetLobbyOwner(SteamNet.currentLobby);
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0001848C File Offset: 0x0001668C
		internal static void OnReceive(ulong sender, byte[] data)
		{
			using (Packet packet = new Packet(data))
			{
				NetEventWorker.HandlePacket(sender, packet);
			}
		}

		// Token: 0x06000340 RID: 832 RVA: 0x000184C4 File Offset: 0x000166C4
		private void OnLevelWasLoaded(int levelId)
		{
			string loadedLevelName = Application.loadedLevelName;
			if (!(loadedLevelName == "MainMenu"))
			{
				if (loadedLevelName == "GAME")
				{
					NetManager.currentScene = GameScene.GAME;
					if (NetManager.IsConnected && !NetManager.init)
					{
						NetManager.init = true;
						NetManager.CreateUserObjects();
						NetManager.AddBeerMPSyncs();
						if (!BeerMPGlobals.IsHost)
						{
							NetManager.DisableSaveFSM();
						}
						NetManager.SystemContainer.AddComponent<LocalNetPlayer>();
					}
				}
			}
			else
			{
				NetManager.currentScene = GameScene.MainMenu;
				NetManager.init = false;
				this.StartMenu();
			}
			Action<GameScene> action = NetManager.sceneLoaded;
			if (action == null)
			{
				return;
			}
			action(NetManager.currentScene);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x00018556 File Offset: 0x00016756
		private static void DisableSaveFSM()
		{
			ObjectsLoader.gameLoaded += delegate
			{
				for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
				{
					if (!(ObjectsLoader.ObjectsInGame[i].name != "SAVEGAME"))
					{
						PlayMakerFSM component = ObjectsLoader.ObjectsInGame[i].GetComponent<PlayMakerFSM>();
						if (!(component == null) && !(component.FsmName != "Button"))
						{
							component.Initialize();
							FsmState state = component.GetState("Wait for click");
							if (state == null)
							{
								Console.LogError("Wait for click is null on " + component.transform.GetGameobjectHashString(), false);
							}
							else
							{
								SetStringValue setStringValue = state.Actions.First((FsmStateAction a) => a is SetStringValue) as SetStringValue;
								if (setStringValue == null)
								{
									Console.LogError("Wait for click string is null on " + component.transform.GetGameobjectHashString(), false);
								}
								else
								{
									setStringValue.stringValue = "DISCONNECT";
									FsmState state2 = component.GetState("Mute audio");
									if (state2 == null)
									{
										Console.LogError("Mute audio is null on " + component.transform.GetGameobjectHashString(), false);
									}
									else
									{
										SetStringValue setStringValue2 = state2.Actions.First((FsmStateAction a) => a is SetStringValue) as SetStringValue;
										if (setStringValue2 == null)
										{
											Console.LogError("Mute audio string is null on " + component.transform.GetGameobjectHashString(), false);
										}
										else
										{
											setStringValue2.stringValue = "DISCONNECTING...";
											state2.Transitions[0].ToState = "Load menu";
											component.Initialize();
										}
									}
								}
							}
						}
					}
				}
			};
		}

		// Token: 0x06000342 RID: 834 RVA: 0x00018588 File Offset: 0x00016788
		private static void CreateUserObjects()
		{
			for (int i = 0; i < NetManager.connectedPlayers.Count; i++)
			{
				if (!(NetManager.connectedPlayers[i] == SteamUser.GetSteamID()) && !NetManager.userObjects.ContainsKey(NetManager.connectedPlayers[i]))
				{
					NetManager.userObjects.Add(NetManager.connectedPlayers[i], new List<Component>());
					if (!NetManager.userObjects[NetManager.connectedPlayers[i]].Any((Component x) => x.GetType() != typeof(NetPlayer)))
					{
						NetPlayer netPlayer = BeerMP.instance.gameObject.AddComponent<NetPlayer>();
						netPlayer.steamID = NetManager.connectedPlayers[i];
						if (!NetManager.loadingPlayers.Contains(NetManager.connectedPlayers[i]))
						{
							netPlayer.disableModel = false;
						}
						NetManager.userObjects[NetManager.connectedPlayers[i]].Add(netPlayer);
					}
				}
			}
		}

		// Token: 0x06000343 RID: 835 RVA: 0x00018694 File Offset: 0x00016894
		private static void DeleteUserObjects(CSteamID steamID)
		{
			if (NetManager.userObjects.ContainsKey(steamID))
			{
				foreach (Component component in NetManager.userObjects[steamID])
				{
					global::UnityEngine.Object.Destroy(component);
				}
			}
			NetManager.userObjects.Remove(steamID);
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00018704 File Offset: 0x00016904
		public static void SendReliable(Packet packet, CSteamID user = default(CSteamID))
		{
			if (!NetManager.IsConnected)
			{
				return;
			}
			byte[] array = packet.ToArray();
			if (user == default(CSteamID))
			{
				for (int i = 0; i < NetManager.connectedPlayers.Count; i++)
				{
					if (NetManager.connectedPlayers[i].m_SteamID != BeerMPGlobals.UserID)
					{
						SteamNetworking.SendP2PPacket(NetManager.connectedPlayers[i], array, (uint)array.Length, EP2PSend.k_EP2PSendReliable, 0);
					}
				}
				return;
			}
			SteamNetworking.SendP2PPacket(user, array, (uint)array.Length, EP2PSend.k_EP2PSendReliable, 0);
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00018784 File Offset: 0x00016984
		public static void SendUnreliable(Packet packet, CSteamID user = default(CSteamID))
		{
			if (!NetManager.IsConnected)
			{
				return;
			}
			byte[] array = packet.ToArray();
			if (user == default(CSteamID))
			{
				for (int i = 0; i < NetManager.connectedPlayers.Count; i++)
				{
					if (NetManager.connectedPlayers[i].m_SteamID != BeerMPGlobals.UserID)
					{
						SteamNetworking.SendP2PPacket(NetManager.connectedPlayers[i], array, (uint)array.Length, EP2PSend.k_EP2PSendUnreliable, 0);
					}
				}
				return;
			}
			SteamNetworking.SendP2PPacket(user, array, (uint)array.Length, EP2PSend.k_EP2PSendUnreliable, 0);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00018804 File Offset: 0x00016A04
		private void StartMenu()
		{
			Console.Log("StartMenu", true);
			GameObject gameObject = GameObject.Find("Interface/Buttons");
			if (gameObject != null)
			{
				gameObject.GetComponent<PlayMakerFSM>().enabled = false;
				this.menuButtons = new PlayMakerFSM[]
				{
					this.InitMenuButton(gameObject.transform.Find("ButtonContinue"), "OPEN LOBBY", delegate
					{
						this.showingLobbyDialog = 1;
					}),
					this.InitMenuButton(gameObject.transform.Find("ButtonNewgame"), "JOIN LOBBY", delegate
					{
						this.showingLobbyDialog = 2;
					})
				};
			}
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0001889C File Offset: 0x00016A9C
		private PlayMakerFSM InitMenuButton(Transform parent, string name, Action click)
		{
			PlayMakerFSM component = parent.GetComponent<PlayMakerFSM>();
			FsmState fsmState = component.FsmStates.First((FsmState s) => s.Name == "Action");
			string clickStateName = fsmState.Transitions.First((FsmTransition t) => t.EventName == "DOWN").ToState;
			FsmState fsmState2 = component.FsmStates.First((FsmState s) => s.Name == clickStateName);
			FsmState fsmState3 = fsmState2;
			FsmTransition[] array = new FsmTransition[1];
			int num = 0;
			FsmTransition fsmTransition = new FsmTransition();
			fsmTransition.FsmEvent = component.FsmEvents.First((FsmEvent e) => e.Name == "FINISHED");
			fsmTransition.ToState = component.FsmStates[0].Name;
			array[num] = fsmTransition;
			fsmState3.Transitions = array;
			fsmState2.Actions = new FsmStateAction[]
			{
				new PlayMakerUtilities.PM_Hook(click, false),
				new Wait
				{
					time = 0.2f
				}
			};
			TextMesh component2 = parent.GetChild(0).GetComponent<TextMesh>();
			TextMesh component3 = component2.transform.GetChild(0).GetComponent<TextMesh>();
			component3.text = name;
			component2.text = name;
			return component;
		}

		// Token: 0x06000348 RID: 840 RVA: 0x000189E8 File Offset: 0x00016BE8
		private void Update()
		{
			if (NetManager.currentScene == GameScene.MainMenu && BeerMPGlobals.ModLoaderInstalled)
			{
				if (this.doMenuReset && !Application.isLoadingLevel)
				{
					GameObject[] array = global::UnityEngine.Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						if (!(array[i].name == "MSCUnloader") && !(array[i].name == "BeerMP"))
						{
							global::UnityEngine.Object.Destroy(array[i]);
						}
					}
					GameObject[] array2 = Resources.FindObjectsOfTypeAll<GameObject>().Where((GameObject x) => !x.activeInHierarchy && x.transform.parent == null).ToArray<GameObject>();
					List<string> mscloaderLoadAssetsAssetNames = NetManager.MscloaderLoadAssetsAssetNames;
					for (int j = 0; j < array.Length; j++)
					{
						if (mscloaderLoadAssetsAssetNames.Contains(array2[j].name.ToLower()))
						{
							global::UnityEngine.Object.Destroy(array2[j]);
						}
					}
					PlayMakerGlobals.Instance.Variables.FindFsmBool("SongImported").Value = false;
					Application.LoadLevel(Application.loadedLevelName);
					this.doMenuReset = false;
				}
				if (this.showingLobbyDialog != 0 && Input.GetKeyDown(KeyCode.Escape))
				{
					this.showingLobbyDialog = 0;
				}
				for (int k = 0; k < this.menuButtons.Length; k++)
				{
					if (!this.menuButtons[k].enabled)
					{
						this.menuButtons[k].enabled = true;
					}
				}
			}
			if (NetManager.IsConnected)
			{
				SteamNet.GetNetworkData();
			}
			if (NetManager.SystemContainer != null && !NetManager.SystemContainer.activeSelf)
			{
				NetManager.SystemContainer.SetActive(true);
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00018B71 File Offset: 0x00016D71
		private void FixedUpdate()
		{
			if (NetManager.IsConnected)
			{
				SteamNet.CheckConnections();
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00018B80 File Offset: 0x00016D80
		internal void Init()
		{
			SteamNet.Start();
			NetManager.userObjects = new Dictionary<CSteamID, List<Component>>();
			NetEvent<NetManager>.Register("Savegame", new NetEventHandler(NetManager.RecieveSavegame));
			NetEvent<NetManager>.Register("PlayerLoaded", delegate(ulong sender, Packet _p)
			{
				NetManager.loadingPlayers.Remove((CSteamID)sender);
				NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)sender).player.SetActive(true);
				ActionContainer<ulong> onMemberReady = BeerMPGlobals.OnMemberReady;
				if (onMemberReady == null)
				{
					return;
				}
				onMemberReady.Invoke(sender);
			});
			if (Application.loadedLevelName == "MainMenu")
			{
				NetManager.currentScene = GameScene.MainMenu;
			}
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00018BF4 File Offset: 0x00016DF4
		public static void Disconnect()
		{
			if (NetManager.savegameRecieved && NetManager.hadSavegame)
			{
				File.Copy(Application.persistentDataPath + "/defaultES2File.bak", Application.persistentDataPath + "/defaultES2File.txt", true);
				File.Copy(Application.persistentDataPath + "/items.bak", Application.persistentDataPath + "/items.txt", true);
			}
			NetManager.IsConnected = false;
			SteamNet.CloseConnections();
			NetManager.connectedPlayers = new List<CSteamID>();
			NetManager.init = false;
			if (BeerMPGlobals.ModLoaderInstalled)
			{
				global::UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(BeerMPGlobals.mscloader.GetType("MSCLoader.MSCUnloader"));
				if (array.Length != 0)
				{
					GameObject gameObject = (array[0] as MonoBehaviour).gameObject;
					gameObject.SetActive(false);
					global::UnityEngine.Object.Destroy(gameObject);
					BeerMP.instance.netman.doMenuReset = true;
				}
			}
			SceneLoader.LoadScene(GameScene.MainMenu);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00018CC4 File Offset: 0x00016EC4
		private void OnApplicationQuit()
		{
			SteamNet.CloseConnections();
			if (NetManager.savegameRecieved)
			{
				File.Copy(Application.persistentDataPath + "/defaultES2File.bak", Application.persistentDataPath + "/defaultES2File.txt", true);
				File.Copy(Application.persistentDataPath + "/items.bak", Application.persistentDataPath + "/items.txt", true);
			}
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00018D28 File Offset: 0x00016F28
		private void OnGUI()
		{
			if (!NetManager.IsConnected)
			{
				if (this.showingLobbyDialog > 0)
				{
					this.windowRect = GUI.Window(0, this.windowRect, delegate(int id)
					{
						GUI.DragWindow(new Rect(0f, 0f, float.PositiveInfinity, 20f));
						if (this.showingLobbyDialog == 1)
						{
							NetManager.maxPlayers = Mathf.Clamp(int.Parse(GUILayout.TextField(NetManager.maxPlayers.ToString(), new GUILayoutOption[0])), 2, 250);
							if (GUILayout.Button("Open lobby", new GUILayoutOption[0]))
							{
								SteamNet.CreateLobby(SteamUser.GetSteamID(), NetManager.maxPlayers, ELobbyType.k_ELobbyTypePublic);
							}
							if (!char.IsNumber(Event.current.character))
							{
								Event.current.character = '\0';
								return;
							}
						}
						else
						{
							this.lobbyCode = GUILayout.TextField(this.lobbyCode.ToString(), new GUILayoutOption[0]);
							GUI.enabled = false;
							if (this.lobbyCode != "")
							{
								GUI.enabled = true;
							}
							if (GUILayout.Button("Join lobby", new GUILayoutOption[0]))
							{
								SteamNet.JoinLobby(this.lobbyCode);
							}
							GUI.enabled = true;
						}
					}, (this.showingLobbyDialog == 1) ? "Open lobby" : "Join lobby");
					return;
				}
			}
			else
			{
				GUILayout.BeginArea(new Rect(5f, 5f, (float)(Screen.width - 10), (float)(Screen.height - 10)));
				CSteamID[] array = NetManager.connectedPlayers.ToArray();
				if (this.cam == null)
				{
					FsmGameObject fsmGameObject = FsmVariables.GlobalVariables.FindFsmGameObject("POV");
					if (fsmGameObject != null)
					{
						this.cam = fsmGameObject.Value.GetComponent<Camera>();
					}
				}
				GUILayout.Label(string.Format("Current Lobby ID: {0}", SteamNet.currentLobby), new GUILayoutOption[0]);
				GUILayout.Label("Connected players:", new GUILayoutOption[0]);
				for (int i = 0; i < array.Length; i++)
				{
					if (!NetManager.playerNames.ContainsKey(array[i]))
					{
						CSteamID steamID = SteamUser.GetSteamID();
						if (array[i] != steamID)
						{
							string friendPersonaName = SteamFriends.GetFriendPersonaName(array[i]);
							NetManager.playerNames.Add(array[i], friendPersonaName);
						}
						else
						{
							NetManager.playerNames.Add(array[i], SteamFriends.GetPersonaName());
						}
					}
					else
					{
						GUILayout.Label(string.Concat(new string[]
						{
							NetManager.playerNames[array[i]],
							" ",
							(array[i] == SteamMatchmaking.GetLobbyOwner(SteamNet.currentLobby)) ? " <color=red>[Host]</color>" : " <color=lime>[Client]</color>",
							" ",
							NetManager.loadingPlayers.Contains(array[i]) ? "Loading...".Color("orange") : ""
						}), new GUILayoutOption[0]);
						if (NetManager.userObjects.ContainsKey(array[i]) && array[i].m_SteamID != BeerMPGlobals.UserID && this.cam != null && !NetManager.loadingPlayers.Contains(array[i]))
						{
							NetPlayer playerComponentById = NetManager.GetPlayerComponentById<NetPlayer>(array[i]);
							if (playerComponentById != null)
							{
								Vector3 headPos = playerComponentById.HeadPos;
								Vector3 position = this.cam.transform.position;
								if ((this.cam.transform.position + this.cam.transform.forward - headPos).sqrMagnitude < (position - headPos).sqrMagnitude)
								{
									this.DrawUsername(this.cam, headPos, NetManager.playerNames[array[i]]);
								}
							}
						}
					}
				}
				GUILayout.EndArea();
			}
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00019014 File Offset: 0x00017214
		private void DrawUsername(Camera cam, Vector3 worldPosition, string name)
		{
			Vector3 vector = cam.WorldToScreenPoint(worldPosition + Vector3.up * 0.5f);
			float magnitude = (worldPosition - cam.transform.position).magnitude;
			GUIStyle guistyle = new GUIStyle
			{
				fontSize = Mathf.FloorToInt(36f / magnitude)
			};
			if (guistyle.fontSize == 0)
			{
				return;
			}
			vector.x -= guistyle.CalcSize(new GUIContent(name)).x / 2f;
			GUI.Label(new Rect(vector.x, (float)Screen.height - vector.y, (float)Screen.width, (float)Screen.height), name.Color("white"), guistyle);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000190D0 File Offset: 0x000172D0
		public NetManager()
		{
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00019128 File Offset: 0x00017328
		// Note: this type is marked as 'beforefieldinit'.
		static NetManager()
		{
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00019199 File Offset: 0x00017399
		[CompilerGenerated]
		private void <StartMenu>b__37_0()
		{
			this.showingLobbyDialog = 1;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x000191A2 File Offset: 0x000173A2
		[CompilerGenerated]
		private void <StartMenu>b__37_1()
		{
			this.showingLobbyDialog = 2;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x000191AC File Offset: 0x000173AC
		[CompilerGenerated]
		private void <OnGUI>b__48_0(int id)
		{
			GUI.DragWindow(new Rect(0f, 0f, float.PositiveInfinity, 20f));
			if (this.showingLobbyDialog == 1)
			{
				NetManager.maxPlayers = Mathf.Clamp(int.Parse(GUILayout.TextField(NetManager.maxPlayers.ToString(), new GUILayoutOption[0])), 2, 250);
				if (GUILayout.Button("Open lobby", new GUILayoutOption[0]))
				{
					SteamNet.CreateLobby(SteamUser.GetSteamID(), NetManager.maxPlayers, ELobbyType.k_ELobbyTypePublic);
				}
				if (!char.IsNumber(Event.current.character))
				{
					Event.current.character = '\0';
					return;
				}
			}
			else
			{
				this.lobbyCode = GUILayout.TextField(this.lobbyCode.ToString(), new GUILayoutOption[0]);
				GUI.enabled = false;
				if (this.lobbyCode != "")
				{
					GUI.enabled = true;
				}
				if (GUILayout.Button("Join lobby", new GUILayoutOption[0]))
				{
					SteamNet.JoinLobby(this.lobbyCode);
				}
				GUI.enabled = true;
			}
		}

		// Token: 0x0400033B RID: 827
		private static Dictionary<CSteamID, List<Component>> userObjects = new Dictionary<CSteamID, List<Component>>();

		// Token: 0x0400033C RID: 828
		public static AssetBundle assetBundle;

		// Token: 0x0400033D RID: 829
		public static GameObject SystemContainer;

		// Token: 0x0400033E RID: 830
		public static GameScene currentScene = GameScene.Unknown;

		// Token: 0x0400033F RID: 831
		public static Action<GameScene> sceneLoaded;

		// Token: 0x04000340 RID: 832
		public static int maxPlayers = 2;

		// Token: 0x04000341 RID: 833
		public static bool IsConnected;

		// Token: 0x04000342 RID: 834
		public static bool IsHost;

		// Token: 0x04000343 RID: 835
		public static CSteamID HostID;

		// Token: 0x04000344 RID: 836
		public static CSteamID UserID;

		// Token: 0x04000345 RID: 837
		private static bool init = false;

		// Token: 0x04000346 RID: 838
		private static bool savegameRecieved;

		// Token: 0x04000347 RID: 839
		private static bool hadSavegame;

		// Token: 0x04000348 RID: 840
		internal static List<CSteamID> connectedPlayers = new List<CSteamID>();

		// Token: 0x04000349 RID: 841
		internal static List<CSteamID> loadingPlayers = new List<CSteamID>();

		// Token: 0x0400034A RID: 842
		private static readonly FieldInfo f_MscloaderLoadAssetsAssetNames = ((!BeerMPGlobals.ModLoaderInstalled) ? null : BeerMPGlobals.mscloader.GetType("MSCLoader.LoadAssets").GetField("assetNames", BindingFlags.Static | BindingFlags.NonPublic));

		// Token: 0x0400034B RID: 843
		private int showingLobbyDialog;

		// Token: 0x0400034C RID: 844
		private bool doMenuReset;

		// Token: 0x0400034D RID: 845
		private PlayMakerFSM[] menuButtons;

		// Token: 0x0400034E RID: 846
		private Rect windowRect = new Rect((float)Screen.width / 2f - 150f, (float)Screen.height / 2f - 32.5f, 300f, 65f);

		// Token: 0x0400034F RID: 847
		public static Dictionary<CSteamID, string> playerNames = new Dictionary<CSteamID, string>();

		// Token: 0x04000350 RID: 848
		private string lobbyCode = "";

		// Token: 0x04000351 RID: 849
		private Camera cam;

		// Token: 0x02000142 RID: 322
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060005F7 RID: 1527 RVA: 0x00020575 File Offset: 0x0001E775
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060005F8 RID: 1528 RVA: 0x00020581 File Offset: 0x0001E781
			public <>c()
			{
			}

			// Token: 0x060005F9 RID: 1529 RVA: 0x00020589 File Offset: 0x0001E789
			internal bool <AddBeerMPSyncs>b__22_0(Type x)
			{
				return x.GetCustomAttributes(typeof(ManagerCreate), true).Length != 0 && typeof(MonoBehaviour).IsAssignableFrom(x);
			}

			// Token: 0x060005FA RID: 1530 RVA: 0x000205B1 File Offset: 0x0001E7B1
			internal int <AddBeerMPSyncs>b__22_1(Type x)
			{
				return x.GetCustomAttributes(typeof(ManagerCreate), true).Sum((object x) => ((ManagerCreate)x).priority);
			}

			// Token: 0x060005FB RID: 1531 RVA: 0x000205E8 File Offset: 0x0001E7E8
			internal int <AddBeerMPSyncs>b__22_2(object x)
			{
				return ((ManagerCreate)x).priority;
			}

			// Token: 0x060005FC RID: 1532 RVA: 0x000205F8 File Offset: 0x0001E7F8
			internal void <DisableSaveFSM>b__31_0()
			{
				for (int i = 0; i < ObjectsLoader.ObjectsInGame.Length; i++)
				{
					if (!(ObjectsLoader.ObjectsInGame[i].name != "SAVEGAME"))
					{
						PlayMakerFSM component = ObjectsLoader.ObjectsInGame[i].GetComponent<PlayMakerFSM>();
						if (!(component == null) && !(component.FsmName != "Button"))
						{
							component.Initialize();
							FsmState state = component.GetState("Wait for click");
							if (state == null)
							{
								Console.LogError("Wait for click is null on " + component.transform.GetGameobjectHashString(), false);
							}
							else
							{
								SetStringValue setStringValue = state.Actions.First((FsmStateAction a) => a is SetStringValue) as SetStringValue;
								if (setStringValue == null)
								{
									Console.LogError("Wait for click string is null on " + component.transform.GetGameobjectHashString(), false);
								}
								else
								{
									setStringValue.stringValue = "DISCONNECT";
									FsmState state2 = component.GetState("Mute audio");
									if (state2 == null)
									{
										Console.LogError("Mute audio is null on " + component.transform.GetGameobjectHashString(), false);
									}
									else
									{
										SetStringValue setStringValue2 = state2.Actions.First((FsmStateAction a) => a is SetStringValue) as SetStringValue;
										if (setStringValue2 == null)
										{
											Console.LogError("Mute audio string is null on " + component.transform.GetGameobjectHashString(), false);
										}
										else
										{
											setStringValue2.stringValue = "DISCONNECTING...";
											state2.Transitions[0].ToState = "Load menu";
											component.Initialize();
										}
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x060005FD RID: 1533 RVA: 0x000207A9 File Offset: 0x0001E9A9
			internal bool <DisableSaveFSM>b__31_1(FsmStateAction a)
			{
				return a is SetStringValue;
			}

			// Token: 0x060005FE RID: 1534 RVA: 0x000207B4 File Offset: 0x0001E9B4
			internal bool <DisableSaveFSM>b__31_2(FsmStateAction a)
			{
				return a is SetStringValue;
			}

			// Token: 0x060005FF RID: 1535 RVA: 0x000207BF File Offset: 0x0001E9BF
			internal bool <CreateUserObjects>b__32_0(Component x)
			{
				return x.GetType() != typeof(NetPlayer);
			}

			// Token: 0x06000600 RID: 1536 RVA: 0x000207D6 File Offset: 0x0001E9D6
			internal bool <InitMenuButton>b__38_0(FsmState s)
			{
				return s.Name == "Action";
			}

			// Token: 0x06000601 RID: 1537 RVA: 0x000207E8 File Offset: 0x0001E9E8
			internal bool <InitMenuButton>b__38_1(FsmTransition t)
			{
				return t.EventName == "DOWN";
			}

			// Token: 0x06000602 RID: 1538 RVA: 0x000207FA File Offset: 0x0001E9FA
			internal bool <InitMenuButton>b__38_3(FsmEvent e)
			{
				return e.Name == "FINISHED";
			}

			// Token: 0x06000603 RID: 1539 RVA: 0x0002080C File Offset: 0x0001EA0C
			internal bool <Update>b__39_0(GameObject x)
			{
				return !x.activeInHierarchy && x.transform.parent == null;
			}

			// Token: 0x06000604 RID: 1540 RVA: 0x00020829 File Offset: 0x0001EA29
			internal void <Init>b__41_0(ulong sender, Packet _p)
			{
				NetManager.loadingPlayers.Remove((CSteamID)sender);
				NetManager.GetPlayerComponentById<NetPlayer>((CSteamID)sender).player.SetActive(true);
				ActionContainer<ulong> onMemberReady = BeerMPGlobals.OnMemberReady;
				if (onMemberReady == null)
				{
					return;
				}
				onMemberReady.Invoke(sender);
			}

			// Token: 0x04000594 RID: 1428
			public static readonly NetManager.<>c <>9 = new NetManager.<>c();

			// Token: 0x04000595 RID: 1429
			public static Func<Type, bool> <>9__22_0;

			// Token: 0x04000596 RID: 1430
			public static Func<object, int> <>9__22_2;

			// Token: 0x04000597 RID: 1431
			public static Func<Type, int> <>9__22_1;

			// Token: 0x04000598 RID: 1432
			public static Func<FsmStateAction, bool> <>9__31_1;

			// Token: 0x04000599 RID: 1433
			public static Func<FsmStateAction, bool> <>9__31_2;

			// Token: 0x0400059A RID: 1434
			public static Action <>9__31_0;

			// Token: 0x0400059B RID: 1435
			public static Func<Component, bool> <>9__32_0;

			// Token: 0x0400059C RID: 1436
			public static Func<FsmState, bool> <>9__38_0;

			// Token: 0x0400059D RID: 1437
			public static Func<FsmTransition, bool> <>9__38_1;

			// Token: 0x0400059E RID: 1438
			public static Func<FsmEvent, bool> <>9__38_3;

			// Token: 0x0400059F RID: 1439
			public static Func<GameObject, bool> <>9__39_0;

			// Token: 0x040005A0 RID: 1440
			public static NetEventHandler <>9__41_0;
		}

		// Token: 0x02000143 RID: 323
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c__2<T> where T : class
		{
			// Token: 0x06000605 RID: 1541 RVA: 0x00020862 File Offset: 0x0001EA62
			// Note: this type is marked as 'beforefieldinit'.
			static <>c__2()
			{
			}

			// Token: 0x06000606 RID: 1542 RVA: 0x0002086E File Offset: 0x0001EA6E
			public <>c__2()
			{
			}

			// Token: 0x06000607 RID: 1543 RVA: 0x00020876 File Offset: 0x0001EA76
			internal bool <GetPlayerComponentById>b__2_0(Component x)
			{
				return x.GetType() == typeof(T);
			}

			// Token: 0x040005A1 RID: 1441
			public static readonly NetManager.<>c__2<T> <>9 = new NetManager.<>c__2<T>();

			// Token: 0x040005A2 RID: 1442
			public static Func<Component, bool> <>9__2_0;
		}

		// Token: 0x02000144 RID: 324
		[CompilerGenerated]
		private sealed class <>c__DisplayClass38_0
		{
			// Token: 0x06000608 RID: 1544 RVA: 0x0002088A File Offset: 0x0001EA8A
			public <>c__DisplayClass38_0()
			{
			}

			// Token: 0x06000609 RID: 1545 RVA: 0x00020892 File Offset: 0x0001EA92
			internal bool <InitMenuButton>b__2(FsmState s)
			{
				return s.Name == this.clickStateName;
			}

			// Token: 0x040005A3 RID: 1443
			public string clickStateName;
		}
	}
}
