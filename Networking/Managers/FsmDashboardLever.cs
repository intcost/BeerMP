using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Steamworks;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000067 RID: 103
	internal class FsmDashboardLever
	{
		// Token: 0x060002C6 RID: 710 RVA: 0x000146B4 File Offset: 0x000128B4
		public FsmDashboardLever(PlayMakerFSM fsm)
		{
			this.hash = fsm.transform.GetGameobjectHashString().GetHashCode();
			this.fsm = fsm;
			this.SetupFSM();
			if (FsmDashboardLever.updateEvent == null)
			{
				FsmDashboardLever.updateEvent = NetEvent<FsmDashboardLever>.Register("Update", new NetEventHandler(FsmDashboardLever.OnLeverUpdate));
			}
			if (FsmDashboardLever.initSync == null)
			{
				FsmDashboardLever.initSync = NetEvent<FsmDashboardLever>.Register("InitSync", delegate(ulong s, Packet p)
				{
					while (p.UnreadLength() > 0)
					{
						int hash = p.ReadInt(true);
						ulong num = (ulong)p.ReadLong(true);
						float num2 = p.ReadFloat(true);
						FsmDashboardLever fsmDashboardLever = FsmDashboardLever.levers.FirstOrDefault((FsmDashboardLever l) => l.hash == hash);
						if (fsmDashboardLever == null)
						{
							Console.LogError(string.Format("Received dashboard lever init sync from {0} but the hash {1} cannot be found", NetManager.playerNames[(CSteamID)s], hash), false);
							return;
						}
						fsmDashboardLever.owner = num;
						fsmDashboardLever.SetKnobPos(num2);
					}
				});
				BeerMPGlobals.OnMemberReady += delegate(ulong u)
				{
					if (!BeerMPGlobals.IsHost)
					{
						return;
					}
					using (Packet packet = new Packet(0))
					{
						for (int i = 0; i < FsmDashboardLever.levers.Count; i++)
						{
							packet.Write(FsmDashboardLever.levers[i].hash, -1);
							packet.Write((long)FsmDashboardLever.levers[i].owner, -1);
							packet.Write(FsmDashboardLever.levers[i].knobPos.Value, -1);
						}
						FsmDashboardLever.initSync.Send(packet, u, true);
					}
				};
			}
			BeerMPGlobals.OnMemberExit += delegate(ulong user)
			{
				if (this.owner == user)
				{
					this.StopMoving(null);
				}
			};
			FsmDashboardLever.levers.Add(this);
			NetManager.sceneLoaded = (Action<GameScene>)Delegate.Combine(NetManager.sceneLoaded, new Action<GameScene>(delegate(GameScene a)
			{
				if (FsmDashboardLever.levers.Contains(this))
				{
					FsmDashboardLever.levers.Remove(this);
				}
			}));
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x000147B8 File Offset: 0x000129B8
		private static void OnLeverUpdate(ulong sender, Packet packet)
		{
			int hash = packet.ReadInt(true);
			FsmDashboardLever fsmDashboardLever = FsmDashboardLever.levers.FirstOrDefault((FsmDashboardLever l) => l.hash == hash);
			if (fsmDashboardLever == null)
			{
				Console.LogError(string.Format("Received dashboard lever triggered action from {0} but the hash {1} cannot be found", NetManager.playerNames[(CSteamID)sender], hash), false);
				return;
			}
			bool flag = packet.ReadBool(true);
			bool flag2 = packet.ReadBool(true);
			float num = packet.ReadFloat(true);
			if (flag)
			{
				fsmDashboardLever.StartMoving(flag2, sender);
				return;
			}
			fsmDashboardLever.StopMoving(new float?(num));
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0001484C File Offset: 0x00012A4C
		public void Update()
		{
			if (this.owner != 0UL && this.owner != BeerMPGlobals.UserID)
			{
				float num = this.knobPos.Value;
				num += (this.moveDirection ? this.moveRate : (-this.moveRate)) * (this.movePerSecond ? Time.deltaTime : 1f);
				num = Mathf.Clamp(num, this.minMove, this.maxMove);
				this.SetKnobPos(num);
				return;
			}
			if (this.owner == BeerMPGlobals.UserID && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
			{
				this.Move(null);
			}
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x000148F0 File Offset: 0x00012AF0
		private void StartMoving(bool direction, ulong newOwner)
		{
			this.owner = newOwner;
			this.fsm.enabled = false;
			this.moveDirection = direction;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0001490C File Offset: 0x00012B0C
		private void StopMoving(float? knobPos)
		{
			this.owner = 0UL;
			this.fsm.enabled = true;
			if (knobPos != null)
			{
				float value = knobPos.Value;
				this.SetKnobPos(value);
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00014948 File Offset: 0x00012B48
		private void SetKnobPos(float value)
		{
			this.knobPos.Value = value;
			if (this.multiplyResult != null)
			{
				float num = value * this.multiplyRate;
				if (!float.IsNaN(this.minmultiply) && !float.IsNaN(this.maxmultiply))
				{
					num = Mathf.Clamp(num, this.minmultiply, this.maxmultiply);
				}
				this.multiplyResult.Value = num;
			}
			if (this.mesh != null)
			{
				Vector3 localPosition = this.mesh.localPosition;
				localPosition[this.axis] = value;
				this.mesh.localPosition = localPosition;
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x000149E0 File Offset: 0x00012BE0
		private void SetupFSM()
		{
			this.fsm.InsertAction("INCREASE", delegate
			{
				this.Move(new bool?(true));
			}, 0);
			this.fsm.InsertAction("DECREASE", delegate
			{
				this.Move(new bool?(false));
			}, 0);
			try
			{
				FsmState state = this.fsm.GetState("INCREASE");
				FloatAdd floatAdd = state.Actions.First((FsmStateAction f) => f is FloatAdd) as FloatAdd;
				this.knobPos = floatAdd.floatVariable;
				this.moveRate = floatAdd.add.Value;
				this.movePerSecond = floatAdd.perSecond;
				FsmStateAction[] array = state.Actions.Where((FsmStateAction f) => f is FloatClamp).ToArray<FsmStateAction>();
				FloatClamp floatClamp = array[0] as FloatClamp;
				this.minMove = floatClamp.minValue.Value;
				this.maxMove = floatClamp.maxValue.Value;
				FloatOperator floatOperator = state.Actions.FirstOrDefault((FsmStateAction f) => f is FloatOperator) as FloatOperator;
				if (floatOperator != null)
				{
					this.multiplyResult = floatOperator.storeResult;
					this.multiplyRate = floatOperator.float2.Value;
					if (floatOperator.operation == FloatOperator.Operation.Divide)
					{
						this.multiplyRate = 1f / this.multiplyRate;
					}
					if (array.Length >= 2)
					{
						FloatClamp floatClamp2 = array[1] as FloatClamp;
						this.minmultiply = floatClamp2.minValue.Value;
						this.maxmultiply = floatClamp2.maxValue.Value;
					}
					else
					{
						this.minmultiply = (this.maxmultiply = float.NaN);
					}
				}
				SetPosition setPosition = state.Actions.FirstOrDefault((FsmStateAction f) => f is SetPosition) as SetPosition;
				if (setPosition != null)
				{
					this.mesh = setPosition.gameObject.GameObject.Value.transform;
					if (setPosition.x != null && !setPosition.x.IsNone)
					{
						this.axis = 0;
					}
					else if (setPosition.y != null && !setPosition.y.IsNone)
					{
						this.axis = 1;
					}
					else if (setPosition.z != null && !setPosition.z.IsNone)
					{
						this.axis = 2;
					}
				}
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("Failed to setup dashboard lever {0} ({1}): {2}, {3}, {4}", new object[]
				{
					this.hash,
					this.fsm.transform.GetGameobjectHashString(),
					ex.GetType(),
					ex.Message,
					ex.StackTrace
				}), false);
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00014CCC File Offset: 0x00012ECC
		private void Move(bool? direction)
		{
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write(direction != null, -1);
				packet.Write(direction != null && direction.Value, -1);
				packet.Write(this.knobPos.Value, -1);
				this.owner = ((direction != null) ? BeerMPGlobals.UserID : 0UL);
				FsmDashboardLever.updateEvent.Send(packet, true);
			}
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00014D68 File Offset: 0x00012F68
		// Note: this type is marked as 'beforefieldinit'.
		static FsmDashboardLever()
		{
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00014D74 File Offset: 0x00012F74
		[CompilerGenerated]
		private void <.ctor>b__18_2(ulong user)
		{
			if (this.owner == user)
			{
				this.StopMoving(null);
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00014D99 File Offset: 0x00012F99
		[CompilerGenerated]
		private void <.ctor>b__18_3(GameScene a)
		{
			if (FsmDashboardLever.levers.Contains(this))
			{
				FsmDashboardLever.levers.Remove(this);
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00014DB4 File Offset: 0x00012FB4
		[CompilerGenerated]
		private void <SetupFSM>b__24_0()
		{
			this.Move(new bool?(true));
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00014DC2 File Offset: 0x00012FC2
		[CompilerGenerated]
		private void <SetupFSM>b__24_1()
		{
			this.Move(new bool?(false));
		}

		// Token: 0x040002CB RID: 715
		private PlayMakerFSM fsm;

		// Token: 0x040002CC RID: 716
		private FsmFloat knobPos;

		// Token: 0x040002CD RID: 717
		private FsmFloat multiplyResult;

		// Token: 0x040002CE RID: 718
		private Transform mesh;

		// Token: 0x040002CF RID: 719
		private int axis;

		// Token: 0x040002D0 RID: 720
		private int hash;

		// Token: 0x040002D1 RID: 721
		private ulong owner;

		// Token: 0x040002D2 RID: 722
		private float moveRate;

		// Token: 0x040002D3 RID: 723
		private float minMove;

		// Token: 0x040002D4 RID: 724
		private float maxMove;

		// Token: 0x040002D5 RID: 725
		private bool movePerSecond;

		// Token: 0x040002D6 RID: 726
		private bool moveDirection;

		// Token: 0x040002D7 RID: 727
		private float multiplyRate;

		// Token: 0x040002D8 RID: 728
		private float minmultiply;

		// Token: 0x040002D9 RID: 729
		private float maxmultiply;

		// Token: 0x040002DA RID: 730
		private static NetEvent<FsmDashboardLever> updateEvent;

		// Token: 0x040002DB RID: 731
		private static NetEvent<FsmDashboardLever> initSync;

		// Token: 0x040002DC RID: 732
		private static List<FsmDashboardLever> levers = new List<FsmDashboardLever>();

		// Token: 0x0200011A RID: 282
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000593 RID: 1427 RVA: 0x0001F656 File Offset: 0x0001D856
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000594 RID: 1428 RVA: 0x0001F662 File Offset: 0x0001D862
			public <>c()
			{
			}

			// Token: 0x06000595 RID: 1429 RVA: 0x0001F66C File Offset: 0x0001D86C
			internal void <.ctor>b__18_0(ulong s, Packet p)
			{
				while (p.UnreadLength() > 0)
				{
					FsmDashboardLever.<>c__DisplayClass18_0 CS$<>8__locals1 = new FsmDashboardLever.<>c__DisplayClass18_0();
					CS$<>8__locals1.hash = p.ReadInt(true);
					ulong num = (ulong)p.ReadLong(true);
					float num2 = p.ReadFloat(true);
					FsmDashboardLever fsmDashboardLever = FsmDashboardLever.levers.FirstOrDefault((FsmDashboardLever l) => l.hash == CS$<>8__locals1.hash);
					if (fsmDashboardLever == null)
					{
						Console.LogError(string.Format("Received dashboard lever init sync from {0} but the hash {1} cannot be found", NetManager.playerNames[(CSteamID)s], CS$<>8__locals1.hash), false);
						return;
					}
					fsmDashboardLever.owner = num;
					fsmDashboardLever.SetKnobPos(num2);
				}
			}

			// Token: 0x06000596 RID: 1430 RVA: 0x0001F6FC File Offset: 0x0001D8FC
			internal void <.ctor>b__18_1(ulong u)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				using (Packet packet = new Packet(0))
				{
					for (int i = 0; i < FsmDashboardLever.levers.Count; i++)
					{
						packet.Write(FsmDashboardLever.levers[i].hash, -1);
						packet.Write((long)FsmDashboardLever.levers[i].owner, -1);
						packet.Write(FsmDashboardLever.levers[i].knobPos.Value, -1);
					}
					FsmDashboardLever.initSync.Send(packet, u, true);
				}
			}

			// Token: 0x06000597 RID: 1431 RVA: 0x0001F7A0 File Offset: 0x0001D9A0
			internal bool <SetupFSM>b__24_2(FsmStateAction f)
			{
				return f is FloatAdd;
			}

			// Token: 0x06000598 RID: 1432 RVA: 0x0001F7AB File Offset: 0x0001D9AB
			internal bool <SetupFSM>b__24_3(FsmStateAction f)
			{
				return f is FloatClamp;
			}

			// Token: 0x06000599 RID: 1433 RVA: 0x0001F7B6 File Offset: 0x0001D9B6
			internal bool <SetupFSM>b__24_4(FsmStateAction f)
			{
				return f is FloatOperator;
			}

			// Token: 0x0600059A RID: 1434 RVA: 0x0001F7C1 File Offset: 0x0001D9C1
			internal bool <SetupFSM>b__24_5(FsmStateAction f)
			{
				return f is SetPosition;
			}

			// Token: 0x04000548 RID: 1352
			public static readonly FsmDashboardLever.<>c <>9 = new FsmDashboardLever.<>c();

			// Token: 0x04000549 RID: 1353
			public static NetEventHandler <>9__18_0;

			// Token: 0x0400054A RID: 1354
			public static Action<ulong> <>9__18_1;

			// Token: 0x0400054B RID: 1355
			public static Func<FsmStateAction, bool> <>9__24_2;

			// Token: 0x0400054C RID: 1356
			public static Func<FsmStateAction, bool> <>9__24_3;

			// Token: 0x0400054D RID: 1357
			public static Func<FsmStateAction, bool> <>9__24_4;

			// Token: 0x0400054E RID: 1358
			public static Func<FsmStateAction, bool> <>9__24_5;
		}

		// Token: 0x0200011B RID: 283
		[CompilerGenerated]
		private sealed class <>c__DisplayClass18_0
		{
			// Token: 0x0600059B RID: 1435 RVA: 0x0001F7CC File Offset: 0x0001D9CC
			public <>c__DisplayClass18_0()
			{
			}

			// Token: 0x0600059C RID: 1436 RVA: 0x0001F7D4 File Offset: 0x0001D9D4
			internal bool <.ctor>b__4(FsmDashboardLever l)
			{
				return l.hash == this.hash;
			}

			// Token: 0x0400054F RID: 1359
			public int hash;
		}

		// Token: 0x0200011C RID: 284
		[CompilerGenerated]
		private sealed class <>c__DisplayClass19_0
		{
			// Token: 0x0600059D RID: 1437 RVA: 0x0001F7E4 File Offset: 0x0001D9E4
			public <>c__DisplayClass19_0()
			{
			}

			// Token: 0x0600059E RID: 1438 RVA: 0x0001F7EC File Offset: 0x0001D9EC
			internal bool <OnLeverUpdate>b__0(FsmDashboardLever l)
			{
				return l.hash == this.hash;
			}

			// Token: 0x04000550 RID: 1360
			public int hash;
		}
	}
}
