using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BeerMP.Helpers
{
	// Token: 0x0200007D RID: 125
	public class ActionContainer<T1, T2, T3, T4>
	{
		// Token: 0x0600038A RID: 906 RVA: 0x0001A7EC File Offset: 0x000189EC
		public static ActionContainer<T1, T2, T3, T4>operator +(ActionContainer<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> action)
		{
			if (self == null)
			{
				self = new ActionContainer<T1, T2, T3, T4>();
			}
			StackTrace stackTrace = new StackTrace();
			self.actions.Add(action, stackTrace.GetFrame(1).GetMethod());
			return self;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0001A822 File Offset: 0x00018A22
		public static ActionContainer<T1, T2, T3, T4>operator -(ActionContainer<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> action)
		{
			if (self == null)
			{
				throw new NullReferenceException();
			}
			self.actions.Remove(action);
			return self;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0001A83C File Offset: 0x00018A3C
		public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			foreach (KeyValuePair<Action<T1, T2, T3, T4>, MethodBase> keyValuePair in this.actions)
			{
				try
				{
					Action<T1, T2, T3, T4> key = keyValuePair.Key;
					if (key != null)
					{
						key(arg1, arg2, arg3, arg4);
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

		// Token: 0x0600038D RID: 909 RVA: 0x0001A8F8 File Offset: 0x00018AF8
		public ActionContainer()
		{
		}

		// Token: 0x0400037E RID: 894
		internal Dictionary<Action<T1, T2, T3, T4>, MethodBase> actions = new Dictionary<Action<T1, T2, T3, T4>, MethodBase>();
	}
}
