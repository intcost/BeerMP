using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BeerMP.Helpers
{
	// Token: 0x0200007C RID: 124
	public class ActionContainer<T1, T2, T3>
	{
		// Token: 0x06000386 RID: 902 RVA: 0x0001A6D0 File Offset: 0x000188D0
		public static ActionContainer<T1, T2, T3>operator +(ActionContainer<T1, T2, T3> self, Action<T1, T2, T3> action)
		{
			if (self == null)
			{
				self = new ActionContainer<T1, T2, T3>();
			}
			StackTrace stackTrace = new StackTrace();
			self.actions.Add(action, stackTrace.GetFrame(1).GetMethod());
			return self;
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0001A706 File Offset: 0x00018906
		public static ActionContainer<T1, T2, T3>operator -(ActionContainer<T1, T2, T3> self, Action<T1, T2, T3> action)
		{
			if (self == null)
			{
				throw new NullReferenceException();
			}
			self.actions.Remove(action);
			return self;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001A720 File Offset: 0x00018920
		public void Invoke(T1 arg1, T2 arg2, T3 arg3)
		{
			foreach (KeyValuePair<Action<T1, T2, T3>, MethodBase> keyValuePair in this.actions)
			{
				try
				{
					Action<T1, T2, T3> key = keyValuePair.Key;
					if (key != null)
					{
						key(arg1, arg2, arg3);
					}
				}
				catch (Exception ex)
				{
					Console.LogError(string.Format("ActionContainer: action of {0}.{1} threw an exception!\nMessage: {2}, stack trace: {3}", new object[]
					{
						keyValuePair.Value.DeclaringType,
						keyValuePair.Value.Name,
						ex.Message,
						ex.StackTrace
					}), false);
				}
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0001A7D8 File Offset: 0x000189D8
		public ActionContainer()
		{
		}

		// Token: 0x0400037D RID: 893
		internal Dictionary<Action<T1, T2, T3>, MethodBase> actions = new Dictionary<Action<T1, T2, T3>, MethodBase>();
	}
}
