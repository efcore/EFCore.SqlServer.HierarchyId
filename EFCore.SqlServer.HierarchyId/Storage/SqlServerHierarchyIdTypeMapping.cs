using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Microsoft.EntityFrameworkCore.SqlServer.Storage
{
    internal class SqlServerHierarchyIdTypeMapping : RelationalTypeMapping
    {
        private static readonly MethodInfo _getSqlBytes
            = typeof(SqlDataReader).GetRuntimeMethod(nameof(SqlDataReader.GetSqlBytes), new[] { typeof(int) });

        private static readonly MethodInfo _parseHierarchyId
            = typeof(HierarchyId).GetRuntimeMethod(nameof(HierarchyId.Parse), new[] { typeof(string) });

        private static readonly SqlServerHierarchyIdValueConverter _valueConverter = new SqlServerHierarchyIdValueConverter();

        private static Action<DbParameter, SqlDbType> _sqlDbTypeSetter;
        private static Action<DbParameter, string> _udtTypeNameSetter;


        public SqlServerHierarchyIdTypeMapping(string storeType, Type clrType)
            : base(CreateRelationalTypeMappingParameters(storeType, clrType))
        {}

        private static RelationalTypeMappingParameters CreateRelationalTypeMappingParameters(string storeType, Type clrType)
        {
            return new RelationalTypeMappingParameters(
                new CoreTypeMappingParameters(
                    clrType: clrType,
                    converter: null //this gets the generatecodeliteral to run
                ),
                storeType);
        }

        // needed to implement Clone
        protected SqlServerHierarchyIdTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {}

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        {
            return new SqlServerHierarchyIdTypeMapping(parameters);
        }

        /// <summary>
        ///     Configures type information of a <see cref="DbParameter" />.
        /// </summary>
        /// <param name="parameter"> The parameter to be configured. </param>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            var type = parameter.GetType();
            LazyInitializer.EnsureInitialized(ref _sqlDbTypeSetter, () => CreateSqlDbTypeAccessor(type));
            LazyInitializer.EnsureInitialized(ref _udtTypeNameSetter, () => CreateUdtTypeNameAccessor(type));

            if (parameter.Value == DBNull.Value)
            {
                parameter.Value = SqlBytes.Null;
            }

            _sqlDbTypeSetter(parameter, SqlDbType.Udt);
            _udtTypeNameSetter(parameter, StoreType);
        }

        /// <summary>
        ///     The method to use when reading values of the given type. The method must be defined
        ///     on <see cref="DbDataReader" /> or one of its subclasses.
        /// </summary>
        /// <returns> The method to use to read the value. </returns>
        public override MethodInfo GetDataReaderMethod()
        {
            return _getSqlBytes;
        }

        /// <summary>
        ///     Creates a an expression tree that can be used to generate code for the literal value.
        ///     Currently, only very basic expressions such as constructor calls and factory methods taking
        ///     simple constants are supported.
        /// </summary>
        /// <param name="value"> The value for which a literal is needed. </param>
        /// <returns> An expression tree that can be used to generate code for the literal value. </returns>
        public override Expression GenerateCodeLiteral(object value)
        {
            return Expression.Call(
                _parseHierarchyId,
                Expression.Constant(value.ToString(), typeof(string))
            );
        }

        /// <summary>
        ///     Generates the SQL representation of a non-null literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral(object value)
        {
            //this appears to only be called when using the update-database
            //command, and the value is already a hierarchyid
            
            if (value is HierarchyId)
                return $"'{value}'";

            //not sure how sqlbytes would be passed here, but it's here
            //in case it ever is
            value = _valueConverter.ConvertFromProvider(value);
            return $"'{value}'";
        }

        /// <summary>
        ///     Creates a <see cref="DbParameter" /> with the appropriate type information configured.
        /// </summary>
        /// <param name="command"> The command the parameter should be created on. </param>
        /// <param name="name"> The name of the parameter. </param>
        /// <param name="value"> The value to be assigned to the parameter. </param>
        /// <param name="nullable"> A value indicating whether the parameter should be a nullable type. </param>
        /// <returns> The newly created parameter. </returns>
        public override DbParameter CreateParameter(DbCommand command, string name, object value, bool? nullable = null)
        {
            var parameter = command.CreateParameter();
            parameter.Direction = ParameterDirection.Input;
            parameter.ParameterName = name;

            parameter.Value = value == null
                ? DBNull.Value
                : _valueConverter.ConvertToProvider(value);

            if (nullable.HasValue)
            {
                parameter.IsNullable = nullable.Value;
            }

            ConfigureParameter(parameter);

            return parameter;
        }

        /// <summary>
        ///     Gets a custom expression tree for the code to convert from the database value
        ///     to the model value.
        /// </summary>
        /// <param name="expression"> The input expression, containing the database value. </param>
        /// <returns> The expression with conversion added. </returns>
        public override Expression CustomizeDataReaderExpression(Expression expression)
        {
            //because _getSqlBytes is specified as the datareader method, 
            //the value will need to be converted from sqlbytes a hierarchyid
            return ReplacingExpressionVisitor.Replace(
                _valueConverter.ConvertFromProviderExpression.Parameters.Single(),
                expression,
                _valueConverter.ConvertFromProviderExpression.Body);
        }

        private static Action<DbParameter, SqlDbType> CreateSqlDbTypeAccessor(Type paramType)
        {
            var paramParam = Expression.Parameter(typeof(DbParameter), "parameter");
            var valueParam = Expression.Parameter(typeof(SqlDbType), "value");

            return Expression.Lambda<Action<DbParameter, SqlDbType>>(
                Expression.Call(
                    Expression.Convert(paramParam, paramType),
                    paramType.GetProperty("SqlDbType").SetMethod,
                    valueParam),
                paramParam,
                valueParam).Compile();
        }

        private static Action<DbParameter, string> CreateUdtTypeNameAccessor(Type paramType)
        {
            var paramParam = Expression.Parameter(typeof(DbParameter), "parameter");
            var valueParam = Expression.Parameter(typeof(string), "value");

            return Expression.Lambda<Action<DbParameter, string>>(
                Expression.Call(
                    Expression.Convert(paramParam, paramType),
                    paramType.GetProperty("UdtTypeName").SetMethod,
                    valueParam),
                paramParam,
                valueParam).Compile();
        }
    }
}
