using System;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200006A RID: 106
	public class NetVehicleDriverPivots
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x00015860 File Offset: 0x00013A60
		public Transform gearStick
		{
			get
			{
				if (this.gearSticks == null)
				{
					return null;
				}
				for (int i = 0; i < this.gearSticks.Length; i++)
				{
					if (this.gearSticks[i].gameObject.activeInHierarchy)
					{
						return this.gearSticks[i];
					}
				}
				if (this.gearSticks.Length == 0)
				{
					return null;
				}
				return this.gearSticks[0];
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x000158BA File Offset: 0x00013ABA
		public NetVehicleDriverPivots()
		{
		}

		// Token: 0x040002F3 RID: 755
		public Transform throttlePedal;

		// Token: 0x040002F4 RID: 756
		public Transform brakePedal;

		// Token: 0x040002F5 RID: 757
		public Transform clutchPedal;

		// Token: 0x040002F6 RID: 758
		public Transform steeringWheel;

		// Token: 0x040002F7 RID: 759
		public Transform driverParent;

		// Token: 0x040002F8 RID: 760
		public Transform[] gearSticks;
	}
}
