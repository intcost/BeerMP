using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Helpers
{
	// Token: 0x02000080 RID: 128
	public static class ObjectUtilities
	{
		// Token: 0x0600039B RID: 923 RVA: 0x0001B062 File Offset: 0x00019262
		public static int GetPlaymakerHash(this PlayMakerFSM fsm)
		{
			return (fsm.transform.GetGameobjectHashString() + "_" + fsm.FsmName).GetHashCode();
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0001B084 File Offset: 0x00019284
		public static string GetGameobjectHashString(this Transform obj)
		{
			if (obj.gameObject.IsPrefab())
			{
				return obj.name + "_PREFAB";
			}
			PlayMakerFSM playMakerFSM = obj.GetComponents<PlayMakerFSM>().FirstOrDefault((PlayMakerFSM f) => f.FsmName == "Use");
			if (playMakerFSM == null)
			{
				return obj.GetPath();
			}
			FsmString fsmString = playMakerFSM.FsmVariables.StringVariables.FirstOrDefault((FsmString s) => s.Name == "ID");
			if (fsmString == null)
			{
				return obj.GetPath();
			}
			if (string.IsNullOrEmpty(fsmString.Value))
			{
				return obj.GetPath();
			}
			return fsmString.Value;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x0001B13E File Offset: 0x0001933E
		public static bool IsPrefab(this GameObject go)
		{
			return !go.activeInHierarchy && go.activeSelf && go.transform.parent == null;
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0001B164 File Offset: 0x00019364
		public static string GetPath(this Transform transform)
		{
			string text;
			if (transform.parent == null)
			{
				text = transform.name ?? "";
			}
			else
			{
				text = string.Format("{0}/{1}_{2}", transform.parent.GetFullPath(), transform.name, transform.GetSiblingIndex());
			}
			return text;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0001B1C0 File Offset: 0x000193C0
		public static string GetFullPath(this Transform transform)
		{
			string text = string.Format("{0}_{1}", transform.name, transform.GetSiblingIndex());
			if (transform.parent == null)
			{
				return text;
			}
			Transform transform2 = transform.parent;
			while (transform2 != null)
			{
				text = string.Format("{0}_{1}/{2}", transform2.name, transform2.GetSiblingIndex(), text);
				transform2 = transform2.parent;
			}
			return text;
		}

		// Token: 0x02000153 RID: 339
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000643 RID: 1603 RVA: 0x0002138D File Offset: 0x0001F58D
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000644 RID: 1604 RVA: 0x00021399 File Offset: 0x0001F599
			public <>c()
			{
			}

			// Token: 0x06000645 RID: 1605 RVA: 0x000213A1 File Offset: 0x0001F5A1
			internal bool <GetGameobjectHashString>b__1_0(PlayMakerFSM f)
			{
				return f.FsmName == "Use";
			}

			// Token: 0x06000646 RID: 1606 RVA: 0x000213B3 File Offset: 0x0001F5B3
			internal bool <GetGameobjectHashString>b__1_1(FsmString s)
			{
				return s.Name == "ID";
			}

			// Token: 0x040005C6 RID: 1478
			public static readonly ObjectUtilities.<>c <>9 = new ObjectUtilities.<>c();

			// Token: 0x040005C7 RID: 1479
			public static Func<PlayMakerFSM, bool> <>9__1_0;

			// Token: 0x040005C8 RID: 1480
			public static Func<FsmString, bool> <>9__1_1;
		}
	}
}
