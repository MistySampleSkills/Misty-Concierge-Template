using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;
using Newtonsoft.Json;

namespace Misty.Services
{
    /// <summary>
    /// This service calls a stock API to search for the stock symbol using a company name and provide the current stock price for a specific stock symbol.
    /// </summary>
    public class MistyStockApi
    {
        private IRobotMessenger _misty;

        public MistyStockApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Call the stock quote api to get the current price of the stock.
        /// API Website:  
        /// https://www.alphavantage.co/documentation/
        /// https://medium.com/alpha-vantage/get-started-with-alpha-vantage-data-619a70c7f33a
        /// </summary>
        /// <param name="company">the name of the company for the quote</param>
        /// <returns></returns>
        public string GetStockQuoteInfo(string company)
        {
            string stockInfoText = "";
            try
            {
                // Call Stock Api to get the company symbol.
                string stockSymbol = GetStockSymbolInfo(company);
                // Stock Quote API Endpoint
                var stockQuoteUrl = MistyApiInfo.StockBaseUrl +
                    "/query" +
                    "?function=GLOBAL_QUOTE" +
                    "&symbol=" + stockSymbol +
                    "&apikey=" + MistyApiInfo.StockApiKey;

                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetStockQuoteInfo() -- stockQuoteUrl: {stockQuoteUrl}");
                var response = GetResponse(stockQuoteUrl);
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetStockQuoteInfo() -- jsonResp: {jsonResp.ToString(Formatting.Indented)}");
                // {
                //    "Global Quote": {
                //          "01. symbol": "MSFT",
                //          "02. open": "183.1700",
                //          "03. high": "183.5000",
                //          "04. low": "177.2500",
                //          "05. price": "178.5900",
                //          "06. volume": "48438140",
                //          "07. latest trading day": "2020-02-21",
                //          "08. previous close": "184.4200",
                //          "09. change": "-5.8300",
                //          "10. change percent": "-3.1613%"
                //    }
                //}
                string stockPrice = (string)jsonResp["Global Quote"]["05. price"];
                stockInfoText = $"The current price for company: {company} ... symbol: {stockSymbol} is {stockPrice}";
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetStockQuoteInfo() -- stockInfoText: {stockInfoText}");
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"MistyConcierge: IN GetStockQuoteInfo() => Exception", ex);
            }
            return stockInfoText;
        }

        /// <summary>
        /// Get the stock symbol for the company to get a stock quote.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public string GetStockSymbolInfo(string company)
        {
            string stockSymbol = "";
            try
            {
                // Stock Symbol Search API Endpoint
                var symbolSearchUrl = MistyApiInfo.StockBaseUrl +
                    "/query" +
                    "?function=SYMBOL_SEARCH" +
                    "&keywords=" + company +
                    "&apikey=" + MistyApiInfo.StockApiKey;

                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetStockSymbolInfo() -- symbolSearchUrl: {symbolSearchUrl}");
                var response = GetResponse(symbolSearchUrl);
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetStockSymbolInfo() -- jsonResp: {jsonResp.ToString(Formatting.Indented)}");
                // {
                //    "bestMatches": [
                //      {
                //          "1. symbol": "MSFT",
                //          "2. name": "Microsoft Corporation",
                //          "3. type": "Equity",
                //          "4. region": "United States",
                //          "5. marketOpen": "09:30",
                //          "6. marketClose": "16:00",
                //          "7. timezone": "UTC-05",
                //          "8. currency": "USD",
                //          "9. matchScore": "0.7826"
                //      },
                //      {
                //          "1. symbol": "MSFT.ARG",
                //          "2. name": "Microsoft Corporation",
                //          "3. type": "Equity",
                //          "4. region": "Argentina",
                //          "5. marketOpen": "11:00",
                //          "6. marketClose": "17:00",
                //          "7. timezone": "UTC-03",
                //          "8. currency": "ARS",
                //          "9. matchScore": "0.6154"
                //      },
                //      {
                //          "1. symbol": "MSF.DEX",
                //          "2. name": "Microsoft Corporation",
                //          "3. type": "Equity",
                //          "4. region": "XETRA",
                //          "5. marketOpen": "08:00",
                //          "6. marketClose": "20:00",
                //          "7. timezone": "UTC+01",
                //          "8. currency": "EUR",
                //          "9. matchScore": "0.6000"
                //      },
                //
                // Get the 1st match in the list.
                string stockCompanyName = (string)jsonResp["bestMatches"][0]["2. name"];
                stockSymbol = (string)jsonResp["bestMatches"][0]["1. symbol"];
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetStockSymbolInfo() -- stockSymbol: {stockSymbol}");
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN GetStockQuoteInfo() => Exception", ex);
            }
            return stockSymbol;
        }

        private string GetResponse(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return httpClient.GetStringAsync(new Uri(url)).Result;
            }
        }
    }
}
