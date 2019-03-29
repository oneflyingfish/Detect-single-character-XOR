using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test4
{
    /// <summary>
    /// 存储一个字符串评分最高的一组可能
    /// </summary>
    class HignGrade
    {
        /// <summary>
        /// 默认构造函数
        /// inputString=""
        /// key=0
        /// </summary>
        public HignGrade() { }

        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key"></param>
        /// <param name="grade"></param>
        public HignGrade(string ciphertextString,string plaintext, int key,double grade)
        {
            this.CiphertextString = ciphertextString;
            this.Plaintext = plaintext;
            this.key = key;
            this.grade = grade;
        }

        /// <summary>
        /// 密文属性
        /// </summary>
        public string CiphertextString { get => ciphertextString; set => ciphertextString = value; }
        /// <summary>
        /// 明文属性
        /// </summary>
        public string Plaintext { get => plaintext; set => plaintext = value; }
        public int Key { get => key; set => key = value; }
        public double Grade { get => grade; set => grade = value; }

        private string ciphertextString = "";   //密文
        private string plaintext = "";          //明文
        private int key = 0;
        private double grade = 0.0;
    }
}
