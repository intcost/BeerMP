using System;
using System.Collections.Generic;
using BeerMP.Helpers;
using BeerMP.Networking.Managers;
using Discord;
using Steamworks;

namespace BeerMP.Networking
{
	// Token: 0x0200004A RID: 74
	internal class SteamNet
	{
		// Token: 0x06000192 RID: 402 RVA: 0x00007EA8 File Offset: 0x000060A8
		public static CSteamID[] GetMembers()
		{
			LobbyID lobbyID = SteamNet.currentLobby;
			int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
			CSteamID[] array = new CSteamID[numLobbyMembers];
			for (int i = 0; i < numLobbyMembers; i++)
			{
				array[i] = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
			}
			return array;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00007EF0 File Offset: 0x000060F0
		public static void CloseConnections()
		{
			for (int i = 0; i < SteamNet.p2pConnections.Count; i++)
			{
				SteamNet.CloseConnection(SteamNet.p2pConnections[i]);
				SteamNet.p2pConnections.Remove(SteamNet.p2pConnections[i]);
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00007F38 File Offset: 0x00006138
		public static void CloseConnection(CSteamID user)
		{
			SteamNetworking.CloseP2PSessionWithUser(user);
			SteamNet.p2pConnections.Remove(user);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00007F4D File Offset: 0x0000614D
		private static void OnP2PSessionConnectFail(P2PSessionConnectFail_t cb)
		{
			Console.LogError(string.Format("P2P Connection failed {0}", (EP2PSessionError)cb.m_eP2PSessionError), false);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00007F6C File Offset: 0x0000616C
		private static void OnP2PSessionRequest(P2PSessionRequest_t cb)
		{
			Console.Log(string.Format("P2P Session requested by {0}", cb.m_steamIDRemote), true);
			if (SteamNetworking.AcceptP2PSessionWithUser(cb.m_steamIDRemote))
			{
				NetManager.OnMemberConnect(cb.m_steamIDRemote, SteamNet.GetMembers());
				if (!SteamNet.p2pConnections.Contains(cb.m_steamIDRemote))
				{
					SteamNet.p2pConnections.Add(cb.m_steamIDRemote);
				}
				Console.Log(string.Format("Accepting P2P Session with {0} successful.", cb.m_steamIDRemote.m_SteamID), true);
				SteamNet.SetActivity();
				return;
			}
			Console.LogError(string.Format("Accepting P2P Session with {0} failed.", cb.m_steamIDRemote.m_SteamID), false);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00008019 File Offset: 0x00006219
		private static void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t cb)
		{
			SteamMatchmaking.JoinLobby(cb.m_steamIDLobby);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00008028 File Offset: 0x00006228
		private static void OnLobbyCreated(LobbyCreated_t cb)
		{
			SteamNet.p2pConnections = new List<CSteamID>();
			ulong ulSteamIDLobby = cb.m_ulSteamIDLobby;
			SteamMatchmaking.SetLobbyData((CSteamID)ulSteamIDLobby, "ver", "v0.1.15");
			SteamMatchmaking.SetLobbyData((CSteamID)ulSteamIDLobby, "gver", SteamApps.GetAppBuildId().ToString());
			SteamNet.currentLobby = new LobbyID(LobbyCodeParser.GetString(ulSteamIDLobby));
			if (cb.m_eResult == EResult.k_EResultOK)
			{
				Console.Log(string.Format("Created Lobby {0}", SteamNet.currentLobby), true);
				Clipboard.text = SteamNet.currentLobby.ToString();
				Console.Log("Copied Lobby ID to Clipboard.", true);
				NetManager.OnLobbyCreate(SteamNet.GetMembers(), SteamUser.GetSteamID());
				return;
			}
			Console.LogError(string.Format("Creating Lobby failed. {0}", cb.m_eResult), false);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x000080F8 File Offset: 0x000062F8
		private static void OnLobbyEnter(LobbyEnter_t cb)
		{
			SteamNet.p2pConnections = new List<CSteamID>();
			if (cb.m_EChatRoomEnterResponse == 1U)
			{
				SteamNet.currentLobby = new LobbyID(LobbyCodeParser.GetString(cb.m_ulSteamIDLobby));
				NetManager.OnConnect(SteamNet.GetMembers(), SteamUser.GetSteamID());
				Console.Log(string.Format("Joined Lobby {0}", SteamNet.currentLobby), true);
				NetManager.maxPlayers = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)cb.m_ulSteamIDLobby);
				using (Packet packet = new Packet())
				{
					packet.InsertString("BEERMP_IGNORE");
					NetManager.SendReliable(packet, default(CSteamID));
				}
				SteamNet.SetActivity();
				return;
			}
			Console.LogError(string.Format("Joining Lobby failed. {0}", (EChatRoomEnterResponse)cb.m_EChatRoomEnterResponse), false);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x000081CC File Offset: 0x000063CC
		public static void Start()
		{
			SteamAPI.Init();
			SteamNet.onP2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(new Callback<P2PSessionConnectFail_t>.DispatchDelegate(SteamNet.OnP2PSessionConnectFail));
			SteamNet.onP2PSessionRequest = Callback<P2PSessionRequest_t>.Create(new Callback<P2PSessionRequest_t>.DispatchDelegate(SteamNet.OnP2PSessionRequest));
			SteamNet.onGameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(SteamNet.OnGameLobbyJoinRequested));
			SteamNet.onLobbyCreated = Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(SteamNet.OnLobbyCreated));
			SteamNet.onLobbyEnter = Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(SteamNet.OnLobbyEnter));
			SteamNet.onLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(new Callback<LobbyChatUpdate_t>.DispatchDelegate(SteamNet.OnLobbyMemberStateUpdate));
			SteamNet.onLobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(SteamNet.OnLobbyDataUpdate));
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000827C File Offset: 0x0000647C
		private static void OnLobbyDataUpdate(LobbyDataUpdate_t param)
		{
			if (!SteamNet.joiningLobby)
			{
				return;
			}
			SteamNet.joiningLobby = false;
			CSteamID csteamID = (CSteamID)param.m_ulSteamIDLobby;
			string @string = LobbyCodeParser.GetString(param.m_ulSteamIDLobby);
			string lobbyData = SteamMatchmaking.GetLobbyData(csteamID, "gver");
			string lobbyData2 = SteamMatchmaking.GetLobbyData(csteamID, "ver");
			string text = SteamApps.GetAppBuildId().ToString();
			if (string.IsNullOrEmpty(lobbyData))
			{
				Console.LogError("Can't join lobby " + @string + " because it's still loading (ERR: gver null)", true);
				return;
			}
			if (lobbyData != text)
			{
				Console.LogError(string.Concat(new string[] { "Can't join lobby ", @string, " because it's targetting game version ", lobbyData, " (current version is ", text, ")" }), true);
				return;
			}
			if (string.IsNullOrEmpty(lobbyData2))
			{
				Console.LogError("Can't join lobby " + @string + " because it's still loading (ERR: ver null)", true);
				return;
			}
			if (lobbyData2 != "v0.1.15")
			{
				Console.LogError(string.Concat(new string[] { "Can't join lobby ", @string, " because it's targetting BeerMP version ", lobbyData2, " (current version is v0.1.15)" }), true);
				return;
			}
			SteamMatchmaking.JoinLobby(csteamID);
			Console.Log("Trying to Join Lobby (" + @string + ")...", true);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000083BC File Offset: 0x000065BC
		private static void OnLobbyMemberStateUpdate(LobbyChatUpdate_t param)
		{
			CSteamID csteamID = (CSteamID)param.m_ulSteamIDUserChanged;
			if (param.m_rgfChatMemberStateChange == 2U)
			{
				if (param.m_ulSteamIDUserChanged == BeerMPGlobals.HostID)
				{
					NetManager.Disconnect();
					Console.Log("Session closed. Host disconnected.", true);
					return;
				}
				SteamNet.CloseConnection(csteamID);
				NetManager.OnMemberDisconnect(csteamID);
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008408 File Offset: 0x00006608
		public static void JoinLobby(string steamIDLobby)
		{
			if (!SteamMatchmaking.RequestLobbyData((CSteamID)LobbyCodeParser.GetUlong(steamIDLobby)))
			{
				Console.LogError("Can't join lobby " + steamIDLobby + " failed to request lobby data", true);
				return;
			}
			SteamNet.joiningLobby = true;
			Console.Log("Requesting Lobby data ...", true);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00008444 File Offset: 0x00006644
		public static void CreateLobby(CSteamID owner, int maxPlayers, ELobbyType lobbyType)
		{
			NetManager.connectedPlayers = new List<CSteamID>();
			SteamMatchmaking.CreateLobby(lobbyType, maxPlayers);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00008458 File Offset: 0x00006658
		public static void GetNetworkData()
		{
			uint num;
			while (SteamNetworking.IsP2PPacketAvailable(out num, 0))
			{
				byte[] array = new byte[num];
				uint num2 = 0U;
				CSteamID csteamID;
				if (SteamNetworking.ReadP2PPacket(array, num, out num2, out csteamID, 0) && csteamID != SteamUser.GetSteamID())
				{
					NetManager.OnReceive(csteamID.m_SteamID, array);
				}
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x000084A4 File Offset: 0x000066A4
		public static void CheckConnections()
		{
			foreach (CSteamID csteamID in SteamNet.p2pConnections)
			{
				P2PSessionState_t p2PSessionState_t;
				SteamNetworking.GetP2PSessionState(csteamID, out p2PSessionState_t);
				bool flag = Convert.ToBoolean(p2PSessionState_t.m_bConnecting);
				bool flag2 = Convert.ToBoolean(p2PSessionState_t.m_bConnectionActive);
				if (!flag && !flag2)
				{
					if (SteamNet.p2pConnections.Contains(csteamID))
					{
						SteamNet.p2pConnections.Remove(csteamID);
					}
					NetManager.OnMemberDisconnect(csteamID);
					SteamNet.SetActivity();
				}
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00008538 File Offset: 0x00006738
		private static void SetActivity()
		{
			SteamNet.activity = new Activity
			{
				State = string.Format("Playing Online ({0} of {1})", NetManager.connectedPlayers.Count, NetManager.maxPlayers),
				Details = "Roaming the roads of Alivieska",
				Type = ActivityType.Playing,
				Timestamps = new ActivityTimestamps
				{
					Start = DateTime.Now.ToUnixTimestamp()
				},
				Instance = true
			};
			BeerMP.UpdateActivity(SteamNet.activity);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x000085C4 File Offset: 0x000067C4
		public SteamNet()
		{
		}

		// Token: 0x04000147 RID: 327
		public static LobbyID currentLobby;

		// Token: 0x04000148 RID: 328
		private static List<CSteamID> p2pConnections;

		// Token: 0x04000149 RID: 329
		private static Callback<P2PSessionConnectFail_t> onP2PSessionConnectFail;

		// Token: 0x0400014A RID: 330
		private static Callback<P2PSessionRequest_t> onP2PSessionRequest;

		// Token: 0x0400014B RID: 331
		private static Callback<GameLobbyJoinRequested_t> onGameLobbyJoinRequested;

		// Token: 0x0400014C RID: 332
		private static Callback<LobbyCreated_t> onLobbyCreated;

		// Token: 0x0400014D RID: 333
		private static Callback<LobbyEnter_t> onLobbyEnter;

		// Token: 0x0400014E RID: 334
		private static Callback<LobbyChatUpdate_t> onLobbyChatUpdate;

		// Token: 0x0400014F RID: 335
		private static Callback<LobbyDataUpdate_t> onLobbyDataUpdate;

		// Token: 0x04000150 RID: 336
		private static Activity activity;

		// Token: 0x04000151 RID: 337
		private static bool joiningLobby;
	}
}
