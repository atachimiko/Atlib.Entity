﻿using Atlib.Exceptions;
using EntityFramework.Patterns;
using EntityFramework.Patterns.Decorators;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Entity.Infrastructures
{
	public class BqRepository<T> where T : class, IEntityData
	{
		static ILog LOG = LogManager.GetLogger(typeof(BqRepository<T>));

		public BqRepository(DbContext context)
		{
			this._Context = context;

			var @adp = new DbContextAdapter(_Context);

			var r = new Repository<T>(@adp);
			this.Archivable  = new ArchivableRepository<T>(r);
			this.Auditable = new AuditableRepository<T>(r);
		}

		public T GetById(long key)
		{
			return this.Archivable.Find(p => p.Id == key).FirstOrDefault();
		}

		public void Save()
		{
			try
			{
				var @adp = new DbContextAdapter(_Context);
				var @uow = new UnitOfWork(@adp);
				@uow.Commit();
			}
			catch (DbEntityValidationException expr)
			{
				foreach (var errors in expr.EntityValidationErrors)
				{
					foreach (var error in errors.ValidationErrors)
					{
						LOG.Error(error.ErrorMessage);    // log4netの出力に表示
					}
				}

				throw new EntityException();
			}
		}


		public ArchivableRepository<T> Archivable { get; private set; }
		public AuditableRepository<T> Auditable { get; private set; }
		readonly DbContext _Context;
	}
}