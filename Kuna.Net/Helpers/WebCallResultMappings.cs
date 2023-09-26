﻿using CryptoExchange.Net.Objects;
using System;

namespace Kuna.Net.Helpers
{
    public static class WebCallResultMappings
    {
        public static WebCallResult<TMap> Map<TMap, TOriginal>(WebCallResult<TOriginal> toMap, Func<WebCallResult<TOriginal>, TMap> mapFunc)
        {
            return new WebCallResult<TMap>(
                toMap.ResponseStatusCode,
                toMap.ResponseHeaders,
                toMap.ResponseTime,
                toMap.ResponseLength,
                toMap.OriginalData,
                toMap.RequestUrl,
                toMap.RequestBody,
                toMap.RequestMethod,
                toMap.ResponseHeaders,
                toMap ? mapFunc(toMap) : default,
                toMap.Error
                );
        }
    }
}
