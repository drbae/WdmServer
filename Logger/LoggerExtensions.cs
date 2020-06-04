using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace DrBAE.WdmServer.Logging
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> _indexPageRequested;
        private static readonly Action<ILogger, string, Exception?> _quoteAdded;
        private static readonly Action<ILogger, string, int, Exception?> _quoteDeleted;
        private static readonly Action<ILogger, int, Exception?> _quoteDeleteFailed;
        private static Func<ILogger, int, IDisposable> _allQuotesDeletedScope;

        static LoggerExtensions()
        {
            _indexPageRequested = LoggerMessage.Define(LogLevel.Information, new EventId(1, nameof(IndexPageRequested)), "GET request for Index page");
            _quoteAdded = LoggerMessage.Define<string>(LogLevel.Information, new EventId(2, nameof(QuoteAdded)), "Quote added (Quote = '{Quote}')");
            // Reserve EventId: 3 (QuoteModified)
            _quoteDeleted = LoggerMessage.Define<string, int>(LogLevel.Information, new EventId(4, nameof(QuoteDeleted)), "Quote deleted (Quote = '{Quote}' Id = {Id})");
            _quoteDeleteFailed = LoggerMessage.Define<int>(LogLevel.Error, new EventId(5, nameof(QuoteDeleteFailed)), "Quote delete failed (Id = {Id})");
            _allQuotesDeletedScope = LoggerMessage.DefineScope<int>("All quotes deleted (Count = {Count})");
        }

        public static void IndexPageRequested(this ILogger logger) => _indexPageRequested(logger, null);
        public static void QuoteAdded(this ILogger logger, string quote) => _quoteAdded(logger, quote, null);
        public static void QuoteModified(this ILogger logger, string id, string priorQuote, string newQuote) { /*Reserve for future feature*/ }
        public static void QuoteDeleted(this ILogger logger, string quote, int id) => _quoteDeleted(logger, quote, id, null);
        public static void QuoteDeleteFailed(this ILogger logger, int id, Exception ex) => _quoteDeleteFailed(logger, id, ex);
        public static IDisposable AllQuotesDeletedScope(this ILogger logger, int count) => _allQuotesDeletedScope(logger, count);

    }//class
}
