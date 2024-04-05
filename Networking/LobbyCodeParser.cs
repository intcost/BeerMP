using System;

namespace BeerMP.Networking
{
	// Token: 0x0200004C RID: 76
	internal static class LobbyCodeParser
	{
		// Token: 0x060001AC RID: 428 RVA: 0x0000864C File Offset: 0x0000684C
		internal static string GetString(ulong lobbyCode)
		{
			string text = "";
			int num = 0;
			while (num < 11 && lobbyCode != 0UL)
			{
				text += LobbyCodeParser.Get6BitChar((int)lobbyCode & 63).ToString();
				lobbyCode >>= 6;
				num++;
			}
			return text;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00008690 File Offset: 0x00006890
		internal static ulong GetUlong(string lobbyCode)
		{
			ulong num = 0UL;
			for (int i = 0; i < lobbyCode.Length; i++)
			{
				num |= (ulong)LobbyCodeParser.GetByte(lobbyCode[i]) << i * 6;
			}
			return num;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x000086CC File Offset: 0x000068CC
		private static char Get6BitChar(int b)
		{
			int num;
			if (b < 10)
			{
				num = 48 + b;
			}
			else if (b < 36)
			{
				num = 65 + (b - 10);
			}
			else if (b == 36)
			{
				num = 95;
			}
			else if (b < 63)
			{
				num = 97 + (b - 37);
			}
			else
			{
				num = 45;
			}
			return Convert.ToChar((byte)num);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000871C File Offset: 0x0000691C
		private static byte GetByte(char b)
		{
			int num = 0;
			if (b == '-')
			{
				num = 63;
			}
			else if (b < ':')
			{
				num = (int)(b - '0');
			}
			else if (b < '[')
			{
				num = (int)(b - 'A' + '\n');
			}
			else if (b == '_')
			{
				num = 36;
			}
			else if (b < '{')
			{
				num = (int)(b - 'a' + '%');
			}
			return (byte)num;
		}
	}
}
