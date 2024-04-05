using System;
using UnityEngine;

namespace BeerMP
{
	// Token: 0x02000043 RID: 67
	internal class SceneLoader
	{
		// Token: 0x0600014F RID: 335 RVA: 0x000073B3 File Offset: 0x000055B3
		public static void LoadScene(GameScene scene)
		{
			if (scene == GameScene.Unknown)
			{
				return;
			}
			Application.LoadLevel(scene.ToString());
		}

		// Token: 0x06000150 RID: 336 RVA: 0x000073CC File Offset: 0x000055CC
		public SceneLoader()
		{
		}
	}
}
