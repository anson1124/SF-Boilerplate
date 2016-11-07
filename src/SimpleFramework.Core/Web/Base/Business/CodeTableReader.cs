﻿
using Microsoft.Extensions.Logging;
using SimpleFramework.Core.Abstraction.Data;
using SimpleFramework.Core.Abstraction.Entitys;
using SimpleFramework.Core.Data.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleFramework.Core.Web.Base.Business
{
    /// <summary>
    /// 读取处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CodetableReader<T> : ICodetableReader<T> where T : BaseEntity
    {
        #region Fields

        private readonly IRepositoryAsync<T> _repository;
        protected readonly ILogger _logger;

        #endregion

        #region Constructors
        public CodetableReader(ILogger<T> logger, IRepositoryAsync<T> repository)
        {
            _logger = logger;
            _repository = repository;
        }
        #endregion

        #region Utilities
        protected OrderBy<T> Ordering => new OrderBy<T>(query => query.OrderBy(a => a.Sortindex));
        #endregion

        #region Method

        /// <summary>
        /// 异步获取所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.FilterAsync(orderBy: Ordering.Expression);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return _repository.Filter(orderBy: Ordering.Expression);
        }
        /// <summary>
        /// 根据ID获取实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(int id)
        {
            return _repository.Find(id);
        }
        /// <summary>
        /// 异步根据ID获取实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetAsync(int id)
        {
            return await _repository.FindAsync(id);
        }
        #endregion


    }
}