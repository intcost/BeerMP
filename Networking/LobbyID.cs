using System;
using Steamworks;

namespace BeerMP.Networking
{
	// Token: 0x0200004B RID: 75
	internal struct LobbyID
	{
		// Token: 0x060001A3 RID: 419 RVA: 0x000085CC File Offset: 0x000067CC
		public LobbyID(CSteamID cSteamID, string lobbyCode)
		{
			this.m_SteamID = cSteamID;
			this.m_LobbyCode = lobbyCode;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x000085DC File Offset: 0x000067DC
		public LobbyID(CSteamID cSteamID)
		{
			this.m_SteamID = cSteamID;
			this.m_LobbyCode = LobbyCodeParser.GetString(cSteamID.m_SteamID);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000085F6 File Offset: 0x000067F6
		public LobbyID(string lobbyCode)
		{
			this.m_SteamID = new CSteamID(LobbyCodeParser.GetUlong(lobbyCode));
			this.m_LobbyCode = lobbyCode;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00008610 File Offset: 0x00006810
		public static explicit operator LobbyID(string lobbyCode)
		{
			return new LobbyID(lobbyCode);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00008618 File Offset: 0x00006818
		public static explicit operator LobbyID(CSteamID cSteamID)
		{
			return new LobbyID(cSteamID);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00008620 File Offset: 0x00006820
		public static implicit operator CSteamID(LobbyID lobbyID)
		{
			return lobbyID.m_SteamID;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00008628 File Offset: 0x00006828
		public static implicit operator string(LobbyID lobbyID)
		{
			return lobbyID.m_LobbyCode;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00008630 File Offset: 0x00006830
		public override string ToString()
		{
			return this.m_LobbyCode;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00008638 File Offset: 0x00006838
		public override int GetHashCode()
		{
			return this.m_SteamID.m_SteamID.GetHashCode();
		}

		// Token: 0x04000152 RID: 338
		public CSteamID m_SteamID;

		// Token: 0x04000153 RID: 339
		public string m_LobbyCode;
	}
}
