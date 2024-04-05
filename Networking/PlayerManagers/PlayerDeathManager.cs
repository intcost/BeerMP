using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.Managers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Steamworks;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x02000056 RID: 86
	internal class PlayerDeathManager : MonoBehaviour
	{
		// Token: 0x06000201 RID: 513 RVA: 0x0000BAAC File Offset: 0x00009CAC
		public PlayerDeathManager()
		{
			this.player = GameObject.Find("PLAYER");
			this.charMotor = this.player.GetComponent<CharacterMotor>();
			PlayMakerFSM playMaker = this.player.GetPlayMaker("Crouch");
			this.playerInCar = playMaker.FsmVariables.FindFsmBool("PlayerInCar");
			this.playerInWater = playMaker.FsmVariables.FindFsmBool("PlayerInWater");
			this.playerThirst = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerThirst");
			this.playerHunger = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerHunger");
			this.playerStress = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerStress");
			this.playerFatigue = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerFatigue");
			this.playerDirtiness = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerDirtiness");
			this.playerUrine = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine");
			this.playerMoney = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerMoney");
			GameObject gameObject = GameObject.Find("Systems");
			this.newsPhotos = gameObject.transform.Find("Photomode Cam/NewsPhotos").gameObject;
			this.optionsToggle = gameObject.transform.Find("Options").gameObject;
			this.fpsCameraParent = this.player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera");
			this.fpsCamera = this.fpsCameraParent.Find("FPSCamera").gameObject;
			this.blood = this.fpsCamera.GetComponent<ScreenOverlay>();
			this.fpsCameraClone = global::UnityEngine.Object.Instantiate<GameObject>(this.fpsCamera);
			this.fpsCameraClone.transform.parent = null;
			this.fpsCameraClone.SetActive(false);
			this.gameVolume = PlayMakerGlobals.Instance.Variables.FindFsmFloat("GameVolume");
			this.gui = GameObject.Find("GUI");
			this.gameOverScreen = base.transform.Find("GameOverScreen").gameObject;
			this.gameOverRespawningLabel = this.gameOverScreen.transform.Find("Saving").gameObject;
			this.gameOverRespawningLabel.transform.GetComponent<TextMesh>().text = "RESPAWNING...";
			this.deadBody = base.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmGameObject("DeadBody");
			this.playerCtrlCache = new PlayerDeathManager.PlayerCtrlCache(this.player);
			this.DeathEvent = NetEvent<PlayerDeathManager>.Register("Death", new NetEventHandler(this.OnSomeoneDieEvent));
			PlayMakerFSM[] array = Resources.FindObjectsOfTypeAll<PlayMakerFSM>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Initialize();
				for (int j = 0; j < array[i].FsmStates.Length; j++)
				{
					for (int k = 0; k < array[i].FsmStates[j].Actions.Length; k++)
					{
						DestroyComponent destroyComponent = array[i].FsmStates[j].Actions[k] as DestroyComponent;
						if (destroyComponent != null && (!(destroyComponent.component.Value != "CharacterController") || !(destroyComponent.component.Value != "CharacterMotor") || !(destroyComponent.component.Value != "FPSInputController")))
						{
							destroyComponent.Enabled = false;
							if (destroyComponent.component.Value == "CharacterMotor")
							{
								array[i].InsertAction(array[i].FsmStates[j].Name, delegate
								{
									this.charMotor.canControl = false;
								}, k + 1);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000BE6C File Offset: 0x0000A06C
		private void OnSomeoneDieEvent(ulong sender, Packet packet)
		{
			CSteamID csteamID = (CSteamID)sender;
			bool flag = packet.ReadBool(true);
			Console.Log(NetManager.playerNames[csteamID] + " " + (flag ? "has respawned" : "has died") + ". You have been charged 300 MK!", true);
			if (!flag)
			{
				this.playerMoney.Value = Mathf.Clamp(this.playerMoney.Value - 300f, 0f, float.MaxValue);
			}
			NetManager.GetPlayerComponentById<NetPlayer>(csteamID).player.SetActive(flag);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000BEF8 File Offset: 0x0000A0F8
		private void SendIDied(bool respawned)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(respawned, -1);
				this.DeathEvent.Send(packet, true);
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000BF40 File Offset: 0x0000A140
		private void OnEnable()
		{
			base.StartCoroutine(this.OnPlayerDie());
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000BF4F File Offset: 0x0000A14F
		private IEnumerator OnPlayerDie()
		{
			this.SendIDied(false);
			this.newsPhotos.SetActive(true);
			float volume = 1f;
			while (volume > 0f)
			{
				volume = Mathf.Clamp01(volume - Time.deltaTime / 2f);
				this.gameVolume.Value = volume;
				yield return new WaitForEndOfFrame();
			}
			this.gameVolume.Value = 0f;
			yield return new WaitForSeconds(0.4f);
			this.fpsCamera.SetActive(false);
			this.gameVolume.Value = 1f;
			this.player.SetActive(false);
			this.deadBody.Value.SetActive(false);
			this.gui.SetActive(false);
			this.gameOverScreen.SetActive(true);
			while (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Return))
			{
				yield return new WaitForEndOfFrame();
			}
			this.gameOverRespawningLabel.SetActive(true);
			yield return new WaitForSeconds(2f);
			this.fpsCamera.transform.parent = this.fpsCameraParent;
			this.fpsCamera.transform.localPosition = Vector3.forward * -0.05f;
			this.fpsCamera.transform.localEulerAngles = Vector3.zero;
			this.fpsCamera.SetActive(true);
			this.fpsCamera.gameObject.tag = "MainCamera";
			Camera component = this.fpsCamera.GetComponent<Camera>();
			if (component == null)
			{
				Console.LogError("Player camera null after death", false);
			}
			else
			{
				component.enabled = true;
			}
			this.player.SetActive(true);
			this.player.transform.parent = null;
			this.player.transform.position = new Vector3(-1434.642f, 4.682786f, 1151.625f);
			this.player.transform.eulerAngles = new Vector3(0f, 252.6235f, 0f);
			this.charMotor.canControl = true;
			this.blood.enabled = false;
			this.playerThirst.Value = (this.playerHunger.Value = (this.playerStress.Value = (this.playerUrine.Value = (this.playerDirtiness.Value = (this.playerFatigue.Value = 0f)))));
			this.playerInCar.Value = (this.playerInWater.Value = false);
			this.gui.SetActive(true);
			this.optionsToggle.SetActive(true);
			this.newsPhotos.SetActive(false);
			this.gameOverRespawningLabel.SetActive(false);
			this.gameOverScreen.SetActive(false);
			base.gameObject.SetActive(false);
			this.playerMoney.Value = Mathf.Clamp(this.playerMoney.Value - 300f, 0f, float.MaxValue);
			Console.Log("You died! You have been charged 300 MK for respawn.", true);
			this.SendIDied(true);
			yield break;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000BF5E File Offset: 0x0000A15E
		[CompilerGenerated]
		private void <.ctor>b__25_0()
		{
			this.charMotor.canControl = false;
		}

		// Token: 0x040001D1 RID: 465
		private Transform fpsCameraParent;

		// Token: 0x040001D2 RID: 466
		private GameObject newsPhotos;

		// Token: 0x040001D3 RID: 467
		private GameObject fpsCamera;

		// Token: 0x040001D4 RID: 468
		private GameObject fpsCameraClone;

		// Token: 0x040001D5 RID: 469
		private GameObject player;

		// Token: 0x040001D6 RID: 470
		private GameObject gui;

		// Token: 0x040001D7 RID: 471
		private GameObject gameOverScreen;

		// Token: 0x040001D8 RID: 472
		private GameObject gameOverRespawningLabel;

		// Token: 0x040001D9 RID: 473
		private GameObject optionsToggle;

		// Token: 0x040001DA RID: 474
		private FsmGameObject deadBody;

		// Token: 0x040001DB RID: 475
		private FsmFloat gameVolume;

		// Token: 0x040001DC RID: 476
		private FsmFloat playerThirst;

		// Token: 0x040001DD RID: 477
		private FsmFloat playerHunger;

		// Token: 0x040001DE RID: 478
		private FsmFloat playerStress;

		// Token: 0x040001DF RID: 479
		private FsmFloat playerFatigue;

		// Token: 0x040001E0 RID: 480
		private FsmFloat playerDirtiness;

		// Token: 0x040001E1 RID: 481
		private FsmFloat playerUrine;

		// Token: 0x040001E2 RID: 482
		private FsmFloat playerMoney;

		// Token: 0x040001E3 RID: 483
		private FsmBool playerInCar;

		// Token: 0x040001E4 RID: 484
		private FsmBool playerInWater;

		// Token: 0x040001E5 RID: 485
		private CharacterMotor charMotor;

		// Token: 0x040001E6 RID: 486
		private PlayerDeathManager.PlayerCtrlCache playerCtrlCache;

		// Token: 0x040001E7 RID: 487
		private ScreenOverlay blood;

		// Token: 0x040001E8 RID: 488
		private NetEvent<PlayerDeathManager> DeathEvent;

		// Token: 0x020000EB RID: 235
		private class PlayerCtrlCache
		{
			// Token: 0x060004DA RID: 1242 RVA: 0x0001C4F0 File Offset: 0x0001A6F0
			public PlayerCtrlCache(GameObject player)
			{
				CharacterController component = player.GetComponent<CharacterController>();
				this.radius = component.radius;
				this.height = component.height;
				this.center = component.center;
				this.slopeLimit = component.slopeLimit;
				this.stepOffset = component.stepOffset;
				this.detectCollisions = component.detectCollisions;
			}

			// Token: 0x060004DB RID: 1243 RVA: 0x0001C554 File Offset: 0x0001A754
			public void Apply(GameObject player)
			{
				CharacterController characterController = player.AddComponent<CharacterController>();
				player.AddComponent<CharacterMotor>();
				player.AddComponent<FPSInputController>();
				characterController.radius = this.radius;
				characterController.height = this.height;
				characterController.center = this.center;
				characterController.slopeLimit = this.slopeLimit;
				characterController.stepOffset = this.stepOffset;
				characterController.detectCollisions = this.detectCollisions;
			}

			// Token: 0x04000484 RID: 1156
			public float radius;

			// Token: 0x04000485 RID: 1157
			public float height;

			// Token: 0x04000486 RID: 1158
			public Vector3 center;

			// Token: 0x04000487 RID: 1159
			public float slopeLimit;

			// Token: 0x04000488 RID: 1160
			public float stepOffset;

			// Token: 0x04000489 RID: 1161
			public bool detectCollisions;
		}

		// Token: 0x020000EC RID: 236
		[CompilerGenerated]
		private sealed class <OnPlayerDie>d__29 : IEnumerator<object>, IDisposable, IEnumerator
		{
			// Token: 0x060004DC RID: 1244 RVA: 0x0001C5BC File Offset: 0x0001A7BC
			[DebuggerHidden]
			public <OnPlayerDie>d__29(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x060004DD RID: 1245 RVA: 0x0001C5CB File Offset: 0x0001A7CB
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060004DE RID: 1246 RVA: 0x0001C5D0 File Offset: 0x0001A7D0
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayerDeathManager playerDeathManager = this;
				switch (num)
				{
				case 0:
					this.<>1__state = -1;
					playerDeathManager.SendIDied(false);
					playerDeathManager.newsPhotos.SetActive(true);
					volume = 1f;
					break;
				case 1:
					this.<>1__state = -1;
					break;
				case 2:
					this.<>1__state = -1;
					playerDeathManager.fpsCamera.SetActive(false);
					playerDeathManager.gameVolume.Value = 1f;
					playerDeathManager.player.SetActive(false);
					playerDeathManager.deadBody.Value.SetActive(false);
					playerDeathManager.gui.SetActive(false);
					playerDeathManager.gameOverScreen.SetActive(true);
					goto IL_145;
				case 3:
					this.<>1__state = -1;
					goto IL_145;
				case 4:
				{
					this.<>1__state = -1;
					playerDeathManager.fpsCamera.transform.parent = playerDeathManager.fpsCameraParent;
					playerDeathManager.fpsCamera.transform.localPosition = Vector3.forward * -0.05f;
					playerDeathManager.fpsCamera.transform.localEulerAngles = Vector3.zero;
					playerDeathManager.fpsCamera.SetActive(true);
					playerDeathManager.fpsCamera.gameObject.tag = "MainCamera";
					Camera component = playerDeathManager.fpsCamera.GetComponent<Camera>();
					if (component == null)
					{
						Console.LogError("Player camera null after death", false);
					}
					else
					{
						component.enabled = true;
					}
					playerDeathManager.player.SetActive(true);
					playerDeathManager.player.transform.parent = null;
					playerDeathManager.player.transform.position = new Vector3(-1434.642f, 4.682786f, 1151.625f);
					playerDeathManager.player.transform.eulerAngles = new Vector3(0f, 252.6235f, 0f);
					playerDeathManager.charMotor.canControl = true;
					playerDeathManager.blood.enabled = false;
					playerDeathManager.playerThirst.Value = (playerDeathManager.playerHunger.Value = (playerDeathManager.playerStress.Value = (playerDeathManager.playerUrine.Value = (playerDeathManager.playerDirtiness.Value = (playerDeathManager.playerFatigue.Value = 0f)))));
					playerDeathManager.playerInCar.Value = (playerDeathManager.playerInWater.Value = false);
					playerDeathManager.gui.SetActive(true);
					playerDeathManager.optionsToggle.SetActive(true);
					playerDeathManager.newsPhotos.SetActive(false);
					playerDeathManager.gameOverRespawningLabel.SetActive(false);
					playerDeathManager.gameOverScreen.SetActive(false);
					playerDeathManager.gameObject.SetActive(false);
					playerDeathManager.playerMoney.Value = Mathf.Clamp(playerDeathManager.playerMoney.Value - 300f, 0f, float.MaxValue);
					Console.Log("You died! You have been charged 300 MK for respawn.", true);
					playerDeathManager.SendIDied(true);
					return false;
				}
				default:
					return false;
				}
				if (volume <= 0f)
				{
					playerDeathManager.gameVolume.Value = 0f;
					this.<>2__current = new WaitForSeconds(0.4f);
					this.<>1__state = 2;
					return true;
				}
				volume = Mathf.Clamp01(volume - Time.deltaTime / 2f);
				playerDeathManager.gameVolume.Value = volume;
				this.<>2__current = new WaitForEndOfFrame();
				this.<>1__state = 1;
				return true;
				IL_145:
				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
				{
					playerDeathManager.gameOverRespawningLabel.SetActive(true);
					this.<>2__current = new WaitForSeconds(2f);
					this.<>1__state = 4;
					return true;
				}
				this.<>2__current = new WaitForEndOfFrame();
				this.<>1__state = 3;
				return true;
			}

			// Token: 0x1700003A RID: 58
			// (get) Token: 0x060004DF RID: 1247 RVA: 0x0001C971 File Offset: 0x0001AB71
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060004E0 RID: 1248 RVA: 0x0001C979 File Offset: 0x0001AB79
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x1700003B RID: 59
			// (get) Token: 0x060004E1 RID: 1249 RVA: 0x0001C980 File Offset: 0x0001AB80
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0400048A RID: 1162
			private int <>1__state;

			// Token: 0x0400048B RID: 1163
			private object <>2__current;

			// Token: 0x0400048C RID: 1164
			public PlayerDeathManager <>4__this;

			// Token: 0x0400048D RID: 1165
			private float <volume>5__2;
		}
	}
}
