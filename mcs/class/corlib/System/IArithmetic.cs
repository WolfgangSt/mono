//
// System.IArithmetic.cs
//
// Author:
//   Alex Rønne Petersen <xtzgzorex@gmail.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System {
	[Serializable]
	public enum ArithmeticSign {
		Negative = -1,
		Zero = 0,
		Positive = 1,
	}
	
	[Serializable]
	public enum FloatingPointInfinity {
		NegativeInfinity = -1,
		PositiveInfinity = 1,
	}
	
	public interface IArithmetic <T> {
		
		// instance operations on the struct
		
		T Add(T addend);
		
		T Subtract(T subtrahend);
		
		T Multiply(T multiplier);
		
		T Divide(T divisor);
		
		T Negate();
		
		//T Remainder(T divisor);
		
		//T Abs();
		
		T Max(T other);
		
		T Min(T other);
		
		T Sqrt();
		
		Nullable<ArithmeticSign> Sign { get; }
		
		// static information about the type
		
		T MinValue { get; }
		
		T MaxValue { get; }
		
		T Zero { get; }
		
		T One { get; }
		
		bool IsUnsigned { get; }
	}
	
	public interface IFloatingPointArithmetic <T> : IArithmetic <T> {
		
		// instance operations on the struct
		
		bool IsNaN { get; }
		
		Nullable<FloatingPointInfinity> Infinity { get; }
		
		T Acos();
		
		T Asin();
		
		T Atan();
		
		T Atan2x(T x);
		
		T Atan2y(T y);
		
		T Ceiling();
		
		T Cos();
		
		T Cosh();
		
		T Exp();
		
		T Floor();
		
		T Log();
		
		T Log10();
		
		T Pow(T n);
		
		T Round(int digits, MidpointRounding mode);
		
		T Sin();
		
		T Sinh();
		
		T Tan();
		
		T Tanh();
		
		T Truncate();
	}
}
