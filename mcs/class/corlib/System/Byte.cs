//
// System.Byte.cs
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
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

using System.Globalization;

namespace System
{
	[Serializable]
	[System.Runtime.InteropServices.ComVisible (true)]
	public struct Byte : IFormattable, IConvertible, IComparable, IComparable<Byte>, IEquatable <Byte>, IArithmetic <Byte>
	{
		public const byte MinValue = 0;
		public const byte MaxValue = 255;

		internal byte m_value;

		public int CompareTo (object value)
		{
			if (value == null)
				return 1;

			if (!(value is System.Byte))
				throw new ArgumentException (Locale.GetText ("Value is not a System.Byte."));

			byte xv = (byte) value;

			if (m_value == xv)
				return 0;
			if (m_value > xv)
				return 1;
			else
				return -1;
		}

		public override bool Equals (object obj)
		{
			if (!(obj is System.Byte))
				return false;

			return ((byte) obj) == m_value;
		}

		public override int GetHashCode ()
		{
			return m_value;
		}

		public int CompareTo (byte value)
		{
			if (m_value == value)
				return 0;
			if (m_value > value)
				return 1;
			else
				return -1;
		}

		public bool Equals (byte obj)
		{
			return m_value == obj;
		}

		public static byte Parse (string s, IFormatProvider provider)
		{
			return Parse (s, NumberStyles.Integer, provider);
		}

		public static byte Parse (string s, NumberStyles style)
		{
			return Parse (s, style, null);
		}

		public static byte Parse (string s, NumberStyles style, IFormatProvider provider)
		{
			uint tmpResult = UInt32.Parse (s, style, provider);
			if (tmpResult > Byte.MaxValue)
				throw new OverflowException (Locale.GetText ("Value too large."));

			return (byte) tmpResult;
		}

		public static byte Parse (string s) 
		{
			return Parse (s, NumberStyles.Integer, null);
		}

		public static bool TryParse (string s, out byte result) 
		{
			return TryParse (s, NumberStyles.Integer, null, out result);
		}

		public static bool TryParse (string s, NumberStyles style, IFormatProvider provider, out byte result) 
		{
			uint tmpResult;
			result = 0;
			
			if (!UInt32.TryParse (s, style, provider, out tmpResult))
				return false;
				
			if (tmpResult > Byte.MaxValue)
				return false;
				
			result = (byte)tmpResult;
			return true;
		}

		public override string ToString ()
		{
			return NumberFormatter.NumberToString (m_value, null);
		}

		public string ToString (string format)
		{
			return ToString (format, null);
		}

		public string ToString (IFormatProvider provider)
		{
			return NumberFormatter.NumberToString (m_value, provider);
		}

		public string ToString (string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString (format, m_value, provider);
		}

		// =========== IConvertible Methods =========== //
		public TypeCode GetTypeCode ()
		{
			return TypeCode.Byte;
		}

		object IConvertible.ToType (Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
				throw new ArgumentNullException ("targetType");
			return System.Convert.ToType (m_value, targetType, provider, false);
		}

		bool IConvertible.ToBoolean (IFormatProvider provider)
		{
			return System.Convert.ToBoolean (m_value);
		}

		byte IConvertible.ToByte (IFormatProvider provider)
		{
			return m_value;
		}

		char IConvertible.ToChar (IFormatProvider provider)
		{
			return System.Convert.ToChar (m_value);
		}

		DateTime IConvertible.ToDateTime (IFormatProvider provider)
		{
			throw new InvalidCastException ();
		}

		decimal IConvertible.ToDecimal (IFormatProvider provider)
		{
			return System.Convert.ToDecimal (m_value);
		}

		double IConvertible.ToDouble (IFormatProvider provider)
		{
			return System.Convert.ToDouble (m_value);
		}

		short IConvertible.ToInt16 (IFormatProvider provider)
		{
			return System.Convert.ToInt16 (m_value);
		}

		int IConvertible.ToInt32 (IFormatProvider provider)
		{
			return System.Convert.ToInt32 (m_value);
		}

		long IConvertible.ToInt64 (IFormatProvider provider)
		{
			return System.Convert.ToInt64 (m_value);
		}

		sbyte IConvertible.ToSByte (IFormatProvider provider)
		{
			return System.Convert.ToSByte (m_value);
		}

		float IConvertible.ToSingle (IFormatProvider provider)
		{
			return System.Convert.ToSingle (m_value);
		}

		ushort IConvertible.ToUInt16 (IFormatProvider provider)
		{
			return System.Convert.ToUInt16 (m_value);
		}

		uint IConvertible.ToUInt32 (IFormatProvider provider)
		{
			return System.Convert.ToUInt32 (m_value);
		}

		ulong IConvertible.ToUInt64 (IFormatProvider provider)
		{
			return System.Convert.ToUInt64 (m_value);
		}
		
		Byte IArithmetic<Byte>.Add (Byte addend)
		{
			return (Byte)(m_value + addend);
		}
		
		Byte IArithmetic<Byte>.Subtract (Byte subtrahend)
		{
			return (Byte)(m_value - subtrahend);
		}
		
		Byte IArithmetic<Byte>.Multiply (Byte multiplier)
		{
			return (Byte)(m_value * multiplier);
		}
		
		Byte IArithmetic<Byte>.Divide (Byte divisor)
		{
			return (Byte)(m_value / divisor);
		}
		
		Byte IArithmetic<Byte>.Negate ()
		{
			return m_value;
		}
		
		Byte IArithmetic<Byte>.Max (Byte other)
		{
			return (Byte) Math.Max (m_value, other);
		}
		
		Byte IArithmetic<Byte>.Min (Byte other)
		{
			return (Byte) Math.Min (m_value, other);
		}
		
		Byte IArithmetic<Byte>.Sqrt ()
		{
			return (Byte) Math.Sqrt (m_value);
		}
		
		Nullable<ArithmeticSign> IArithmetic<Byte>.Sign {
			get {
				if (m_value > 0)
					return ArithmeticSign.Positive;
				else
					return ArithmeticSign.Zero;
			}
		}
		
		Byte IArithmetic<Byte>.MaxValue {
			get {
				return Byte.MaxValue;
			}
		}
		
		Byte IArithmetic<Byte>.MinValue {
			get {
				return Byte.MinValue;
			}
		}
		
		Byte IArithmetic<Byte>.Zero {
			get {
				return (Byte) 0;
			}
		}
		
		Byte IArithmetic<Byte>.One {
			get {
				return (Byte) 1;
			}
		}
		
		Boolean IArithmetic<Byte>.IsUnsigned {
			get {
				return true;
			}
		}
	}
}
