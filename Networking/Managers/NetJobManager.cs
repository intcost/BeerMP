using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200005A RID: 90
	[ManagerCreate(10)]
	public class NetJobManager : MonoBehaviour
	{
		// Token: 0x06000224 RID: 548 RVA: 0x0000CEE8 File Offset: 0x0000B0E8
		private void Start()
		{
			Action action = delegate
			{
			};
			if (ObjectsLoader.IsGameLoaded)
			{
				action();
				return;
			}
			ObjectsLoader.gameLoaded += action;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000CF34 File Offset: 0x0000B134
		public NetJobManager()
		{
		}

		// Token: 0x04000200 RID: 512
		private PlayMakerFSM logwall;

		// Token: 0x04000201 RID: 513
		private PlayMakerFSM inspectionOrder;

		// Token: 0x04000202 RID: 514
		private PlayMakerFSM teimoAdvertPile;

		// Token: 0x04000203 RID: 515
		private PlayMakerFSM waterFacilityCashRegister;

		// Token: 0x04000204 RID: 516
		private List<PlayMakerFSM> mailboxes = new List<PlayMakerFSM>();

		// Token: 0x04000205 RID: 517
		private List<FsmGameObject> mailboxAdvertVariables = new List<FsmGameObject>();

		// Token: 0x04000206 RID: 518
		private FsmGameObject currentLog;

		// Token: 0x04000207 RID: 519
		private FsmGameObject newSheet;

		// Token: 0x04000208 RID: 520
		private FsmEvent logwallEvent;

		// Token: 0x04000209 RID: 521
		private FsmEvent inspectionOrderEvent;

		// Token: 0x0400020A RID: 522
		private FsmEvent spawnAdvertSheetEvent;

		// Token: 0x0400020B RID: 523
		private FsmEvent waterFacilityCalcPriceEvent;

		// Token: 0x0400020C RID: 524
		private FsmEvent waterFacilityPayEvent;

		// Token: 0x0400020D RID: 525
		private List<FsmEvent> mailboxDropAdvertEvents = new List<FsmEvent>();

		// Token: 0x0400020E RID: 526
		private Dictionary<int, FixedJoint> logs = new Dictionary<int, FixedJoint>();

		// Token: 0x0400020F RID: 527
		private List<GameObject> advertSheets = new List<GameObject>();

		// Token: 0x04000210 RID: 528
		private List<bool> mailboxesDropping = new List<bool>();

		// Token: 0x04000211 RID: 529
		private bool updatingLogs;

		// Token: 0x04000212 RID: 530
		private bool triggeringInspection;

		// Token: 0x04000213 RID: 531
		private bool spawningSheet;

		// Token: 0x04000214 RID: 532
		private bool updatingWaterFacility;

		// Token: 0x04000215 RID: 533
		internal static int logsCount;

		// Token: 0x04000216 RID: 534
		private NetEvent<NetJobManager> spawnLog;

		// Token: 0x04000217 RID: 535
		private NetEvent<NetJobManager> cutLog;

		// Token: 0x04000218 RID: 536
		private NetEvent<NetJobManager> triggerInspection;

		// Token: 0x04000219 RID: 537
		private NetEvent<NetJobManager> advertUpdate;

		// Token: 0x0400021A RID: 538
		private NetEvent<NetJobManager> advertInMailbox;

		// Token: 0x0400021B RID: 539
		private NetEvent<NetJobManager> waterFacilityUpdateEvent;

		// Token: 0x020000EF RID: 239
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004EA RID: 1258 RVA: 0x0001CA8B File Offset: 0x0001AC8B
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004EB RID: 1259 RVA: 0x0001CA97 File Offset: 0x0001AC97
			public <>c()
			{
			}

			// Token: 0x060004EC RID: 1260 RVA: 0x0001CA9F File Offset: 0x0001AC9F
			internal void <Start>b__28_0()
			{
			}

			// Token: 0x04000495 RID: 1173
			public static readonly NetJobManager.<>c <>9 = new NetJobManager.<>c();

			// Token: 0x04000496 RID: 1174
			public static Action <>9__28_0;
		}
	}
}
