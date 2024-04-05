using System;
using UnityEngine;

namespace BeerMP
{
	// Token: 0x02000041 RID: 65
	internal static class RichTextUtility
	{
		// Token: 0x06000148 RID: 328 RVA: 0x000071B8 File Offset: 0x000053B8
		private static string Hexify(this Color color)
		{
			Color white = global::UnityEngine.Color.white;
			int num = (int)color.a;
			int num2 = (int)((float)(num / 255) * color.r + (float)(1 - num / 255) * white.r);
			int num3 = (int)((float)(num / 255) * color.g + (float)(1 - num / 255) * white.g);
			int num4 = (int)((float)(num / 255) * color.b + (float)(1 - num / 255) * white.b);
			return "#" + num2.ToString("X2") + num3.ToString("X2") + num4.ToString("X2");
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000726B File Offset: 0x0000546B
		public static string Bold(this string text)
		{
			return "<b>" + text + "</b>";
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000727D File Offset: 0x0000547D
		public static string Italic(this string text)
		{
			return "<i>" + text + "</i>";
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000728F File Offset: 0x0000548F
		public static string Size(this string text, int size)
		{
			return string.Format("<size={0}>{1}</size>", size, text);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000072A2 File Offset: 0x000054A2
		public static string Color(this string text, Color color)
		{
			return string.Concat(new string[]
			{
				"<color=",
				color.Hexify(),
				">",
				text,
				"</color>"
			});
		}

		// Token: 0x0600014D RID: 333 RVA: 0x000072D4 File Offset: 0x000054D4
		public static string Color(this string text, string colorText)
		{
			return string.Concat(new string[] { "<color=", colorText, ">", text, "</color>" });
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00007304 File Offset: 0x00005504
		public static string ClearColor(this string text)
		{
			string text2 = "";
			bool flag = false;
			int num = text.Length;
			if (text.EndsWith("</color>"))
			{
				num -= "</color>".Length;
			}
			for (int i = 0; i < num; i++)
			{
				if (i + 7 < num && text.Substring(i, 7) == "<color=")
				{
					flag = true;
					i += 6;
				}
				else if (i + 8 < num && text.Substring(i, 8) == "</color>")
				{
					i += 7;
				}
				else
				{
					if (!flag)
					{
						text2 += text[i].ToString();
					}
					if (flag && text[i] == '>')
					{
						flag = false;
					}
				}
			}
			return text2;
		}
	}
}
