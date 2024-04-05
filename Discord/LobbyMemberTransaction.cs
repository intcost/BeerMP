using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x0200002A RID: 42
	public struct LobbyMemberTransaction
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002895 File Offset: 0x00000A95
		private LobbyMemberTransaction.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(LobbyMemberTransaction.FFIMethods));
				}
				return (LobbyMemberTransaction.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000028C8 File Offset: 0x00000AC8
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

		// Token: 0x06000028 RID: 40 RVA: 0x0000290C File Offset: 0x00000B0C
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

		// Token: 0x040000C6 RID: 198
		internal IntPtr MethodsPtr;

		// Token: 0x040000C7 RID: 199
		internal object MethodsStructure;

		// Token: 0x02000092 RID: 146
		internal struct FFIMethods
		{
			// Token: 0x040003AA RID: 938
			internal LobbyMemberTransaction.FFIMethods.SetMetadataMethod SetMetadata;

			// Token: 0x040003AB RID: 939
			internal LobbyMemberTransaction.FFIMethods.DeleteMetadataMethod DeleteMetadata;

			// Token: 0x02000171 RID: 369
			// (Invoke) Token: 0x060006A9 RID: 1705
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetMetadataMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);

			// Token: 0x02000172 RID: 370
			// (Invoke) Token: 0x060006AD RID: 1709
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result DeleteMetadataMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string key);
		}
	}
}
