//
// System.Int64.cs
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
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
using System.Threading;

namespace System {
	
	[Serializable]
	[System.Runtime.InteropServices.ComVisible (true)]
	public struct Int64 : IFormattable, IConvertible, IComparable, IComparable<Int64>, IEquatable <Int64>, IArithmetic <Int64>
	{

		public const long MaxValue = 0x7fffffffffffffff;
		public const long MinValue = -9223372036854775808;
		
		internal long m_value;

		public int CompareTo (object value)
		{
			if (value == null)
				return 1;
			
			if (!(value is System.Int64))
				throw new ArgumentException (Locale.GetText ("Value is not a System.Int64"));

			long lValue = (long) value;

			if (m_value == lValue)
				return 0;

			return (m_value < lValue) ? -1 : 1;
		}

		public override bool Equals (object obj)
		{
			if (!(obj is System.Int64))
				return false;

			return ((long) obj) == m_value;
		}

		public override int GetHashCode ()
		{
			return (int)(m_value & 0xffffffff) ^ (int)(m_value >> 32);
		}

		public int CompareTo (long value)
		{
			if (m_value == value)
				return 0;
			if (m_value > value)
				return 1;
			else
				return -1;
		}

		public bool Equals (long obj)
		{
			return obj == m_value;
		}

		internal static bool Parse (string s, bool tryParse, out long result, out Exception exc)
		{
			long val = 0;
			int len;
			int i;
			int sign = 1;
			bool digits_seen = false;

			result = 0;
			exc = null;

			if (s == null) {
				if (!tryParse) 
					exc = new ArgumentNullException ("s");
				return false;
			}

			len = s.Length;

			char c;
			for (i = 0; i < len; i++){
				c = s [i];
				if (!Char.IsWhiteSpace (c))
					break;
			}
			
			if (i == len) {
				if (!tryParse)
					exc = Int32.GetFormatException ();
				return false;
			}

			c = s [i];
			if (c == '+')
				i++;
			else if (c == '-'){
				sign = -1;
				i++;
			}
			
			for (; i < len; i++){
				c = s [i];

				if (c >= '0' && c <= '9'){
					byte d = (byte) (c - '0');
						
					if (val > (MaxValue/10))
						goto overflow;
					
					if (val == (MaxValue/10)){
						if ((d > (MaxValue % 10)) && (sign == 1 || (d > ((MaxValue % 10) + 1))))
							goto overflow;
						if (sign == -1)
							val = (val * sign * 10) - d;
						else
							val = (val * 10) + d;

						if (Int32.ProcessTrailingWhitespace (tryParse, s, i + 1, ref exc)){
							result = val;
							return true;
						}
						goto overflow;
					} else 
						val = val * 10 + d;
					
					
					digits_seen = true;
				} else if (!Int32.ProcessTrailingWhitespace (tryParse, s, i, ref exc))
					return false;
					
			}
			if (!digits_seen) {
				if (!tryParse)
					exc = Int32.GetFormatException ();
				return false;
			}
			
			if (sign == -1)
				result = val * sign;
			else
				result = val;

			return true;

		overflow:
			if (!tryParse)
				exc = new OverflowException ("Value is too large");
			return false;
		}

		public static long Parse (string s, IFormatProvider provider)
		{
			return Parse (s, NumberStyles.Integer, provider);
		}

		public static long Parse (string s, NumberStyles style)
		{
			return Parse (s, style, null);
		}

