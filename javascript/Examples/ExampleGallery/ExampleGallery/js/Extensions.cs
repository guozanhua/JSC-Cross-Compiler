﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using ScriptCoreLib.JavaScript.Runtime;

namespace ExampleGallery.js
{

    [Script]
    static class Extensions
    {
        public static Timer AtInterval(this int x, Action<Timer> h)
        {
            return new Timer(t => h(t), x, x);
        }

        public static Action ForEachAtInterval<T>(this IEnumerable<T> e, int interval, Action<T> h)
        {
            var x = e.GetEnumerator();

            var t = default(Timer);

            Action dispose = delegate
            {
                if (t != null)
                {
                    t.Stop();
                    t = null;
                }

                if (x != null)
                {
                    x.Dispose();
                    x = null;
                }
            };

            Action done = () => { };

            t = interval.AtInterval(
                delegate
                {
                    if (x.MoveNext())
                        h(x.Current);
                    else
                    {
                        dispose();

                        if (done != null)
                            done();
                    }
                }
            );

            return done;
        }

        public static Action Until(this int i, Func<Timer, bool> h)
        {
            Action done = () => { };

            new Timer(
                t =>
                {
                    if (h(t))
                    {
                        t.Stop();

                        done();
                    }
                }, i, i);

            return done;
        }

        public static int Random(this int i)
        {
            return new Random().Next(i);
        }
    }
}
