using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking;
using HutongGames.PlayMaker;
using Steamworks;
using UnityEngine;

namespace BeerMP
{
	// Token: 0x0200003D RID: 61
	public class Chat : MonoBehaviour
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000133 RID: 307 RVA: 0x0000684B File Offset: 0x00004A4B
		private bool visible
		{
			get
			{
				return this.msg.Length > 0 || this.forceShow;
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00006864 File Offset: 0x00004A64
		private void Start()
		{
			NetEvent<Chat>.Register("ChatMessage", delegate(ulong sender, Packet p)
			{
				string text = p.ReadString(true);
				this.SendChatMessage(SteamFriends.GetFriendPersonaName((CSteamID)sender), text);
			});
			Action action = delegate
			{
				Chat.night = GameObject.Find("MAP").transform.Find("SUN/Pivot/SUN").GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("Night");
			};
			if (ObjectsLoader.IsGameLoaded)
			{
				action();
				return;
			}
			ObjectsLoader.gameLoaded += action;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000068C8 File Offset: 0x00004AC8
		private void OnGUI()
		{
			if (this.visible)
			{
				GUI.Label(new Rect(10f, (float)Screen.height - 80f, (float)Screen.width - 20f, (float)Screen.height - 20f), ("Chat message: ".Color("yellow") + this.msg).Size(20).Bold());
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00006938 File Offset: 0x00004B38
		private void Update()
		{
			string text;
			bool flag;
			if (this.visible && this.GetInput(out text, out flag))
			{
				this.forceShow = false;
				if (text == "" && this.msg.Length >= 1 && !flag)
				{
					this.msg = ((this.msg.Length == 1) ? "" : this.msg.Substring(0, this.msg.Length - 1));
				}
				else
				{
					this.msg += text;
				}
				if (flag)
				{
					if (this.msg.StartsWith("/"))
					{
						if (this.msg.Contains(' '))
						{
							string[] array = this.msg.Split(new char[] { ' ' });
							string[] array2 = new string[array.Length - 1];
							for (int i = 0; i < array2.Length; i++)
							{
								array2[i] = array[i + 1];
							}
							CommandHandler.Execute(array[0], array2);
						}
						else
						{
							CommandHandler.Execute(this.msg, new string[0]);
						}
					}
					else
					{
						this.SendChatMessage(SteamFriends.GetPersonaName(), this.msg);
						using (Packet packet = new Packet(1))
						{
							packet.Write(this.msg, -1);
							NetEvent<Chat>.Send("ChatMessage", packet, true);
						}
					}
					this.msg = "";
				}
			}
			if (!this.visible && Input.GetKeyDown(KeyCode.T))
			{
				this.forceShow = true;
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00006AC4 File Offset: 0x00004CC4
		private void SendChatMessage(string senderName, string message)
		{
			Console.Log(senderName.Bold() + ": " + message.Color("white"), true);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00006AE8 File Offset: 0x00004CE8
		private bool GetInput(out string s, out bool sendMessage)
		{
			s = "";
			sendMessage = false;
			for (int i = 0; i < Input.inputString.Length; i++)
			{
				char c = Input.inputString[i];
				if (c == '\b')
				{
					if (s.Length <= 0)
					{
						return true;
					}
					s = s.Substring(0, s.Length - 1);
				}
				else
				{
					if (c == '\n' || c == '\r')
					{
						sendMessage = true;
						return true;
					}
					s += c.ToString();
				}
			}
			return Input.inputString.Length > 0;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00006B71 File Offset: 0x00004D71
		public Chat()
		{
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00006B84 File Offset: 0x00004D84
		[CompilerGenerated]
		private void <Start>b__6_0(ulong sender, Packet p)
		{
			string text = p.ReadString(true);
			this.SendChatMessage(SteamFriends.GetFriendPersonaName((CSteamID)sender), text);
		}

		// Token: 0x04000129 RID: 297
		public static FsmBool night;

		// Token: 0x0400012A RID: 298
		private string msg = "";

		// Token: 0x0400012B RID: 299
		private bool forceShow;

		// Token: 0x0400012C RID: 300
		private const string messageEventName = "ChatMessage";

		// Token: 0x020000DF RID: 223
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004A1 RID: 1185 RVA: 0x0001B93C File Offset: 0x00019B3C
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004A2 RID: 1186 RVA: 0x0001B948 File Offset: 0x00019B48
			public <>c()
			{
			}

			// Token: 0x060004A3 RID: 1187 RVA: 0x0001B950 File Offset: 0x00019B50
			internal void <Start>b__6_1()
			{
				Chat.night = GameObject.Find("MAP").transform.Find("SUN/Pivot/SUN").GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("Night");
			}

			// Token: 0x04000452 RID: 1106
			public static readonly Chat.<>c <>9 = new Chat.<>c();

			// Token: 0x04000453 RID: 1107
			public static Action <>9__6_1;
		}
	}
}
