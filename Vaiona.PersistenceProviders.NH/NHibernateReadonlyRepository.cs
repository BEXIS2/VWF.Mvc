﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using System.Linq.Expressions;
using System.Diagnostics.Contracts;
using NHibernate.Metadata;
using Vaiona.Persistence.Api;
using System.Collections;

namespace Vaiona.PersistenceProviders.NH
{
    public class NHibernateReadonlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        protected NHibernateUnitOfWork UoW = null;

        internal NHibernateReadonlyRepository(NHibernateUnitOfWork uow)
        {
            this.UoW = uow;
        }

        public IUnitOfWork UnitOfWork { get { return (UoW);} }

        public void Evict()
        {
            UoW.Session.SessionFactory.Evict(typeof(TEntity));
        }

        public void Evict(object id)
        {
            UoW.Session.SessionFactory.Evict(typeof(TEntity), id);
        }

        public void Evict(TEntity entity)
        {
            UoW.Session.Evict(entity);
        }

        public TEntity Get(long id)
        {
            // NHibernateUtil.Initialize( paths
            return (UoW.Session.Get<TEntity>(id)); 
        }

        public TEntity Reload(TEntity entity)
        {
            Evict(entity);
            IClassMetadata metaInfo = UoW.Session.SessionFactory.GetClassMetadata(typeof(TEntity));
            if (metaInfo.HasIdentifierProperty)
            {
                object idValue = entity.GetType().GetProperty(metaInfo.IdentifierPropertyName).GetValue(entity, null);
                return (UoW.Session.Get<TEntity>(idValue)); 
            }
            return(default(TEntity));
        }

        public TEntity Refresh(Int64 id)
        {
            Evict(id);
            return (UoW.Session.Load<TEntity>(id)); 
        }

        public IList<TEntity> Get(Expression<Func<TEntity, bool>> expression)
        {
            return (this.Query(expression).ToList());
        }

        public IList<TEntity> Get()
        {
            return (this.Query().ToList());
        }

        public IList<TEntity> Get(string namedQuery, Dictionary<string, object> parameters)
        {
            if (parameters != null && !Contract.ForAll(parameters, (KeyValuePair<string, object> p) => p.Value != null))
                throw new ArgumentException("The parameter array has a null element", "parameters");

            IQuery query = UoW.Session.GetNamedQuery(namedQuery);
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    query.SetParameter(item.Key, item.Value);
                }
            }
            return (query.List<TEntity>());
        }

        public IList Get2(string namedQuery, Dictionary<string, object> parameters)
        {
            if (parameters != null && !Contract.ForAll(parameters, (KeyValuePair<string, object> p) => p.Value != null))
                throw new ArgumentException("The parameter array has a null element", "parameters");

            IQuery query = UoW.Session.GetNamedQuery(namedQuery);
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    query.SetParameter(item.Key, item.Value);
                }
            }
            return (query.List()); // returns an un-typed list, a list of objects!
        }

        public IList<TEntity> Get(string queryString, Dictionary<string, object> parameters, bool isNativeOrORM = false)
        {
            if (parameters != null && !Contract.ForAll(parameters, (KeyValuePair<string, object> p) => p.Value != null))
                throw new ArgumentException("The parameter array has a null element", "parameters");

            IQuery query = null;
            if (isNativeOrORM == false) // ORM native query: like HQL
            {
                query = UoW.Session.CreateQuery(queryString);
            }
            else // Database native query
            {
                query = UoW.Session.CreateSQLQuery(queryString).AddEntity(typeof(TEntity));
            }
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    query.SetParameter(item.Key, item.Value);
                }
            }
            return (query.List<TEntity>());
        }

        public IList<TEntity> Get(object criteria)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Query()
        {
            return (UoW.Session.Query<TEntity>());
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            return (this.Query().Where(expression).AsQueryable());
        }

        public IQueryable<TEntity> Query(string expression)
        {
            return (null); // use DynamicLinq to implement this method
        }

        public IQueryable<TEntity> Query(object criteria)
        {
            throw new NotImplementedException();
        }
    
        public bool IsPropertyLoaded(object proxy, string propertyName)
        {
            return (NHibernateUtil.IsPropertyInitialized(proxy, propertyName));
        }
     
        public bool IsLoaded(object proxy)
        {
            return (NHibernateUtil.IsInitialized(proxy));
        }

        public void Load(object proxy)
        {
            NHibernateUtil.Initialize(proxy);
        }

        //public IQueryable<TEntity> QueryWithPath<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> relatedObjectSelector, params List<Expression<Func<TEntity, IEnumerable<TRelated>>>> relatedObjectSelectors)
        //{
        //    var q = UoW.Session.Query<TEntity>().FetchMany(relatedObjectSelector);
        //    foreach (var item in relatedObjectSelectors)
        //    {
        //        q = q.ThenFetchMany<TRelated>(item);
        //    }
        //    return (UoW.Session.Query<TEntity>().FetchMany(relatedObjectSelector));
        //}

        public void LoadIfNot(object proxy)
        {
            if (NHibernateUtil.IsInitialized(proxy))
            {
                NHibernateUtil.Initialize(proxy);
            }
        }
    }
}
