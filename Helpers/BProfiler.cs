using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Harmony;
using UnityEngine;

namespace BeerMP.Helpers
{
	// Token: 0x0200007E RID: 126
	public class BProfiler
	{
		// Token: 0x0600038E RID: 910 RVA: 0x0001A90B File Offset: 0x00018B0B
		internal static void Start()
		{
			Console.Log("Started Profiling".Color("lime"), true);
			BProfiler.doProfiling = true;
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0001A928 File Offset: 0x00018B28
		internal static void Stop(string filePath = "")
		{
			if (BProfiler.doProfiling)
			{
				Console.Log("Stopped Profiling".Color("red"), true);
			}
			else
			{
				BProfiler.execTimes.Clear();
				Console.Log("Cleared execution times".Color("teal"), true);
			}
			if (filePath != "" && BProfiler.doProfiling)
			{
				BProfiler.WriteToFile(filePath);
			}
			BProfiler.doProfiling = false;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0001A994 File Offset: 0x00018B94
		public static void Attach(Type targetType, bool attachNestedTypes = true)
		{
			try
			{
				if (BProfiler.instance == null)
				{
					BProfiler.instance = HarmonyInstance.Create("BeerMP.Profiler");
				}
				if (BProfiler.ts == null)
				{
					if (File.Exists("BeerMP_Profiler_output_log.txt"))
					{
						File.Delete("BeerMP_Profiler_output_log.txt");
					}
					BProfiler.tw = new TextWriterTraceListener("BeerMP_Profiler_output_log.txt");
					BProfiler.ts = new TraceSource("BeerMP_Profiler");
					BProfiler.ts.Switch.Level = SourceLevels.All;
					BProfiler.ts.Listeners.Add(BProfiler.tw);
				}
				if (targetType != typeof(BProfiler))
				{
					if (!targetType.IsGenericType)
					{
						Type[] nestedTypes = targetType.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
						MethodInfo[] methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
						for (int i = 0; i < methods.Length; i++)
						{
							if (methods[i].DeclaringType == targetType && !BProfiler.patched.Contains(methods[i]))
							{
								if (methods[i].IsGenericMethod)
								{
									BProfiler.tw.WriteLine("Warning: " + methods[i].FullDescription() + " is a generic method, not supported!");
									BProfiler.tw.Flush();
								}
								else if (methods[i].GetMethodBody() == null)
								{
									BProfiler.tw.WriteLine("Warning: " + methods[i].FullDescription() + " has no body");
									BProfiler.tw.Flush();
								}
								else
								{
									bool flag = BProfiler.instance.Patch(methods[i], new HarmonyMethod(typeof(BProfiler.Patch), "Prefix", null), new HarmonyMethod(typeof(BProfiler.Patch), "Postfix", null), null) != null;
									BProfiler.tw.WriteLine("Info: attached to " + methods[i].FullDescription());
									BProfiler.tw.Flush();
									if (flag)
									{
										BProfiler.patched.Add(methods[i]);
									}
								}
							}
						}
						if (attachNestedTypes)
						{
							for (int j = 0; j < nestedTypes.Length; j++)
							{
								BProfiler.Attach(nestedTypes[j], true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				BProfiler.tw.WriteLine(string.Format("Error: {0}", ex));
				BProfiler.tw.Flush();
			}
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0001ABB8 File Offset: 0x00018DB8
		internal static void Reset()
		{
			BProfiler.doProfiling = false;
			BProfiler.Stop("");
			if (BProfiler.instance != null)
			{
				BProfiler.instance.UnpatchAll(null);
			}
			BProfiler.patched = new List<MethodBase>();
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0001ABE8 File Offset: 0x00018DE8
		internal static void DoGUI()
		{
			string text = "";
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
			GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(string.Format("<color=white>[BeerMP Profiler] max samples: {0}</color>", BProfiler.maxSamples), "", new GUILayoutOption[]
			{
				GUILayout.MaxHeight((float)Screen.height),
				GUILayout.ExpandWidth(true)
			});
			GUILayout.Space(10f);
			BProfiler.scrollRect = GUILayout.BeginScrollView(BProfiler.scrollRect, "", "", new GUILayoutOption[0]);
			foreach (KeyValuePair<MethodBase, Queue<long>> keyValuePair in BProfiler.execTimes.OrderBy((KeyValuePair<MethodBase, Queue<long>> x) => x.Key.DeclaringType.FullName))
			{
				string fullName = keyValuePair.Key.DeclaringType.FullName;
				string name = keyValuePair.Key.Name;
				if (text != fullName)
				{
					GUILayout.Label("<color=white>" + fullName + ":</color>", new GUILayoutOption[0]);
					text = fullName;
				}
				GUILayout.Label(string.Format("<color=white>    {0}: {1}ms  {2}ticks</color>", name, (keyValuePair.Value.Count > 0) ? BProfiler.avg(BProfiler.execTimes[keyValuePair.Key], true) : 0.0, (keyValuePair.Value.Count > 0) ? BProfiler.avg(BProfiler.execTimes[keyValuePair.Key], false) : 0.0), new GUILayoutOption[0]);
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001ADE8 File Offset: 0x00018FE8
		internal static void WriteToFile(string fileName)
		{
			string text = "";
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<MethodBase, Queue<long>> keyValuePair in BProfiler.execTimes.OrderBy((KeyValuePair<MethodBase, Queue<long>> x) => x.Key.DeclaringType.FullName))
			{
				string fullName = keyValuePair.Key.DeclaringType.FullName;
				string name = keyValuePair.Key.Name;
				if (text != fullName)
				{
					if (stringBuilder.Length <= 0)
					{
						stringBuilder.AppendLine(string.Format("[BeerMP Profiler] max samples: {0}", BProfiler.maxSamples));
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(fullName + ":");
					text = fullName;
				}
				stringBuilder.AppendLine("\n    [" + name + "]" + string.Format("\n     average execution time: {0}ms  {1}ticks", (keyValuePair.Value.Count > 0) ? BProfiler.avg(BProfiler.execTimes[keyValuePair.Key], true) : 0.0, (keyValuePair.Value.Count > 0) ? BProfiler.avg(BProfiler.execTimes[keyValuePair.Key], false) : 0.0));
			}
			File.WriteAllText(fileName, stringBuilder.ToString());
			Console.Log("Saved Profiling results to '" + fileName + "' in Game Folder", true);
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0001AF8C File Offset: 0x0001918C
		private static double avg(Queue<long> times, bool milliseconds)
		{
			float num = 0f;
			double num2 = 0.0;
			List<long> list = times.ToList<long>();
			if (times.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					num += (float)list[i];
				}
				num2 = (double)(num / (float)list.Count);
			}
			return num2 / (double)(milliseconds ? 10000L : 1L);
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0001AFF0 File Offset: 0x000191F0
		public BProfiler()
		{
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0001AFF8 File Offset: 0x000191F8
		// Note: this type is marked as 'beforefieldinit'.
		static BProfiler()
		{
		}

		// Token: 0x0400037F RID: 895
		private static TraceSource ts;

		// Token: 0x04000380 RID: 896
		private static TextWriterTraceListener tw;

		// Token: 0x04000381 RID: 897
		private static HarmonyInstance instance;

		// Token: 0x04000382 RID: 898
		private static Dictionary<MethodBase, Queue<long>> execTimes = new Dictionary<MethodBase, Queue<long>>();

		// Token: 0x04000383 RID: 899
		public static int maxSamples = 10;

		// Token: 0x04000384 RID: 900
		private static bool doProfiling = false;

		// Token: 0x04000385 RID: 901
		private static List<MethodBase> patched = new List<MethodBase>();

		// Token: 0x04000386 RID: 902
		private static Vector2 scrollRect;

		// Token: 0x02000151 RID: 337
		private class Patch
		{
			// Token: 0x0600063C RID: 1596 RVA: 0x000212BA File Offset: 0x0001F4BA
			private static bool Prefix(out long __state)
			{
				__state = 0L;
				if (!BProfiler.doProfiling)
				{
					return true;
				}
				__state = Stopwatch.GetTimestamp();
				return true;
			}

			// Token: 0x0600063D RID: 1597 RVA: 0x000212D4 File Offset: 0x0001F4D4
			private static void Postfix(MethodBase __originalMethod, long __state)
			{
				if (!BProfiler.doProfiling)
				{
					return;
				}
				long timestamp = Stopwatch.GetTimestamp();
				if (!BProfiler.execTimes.ContainsKey(__originalMethod))
				{
					BProfiler.execTimes.Add(__originalMethod, new Queue<long>());
				}
				if (__state < timestamp)
				{
					BProfiler.execTimes[__originalMethod].Enqueue(timestamp - __state);
				}
				if (BProfiler.execTimes[__originalMethod].Count > BProfiler.maxSamples)
				{
					BProfiler.execTimes[__originalMethod].Dequeue();
				}
			}

			// Token: 0x0600063E RID: 1598 RVA: 0x0002134B File Offset: 0x0001F54B
			public Patch()
			{
			}
		}

		// Token: 0x02000152 RID: 338
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600063F RID: 1599 RVA: 0x00021353 File Offset: 0x0001F553
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000640 RID: 1600 RVA: 0x0002135F File Offset: 0x0001F55F
			public <>c()
			{
			}

			// Token: 0x06000641 RID: 1601 RVA: 0x00021367 File Offset: 0x0001F567
			internal string <DoGUI>b__12_0(KeyValuePair<MethodBase, Queue<long>> x)
			{
				return x.Key.DeclaringType.FullName;
			}

			// Token: 0x06000642 RID: 1602 RVA: 0x0002137A File Offset: 0x0001F57A
			internal string <WriteToFile>b__13_0(KeyValuePair<MethodBase, Queue<long>> x)
			{
				return x.Key.DeclaringType.FullName;
			}

			// Token: 0x040005C3 RID: 1475
			public static readonly BProfiler.<>c <>9 = new BProfiler.<>c();

			// Token: 0x040005C4 RID: 1476
			public static Func<KeyValuePair<MethodBase, Queue<long>>, string> <>9__12_0;

			// Token: 0x040005C5 RID: 1477
			public static Func<KeyValuePair<MethodBase, Queue<long>>, string> <>9__13_0;
		}
	}
}
