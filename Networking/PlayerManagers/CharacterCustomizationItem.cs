using System;
using BeerMP.Helpers;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x0200004E RID: 78
	internal class CharacterCustomizationItem : MonoBehaviour
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00009683 File Offset: 0x00007883
		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000968C File Offset: 0x0000788C
		public static CharacterCustomizationItem Init(int clothingIndex, Action clothingChanged, Transform buttonsParent, Transform fieldStringParent, string[] names = null, Texture2D[] textures = null, Material targetMaterial = null, Transform targetParent = null, Transform targetParent2 = null)
		{
			CharacterCustomizationItem characterCustomizationItem = (buttonsParent ?? CharacterCustomizationItem.parentTo).gameObject.AddComponent<CharacterCustomizationItem>();
			if (buttonsParent != null)
			{
				characterCustomizationItem.buttonLeft = buttonsParent.GetChild(1).GetComponent<Collider>();
				characterCustomizationItem.buttonRight = buttonsParent.GetChild(0).GetComponent<Collider>();
			}
			if (fieldStringParent != null)
			{
				characterCustomizationItem.fieldString = fieldStringParent.GetComponent<TextMesh>();
				characterCustomizationItem.fieldStringBackground = fieldStringParent.GetChild(0).GetComponent<TextMesh>();
			}
			characterCustomizationItem.names = names;
			characterCustomizationItem.textures = textures;
			characterCustomizationItem.targetMaterial = targetMaterial;
			characterCustomizationItem.targetParent = targetParent;
			characterCustomizationItem.targetParent2 = targetParent2;
			characterCustomizationItem.clothingIndex = clothingIndex;
			characterCustomizationItem.clothingChanged = clothingChanged;
			return characterCustomizationItem;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000973C File Offset: 0x0000793C
		private void Update()
		{
			if (this.buttonLeft != null && this.buttonRight != null && Input.GetMouseButtonDown(0))
			{
				if (Raycaster.Raycast(this.buttonLeft, 1.35f, -1))
				{
					this.SetOption(this.selectedIndex - 1, true);
					return;
				}
				if (Raycaster.Raycast(this.buttonRight, 1.35f, -1))
				{
					this.SetOption(this.selectedIndex + 1, true);
				}
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x000097B4 File Offset: 0x000079B4
		public void SetOption(int index, bool sendEvent = true)
		{
			int num = ((this.textures == null) ? this.targetParent.childCount : ((this.targetParent == null) ? this.textures.Length : (this.textures.Length + 1)));
			index = Mathf.Clamp(index, 0, num - 1);
			if (this.fieldString != null && this.fieldStringBackground != null)
			{
				string text = ((this.names != null) ? this.names[index] : ((this.targetParent != null && this.textures != null) ? ((index == 0) ? "None" : string.Format("INDEX {0}", index)) : ((this.targetParent != null) ? this.targetParent.GetChild(index).name : string.Format("INDEX {0}", index))));
				text = text.ToUpper();
				this.fieldString.text = (this.fieldStringBackground.text = text);
			}
			if (this.targetParent != null && this.textures != null)
			{
				this.targetParent.gameObject.SetActive(index > 0);
				if (this.targetParent2 != null)
				{
					this.targetParent2.gameObject.SetActive(index > 0);
				}
				this.targetMaterial.mainTexture = this.textures[(index == 0) ? 0 : (index - 1)];
			}
			else if (this.textures != null)
			{
				this.targetMaterial.mainTexture = this.textures[index];
			}
			else
			{
				this.targetParent.GetChild(this.selectedIndex).gameObject.SetActive(false);
				this.targetParent.GetChild(index).gameObject.SetActive(true);
			}
			this.selectedIndex = index;
			if (sendEvent)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.clothingIndex, -1);
					packet.Write(this.selectedIndex, -1);
					NetEvent<CharacterCustomization>.Send("SkinChange", packet, true);
				}
			}
			Action action = this.clothingChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x060001BF RID: 447 RVA: 0x000099DC File Offset: 0x00007BDC
		public CharacterCustomizationItem()
		{
		}

		// Token: 0x0400016C RID: 364
		public Collider buttonLeft;

		// Token: 0x0400016D RID: 365
		public Collider buttonRight;

		// Token: 0x0400016E RID: 366
		public TextMesh fieldString;

		// Token: 0x0400016F RID: 367
		public TextMesh fieldStringBackground;

		// Token: 0x04000170 RID: 368
		public string[] names;

		// Token: 0x04000171 RID: 369
		public Texture2D[] textures;

		// Token: 0x04000172 RID: 370
		public Material targetMaterial;

		// Token: 0x04000173 RID: 371
		public Transform targetParent;

		// Token: 0x04000174 RID: 372
		public Transform targetParent2;

		// Token: 0x04000175 RID: 373
		internal int clothingIndex = -1;

		// Token: 0x04000176 RID: 374
		private int selectedIndex;

		// Token: 0x04000177 RID: 375
		public static Transform parentTo;

		// Token: 0x04000178 RID: 376
		private Action clothingChanged;
	}
}
