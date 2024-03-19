using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingEPOS; 

public interface ICopyable {
    public object Copy();
}

public interface ICopyable<T> {
    public T Copy();
}