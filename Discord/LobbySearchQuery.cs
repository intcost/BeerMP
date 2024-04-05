using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x0200002B RID: 43
	public struct LobbySearchQuery
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000294D File Offset: 0x00000B4D
		private LobbySearchQuery.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(LobbySearchQuery.FFIMethods));
				}
				return (LobbySearchQuery.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002980 File Offset: 0x00000B80
		public void Filter(string key, LobbySearchComparison comparison, LobbySearchCast cast, string value)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.Filter(this.MethodsPtr, key, comparison, cast, value);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000029C8 File Offset: 0x00000BC8
		public void Sort(string key, LobbySearchCast cast, string value)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.Sort(this.MethodsPtr, key, cast, value);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002A0C File Offset: 0x00000C0C
		public void Limit(uint limit)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.Limit(this.MethodsPtr, limit);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002A50 File Offset: 0x00000C50
		public void Distance(LobbySearchDistance distance)
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				Result result = this.Methods.Distance(this.MethodsPtr, distance);
				if (result != Result.Ok)
				{
					throw new ResultException(result);
				}
			}
		}

		// Token: 0x040000C8 RID: 200
		internal IntPtr MethodsPtr;

		// Token: 0x040000C9 RID: 201
		internal object MethodsStructure;

		// Token: 0x02000093 RID: 147
		internal struct FFIMethods
		{
			// Token: 0x040003AC RID: 940
			internal LobbySearchQuery.FFIMethods.FilterMethod Filter;

			// Token: 0x040003AD RID: 941
			internal LobbySearchQuery.FFIMethods.SortMethod Sort;

			// Token: 0x040003AE RID: 942
			internal LobbySearchQuery.FFIMethods.LimitMethod Limit;

			// Token: 0x040003AF RID: 943
			internal LobbySearchQuery.FFIMethods.DistanceMethod Distance;

			// Token: 0x02000173 RID: 371
			// (Invoke) Token: 0x060006B1 RID: 1713
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result FilterMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string key, LobbySearchComparison comparison, LobbySearchCast cast, [MarshalAs(UnmanagedType.LPStr)] string value);

			// Token: 0x02000174 RID: 372
			// (Invoke) Token: 0x060006B5 RID: 1717
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SortMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string key, LobbySearchCast cast, [MarshalAs(UnmanagedType.LPStr)] string value);

			// Token: 0x02000175 RID: 373
			// (Invoke) Token: 0x060006B9 RID: 1721
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result LimitMethod(IntPtr methodsPtr, uint limit);

			// Token: 0x02000176 RID: 374
			// (Invoke) Token: 0x060006BD RID: 1725
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result DistanceMethod(IntPtr methodsPtr, LobbySearchDistance distance);
		}
	}
}
