using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000032 RID: 50
	public class RelationshipManager
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000063 RID: 99 RVA: 0x0000396B File Offset: 0x00001B6B
		private RelationshipManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(RelationshipManager.FFIMethods));
				}
				return (RelationshipManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000064 RID: 100 RVA: 0x0000399C File Offset: 0x00001B9C
		// (remove) Token: 0x06000065 RID: 101 RVA: 0x000039D4 File Offset: 0x00001BD4
		public event RelationshipManager.RefreshHandler OnRefresh
		{
			[CompilerGenerated]
			add
			{
				RelationshipManager.RefreshHandler refreshHandler = this.OnRefresh;
				RelationshipManager.RefreshHandler refreshHandler2;
				do
				{
					refreshHandler2 = refreshHandler;
					RelationshipManager.RefreshHandler refreshHandler3 = (RelationshipManager.RefreshHandler)Delegate.Combine(refreshHandler2, value);
					refreshHandler = Interlocked.CompareExchange<RelationshipManager.RefreshHandler>(ref this.OnRefresh, refreshHandler3, refreshHandler2);
				}
				while (refreshHandler != refreshHandler2);
			}
			[CompilerGenerated]
			remove
			{
				RelationshipManager.RefreshHandler refreshHandler = this.OnRefresh;
				RelationshipManager.RefreshHandler refreshHandler2;
				do
				{
					refreshHandler2 = refreshHandler;
					RelationshipManager.RefreshHandler refreshHandler3 = (RelationshipManager.RefreshHandler)Delegate.Remove(refreshHandler2, value);
					refreshHandler = Interlocked.CompareExchange<RelationshipManager.RefreshHandler>(ref this.OnRefresh, refreshHandler3, refreshHandler2);
				}
				while (refreshHandler != refreshHandler2);
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000066 RID: 102 RVA: 0x00003A0C File Offset: 0x00001C0C
		// (remove) Token: 0x06000067 RID: 103 RVA: 0x00003A44 File Offset: 0x00001C44
		public event RelationshipManager.RelationshipUpdateHandler OnRelationshipUpdate
		{
			[CompilerGenerated]
			add
			{
				RelationshipManager.RelationshipUpdateHandler relationshipUpdateHandler = this.OnRelationshipUpdate;
				RelationshipManager.RelationshipUpdateHandler relationshipUpdateHandler2;
				do
				{
					relationshipUpdateHandler2 = relationshipUpdateHandler;
					RelationshipManager.RelationshipUpdateHandler relationshipUpdateHandler3 = (RelationshipManager.RelationshipUpdateHandler)Delegate.Combine(relationshipUpdateHandler2, value);
					relationshipUpdateHandler = Interlocked.CompareExchange<RelationshipManager.RelationshipUpdateHandler>(ref this.OnRelationshipUpdate, relationshipUpdateHandler3, relationshipUpdateHandler2);
				}
				while (relationshipUpdateHandler != relationshipUpdateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				RelationshipManager.RelationshipUpdateHandler relationshipUpdateHandler = this.OnRelationshipUpdate;
				RelationshipManager.RelationshipUpdateHandler relationshipUpdateHandler2;
				do
				{
					relationshipUpdateHandler2 = relationshipUpdateHandler;
					RelationshipManager.RelationshipUpdateHandler relationshipUpdateHandler3 = (RelationshipManager.RelationshipUpdateHandler)Delegate.Remove(relationshipUpdateHandler2, value);
					relationshipUpdateHandler = Interlocked.CompareExchange<RelationshipManager.RelationshipUpdateHandler>(ref this.OnRelationshipUpdate, relationshipUpdateHandler3, relationshipUpdateHandler2);
				}
				while (relationshipUpdateHandler != relationshipUpdateHandler2);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003A7C File Offset: 0x00001C7C
		internal RelationshipManager(IntPtr ptr, IntPtr eventsPtr, ref RelationshipManager.FFIEvents events)
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

		// Token: 0x06000069 RID: 105 RVA: 0x00003ACB File Offset: 0x00001CCB
		private void InitEvents(IntPtr eventsPtr, ref RelationshipManager.FFIEvents events)
		{
			events.OnRefresh = new RelationshipManager.FFIEvents.RefreshHandler(RelationshipManager.OnRefreshImpl);
			events.OnRelationshipUpdate = new RelationshipManager.FFIEvents.RelationshipUpdateHandler(RelationshipManager.OnRelationshipUpdateImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003B04 File Offset: 0x00001D04
		[MonoPInvokeCallback]
		private static bool FilterCallbackImpl(IntPtr ptr, ref Relationship relationship)
		{
			return ((RelationshipManager.FilterHandler)GCHandle.FromIntPtr(ptr).Target)(ref relationship);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003B2C File Offset: 0x00001D2C
		public void Filter(RelationshipManager.FilterHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.Filter(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new RelationshipManager.FFIMethods.FilterCallback(RelationshipManager.FilterCallbackImpl));
			gchandle.Free();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003B70 File Offset: 0x00001D70
		public int Count()
		{
			int num = 0;
			Result result = this.Methods.Count(this.MethodsPtr, ref num);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return num;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003BA4 File Offset: 0x00001DA4
		public Relationship Get(long userId)
		{
			Relationship relationship = default(Relationship);
			Result result = this.Methods.Get(this.MethodsPtr, userId, ref relationship);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return relationship;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003BE0 File Offset: 0x00001DE0
		public Relationship GetAt(uint index)
		{
			Relationship relationship = default(Relationship);
			Result result = this.Methods.GetAt(this.MethodsPtr, index, ref relationship);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return relationship;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003C1C File Offset: 0x00001E1C
		[MonoPInvokeCallback]
		private static void OnRefreshImpl(IntPtr ptr)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.RelationshipManagerInstance.OnRefresh != null)
			{
				discord.RelationshipManagerInstance.OnRefresh();
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003C5C File Offset: 0x00001E5C
		[MonoPInvokeCallback]
		private static void OnRelationshipUpdateImpl(IntPtr ptr, ref Relationship relationship)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.RelationshipManagerInstance.OnRelationshipUpdate != null)
			{
				discord.RelationshipManagerInstance.OnRelationshipUpdate(ref relationship);
			}
		}

		// Token: 0x040000FC RID: 252
		private IntPtr MethodsPtr;

		// Token: 0x040000FD RID: 253
		private object MethodsStructure;

		// Token: 0x040000FE RID: 254
		[CompilerGenerated]
		private RelationshipManager.RefreshHandler OnRefresh;

		// Token: 0x040000FF RID: 255
		[CompilerGenerated]
		private RelationshipManager.RelationshipUpdateHandler OnRelationshipUpdate;

		// Token: 0x020000A4 RID: 164
		internal struct FFIEvents
		{
			// Token: 0x040003E8 RID: 1000
			internal RelationshipManager.FFIEvents.RefreshHandler OnRefresh;

			// Token: 0x040003E9 RID: 1001
			internal RelationshipManager.FFIEvents.RelationshipUpdateHandler OnRelationshipUpdate;

			// Token: 0x02000199 RID: 409
			// (Invoke) Token: 0x06000749 RID: 1865
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void RefreshHandler(IntPtr ptr);

			// Token: 0x0200019A RID: 410
			// (Invoke) Token: 0x0600074D RID: 1869
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void RelationshipUpdateHandler(IntPtr ptr, ref Relationship relationship);
		}

		// Token: 0x020000A5 RID: 165
		internal struct FFIMethods
		{
			// Token: 0x040003EA RID: 1002
			internal RelationshipManager.FFIMethods.FilterMethod Filter;

			// Token: 0x040003EB RID: 1003
			internal RelationshipManager.FFIMethods.CountMethod Count;

			// Token: 0x040003EC RID: 1004
			internal RelationshipManager.FFIMethods.GetMethod Get;

			// Token: 0x040003ED RID: 1005
			internal RelationshipManager.FFIMethods.GetAtMethod GetAt;

			// Token: 0x0200019B RID: 411
			// (Invoke) Token: 0x06000751 RID: 1873
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate bool FilterCallback(IntPtr ptr, ref Relationship relationship);

			// Token: 0x0200019C RID: 412
			// (Invoke) Token: 0x06000755 RID: 1877
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FilterMethod(IntPtr methodsPtr, IntPtr callbackData, RelationshipManager.FFIMethods.FilterCallback callback);

			// Token: 0x0200019D RID: 413
			// (Invoke) Token: 0x06000759 RID: 1881
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result CountMethod(IntPtr methodsPtr, ref int count);

			// Token: 0x0200019E RID: 414
			// (Invoke) Token: 0x0600075D RID: 1885
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetMethod(IntPtr methodsPtr, long userId, ref Relationship relationship);

			// Token: 0x0200019F RID: 415
			// (Invoke) Token: 0x06000761 RID: 1889
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetAtMethod(IntPtr methodsPtr, uint index, ref Relationship relationship);
		}

		// Token: 0x020000A6 RID: 166
		// (Invoke) Token: 0x060003F7 RID: 1015
		public delegate bool FilterHandler(ref Relationship relationship);

		// Token: 0x020000A7 RID: 167
		// (Invoke) Token: 0x060003FB RID: 1019
		public delegate void RefreshHandler();

		// Token: 0x020000A8 RID: 168
		// (Invoke) Token: 0x060003FF RID: 1023
		public delegate void RelationshipUpdateHandler(ref Relationship relationship);
	}
}
