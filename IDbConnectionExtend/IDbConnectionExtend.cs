using Expression2Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IDbConnectionExtend
{
    public static class IDbConnectionExtend
    {
        static IDbConnectionExtend()
        {
            //通过静态属性DatabaseType或者静态方法Init均可配置数据库类型
            Expre2Sql.DatabaseType = DatabaseType.SQLServer;
            //Expre2Sql.Init(DatabaseType.SQLServer);
        }

        #region 基于基础封装的扩展
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Add<T>(this IDbConnection con, T t)
        {
            return con.Execute(ModelCache<T>._AddSql, t); ;
        }
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Add<T>(this IDbCommand cmd, T t)
        {
            return cmd.Execute(ModelCache<T>._AddSql, t); ;
        }
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete<T>(this IDbConnection con, int id)
        {
            return con.Execute(ModelCache<T>._DeleteSql, new { ID = id });
        }

        public static int Delete<T>(this IDbConnection con, Expression<Func<T, bool>> expression)
        {
            var exp = Expre2Sql.Delete<T>().Where(expression);
            return con.Execute(exp.SqlStr, exp.DbParams);
        }
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete<T>(this IDbCommand cmd, int id)
        {
            return cmd.Execute(ModelCache<T>._DeleteSql, new { ID = id });
        }
        public static int Delete<T>(this IDbCommand cmd, Expression<Func<T, bool>> expression)
        {
            var exp = Expre2Sql.Delete<T>().Where(expression);
            return cmd.Execute(exp.SqlStr, exp.DbParams);
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Update<T>(this IDbConnection con, T t)
        {
            return con.Execute(ModelCache<T>._UpdateSql, t);
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Update<T>(this IDbCommand cmd, T t)
        {
            return cmd.Execute(ModelCache<T>._UpdateSql, t);
        }
        /// <summary>
        /// 查找记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IList<T> Query<T>(this IDbConnection con, Expression<Func<T, bool>> expression)
        {
            var exp = Expre2Sql.Select<T>().Where(expression);
            return con.Query<T>(exp.SqlStr, exp.DbParams);
        }
        /// <summary>
        /// 查找记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IList<T> Query<T>(this IDbCommand cmd, Expression<Func<T, bool>> expression)
        {
            var exp = Expre2Sql.Select<T>().Where(expression);
            return cmd.Query<T>(exp.SqlStr, exp.DbParams);
        }


        /// <summary>
        /// 获取总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <returns></returns>
        public static int Count<T>(this IDbConnection con)
        {
            var sql = string.Format("select count(*) from {0}", ModelCache<T>._TableName);
            return con.Execute<int>(sql);
        }
        public static int Count<T>(this IDbConnection con, Expression<Func<T, bool>> expression)
        {
            var exp = Expre2Sql.Select<T>().Where(expression);
            return con.Execute<int>(exp.SqlStr, exp.DbParams);
        }
        /// <summary>
        /// 获取总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <returns></returns>
        public static int Count<T>(this IDbCommand cmd)
        {
            var sql = string.Format("select count(*) from {0}", ModelCache<T>._TableName);
            return cmd.Execute<int>(sql);
        }
        public static int Count<T>(this IDbCommand cmd, Expression<Func<T, bool>> expression)
        {
            var exp = Expre2Sql.Select<T>().Where(expression);
            return cmd.Execute<int>(exp.SqlStr, exp.DbParams);
        }
        #endregion


        /// <summary>
        /// 执行非查询语句或者查询单个结果的语句
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="defaultValue">获取异常的默认返回值</param>
        /// <returns></returns>
        public static T Execute<T>(this IDbConnection con, string sql, object param = null, IList<IDataParameter> parameters = null,T defaultValue = default(T)) 
        {

            return BaseExecute<T>(con, sql, param, parameters, _cmd =>
            {
                try
                {
                    var value = _cmd.ExecuteScalar();
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            });
        }

        /// <summary>
        /// 执行非查询语句或者查询单个结果的语句
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="defaultValue">获取异常的默认返回值</param>
        /// <returns></returns>
        public static T Execute<T>(this IDbCommand cmd, string sql, object param = null, IList<IDataParameter> parameters = null, T defaultValue = default(T)) 
        {

            return BaseExecute<T>(cmd, sql, param, parameters, _cmd =>
            {
                try
                {
                    var value = cmd.ExecuteScalar();
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            });
        }
        /// <summary>
        /// 执行返回影响的行数
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>

        public static int Execute(this IDbConnection con, string sql, object param = null, IList<IDataParameter> parameters = null)
        {
            return BaseExecute<int>(con, sql, param, parameters, _cmd =>
            {
                return _cmd.ExecuteNonQuery();
            });
        }
        /// <summary>
        /// 执行返回影响的行数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int Execute(this IDbCommand cmd, string sql, object param = null, IList<IDataParameter> parameters = null)
        {
            return BaseExecute<int>(cmd, sql, param, parameters, _cmd =>
            {
                return _cmd.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// 执行非查询语句或者查询单个结果的语句
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="defaultValue">获取异常的默认返回值</param>
        /// <returns></returns>
        public static T ExecuteWithReader<T>(this IDbConnection con, string sql, object param = null, IList<IDataParameter> parameters = null, T defaultValue = default(T)) 
        {

            return ExecuteReader<T>(con, sql, param, parameters, reader =>
            {
                try
                {
                    var value = reader[0];
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            });
        }

        /// <summary>
        /// 执行非查询语句或者查询单个结果的语句
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="defaultValue">获取异常的默认返回值</param>
        /// <returns></returns>
        public static T ExecuteWithReader<T>(this IDbCommand cmd, string sql, object param = null, IList<IDataParameter> parameters = null, T defaultValue = default(T))
        {

            return ExecuteReader<T>(cmd, sql, param, parameters, reader =>
            {
                try
                {
                    var value = reader[0];
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            });
        }

        /// <summary>
        /// 查询语句
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IList<T> Query<T>(this IDbCommand cmd, string sql, object param = null, IList<IDataParameter> parameters = null)
        {
            return ExecuteReader<IList<T>>(cmd, sql, param, parameters, reader =>
            {
                var tempList = new List<T>();
                while (reader.Read())
                {
                    var obj = (T)Activator.CreateInstance(typeof(T));
                    SetObjectValue<T>(obj, reader);
                    tempList.Add(obj);
                }
                return tempList;
            });
        }

        /// <summary>
        /// 查询语句
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IList<T> Query<T>(this IDbConnection con, string sql, object param = null, IList<IDataParameter> parameters = null)
        {
            return ExecuteReader<IList<T>>(con, sql, param, parameters, reader =>
            {
                var tempList = new List<T>();
                while (reader.Read())
                {
                    var obj = (T)Activator.CreateInstance(typeof(T));
                    SetObjectValue<T>(obj, reader);
                    tempList.Add(obj);
                }
                return tempList;
            });
        }


        /// <summary>
        /// 执行reader查询
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="funcReader">reader扩展委托</param>
        /// <returns></returns>
        public static T ExecuteReader<T>(this IDbConnection con, string sql, object param, IList<IDataParameter> parameters, Func<IDataReader, T> funcReader)
        {
            return BaseExecute<T>(con, sql, param, parameters, cmd =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    return funcReader != null ? funcReader(reader) : default(T);
                }
            });
        }
        /// <summary>
        /// 执行reader查询
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="funcReader">reader扩展委托</param>
        /// <returns></returns>
        public static T ExecuteReader<T>(this IDbCommand cmd, string sql, object param, IList<IDataParameter> parameters, Func<IDataReader, T> funcReader)
        {
            return BaseExecute<T>(cmd, sql, param, parameters, _cmd =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    return funcReader != null ? funcReader(reader) : default(T);
                }
            });
        }

        /// <summary>
        /// dataset 结果集
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="funcDataSet">结果集查询委托</param>
        /// <returns></returns>
        public static T ExecuteDataSet<T>(this IDbConnection con, string sql, object param, IList<IDataParameter> parameters, DbDataAdapter dataAdapter, Func<DataSet, T> funcDataSet)
        {
            if (dataAdapter == null) throw new Exception("dataAdapter can not be null!");

            return BaseExecute<T>(con, sql, param, parameters, _cmd =>
            {
                ((IDbDataAdapter)dataAdapter).SelectCommand = _cmd;
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return funcDataSet != null ? funcDataSet(dataSet) : default(T);
            });
        }
        /// <summary>
        /// dataset 结果集
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="funcDataSet">结果集查询委托</param>
        /// <returns></returns>
        public static T ExecuteDataSet<T>(this IDbCommand cmd, string sql, object param, IList<IDataParameter> parameters, DbDataAdapter dataAdapter, Func<DataSet, T> funcDataSet)
        {
            if (dataAdapter == null) throw new Exception("dataAdapter can not be null!");

            return BaseExecute<T>(cmd, sql, param, parameters, _cmd =>
            {
                ((IDbDataAdapter)dataAdapter).SelectCommand = cmd;
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return funcDataSet != null ? funcDataSet(dataSet) : default(T);
            });
        }

        /// <summary>
        /// dataset 结果集
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="funcDataSet">结果集查询委托</param>
        /// <returns></returns>
        public static T ExecuteDataTable<T>(this IDbCommand cmd, string sql, object param, IList<IDataParameter> parameters, DbDataAdapter dataAdapter, Func<DataTable, T> funcDataTable)
        {
            return ExecuteDataSet<T>(cmd, sql, param, parameters, dataAdapter, dataSet =>
            {
                return funcDataTable != null ? funcDataTable(dataSet.Tables[0]) : default(T);

            });
        }
        /// <summary>
        /// dataset 结果集
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="funcDataSet">结果集查询委托</param>
        /// <returns></returns>
        public static T ExecuteDataTable<T>(this IDbConnection con, string sql, object param, IList<IDataParameter> parameters, DbDataAdapter dataAdapter, Func<DataTable, T> funcDataTable)
        {
            return ExecuteDataSet<T>(con, sql, param, parameters, dataAdapter, dataSet =>
            {
                return funcDataTable != null ? funcDataTable(dataSet.Tables[0]) : default(T);

            });
        }

        /// <summary>
        ///  datatable结果集
        /// </summary>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this IDbCommand cmd, string sql, DbDataAdapter dataAdapter, object param = null, IList<IDataParameter> parameters = null)
        {
            return ExecuteDataTable<DataTable>(cmd, sql, param, parameters, dataAdapter, table => table);
        }

        /// <summary>
        ///  datatable结果集
        /// </summary>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this IDbConnection con, string sql, DbDataAdapter dataAdapter, object param = null, IList<IDataParameter> parameters = null)
        {
            return ExecuteDataTable<DataTable>(con, sql, param, parameters, dataAdapter, table => table);
        }

        /// <summary>
        /// 基础查询方法
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="con">连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="funcCmd">command 委托</param>
        /// <returns></returns>
        public static T BaseExecute<T>(this IDbConnection con, string sql, object param, IList<IDataParameter> parameters, Func<IDbCommand, T> funcCmd)
        {
            /*
             * 如果用户先 open 再进来，我们不主动关闭链接，避免频繁打开链接（connection 有连接池控制，应该不会关这么快）
             * 如果用户进来没打开链接，由我们打开的链接，我们主动关闭
             */

            if (con.State != System.Data.ConnectionState.Open)
            {
                using (con)
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        return cmd.BaseExecute<T>(sql, param, parameters, funcCmd);
                    }
                }
            }
            else
            {
                using (var cmd = con.CreateCommand())
                {
                    return cmd.BaseExecute<T>(sql, param, parameters, funcCmd);
                }
            }
            
        }
        /// <summary>
        /// 查询执行的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="parameters"></param>
        /// <param name="funcCmd"></param>
        /// <returns></returns>
        private static T BaseExecute<T>(this IDbCommand cmd, string sql, object param, IList<IDataParameter> parameters, Func<IDbCommand, T> funcCmd)
        {
            cmd.CommandText = sql;
            if (param != null)
            {
                if(param is Dictionary<string,object>)
                {
                    ObjectToParameter(cmd.CreateParameter().GetType(), (Dictionary<string, object>)param, cmd.Parameters);
                }
                else
                {
                    ObjectToParameter(cmd.CreateParameter().GetType(), param, cmd.Parameters);
                }
            }
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }
            //cmd.Parameters.AddRange(paramArr.ToArray());

            var t = funcCmd != null ? funcCmd(cmd) : default(T);
            return t;
        }

        /// <summary>
        /// 事务执行
        /// </summary>
        /// <param name="con"></param>
        /// <param name="funcCmd">事务执行的命令，正常提交请返回 true，否则返回false </param>
        /// <param name="actRollbackCallback">回滚回调</param>
        public static void Transaction(this IDbConnection con, Func<IDbCommand, bool> funcCmd, Action actRollbackCallback = null)
        {
            using (con)
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = con.BeginTransaction();
                    if (funcCmd != null)
                    {
                        if (!funcCmd(cmd))
                        {
                            cmd.Transaction.Rollback();
                            if (actRollbackCallback != null) actRollbackCallback.Invoke();
                        }
                        else
                        {
                            /*
                             * 如果用户在 funcCmd 主动 commit 的话，这里提交就会异常
                             */
                            try
                            {
                                cmd.Transaction.Commit();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析reader结果集为 模型对象
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="obj">模型数据</param>
        /// <param name="reader">datareader</param>
        private static void SetObjectValue<T>(object obj, IDataReader reader)
        {
            //var type = typeof(T);
            //var properties = type.GetProperties();
            var properties = ModelCache<T>._Properties;
            foreach (PropertyInfo property in properties)
            {
                object value = null;
                try
                {
                    value = reader[property.Name];
                }
                catch (Exception)
                {
                }
                if (value == null || value is DBNull) continue;
                property.SetValue(obj, Convert.ChangeType(value, property.PropertyType), null);
            }
        }
        /// <summary>
        /// 参数对象转 参数集
        /// </summary>
        /// <param name="ptType"></param>
        /// <param name="param"></param>
        /// <param name="Parameters"></param>
        private static void ObjectToParameter(Type ptType, object param, IDataParameterCollection Parameters)
        {
            if (param == null) return;
            var type = param.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var key = string.Format("@{0}", property.Name);
                var value = property.GetValue(param, null) ?? "";
                var t = Activator.CreateInstance(ptType, new object[] { key, value });
                if (Parameters.Contains(key))
                    Parameters[key] = value;
                else
                    Parameters.Add(t);
            }
        }
        private static void ObjectToParameter(Type ptType, Dictionary<string,object> param, IDataParameterCollection Parameters)
        {
            foreach (var item in param)
            {
                var t = Activator.CreateInstance(ptType, new object[] { item.Key, item.Value });
                if (Parameters.Contains(item.Key))
                    Parameters[item.Key] = item.Value;
                else
                    Parameters.Add(t);
            }
        }

    }
}
