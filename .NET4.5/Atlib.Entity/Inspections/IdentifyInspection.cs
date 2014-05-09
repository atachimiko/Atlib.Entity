using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Inspections
{
	/// <summary>
	///     Identifyとして使用できるかチェックします
	/// </summary>
	public class IdentifyInspection : InspectionBase
	{
		public bool Inspection(string target)
		{
			// : 4文字以上であること(250文字未満であること)
			// : 英数字(アンダーバー含む)で構成される文字列であること

			if (string.IsNullOrEmpty(target)) return ErrorThrow("文字列が入力されていません");
			if (target.Length < 4) return ErrorThrow("文字の長さが短すぎます。");
			if (target.Length > 250) return ErrorThrow("文字の長さが長すぎます。");
			if (!System.Text.RegularExpressions.Regex.IsMatch(target, @"^[_a-zA-Z0-9]+$")) return ErrorThrow("文字列に使用できない文字が含まれています。");
			return true;
		}

		
	}
}
