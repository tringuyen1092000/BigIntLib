using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.IO;

namespace GCD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Du lieu lan luot cho x, y, p, n phai thoa dieu kien gia tri x, y < n va p tuy y!");
            Console.WriteLine("Neu khong thoa dieu kien tren qua trinh tinh toan se xay ra kha lau khi gap so lon!");
            Console.WriteLine("Lua chon cac cach tao du lieu:");
            Console.WriteLine("1. Lay du lieu tu file data.txt (moi dong tuong ung voi chuoi bit cua lan luot cac so: x, y, p, n).");
            Console.WriteLine("2. Tao du lieu ngau nhien cho tung so.");
            Console.Write("Nhap lua chon (1 hoac 2): ");
            int select = -1;
            select = Console.ReadKey().KeyChar;
            while (select != '1' && select != '2')
            {
                Console.Write("\nLua chon khong hop le! Vui long nhap lai: ");
                select = Console.ReadKey().KeyChar;
            }
            String str1 = "";
            String str2 = "";
            String str3 = "";
            String str4 = "";
            Console.WriteLine();

            if (select == '1')
            {
                //đọc dữ liệu từ file data.txt, các số x, y, p, n lần lượt là các dòng trong file data.txt
                string filePath = @"data.txt";
                string[] lines = ReadAllLines(filePath);
                if (lines == null)
                {
                    Console.ReadKey();
                    return;
                }
                int lineNum = 0;
                str1 = lines[lineNum];
                str2 = lines[lineNum + 1];
                str3 = lines[lineNum + 2];
                str4 = lines[lineNum + 3];
            }
            if (select == '2')
            {
                // bigIntGen(int n) là hàm tạo ra số nguyên ngẫu nhiên n bit
                Console.WriteLine("Nhap do dai chuoi nhi phan cua tung so: ");
                Console.Write("x.Length = ");
                int len = ReadInt();
                if (len != -1) str1 = new String(bigIntGen(len));
                Console.Write("y.Length = ");
                len = ReadInt();
                if (len != -1) str2 = new String(bigIntGen(len));
                Console.Write("p.Length = ");
                len = ReadInt();
                if (len != -1) str3 = new String(bigIntGen(len));
                Console.Write("n.Length = ");
                len = ReadInt();
                if (len != -1) str4 = new String(bigIntGen(len));
            }
            

            bigInt x = new bigInt(str1);
            bigInt y = new bigInt(str2);
            bigInt p = new bigInt(str3);
            bigInt n = new bigInt(str4);

            Console.WriteLine($"x = {x._bits}"); // x thuoc Zn
            Console.WriteLine($"y = {y._bits}"); // y thuoc Zn
            Console.WriteLine($"p = {p._bits}");
            Console.WriteLine($"n = {n._bits}");
            Console.WriteLine($"gcd (x, y) = {GCD(x, y)._bits}");
            Console.WriteLine($"x * y (mod n) = {mulMod(x, y, n)._bits}");
            Console.WriteLine($"x ^ p (mod n) = {powerMod(x, p, n)._bits}");
            Console.WriteLine("Done!");
            Console.ReadKey(true);
        }

        static public int ReadInt()
        {
            int value = 0;
            string str = Console.ReadLine();
            for (int i = 0; i < str.Length; i++)
            {
                value = value * 10 + (str[i] - 48);
                if (str[i] < 48 || str[i] > 57)
                {
                    Console.WriteLine("Nhap khong hop le");
                    value = -1;
                    break;
                }
            }
            return value;
        }

        static public string[] ReadAllLines(string filePath)
        {
            string[] data = {};
            if (System.IO.File.Exists(filePath))
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] != '1' && line[i] != '0')
                        {
                            Console.WriteLine("File khong hop le!");
                            break;
                        }
                    }
                }
                data = lines;
            }
            else
            {
                Console.WriteLine("File khong ton tai!");
                return null;
            }
            return data;
        }
        static bigInt GCD(bigInt a, bigInt b)
        {
            if (a.equalToZero() || b.equalToZero()) return a + b;
            string bits = "1";
            bigInt gcd = new bigInt(bits);
            while (a._bits[a._length - 1] == '0' && b._bits[b._length - 1] == '0')
            {
                a = a >> 1;
                b = b >> 1;
                gcd = gcd << 1;
            }
            while (!a.equalToZero())
            {
                while (a._bits[a._length - 1] == '0') a = a >> 1;
                while (b._bits[b._length - 1] == '0') b = b >> 1;
                bigInt t;
                if (a > b) t = a - b;
                else t = b - a;
                t = t >> 1;
                if (a > b || a == b) a = t;
                else b = t;
            }
            if (a._length > b._length) gcd = gcd >> b._length;
            else gcd = gcd >> a._length;
            gcd = bigInt.standardize(gcd);
            return gcd;
        }
        
        static bigInt addMod(bigInt a, bigInt b, bigInt n)
        {
            a = a % n;
            b = b % n;
            // a, b thuoc Zn
            if (a + b < n) return a + b;
            else return a + b - n;
        }

        static bigInt subMod(bigInt a, bigInt b, bigInt n)
        {
            a = a % n;
            b = b % n;
            // a, b thuoc Zn
            if (a < b) return a - b + n;
            else return a - n;
        }

        static bigInt mulMod(bigInt a, bigInt b, bigInt n)
        {
            bigInt result = new bigInt("0");
            a = a % n;
            b = b % n;
            // a, b thuoc Zn
            if (b._bits[b._length - 1] == '1') result = a;
            for (int i = 1; i < b._length; i++)
            {
                a = addMod(a, a, n);
                if (b._bits[b._length - 1 - i] == '1') result = addMod(result, a, n);
            }
            return result;
        }
        
        static bigInt powerMod(bigInt x, bigInt p, bigInt n)
        {
            //Return: result = (x ^ p) % n
            x = x % n;
            String bits = "1";
            bigInt result = new bigInt(bits);
            // if (p == 0) return 1;
            if (p.equalToZero()) return result;
            else
            {
                //Cach 1:
                //bigInt a = new bigInt(x._bits);
                //if (p._bits[p._length - 1] == '1') result = x;
                //for (int i = 1; i < p._length; i++)
                //{
                //    a = mulMod(a, a, n);
                //    if (p._bits[p._length - 1 - i] == '1') result = mulMod(result, a, n);
                //}

                //Cach 2
                for (int i = 0; i < p._length; i++)
                {
                    result = mulMod(result, result, n);
                    if (p._bits[i] == '1') result = mulMod(result, x, n);
                }    
            }
            result = bigInt.standardize(result);
            return result;
        }

        static void bigIntInput()
        {

        }

        // Return the Decimal value of the bigInt (use for bigInt <= 32 bits)
        static int value(bigInt num)
        {
            if (num._length > 32) throw new ArithmeticException("The number size is bigger than 32 bits!");
            int v = 0;
            String temp = Program.reverseString(num._bits);
            for (int i = 0; i < temp.Length; i++)
                v += ((temp[i] - 48) * (int)Math.Pow(2, i));
            return v;
        }

        static char[] bigIntGen(int bitLength)
        {
            char[] result = new char[bitLength];
            MyRandom rng = new MyRandom();
            for (int i = 0; i < bitLength; i++)
                result[i] = (char)rng.Next(48, 50);
            return result;
        }

        static int countTrueBits(bigInt n)
        {
            int count = 0;
            for (int i = 0; i < n._length; i++)
                if (n._bits[i] == '1') count++;
            return count;
        }

        static bool smallPrimeTest(int n)
        {
            if (n <= 0 || n == 1) return false;
            if (n == 2 || n == 3) return true;
            for (int i = 2; i <= Math.Sqrt(n); i++)
                if (n % i == 0) return false;
            return true;
        }

        static int smallPrimeGen(int n)
        {
            MyRandom _rng = new MyRandom();
            int p = _rng.Next(n);
            while (!smallPrimeTest(p))
            {
                if (p < n - 2)
                {
                    if (p % 2 == 0) p += 1;
                    else p += 2;
                }
                else p = _rng.Next(n);
            }
            return p;
        }

        //static bool primeTest(bigInt n) // n != 2
        //{
        //    int b = smallPrimeGen(value(n)), r = 0, m = 0;
        //    //
        //    Console.WriteLine($"b = {b}");
        //    Console.WriteLine($"n = {value(n)}");
        //    //
        //    if ((countTrueBits(n) == 1 && n._bits[n._length - 1] == '1') || countTrueBits(n) == 0) return false;
        //    if (countTrueBits(n) == 1 && n._bits[n._length - 2] == '1') return true;
        //    char[] temp = n._bits.ToArray();
        //    temp[n._length - 1] = '0';
        //    for (int i = n._length - 1; temp[i] != '1' && i > 0; i--)
        //        r++;
        //    //
        //    Console.WriteLine($"r = {r}");
        //    //
        //    String tmp = new String(temp.Reverse().ToArray());
        //    tmp = tmp.Substring(r, temp.Length - r);
        //    //
        //    Console.WriteLine($"tmp = {tmp}");
        //    //
        //    for (int j = 0; j < tmp.Length; j++)
        //        if (tmp[j] == '1') m += (int)Math.Pow(2, j);
        //    //
        //    Console.WriteLine($"m = {m}");
        //    //
        //    for (int i = 1; i <= r; i++)
        //    {
        //        //
        //        Console.WriteLine(Math.Pow(b, (m * Math.Pow(2, 0))) % value(n));
        //        //
        //        if (Math.Abs(Math.Pow(b, (m * Math.Pow(2, i))) % value(n)) != 1) return false;
        //    }
        //    return true;
        //}

        static public String reverseString(String str)
        {
            String revStr = new String(str.ToCharArray().Reverse().ToArray());
            return revStr;
        }
    }
}
