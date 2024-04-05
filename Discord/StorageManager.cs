using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Discord
{
	// Token: 0x02000036 RID: 54
	public class StorageManager
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000054FB File Offset: 0x000036FB
		private StorageManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(StorageManager.FFIMethods));
				}
				return (StorageManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000552C File Offset: 0x0000372C
		internal StorageManager(IntPtr ptr, IntPtr eventsPtr, ref StorageManager.FFIEvents events)
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

		// Token: 0x060000DD RID: 221 RVA: 0x0000557B File Offset: 0x0000377B
		private void InitEvents(IntPtr eventsPtr, ref StorageManager.FFIEvents events)
		{
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00005590 File Offset: 0x00003790
		public uint Read(string name, byte[] data)
		{
			uint num = 0U;
			Result result = this.Methods.Read(this.MethodsPtr, name, data, data.Length, ref num);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return num;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000055C8 File Offset: 0x000037C8
		[MonoPInvokeCallback]
		private static void ReadAsyncCallbackImpl(IntPtr ptr, Result result, IntPtr dataPtr, int dataLen)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			StorageManager.ReadAsyncHandler readAsyncHandler = (StorageManager.ReadAsyncHandler)gchandle.Target;
			gchandle.Free();
			byte[] array = new byte[dataLen];
			Marshal.Copy(dataPtr, array, 0, dataLen);
			readAsyncHandler(result, array);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00005608 File Offset: 0x00003808
		public void ReadAsync(string name, StorageManager.ReadAsyncHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.ReadAsync(this.MethodsPtr, name, GCHandle.ToIntPtr(gchandle), new StorageManager.FFIMethods.ReadAsyncCallback(StorageManager.ReadAsyncCallbackImpl));
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00005648 File Offset: 0x00003848
		[MonoPInvokeCallback]
		private static void ReadAsyncPartialCallbackImpl(IntPtr ptr, Result result, IntPtr dataPtr, int dataLen)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			StorageManager.ReadAsyncPartialHandler readAsyncPartialHandler = (StorageManager.ReadAsyncPartialHandler)gchandle.Target;
			gchandle.Free();
			byte[] array = new byte[dataLen];
			Marshal.Copy(dataPtr, array, 0, dataLen);
			readAsyncPartialHandler(result, array);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00005688 File Offset: 0x00003888
		public void ReadAsyncPartial(string name, ulong offset, ulong length, StorageManager.ReadAsyncPartialHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.ReadAsyncPartial(this.MethodsPtr, name, offset, length, GCHandle.ToIntPtr(gchandle), new StorageManager.FFIMethods.ReadAsyncPartialCallback(StorageManager.ReadAsyncPartialCallbackImpl));
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000056C8 File Offset: 0x000038C8
		public void Write(string name, byte[] data)
		{
			Result result = this.Methods.Write(this.MethodsPtr, name, data, data.Length);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000056FC File Offset: 0x000038FC
		[MonoPInvokeCallback]
		private static void WriteAsyncCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			StorageManager.WriteAsyncHandler writeAsyncHandler = (StorageManager.WriteAsyncHandler)gchandle.Target;
			gchandle.Free();
			writeAsyncHandler(result);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000572C File Offset: 0x0000392C
		public void WriteAsync(string name, byte[] data, StorageManager.WriteAsyncHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.WriteAsync(this.MethodsPtr, name, data, data.Length, GCHandle.ToIntPtr(gchandle), new StorageManager.FFIMethods.WriteAsyncCallback(StorageManager.WriteAsyncCallbackImpl));
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00005770 File Offset: 0x00003970
		public void Delete(string name)
		{
			Result result = this.Methods.Delete(this.MethodsPtr, name);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000057A0 File Offset: 0x000039A0
		public bool Exists(string name)
		{
			bool flag = false;
			Result result = this.Methods.Exists(this.MethodsPtr, name, ref flag);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return flag;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000057D4 File Offset: 0x000039D4
		public int Count()
		{
			int num = 0;
			this.Methods.Count(this.MethodsPtr, ref num);
			return num;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x000057FC File Offset: 0x000039FC
		public FileStat Stat(string name)
		{
			FileStat fileStat = default(FileStat);
			Result result = this.Methods.Stat(this.MethodsPtr, name, ref fileStat);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return fileStat;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00005838 File Offset: 0x00003A38
		public FileStat StatAt(int index)
		{
			FileStat fileStat = default(FileStat);
			Result result = this.Methods.StatAt(this.MethodsPtr, index, ref fileStat);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return fileStat;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00005874 File Offset: 0x00003A74
		public string GetPath()
		{
			StringBuilder stringBuilder = new StringBuilder(4096);
			Result result = this.Methods.GetPath(this.MethodsPtr, stringBuilder);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000058B4 File Offset: 0x00003AB4
		public IEnumerable<FileStat> Files()
		{
			int num = this.Count();
			List<FileStat> list = new List<FileStat>();
			for (int i = 0; i < num; i++)
			{
				list.Add(this.StatAt(i));
			}
			return list;
		}

		// Token: 0x04000111 RID: 273
		private IntPtr MethodsPtr;

		// Token: 0x04000112 RID: 274
		private object MethodsStructure;

		// Token: 0x020000C9 RID: 201
		internal struct FFIEvents
		{
		}

		// Token: 0x020000CA RID: 202
		internal struct FFIMethods
		{
			// Token: 0x04000428 RID: 1064
			internal StorageManager.FFIMethods.ReadMethod Read;

			// Token: 0x04000429 RID: 1065
			internal StorageManager.FFIMethods.ReadAsyncMethod ReadAsync;

			// Token: 0x0400042A RID: 1066
			internal StorageManager.FFIMethods.ReadAsyncPartialMethod ReadAsyncPartial;

			// Token: 0x0400042B RID: 1067
			internal StorageManager.FFIMethods.WriteMethod Write;

			// Token: 0x0400042C RID: 1068
			internal StorageManager.FFIMethods.WriteAsyncMethod WriteAsync;

			// Token: 0x0400042D RID: 1069
			internal StorageManager.FFIMethods.DeleteMethod Delete;

			// Token: 0x0400042E RID: 1070
			internal StorageManager.FFIMethods.ExistsMethod Exists;

			// Token: 0x0400042F RID: 1071
			internal StorageManager.FFIMethods.CountMethod Count;

			// Token: 0x04000430 RID: 1072
			internal StorageManager.FFIMethods.StatMethod Stat;

			// Token: 0x04000431 RID: 1073
			internal StorageManager.FFIMethods.StatAtMethod StatAt;

			// Token: 0x04000432 RID: 1074
			internal StorageManager.FFIMethods.GetPathMethod GetPath;

			// Token: 0x020001E9 RID: 489
			// (Invoke) Token: 0x06000889 RID: 2185
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result ReadMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name, byte[] data, int dataLen, ref uint read);

			// Token: 0x020001EA RID: 490
			// (Invoke) Token: 0x0600088D RID: 2189
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ReadAsyncCallback(IntPtr ptr, Result result, IntPtr dataPtr, int dataLen);

			// Token: 0x020001EB RID: 491
			// (Invoke) Token: 0x06000891 RID: 2193
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ReadAsyncMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr callbackData, StorageManager.FFIMethods.ReadAsyncCallback callback);

			// Token: 0x020001EC RID: 492
			// (Invoke) Token: 0x06000895 RID: 2197
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ReadAsyncPartialCallback(IntPtr ptr, Result result, IntPtr dataPtr, int dataLen);

			// Token: 0x020001ED RID: 493
			// (Invoke) Token: 0x06000899 RID: 2201
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ReadAsyncPartialMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name, ulong offset, ulong length, IntPtr callbackData, StorageManager.FFIMethods.ReadAsyncPartialCallback callback);

			// Token: 0x020001EE RID: 494
			// (Invoke) Token: 0x0600089D RID: 2205
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result WriteMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name, byte[] data, int dataLen);

			// Token: 0x020001EF RID: 495
			// (Invoke) Token: 0x060008A1 RID: 2209
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void WriteAsyncCallback(IntPtr ptr, Result result);

			// Token: 0x020001F0 RID: 496
			// (Invoke) Token: 0x060008A5 RID: 2213
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void WriteAsyncMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name, byte[] data, int dataLen, IntPtr callbackData, StorageManager.FFIMethods.WriteAsyncCallback callback);

			// Token: 0x020001F1 RID: 497
			// (Invoke) Token: 0x060008A9 RID: 2217
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result DeleteMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name);

			// Token: 0x020001F2 RID: 498
			// (Invoke) Token: 0x060008AD RID: 2221
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result ExistsMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name, ref bool exists);

			// Token: 0x020001F3 RID: 499
			// (Invoke) Token: 0x060008B1 RID: 2225
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CountMethod(IntPtr methodsPtr, ref int count);

			// Token: 0x020001F4 RID: 500
			// (Invoke) Token: 0x060008B5 RID: 2229
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result StatMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string name, ref FileStat stat);

			// Token: 0x020001F5 RID: 501
			// (Invoke) Token: 0x060008B9 RID: 2233
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result StatAtMethod(IntPtr methodsPtr, int index, ref FileStat stat);

			// Token: 0x020001F6 RID: 502
			// (Invoke) Token: 0x060008BD RID: 2237
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetPathMethod(IntPtr methodsPtr, StringBuilder path);
		}

		// Token: 0x020000CB RID: 203
		// (Invoke) Token: 0x0600046B RID: 1131
		public delegate void ReadAsyncHandler(Result result, byte[] data);

		// Token: 0x020000CC RID: 204
		// (Invoke) Token: 0x0600046F RID: 1135
		public delegate void ReadAsyncPartialHandler(Result result, byte[] data);

		// Token: 0x020000CD RID: 205
		// (Invoke) Token: 0x06000473 RID: 1139
		public delegate void WriteAsyncHandler(Result result);
	}
}
