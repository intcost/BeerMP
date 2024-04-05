using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000030 RID: 48
	public class UserManager
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004F RID: 79 RVA: 0x0000354C File Offset: 0x0000174C
		private UserManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(UserManager.FFIMethods));
				}
				return (UserManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000050 RID: 80 RVA: 0x0000357C File Offset: 0x0000177C
		// (remove) Token: 0x06000051 RID: 81 RVA: 0x000035B4 File Offset: 0x000017B4
		public event UserManager.CurrentUserUpdateHandler OnCurrentUserUpdate
		{
			[CompilerGenerated]
			add
			{
				UserManager.CurrentUserUpdateHandler currentUserUpdateHandler = this.OnCurrentUserUpdate;
				UserManager.CurrentUserUpdateHandler currentUserUpdateHandler2;
				do
				{
					currentUserUpdateHandler2 = currentUserUpdateHandler;
					UserManager.CurrentUserUpdateHandler currentUserUpdateHandler3 = (UserManager.CurrentUserUpdateHandler)Delegate.Combine(currentUserUpdateHandler2, value);
					currentUserUpdateHandler = Interlocked.CompareExchange<UserManager.CurrentUserUpdateHandler>(ref this.OnCurrentUserUpdate, currentUserUpdateHandler3, currentUserUpdateHandler2);
				}
				while (currentUserUpdateHandler != currentUserUpdateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				UserManager.CurrentUserUpdateHandler currentUserUpdateHandler = this.OnCurrentUserUpdate;
				UserManager.CurrentUserUpdateHandler currentUserUpdateHandler2;
				do
				{
					currentUserUpdateHandler2 = currentUserUpdateHandler;
					UserManager.CurrentUserUpdateHandler currentUserUpdateHandler3 = (UserManager.CurrentUserUpdateHandler)Delegate.Remove(currentUserUpdateHandler2, value);
					currentUserUpdateHandler = Interlocked.CompareExchange<UserManager.CurrentUserUpdateHandler>(ref this.OnCurrentUserUpdate, currentUserUpdateHandler3, currentUserUpdateHandler2);
				}
				while (currentUserUpdateHandler != currentUserUpdateHandler2);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000035EC File Offset: 0x000017EC
		internal UserManager(IntPtr ptr, IntPtr eventsPtr, ref UserManager.FFIEvents events)
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

		// Token: 0x06000053 RID: 83 RVA: 0x0000363B File Offset: 0x0000183B
		private void InitEvents(IntPtr eventsPtr, ref UserManager.FFIEvents events)
		{
			events.OnCurrentUserUpdate = new UserManager.FFIEvents.CurrentUserUpdateHandler(UserManager.OnCurrentUserUpdateImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003664 File Offset: 0x00001864
		public User GetCurrentUser()
		{
			User user = default(User);
			Result result = this.Methods.GetCurrentUser(this.MethodsPtr, ref user);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return user;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000036A0 File Offset: 0x000018A0
		[MonoPInvokeCallback]
		private static void GetUserCallbackImpl(IntPtr ptr, Result result, ref User user)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			UserManager.GetUserHandler getUserHandler = (UserManager.GetUserHandler)gchandle.Target;
			gchandle.Free();
			getUserHandler(result, ref user);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000036D0 File Offset: 0x000018D0
		public void GetUser(long userId, UserManager.GetUserHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.GetUser(this.MethodsPtr, userId, GCHandle.ToIntPtr(gchandle), new UserManager.FFIMethods.GetUserCallback(UserManager.GetUserCallbackImpl));
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003710 File Offset: 0x00001910
		public PremiumType GetCurrentUserPremiumType()
		{
			PremiumType premiumType = PremiumType.None;
			Result result = this.Methods.GetCurrentUserPremiumType(this.MethodsPtr, ref premiumType);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return premiumType;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003744 File Offset: 0x00001944
		public bool CurrentUserHasFlag(UserFlag flag)
		{
			bool flag2 = false;
			Result result = this.Methods.CurrentUserHasFlag(this.MethodsPtr, flag, ref flag2);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return flag2;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003778 File Offset: 0x00001978
		[MonoPInvokeCallback]
		private static void OnCurrentUserUpdateImpl(IntPtr ptr)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.UserManagerInstance.OnCurrentUserUpdate != null)
			{
				discord.UserManagerInstance.OnCurrentUserUpdate();
			}
		}

		// Token: 0x040000F7 RID: 247
		private IntPtr MethodsPtr;

		// Token: 0x040000F8 RID: 248
		private object MethodsStructure;

		// Token: 0x040000F9 RID: 249
		[CompilerGenerated]
		private UserManager.CurrentUserUpdateHandler OnCurrentUserUpdate;

		// Token: 0x0200009D RID: 157
		internal struct FFIEvents
		{
			// Token: 0x040003E0 RID: 992
			internal UserManager.FFIEvents.CurrentUserUpdateHandler OnCurrentUserUpdate;

			// Token: 0x0200018F RID: 399
			// (Invoke) Token: 0x06000721 RID: 1825
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CurrentUserUpdateHandler(IntPtr ptr);
		}

		// Token: 0x0200009E RID: 158
		internal struct FFIMethods
		{
			// Token: 0x040003E1 RID: 993
			internal UserManager.FFIMethods.GetCurrentUserMethod GetCurrentUser;

			// Token: 0x040003E2 RID: 994
			internal UserManager.FFIMethods.GetUserMethod GetUser;

			// Token: 0x040003E3 RID: 995
			internal UserManager.FFIMethods.GetCurrentUserPremiumTypeMethod GetCurrentUserPremiumType;

			// Token: 0x040003E4 RID: 996
			internal UserManager.FFIMethods.CurrentUserHasFlagMethod CurrentUserHasFlag;

			// Token: 0x02000190 RID: 400
			// (Invoke) Token: 0x06000725 RID: 1829
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetCurrentUserMethod(IntPtr methodsPtr, ref User currentUser);

			// Token: 0x02000191 RID: 401
			// (Invoke) Token: 0x06000729 RID: 1833
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetUserCallback(IntPtr ptr, Result result, ref User user);

			// Token: 0x02000192 RID: 402
			// (Invoke) Token: 0x0600072D RID: 1837
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetUserMethod(IntPtr methodsPtr, long userId, IntPtr callbackData, UserManager.FFIMethods.GetUserCallback callback);

			// Token: 0x02000193 RID: 403
			// (Invoke) Token: 0x06000731 RID: 1841
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetCurrentUserPremiumTypeMethod(IntPtr methodsPtr, ref PremiumType premiumType);

			// Token: 0x02000194 RID: 404
			// (Invoke) Token: 0x06000735 RID: 1845
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result CurrentUserHasFlagMethod(IntPtr methodsPtr, UserFlag flag, ref bool hasFlag);
		}

		// Token: 0x0200009F RID: 159
		// (Invoke) Token: 0x060003EB RID: 1003
		public delegate void GetUserHandler(Result result, ref User user);

		// Token: 0x020000A0 RID: 160
		// (Invoke) Token: 0x060003EF RID: 1007
		public delegate void CurrentUserUpdateHandler();
	}
}
