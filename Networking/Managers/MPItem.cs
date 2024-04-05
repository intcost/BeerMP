using System;
using BeerMP.Networking.PlayerManagers;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000058 RID: 88
	internal class MPItem : MonoBehaviour
	{
		// Token: 0x06000213 RID: 531 RVA: 0x0000C4EC File Offset: 0x0000A6EC
		internal void UpdateOwner()
		{
			if (MPItem.vehicleItemCollidersTransforms.Length != NetVehicleManager.vehicles.Count + 1 || MPItem.vehicleItemCollidersRadiuses.Length != NetVehicleManager.vehicles.Count + 1)
			{
				MPItem.vehicleItemCollidersTransforms = new Vector3[NetVehicleManager.vehicles.Count + 1];
				MPItem.vehicleItemCollidersRadiuses = new float[NetVehicleManager.vehicles.Count + 1];
				MPItem.vehicleItemCollidersTransforms[0] = NetBoatManager.instance.itemCollider.transform.localPosition;
				MPItem.vehicleItemCollidersRadiuses[0] = NetBoatManager.instance.itemCollider.radius;
				for (int i = 1; i < MPItem.vehicleItemCollidersTransforms.Length; i++)
				{
					MPItem.vehicleItemCollidersTransforms[i] = NetVehicleManager.vehicles[i - 1].itemCollider.transform.localPosition;
					MPItem.vehicleItemCollidersRadiuses[i] = NetVehicleManager.vehicles[i - 1].itemCollider.radius;
				}
			}
			if (!this.doUpdate)
			{
				return;
			}
			if (!this.RB.Rigidbody)
			{
				this.doUpdate = false;
				return;
			}
			if (NetPlayer.grabbedItemsHashes.Contains(this.RB.hash))
			{
				return;
			}
			int j = 0;
			while (j < MPItem.vehicleItemCollidersTransforms.Length)
			{
				Vector3 vector = ((j == 0) ? NetBoatManager.instance.boat.transform : NetVehicleManager.vehicles[j - 1].transform).position + MPItem.vehicleItemCollidersTransforms[j];
				float num = MPItem.vehicleItemCollidersRadiuses[j];
				num *= num;
				if ((base.transform.position - vector).sqrMagnitude < num)
				{
					ulong num2 = ((j == 0) ? NetBoatManager.instance.owner : NetVehicleManager.vehicles[j - 1].owner);
					if (this.RB.OwnerID != num2)
					{
						NetRigidbodyManager.RequestOwnership(this.RB, num2);
						return;
					}
					break;
				}
				else
				{
					j++;
				}
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000C6D6 File Offset: 0x0000A8D6
		public MPItem()
		{
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000C6E5 File Offset: 0x0000A8E5
		// Note: this type is marked as 'beforefieldinit'.
		static MPItem()
		{
		}

		// Token: 0x040001F3 RID: 499
		internal NetRigidbodyManager.OwnedRigidbody RB;

		// Token: 0x040001F4 RID: 500
		internal bool doUpdate = true;

		// Token: 0x040001F5 RID: 501
		private static Vector3[] vehicleItemCollidersTransforms = new Vector3[0];

		// Token: 0x040001F6 RID: 502
		private static float[] vehicleItemCollidersRadiuses = new float[0];
	}
}
