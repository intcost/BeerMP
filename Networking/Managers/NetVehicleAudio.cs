using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x0200006C RID: 108
	internal class NetVehicleAudio
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000308 RID: 776 RVA: 0x00016347 File Offset: 0x00014547
		// (set) Token: 0x06000309 RID: 777 RVA: 0x0001634F File Offset: 0x0001454F
		public bool IsDrivenBySoundController
		{
			[CompilerGenerated]
			get
			{
				return this.<IsDrivenBySoundController>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<IsDrivenBySoundController>k__BackingField = value;
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00016358 File Offset: 0x00014558
		public NetVehicleAudio(Transform parent, SoundController ctrl)
		{
			this.ctor(parent, ctrl);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00016374 File Offset: 0x00014574
		private void ctor(Transform parent, SoundController ctrl)
		{
			this.controller = ctrl;
			if (this.controller == null)
			{
				Console.LogError("Init " + parent.name + " SoundCOntroller is null", false);
				return;
			}
			if (!parent.gameObject.activeInHierarchy)
			{
				NetVehicleAudio.SoundControllerStart.Invoke(this.controller, null);
			}
			for (int i = 0; i < parent.childCount; i++)
			{
				GameObject gameObject = parent.GetChild(i).gameObject;
				if (gameObject.name == "audio")
				{
					AudioSource component = gameObject.GetComponent<AudioSource>();
					if (component != null)
					{
						this.sources.Add(new NetVehicleAudio.WatchedAudioSource(component));
					}
				}
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00016422 File Offset: 0x00014622
		public void Update()
		{
			this.controller.enabled = this.IsDrivenBySoundController;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00016438 File Offset: 0x00014638
		public bool WriteUpdate(Packet p, int vehicleHash, bool initSync = false)
		{
			if (p == null)
			{
				return false;
			}
			bool flag = initSync;
			int num = 0;
			while (num < this.sources.Count && !flag)
			{
				if (this.sources[num].HasUpdate)
				{
					flag = true;
				}
				num++;
			}
			if (!flag)
			{
				return false;
			}
			p.Write(7, -1);
			p.Write(vehicleHash, -1);
			for (int i = 0; i < this.sources.Count; i++)
			{
				this.sources[i].WriteUpdates(p, i, initSync);
			}
			p.Write(247, -1);
			return true;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x000164C6 File Offset: 0x000146C6
		// Note: this type is marked as 'beforefieldinit'.
		static NetVehicleAudio()
		{
		}

		// Token: 0x04000313 RID: 787
		public SoundController controller;

		// Token: 0x04000314 RID: 788
		public List<NetVehicleAudio.WatchedAudioSource> sources = new List<NetVehicleAudio.WatchedAudioSource>();

		// Token: 0x04000315 RID: 789
		[CompilerGenerated]
		private bool <IsDrivenBySoundController>k__BackingField;

		// Token: 0x04000316 RID: 790
		private static MethodInfo SoundControllerStart = typeof(SoundController).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x02000128 RID: 296
		public class WatchedAudioSource
		{
			// Token: 0x1700003F RID: 63
			// (get) Token: 0x060005BE RID: 1470 RVA: 0x0001FBDC File Offset: 0x0001DDDC
			public bool HasUpdate
			{
				get
				{
					return this.lastPlaying != this.src.isPlaying || this.lastVolume != this.src.volume || this.lastPitch != this.src.pitch;
				}
			}

			// Token: 0x060005BF RID: 1471 RVA: 0x0001FC1C File Offset: 0x0001DE1C
			public WatchedAudioSource(AudioSource src)
			{
				this.src = src;
				this.lastPlaying = src.isPlaying;
				this.lastVolume = src.volume;
				this.lastPitch = src.pitch;
			}

			// Token: 0x060005C0 RID: 1472 RVA: 0x0001FC70 File Offset: 0x0001DE70
			public void WriteUpdates(Packet p, int srcIndex, bool initSync = false)
			{
				if (!this.HasUpdate && !initSync)
				{
					return;
				}
				p.Write(15, -1);
				p.Write(srcIndex, -1);
				if (this.lastPlaying != this.src.isPlaying || initSync)
				{
					p.Write(31, -1);
					p.Write(this.src.isPlaying, -1);
					if (this.src.isPlaying)
					{
						p.Write(this.src.time, -1);
					}
					this.lastPlaying = this.src.isPlaying;
				}
				if (this.lastVolume != this.src.volume || initSync)
				{
					p.Write(47, -1);
					p.Write(this.src.volume, -1);
					this.lastVolume = this.src.volume;
				}
				if (this.lastPitch != this.src.pitch || initSync)
				{
					p.Write(63, -1);
					p.Write(this.src.pitch, -1);
					this.lastPitch = this.src.pitch;
				}
				p.Write(byte.MaxValue, -1);
			}

			// Token: 0x060005C1 RID: 1473 RVA: 0x0001FD98 File Offset: 0x0001DF98
			public void OnUpdate(bool? isPlaying, float? time, float? volume, float? pitch)
			{
				if (isPlaying != null)
				{
					if (isPlaying.Value)
					{
						this.src.Play();
						if (time != null)
						{
							this.src.time = time.Value;
						}
					}
					else
					{
						this.src.Stop();
					}
				}
				if (volume != null)
				{
					this.src.volume = volume.Value;
				}
				if (pitch != null)
				{
					this.src.pitch = pitch.Value;
				}
			}

			// Token: 0x04000567 RID: 1383
			private AudioSource src;

			// Token: 0x04000568 RID: 1384
			private bool lastPlaying;

			// Token: 0x04000569 RID: 1385
			private float lastVolume = 1f;

			// Token: 0x0400056A RID: 1386
			private float lastPitch = 1f;
		}
	}
}
