using System;
using System.Reflection;
using BeerMP.Networking;
using BeerMP.Networking.Managers;
using UnityEngine;

namespace BeerMP.Helpers
{
	// Token: 0x02000081 RID: 129
	[ManagerCreate(10000)]
	public class ObjectsLoader : MonoBehaviour
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x0001B230 File Offset: 0x00019430
		public static GameObject[] ObjectsInGame
		{
			get
			{
				return ObjectsLoader.objectsInGame;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x0001B237 File Offset: 0x00019437
		public static bool IsGameLoaded
		{
			get
			{
				return ObjectsLoader.isGameLoaded;
			}
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0001B240 File Offset: 0x00019440
		public ObjectsLoader()
		{
			if (BeerMPGlobals.ModLoaderInstalled)
			{
				Type type = BeerMPGlobals.mscloader.GetType("MSCLoader.ModLoader");
				Console.Log(string.Format("modloader null {0}", type == null), true);
				this.modloaderInstance = type.GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				if (this.modloaderInstance == null)
				{
					Console.LogError("ModLoader instance is null but it is present", false);
					return;
				}
				this.allModsLoaded = type.GetField("allModsLoaded", BindingFlags.Instance | BindingFlags.NonPublic);
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0001B2C3 File Offset: 0x000194C3
		private void Start()
		{
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0001B2C8 File Offset: 0x000194C8
		private void Update()
		{
			if (!ObjectsLoader.isGameLoaded && GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera") != null && (!BeerMPGlobals.ModLoaderInstalled || (bool)this.allModsLoaded.GetValue(this.modloaderInstance)))
			{
				ObjectsLoader.isGameLoaded = true;
				ObjectsLoader.objectsInGame = Resources.FindObjectsOfTypeAll<GameObject>();
				ActionContainer actionContainer = ObjectsLoader.gameLoaded;
				if (actionContainer != null)
				{
					actionContainer.Invoke();
				}
				if (!BeerMPGlobals.IsHost)
				{
					using (Packet packet = new Packet())
					{
						NetEvent<NetManager>.Send("PlayerLoaded", packet, true);
					}
				}
				Console.Log("Game loaded!", true);
			}
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0001B374 File Offset: 0x00019574
		private void OnDestroy()
		{
			ObjectsLoader.objectsInGame = null;
			ObjectsLoader.gameLoaded = null;
			ObjectsLoader.isGameLoaded = false;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0001B388 File Offset: 0x00019588
		// Note: this type is marked as 'beforefieldinit'.
		static ObjectsLoader()
		{
		}

		// Token: 0x04000388 RID: 904
		private static GameObject[] objectsInGame;

		// Token: 0x04000389 RID: 905
		public static ActionContainer gameLoaded = new ActionContainer();

		// Token: 0x0400038A RID: 906
		private static bool isGameLoaded = false;

		// Token: 0x0400038B RID: 907
		private FieldInfo allModsLoaded;

		// Token: 0x0400038C RID: 908
		private object modloaderInstance;
	}
}
