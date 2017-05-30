

namespace LearningFoundation.Math.Random
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    

    public static class Generator
    {
        private static Random sourceRandom = new Random();

        private static readonly object sourceRandomLock = new Object();

        private static int? sourceSeed;
        private static long sourceLastUpdateTicks;
        private static readonly object sourceSeedLock = new Object();


        [ThreadStatic]
        private static long threadLastUpdateTicks;

        [ThreadStatic]
        private static bool threadOverriden;

        [ThreadStatic]
        private static int? threadSeed;

        [ThreadStatic]
        private static Random threadRandom;

        private static int GetRandomSeed()
        {

            lock (sourceRandomLock)
            {
                lock (sourceSeedLock)
                {
                    if (sourceRandom == null)
                    {

                        if (Generator.sourceSeed.HasValue)
                        {
                            if (Generator.sourceSeed.Value > 0)
                                return unchecked((int)(13 * Thread.CurrentThread.ManagedThreadId ^ Generator.sourceSeed.Value));

                            return Generator.sourceSeed.Value;
                        }
                        else
                        {
                            return unchecked((int)(13 * Thread.CurrentThread.ManagedThreadId ^ DateTime.Now.Ticks));
                        }
                    }
                    else
                    {
                        return sourceRandom.Next(); // We have a source random generator
                    }
                }
            }
        }

        public static long LastUpdateTicks
        {
            get { return sourceLastUpdateTicks; }
            
        }

        public static long ThreadLastUpdateTicks
        {
            get { return threadLastUpdateTicks; }
        }

        public static int? ThreadSeed
        {
            get { return threadSeed; }
            set
            {
                Generator.threadSeed = value;

                if (value.HasValue)
                {
                    Generator.threadOverriden = true;
                    Generator.threadLastUpdateTicks = DateTime.Now.Ticks;
                    Generator.threadRandom = (value.HasValue) ? new Random(threadSeed.Value) : new Random();
                }
                else
                {
                    Generator.threadRandom = null;
                }
            }
        }

        public static Random Random
        {
            get
            {
                if (Generator.threadOverriden)
                    return threadRandom;

                // No possibility of race condition here since its thread static
                if (Generator.threadRandom == null || Generator.threadLastUpdateTicks < Generator.sourceLastUpdateTicks)
                {
                    Generator.threadSeed = GetRandomSeed();
                    Generator.threadLastUpdateTicks = Generator.sourceLastUpdateTicks;
                    Generator.threadRandom = (Generator.threadSeed.HasValue) ?
                        new Random(threadSeed.Value) : new Random();
                }

                return threadRandom;
            }
        }

        public static int? Seed
        {
            get { return Generator.sourceSeed; }
            set
            {
                lock (sourceSeedLock)
                {
                    Generator.sourceSeed = value;

                    lock (sourceRandomLock)
                    {
                        Generator.sourceLastUpdateTicks = DateTime.Now.Ticks;

                        if (value.HasValue)
                        {
                            if (value.Value <= 0)
                            {

                                Generator.sourceRandom = null;
                            }
                            else
                            {

                                Generator.sourceRandom = new Random(value.Value);
                            }
                        }
                        else
                        {
                            int s = unchecked((int)(13 * Thread.CurrentThread.ManagedThreadId ^ Generator.sourceLastUpdateTicks));
                            Generator.sourceRandom = new Random(s);
                        }
                    }
                }
            }
        }
    }
}


