using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS.Common;

/// <summary>
/// Interface for objects that can be copied.
/// </summary>
public interface ICopyable {
    /// <summary>
    /// Creates a copy of the object.
    /// </summary>
    public object Copy();
}

/// <summary>
/// Interface for objects that can be copied.
/// </summary>
/// <typeparam name="T">The type of object to copy.</typeparam>
public interface ICopyable<T> {
    /// <summary>
    /// Creates a copy of the object.
    /// </summary>
    public T Copy();
}