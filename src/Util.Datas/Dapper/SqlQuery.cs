﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Util.Datas.Sql;
using Util.Datas.Sql.Queries.Builders.Abstractions;
using Util.Domains.Repositories;

namespace Util.Datas.Dapper {
    /// <summary>
    /// Dapper Sql查询对象
    /// </summary>
    public class SqlQuery : Util.Datas.Sql.Queries.SqlQueryBase {
        /// <summary>
        /// 初始化Dapper Sql查询对象
        /// </summary>
        /// <param name="sqlBuilder">Sql生成器</param>
        /// <param name="database">数据库</param>
        public SqlQuery( ISqlBuilder sqlBuilder, IDatabase database = null ) : base( sqlBuilder, database ) {
        }

        /// <summary>
        /// 获取单值
        /// </summary>
        protected override object ToScalar( IDbConnection connection, string sql, IDictionary<string, object> parameters ) {
            return GetConnection( connection ).ExecuteScalar( sql, parameters );
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <typeparam name="TResult">实体类型</typeparam>
        /// <param name="connection">数据库连接</param>
        public override TResult To<TResult>( IDbConnection connection = null ) {
            return GetConnection( connection ).QueryFirstOrDefault<TResult>( Sql, Params );
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <typeparam name="TResult">实体类型</typeparam>
        /// <param name="connection">数据库连接</param>
        public override async Task<TResult> ToAsync<TResult>( IDbConnection connection = null ) {
            return await GetConnection( connection ).QueryFirstOrDefaultAsync<TResult>( Sql, Params );
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="connection">数据库连接</param>
        public override List<TResult> ToList<TResult>( IDbConnection connection = null ) {
            return GetConnection( connection ).Query<TResult>( Sql, Params ).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="connection">数据库连接</param>
        public override async Task<List<TResult>> ToListAsync<TResult>( IDbConnection connection = null ) {
            return ( await GetConnection( connection ).QueryAsync<TResult>( Sql, Params ) ).ToList();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="parameter">分页参数</param>
        /// <param name="connection">数据库连接</param>
        public override PagerList<TResult> ToPagerList<TResult>( IPager parameter, IDbConnection connection = null ) {
            SetPager( parameter, connection );
            return new PagerList<TResult>( parameter, ToList<TResult>( connection ) );
        }

        /// <summary>
        /// 设置分页参数
        /// </summary>
        private void SetPager( IPager parameter, IDbConnection connection ) {
            if( parameter.TotalCount == 0 )
                parameter.TotalCount = GetCount( connection );
            Builder.OrderBy( parameter.Order );
            Builder.Pager( parameter );
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页显示行数</param>
        /// <param name="connection">数据库连接</param>
        public override PagerList<TResult> ToPagerList<TResult>( int page, int pageSize, IDbConnection connection = null ) {
            return ToPagerList<TResult>( new Pager( page, pageSize ), connection );
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="parameter">分页参数</param>
        /// <param name="connection">数据库连接</param>
        public override async Task<PagerList<TResult>> ToPagerListAsync<TResult>( IPager parameter, IDbConnection connection = null ) {
            SetPager( parameter, connection );
            return new PagerList<TResult>( parameter, await ToListAsync<TResult>( connection ) );
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页显示行数</param>
        /// <param name="connection">数据库连接</param>
        public override async Task<PagerList<TResult>> ToPagerListAsync<TResult>( int page, int pageSize, IDbConnection connection = null ) {
            return await ToPagerListAsync<TResult>( new Pager( page, pageSize ), connection );
        }
    }
}