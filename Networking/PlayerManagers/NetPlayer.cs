using System;
using System.Collections.Generic;
using BeerMP.Networking.Managers;
using BeerMP.Properties;
using Steamworks;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x02000050 RID: 80
	internal class NetPlayer : MonoBehaviour
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060001CB RID: 459 RVA: 0x00009CB9 File Offset: 0x00007EB9
		public Vector3 HeadPos
		{
			get
			{
				return this.head.position;
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00009CC6 File Offset: 0x00007EC6
		public void SetInCar(bool inCar, NetVehicle car)
		{
			this.SetPlayerParent(inCar ? car.driverPivots.driverParent : null, false);
			this.playerAnimationManager.SetPlayerInCar(inCar, car);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00009CED File Offset: 0x00007EED
		internal void SetPassengerMode(bool enter, Transform car, bool applySitAnim = true)
		{
			this.SetPlayerParent(enter ? car : null, true);
			this.playerAnimationManager.SetPassengerMode(enter && applySitAnim);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00009D0C File Offset: 0x00007F0C
		public void SetPlayerParent(Transform parent, bool worldPositionStays)
		{
			this.player.transform.SetParent((parent == null) ? base.transform : parent, parent == null || worldPositionStays);
			if (!worldPositionStays && parent != null)
			{
				this.player.transform.localPosition = (this.player.transform.localEulerAngles = Vector3.zero);
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00009D78 File Offset: 0x00007F78
		private void Start()
		{
			base.transform.parent = BeerMP.instance.transform;
			this.SyncPosition = NetEvent<NetPlayer>.Register(string.Format("SyncPosition{0}", this.steamID), new NetEventHandler(this.OnSyncPosition));
			string text = "GrabItem";
			CSteamID csteamID = this.steamID;
			this.grabItem = NetEvent<NetPlayer>.Register(text + csteamID.ToString(), new NetEventHandler(this.OnGrabItem));
			AssetBundle assetBundle = AssetBundle.CreateFromMemoryImmediate(BeerMP.Properties.Resources.clothes);
			this.player = global::UnityEngine.Object.Instantiate<GameObject>(assetBundle.LoadAsset<GameObject>("char.prefab"));
			if (this.disableModel)
			{
				this.player.SetActive(false);
			}
			this.player.name = this.steamID.ToString();
			this.player.transform.parent = base.transform;
			this.head = this.player.transform.Find("char/skeleton/pelvis/RotationBendPivot/spine_middle/spine_upper/headPivot/HeadRotationPivot");
			SkinnedMeshRenderer component = this.player.transform.Find("char/bodymesh").GetComponent<SkinnedMeshRenderer>();
			component.materials[0] = new Material(component.materials[0]);
			component.materials[1] = new Material(component.materials[1]);
			component.materials[2] = new Material(component.materials[2]);
			this.playerAnimationManager = this.player.transform.Find("char/skeleton").gameObject.AddComponent<PlayerAnimationManager>();
			this.playerAnimationManager.charTf = this.player.transform.Find("char");
			MeshRenderer component2 = this.player.transform.Find("char/skeleton/thig_left/knee_left/ankle_left/shoeLeft").GetComponent<MeshRenderer>();
			MeshRenderer component3 = this.player.transform.Find("char/skeleton/thig_right/knee_right/ankle_right/shoeRight").GetComponent<MeshRenderer>();
			Material material = new Material(component2.material);
			component2.material = material;
			component3.material = material;
			CharacterCustomizationItem.parentTo = base.transform;
			this.characterCustomizationItems = new CharacterCustomizationItem[]
			{
				CharacterCustomizationItem.Init(0, null, null, null, null, null, null, this.player.transform.Find("char/skeleton/pelvis/RotationBendPivot/spine_middle/spine_upper/headPivot/HeadRotationPivot/head/glasses"), null),
				CharacterCustomizationItem.Init(1, null, null, null, null, null, null, this.player.transform.Find("char/skeleton/pelvis/RotationBendPivot/spine_middle/spine_upper/headPivot/HeadRotationPivot/head/head_end"), null),
				CharacterCustomizationItem.Init(2, null, null, null, null, CharacterCustomization.faces, component.materials[2], null, null),
				CharacterCustomizationItem.Init(3, null, null, null, null, CharacterCustomization.shirts, component.materials[0], null, null),
				CharacterCustomizationItem.Init(4, null, null, null, null, CharacterCustomization.pants, component.materials[1], null, null),
				CharacterCustomizationItem.Init(5, null, null, null, null, CharacterCustomization.shoes, material, component2.transform, component3.transform)
			};
			CharacterCustomizationItem.parentTo = null;
			assetBundle.Unload(false);
			this.pvc = this.player.AddComponent<ProximityVoiceChat>();
			this.pvc.net = this;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000A070 File Offset: 0x00008270
		private void OnGrabItem(ulong sender, Packet packet)
		{
			if (sender != this.steamID.m_SteamID)
			{
				return;
			}
			bool flag = packet.ReadBool(true);
			int num = packet.ReadInt(true);
			if (flag)
			{
				this.grabbedItem = NetRigidbodyManager.GetRigidbody(num);
				if (this.grabbedItem == null)
				{
					Console.LogError(string.Format("Player {0} grabbed unknown rigidbody of hash {1}", NetManager.playerNames[this.steamID], num), false);
				}
				else
				{
					this.grabbedItem.isKinematic = true;
					this.grabbedItem.gameObject.layer = 16;
					NetPlayer.grabbedItems.Add(this.grabbedItem);
					NetPlayer.grabbedItemsHashes.Add(num);
				}
			}
			else if (this.grabbedItem != null)
			{
				this.grabbedItem.isKinematic = false;
				this.grabbedItem.gameObject.layer = 19;
				NetPlayer.grabbedItems.Remove(this.grabbedItem);
				NetPlayer.grabbedItemsHashes.Remove(num);
				this.grabbedItem = null;
			}
			this.playerAnimationManager.GrabItem(this.grabbedItem);
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000A184 File Offset: 0x00008384
		public void OnSyncPosition(ulong sender, Packet packet)
		{
			this.pos = packet.ReadVector3(true);
			this.pos += this.offest;
			this.rot = packet.ReadQuaternion(true);
			float num = packet.ReadFloat(true);
			if (this.player.transform.parent == base.transform)
			{
				this.head.localEulerAngles = Vector3.forward * -num;
				return;
			}
			this.head.eulerAngles = new Vector3(0f, this.rot.eulerAngles.y - 90f, -num);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000A22C File Offset: 0x0000842C
		public void OnInitialSkinSync(int[] skinPreset)
		{
			for (int i = 0; i < skinPreset.Length; i++)
			{
				this.OnSkinChange(i, skinPreset[i]);
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000A251 File Offset: 0x00008451
		public void OnSkinChange(int clothesIndex, int selectedIndex)
		{
			this.characterCustomizationItems[clothesIndex].SetOption(selectedIndex, false);
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000A264 File Offset: 0x00008464
		private void FixedUpdate()
		{
			if (this.player.transform.parent == base.transform)
			{
				this.player.transform.position = Vector3.Lerp(this.player.transform.position, this.pos, Time.deltaTime * 15f);
				this.player.transform.rotation = Quaternion.Lerp(this.player.transform.rotation, this.rot, Time.deltaTime * 30f);
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000A2FA File Offset: 0x000084FA
		private void OnDestroy()
		{
			global::UnityEngine.Object.Destroy(this.player);
			this.SyncPosition.Unregister();
			this.grabItem.Unregister();
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000A31D File Offset: 0x0000851D
		public NetPlayer()
		{
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000A346 File Offset: 0x00008546
		// Note: this type is marked as 'beforefieldinit'.
		static NetPlayer()
		{
		}

		// Token: 0x04000187 RID: 391
		public CSteamID steamID;

		// Token: 0x04000188 RID: 392
		public GameObject player;

		// Token: 0x04000189 RID: 393
		public PlayerAnimationManager playerAnimationManager;

		// Token: 0x0400018A RID: 394
		private NetEvent<NetPlayer> SyncPosition;

		// Token: 0x0400018B RID: 395
		private NetEvent<NetPlayer> grabItem;

		// Token: 0x0400018C RID: 396
		public Vector3 offest = new Vector3(0f, 0.17f, 0f);

		// Token: 0x0400018D RID: 397
		private Vector3 pos;

		// Token: 0x0400018E RID: 398
		private Quaternion rot;

		// Token: 0x0400018F RID: 399
		private Transform head;

		// Token: 0x04000190 RID: 400
		private CharacterCustomizationItem[] characterCustomizationItems;

		// Token: 0x04000191 RID: 401
		private Rigidbody grabbedItem;

		// Token: 0x04000192 RID: 402
		public const string grabItemEvent = "GrabItem";

		// Token: 0x04000193 RID: 403
		public static List<Rigidbody> grabbedItems = new List<Rigidbody>();

		// Token: 0x04000194 RID: 404
		public static List<int> grabbedItemsHashes = new List<int>();

		// Token: 0x04000195 RID: 405
		internal bool disableModel = true;

		// Token: 0x04000196 RID: 406
		private ProximityVoiceChat pvc;
	}
}
