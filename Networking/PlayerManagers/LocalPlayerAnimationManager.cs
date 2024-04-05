using System;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x02000054 RID: 84
	internal class LocalPlayerAnimationManager : MonoBehaviour
	{
		// Token: 0x060001F8 RID: 504 RVA: 0x0000B621 File Offset: 0x00009821
		private void Start()
		{
			this.player = FsmVariables.GlobalVariables.FindFsmGameObject("SavePlayer");
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000B638 File Offset: 0x00009838
		private void Update()
		{
			if (this.player.Value == null)
			{
				return;
			}
			if (this.playerHeight == null)
			{
				this.InitPlayerVariables();
			}
			bool key = cInput.GetKey("Run");
			if (key != this.lastSprint)
			{
				this.lastSprint = key;
				using (Packet packet = new Packet(1))
				{
					packet.Write(key, -1);
					NetEvent<PlayerAnimationManager>.Send("IsSprinting", packet, true);
				}
			}
			if (Input.GetMouseButtonDown(0))
			{
				using (Packet packet2 = new Packet(1))
				{
					NetEvent<PlayerAnimationManager>.Send("PlayerClicked", packet2, true);
				}
			}
			byte b = (this.playerInCar.Value ? 2 : ((this.playerHeight.Value >= 1.3f) ? 0 : ((this.playerHeight.Value >= 0.5f) ? 1 : 2)));
			if (b != this.lastCrouch)
			{
				this.lastCrouch = b;
				using (Packet packet3 = new Packet(1))
				{
					packet3.Write(b, -1);
					NetEvent<PlayerAnimationManager>.Send("Crouch", packet3, true);
				}
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000B774 File Offset: 0x00009974
		private void InitPlayerVariables()
		{
			PlayMakerFSM playMaker = this.player.Value.GetPlayMaker("Crouch");
			this.playerHeight = playMaker.FsmVariables.FindFsmFloat("Position");
			this.playerInCar = playMaker.FsmVariables.FindFsmBool("PlayerInCar");
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000B7C3 File Offset: 0x000099C3
		public LocalPlayerAnimationManager()
		{
		}

		// Token: 0x040001C9 RID: 457
		private FsmGameObject player;

		// Token: 0x040001CA RID: 458
		private FsmFloat playerHeight;

		// Token: 0x040001CB RID: 459
		private FsmBool playerInCar;

		// Token: 0x040001CC RID: 460
		private bool lastSprint;

		// Token: 0x040001CD RID: 461
		private byte lastCrouch;
	}
}
