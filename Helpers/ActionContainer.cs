using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BeerMP.Helpers
{
	// Token: 0x02000079 RID: 121
	public class ActionContainer
	{
		// Token: 0x0600037A RID: 890 RVA: 0x0001A37C File Offset: 0x0001857C
		public static ActionContainer operator +(ActionContainer self, Action action)
		{
			if (self == null)
			{
				self = new ActionContainer();
			}
			StackTrace stackTrace = new StackTrace();
			self.actions.Add(action, stackTrace.GetFrame(1).GetMethod());
			return self;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0001A3B2 File Offset: 0x000185B2
		public static ActionContainer operator -(ActionContainer self, Action action)
		{
			if (self == null)
			{
				throw new NullReferenceException();
			}
			self.actions.Remove(action);
			return self;
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0001A3CC File Offset: 0x000185CC
		public void Invoke()
		{
			foreach (KeyValuePair<Action, MethodBase> keyValuePair in this.actions)
			{
				try
				{
					Action key = keyValuePair.Key;
					if (key != null)
					{
						key();
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

		// Token: 0x0600037D RID: 893 RVA: 0x0001A484 File Offset: 0x00018684
		public ActionContainer()
		{
		}

		// Token: 0x0400037A RID: 890
		internal Dictionary<Action, MethodBase> actions = new Dictionary<Action, MethodBase>();
	}
}
