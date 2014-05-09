using EntityFramework.Patterns;
using EntityFramework.Patterns.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Entity.Infrastructures
{
	public class AuditableRepository<T> : EntityFramework.Patterns.Decorators.AuditableRepository<T>
		where T : class
	{
		public AuditableRepository(IRepository<T> surrogate) : base(surrogate) { }

		/// <summary>
		/// エンティティを新たに挿入します
		/// </summary>
		/// <param name="entity"></param>
		public override void Insert(T entity)
		{
			base.Insert(entity);

			IEntityMaster auditable = entity as IEntityMaster;
			if (auditable != null)
				auditable.ModifiedOn = DateTime.Now;
		}

		/// <summary>
		/// エンティティを編集モードに変更します。
		/// </summary>
		/// <param name="entity"></param>
		public override void Update(T entity)
		{
			base.Update(entity);

			IEntityMaster auditable = entity as IEntityMaster;
			if (auditable != null)
				auditable.ModifiedOn = DateTime.Now; // ConcurrencyCheckがマークされたプロパティは、先に編集モードにしてから値を更新しなければなりません。
		}
	}
}
