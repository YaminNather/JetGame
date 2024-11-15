﻿#define USE_THREADING

using System;
using System.Text;

namespace Sisus
{
	/// <summary>
	/// A container of CodeLines, optimized for line-by-line renderering of Code.
	/// Each CodeLine contains a syntax formatted and plain text representation of its code,
	/// for easy switching between the two representations when displaying the code to the end-user. 
	/// </summary>
	[Serializable]
	public class Code
	{
		public CodeLine[] lines;
		public float width;

		private ITextSyntaxFormatter builder;

		public int LineCount => lines.Length;

        public string this[int index] => lines[index].formatted;

        public Code(ITextSyntaxFormatter codeBuilder)
		{
			lines = new CodeLine[0];
			builder = codeBuilder;
		}
	
		public string GetLineUnformatted(int index)
		{
			return lines[index].unformatted;
		}

		public void SetLine(int index, string value)
		{
			lines[index].Set(value, builder);
		}

		public void RemoveAt(int index)
		{
			lines = lines.RemoveAt(index);
		}

		public void InsertAt(int index, string content)
		{
			lines = lines.InsertAt(index, CodeLinePool.Create(content, builder));
		}

		public override string ToString()
		{
			int count = lines.Length;
			if(count > 0)
			{
				var sb = new StringBuilder(count*15);
				sb.Append(lines[0].unformatted);

				for(int n = 1; n < count; n++)
				{
					sb.Append(Environment.NewLine);
					sb.Append(lines[n].unformatted);
				}

				return sb.ToString();
			}
			return "";
		}

		public void Dispose()
		{
			if(builder != null)
			{
				builder.Dispose();
				builder = null;
			}

			for(int n = LineCount - 1; n >= 0; n--)
			{
				CodeLinePool.Dispose(ref lines[n]);
			}
		}

		public string TextUnformatted => builder.TextUnformatted;
    }
}