		internal static bool Parse (string s, NumberStyles style, IFormatProvider fp, bool tryParse, out long result, out Exception exc)
		{
			result = 0;
			exc = null;

			if (s == null) {
				if (!tryParse)
					exc = new ArgumentNullException ("s");
				return false;
			}

			if (s.Length == 0) {
				if (!tryParse)
					exc = new FormatException ("Input string was not " + 
							"in the correct format: s.Length==0.");
				return false;
			}

			NumberFormatInfo nfi = null;
			if (fp != null) {
				Type typeNFI = typeof (System.Globalization.NumberFormatInfo);
				nfi = (NumberFormatInfo) fp.GetFormat (typeNFI);
			} 
			if (nfi == null)
				nfi = Thread.CurrentThread.CurrentCulture.NumberFormat;

			if (!Int32.CheckStyle (style, tryParse, ref exc))
				return false;

			bool AllowCurrencySymbol = (style & NumberStyles.AllowCurrencySymbol) != 0;
			bool AllowHexSpecifier = (style & NumberStyles.AllowHexSpecifier) != 0;
			bool AllowThousands = (style & NumberStyles.AllowThousands) != 0;
			bool AllowDecimalPoint = (style & NumberStyles.AllowDecimalPoint) != 0;
			bool AllowParentheses = (style & NumberStyles.AllowParentheses) != 0;
			bool AllowTrailingSign = (style & NumberStyles.AllowTrailingSign) != 0;
			bool AllowLeadingSign = (style & NumberStyles.AllowLeadingSign) != 0;
			bool AllowTrailingWhite = (style & NumberStyles.AllowTrailingWhite) != 0;
			bool AllowLeadingWhite = (style & NumberStyles.AllowLeadingWhite) != 0;

			int pos = 0;

			if (AllowLeadingWhite && !Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
				return false;

			bool foundOpenParentheses = false;
			bool negative = false;
			bool foundSign = false;
			bool foundCurrency = false;

			// Pre-number stuff
			if (AllowParentheses && s [pos] == '(') {
				foundOpenParentheses = true;
				foundSign = true;
				negative = true; // MS always make the number negative when there parentheses
						 // even when NumberFormatInfo.NumberNegativePattern != 0!!!
				pos++;
				if (AllowLeadingWhite && !Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
					return false;

				if (s.Substring (pos, nfi.NegativeSign.Length) == nfi.NegativeSign) {
					if (!tryParse)
						exc = new FormatException ("Input string was not in the correct " +
								"format: Has Negative Sign.");
					return false;
				}
				if (s.Substring (pos, nfi.PositiveSign.Length) == nfi.PositiveSign) {
					if (!tryParse)
						exc = new FormatException ("Input string was not in the correct " +
								"format: Has Positive Sign.");
					return false;
				}
			}

			if (AllowLeadingSign && !foundSign) {
				// Sign + Currency
				Int32.FindSign (ref pos, s, nfi, ref foundSign, ref negative);
				if (foundSign) {
					if (AllowLeadingWhite && !Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
						return false;
					if (AllowCurrencySymbol) {
						Int32.FindCurrency (ref pos, s, nfi,
								    ref foundCurrency);
						if (foundCurrency && AllowLeadingWhite && 
								!Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
							return false;
					}
				}
			}
			
			if (AllowCurrencySymbol && !foundCurrency) {
				// Currency + sign
				Int32.FindCurrency (ref pos, s, nfi, ref foundCurrency);
				if (foundCurrency) {
					if (AllowLeadingWhite && !Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
							return false;
					if (foundCurrency) {
						if (!foundSign && AllowLeadingSign) {
							Int32.FindSign (ref pos, s, nfi, ref foundSign,
									ref negative);
							if (foundSign && AllowLeadingWhite &&
								!Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
								return false;
						}
					}
				}
			}
			
			long number = 0;
			int nDigits = 0;
			bool decimalPointFound = false;
			int digitValue;
			char hexDigit;
				
			// Number stuff
			do {

				if (!Int32.ValidDigit (s [pos], AllowHexSpecifier)) {
					if (AllowThousands &&
					    (Int32.FindOther (ref pos, s, nfi.NumberGroupSeparator)
						|| Int32.FindOther (ref pos, s, nfi.CurrencyGroupSeparator)))
					    continue;
					else
					if (!decimalPointFound && AllowDecimalPoint &&
					    (Int32.FindOther (ref pos, s, nfi.NumberDecimalSeparator)
						|| Int32.FindOther (ref pos, s, nfi.CurrencyDecimalSeparator))) {
					    decimalPointFound = true;
					    continue;
					}

					break;
				}
				else if (AllowHexSpecifier) {
					nDigits++;
					hexDigit = s [pos++];
					if (Char.IsDigit (hexDigit))
						digitValue = (int) (hexDigit - '0');
					else if (Char.IsLower (hexDigit))
						digitValue = (int) (hexDigit - 'a' + 10);
					else
						digitValue = (int) (hexDigit - 'A' + 10);

					ulong unumber = (ulong)number;
					
					// IMPROVME: We could avoid catching OverflowException
					try {
						number = (long)checked(unumber * 16ul + (ulong)digitValue);
					} catch (OverflowException e){
						if (!tryParse)
							exc = e;
						return false;
					}
				}
				else if (decimalPointFound) {
					nDigits++;
					// Allows decimal point as long as it's only 
					// followed by zeroes.
					if (s [pos++] != '0') {
						if (!tryParse)
							exc = new OverflowException ("Value too large or too " +
									"small.");
						return false;
					}
				}
				else {
					nDigits++;

					try {
						// Calculations done as negative
						// (abs (MinValue) > abs (MaxValue))
						number = checked (
							number * 10 - 
							(long) (s [pos++] - '0')
							);
					} catch (OverflowException) {
						if (!tryParse)
							exc = new OverflowException ("Value too large or too " +
									"small.");
						return false;
					}
				}
			} while (pos < s.Length);

			// Post number stuff
			if (nDigits == 0) {
				if (!tryParse)
					exc = new FormatException ("Input string was not in the correct format: nDigits == 0.");
				return false;
			}

			if (AllowTrailingSign && !foundSign) {
				// Sign + Currency
				Int32.FindSign (ref pos, s, nfi, ref foundSign, ref negative);
				if (foundSign) {
					if (AllowTrailingWhite && !Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
						return false;
					if (AllowCurrencySymbol)
						Int32.FindCurrency (ref pos, s, nfi,
								    ref foundCurrency);
				}
			}
			
			if (AllowCurrencySymbol && !foundCurrency) {
				// Currency + sign
				if (nfi.CurrencyPositivePattern == 3 && s[pos++] != ' ')
					if (tryParse)
						return false;
					else
						throw new FormatException ("Input string was not in the correct format: no space between number and currency symbol.");

				Int32.FindCurrency (ref pos, s, nfi, ref foundCurrency);
				if (foundCurrency && pos < s.Length) {
					if (AllowTrailingWhite && !Int32.JumpOverWhite (ref pos, s, true, tryParse, ref exc))
						return false;
					if (!foundSign && AllowTrailingSign)
						Int32.FindSign (ref pos, s, nfi, ref foundSign,
								ref negative);
				}
			}
			
			if (AllowTrailingWhite && pos < s.Length && !Int32.JumpOverWhite (ref pos, s, false, tryParse, ref exc))
				return false;

			if (foundOpenParentheses) {
				if (pos >= s.Length || s [pos++] != ')') {
					if (!tryParse)
						exc = new FormatException ("Input string was not in the correct " +
								"format: No room for close parens.");
					return false;
				}
				if (AllowTrailingWhite && pos < s.Length && !Int32.JumpOverWhite (ref pos, s, false, tryParse, ref exc))
					return false;
			}

			if (pos < s.Length && s [pos] != '\u0000') {
				if (!tryParse)
					exc = new FormatException ("Input string was not in the correct format: Did not parse entire string. pos = " 
							+ pos + " s.Length = " + s.Length);
				return false;
			}

			
			if (!negative && !AllowHexSpecifier){
				try {
					number = checked (-number);
				} catch (OverflowException e){
					if (!tryParse)
						exc = e;
					return false;
				}
			}

			result = number;
			return true;
		}

		public static long Parse (string s) 
		{
			Exception exc;
			long res;

			if (!Parse (s, false, out res, out exc))
				throw exc;

			return res;
		}

		public static long Parse (string s, NumberStyles style, IFormatProvider provider) 
		{
			Exception exc;
			long res;

			if (!Parse (s, style, provider, false, out res, out exc))
				throw exc;

			return res;
		}

		public static bool TryParse (string s, out long result) 
		{
			Exception exc;
			if (!Parse (s, true, out result, out exc)) {
				result = 0;
				return false;
			}

			return true;
		}

		public static bool TryParse (string s, NumberStyles style, IFormatProvider provider, out long result) 
		{
			Exception exc;
			if (!Parse (s, style, provider, true, out result, out exc)) {
				result = 0;
				return false;
			}

			return true;
		}

		public override string ToString ()
		{
			return NumberFormatter.NumberToString (m_value, null);
		}

		public string ToString (IFormatProvider provider)
		{
			return NumberFormatter.NumberToString (m_value, provider);
		}

		public string ToString (string format)
		{
			return ToString (format, null);
		}

		public string ToString (string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString (format, m_value, provider);
		}

		// =========== IConvertible Methods =========== //

		public TypeCode GetTypeCode ()
		{
			return TypeCode.Int64;
		}

		bool IConvertible.ToBoolean (IFormatProvider provider)
		{
			return System.Convert.ToBoolean (m_value);
		}

		byte IConvertible.ToByte (IFormatProvider provider)
		{
			return System.Convert.ToByte (m_value);
		}

		char IConvertible.ToChar (IFormatProvider provider)
		{
			return System.Convert.ToChar (m_value);
		}

		DateTime IConvertible.ToDateTime (IFormatProvider provider)
		{
			return System.Convert.ToDateTime (m_value);
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

		object IConvertible.ToType (Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
				throw new ArgumentNullException ("targetType");
			return System.Convert.ToType (m_value, targetType, provider, false);
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
		
		Int64 IArithmetic<Int64>.Add (Int64 addend)
		{
			return m_value + addend;
		}
		
		Int64 IArithmetic<Int64>.Subtract (Int64 subtrahend)
		{
			return m_value - subtrahend;
		}
		
		Int64 IArithmetic<Int64>.Multiply (Int64 multiplier)
		{
			return m_value * multiplier;
		}
		
		Int64 IArithmetic<Int64>.Divide (Int64 divisor)
		{
			return m_value / divisor;
		}
		
		Int64 IArithmetic<Int64>.Negate ()
		{
			return -m_value;
		}
		
		Int64 IArithmetic<Int64>.Max (Int64 other)
		{
			return (Int64) Math.Max (m_value, other);
		}
		
		Int64 IArithmetic<Int64>.Min (Int64 other)
		{
			return (Int64) Math.Min (m_value, other);
		}
		
		Int64 IArithmetic<Int64>.Sqrt ()
		{
			return (Int64) Math.Sqrt (m_value);
		}
		
		Nullable<ArithmeticSign> IArithmetic<Int64>.Sign {
			get {
				if (m_value > 0)
					return ArithmeticSign.Positive;
				else if (m_value == 0)
					return ArithmeticSign.Zero;
				else
					return ArithmeticSign.Negative;
			}
		}
		
		Int64 IArithmetic<Int64>.MaxValue {
			get {
				return Int64.MaxValue;
			}
		}
		
		Int64 IArithmetic<Int64>.MinValue {
			get {
				return Int64.MinValue;
			}
		}
		
		Int64 IArithmetic<Int64>.Zero {
			get {
				return (Int64) 0;
			}
		}
		
		Int64 IArithmetic<Int64>.One {
			get {
				return (Int64) 1;
			}
		}
		
		Boolean IArithmetic<Int64>.IsUnsigned {
			get {
				return false;
			}
		}
	}
}
