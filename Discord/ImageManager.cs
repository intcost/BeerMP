using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000031 RID: 49
	public class ImageManager
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600005A RID: 90 RVA: 0x000037B6 File Offset: 0x000019B6
		private ImageManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(ImageManager.FFIMethods));
				}
				return (ImageManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000037E8 File Offset: 0x000019E8
		internal ImageManager(IntPtr ptr, IntPtr eventsPtr, ref ImageManager.FFIEvents events)
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

		// Token: 0x0600005C RID: 92 RVA: 0x00003837 File Offset: 0x00001A37
		private void InitEvents(IntPtr eventsPtr, ref ImageManager.FFIEvents events)
		{
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000384C File Offset: 0x00001A4C
		[MonoPInvokeCallback]
		private static void FetchCallbackImpl(IntPtr ptr, Result result, ImageHandle handleResult)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ImageManager.FetchHandler fetchHandler = (ImageManager.FetchHandler)gchandle.Target;
			gchandle.Free();
			fetchHandler(result, handleResult);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000387C File Offset: 0x00001A7C
		public void Fetch(ImageHandle handle, bool refresh, ImageManager.FetchHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.Fetch(this.MethodsPtr, handle, refresh, GCHandle.ToIntPtr(gchandle), new ImageManager.FFIMethods.FetchCallback(ImageManager.FetchCallbackImpl));
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000038BC File Offset: 0x00001ABC
		public ImageDimensions GetDimensions(ImageHandle handle)
		{
			ImageDimensions imageDimensions = default(ImageDimensions);
			Result result = this.Methods.GetDimensions(this.MethodsPtr, handle, ref imageDimensions);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return imageDimensions;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000038F8 File Offset: 0x00001AF8
		public void GetData(ImageHandle handle, byte[] data)
		{
			Result result = this.Methods.GetData(this.MethodsPtr, handle, data, data.Length);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000392B File Offset: 0x00001B2B
		public void Fetch(ImageHandle handle, ImageManager.FetchHandler callback)
		{
			this.Fetch(handle, false, callback);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003938 File Offset: 0x00001B38
		public byte[] GetData(ImageHandle handle)
		{
			ImageDimensions dimensions = this.GetDimensions(handle);
			byte[] array = new byte[dimensions.Width * dimensions.Height * 4U];
			this.GetData(handle, array);
			return array;
		}

		// Token: 0x040000FA RID: 250
		private IntPtr MethodsPtr;

		// Token: 0x040000FB RID: 251
		private object MethodsStructure;

		// Token: 0x020000A1 RID: 161
		internal struct FFIEvents
		{
		}

		// Token: 0x020000A2 RID: 162
		internal struct FFIMethods
		{
			// Token: 0x040003E5 RID: 997
			internal ImageManager.FFIMethods.FetchMethod Fetch;

			// Token: 0x040003E6 RID: 998
			internal ImageManager.FFIMethods.GetDimensionsMethod GetDimensions;

			// Token: 0x040003E7 RID: 999
			internal ImageManager.FFIMethods.GetDataMethod GetData;

			// Token: 0x02000195 RID: 405
			// (Invoke) Token: 0x06000739 RID: 1849
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchCallback(IntPtr ptr, Result result, ImageHandle handleResult);

			// Token: 0x02000196 RID: 406
			// (Invoke) Token: 0x0600073D RID: 1853
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchMethod(IntPtr methodsPtr, ImageHandle handle, bool refresh, IntPtr callbackData, ImageManager.FFIMethods.FetchCallback callback);

			// Token: 0x02000197 RID: 407
			// (Invoke) Token: 0x06000741 RID: 1857
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetDimensionsMethod(IntPtr methodsPtr, ImageHandle handle, ref ImageDimensions dimensions);

			// Token: 0x02000198 RID: 408
			// (Invoke) Token: 0x06000745 RID: 1861
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetDataMethod(IntPtr methodsPtr, ImageHandle handle, byte[] data, int dataLen);
		}

		// Token: 0x020000A3 RID: 163
		// (Invoke) Token: 0x060003F3 RID: 1011
		public delegate void FetchHandler(Result result, ImageHandle handleResult);
	}
}
