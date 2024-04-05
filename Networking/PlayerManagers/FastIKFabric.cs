using System;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x02000053 RID: 83
	internal class FastIKFabric : MonoBehaviour
	{
		// Token: 0x060001EB RID: 491 RVA: 0x0000AD3A File Offset: 0x00008F3A
		public static FastIKFabric CreateInstance(Transform lastBone, int length, Transform hint)
		{
			FastIKFabric fastIKFabric = lastBone.gameObject.AddComponent<FastIKFabric>();
			fastIKFabric.ChainLength = length;
			fastIKFabric.Pole = hint;
			fastIKFabric.Target = new GameObject(lastBone.name + "_target").transform;
			return fastIKFabric;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000AD75 File Offset: 0x00008F75
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000AD80 File Offset: 0x00008F80
		private void Init()
		{
			this.Bones = new Transform[this.ChainLength + 1];
			this.Positions = new Vector3[this.ChainLength + 1];
			this.BonesLength = new float[this.ChainLength];
			this.StartDirectionSucc = new Vector3[this.ChainLength + 1];
			this.StartRotationBone = new Quaternion[this.ChainLength + 1];
			this.Root = base.transform;
			for (int i = 0; i <= this.ChainLength; i++)
			{
				if (this.Root == null)
				{
					throw new UnityException("The chain value is longer than the ancestor chain!");
				}
				this.Root = this.Root.parent;
			}
			if (this.Target == null)
			{
				this.Target = new GameObject(base.gameObject.name + " Target").transform;
				this.SetPositionRootSpace(this.Target, this.GetPositionRootSpace(base.transform));
			}
			this.StartRotationTarget = this.GetRotationRootSpace(this.Target);
			Transform transform = base.transform;
			this.CompleteLength = 0f;
			for (int j = this.Bones.Length - 1; j >= 0; j--)
			{
				this.Bones[j] = transform;
				this.StartRotationBone[j] = this.GetRotationRootSpace(transform);
				if (j == this.Bones.Length - 1)
				{
					this.StartDirectionSucc[j] = this.GetPositionRootSpace(this.Target) - this.GetPositionRootSpace(transform);
				}
				else
				{
					this.StartDirectionSucc[j] = this.GetPositionRootSpace(this.Bones[j + 1]) - this.GetPositionRootSpace(transform);
					this.BonesLength[j] = this.StartDirectionSucc[j].magnitude;
					this.CompleteLength += this.BonesLength[j];
				}
				transform = transform.parent;
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000AF67 File Offset: 0x00009167
		private void Update()
		{
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000AF69 File Offset: 0x00009169
		private void LateUpdate()
		{
			this.ResolveIK();
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000AF74 File Offset: 0x00009174
		private void ResolveIK()
		{
			if (this.Target == null || !this.AllowIK)
			{
				return;
			}
			if (this.BonesLength.Length != this.ChainLength)
			{
				this.Init();
			}
			for (int i = 0; i < this.Bones.Length; i++)
			{
				this.Positions[i] = this.GetPositionRootSpace(this.Bones[i]);
			}
			Vector3 positionRootSpace = this.GetPositionRootSpace(this.Target);
			Quaternion rotationRootSpace = this.GetRotationRootSpace(this.Target);
			if ((positionRootSpace - this.GetPositionRootSpace(this.Bones[0])).sqrMagnitude >= this.CompleteLength * this.CompleteLength)
			{
				Vector3 normalized = (positionRootSpace - this.Positions[0]).normalized;
				for (int j = 1; j < this.Positions.Length; j++)
				{
					this.Positions[j] = this.Positions[j - 1] + normalized * this.BonesLength[j - 1];
				}
			}
			else
			{
				for (int k = 0; k < this.Positions.Length - 1; k++)
				{
					this.Positions[k + 1] = Vector3.Lerp(this.Positions[k + 1], this.Positions[k] + this.StartDirectionSucc[k], this.SnapBackStrength);
				}
				for (int l = 0; l < this.Iterations; l++)
				{
					for (int m = this.Positions.Length - 1; m > 0; m--)
					{
						if (m == this.Positions.Length - 1)
						{
							this.Positions[m] = positionRootSpace;
						}
						else
						{
							this.Positions[m] = this.Positions[m + 1] + (this.Positions[m] - this.Positions[m + 1]).normalized * this.BonesLength[m];
						}
					}
					for (int n = 1; n < this.Positions.Length; n++)
					{
						this.Positions[n] = this.Positions[n - 1] + (this.Positions[n] - this.Positions[n - 1]).normalized * this.BonesLength[n - 1];
					}
					if ((this.Positions[this.Positions.Length - 1] - positionRootSpace).sqrMagnitude < this.Delta * this.Delta)
					{
						break;
					}
				}
			}
			if (this.Pole != null)
			{
				Vector3 positionRootSpace2 = this.GetPositionRootSpace(this.Pole);
				for (int num = 1; num < this.Positions.Length - 1; num++)
				{
					Plane plane = new Plane(this.Positions[num + 1] - this.Positions[num - 1], this.Positions[num - 1]);
					Vector3 vector = this.ClosestPointOnPlane(plane, positionRootSpace2);
					float num2 = FastIKFabric.SignedAngle(this.ClosestPointOnPlane(plane, this.Positions[num]) - this.Positions[num - 1], vector - this.Positions[num - 1], plane.normal);
					this.Positions[num] = Quaternion.AngleAxis(num2, plane.normal) * (this.Positions[num] - this.Positions[num - 1]) + this.Positions[num - 1];
				}
			}
			for (int num3 = 0; num3 < this.Positions.Length; num3++)
			{
				if (num3 == this.Positions.Length - 1)
				{
					this.SetRotationRootSpace(this.Bones[num3], Quaternion.Inverse(rotationRootSpace) * this.StartRotationTarget * Quaternion.Inverse(this.StartRotationBone[num3]));
				}
				else
				{
					this.SetRotationRootSpace(this.Bones[num3], Quaternion.FromToRotation(this.StartDirectionSucc[num3], this.Positions[num3 + 1] - this.Positions[num3]) * Quaternion.Inverse(this.StartRotationBone[num3]));
				}
				this.SetPositionRootSpace(this.Bones[num3], this.Positions[num3]);
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000B43C File Offset: 0x0000963C
		private Vector3 ClosestPointOnPlane(Plane plane, Vector3 point)
		{
			float num = Vector3.Dot(plane.normal, point) + plane.distance;
			return point - plane.normal * num;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000B474 File Offset: 0x00009674
		public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
		{
			float num = Vector3.Angle(from, to);
			float num2 = from.y * to.z - from.z * to.y;
			float num3 = from.z * to.x - from.x * to.z;
			float num4 = from.x * to.y - from.y * to.x;
			float num5 = Mathf.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4);
			return num * num5;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000B500 File Offset: 0x00009700
		private Vector3 GetPositionRootSpace(Transform current)
		{
			if (this.Root == null)
			{
				return current.position;
			}
			return Quaternion.Inverse(this.Root.rotation) * (current.position - this.Root.position);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000B54D File Offset: 0x0000974D
		private void SetPositionRootSpace(Transform current, Vector3 position)
		{
			if (this.Root == null)
			{
				current.position = position;
				return;
			}
			current.position = this.Root.rotation * position + this.Root.position;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000B58C File Offset: 0x0000978C
		private Quaternion GetRotationRootSpace(Transform current)
		{
			if (this.Root == null)
			{
				return current.rotation;
			}
			return Quaternion.Inverse(current.rotation) * this.Root.rotation;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000B5BE File Offset: 0x000097BE
		private void SetRotationRootSpace(Transform current, Quaternion rotation)
		{
			if (this.Root == null)
			{
				current.rotation = rotation;
				return;
			}
			current.rotation = this.Root.rotation * rotation;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000B5ED File Offset: 0x000097ED
		public FastIKFabric()
		{
		}

		// Token: 0x040001BA RID: 442
		public int ChainLength = 2;

		// Token: 0x040001BB RID: 443
		public Transform Target;

		// Token: 0x040001BC RID: 444
		public Transform Pole;

		// Token: 0x040001BD RID: 445
		[Header("Solver Parameters")]
		public int Iterations = 10;

		// Token: 0x040001BE RID: 446
		public float Delta = 0.001f;

		// Token: 0x040001BF RID: 447
		[Range(0f, 1f)]
		public float SnapBackStrength = 1f;

		// Token: 0x040001C0 RID: 448
		public bool AllowIK = true;

		// Token: 0x040001C1 RID: 449
		protected float[] BonesLength;

		// Token: 0x040001C2 RID: 450
		protected float CompleteLength;

		// Token: 0x040001C3 RID: 451
		protected Transform[] Bones;

		// Token: 0x040001C4 RID: 452
		protected Vector3[] Positions;

		// Token: 0x040001C5 RID: 453
		protected Vector3[] StartDirectionSucc;

		// Token: 0x040001C6 RID: 454
		protected Quaternion[] StartRotationBone;

		// Token: 0x040001C7 RID: 455
		protected Quaternion StartRotationTarget;

		// Token: 0x040001C8 RID: 456
		protected Transform Root;
	}
}
