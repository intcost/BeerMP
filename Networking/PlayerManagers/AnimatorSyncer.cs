using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x02000052 RID: 82
	internal class AnimatorSyncer : MonoBehaviour
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x0000AAE6 File Offset: 0x00008CE6
		private void Start()
		{
			this.InitBones();
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000AAF0 File Offset: 0x00008CF0
		private void InitBones()
		{
			int num = 0;
			this.LoopBones(this.sourceSkeleton.Find("pelvis"), "", ref num);
			this.pelvisEndIndex = this.sourceBones.Count;
			this.LoopBones(this.sourceSkeleton.Find("thig_left"), "", ref num);
			this.LoopBones(this.sourceSkeleton.Find("thig_right"), "", ref num);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000AB68 File Offset: 0x00008D68
		private void LoopBones(Transform bone, string subPath, ref int successCount)
		{
			if (subPath != "")
			{
				subPath += "/";
			}
			subPath += bone.name;
			for (int i = 0; i < bone.childCount; i++)
			{
				Transform child = bone.GetChild(i);
				this.LoopBones(child, subPath, ref successCount);
			}
			this.sourceBones.Add(bone);
			Transform transform = base.transform.Find(subPath);
			this.targetBones.Add(transform);
			if (transform != null && bone != null)
			{
				successCount++;
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000ABFC File Offset: 0x00008DFC
		private void LateUpdate()
		{
			for (int i = 0; i < this.sourceBones.Count; i++)
			{
				string text = this.sourceBones[i].name.ToLower();
				bool flag = i >= this.pelvisEndIndex;
				bool flag2 = text.Contains("head");
				bool flag3 = text.Contains("left");
				bool flag4 = text.Contains("right");
				if ((this.leftLeg || !flag3 || !flag) && (this.rightLeg || !flag4 || !flag) && (this.leftArm || !flag3 || flag) && (this.rightArm || !flag4 || flag) && (this.head || !flag2))
				{
					this.targetBones[i].localPosition = this.sourceBones[i].localPosition;
					this.targetBones[i].localEulerAngles = this.sourceBones[i].localEulerAngles;
				}
			}
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000AD00 File Offset: 0x00008F00
		public AnimatorSyncer()
		{
		}

		// Token: 0x040001B1 RID: 433
		internal Transform sourceSkeleton;

		// Token: 0x040001B2 RID: 434
		private List<Transform> sourceBones = new List<Transform>();

		// Token: 0x040001B3 RID: 435
		private List<Transform> targetBones = new List<Transform>();

		// Token: 0x040001B4 RID: 436
		private int pelvisEndIndex;

		// Token: 0x040001B5 RID: 437
		public bool head;

		// Token: 0x040001B6 RID: 438
		public bool leftLeg = true;

		// Token: 0x040001B7 RID: 439
		public bool rightLeg = true;

		// Token: 0x040001B8 RID: 440
		public bool leftArm = true;

		// Token: 0x040001B9 RID: 441
		public bool rightArm = true;
	}
}
