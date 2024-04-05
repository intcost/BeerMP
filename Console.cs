using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace BeerMP
{
	// Token: 0x0200003F RID: 63
	public static class Console
	{
		// Token: 0x0600013D RID: 317 RVA: 0x00006E14 File Offset: 0x00005014
		internal static void UpdateLogDeleteTime()
		{
			for (int i = 0; i < Console.logs.Count; i++)
			{
				Console.logs[i].time -= Time.deltaTime;
				if (Console.logs[i].time <= 0f)
				{
					Console.logs.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00006E78 File Offset: 0x00005078
		internal static void Init()
		{
			Console.ts.Switch.Level = SourceLevels.All;
			Console.ts.Listeners.Add(Console.tw);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00006EA0 File Offset: 0x000050A0
		internal static void DrawGUI()
		{
			float num = 1f;
			Console.DrawGUIText(num, num, true);
			Console.DrawGUIText(-num, num, true);
			Console.DrawGUIText(num, -num, true);
			Console.DrawGUIText(-num, -num, true);
			Console.DrawGUIText(0f, 0f, false);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00006EE8 File Offset: 0x000050E8
		private static void DrawGUIText(float xOffset, float yOffset, bool isBlack)
		{
			GUILayout.BeginArea(new Rect(10f + xOffset, 10f + yOffset, (float)Screen.width - 20f, (float)Screen.height - 20f));
			for (int i = 0; i < Console.logs.Count; i++)
			{
				GUILayout.Label(isBlack ? (Console.logs[i].og ?? "").Size(20).Bold().Color("black") : (Console.logs[i].log ?? "").Size(20).Bold(), new GUILayoutOption[0]);
			}
			GUILayout.EndArea();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00006FA4 File Offset: 0x000051A4
		public static void Log(object message, bool show = true)
		{
			new Console.LogData(string.Concat(new string[]
			{
				"[",
				"INFO".Color("lime"),
				"] [",
				DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss").Color("yellow"),
				"]: ",
				message.ToString().Color("yellow")
			}), string.Format("[INFO] [{0}]: {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), message), show);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000703C File Offset: 0x0000523C
		public static void LogWarning(object message, bool show = true)
		{
			new Console.LogData(string.Concat(new string[]
			{
				"[",
				"WARNING".Color("orange"),
				"] [",
				DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss").Color("yellow"),
				"]: ",
				message.ToString().Color("yellow")
			}), string.Format("[WARNING] [{0}]: {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), message), show);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x000070D4 File Offset: 0x000052D4
		public static void LogError(object message, bool show = false)
		{
			new Console.LogData(string.Concat(new string[]
			{
				"[",
				"ERROR".Color("red"),
				"] [",
				DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss").Color("yellow"),
				"]: ",
				message.ToString().Color("yellow")
			}), string.Format("[ERROR] [{0}]: {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), message), show);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000716C File Offset: 0x0000536C
		// Note: this type is marked as 'beforefieldinit'.
		static Console()
		{
		}

		// Token: 0x0400012E RID: 302
		private static TraceSource ts = new TraceSource("BeerMP-Console");

		// Token: 0x0400012F RID: 303
		private static TextWriterTraceListener tw = new TextWriterTraceListener("BeerMP_output_log.txt");

		// Token: 0x04000130 RID: 304
		internal static List<Console.LogData> logs = new List<Console.LogData>();

		// Token: 0x020000E2 RID: 226
		internal class LogData
		{
			// Token: 0x060004B1 RID: 1201 RVA: 0x0001BF34 File Offset: 0x0001A134
			internal LogData(string message, string og, bool show)
			{
				this.log = message.ToString();
				this.og = og.ClearColor();
				Console.tw.WriteLine(og);
				Console.tw.Flush();
				if (show)
				{
					Console.logs.Insert(0, this);
				}
			}

			// Token: 0x0400045B RID: 1115
			public string log;

			// Token: 0x0400045C RID: 1116
			public string og;

			// Token: 0x0400045D RID: 1117
			public float time = 5f;
		}
	}
}
