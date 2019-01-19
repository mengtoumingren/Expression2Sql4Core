using Expression2Sql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Expression2Sql
{
    /// <summary>
    /// 模型缓存，第一次用到某个模型时进行属性缓存，下次再用到时将不再重新反射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelCache<T>
    {

        /// <summary>
        /// 插入sql
        /// </summary>
        public static string _AddSql = null;
        /// <summary>
        /// 更新sql
        /// </summary>
        public static string _UpdateSql = null;
        /// <summary>
        /// 删除sql
        /// </summary>
        public static string _DeleteSql = null;
        /// <summary>
        /// 查询sql
        /// </summary>
        public static string _FindSql = null;
        /// <summary>
        /// 获取所有列表的sql
        /// </summary>
        public static string _GetAllSql = null;
        /// <summary>
        /// 选择的字段，只列出字段
        /// </summary>
        public static string _SelectFields = null;

        /// <summary>
        /// 属性和真实字段映射
        /// </summary>
        public static Dictionary<string, string> _DicPropertyField = null;
        /// <summary>
        /// 真实字段和属性映射
        /// </summary>
        public static Dictionary<string, string> _DicFieldProperty = null;
        /// <summary>
        /// 主键 默认为 ：ID
        /// </summary>
        public static string _PrimaryKey = "ID".ToUpper();
        public static string _TableName = null;
        /// <summary>
        /// 属性列表
        /// </summary>
        public static PropertyInfo[] _Properties = null;

        public static Type _TType  = null;

        static ModelCache()
        {
            //初始化属性和映射字段
            GetPropertyField();
            //初始化默认 sql
            var type = typeof(T);
            _TType = type;
            //初始化表名称
            _TableName = type.Name;
            var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
            if (tableAttr != null)
            {
                _TableName = ((TableAttribute)tableAttr).Name;
            }

            InitSql(_TableName);
        }

        /// <summary>
        /// 初始化 基本sql
        /// </summary>
        private static void InitSql(string tableName)
        {
            var fieldNames = _DicPropertyField.Values.ToList();

            _AddSql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, string.Join(",", fieldNames.Where(s => !s.Equals(_PrimaryKey)).ToArray()), FieldNameToPropertyName(fieldNames.Where(s => !s.Equals(_PrimaryKey)).ToArray()));

            _UpdateSql = string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, FieldNameEqualPropertyName(fieldNames.Where(s => !s.Equals(_PrimaryKey)).ToArray()), FieldNameEqualPropertyName(_PrimaryKey));

            _DeleteSql = string.Format("DELETE FROM {0} WHERE {1}", tableName, FieldNameEqualPropertyName(_PrimaryKey));

            _FindSql = string.Format("SELECT {0} FROM {1} WHERE {2}", FieldNameAsPropertyName(fieldNames.ToArray()), tableName, FieldNameEqualPropertyName(_PrimaryKey));

            _GetAllSql = string.Format("SELECT {0} FROM {1}", FieldNameAsPropertyName(fieldNames.ToArray()), tableName);

            _SelectFields = FieldNameAsPropertyName(fieldNames.ToArray());

        }

        /// <summary>
        /// 获取属性和字段的映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void GetPropertyField()
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            _Properties = properties;
            _DicPropertyField = new Dictionary<string, string>();
            _DicFieldProperty = new Dictionary<string, string>();
            foreach (PropertyInfo property in properties)
            {
                //获取 field 别名
                var fieldAttr = property.GetCustomAttributes(typeof(FieldAttribute), false).FirstOrDefault();

                var field = property.Name;
                if (fieldAttr != null)
                {
                    field = ((FieldAttribute)fieldAttr).Name;
                }
                if (!_DicPropertyField.ContainsKey(property.Name.ToUpper()))
                {
                    _DicPropertyField.Add(property.Name.ToUpper(), field.ToUpper());
                }
                if (!_DicFieldProperty.ContainsKey(field.ToUpper()))
                {
                    _DicFieldProperty.Add(field.ToUpper(), property.Name.ToUpper());
                }
                //判断获取主键
                var identityAttr = property.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).FirstOrDefault();
                if (identityAttr != null) _PrimaryKey = _DicPropertyField[property.Name.ToUpper()];

            }
        }


        /// <summary>
        /// 获取真实字段并 as 为属性
        /// </summary>
        /// <param name="funcResult">接收两个参数：field , field 对应的 property,返回新的字符</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        public static string FieldNameToPropertyName(Func<string, string, string> funcResult, params string[] fieldNames)
        {
            var result = new List<string>();
            foreach (var field in fieldNames)
            {
                if (string.IsNullOrEmpty(field)) continue;
                var tempField = field.ToUpper().Trim();
                if (_DicFieldProperty.ContainsKey(tempField))
                {
                    result.Add(funcResult(tempField, _DicFieldProperty[tempField]));
                }
                else
                {
                    result.Add(tempField);
                }
            }
            return string.Join(",", result.ToArray());
        }

        /// <summary>
        /// 获取 字段 as model的属性名
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public static string FieldNameAsPropertyName(params string[] fieldNames)
        {
            return FieldNameToPropertyName((field, property) => string.Format("{0} as {1}", field, property), fieldNames);
        }

        /// <summary>
        /// 获取 字段 =@model的属性名
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public static string FieldNameEqualPropertyName(params string[] fieldNames)
        {
            return FieldNameToPropertyName((field, property) => string.Format("{0}=@{1}", field, property), fieldNames);
        }
        /// <summary>
        /// 获取 @model属性名
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public static string FieldNameToPropertyName(params string[] fieldNames)
        {
            return FieldNameToPropertyName((field, property) => string.Format("@{0}", property), fieldNames);
        }


        /// <summary>
        /// 获取属性对应的真实字段
        /// </summary>
        /// <param name="funcResult">接收两个参数：property , property 对应的 field,返回新的字符</param>
        /// <param name="propertyNames">属性名称</param>
        /// <returns></returns>
        public static string PropertyNameToFieldName(Func<string, string, string> funcResult, params string[] propertyNames)
        {
            var result = new List<string>();
            foreach (var propertyName in propertyNames)
            {
                var tempProperty = propertyName.ToUpper().Trim();
                if (_DicPropertyField.ContainsKey(tempProperty))
                {
                    result.Add(funcResult(tempProperty, _DicPropertyField[tempProperty]));
                }
                else
                {
                    result.Add(tempProperty);
                }
            }
            return string.Join(",", result.ToArray());
        }
    }
}
