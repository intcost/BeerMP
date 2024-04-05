using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;

namespace BeerMP.Networking.PlayerManagers
{
	// Token: 0x02000057 RID: 87
	internal class ProximityVoiceChat : MonoBehaviour
	{
		// Token: 0x06000207 RID: 519 RVA: 0x0000BF6C File Offset: 0x0000A16C
		private void Start()
		{
			if (this.net == null)
			{
				ProximityVoiceChat.LoadSettings();
				return;
			}
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.loop = false;
			this.audioSource.volume = 1f;
			this.audioSource.rolloffMode = AudioRolloffMode.Linear;
			this.audioSource.maxDistance = 15f;
			this.audioSource.spatialBlend = 0.7f;
			this.audioSource.playOnAwake = false;
			this.ReceiveVoice = NetEvent<ProximityVoiceChat>.Register(string.Format("ProximityVoiceUpdate{0}", this.net.steamID.m_SteamID), new NetEventHandler(this.ReceiveVoiceUpdate));
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000C028 File Offset: 0x0000A228
		private static void SaveSettings()
		{
			List<byte> list = new List<byte>();
			list.Add(ProximityVoiceChat.allowVC ? byte.MaxValue : 0);
			list.Add(ProximityVoiceChat.push_to_talk ? byte.MaxValue : 0);
			list.AddRange(BitConverter.GetBytes((int)ProximityVoiceChat.push_to_talk_key));
			list.AddRange(BitConverter.GetBytes(ProximityVoiceChat.masterVolume));
			File.WriteAllBytes(ProximityVoiceChat.pvcDataPath, list.ToArray());
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000C098 File Offset: 0x0000A298
		private static void LoadSettings()
		{
			if (!File.Exists(ProximityVoiceChat.pvcDataPath))
			{
				return;
			}
			byte[] array = File.ReadAllBytes(ProximityVoiceChat.pvcDataPath);
			ProximityVoiceChat.allowVC = array[0] > 0;
			ProximityVoiceChat.push_to_talk = array[1] > 0;
			ProximityVoiceChat.push_to_talk_key = (KeyCode)BitConverter.ToInt32(array, 2);
			ProximityVoiceChat.masterVolume = BitConverter.ToSingle(array, 6);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000C0E9 File Offset: 0x0000A2E9
		public static void SetEnabled(bool enable)
		{
			ProximityVoiceChat.allowVC = enable;
			Console.Log("PVC " + (enable ? "enabled" : "disabled"), true);
			ProximityVoiceChat.SaveSettings();
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000C115 File Offset: 0x0000A315
		public static void SetPushToTalk(bool enable)
		{
			ProximityVoiceChat.push_to_talk = enable;
			Console.Log("PVC: Push to Talk " + (enable ? "enabled" : "disabled"), true);
			ProximityVoiceChat.SaveSettings();
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000C141 File Offset: 0x0000A341
		public static void ChangePTT_Keybing()
		{
			ProximityVoiceChat.changeKey = true;
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000C149 File Offset: 0x0000A349
		public static void SetMasterVolume(float volume)
		{
			volume = Mathf.Clamp(volume, 0f, 10f);
			ProximityVoiceChat.masterVolume = volume;
			Console.Log(string.Format("PVC: Master Volume set to {0}", volume), true);
			ProximityVoiceChat.SaveSettings();
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000C17E File Offset: 0x0000A37E
		private void OnApplicationQuit()
		{
			if (ProximityVoiceChat.isRecording)
			{
				ProximityVoiceChat.isRecording = false;
				SteamUser.StopVoiceRecording();
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000C194 File Offset: 0x0000A394
		private void Update()
		{
			if (this.net != null)
			{
				return;
			}
			if (!ProximityVoiceChat.allowVC)
			{
				return;
			}
			if (ProximityVoiceChat.changeKey)
			{
				foreach (object obj in Enum.GetValues(typeof(KeyCode)))
				{
					if ((KeyCode)obj != KeyCode.Return && (KeyCode)obj != KeyCode.KeypadEnter && Input.GetKey((KeyCode)obj))
					{
						ProximityVoiceChat.push_to_talk_key = (KeyCode)obj;
						ProximityVoiceChat.changeKey = false;
						Console.Log(string.Format("PVC: changed Push to Talk key to {0}", ProximityVoiceChat.push_to_talk_key), true);
						ProximityVoiceChat.SaveSettings();
					}
				}
			}
			uint num;
			if (SteamNetworking.IsP2PPacketAvailable(out num, 1))
			{
				byte[] array = new byte[num];
				uint num2 = 0U;
				CSteamID csteamID;
				if (SteamNetworking.ReadP2PPacket(array, num, out num2, out csteamID, 0))
				{
					using (Packet packet = new Packet(array))
					{
						if (csteamID != SteamUser.GetSteamID())
						{
							this.ReceiveVoiceUpdate(csteamID.m_SteamID, packet);
						}
					}
				}
			}
			if (ProximityVoiceChat.push_to_talk)
			{
				if (Input.GetKey(ProximityVoiceChat.push_to_talk_key))
				{
					if (!ProximityVoiceChat.isRecording)
					{
						ProximityVoiceChat.isRecording = true;
						SteamUser.StartVoiceRecording();
					}
				}
				else if (ProximityVoiceChat.isRecording)
				{
					ProximityVoiceChat.isRecording = false;
					SteamUser.StopVoiceRecording();
				}
			}
			else if (!ProximityVoiceChat.isRecording)
			{
				ProximityVoiceChat.isRecording = true;
				SteamUser.StartVoiceRecording();
			}
			uint num3;
			uint num4;
			if (SteamUser.GetAvailableVoice(out num3, out num4, 0U) == EVoiceResult.k_EVoiceResultOK && num3 > 1024U)
			{
				Debug.Log(num3);
				byte[] array2 = new byte[1024];
				uint num5;
				uint num6;
				if (SteamUser.GetVoice(true, array2, 1024U, out num5, false, new byte[0], 0U, out num6, 0U) == EVoiceResult.k_EVoiceResultOK && num5 > 0U)
				{
					using (Packet packet2 = new Packet(1))
					{
						packet2.Write((long)((ulong)num5), -1);
						packet2.Write(array2, -1);
						NetEvent<ProximityVoiceChat>.Send(string.Format("ProximityVoiceUpdate{0}", BeerMPGlobals.UserID), packet2, false);
					}
				}
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000C3BC File Offset: 0x0000A5BC
		private void ReceiveVoiceUpdate(ulong sender, Packet packet)
		{
			uint num = (uint)packet.ReadLong(true);
			byte[] array = packet.ReadBytes((int)num, true);
			uint voiceOptimalSampleRate = SteamUser.GetVoiceOptimalSampleRate();
			byte[] array2 = new byte[voiceOptimalSampleRate * 2U];
			uint num2;
			if (SteamUser.DecompressVoice(array, num, array2, (uint)array2.Length, out num2, voiceOptimalSampleRate) == EVoiceResult.k_EVoiceResultOK && num2 > 0U)
			{
				this.audioSource.clip = AudioClip.Create(Guid.NewGuid().ToString(), (int)voiceOptimalSampleRate, 1, (int)voiceOptimalSampleRate, false);
				float[] array3 = new float[voiceOptimalSampleRate];
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i] = (float)((short)((int)array2[i * 2] | ((int)array2[i * 2 + 1] << 8))) / 32768f;
					array3[i] *= 2f;
					array3[i] *= ProximityVoiceChat.masterVolume;
				}
				this.audioSource.clip.SetData(array3, 0);
				this.audioSource.Play();
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000C4A6 File Offset: 0x0000A6A6
		public ProximityVoiceChat()
		{
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000C4AE File Offset: 0x0000A6AE
		// Note: this type is marked as 'beforefieldinit'.
		static ProximityVoiceChat()
		{
		}

		// Token: 0x040001E9 RID: 489
		internal NetPlayer net;

		// Token: 0x040001EA RID: 490
		public static bool allowVC = false;

		// Token: 0x040001EB RID: 491
		public static bool push_to_talk = false;

		// Token: 0x040001EC RID: 492
		public static KeyCode push_to_talk_key = KeyCode.None;

		// Token: 0x040001ED RID: 493
		internal static bool isRecording = false;

		// Token: 0x040001EE RID: 494
		internal static bool changeKey = false;

		// Token: 0x040001EF RID: 495
		public static float masterVolume = 1f;

		// Token: 0x040001F0 RID: 496
		internal static string pvcDataPath = Path.Combine(Application.persistentDataPath, "BeerMP_proximityvoicechat");

		// Token: 0x040001F1 RID: 497
		private NetEvent<ProximityVoiceChat> ReceiveVoice;

		// Token: 0x040001F2 RID: 498
		private AudioSource audioSource;
	}
}
