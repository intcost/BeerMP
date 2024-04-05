using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.Managers;
using BeerMP.Properties;
using Discord;
using Steamworks;
using UnityEngine;

namespace BeerMP
{
	// Token: 0x0200003C RID: 60
	internal class BeerMP : MonoBehaviour
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000126 RID: 294 RVA: 0x000064E9 File Offset: 0x000046E9
		// (set) Token: 0x06000127 RID: 295 RVA: 0x000064F1 File Offset: 0x000046F1
		public global::Discord.Discord discord
		{
			[CompilerGenerated]
			get
			{
				return this.<discord>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<discord>k__BackingField = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000064FA File Offset: 0x000046FA
		private static long ApplicationID
		{
			[CompilerGenerated]
			get
			{
				return BeerMP.<ApplicationID>k__BackingField;
			}
		} = long.Parse(BeerMP.Properties.Resources.clientID);

		// Token: 0x06000129 RID: 297 RVA: 0x00006504 File Offset: 0x00004704
		private void Awake()
		{
			SteamAPI.Init();
			if (SteamApps.GetAppBuildId() < 100)
			{
				Debug.Log(string.Format("BEERMP CAN'T LOAD: APP BUILD ID = {0}, PLEASE UPDATE YOUR GAME", SteamApps.GetAppBuildId()));
				Application.Quit();
				return;
			}
			BeerMP.instance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.netman = base.gameObject.AddComponent<NetManager>();
			Environment.SetEnvironmentVariable("BeerMP-Present", "fuck MSCO lmao - brenn 2024");
			try
			{
				this.discord = new global::Discord.Discord(BeerMP.ApplicationID, 1UL);
				if (this.discord != null)
				{
					BeerMP.activityManager = this.discord.GetActivityManager();
				}
			}
			catch (Exception ex)
			{
				this.discord = null;
				Debug.LogError(ex.Message);
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000065C0 File Offset: 0x000047C0
		private void OnLevelWasLoaded(int levelId)
		{
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000065CD File Offset: 0x000047CD
		private void OnGUI()
		{
			Console.DrawGUI();
			this.Watermark();
		}

		// Token: 0x0600012C RID: 300
		private void Watermark()
		{
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(("BeerMP cracked by intcost".Color("yellow") + " | " + "v0.1.15 | Telegram: @BeerMP".Color("yellow")).Size(16), new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000066C0 File Offset: 0x000048C0
		private void Update()
		{
			if (this.discord != null)
			{
				this.discord.RunCallbacks();
			}
			SteamAPI.RunCallbacks();
			if (Application.loadedLevelName == "MainMenu" && !this.init)
			{
				Debug.Log("[BeerMP Init]");
				Console.Init();
				this.netman.Init();
				this.init = true;
				BeerMP.ResetActivity();
			}
			if (Input.GetKey(KeyCode.Alpha0))
			{
				Console.Log(Camera.main.transform.GetGameobjectHashString(), true);
			}
			Console.UpdateLogDeleteTime();
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00006748 File Offset: 0x00004948
		public static void ResetActivity()
		{
			if (BeerMP.instance.discord == null)
			{
				return;
			}
			BeerMP.activity = new Activity
			{
				State = "Idling",
				Timestamps = new ActivityTimestamps
				{
					Start = DateTime.Now.ToUnixTimestamp()
				}
			};
			BeerMP.UpdateActivity(BeerMP.activity);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000067A8 File Offset: 0x000049A8
		public static void UpdateActivity(Activity activity)
		{
			if (BeerMP.instance.discord == null)
			{
				return;
			}
			activity.ApplicationId = BeerMP.ApplicationID;
			activity.Assets = new ActivityAssets
			{
				LargeImage = "beermp_logo",
				LargeText = "No alcohol is no solution."
			};
			BeerMP.activityManager.UpdateActivity(activity, delegate(Result res)
			{
				if (res == Result.Ok)
				{
					Console.Log("Discord: Status Updated", true);
					return;
				}
				Console.LogError(string.Format("Discord: Status Update failed! {0}", res), false);
			});
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00006820 File Offset: 0x00004A20
		private void OnApplicationQuit()
		{
			Environment.SetEnvironmentVariable("BeerMP-Present", null);
			SteamAPI.Shutdown();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00006832 File Offset: 0x00004A32
		public BeerMP()
		{
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000683A File Offset: 0x00004A3A
		static BeerMP()
		{
		}

		// Token: 0x0400011E RID: 286
		public static BeerMP instance;

		// Token: 0x0400011F RID: 287
		internal NetManager netman;

		// Token: 0x04000120 RID: 288
		[CompilerGenerated]
		private global::Discord.Discord <discord>k__BackingField;

		// Token: 0x04000121 RID: 289
		private bool init;

		// Token: 0x04000122 RID: 290
		public static bool debug;

		// Token: 0x04000123 RID: 291
		[CompilerGenerated]
		private static readonly long <ApplicationID>k__BackingField;

		// Token: 0x04000124 RID: 292
		private static Activity activity;

		// Token: 0x04000125 RID: 293
		private static ActivityAssets activityAssets;

		// Token: 0x04000126 RID: 294
		private static ActivityManager activityManager;

		// Token: 0x04000127 RID: 295
		private const string _version = "v0.1.15";

		// Token: 0x04000128 RID: 296
		public const string version = "v0.1.15";

		// Token: 0x020000DE RID: 222
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600049E RID: 1182 RVA: 0x0001B901 File Offset: 0x00019B01
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600049F RID: 1183 RVA: 0x0001B90D File Offset: 0x00019B0D
			public <>c()
			{
			}

			// Token: 0x060004A0 RID: 1184 RVA: 0x0001B915 File Offset: 0x00019B15
			internal void <UpdateActivity>b__22_0(Result res)
			{
				if (res == Result.Ok)
				{
					Console.Log("Discord: Status Updated", true);
					return;
				}
				Console.LogError(string.Format("Discord: Status Update failed! {0}", res), false);
			}

			// Token: 0x04000450 RID: 1104
			public static readonly BeerMP.<>c <>9 = new BeerMP.<>c();

			// Token: 0x04000451 RID: 1105
			public static ActivityManager.UpdateActivityHandler <>9__22_0;
		}
	}
}
