﻿
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using LearningFoundation;

public static class Generator
{

    // Random generator used to seed other generators. It is used to prevent generators 
    // that have been created in short time spans to be initialized with the same seed.
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
        // We initialize new Random objects using the next value from a global 
        // static shared random generator in order to avoid creating many random 
        // objects with the random seed. This guarantees reproducibility but does
        // not compromise the effectiveness of parallel methods that depends on 
        // the generation of true random sequences with different values.
        lock (sourceRandomLock)
        {
            lock (sourceSeedLock)
            {
                if (sourceRandom == null)
                {
                    // There is no source random generator. This means we need to initialize the 
                    // generator for the current thread with a value that is (almost) unpredictable, 
                    // but still different from threads being initialized at almost the same time.

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

    ///// <summary>
    /////   Gets an object that can be used to synchronize access to the generator.
    ///// </summary>
    ///// 
    //public static readonly object SyncObject = new Object();

    /// <summary>
    ///   Gets the timestamp for when the global random generator
    ///   was last changed (i.e. after setting <see cref="Seed"/>).
    /// </summary>
    /// 
    public static long LastUpdateTicks
    {
        get { return sourceLastUpdateTicks; }
    }

    /// <summary>
    ///   Gets the timestamp for when the thread random generator was last 
    ///   changed (i.e. after creating the first random generator in this 
    ///   thread context or by setting <see cref="ThreadSeed"/>).
    /// </summary>
    /// 
    public static long ThreadLastUpdateTicks
    {
        get { return threadLastUpdateTicks; }
    }

    /// <summary>
    ///   Gets or sets the seed for the current thread. Changing
    ///   this seed will not impact other threads or generators
    ///   that have already been created from this thread.
    /// </summary>
    /// 
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

    /// <summary>
    ///   Gets a reference to the random number generator used internally by 
    ///   the Accord.NET classes and methods. Objects retrieved from this property
    ///   should not be shared across threads. Instead, call this property from
    ///   each thread you would like to use a random generator for.
    /// </summary>
    /// 
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

    /// <summary>
    ///   Sets a random seed for the framework's main <see cref="Random">internal number 
    ///   generator</see>. Preferably, this method should be called <b>before</b> other 
    ///   computations. If set to a value less than or equal to zero, all generators will 
    ///   start with the same fixed seed, <b>even among multiple threads</b>. If set to any 
    ///   other value, the generators in other threads will start with fixed, but different, 
    ///   seeds.
    /// </summary>
    /// 
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
                            Trace.WriteLine("All threads will be initialized with the same seed: " + value);
                            Generator.sourceRandom = null;
                        }
                        else
                        {
                            Trace.WriteLine("All threads will be initialized with predictable, but random seeds.");
                            Generator.sourceRandom = new Random(value.Value);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("All threads will be initialized with unpredictable random seeds.");
                        int s = unchecked((int)(13 * Thread.CurrentThread.ManagedThreadId ^ Generator.sourceLastUpdateTicks));
                        Generator.sourceRandom = new Random(s);
                    }
                }
            }
        }
    }

}

