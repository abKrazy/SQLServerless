﻿using SQLServerless.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLServerless.SQLCore.Helpers
{
    internal static class QueryFactory
    {
        public static string GetChangeTrackingCurrentVersionQuery()
        {
            return "select CHANGE_TRACKING_CURRENT_VERSION()";
        }

        public static string GetChangesQuery(string tableName, string keyName, long trackingVersion)
        {
            return $"SELECT P.*, CT.{keyName}, CT.SYS_CHANGE_VERSION, CT.SYS_CHANGE_CREATION_VERSION, CT.SYS_CHANGE_COLUMNS, CT.SYS_CHANGE_CONTEXT, CT.SYS_CHANGE_OPERATION FROM CHANGETABLE(CHANGES {tableName}, {trackingVersion}) AS CT left join {tableName} AS P ON CT.{keyName} = P.{keyName}";
        }

        public static string GetInsertStatement(string tableName, TableRowData row)
        {
            // INSERT INTO Production.UnitMeasure (Name, UnitMeasureCode, ModifiedDate) VALUES (N'Square Yards', N'Y2', GETDATE());
            var strBuilder = new StringBuilder();
            strBuilder.Append($"INSERT INTO {tableName} (");
            for (int i = 0; i < row.Count; i++)
            {
                strBuilder.Append($"{row.Keys.ElementAt(i)}");
                if (i < row.Count - 1)
                    strBuilder.Append(", ");
            }
            strBuilder.Append(") VALUES (");

            for (int i = 0; i < row.Count; i++)
            {
                strBuilder.Append($"{GetSqlFormattedValue(row.Values.ElementAt(i))}");
                if (i < row.Count - 1)
                    strBuilder.Append(", ");
            }
            strBuilder.Append(") ");

            return strBuilder.ToString();
        }

        public static string GetInsertStatement(TableData table)
        {
            if (!table.Rows.Any())
                throw new ArgumentException(nameof(table.Rows));

            // INSERT INTO Production.UnitMeasure (Name, UnitMeasureCode, ModifiedDate) VALUES (N'Square Yards', N'Y2', GETDATE()),(N'Square Yards', N'Y2', GETDATE());
            var strBuilder = new StringBuilder();
            strBuilder.Append($"INSERT INTO {table.TableName} (");
            var firstRow = table.Rows[0];
            for (var i = 0; i < firstRow.Count; i++)
            {
                strBuilder.Append($"{firstRow.Keys.ElementAt(i)}");
                if (i < firstRow.Count - 1)
                    strBuilder.Append(", ");
            }
            strBuilder.Append(") VALUES");

            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                strBuilder.Append(" (");
                var row = table.Rows[rowIndex];
                for (int i = 0; i < row.Count; i++)
                {
                    strBuilder.Append($"{GetSqlFormattedValue(row.Values.ElementAt(i))}");
                    if (i < row.Count - 1)
                        strBuilder.Append(", ");
                }
                strBuilder.Append(") ");
                if (rowIndex < table.Rows.Count - 1)
                    strBuilder.Append(", ");
            }

            return strBuilder.ToString();
        }

        private static string GetSqlFormattedValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }

            Type valueType = value.GetType();

            if (valueType == typeof(string)
                || valueType == typeof(Guid))
            {
                return $"'{value}'";
            }

            if (valueType == typeof(DateTime))
            {
                return $"'{(DateTime)value:yyyy-MM-dd HH:mm:ss}'";
            }

            return value.ToString();
        }
    }
}
