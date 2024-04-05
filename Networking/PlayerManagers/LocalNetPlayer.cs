using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Properties;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x0200004F RID: 79
	internal class LocalNetPlayer : MonoBehaviour
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x000099EB File Offset: 0x00007BEB
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x000099F2 File Offset: 0x00007BF2
		public static LocalNetPlayer Instance
		{
			[CompilerGenerated]
			get
			{
				return LocalNetPlayer.<Instance>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				LocalNetPlayer.<Instance>k__BackingField = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x000099FA File Offset: 0x00007BFA
		public Transform playerRoot
		{
			get
			{
				return this.player.Value.transform.root;
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00009A11 File Offset: 0x00007C11
		private void Awake()
		{
			AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(BeerMP.Properties.Resources.clothes);
			CharacterCustomization.LoadTextures(assetBundle);
			assetBundle.Unload(false);
			LocalNetPlayer.Instance = this;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00009A30 File Offset: 0x00007C30
		private void Start()
		{
			string text = "PlayerJoined";
			NetEvent<LocalNetPlayer>.Register(text, delegate(ulong player, Packet p)
			{
				ActionContainer<ulong> onMemberJoin = BeerMPGlobals.OnMemberJoin;
				if (onMemberJoin == null)
				{
					return;
				}
				onMemberJoin.Invoke(player);
			});
			using (Packet packet = new Packet(0))
			{
				NetEvent<LocalNetPlayer>.Send(text, packet, true);
				this.player = FsmVariables.GlobalVariables.FindFsmGameObject("SavePlayer");
				this.headTrans = FsmVariables.GlobalVariables.FindFsmGameObject("SavePlayerCam");
				PlayerAnimationManager.RegisterEvents();
				base.gameObject.AddComponent<LocalPlayerAnimationManager>();
				ObjectsLoader.gameLoaded += delegate
				{
					AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(BeerMP.Properties.Resources.clothes);
					Console.Log("charcustom init", false);
					this.characterCustomization = CharacterCustomization.Init(assetBundle);
					Console.Log("charcustom init 2", false);
					BeerMPGlobals.OnMemberReady += delegate(ulong userId)
					{
						this.characterCustomization.InitialSkinSync(null, userId);
					};
					new GameObject("BeerMPChat").AddComponent<Chat>();
					assetBundle.Unload(false);
					this.EditDeath();
				};
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00009AEC File Offset: 0x00007CEC
		private void EditDeath()
		{
			this.death = GameObject.Find("Systems").transform.Find("Death").gameObject;
			FsmState fsmState = this.death.GetPlayMaker("Activate Dead Body").FsmStates[0];
			fsmState.Actions = new FsmStateAction[0];
			fsmState.Transitions = new FsmTransition[0];
			this.death.AddComponent<PlayerDeathManager>();
			this.pvc = base.gameObject.AddComponent<ProximityVoiceChat>();
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00009B68 File Offset: 0x00007D68
		private void FixedUpdate()
		{
			if (this.player.Value == null)
			{
				return;
			}
			if (this.headTrans.Value == null)
			{
				return;
			}
			using (Packet packet = new Packet(1))
			{
				Transform transform = this.player.Value.transform;
				packet.Write(transform.position, -1);
				packet.Write(transform.rotation, -1);
				packet.Write(this.headTrans.Value.transform.localEulerAngles.x, -1);
				NetEvent<NetPlayer>.Send(string.Format("SyncPosition{0}", BeerMPGlobals.UserID), packet, false);
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00009C28 File Offset: 0x00007E28
		private void OnDestroy()
		{
			LocalNetPlayer.Instance = null;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00009C30 File Offset: 0x00007E30
		public LocalNetPlayer()
		{
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00009C38 File Offset: 0x00007E38
		[CompilerGenerated]
		private void <Start>b__20_1()
		{
			AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(BeerMP.Properties.Resources.clothes);
			Console.Log("charcustom init", false);
			this.characterCustomization = CharacterCustomization.Init(assetBundle);
			Console.Log("charcustom init 2", false);
			BeerMPGlobals.OnMemberReady += delegate(ulong userId)
			{
				this.characterCustomization.InitialSkinSync(null, userId);
			};
			new GameObject("BeerMPChat").AddComponent<Chat>();
			assetBundle.Unload(false);
			this.EditDeath();
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00009CAA File Offset: 0x00007EAA
		[CompilerGenerated]
		private void <Start>b__20_2(ulong userId)
		{
			this.characterCustomization.InitialSkinSync(null, userId);
		}

		// Token: 0x04000179 RID: 377
		[CompilerGenerated]
		private static LocalNetPlayer <Instance>k__BackingField;

		// Token: 0x0400017A RID: 378
		public FsmGameObject player;

		// Token: 0x0400017B RID: 379
		public FsmGameObject headTrans;

		// Token: 0x0400017C RID: 380
		public bool inCar;

		// Token: 0x0400017D RID: 381
		private Transform fpsCameraDefaultParent;

		// Token: 0x0400017E RID: 382
		private GameObject fpsCamera;

		// Token: 0x0400017F RID: 383
		private GameObject death;

		// Token: 0x04000180 RID: 384
		private GameObject gui;

		// Token: 0x04000181 RID: 385
		private GameObject gameOverScreen;

		// Token: 0x04000182 RID: 386
		private GameObject gameOverRespawningLabel;

		// Token: 0x04000183 RID: 387
		private FsmFloat gameVolume;

		// Token: 0x04000184 RID: 388
		private bool respawning;

		// Token: 0x04000185 RID: 389
		private CharacterCustomization characterCustomization;

		// Token: 0x04000186 RID: 390
		private ProximityVoiceChat pvc;

		// Token: 0x020000E5 RID: 229
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004BA RID: 1210 RVA: 0x0001BFE4 File Offset: 0x0001A1E4
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004BB RID: 1211 RVA: 0x0001BFF0 File Offset: 0x0001A1F0
			public <>c()
			{
			}

			// Token: 0x060004BC RID: 1212 RVA: 0x0001BFF8 File Offset: 0x0001A1F8
			internal void <Start>b__20_0(ulong player, Packet p)
			{
				ActionContainer<ulong> onMemberJoin = BeerMPGlobals.OnMemberJoin;
				if (onMemberJoin == null)
				{
					return;
				}
				onMemberJoin.Invoke(player);
			}

			// Token: 0x04000463 RID: 1123
			public static readonly LocalNetPlayer.<>c <>9 = new LocalNetPlayer.<>c();

			// Token: 0x04000464 RID: 1124
			public static NetEventHandler <>9__20_0;
		}
	}
}
