using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeerMP.Helpers
{
	// Token: 0x02000083 RID: 131
	public static class Raycaster
	{
		// Token: 0x060003B0 RID: 944 RVA: 0x0001B5C4 File Offset: 0x000197C4
		public static bool Raycast(Collider collider, float distance = 1f, int layerMask = -1)
		{
			RaycastHit raycastHit;
			return Raycaster.Raycast(out raycastHit, distance, layerMask) && raycastHit.collider == collider;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0001B5EC File Offset: 0x000197EC
		public static bool Raycast(out RaycastHit hit, float distance = 1f, int layerMask = -1)
		{
			if (Raycaster.camera == null)
			{
				Raycaster.camera = Camera.main;
			}
			if (Time.frameCount != Raycaster.lastRaycastFrame)
			{
				Raycaster.raycasts.Clear();
				Raycaster.lastRaycastFrame = Time.frameCount;
			}
			Ray ray = Raycaster.camera.ScreenPointToRay(Input.mousePosition);
			if (Raycaster.raycasts.ContainsKey(layerMask))
			{
				RaycastHit raycastHit = Raycaster.raycasts[layerMask];
				if (raycastHit.distance >= distance || raycastHit.collider != null)
				{
					hit = raycastHit;
					return raycastHit.distance < distance;
				}
			}
			Physics.Raycast(ray, out hit, distance, layerMask);
			Raycaster.raycasts[layerMask] = hit;
			return hit.collider;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0001B6AA File Offset: 0x000198AA
		// Note: this type is marked as 'beforefieldinit'.
		static Raycaster()
		{
		}

		// Token: 0x0400038D RID: 909
		private static readonly Dictionary<int, RaycastHit> raycasts = new Dictionary<int, RaycastHit>();

		// Token: 0x0400038E RID: 910
		private static int lastRaycastFrame;

		// Token: 0x0400038F RID: 911
		private static Camera camera;
	}
}
