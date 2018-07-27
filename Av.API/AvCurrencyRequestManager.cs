// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;
using Av.API.Provider;
using Av.API.Data;

namespace Av.API
{

    using CurrencyRequestData = Tuple<CurrencyRequestType, string, string, Action<CurrencyRequestType, string, string, CurrencyExchangeRate>>;

    public class AvCurrencyRequestManager : AvRequestManager<CurrencyExchangeRate, CurrencyRequestData>
    {
        private AvCurrencyProvider _currencyProvider;

        public AvCurrencyRequestManager(AvCurrencyProvider currencyProvider)
        {
            _currencyProvider = currencyProvider;
        }

        public void Add(CurrencyRequestType requestType, string fromCurr, string toCurr,
            Action<CurrencyRequestType, string, string, CurrencyExchangeRate> action)
        {
            _requests.Enqueue(new CurrencyRequestData(requestType, fromCurr, toCurr, action));
        }

        protected override void Execute(CurrencyRequestData requestData)
        {
            var requestType = requestData.Item1;
            var fromCurrency = requestData.Item2;
            var toCurrency = requestData.Item3;
            var callback = requestData.Item4;

            switch (requestType)
            {
                case CurrencyRequestType.ExchangeRate:
                    callback(requestType, fromCurrency, toCurrency, _currencyProvider.RequestExchangeRate(fromCurrency, toCurrency));
                    break;
            }
        }

    }
}
