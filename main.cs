using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace test4
{
    class Program
    {
        /// <summary>
        /// 输入放在"input.txt"中
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //打开输入文件
            FileStream inputFile = null;
            try
            {
                inputFile = new FileStream("input.txt", FileMode.Open);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n请将输入放在同目录下的input.txt中\n");
                //窗口停顿
                Console.WriteLine("\n按任意键继续...");
                Console.ReadKey();
                return;
            }

            //开始处理读入的字符串
            string inputString;                                         //存储读入的字符串
            int lineNumber = 0;                                         //表示当前读取到第i行
            StreamReader streamReader = new StreamReader(inputFile);
            while (!streamReader.EndOfStream)                           //判断是否读到文件尾部
            {
                lineNumber++;                                           //行数加一
                inputString = streamReader.ReadLine();                  //读取一行

                if(inputString.Length!=60)                              //判断输入是否合法
                {
                    Console.WriteLine("\n第{0}行字符串：{1}\n长度不为60，不符合要求！！将被跳过\n",lineNumber,inputString);
                    continue;
                }

                ChooseHighestString(inputString);                       //读出此行字符串最有可能的明文以及对应秘钥
            }
            //打印结果
            printResult();
            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
        }

        /// <summary>
        /// 判断最终结果并打印到屏幕
        /// </summary>
        public static void printResult()
        {
            if(highGradeInStrings.Count<=0)
            {
                Console.WriteLine("没找到符合要求的明文，正确加密的字符串不存在，也可能是对精度要求太高\n");
                return;
            }

            double highestGrade=highGradeInStrings.Max(x => ((HignGrade)x).Grade); //求评分最高的组

            Console.WriteLine("明文极有可能以下几组中：");
            foreach(HignGrade i in highGradeInStrings)
            {
                if(i.Grade==highestGrade)
                {
                    Console.WriteLine("密文为：" + i.CiphertextString);
                    Console.WriteLine("秘钥为："+ Convert.ToChar(i.Key));
                    Console.WriteLine("明文对应为：" + i.Plaintext);
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// 将一个密文最可能对应的明文、秘钥以及评分输出到highGradeInStrings中
        /// </summary>
        /// <param name="inputString"></param>
        public static void ChooseHighestString(string inputString)
        {
            double highestGrade = -1.0;         //存储最高评分
            double grade = 0.0;                 //存储临时评分
            List<int> keys = new List<int>();   //存储最高评分下的key的ASSIC值

            for (int i = 0; i < 256; i++)
            {
                grade = CalculateGrade(inputString, i);
                if (grade > highestGrade)
                {
                    highestGrade = grade;
                }
            }
            //没有合适的明文
            if (highestGrade==0.0)               
            {
                return;
            }
            //通过字母频率筛选字符串
            for (int i = 0; i < 256; i++)       //字符串长度不足，精度不能要求太高
            {
                grade = CalculateGrade(inputString, i);
                if (grade >= highestGrade * accuracy)
                {
                    keys.Add(i);
                }
            }
            //存储最终可打印的字符串
            string resultString = "";       
            foreach (int key in keys)
            {
                //打印秘钥为key时的明文
                for (int i = 0; i < inputString.Length; i += 2)
                {
                    //累加每个字符的字符数*频率
                    resultString += GetCharInKey(inputString[i], inputString[i + 1], key);
                }
                HignGrade hignGrade = new HignGrade(inputString,resultString, key,highestGrade);    //生成节点
                highGradeInStrings.Add(hignGrade);
            }
        }

        /// <summary>
        /// 计算在秘钥为key的情况下，字符串inputString得分，包含欧美字母默认为0分
        /// </summary>
        /// <param name="testString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double CalculateGrade(string inputString, int key)
        {
            double grade = 0.0;
            char ch = '\0';
            char nextChar = 'a';//预测下一个字符
            for (int i = 0; i < inputString.Length; i += 2)
            {
                ch = GetCharInKey(inputString[i], inputString[i + 1], key);
                if ((i + 3) < inputString.Length)
                {
                    nextChar = GetCharInKey(inputString[i + 2], inputString[i + 3], key); //获取下一个字符
                }
                if (key == 'x')
                {

                }
                if (!JudgeChar(ch, nextChar))
                {
                    return 0.0;
                }
                //累加字符频率
                grade += GetLetterFrequency(ch);
            }
            return grade;
        }

        /// <summary>
        /// 判断一个字符出现在串中是否合法,考虑了部分控制字符可能造成退格
        /// </summary>
        /// <param name="ch"></param>
        public static bool JudgeChar(char ch, char nextChar)
        {
            //下一个字符为控制字符
            if (nextChar == 26 || nextChar == 127 || nextChar == 24 || (nextChar >= 0 && nextChar <= 5))
            {
                return true;
            }

            //下一个字符不为控制字符
            if ((ch >= 0 && ch <= 31) || (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || ch == 127 || (ch >= 'A' && ch <= 'Z') || ch == '-' || ch == ' ' || ch == '!' || ch == ',' || ch == '.' || ch == '\'' || ch == '\\' || ch == '/' || ch == ';' || ch == '?' || ch == '(' || ch == ')' || ch == '\n')
            {
                return true;
            }

            //当前字符不合法且写一个字符不为控制字符
            return false;
        }
        /// <summary>
        /// 判断hexChar1和hexChar2组成的字符，经过key加密后得到的新字符
        /// </summary>
        /// <param name="hexChar1"></param>
        /// <param name="hexChar2"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static char GetCharInKey(char hexChar1, char hexChar2, int key)
        {
            List<int> testChar = ConvertHexToBin(hexChar1);
            testChar.AddRange(ConvertHexToBin(hexChar2));

            int charValue = 0;
            //做异或运算
            for (int i = 7; i >= 0; i--)
            {
                charValue = charValue + (testChar[i] ^ (key % 2)) * Convert.ToInt32(Math.Pow(2, 7 - i));
                key /= 2;
            }

            return Convert.ToChar(charValue);
        }

        /// <summary>
        /// 获取英文字符频率的100倍
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static double GetLetterFrequency(char ch)
        {
            switch (ch)
            {
                case 'A': return 8.17;
                case 'a': return 8.17;
                case 'B': return 1.50;
                case 'b': return 1.50;
                case 'C': return 2.78;
                case 'c': return 2.78;
                case 'D': return 4.25;
                case 'd': return 4.25;
                case 'E': return 12.7;
                case 'e': return 12.7;
                case 'F': return 2.23;
                case 'f': return 2.23;
                case 'G': return 2.02;
                case 'g': return 2.02;
                case 'H': return 6.09;
                case 'h': return 6.09;
                case 'I': return 6.97;
                case 'i': return 6.97;
                case 'J': return 0.15;
                case 'j': return 0.15;
                case 'K': return 0.77;
                case 'k': return 0.77;
                case 'L': return 4.03;
                case 'l': return 4.03;
                case 'M': return 2.414;
                case 'm': return 2.414;
                case 'N': return 6.75;
                case 'n': return 6.75;
                case 'O': return 7.51;
                case 'o': return 7.51;
                case 'P': return 1.93;
                case 'p': return 1.93;
                case 'Q': return 0.10;
                case 'q': return 0.10;
                case 'R': return 5.99;
                case 'r': return 5.99;
                case 'S': return 6.33;
                case 's': return 6.33;
                case 'T': return 9.06;
                case 't': return 9.06;
                case 'U': return 2.76;
                case 'u': return 2.76;
                case 'V': return 0.98;
                case 'v': return 0.98;
                case 'W': return 2.36;
                case 'w': return 2.36;
                case 'X': return 0.15;
                case 'x': return 0.15;
                case 'Y': return 1.97;
                case 'y': return 1.97;
                case 'Z': return 0.07;
                case 'z': return 0.07;
                default: return 0.00;
            }
        }

        /// <summary>
        /// 将十六进制字符转化为0/1数组
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static List<int> ConvertHexToBin(char ch)
        {
            List<int> binaryString = new List<int>(); //用来存储结果
            int a = 0;  //十六进制字符的十进制表示

            //十六进制字符转化为十进制数字
            switch (ch)
            {
                case '0': a = 0; break;
                case '1': a = 1; break;
                case '2': a = 2; break;
                case '3': a = 3; break;
                case '4': a = 4; break;
                case '5': a = 5; break;
                case '6': a = 6; break;
                case '7': a = 7; break;
                case '8': a = 8; break;
                case '9': a = 9; break;
                case 'a': a = 10; break;
                case 'b': a = 11; break;
                case 'c': a = 12; break;
                case 'd': a = 13; break;
                case 'e': a = 14; break;
                case 'f': a = 15; break;
                default: a = 0; break;
            }

            //最终将0/1存入list中
            for (int i = 0; i < 4; i++)
            {
                binaryString.Insert(0, a % 2);
                a = a / 2;
            }
            return binaryString;
        }

        /// <summary>
        /// 存储各字符串中评分最高的组合
        /// </summary>
        public static List<HignGrade> highGradeInStrings = new List<HignGrade>();
        /// <summary>
        /// 判断要求精度
        /// </summary>
        public static double accuracy = 0.75;
    }
}
