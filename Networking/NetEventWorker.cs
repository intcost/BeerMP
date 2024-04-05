using System;
using System.Collections.Generic;
using BeerMP.Helpers;
using BeerMP.Networking.Managers;

namespace BeerMP.Networking
{
	// Token: 0x02000046 RID: 70
	internal static class NetEventWorker
	{
		// Token: 0x0600015C RID: 348 RVA: 0x00007530 File Offset: 0x00005730
		internal static void HandlePacket(ulong sender, Packet packet)
		{
			try
			{
				string text = packet.ReadString(true);
				if (!(text == "BEERMP_IGNORE"))
				{
					string text2 = packet.ReadString(true);
					int num = packet.ReadInt(true);
					GameScene gameScene = (GameScene)packet.ReadInt(true);
					if (num != 1)
					{
						Console.Log(string.Format("received Packet for NetEvent {0} of {1}\nTarget Scene: {2}", text2, text, gameScene), true);
					}
					if (NetManager.currentScene == gameScene && gameScene != GameScene.Unknown && (gameScene != GameScene.GAME || ObjectsLoader.IsGameLoaded))
					{
						if (!NetEventWorker.eventHandlers.ContainsKey(text))
						{
							Console.LogWarning("missing Event Handler Dictionary for " + text, true);
						}
						else if (!NetEventWorker.eventHandlers[text].ContainsKey(text2))
						{
							Console.LogWarning(string.Concat(new string[] { "missing Event Handler for ", text2, " (parent: ", text, ")" }), true);
						}
						else
						{
							NetEventHandler netEventHandler = NetEventWorker.eventHandlers[text][text2];
							if (netEventHandler != null)
							{
								netEventHandler(sender, packet);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.LogError(ex, false);
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00007644 File Offset: 0x00005844
		// Note: this type is marked as 'beforefieldinit'.
		static NetEventWorker()
		{
		}

		// Token: 0x0400013D RID: 317
		internal static Dictionary<string, Dictionary<string, NetEventHandler>> eventHandlers = new Dictionary<string, Dictionary<string, NetEventHandler>>();
	}
}
