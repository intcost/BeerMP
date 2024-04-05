using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000029 RID: 41
	public struct LobbyTransaction
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000026CE File Offset: 0x000008CE
		private LobbyTransaction.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(LobbyTransaction.FFIMethods));
				}
				return (LobbyTransaction.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002700 File Offset: 0x00000900
		public void SetType(LobbyType type)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.SetType(this.MethodsPtr, type);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002744 File Offset: 0x00000944
		public void SetOwner(long ownerId)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.SetOwner(this.MethodsPtr, ownerId);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002788 File Offset: 0x00000988
		public void SetCapacity(uint capacity)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.SetCapacity(this.MethodsPtr, capacity);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000027CC File Offset: 0x000009CC
		public void SetMetadata(string key, string value)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.SetMetadata(this.MethodsPtr, key, value);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002810 File Offset: 0x00000A10
		public void DeleteMetadata(string key)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.DeleteMetadata(this.MethodsPtr, key);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002854 File Offset: 0x00000A54
		public void SetLocked(bool locked)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.SetLocked(this.MethodsPtr, locked);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x040000C4 RID: 196
		internal IntPtr MethodsPtr;

		// Token: 0x040000C5 RID: 197
		internal object MethodsStructure;

		// Token: 0x02000091 RID: 145
		internal struct FFIMethods
		{
			// Token: 0x040003A4 RID: 932
			internal LobbyTransaction.FFIMethods.SetTypeMethod SetType;

			// Token: 0x040003A5 RID: 933
			internal LobbyTransaction.FFIMethods.SetOwnerMethod SetOwner;

			// Token: 0x040003A6 RID: 934
			internal LobbyTransaction.FFIMethods.SetCapacityMethod SetCapacity;

			// Token: 0x040003A7 RID: 935
			internal LobbyTransaction.FFIMethods.SetMetadataMethod SetMetadata;

			// Token: 0x040003A8 RID: 936
			internal LobbyTransaction.FFIMethods.DeleteMetadataMethod DeleteMetadata;

			// Token: 0x040003A9 RID: 937
			internal LobbyTransaction.FFIMethods.SetLockedMethod SetLocked;

			// Token: 0x0200016B RID: 363
			// (Invoke) Token: 0x06000691 RID: 1681
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetTypeMethod(IntPtr methodsPtr, LobbyType type);

			// Token: 0x0200016C RID: 364
			// (Invoke) Token: 0x06000695 RID: 1685
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetOwnerMethod(IntPtr methodsPtr, long ownerId);

			// Token: 0x0200016D RID: 365
			// (Invoke) Token: 0x06000699 RID: 1689
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetCapacityMethod(IntPtr methodsPtr, uint capacity);

			// Token: 0x0200016E RID: 366
			// (Invoke) Token: 0x0600069D RID: 1693
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetMetadataMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);

			// Token: 0x0200016F RID: 367
			// (Invoke) Token: 0x060006A1 RID: 1697
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result DeleteMetadataMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string key);

			// Token: 0x02000170 RID: 368
			// (Invoke) Token: 0x060006A5 RID: 1701
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetLockedMethod(IntPtr methodsPtr, bool locked);
		}
	}
}
