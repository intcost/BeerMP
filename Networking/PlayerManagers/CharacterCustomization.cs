using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x0200004D RID: 77
	internal class CharacterCustomization : MonoBehaviour
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x0000876C File Offset: 0x0000696C
		public static void LoadTextures(AssetBundle ab)
		{
			CharacterCustomization.faces = new Texture2D[17];
			CharacterCustomization.pants = new Texture2D[21];
			CharacterCustomization.shirts = new Texture2D[47];
			CharacterCustomization.shoes = new Texture2D[7];
			for (int i = 1; i <= Mathf.Max(new int[] { 21, 7, 47, 17 }); i++)
			{
				string text = i.ToString();
				if (i < 10)
				{
					text = "0" + text;
				}
				if (i <= 17)
				{
					CharacterCustomization.faces[i - 1] = ab.LoadAsset<Texture2D>("char_face" + text + ".png");
				}
				if (i <= 21)
				{
					CharacterCustomization.pants[i - 1] = ab.LoadAsset<Texture2D>("char_pants" + text + ".png");
				}
				if (i <= 47)
				{
					CharacterCustomization.shirts[i - 1] = ab.LoadAsset<Texture2D>("char_shirt" + text + ".png");
				}
				if (i <= 7)
				{
					CharacterCustomization.shoes[i - 1] = ab.LoadAsset<Texture2D>("char_shoes" + text + ".png");
				}
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00008878 File Offset: 0x00006A78
		public static CharacterCustomization Init(AssetBundle ab)
		{
			Console.Log(string.Format("Enter charcustom init {0}", ab == null), false);
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(ab.LoadAsset<GameObject>("LocalPlayerSkinRender.prefab"));
			gameObject.transform.position = Vector3.up * -10f;
			gameObject.name = "LocalPlayerSkinRender";
			Console.Log("Enter charcustom init 0.1", false);
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(ab.LoadAsset<GameObject>("char.prefab"));
			Console.Log("Enter charcustom init 0.2", false);
			gameObject2.name = "localPlayerModel";
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = (gameObject2.transform.localEulerAngles = Vector3.zero);
			gameObject2.transform.Find("char/Camera").gameObject.SetActive(true);
			gameObject2.SetActive(false);
			Console.Log("Enter charcustom init 1", false);
			GameObject gameObject3 = global::UnityEngine.Object.Instantiate<GameObject>(ab.LoadAsset<GameObject>("Settings_Char.prefab"));
			gameObject3.transform.SetParent(GameObject.Find("Systems").transform.Find("OptionsMenu"));
			gameObject3.name = "PlayerCustomization";
			gameObject3.transform.localPosition = new Vector3(4f, -0.1f, 0f);
			gameObject3.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
			gameObject3.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			gameObject3.SetActive(false);
			Console.Log("Enter charcustom init 2", false);
			GameObject gameObject4 = GameObject.Find("Systems").transform.Find("OptionsMenu/Menu").gameObject;
			gameObject4.transform.Find("Table 5").localPosition = new Vector3(0f, -1.1f, 0.01f);
			gameObject4.transform.Find("Table 5").localScale = new Vector3(1f, 1.3f, 1f);
			gameObject4.transform.Find("Table 3").localPosition = new Vector3(0f, 1.55f, 0.01f);
			gameObject4.transform.Find("Header 4").localPosition = new Vector3(0f, 2.55f, 0.02f);
			gameObject4.transform.Find("BoxBG").localPosition = new Vector3(0f, -0.7f, 1f);
			gameObject4.transform.Find("BoxBG").localScale = new Vector3(6.5f, 7.75f, 1f);
			gameObject4.transform.Find("Btn_Resume").localPosition = new Vector3(2.5f, 1.5f, -0.1f);
			gameObject4.transform.Find("Btn_Quit/GUITextLabel").GetComponent<TextMesh>().text = (gameObject4.transform.Find("Btn_Quit/GUITextLabel/GUITextLabelShadow").GetComponent<TextMesh>().text = "DISCONNECT");
			PlayMakerFSM component = GameObject.Find("Systems").transform.Find("OptionsMenu/Menu/Btn_ConfirmQuit/Button").GetComponent<PlayMakerFSM>();
			component.Initialize();
			FsmState fsmState = component.FsmStates.First((FsmState s) => s.Name == "State 3");
			PlayMakerUtilities.PM_Hook[] array = new PlayMakerUtilities.PM_Hook[1];
			array[0] = new PlayMakerUtilities.PM_Hook(delegate
			{
				NetManager.Disconnect();
			}, false);
			FsmStateAction[] array2 = array;
			fsmState.Actions = array2;
			Console.Log("Enter charcustom init 3", false);
			CharacterCustomization characterCustomization = gameObject4.AddComponent<CharacterCustomization>();
			characterCustomization.guiCharacter = gameObject3;
			characterCustomization.playerModel = gameObject2;
			Console.Log("Registered Skinchange events", false);
			NetEvent<CharacterCustomization>.Register("InitSkinSync", new NetEventHandler(characterCustomization.OnInitialSkinSync));
			NetEvent<CharacterCustomization>.Register("SkinChange", new NetEventHandler(characterCustomization.OnSkinChange));
			Console.Log("Registered Skinchange events done", false);
			characterCustomization._Awake();
			return characterCustomization;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00008C88 File Offset: 0x00006E88
		private void _Awake()
		{
			this.hud = GameObject.Find("Systems").transform.Find("OptionsMenu/Menu");
			this.optionsmenu = GameObject.Find("Systems").transform.Find("OptionsMenu").gameObject;
			this.settingTab = global::UnityEngine.Object.Instantiate<GameObject>(this.hud.transform.Find("Btn_Graphics").gameObject);
			this.settingTab.transform.SetParent(this.hud);
			this.settingTab.name = "Btn_PlayerCustomization";
			this.settingTab.transform.localPosition = new Vector3(2.5f, 0f, -0.1f);
			global::UnityEngine.Object.Destroy(this.settingTab.transform.Find("Button").GetComponent<PlayMakerFSM>());
			this.guiTextLabel = this.settingTab.transform.Find("GUITextLabel");
			this.guiTextLabel.GetComponent<TextMesh>().text = "SKIN CONFIG";
			this.guiTextLabel.GetChild(0).GetComponent<TextMesh>().text = "SKIN CONFIG";
			this.buttonCollider = this.settingTab.transform.Find("Button").GetComponent<Collider>();
			this.othermenus = new GameObject[]
			{
				this.optionsmenu.transform.Find("DEBUG").gameObject,
				this.optionsmenu.transform.Find("Graphics").gameObject,
				this.optionsmenu.transform.Find("VehicleControls").gameObject,
				this.optionsmenu.transform.Find("PlayerControls").gameObject
			};
			Transform transform = this.guiCharacter.transform.Find("Page/Buttons");
			Transform transform2 = this.guiCharacter.transform.Find("Page/FieldString");
			SkinnedMeshRenderer component = this.playerModel.transform.Find("char/bodymesh").GetComponent<SkinnedMeshRenderer>();
			component.materials[0] = new Material(component.materials[0]);
			component.materials[1] = new Material(component.materials[1]);
			component.materials[2] = new Material(component.materials[2]);
			MeshRenderer component2 = this.playerModel.transform.Find("char/skeleton/thig_left/knee_left/ankle_left/shoeLeft").GetComponent<MeshRenderer>();
			MeshRenderer component3 = this.playerModel.transform.Find("char/skeleton/thig_right/knee_right/ankle_right/shoeRight").GetComponent<MeshRenderer>();
			Material material = new Material(component2.material);
			component2.material = material;
			component3.material = material;
			this.characterCustomizationItems = new CharacterCustomizationItem[]
			{
				CharacterCustomizationItem.Init(0, new Action(this.SaveSkin), transform.GetChild(0), transform2.GetChild(0), null, null, null, this.playerModel.transform.Find("char/skeleton/pelvis/RotationBendPivot/spine_middle/spine_upper/headPivot/HeadRotationPivot/head/glasses"), null),
				CharacterCustomizationItem.Init(1, new Action(this.SaveSkin), transform.GetChild(1), transform2.GetChild(1), null, null, null, this.playerModel.transform.Find("char/skeleton/pelvis/RotationBendPivot/spine_middle/spine_upper/headPivot/HeadRotationPivot/head/head_end"), null),
				CharacterCustomizationItem.Init(2, new Action(this.SaveSkin), transform.GetChild(2), transform2.GetChild(2), null, CharacterCustomization.faces, component.materials[2], null, null),
				CharacterCustomizationItem.Init(3, new Action(this.SaveSkin), transform.GetChild(3), transform2.GetChild(3), CharacterCustomization.NAMES_SHIRTS, CharacterCustomization.shirts, component.materials[0], null, null),
				CharacterCustomizationItem.Init(4, new Action(this.SaveSkin), transform.GetChild(4), transform2.GetChild(4), CharacterCustomization.NAMES_PANTS, CharacterCustomization.pants, component.materials[1], null, null),
				CharacterCustomizationItem.Init(5, new Action(this.SaveSkin), transform.GetChild(5), transform2.GetChild(5), null, CharacterCustomization.shoes, material, component2.transform, component3.transform)
			};
			this.LoadSkin();
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00009090 File Offset: 0x00007290
		private void LoadSkin()
		{
			int[] array = new int[] { 0, 0, 6, 12, 4, 3 };
			if (File.Exists(this.skinPresetFilePath))
			{
				byte[] array2 = File.ReadAllBytes(this.skinPresetFilePath);
				if (array2.Length == 8)
				{
					long num = BitConverter.ToInt64(array2, 0);
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = (int)((num >> i * 6) & 63L);
					}
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				this.characterCustomizationItems[j].SetOption(array[j], false);
			}
			this.InitialSkinSync(array, 0UL);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00009124 File Offset: 0x00007324
		private void SaveSkin()
		{
			long num = 0L;
			for (int i = 0; i < this.characterCustomizationItems.Length; i++)
			{
				num |= (long)((long)this.characterCustomizationItems[i].SelectedIndex << i * 6);
			}
			File.WriteAllBytes(this.skinPresetFilePath, BitConverter.GetBytes(num));
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00009170 File Offset: 0x00007370
		public void InitialSkinSync(int[] skinPreset, ulong sendTo = 0UL)
		{
			if (skinPreset == null)
			{
				skinPreset = new int[this.characterCustomizationItems.Length];
				for (int i = 0; i < skinPreset.Length; i++)
				{
					skinPreset[i] = this.characterCustomizationItems[i].SelectedIndex;
				}
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write(skinPreset[0], -1);
				packet.Write(skinPreset[1], -1);
				packet.Write(skinPreset[2], -1);
				packet.Write(skinPreset[3], -1);
				packet.Write(skinPreset[4], -1);
				packet.Write(skinPreset[5], -1);
				if (sendTo == 0UL)
				{
					NetEvent<CharacterCustomization>.Send("InitSkinSync", packet, true);
				}
				else
				{
					NetEvent<CharacterCustomization>.Send("InitSkinSync", packet, sendTo, true);
				}
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000922C File Offset: 0x0000742C
		private void OnInitialSkinSync(ulong player, Packet p)
		{
			NetPlayer playerComponentById = NetManager.GetPlayerComponentById<NetPlayer>(player);
			if (playerComponentById == null)
			{
				Console.LogError(string.Format("CharacterCustomization.OnInitSkinSync: NetPlayer with ID {0} is null", player), false);
				return;
			}
			playerComponentById.OnInitialSkinSync(new int[]
			{
				p.ReadInt(true),
				p.ReadInt(true),
				p.ReadInt(true),
				p.ReadInt(true),
				p.ReadInt(true),
				p.ReadInt(true)
			});
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x000092A8 File Offset: 0x000074A8
		private void OnSkinChange(ulong player, Packet p)
		{
			NetPlayer playerComponentById = NetManager.GetPlayerComponentById<NetPlayer>(player);
			if (playerComponentById == null)
			{
				Console.LogError(string.Format("CharacterCustomization.OnSkinChange: NetPlayer with ID {0} is null", player), false);
				return;
			}
			playerComponentById.OnSkinChange(p.ReadInt(true), p.ReadInt(true));
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x000092F0 File Offset: 0x000074F0
		private void Update()
		{
			bool flag = Raycaster.Raycast(this.buttonCollider, 1.35f, -1);
			if (flag != this.mouseOver)
			{
				this.guiTextLabel.localScale = Vector3.one * (flag ? 0.95f : 1f);
				this.mouseOver = flag;
			}
			if (this.mouseOver && !this.uiVisible && Input.GetMouseButton(0))
			{
				this.guiCharacter.SetActive(true);
				this.playerModel.SetActive(true);
				for (int i = 0; i < this.othermenus.Length; i++)
				{
					this.othermenus[i].SetActive(false);
				}
				this.uiVisible = true;
			}
			if (this.uiVisible)
			{
				if (this.othermenus.Any((GameObject go) => go.activeSelf))
				{
					this.guiCharacter.SetActive(false);
					this.playerModel.SetActive(false);
					this.uiVisible = false;
				}
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x000093EE File Offset: 0x000075EE
		public CharacterCustomization()
		{
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000940C File Offset: 0x0000760C
		// Note: this type is marked as 'beforefieldinit'.
		static CharacterCustomization()
		{
		}

		// Token: 0x04000154 RID: 340
		public static readonly string[] NAMES_PANTS = new string[]
		{
			"Blue", "Checked black", "Stripped blue", "Beige", "Mirrored shorts", "Blue jeans", "BP pants", "Black", "Hayosiko shorts", "Blue with belt",
			"Teal", "Checked brown", "Checked blue", "Checked green", "Checked orange", "Checked red", "Green shorts", "Red shorts", "Stripped black", "Cop jeans",
			"White shorts"
		};

		// Token: 0x04000155 RID: 341
		public static readonly string[] NAMES_SHIRTS = new string[]
		{
			"None", "Teimo", "Gifu", "Yellow", "Grey", "Teal", "Van craze", "White shirt", "Black", "Blue amis",
			"Arvo", "Office", "Yellow FUstreet", "Black shirt", "Green shirt", "Suski", "White shirt 2", "BP top", "White shirt 3", "Blue shirt",
			"Maceeiko", "Black shirt 2", "Maceeiko 2", "Kekmet", "Balfield", "White FUstreet", "RCO", "CORRIS", "Voittous", "Slate blue",
			"Teal shirt", "Satsuma shirt", "Yellow amis", "CORRIS 2", "Polaventris", "Hayosiko", "Hayosiko 2", "CORRIS 3", "Suvi sprint 1992", "Talvi sprint 1993",
			"Office 2", "Blue shirt", "Mafia shirt", "Dassler shirt", "Black shirt 3", "Cop", "Cop 2"
		};

		// Token: 0x04000156 RID: 342
		public static Texture2D[] faces;

		// Token: 0x04000157 RID: 343
		public static Texture2D[] shirts;

		// Token: 0x04000158 RID: 344
		public static Texture2D[] pants;

		// Token: 0x04000159 RID: 345
		public static Texture2D[] shoes;

		// Token: 0x0400015A RID: 346
		private const int numFaces = 17;

		// Token: 0x0400015B RID: 347
		private const int numPants = 21;

		// Token: 0x0400015C RID: 348
		private const int numShirts = 47;

		// Token: 0x0400015D RID: 349
		private const int numShoes = 7;

		// Token: 0x0400015E RID: 350
		public GameObject settingTab;

		// Token: 0x0400015F RID: 351
		public GameObject guiCharacter;

		// Token: 0x04000160 RID: 352
		public GameObject playerModel;

		// Token: 0x04000161 RID: 353
		private bool mouseOver;

		// Token: 0x04000162 RID: 354
		private bool uiVisible;

		// Token: 0x04000163 RID: 355
		public Transform hud;

		// Token: 0x04000164 RID: 356
		public const string initSyncEvent = "InitSkinSync";

		// Token: 0x04000165 RID: 357
		public const string skinChangeEvent = "SkinChange";

		// Token: 0x04000166 RID: 358
		private readonly string skinPresetFilePath = Path.Combine(Application.persistentDataPath, "BeerMP_playerskin");

		// Token: 0x04000167 RID: 359
		private GameObject optionsmenu;

		// Token: 0x04000168 RID: 360
		private Transform guiTextLabel;

		// Token: 0x04000169 RID: 361
		private Collider buttonCollider;

		// Token: 0x0400016A RID: 362
		private GameObject[] othermenus;

		// Token: 0x0400016B RID: 363
		private CharacterCustomizationItem[] characterCustomizationItems;

		// Token: 0x020000E4 RID: 228
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004B5 RID: 1205 RVA: 0x0001BFAF File Offset: 0x0001A1AF
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004B6 RID: 1206 RVA: 0x0001BFBB File Offset: 0x0001A1BB
			public <>c()
			{
			}

			// Token: 0x060004B7 RID: 1207 RVA: 0x0001BFC3 File Offset: 0x0001A1C3
			internal bool <Init>b__11_0(FsmState s)
			{
				return s.Name == "State 3";
			}

			// Token: 0x060004B8 RID: 1208 RVA: 0x0001BFD5 File Offset: 0x0001A1D5
			internal void <Init>b__11_1()
			{
				NetManager.Disconnect();
			}

			// Token: 0x060004B9 RID: 1209 RVA: 0x0001BFDC File Offset: 0x0001A1DC
			internal bool <Update>b__32_0(GameObject go)
			{
				return go.activeSelf;
			}

			// Token: 0x0400045F RID: 1119
			public static readonly CharacterCustomization.<>c <>9 = new CharacterCustomization.<>c();

			// Token: 0x04000460 RID: 1120
			public static Func<FsmState, bool> <>9__11_0;

			// Token: 0x04000461 RID: 1121
			public static Action <>9__11_1;

			// Token: 0x04000462 RID: 1122
			public static Func<GameObject, bool> <>9__32_0;
		}
	}
}
