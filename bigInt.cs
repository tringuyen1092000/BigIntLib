using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GCD
{
    // Class for big integer
    public class bigInt
    {
        // Attributes
        public String _bits { get; set; }
        public int _length { get; set; }
        public int _pointerIndex { get; set; }
        // Definitions
        public bigInt()
        {
            _length = 1;
            _bits = new String('0', 1);
            _pointerIndex = 0;
        }
        public bigInt(String str)
        {
            if (str.IndexOf('1') > -1)
            {
                _length = str.Substring(str.IndexOf('1')).Length;
                _bits = str.Substring(str.IndexOf('1'));
            }
            else
            {
                _bits = new String('0', 1);
                _length = 1;
            }
            _pointerIndex = 0;
        }
        public bigInt(int length)
        {
            _bits = new String('0', length);
            _length = length;
            _pointerIndex = 0;
        }
        
        // Behaviors
        // operator plus two big integer
        static public bigInt operator +(bigInt a, bigInt b)
        {
            a = standardize(a);
            b = standardize(b);
            int mem = 0, flag = 1;
            String aRev, bRev;
            if (a._bits.Length >= b._bits.Length) flag = 0;
            if (flag == 0)
            {
                aRev = Program.reverseString(a._bits);
                bRev = Program.reverseString(b._bits);
            }
            else
            {
                bRev = Program.reverseString(a._bits);
                aRev = Program.reverseString(b._bits);
            }
            String sum = "";
            // Make bRev have the same length as aRev
            bRev = bRev.PadRight(aRev.Length, '0');
            // OR every bits of aRev, bRev and mem
            for (int i = 0; i < aRev.Length; i++)
            {
                sum += ((aRev[i] - 48 + bRev[i] - 48 + mem) % 2).ToString();
                if (aRev[i] - 48 + bRev[i] - 48 + mem < 2) mem = 0;
                else mem = 1;
            }
            sum += mem.ToString();
            bigInt s = new bigInt(Program.reverseString(sum));
            return s;
        }
        // operator minus two big integer
        static public bigInt operator -(bigInt a, bigInt b)
        {
            a = standardize(a);
            b = standardize(b);
            if (a == b) return new bigInt();
            else if (a < b) throw new ArithmeticException("First parameter must equal or bigger than second parameter");
            else
            {
                int mem = 0, flag = 1;
                String aRev, bRev;
                if (a._bits.Length >= b._bits.Length) flag = 0;
                if (flag == 0)
                {
                    aRev = Program.reverseString(a._bits);
                    bRev = Program.reverseString(b._bits);
                }
                else
                {
                    bRev = Program.reverseString(a._bits);
                    aRev = Program.reverseString(b._bits);
                }
                String sum = "";
                // Make bRev have the same length as aRev
                bRev = bRev.PadRight(aRev.Length, '0');
                // OR every bits of aRev, bRev and mem
                for (int i = 0; i < aRev.Length; i++)
                {
                    int temp = (aRev[i] - 48) - (bRev[i] - 48) + mem;
                    if (temp < 0)
                    {
                        temp += 2;
                        mem = -1;
                    }
                    else mem = 0;
                    sum += (temp % 2).ToString();
                }
                bigInt s = new bigInt(Program.reverseString(sum));
                return s;
            }
        }
        static public bool operator >(bigInt a, bigInt b)
        {
            if (a == b) return false;
            else
            {
                if (!a.equalToZero() && !b.equalToZero())
                {
                    String abits = a._bits, bbits = b._bits;
                    if (abits.Length > bbits.Length)
                    {
                        bbits = bbits.PadLeft(abits.Length, '0');
                    }
                    else if (abits.Length < bbits.Length)
                    {
                        abits = abits.PadLeft(bbits.Length, '0');
                    }
                    if (abits.IndexOf('1') < bbits.IndexOf('1')) return true;
                    else if (abits.IndexOf('1') > bbits.IndexOf('1')) return false;
                    else
                    {
                        for (int i = abits.IndexOf('1'); i < abits.Length; i++)
                        {
                            if (abits[i] > bbits[i]) return true;
                            else if (abits[i] < bbits[i]) return false;
                        }
                        return false;
                    }
                }
                else if (a.equalToZero() && !b.equalToZero()) return false;
                else return true;
            }
        }
        static public bool operator ==(bigInt a, bigInt b)
        {
            if (a._length != b._length) return false;
            else
            {
                for (int i = 0; i < a._length; i++)
                {
                    if (a._bits[i] != b._bits[i]) return false;
                }
                return true;
            }
        }
        static public bool operator !=(bigInt a, bigInt b)
        {
            if (a == b) return false;
            else return true;
        }
        static public bool operator <(bigInt a, bigInt b)
        {
            if (a > b || a == b) return false;
            else return true;
        }
        static public bigInt operator <<(bigInt a, int n)
        {
            String bits = "";
            if (a._bits.IndexOf('1') < n)
            {
                bits = a._bits.Substring(a._bits.IndexOf('1'));
                bits = bits.PadRight(bits.Length + n - a._bits.IndexOf('1'), '0');
            }
            else
            {
                bits = a._bits.Substring(n);
                bits = bits.PadRight(bits.Length + n, '0');
            }
            bigInt result = new bigInt(bits);
            return result;
        }
        static public bigInt operator >>(bigInt a, int n)
        {
            bigInt result;
            String bits = "";
            if (n < a._length)
            {
                bits = a._bits.Substring(0, a._length - n);
                bits = bits.PadLeft(a._length, '0');
                result = new bigInt(bits);
            }
            else result = new bigInt(bits.Length);
            return result;
        }
        static public bigInt operator %(bigInt x, bigInt n)
        {
            if (n.equalToZero()) throw new ArithmeticException("Cannot module by zero!");
            else
            {
                x = standardize(x);
                n = standardize(n);
                bigInt one = new bigInt("1");
                bigInt zero = new bigInt("0");
                if (x == n) return zero;
                else
                {
                    bigInt a, b, c;
                    bigInt i = one;
                    while (x > n || x == n)
                    {
                        if (n._length > x._length / 2) x = x - n;
                        else
                        {
                            int q = n._length;
                            String temp = "";
                            for (int j = 0; j < n._length; j++)
                                temp += ('1' - n._bits[j]);
                            c = new bigInt(temp);
                            c = c + one;
                            b = x >> q;
                            a = new bigInt(x._bits.Substring(x._length - q));
                            bigInt bvalue = b;
                            while (i < c)
                            {
                                b = b + bvalue;
                                i = i + one;
                            }
                            if (b.equalToZero()) x = x - n;
                            else x = a + b;
                            i = one;
                        }
                    }
                }
                return x;
            }
            
        }
        static public bigInt standardize(bigInt a)
        {
            bigInt zero = new bigInt("0");
            if (a._bits.IndexOf('1') != 0)
            {
                if (a._bits.IndexOf('1') != -1)
                {
                    String bits = a._bits.Substring(a._bits.IndexOf('1'));
                    bigInt result = new bigInt(bits);
                    return result;
                }
                else return zero;
            }
            else return a;
        }
        public bool equalToZero()
        {
            if (_bits.IndexOf('1') == -1) return true;
            else return false;
        }
        //public override bool Equals(object o)  
        //{  
        //    return true;  
        //}
        //public override int GetHashCode()
        //{
        //    return 0;
        //}
    }

    // Singleton class for random
    public class MyRandom
    {
        static private Random _rng = null;
        int value;
        public MyRandom()
        {
            if (_rng == null) _rng = new Random();
        }
        public MyRandom(int max)
        {
            if (_rng == null) _rng = new Random(max);
        }
        public int Next()
        {
            value = _rng.Next();
            return value;
        }
        public int Next(int max)
        {
            value = _rng.Next(max);
            return value;
        }
        public int Next(int min, int max)
        {
            value = _rng.Next(min, max);
            return value;
        }
    }
}