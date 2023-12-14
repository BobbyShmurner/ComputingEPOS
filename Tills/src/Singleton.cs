using System;
using System.Collections;
using System.Threading;

public abstract class Singleton<T> where T : class {
	public static T Instance => LazyInstance.Value;

	private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateInstanceOfT, LazyThreadSafetyMode.ExecutionAndPublication);
	private static T CreateInstanceOfT() => (T)Activator.CreateInstance(typeof(T), true)!;
}