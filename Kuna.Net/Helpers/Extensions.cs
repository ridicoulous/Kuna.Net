using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryptoExchange.Net.CommonObjects;
using Kuna.Net.Objects.V4;
using Newtonsoft.Json;

namespace Kuna.Net.Helpers
{
    public static class Extensions
    {
        public static string AsStringParameterOrNull<T>(this List<T> source) => AsStringParameterOrNull(source.ToArray());

        public static string AsStringParameterOrNull<T>(this T[] source)
        {
            return (source.Length == 0) ? null : string.Join(",", source);
        }
        public static Dictionary<string, object> AsDictionary(this object source,
           BindingFlags bindingAttr = BindingFlags.FlattenHierarchy |
           BindingFlags.Instance |
           BindingFlags.NonPublic |
           BindingFlags.Public |
           BindingFlags.Static)
        {
            try
            {
                var result = new Dictionary<string, object>();
                var props = source.GetType().GetProperties(bindingAttr);
                foreach (var p in props)
                {
                    string key = p.Name;
                    if (p.IsDefined(typeof(JsonPropertyAttribute)))
                    {
                        key = p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName ?? p.Name;
                    }
                    object value = p.GetValue(source, null);

                    if (value == null)
                    {
                        continue;
                    }
                    if (value is bool)
                    {
                        value = value.ToString().ToLowerInvariant();
                    }
                    if (value is decimal || value is decimal?)
                    {
                        value = value as decimal?;
                    }
                    if (value.GetType().IsEnum)
                    {
                        value = value?.ToString();
                    }
                    if (value is DateTime || value is DateTime?)
                    {
                        value = ((DateTime)value).ToString("o", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (!result.ContainsKey(key) && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value.ToString()))
                    {
                        result.Add(key, value);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Order ConvertToCryptoExchangeOrder(this KunaOrderV4 source)
        {
            if (source is null)
                return null;

            return new()
            {
                SourceObject = source,
                Id = source.Id.ToString(),
                Price = source.Price,
                Quantity = source.Quantity,
                QuantityFilled = source.ExecutedQuantity,
                Side = source.Side == KunaOrderSideV4.Bid ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                Status = source.Status switch
                {
                    KunaOrderStatusV4.Canceled => CommonOrderStatus.Canceled,
                    KunaOrderStatusV4.Expired => CommonOrderStatus.Canceled,
                    KunaOrderStatusV4.Rejected => CommonOrderStatus.Canceled,
                    KunaOrderStatusV4.Closed => CommonOrderStatus.Filled,
                    KunaOrderStatusV4.Open => CommonOrderStatus.Active,
                    KunaOrderStatusV4.Pending => CommonOrderStatus.Active,
                    KunaOrderStatusV4.WaitStop => CommonOrderStatus.Active,
                    _ => CommonOrderStatus.Canceled
                },
                Symbol = source.Pair,
                Timestamp = source.UpdatedAt.DateTime,
                Type = source.Type switch
                {
                    KunaOrderTypeV4.Limit => CommonOrderType.Limit,
                    KunaOrderTypeV4.Market => CommonOrderType.Market,
                    _ => CommonOrderType.Other,
                }
            };
        }
    }
}