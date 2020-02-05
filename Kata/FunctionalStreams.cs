using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    /*
        A Stream is an infinite sequence of items. It is defined recursively
        as a head item followed by the tail, which is another stream.
        Consequently, the tail has to be wrapped with Lazy to prevent
        evaluation.
    */
    public class Stream<T>
    {
        public readonly T Head;
        public readonly Lazy<Stream<T>> Tail;

        public Stream(T head, Lazy<Stream<T>> tail)
        {
            Head = head;
            Tail = tail;
        }
    }

    public static class Stream
    {
        /*
            Your first task is to define a utility function which constructs a
            Stream given a head and a function returning a tail.
        */
        public static Stream<T> Cons<T>(T h, Func<Stream<T>> t)
        {
            return new Stream<T>(h, new Lazy<Stream<T>>(t));
        }

        // .------------------------------.
        // | Static constructor functions |
        // '------------------------------'

        // Construct a stream by repeating a value.
        public static Stream<T> Repeat<T>(T x)
        {
            var stream = new Stream<T>(x, new Lazy<Stream<T>>(() => Repeat(x)));
            return stream;
        }

        // Construct a stream by repeatedly applying a function.
        public static Stream<T> Iterate<T>(Func<T, T> f, T x)
        {
            var stream = new Stream<T>(x, new Lazy<Stream<T>>(() => Iterate<T>(f, f(x))));
            return stream;
        }

        // Construct a stream by repeating an enumeration forever.
        public static Stream<T> Cycle<T>(IEnumerable<T> a, IEnumerable<T> b = null)
        {
            if (b == null || !b.Any())
                b = a;

            var stream = new Stream<T>(b.FirstOrDefault(), new Lazy<Stream<T>>(() => Cycle(a, b.Skip(1))));
            return stream;
        }

        // Construct a stream by counting numbers starting from a given one.
        public static Stream<int> From(int x)
        {
            return Iterate(n => n + 1, x);
        }

        // Same as From but count with a given step width.
        public static Stream<int> FromThen(int x, int d)
        {
            return Iterate(n => n + d, x);
        }

        // .------------------------------------------.
        // | Stream reduction and modification (pure) |
        // '------------------------------------------'

        /*
            Being applied to a stream (x1, x2, x3, ...), Foldr shall return
            f(x1, f(x2, f(x3, ...))). Foldr is a right-associative fold.
            Thus applications of f are nested to the right.
        */
        public static U Foldr<T, U>(this Stream<T> s, Func<T, Func<U>, U> f)
        {

            return f(s.Head, () => s.Tail.Value.Foldr(f));
        }

        // Filter stream with a predicate function.
        public static Stream<T> Filter<T>(this Stream<T> s, Predicate<T> p)
        {
            while (!p(s.Head))
                s = s.Tail.Value;

            return new Stream<T>(s.Head, new Lazy<Stream<T>>(() => Filter(s.Tail.Value, p)));
        }

        // Returns a given amount of elements from the stream.
        public static IEnumerable<T> Take<T>(this Stream<T> s, int n)
        {
            if (n > 0)
            {
                yield return s.Head;

                s = s.Tail.Value;
                for (var i = 1; i < n; ++i)
                {
                    yield return s.Head;
                    s = s.Tail.Value;
                }
            }
        }

        // Drop a given amount of elements from the stream.
        public static Stream<T> Drop<T>(this Stream<T> s, int n)
        {
            if (n == 0)
                return s;

            for (var i = 0; i < n; ++i)
                s = s.Tail.Value;

            return new Stream<T>(s.Head, new Lazy<Stream<T>>(() => s.Tail.Value));
        }

        // Combine 2 streams with a function.
        public static Stream<R> ZipWith<T, U, R>(this Stream<T> s, Func<T, U, R> f, Stream<U> other)
        {
            return new Stream<R>(f(s.Head, other.Head), new Lazy<Stream<R>>(() => s.Tail.Value.ZipWith(f, other.Tail.Value)));
        }

        // Map every value of the stream with a function, returning a new stream.
        public static Stream<U> FMap<T, U>(this Stream<T> s, Func<T, U> f)
        {
            return new Stream<U>(f(s.Head), new Lazy<Stream<U>>(() => s.Tail.Value.FMap(f)));
        }

        // Return the stream of all fibonacci numbers.
        public static Stream<int> Fib(int v0 = 0, int v1 = 0)
        {
            if (v0 == 0 && v1 == 0)
                return new Stream<int>(0, new Lazy<Stream<int>>(() => Fib(0, 1)));
            
            if (v0 == 0 && v1 == 1)
                return new Stream<int>(1, new Lazy<Stream<int>>(() => Fib(1, 0)));
            
            if (v0 == 1 && v1 == 0)
                return new Stream<int>(1,  new Lazy<Stream<int>>(() => Fib(1, 1)));
            
            return new Stream<int>(v0 + v1, new Lazy<Stream<int>>(() => Fib(v1, v0 + v1)));
        }

        // Return the stream of all prime numbers.
        public static Stream<int> Primes(IList<int> primesSoFar = null)
        {
            var head = 2;
            if (primesSoFar == null)
            {
                primesSoFar = new List<int> {head};
            }
            else if (primesSoFar.Count == 1)
            {
                head = 3;
                primesSoFar.Add(head);
            }
            else
            {
                var pp = primesSoFar.LastOrDefault() + 2;
                while (true)
                {
                    var prime = true;
                    for (var i = 1; primesSoFar[i] <= (int)Math.Sqrt(pp); ++i)
                        if (pp % primesSoFar[i] == 0)
                        {
                            prime = false;
                            break;
                        }

                    if (prime)
                    {
                        head = pp;
                        primesSoFar.Add(pp);
                        break;
                    }

                    pp += 2;
                }
            }

            return new Stream<int>(head, new Lazy<Stream<int>>(() => Primes(primesSoFar)));
        }
    }
}
