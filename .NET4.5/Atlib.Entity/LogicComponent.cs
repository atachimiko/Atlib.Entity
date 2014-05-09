using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib
{
	/// <summary>
	///     ビジネスロジックを呼び出すためのインターフェースを実装した
	///     コンポーネントです。
	/// </summary>
	/// <remarks>
	///     Decoratorパターンによる実装を行います。
	///     
	/// </remarks>
	public abstract class LogicComponent
	{
		/// <summary>
		///     処理するビジネスロジックを呼び出すためのインターフェース
		/// </summary>
		public abstract void Operation();
	}
}
