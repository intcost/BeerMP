using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BeerMP.Networking.Managers;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x02000051 RID: 81
	internal class PlayerAnimationManager : MonoBehaviour
	{
		// Token: 0x060001D8 RID: 472 RVA: 0x0000A35C File Offset: 0x0000855C
		public static void RegisterEvents()
		{
			NetEvent<PlayerAnimationManager>.Register("IsSprinting", delegate(ulong sender, Packet p)
			{
				NetManager.GetPlayerComponentById<NetPlayer>(sender).playerAnimationManager.isRunning = p.ReadBool(true);
			});
			NetEvent<PlayerAnimationManager>.Register("PlayerClicked", delegate(ulong sender, Packet p)
			{
				PlayerAnimationManager playerAnimationManager = NetManager.GetPlayerComponentById<NetPlayer>(sender).playerAnimationManager;
				if (playerAnimationManager.currentCar == null)
				{
					playerAnimationManager.OnPlayerClick();
				}
			});
			NetEvent<PlayerAnimationManager>.Register("Crouch", delegate(ulong sender, Packet p)
			{
				NetManager.GetPlayerComponentById<NetPlayer>(sender).playerAnimationManager.SetCrouch(p.ReadByte(true));
			});
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000A3E7 File Offset: 0x000085E7
		public void SetPassengerMode(bool set)
		{
			this.animator.SetBool("passenger", set);
			if (set)
			{
				this.animator.Play("PassengerMode");
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000A410 File Offset: 0x00008610
		public void GrabItem(Rigidbody item)
		{
			bool flag = item != null;
			this.rightHand.enabled = flag;
			this.rightHandOn = flag;
			this.syncer.rightArm = !flag;
			this.grabbedItem = (flag ? item.transform : null);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000A459 File Offset: 0x00008659
		public void OnPlayerClick()
		{
			if (this.rightHandOn)
			{
				return;
			}
			if (this.onPlayerClick != null)
			{
				base.StopCoroutine(this.onPlayerClick);
			}
			this.onPlayerClick = base.StartCoroutine(this.C_OnPlayerClick());
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000A48A File Offset: 0x0000868A
		private IEnumerator C_OnPlayerClick()
		{
			yield return base.StartCoroutine(this.C_LerpLayerWeight(1, true, 0.25f));
			yield return base.StartCoroutine(this.C_LerpLayerWeight(1, false, 0.25f));
			yield break;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000A499 File Offset: 0x00008699
		private IEnumerator C_LerpLayerWeight(int layer, bool on, float time = 0.2f)
		{
			time = 1f / time;
			float t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * time;
				this.animator.SetLayerWeight(layer, on ? t : (1f - t));
				yield return new WaitForEndOfFrame();
			}
			this.animator.SetLayerWeight(layer, on ? 1f : 0f);
			yield break;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000A4BD File Offset: 0x000086BD
		public void SetCrouch(byte val)
		{
			if (this.crouch > 0 != val > 0)
			{
				base.StartCoroutine(this.Crouch(val));
			}
			this.crouch = val;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000A4E4 File Offset: 0x000086E4
		public void SetPlayerInCar(bool inCar, NetVehicle car)
		{
			Behaviour behaviour = this.leftHand;
			Behaviour behaviour2 = this.rightHand;
			Behaviour behaviour3 = this.leftLeg;
			this.rightLeg.enabled = inCar;
			behaviour3.enabled = inCar;
			behaviour2.enabled = inCar;
			behaviour.enabled = inCar;
			if (inCar)
			{
				this.syncer.leftArm = false;
				this.syncer.rightArm = false;
				this.syncer.leftLeg = false;
				this.syncer.rightLeg = false;
			}
			else
			{
				this.syncer.leftArm = !this.leftHandOn;
				this.syncer.rightArm = !this.rightHandOn;
				this.syncer.leftLeg = true;
				this.syncer.rightLeg = true;
			}
			this.animator.enabled = !inCar;
			this.currentCar = (inCar ? car : null);
			this.rotationBendPivot.localEulerAngles = (inCar ? (Vector3.forward * 16f) : Vector3.zero);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000A5DC File Offset: 0x000087DC
		private void Start()
		{
			Transform transform = base.transform.parent.Find("skeleton ANIMATOR");
			this.animator = transform.GetComponent<Animator>();
			this.syncer = base.gameObject.AddComponent<AnimatorSyncer>();
			this.syncer.sourceSkeleton = transform;
			this.rotationBendPivot = base.transform.Find("pelvis/RotationBendPivot");
			this.leftHand = FastIKFabric.CreateInstance(base.transform.Find("pelvis/RotationBendPivot/spine_middle/spine_upper/collar_left/shoulder_left/arm_left/hand_left/finger_left"), 3, base.transform.Find("la_hint"));
			Transform transform2 = base.transform.Find("pelvis/RotationBendPivot/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/fingers_right");
			this.rightFingers = transform2.gameObject.AddComponent<HandPositionFixer>();
			this.rightHand = FastIKFabric.CreateInstance(transform2, 3, base.transform.Find("ra_hint"));
			this.leftLeg = FastIKFabric.CreateInstance(base.transform.Find("thig_left/knee_left/ankle_left"), 2, base.transform.Find("ll_hint"));
			this.rightLeg = FastIKFabric.CreateInstance(base.transform.Find("thig_right/knee_right/ankle_right"), 2, base.transform.Find("rl_hint"));
			this.SetPlayerInCar(false, null);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000A70C File Offset: 0x0000890C
		private void Update()
		{
			if (this.currentCar == null)
			{
				this.animator.GetCurrentAnimatorStateInfo(0);
				Vector3 position = base.transform.position;
				float sqrMagnitude = (position - this.lastPosition).sqrMagnitude;
				this.isWalking = sqrMagnitude > 1E-05f;
				this.animator.SetBool("walking", this.isWalking);
				this.animator.SetBool("running", this.isRunning);
				this.animator.SetInteger("crouch", (int)this.crouch);
				this.lastPosition = position;
			}
			else
			{
				this.leftHand.Target.position = this.currentCar.driverPivots.steeringWheel.position;
				this.leftHand.Target.rotation = this.currentCar.driverPivots.steeringWheel.rotation;
				this.rightHand.Target.position = this.currentCar.driverPivots.gearStick.position;
				this.rightHand.Target.rotation = this.currentCar.driverPivots.gearStick.rotation;
				this.rightLeg.Target.position = this.currentCar.driverPivots.throttlePedal.position;
				this.rightLeg.Target.rotation = this.currentCar.driverPivots.throttlePedal.rotation;
				if (this.currentCar.driverPivots.clutchPedal == null)
				{
					this.leftLeg.Target.position = this.currentCar.driverPivots.brakePedal.position;
					this.leftLeg.Target.rotation = this.currentCar.driverPivots.brakePedal.rotation;
				}
				else if (!this.brakeClutchLerping)
				{
					this.leftLeg.Target.position = Vector3.Lerp(this.currentCar.driverPivots.brakePedal.position, this.currentCar.driverPivots.clutchPedal.position, this.brakeClutchLerp);
					this.leftLeg.Target.rotation = Quaternion.Lerp(this.currentCar.driverPivots.brakePedal.rotation, this.currentCar.driverPivots.clutchPedal.rotation, this.brakeClutchLerp);
					if (this.currentCar.acc.brakeInput > 0f || this.currentCar.acc.clutchInput > 0f)
					{
						float num = 1f;
						if (this.currentCar.acc.brakeInput > 0f)
						{
							num = 0f;
						}
						if (num != this.brakeClutchLerp)
						{
							this.brakeClutchLerping = true;
							base.StartCoroutine(this.MoveIKTarget(this.leftLeg, this.currentCar.driverPivots.brakePedal, this.currentCar.driverPivots.clutchPedal, this.brakeClutchLerp, num, 0.2f, delegate
							{
								this.brakeClutchLerping = false;
							}));
							this.brakeClutchLerp = num;
						}
					}
				}
			}
			if (this.grabbedItem != null)
			{
				this.rightHand.Target.position = this.grabbedItem.position;
				this.rightHand.Target.rotation = base.transform.rotation;
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000AA82 File Offset: 0x00008C82
		private IEnumerator MoveIKTarget(FastIKFabric target, Transform from, Transform to, float oldT, float newT, float time, Action onFinished)
		{
			float t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * (1f / time);
				float num = Mathf.Lerp(oldT, newT, t);
				target.Target.position = Vector3.Lerp(from.position, to.position, num);
				target.Target.rotation = Quaternion.Lerp(from.rotation, to.rotation, num);
				yield return new WaitForEndOfFrame();
			}
			target.Target.position = to.position;
			target.Target.rotation = to.rotation;
			if (onFinished != null)
			{
				onFinished();
			}
			yield break;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000AABF File Offset: 0x00008CBF
		private IEnumerator Crouch(byte newCrouch)
		{
			float a = ((this.crouch == 0) ? (-0.38f) : ((this.crouch == 1) ? (-0.88f) : (-1.08f)));
			float b = ((newCrouch == 0) ? (-0.38f) : ((newCrouch == 1) ? (-0.88f) : (-1.08f)));
			float t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * 3f;
				Vector3 localPosition = this.charTf.localPosition;
				localPosition.y = Mathf.Lerp(a, b, t);
				this.charTf.localPosition = localPosition;
				yield return new WaitForEndOfFrame();
			}
			Vector3 localPosition2 = this.charTf.localPosition;
			localPosition2.y = b;
			this.charTf.localPosition = localPosition2;
			yield break;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000AAD5 File Offset: 0x00008CD5
		public PlayerAnimationManager()
		{
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000AADD File Offset: 0x00008CDD
		[CompilerGenerated]
		private void <Update>b__35_0()
		{
			this.brakeClutchLerping = false;
		}

		// Token: 0x04000197 RID: 407
		public bool isRunning;

		// Token: 0x04000198 RID: 408
		public byte crouch;

		// Token: 0x04000199 RID: 409
		public FastIKFabric leftHand;

		// Token: 0x0400019A RID: 410
		public FastIKFabric rightHand;

		// Token: 0x0400019B RID: 411
		public FastIKFabric leftLeg;

		// Token: 0x0400019C RID: 412
		public FastIKFabric rightLeg;

		// Token: 0x0400019D RID: 413
		private HandPositionFixer rightFingers;

		// Token: 0x0400019E RID: 414
		internal Transform charTf;

		// Token: 0x0400019F RID: 415
		private const float normalY = -0.38f;

		// Token: 0x040001A0 RID: 416
		private const float crouchY = -0.88f;

		// Token: 0x040001A1 RID: 417
		private const float crouch2Y = -1.08f;

		// Token: 0x040001A2 RID: 418
		private Animator animator;

		// Token: 0x040001A3 RID: 419
		private AnimatorSyncer syncer;

		// Token: 0x040001A4 RID: 420
		private Vector3 lastPosition;

		// Token: 0x040001A5 RID: 421
		private Transform rotationBendPivot;

		// Token: 0x040001A6 RID: 422
		private Transform grabbedItem;

		// Token: 0x040001A7 RID: 423
		internal bool isWalking;

		// Token: 0x040001A8 RID: 424
		internal bool leftHandOn;

		// Token: 0x040001A9 RID: 425
		internal bool rightHandOn;

		// Token: 0x040001AA RID: 426
		private NetVehicle currentCar;

		// Token: 0x040001AB RID: 427
		private float brakeClutchLerp;

		// Token: 0x040001AC RID: 428
		private bool brakeClutchLerping;

		// Token: 0x040001AD RID: 429
		private Coroutine onPlayerClick;

		// Token: 0x040001AE RID: 430
		public const string sprintingEvent = "IsSprinting";

		// Token: 0x040001AF RID: 431
		public const string clickEvent = "PlayerClicked";

		// Token: 0x040001B0 RID: 432
		public const string crouchEvent = "Crouch";

		// Token: 0x020000E6 RID: 230
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004BD RID: 1213 RVA: 0x0001C00A File Offset: 0x0001A20A
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004BE RID: 1214 RVA: 0x0001C016 File Offset: 0x0001A216
			public <>c()
			{
			}

			// Token: 0x060004BF RID: 1215 RVA: 0x0001C01E File Offset: 0x0001A21E
			internal void <RegisterEvents>b__26_0(ulong sender, Packet p)
			{
				NetManager.GetPlayerComponentById<NetPlayer>(sender).playerAnimationManager.isRunning = p.ReadBool(true);
			}

			// Token: 0x060004C0 RID: 1216 RVA: 0x0001C038 File Offset: 0x0001A238
			internal void <RegisterEvents>b__26_1(ulong sender, Packet p)
			{
				PlayerAnimationManager playerAnimationManager = NetManager.GetPlayerComponentById<NetPlayer>(sender).playerAnimationManager;
				if (playerAnimationManager.currentCar == null)
				{
					playerAnimationManager.OnPlayerClick();
				}
			}

			// Token: 0x060004C1 RID: 1217 RVA: 0x0001C05F File Offset: 0x0001A25F
			internal void <RegisterEvents>b__26_2(ulong sender, Packet p)
			{
				NetManager.GetPlayerComponentById<NetPlayer>(sender).playerAnimationManager.SetCrouch(p.ReadByte(true));
			}

			// Token: 0x04000465 RID: 1125
			public static readonly PlayerAnimationManager.<>c <>9 = new PlayerAnimationManager.<>c();

			// Token: 0x04000466 RID: 1126
			public static NetEventHandler <>9__26_0;

			// Token: 0x04000467 RID: 1127
			public static NetEventHandler <>9__26_1;

			// Token: 0x04000468 RID: 1128
			public static NetEventHandler <>9__26_2;
		}

		// Token: 0x020000E7 RID: 231
		[CompilerGenerated]
		private sealed class <C_LerpLayerWeight>d__31 : IEnumerator<object>, IDisposable, IEnumerator
		{
			// Token: 0x060004C2 RID: 1218 RVA: 0x0001C078 File Offset: 0x0001A278
			[DebuggerHidden]
			public <C_LerpLayerWeight>d__31(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x060004C3 RID: 1219 RVA: 0x0001C087 File Offset: 0x0001A287
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060004C4 RID: 1220 RVA: 0x0001C08C File Offset: 0x0001A28C
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayerAnimationManager playerAnimationManager = this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
					time = 1f / time;
					t = 0f;
				}
				if (t >= 1f)
				{
					playerAnimationManager.animator.SetLayerWeight(layer, on ? 1f : 0f);
					return false;
				}
				t += Time.deltaTime * time;
				playerAnimationManager.animator.SetLayerWeight(layer, on ? t : (1f - t));
				this.<>2__current = new WaitForEndOfFrame();
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x17000032 RID: 50
			// (get) Token: 0x060004C5 RID: 1221 RVA: 0x0001C16D File Offset: 0x0001A36D
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060004C6 RID: 1222 RVA: 0x0001C175 File Offset: 0x0001A375
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000033 RID: 51
			// (get) Token: 0x060004C7 RID: 1223 RVA: 0x0001C17C File Offset: 0x0001A37C
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x04000469 RID: 1129
			private int <>1__state;

			// Token: 0x0400046A RID: 1130
			private object <>2__current;

			// Token: 0x0400046B RID: 1131
			public float time;

			// Token: 0x0400046C RID: 1132
			public PlayerAnimationManager <>4__this;

			// Token: 0x0400046D RID: 1133
			public int layer;

			// Token: 0x0400046E RID: 1134
			public bool on;

			// Token: 0x0400046F RID: 1135
			private float <t>5__2;
		}

		// Token: 0x020000E8 RID: 232
		[CompilerGenerated]
		private sealed class <C_OnPlayerClick>d__30 : IEnumerator<object>, IDisposable, IEnumerator
		{
			// Token: 0x060004C8 RID: 1224 RVA: 0x0001C184 File Offset: 0x0001A384
			[DebuggerHidden]
			public <C_OnPlayerClick>d__30(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x060004C9 RID: 1225 RVA: 0x0001C193 File Offset: 0x0001A393
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060004CA RID: 1226 RVA: 0x0001C198 File Offset: 0x0001A398
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayerAnimationManager playerAnimationManager = this;
				switch (num)
				{
				case 0:
					this.<>1__state = -1;
					this.<>2__current = playerAnimationManager.StartCoroutine(playerAnimationManager.C_LerpLayerWeight(1, true, 0.25f));
					this.<>1__state = 1;
					return true;
				case 1:
					this.<>1__state = -1;
					this.<>2__current = playerAnimationManager.StartCoroutine(playerAnimationManager.C_LerpLayerWeight(1, false, 0.25f));
					this.<>1__state = 2;
					return true;
				case 2:
					this.<>1__state = -1;
					return false;
				default:
					return false;
				}
			}

			// Token: 0x17000034 RID: 52
			// (get) Token: 0x060004CB RID: 1227 RVA: 0x0001C221 File Offset: 0x0001A421
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060004CC RID: 1228 RVA: 0x0001C229 File Offset: 0x0001A429
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000035 RID: 53
			// (get) Token: 0x060004CD RID: 1229 RVA: 0x0001C230 File Offset: 0x0001A430
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x04000470 RID: 1136
			private int <>1__state;

			// Token: 0x04000471 RID: 1137
			private object <>2__current;

			// Token: 0x04000472 RID: 1138
			public PlayerAnimationManager <>4__this;
		}

		// Token: 0x020000E9 RID: 233
		[CompilerGenerated]
		private sealed class <Crouch>d__37 : IEnumerator<object>, IDisposable, IEnumerator
		{
			// Token: 0x060004CE RID: 1230 RVA: 0x0001C238 File Offset: 0x0001A438
			[DebuggerHidden]
			public <Crouch>d__37(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x060004CF RID: 1231 RVA: 0x0001C247 File Offset: 0x0001A447
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060004D0 RID: 1232 RVA: 0x0001C24C File Offset: 0x0001A44C
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PlayerAnimationManager playerAnimationManager = this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
					a = ((playerAnimationManager.crouch == 0) ? (-0.38f) : ((playerAnimationManager.crouch == 1) ? (-0.88f) : (-1.08f)));
					b = ((newCrouch == 0) ? (-0.38f) : ((newCrouch == 1) ? (-0.88f) : (-1.08f)));
					t = 0f;
				}
				if (t >= 1f)
				{
					Vector3 localPosition = playerAnimationManager.charTf.localPosition;
					localPosition.y = b;
					playerAnimationManager.charTf.localPosition = localPosition;
					return false;
				}
				t += Time.deltaTime * 3f;
				Vector3 localPosition2 = playerAnimationManager.charTf.localPosition;
				localPosition2.y = Mathf.Lerp(a, b, t);
				playerAnimationManager.charTf.localPosition = localPosition2;
				this.<>2__current = new WaitForEndOfFrame();
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x17000036 RID: 54
			// (get) Token: 0x060004D1 RID: 1233 RVA: 0x0001C377 File Offset: 0x0001A577
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060004D2 RID: 1234 RVA: 0x0001C37F File Offset: 0x0001A57F
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000037 RID: 55
			// (get) Token: 0x060004D3 RID: 1235 RVA: 0x0001C386 File Offset: 0x0001A586
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x04000473 RID: 1139
			private int <>1__state;

			// Token: 0x04000474 RID: 1140
			private object <>2__current;

			// Token: 0x04000475 RID: 1141
			public PlayerAnimationManager <>4__this;

			// Token: 0x04000476 RID: 1142
			public byte newCrouch;

			// Token: 0x04000477 RID: 1143
			private float <a>5__2;

			// Token: 0x04000478 RID: 1144
			private float <b>5__3;

			// Token: 0x04000479 RID: 1145
			private float <t>5__4;
		}

		// Token: 0x020000EA RID: 234
		[CompilerGenerated]
		private sealed class <MoveIKTarget>d__36 : IEnumerator<object>, IDisposable, IEnumerator
		{
			// Token: 0x060004D4 RID: 1236 RVA: 0x0001C38E File Offset: 0x0001A58E
			[DebuggerHidden]
			public <MoveIKTarget>d__36(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x060004D5 RID: 1237 RVA: 0x0001C39D File Offset: 0x0001A59D
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x060004D6 RID: 1238 RVA: 0x0001C3A0 File Offset: 0x0001A5A0
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
					t = 0f;
				}
				if (t >= 1f)
				{
					target.Target.position = to.position;
					target.Target.rotation = to.rotation;
					Action action = onFinished;
					if (action != null)
					{
						action();
					}
					return false;
				}
				t += Time.deltaTime * (1f / time);
				float num2 = Mathf.Lerp(oldT, newT, t);
				target.Target.position = Vector3.Lerp(from.position, to.position, num2);
				target.Target.rotation = Quaternion.Lerp(from.rotation, to.rotation, num2);
				this.<>2__current = new WaitForEndOfFrame();
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x17000038 RID: 56
			// (get) Token: 0x060004D7 RID: 1239 RVA: 0x0001C4D9 File Offset: 0x0001A6D9
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x060004D8 RID: 1240 RVA: 0x0001C4E1 File Offset: 0x0001A6E1
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000039 RID: 57
			// (get) Token: 0x060004D9 RID: 1241 RVA: 0x0001C4E8 File Offset: 0x0001A6E8
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0400047A RID: 1146
			private int <>1__state;

			// Token: 0x0400047B RID: 1147
			private object <>2__current;

			// Token: 0x0400047C RID: 1148
			public float time;

			// Token: 0x0400047D RID: 1149
			public float oldT;

			// Token: 0x0400047E RID: 1150
			public float newT;

			// Token: 0x0400047F RID: 1151
			public FastIKFabric target;

			// Token: 0x04000480 RID: 1152
			public Transform from;

			// Token: 0x04000481 RID: 1153
			public Transform to;

			// Token: 0x04000482 RID: 1154
			public Action onFinished;

			// Token: 0x04000483 RID: 1155
			private float <t>5__2;
		}
	}
}
