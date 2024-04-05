using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BeerMP.Helpers
{
	// Token: 0x0200007B RID: 123
	public class ActionContainer<T1, T2>
	{
		// Token: 0x06000382 RID: 898 RVA: 0x0001A5B4 File Offset: 0x000187B4
		public static ActionContainer<T1, T2>operator +(ActionContainer<T1, T2> self, Action<T1, T2> action)
		{
			if (self == null)
			{
				self = new ActionContainer<T1, T2>();
			}
			StackTrace stackTrace = new StackTrace();
			self.actions.Add(action, stackTrace.GetFrame(1).GetMethod());
			return self;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0001A5EA File Offset: 0x000187EA
		public static ActionContainer<T1, T2>operator -(ActionContainer<T1, T2> self, Action<T1, T2> action)
		{
			if (self == null)
			{
				throw new NullReferenceException();
			}
			self.actions.Remove(action);
			return self;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0001A604 File Offset: 0x00018804
		public void Invoke(T1 arg1, T2 arg2)
		{
			foreach (KeyValuePair<Action<T1, T2>, MethodBase> keyValuePair in this.actions)
			{
				try
				{
					Action<T1, T2> key = keyValuePair.Key;
					if (key != null)
					{
						key(arg1, arg2);
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

		// Token: 0x06000385 RID: 901 RVA: 0x0001A6BC File Offset: 0x000188BC
		public ActionContainer()
		{
		}

		// Token: 0x0400037C RID: 892
		internal Dictionary<Action<T1, T2>, MethodBase> actions = new Dictionary<Action<T1, T2>, MethodBase>();
	}
}
