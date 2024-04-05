using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BeerMP.Helpers
{
	// Token: 0x0200007A RID: 122
	public class ActionContainer<T>
	{
		// Token: 0x0600037E RID: 894 RVA: 0x0001A498 File Offset: 0x00018698
		public static ActionContainer<T>operator +(ActionContainer<T> self, Action<T> action)
		{
			if (self == null)
			{
				self = new ActionContainer<T>();
			}
			StackTrace stackTrace = new StackTrace();
			self.actions.Add(action, stackTrace.GetFrame(1).GetMethod());
			return self;
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0001A4CE File Offset: 0x000186CE
		public static ActionContainer<T>operator -(ActionContainer<T> self, Action<T> action)
		{
			if (self == null)
			{
				throw new NullReferenceException();
			}
			self.actions.Remove(action);
			return self;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0001A4E8 File Offset: 0x000186E8
		public void Invoke(T obj)
		{
			foreach (KeyValuePair<Action<T>, MethodBase> keyValuePair in this.actions)
			{
				try
				{
					Action<T> key = keyValuePair.Key;
					if (key != null)
					{
						key(obj);
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

		// Token: 0x06000381 RID: 897 RVA: 0x0001A5A0 File Offset: 0x000187A0
		public ActionContainer()
		{
		}

		// Token: 0x0400037B RID: 891
		internal Dictionary<Action<T>, MethodBase> actions = new Dictionary<Action<T>, MethodBase>();
	}
}
