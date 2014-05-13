using Atlib.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Entity.Decorators
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PersistenceEntityDecorator<T> : BusinessLogicDecorator
		where T : class, IEntityData
	{
		/// <summary>
		///     コンテキストを使って永続化処理を行います
		/// </summary>
		/// <param name="entity" type="T">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="context" type="System.Data.Entity.DbContext">
		///     <para>
		///         
		///     </para>
		/// </param>
		public PersistenceEntityDecorator(T entity, DbContext context)
		{
			this._Entity = entity;
			this._Context = context;
		}

		/// <summary>
		///     指定したEntityViewModelを使って永続化処理を行います
		/// </summary>
		/// <param name="entityViewModel" type="BacksQuartet.Core.EntityModelBase<T>">
		///     <para>
		///         
		///     </para>
		/// </param>
		public PersistenceEntityDecorator(EntityModelBase<T> entityViewModel)
		{
			this._EntityViewModel = entityViewModel;
		}

		public override void Operation()
		{
			base.Operation();

			try
			{
				// IEntityDataを永続化するための処理
				if (_EntityViewModel == null)
				{
					var a = new EntityModelBase<T>(this._Entity, this._Context);
					a.Save();
				}
				else
				{
					_EntityViewModel.Save();
				}
			}
			catch (DbEntityValidationException expr)
			{
				StringBuilder sb = new StringBuilder();
				foreach (var validationErrors in expr.EntityValidationErrors)
				{
					foreach (var validationError in validationErrors.ValidationErrors)
					{
						sb.AppendLine(string.Format("Property: {0} Error: {1}",
								  validationError.PropertyName,
								  validationError.ErrorMessage
								  )
					  );
					}
				}

				throw new EntityException(sb.ToString());
			}
			catch (Exception expr)
			{
				throw new EntityException("保存に失敗しました。", expr);
			}
		}

		EntityModelBase<T> _EntityViewModel;
		T _Entity;
		DbContext _Context;
	}
}
