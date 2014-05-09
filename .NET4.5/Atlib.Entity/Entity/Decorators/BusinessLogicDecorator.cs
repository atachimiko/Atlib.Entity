using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Entity.Decorators
{
	/// <summary>
	///     ビジネスロジックを処理する基本デコレーターを定義したクラス
	/// </summary>
	/// <remarks>
	///     デコレーター間は依存関係がないように各デコレーターを実装する必要があります。
	///     たとえばエンティティを永続化するデコレーターの処理が行われる前に、そのエンティティの状態を調べるような処理を
	///     行うために別のデコレーターを呼び出すというような使い方は、デコレーターの仕組みには向いていません。
	/// </remarks>
	public abstract class BusinessLogicDecorator : LogicComponent
	{
		public override void Operation()
		{
			if (this._Component != null)
				this._Component.Operation();
		}

		/// <summary>
		///     操作を受け取ります
		///    （具象装飾者の操作などを参照するのに使用します）
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public LogicComponent SetComponent
		{
			set { this._Component = value; }
			get { return this._Component; }
		}

		private LogicComponent _Component;
	}
}
