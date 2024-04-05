using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000037 RID: 55
	public class StoreManager
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000ED RID: 237 RVA: 0x000058E8 File Offset: 0x00003AE8
		private StoreManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(StoreManager.FFIMethods));
				}
				return (StoreManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x060000EE RID: 238 RVA: 0x00005918 File Offset: 0x00003B18
		// (remove) Token: 0x060000EF RID: 239 RVA: 0x00005950 File Offset: 0x00003B50
		public event StoreManager.EntitlementCreateHandler OnEntitlementCreate
		{
			[CompilerGenerated]
			add
			{
				StoreManager.EntitlementCreateHandler entitlementCreateHandler = this.OnEntitlementCreate;
				StoreManager.EntitlementCreateHandler entitlementCreateHandler2;
				do
				{
					entitlementCreateHandler2 = entitlementCreateHandler;
					StoreManager.EntitlementCreateHandler entitlementCreateHandler3 = (StoreManager.EntitlementCreateHandler)Delegate.Combine(entitlementCreateHandler2, value);
					entitlementCreateHandler = Interlocked.CompareExchange<StoreManager.EntitlementCreateHandler>(ref this.OnEntitlementCreate, entitlementCreateHandler3, entitlementCreateHandler2);
				}
				while (entitlementCreateHandler != entitlementCreateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				StoreManager.EntitlementCreateHandler entitlementCreateHandler = this.OnEntitlementCreate;
				StoreManager.EntitlementCreateHandler entitlementCreateHandler2;
				do
				{
					entitlementCreateHandler2 = entitlementCreateHandler;
					StoreManager.EntitlementCreateHandler entitlementCreateHandler3 = (StoreManager.EntitlementCreateHandler)Delegate.Remove(entitlementCreateHandler2, value);
					entitlementCreateHandler = Interlocked.CompareExchange<StoreManager.EntitlementCreateHandler>(ref this.OnEntitlementCreate, entitlementCreateHandler3, entitlementCreateHandler2);
				}
				while (entitlementCreateHandler != entitlementCreateHandler2);
			}
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060000F0 RID: 240 RVA: 0x00005988 File Offset: 0x00003B88
		// (remove) Token: 0x060000F1 RID: 241 RVA: 0x000059C0 File Offset: 0x00003BC0
		public event StoreManager.EntitlementDeleteHandler OnEntitlementDelete
		{
			[CompilerGenerated]
			add
			{
				StoreManager.EntitlementDeleteHandler entitlementDeleteHandler = this.OnEntitlementDelete;
				StoreManager.EntitlementDeleteHandler entitlementDeleteHandler2;
				do
				{
					entitlementDeleteHandler2 = entitlementDeleteHandler;
					StoreManager.EntitlementDeleteHandler entitlementDeleteHandler3 = (StoreManager.EntitlementDeleteHandler)Delegate.Combine(entitlementDeleteHandler2, value);
					entitlementDeleteHandler = Interlocked.CompareExchange<StoreManager.EntitlementDeleteHandler>(ref this.OnEntitlementDelete, entitlementDeleteHandler3, entitlementDeleteHandler2);
				}
				while (entitlementDeleteHandler != entitlementDeleteHandler2);
			}
			[CompilerGenerated]
			remove
			{
				StoreManager.EntitlementDeleteHandler entitlementDeleteHandler = this.OnEntitlementDelete;
				StoreManager.EntitlementDeleteHandler entitlementDeleteHandler2;
				do
				{
					entitlementDeleteHandler2 = entitlementDeleteHandler;
					StoreManager.EntitlementDeleteHandler entitlementDeleteHandler3 = (StoreManager.EntitlementDeleteHandler)Delegate.Remove(entitlementDeleteHandler2, value);
					entitlementDeleteHandler = Interlocked.CompareExchange<StoreManager.EntitlementDeleteHandler>(ref this.OnEntitlementDelete, entitlementDeleteHandler3, entitlementDeleteHandler2);
				}
				while (entitlementDeleteHandler != entitlementDeleteHandler2);
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000059F8 File Offset: 0x00003BF8
		internal StoreManager(IntPtr ptr, IntPtr eventsPtr, ref StoreManager.FFIEvents events)
		{
			if (eventsPtr == IntPtr.Zero)
			{
				throw new ResultException(Result.InternalError);
			}
			this.InitEvents(eventsPtr, ref events);
			this.MethodsPtr = ptr;
			if (this.MethodsPtr == IntPtr.Zero)
			{
				throw new ResultException(Result.InternalError);
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00005A47 File Offset: 0x00003C47
		private void InitEvents(IntPtr eventsPtr, ref StoreManager.FFIEvents events)
		{
			events.OnEntitlementCreate = new StoreManager.FFIEvents.EntitlementCreateHandler(StoreManager.OnEntitlementCreateImpl);
			events.OnEntitlementDelete = new StoreManager.FFIEvents.EntitlementDeleteHandler(StoreManager.OnEntitlementDeleteImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00005A80 File Offset: 0x00003C80
		[MonoPInvokeCallback]
		private static void FetchSkusCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			StoreManager.FetchSkusHandler fetchSkusHandler = (StoreManager.FetchSkusHandler)gchandle.Target;
			gchandle.Free();
			fetchSkusHandler(result);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00005AB0 File Offset: 0x00003CB0
		public void FetchSkus(StoreManager.FetchSkusHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.FetchSkus(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new StoreManager.FFIMethods.FetchSkusCallback(StoreManager.FetchSkusCallbackImpl));
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00005AEC File Offset: 0x00003CEC
		public int CountSkus()
		{
			int num = 0;
			this.Methods.CountSkus(this.MethodsPtr, ref num);
			return num;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00005B14 File Offset: 0x00003D14
		public Sku GetSku(long skuId)
		{
			Sku sku = default(Sku);
			Result result = this.Methods.GetSku(this.MethodsPtr, skuId, ref sku);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return sku;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005B50 File Offset: 0x00003D50
		public Sku GetSkuAt(int index)
		{
			Sku sku = default(Sku);
			Result result = this.Methods.GetSkuAt(this.MethodsPtr, index, ref sku);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return sku;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005B8C File Offset: 0x00003D8C
		[MonoPInvokeCallback]
		private static void FetchEntitlementsCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			StoreManager.FetchEntitlementsHandler fetchEntitlementsHandler = (StoreManager.FetchEntitlementsHandler)gchandle.Target;
			gchandle.Free();
			fetchEntitlementsHandler(result);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00005BBC File Offset: 0x00003DBC
		public void FetchEntitlements(StoreManager.FetchEntitlementsHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.FetchEntitlements(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new StoreManager.FFIMethods.FetchEntitlementsCallback(StoreManager.FetchEntitlementsCallbackImpl));
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00005BF8 File Offset: 0x00003DF8
		public int CountEntitlements()
		{
			int num = 0;
			this.Methods.CountEntitlements(this.MethodsPtr, ref num);
			return num;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00005C20 File Offset: 0x00003E20
		public Entitlement GetEntitlement(long entitlementId)
		{
			Entitlement entitlement = default(Entitlement);
			Result result = this.Methods.GetEntitlement(this.MethodsPtr, entitlementId, ref entitlement);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return entitlement;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00005C5C File Offset: 0x00003E5C
		public Entitlement GetEntitlementAt(int index)
		{
			Entitlement entitlement = default(Entitlement);
			Result result = this.Methods.GetEntitlementAt(this.MethodsPtr, index, ref entitlement);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return entitlement;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00005C98 File Offset: 0x00003E98
		public bool HasSkuEntitlement(long skuId)
		{
			bool flag = false;
			Result result = this.Methods.HasSkuEntitlement(this.MethodsPtr, skuId, ref flag);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return flag;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00005CCC File Offset: 0x00003ECC
		[MonoPInvokeCallback]
		private static void StartPurchaseCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			StoreManager.StartPurchaseHandler startPurchaseHandler = (StoreManager.StartPurchaseHandler)gchandle.Target;
			gchandle.Free();
			startPurchaseHandler(result);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00005CFC File Offset: 0x00003EFC
		public void StartPurchase(long skuId, StoreManager.StartPurchaseHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.StartPurchase(this.MethodsPtr, skuId, GCHandle.ToIntPtr(gchandle), new StoreManager.FFIMethods.StartPurchaseCallback(StoreManager.StartPurchaseCallbackImpl));
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00005D3C File Offset: 0x00003F3C
		[MonoPInvokeCallback]
		private static void OnEntitlementCreateImpl(IntPtr ptr, ref Entitlement entitlement)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.StoreManagerInstance.OnEntitlementCreate != null)
			{
				discord.StoreManagerInstance.OnEntitlementCreate(ref entitlement);
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00005D7C File Offset: 0x00003F7C
		[MonoPInvokeCallback]
		private static void OnEntitlementDeleteImpl(IntPtr ptr, ref Entitlement entitlement)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.StoreManagerInstance.OnEntitlementDelete != null)
			{
				discord.StoreManagerInstance.OnEntitlementDelete(ref entitlement);
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00005DBC File Offset: 0x00003FBC
		public IEnumerable<Entitlement> GetEntitlements()
		{
			int num = this.CountEntitlements();
			List<Entitlement> list = new List<Entitlement>();
			for (int i = 0; i < num; i++)
			{
				list.Add(this.GetEntitlementAt(i));
			}
			return list;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00005DF0 File Offset: 0x00003FF0
		public IEnumerable<Sku> GetSkus()
		{
			int num = this.CountSkus();
			List<Sku> list = new List<Sku>();
			for (int i = 0; i < num; i++)
			{
				list.Add(this.GetSkuAt(i));
			}
			return list;
		}

		// Token: 0x04000113 RID: 275
		private IntPtr MethodsPtr;

		// Token: 0x04000114 RID: 276
		private object MethodsStructure;

		// Token: 0x04000115 RID: 277
		[CompilerGenerated]
		private StoreManager.EntitlementCreateHandler OnEntitlementCreate;

		// Token: 0x04000116 RID: 278
		[CompilerGenerated]
		private StoreManager.EntitlementDeleteHandler OnEntitlementDelete;

		// Token: 0x020000CE RID: 206
		internal struct FFIEvents
		{
			// Token: 0x04000433 RID: 1075
			internal StoreManager.FFIEvents.EntitlementCreateHandler OnEntitlementCreate;

			// Token: 0x04000434 RID: 1076
			internal StoreManager.FFIEvents.EntitlementDeleteHandler OnEntitlementDelete;

			// Token: 0x020001F7 RID: 503
			// (Invoke) Token: 0x060008C1 RID: 2241
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void EntitlementCreateHandler(IntPtr ptr, ref Entitlement entitlement);

			// Token: 0x020001F8 RID: 504
			// (Invoke) Token: 0x060008C5 RID: 2245
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void EntitlementDeleteHandler(IntPtr ptr, ref Entitlement entitlement);
		}

		// Token: 0x020000CF RID: 207
		internal struct FFIMethods
		{
			// Token: 0x04000435 RID: 1077
			internal StoreManager.FFIMethods.FetchSkusMethod FetchSkus;

			// Token: 0x04000436 RID: 1078
			internal StoreManager.FFIMethods.CountSkusMethod CountSkus;

			// Token: 0x04000437 RID: 1079
			internal StoreManager.FFIMethods.GetSkuMethod GetSku;

			// Token: 0x04000438 RID: 1080
			internal StoreManager.FFIMethods.GetSkuAtMethod GetSkuAt;

			// Token: 0x04000439 RID: 1081
			internal StoreManager.FFIMethods.FetchEntitlementsMethod FetchEntitlements;

			// Token: 0x0400043A RID: 1082
			internal StoreManager.FFIMethods.CountEntitlementsMethod CountEntitlements;

			// Token: 0x0400043B RID: 1083
			internal StoreManager.FFIMethods.GetEntitlementMethod GetEntitlement;

			// Token: 0x0400043C RID: 1084
			internal StoreManager.FFIMethods.GetEntitlementAtMethod GetEntitlementAt;

			// Token: 0x0400043D RID: 1085
			internal StoreManager.FFIMethods.HasSkuEntitlementMethod HasSkuEntitlement;

			// Token: 0x0400043E RID: 1086
			internal StoreManager.FFIMethods.StartPurchaseMethod StartPurchase;

			// Token: 0x020001F9 RID: 505
			// (Invoke) Token: 0x060008C9 RID: 2249
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchSkusCallback(IntPtr ptr, Result result);

			// Token: 0x020001FA RID: 506
			// (Invoke) Token: 0x060008CD RID: 2253
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchSkusMethod(IntPtr methodsPtr, IntPtr callbackData, StoreManager.FFIMethods.FetchSkusCallback callback);

			// Token: 0x020001FB RID: 507
			// (Invoke) Token: 0x060008D1 RID: 2257
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CountSkusMethod(IntPtr methodsPtr, ref int count);

			// Token: 0x020001FC RID: 508
			// (Invoke) Token: 0x060008D5 RID: 2261
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetSkuMethod(IntPtr methodsPtr, long skuId, ref Sku sku);

			// Token: 0x020001FD RID: 509
			// (Invoke) Token: 0x060008D9 RID: 2265
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetSkuAtMethod(IntPtr methodsPtr, int index, ref Sku sku);

			// Token: 0x020001FE RID: 510
			// (Invoke) Token: 0x060008DD RID: 2269
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchEntitlementsCallback(IntPtr ptr, Result result);

			// Token: 0x020001FF RID: 511
			// (Invoke) Token: 0x060008E1 RID: 2273
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchEntitlementsMethod(IntPtr methodsPtr, IntPtr callbackData, StoreManager.FFIMethods.FetchEntitlementsCallback callback);

			// Token: 0x02000200 RID: 512
			// (Invoke) Token: 0x060008E5 RID: 2277
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CountEntitlementsMethod(IntPtr methodsPtr, ref int count);

			// Token: 0x02000201 RID: 513
			// (Invoke) Token: 0x060008E9 RID: 2281
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetEntitlementMethod(IntPtr methodsPtr, long entitlementId, ref Entitlement entitlement);

			// Token: 0x02000202 RID: 514
			// (Invoke) Token: 0x060008ED RID: 2285
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetEntitlementAtMethod(IntPtr methodsPtr, int index, ref Entitlement entitlement);

			// Token: 0x02000203 RID: 515
			// (Invoke) Token: 0x060008F1 RID: 2289
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result HasSkuEntitlementMethod(IntPtr methodsPtr, long skuId, ref bool hasEntitlement);

			// Token: 0x02000204 RID: 516
			// (Invoke) Token: 0x060008F5 RID: 2293
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void StartPurchaseCallback(IntPtr ptr, Result result);

			// Token: 0x02000205 RID: 517
			// (Invoke) Token: 0x060008F9 RID: 2297
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void StartPurchaseMethod(IntPtr methodsPtr, long skuId, IntPtr callbackData, StoreManager.FFIMethods.StartPurchaseCallback callback);
		}

		// Token: 0x020000D0 RID: 208
		// (Invoke) Token: 0x06000477 RID: 1143
		public delegate void FetchSkusHandler(Result result);

		// Token: 0x020000D1 RID: 209
		// (Invoke) Token: 0x0600047B RID: 1147
		public delegate void FetchEntitlementsHandler(Result result);

		// Token: 0x020000D2 RID: 210
		// (Invoke) Token: 0x0600047F RID: 1151
		public delegate void StartPurchaseHandler(Result result);

		// Token: 0x020000D3 RID: 211
		// (Invoke) Token: 0x06000483 RID: 1155
		public delegate void EntitlementCreateHandler(ref Entitlement entitlement);

		// Token: 0x020000D4 RID: 212
		// (Invoke) Token: 0x06000487 RID: 1159
		public delegate void EntitlementDeleteHandler(ref Entitlement entitlement);
	}
}
