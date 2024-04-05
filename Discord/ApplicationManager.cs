using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Discord
{
	// Token: 0x0200002F RID: 47
	public class ApplicationManager
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003301 File Offset: 0x00001501
		private ApplicationManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(ApplicationManager.FFIMethods));
				}
				return (ApplicationManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003334 File Offset: 0x00001534
		internal ApplicationManager(IntPtr ptr, IntPtr eventsPtr, ref ApplicationManager.FFIEvents events)
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

		// Token: 0x06000046 RID: 70 RVA: 0x00003383 File Offset: 0x00001583
		private void InitEvents(IntPtr eventsPtr, ref ApplicationManager.FFIEvents events)
		{
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003398 File Offset: 0x00001598
		[MonoPInvokeCallback]
		private static void ValidateOrExitCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ApplicationManager.ValidateOrExitHandler validateOrExitHandler = (ApplicationManager.ValidateOrExitHandler)gchandle.Target;
			gchandle.Free();
			validateOrExitHandler(result);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000033C8 File Offset: 0x000015C8
		public void ValidateOrExit(ApplicationManager.ValidateOrExitHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.ValidateOrExit(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new ApplicationManager.FFIMethods.ValidateOrExitCallback(ApplicationManager.ValidateOrExitCallbackImpl));
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003404 File Offset: 0x00001604
		public string GetCurrentLocale()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			this.Methods.GetCurrentLocale(this.MethodsPtr, stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000343C File Offset: 0x0000163C
		public string GetCurrentBranch()
		{
			StringBuilder stringBuilder = new StringBuilder(4096);
			this.Methods.GetCurrentBranch(this.MethodsPtr, stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003474 File Offset: 0x00001674
		[MonoPInvokeCallback]
		private static void GetOAuth2TokenCallbackImpl(IntPtr ptr, Result result, ref OAuth2Token oauth2Token)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ApplicationManager.GetOAuth2TokenHandler getOAuth2TokenHandler = (ApplicationManager.GetOAuth2TokenHandler)gchandle.Target;
			gchandle.Free();
			getOAuth2TokenHandler(result, ref oauth2Token);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000034A4 File Offset: 0x000016A4
		public void GetOAuth2Token(ApplicationManager.GetOAuth2TokenHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.GetOAuth2Token(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new ApplicationManager.FFIMethods.GetOAuth2TokenCallback(ApplicationManager.GetOAuth2TokenCallbackImpl));
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000034E0 File Offset: 0x000016E0
		[MonoPInvokeCallback]
		private static void GetTicketCallbackImpl(IntPtr ptr, Result result, ref string data)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ApplicationManager.GetTicketHandler getTicketHandler = (ApplicationManager.GetTicketHandler)gchandle.Target;
			gchandle.Free();
			getTicketHandler(result, ref data);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003510 File Offset: 0x00001710
		public void GetTicket(ApplicationManager.GetTicketHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.GetTicket(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new ApplicationManager.FFIMethods.GetTicketCallback(ApplicationManager.GetTicketCallbackImpl));
		}

		// Token: 0x040000F5 RID: 245
		private IntPtr MethodsPtr;

		// Token: 0x040000F6 RID: 246
		private object MethodsStructure;

		// Token: 0x02000098 RID: 152
		internal struct FFIEvents
		{
		}

		// Token: 0x02000099 RID: 153
		internal struct FFIMethods
		{
			// Token: 0x040003DB RID: 987
			internal ApplicationManager.FFIMethods.ValidateOrExitMethod ValidateOrExit;

			// Token: 0x040003DC RID: 988
			internal ApplicationManager.FFIMethods.GetCurrentLocaleMethod GetCurrentLocale;

			// Token: 0x040003DD RID: 989
			internal ApplicationManager.FFIMethods.GetCurrentBranchMethod GetCurrentBranch;

			// Token: 0x040003DE RID: 990
			internal ApplicationManager.FFIMethods.GetOAuth2TokenMethod GetOAuth2Token;

			// Token: 0x040003DF RID: 991
			internal ApplicationManager.FFIMethods.GetTicketMethod GetTicket;

			// Token: 0x02000187 RID: 391
			// (Invoke) Token: 0x06000701 RID: 1793
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ValidateOrExitCallback(IntPtr ptr, Result result);

			// Token: 0x02000188 RID: 392
			// (Invoke) Token: 0x06000705 RID: 1797
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ValidateOrExitMethod(IntPtr methodsPtr, IntPtr callbackData, ApplicationManager.FFIMethods.ValidateOrExitCallback callback);

			// Token: 0x02000189 RID: 393
			// (Invoke) Token: 0x06000709 RID: 1801
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetCurrentLocaleMethod(IntPtr methodsPtr, StringBuilder locale);

			// Token: 0x0200018A RID: 394
			// (Invoke) Token: 0x0600070D RID: 1805
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetCurrentBranchMethod(IntPtr methodsPtr, StringBuilder branch);

			// Token: 0x0200018B RID: 395
			// (Invoke) Token: 0x06000711 RID: 1809
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetOAuth2TokenCallback(IntPtr ptr, Result result, ref OAuth2Token oauth2Token);

			// Token: 0x0200018C RID: 396
			// (Invoke) Token: 0x06000715 RID: 1813
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetOAuth2TokenMethod(IntPtr methodsPtr, IntPtr callbackData, ApplicationManager.FFIMethods.GetOAuth2TokenCallback callback);

			// Token: 0x0200018D RID: 397
			// (Invoke) Token: 0x06000719 RID: 1817
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetTicketCallback(IntPtr ptr, Result result, [MarshalAs(UnmanagedType.LPStr)] ref string data);

			// Token: 0x0200018E RID: 398
			// (Invoke) Token: 0x0600071D RID: 1821
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetTicketMethod(IntPtr methodsPtr, IntPtr callbackData, ApplicationManager.FFIMethods.GetTicketCallback callback);
		}

		// Token: 0x0200009A RID: 154
		// (Invoke) Token: 0x060003DF RID: 991
		public delegate void ValidateOrExitHandler(Result result);

		// Token: 0x0200009B RID: 155
		// (Invoke) Token: 0x060003E3 RID: 995
		public delegate void GetOAuth2TokenHandler(Result result, ref OAuth2Token oauth2Token);

		// Token: 0x0200009C RID: 156
		// (Invoke) Token: 0x060003E7 RID: 999
		public delegate void GetTicketHandler(Result result, ref string data);
	}
}
