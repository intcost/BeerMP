using System;
using System.Collections.Generic;
using System.IO;
using BeerMP.Networking.Managers;
using Steamworks;
using UnityEngine;

namespace BeerMP.Helpers
{
	// Token: 0x02000084 RID: 132
	internal class SatsumaProfiler
	{
		// Token: 0x060003B3 RID: 947 RVA: 0x0001B6B8 File Offset: 0x000198B8
		internal SatsumaProfiler(Rigidbody satuma)
		{
			this.satsuma = satuma;
			SatsumaProfiler.Instance = this;
			Console.Log("Satsuma profiler initialized", true);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0001B71C File Offset: 0x0001991C
		internal void Update(bool receivedRBupdate, ulong owner)
		{
			this.logs[this.currentPosition] = string.Format("[{0}] Velocity: {1} ({2}), received update: {3}, owner: {4}", new object[]
			{
				Time.timeSinceLevelLoad,
				this.satsuma.velocity,
				this.satsuma.velocity.magnitude,
				receivedRBupdate,
				NetManager.playerNames[(CSteamID)owner]
			});
			if (this.attached.Count > 0)
			{
				string text = "\n Attached: ";
				for (int i = 0; i < this.attached.Count; i++)
				{
					if (i > 0)
					{
						text += ", ";
					}
					text += this.attached[i];
				}
				string[] array = this.logs;
				int num = this.currentPosition;
				array[num] += text;
			}
			if (this.detached.Count > 0)
			{
				string text2 = "\n Detached: ";
				for (int j = 0; j < this.detached.Count; j++)
				{
					if (j > 0)
					{
						text2 += ", ";
					}
					text2 += this.detached[j];
				}
				string[] array2 = this.logs;
				int num2 = this.currentPosition;
				array2[num2] += text2;
			}
			this.attached.Clear();
			this.detached.Clear();
			this.currentPosition++;
			if (this.currentPosition >= this.logs.Length)
			{
				this.currentPosition = 0;
			}
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0001B8AC File Offset: 0x00019AAC
		internal void PrintToFile()
		{
			string text = "";
			int num = this.currentPosition;
			int num2 = this.currentPosition;
			do
			{
				text = text + this.logs[num2] + "\n";
				num2++;
				if (num2 == this.logs.Length)
				{
					num2 = 0;
				}
			}
			while (num2 != num);
			File.WriteAllText("satsuma_profiler.txt", text);
		}

		// Token: 0x04000390 RID: 912
		internal static SatsumaProfiler Instance;

		// Token: 0x04000391 RID: 913
		internal List<string> attached = new List<string>();

		// Token: 0x04000392 RID: 914
		internal List<string> detached = new List<string>();

		// Token: 0x04000393 RID: 915
		private Rigidbody satsuma;

		// Token: 0x04000394 RID: 916
		private string[] logs = new string[Mathf.RoundToInt(10f * (1f / Time.fixedDeltaTime))];

		// Token: 0x04000395 RID: 917
		private int currentPosition;
	}
}
