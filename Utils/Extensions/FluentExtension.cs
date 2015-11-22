using System;

namespace Utils.Extensions
{
    public static class FluentExtension
    {
        public static TResult With<TInput, TResult>(this TInput self, Func<TInput, TResult> evaluator)
            where TInput : class
        {
            return self == null ? default(TResult) : evaluator(self);
        }

        public static TResult With<TInput, TResult>(this TInput self, Func<TInput, TResult> evaluator, TResult defValue)
            where TInput : class
        {
            return self == null ? defValue : evaluator(self);
        }

        public static TResult Return<TInput, TResult>(this TInput self, Func<TInput, TResult> evaluator,
                                                      TResult failureValue)
            where TInput : class
        {
            return self == null ? failureValue : evaluator(self);
        }

        public static TInput If<TInput>(this TInput self, Func<TInput, bool> evaluator)
            where TInput : class
        {
            return (self == null) ? null : (evaluator(self) ? self : null);
        }

        public static TInput Unless<TInput>(this TInput self, Func<TInput, bool> evaluator)
            where TInput : class
        {
            return (self == null) ? null : (evaluator(self) ? null : self);
        }

        public static TInput Do<TInput>(this TInput self, params Action<TInput>[] action) where TInput : class
        {
            if (self == null) return null;
            action.ForEach(a => a(self));
            return self;
        }
    }
}