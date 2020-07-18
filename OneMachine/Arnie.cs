using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace OneMachine
{
    class Arnie
    {
        private string _src;
        private bool _isPrint;
        Dictionary<string, string> _code;
        StreamReader _streamReader;

        /* 命令処理の配列 */
        private delegate string iFunction(string src);
        private struct iArray
        {
            public iFunction iFunc;
        }
        private iArray[] Exec;

        private enum E_FUNC{
            Nop,
            Succ,
            Decc,
            Sum,
            Prod,
            Div,
            Eq,
            Lt,
            Mod,
            Dem,
            Send,
            Neg,
            Ap,
            S,
            C,
            B,
            T,
            F,
            Pow2,
            I,
            Cons,
            Car,
            Cdr,
            Nil,
            IsNil,
            Lst,
            Vec,
            Draw,
            Checker,
            MDraw,
            MList,
            IsZero,
            Interact,
        }

        public Arnie(string src)
        {
            _src = src;
            _isPrint = false;
            _code = new Dictionary<string, string>();
            _streamReader = new StreamReader("./src/galaxy.txt");
            while (!_streamReader.EndOfStream)
            {
                string s = (string)_streamReader.ReadLine();
                string[] sub = s.Split('=');
                _code.Add(sub[0].Trim(), sub[1].Substring(1, sub[1].Length - 1));
            }

            /* 命令処理の初期化 */
            Exec = new iArray[33];
            Exec[0].iFunc = Nop;
            Exec[1].iFunc = Succ;
            Exec[2].iFunc = Decc;
            Exec[3].iFunc = Sum;
            Exec[4].iFunc = Prod;
            Exec[5].iFunc = Div;
            Exec[6].iFunc = Eq;
            Exec[7].iFunc = Lt;
            Exec[8].iFunc = Mod;
            Exec[9].iFunc = Dem;
            Exec[10].iFunc = Send;
            Exec[11].iFunc = Neg;
            Exec[12].iFunc = Ap;
            Exec[13].iFunc = S;
            Exec[14].iFunc = C;
            Exec[15].iFunc = B;
            Exec[16].iFunc = T;
            Exec[17].iFunc = F;
            Exec[18].iFunc = Pow2;
            Exec[19].iFunc = I;
            Exec[20].iFunc = Cons;
            Exec[21].iFunc = Car;
            Exec[22].iFunc = Cdr;
            Exec[23].iFunc = Nil;
            Exec[24].iFunc = IsNil;
            Exec[25].iFunc = Lst;
            Exec[26].iFunc = Vec;
            Exec[27].iFunc = Draw;
            Exec[28].iFunc = Checker;
            Exec[29].iFunc = MDraw;
            Exec[30].iFunc = MList;
            Exec[31].iFunc = IsZero;
            Exec[32].iFunc = Interact;
        }
        public void togglePrint()
        {
            _isPrint = !_isPrint;
        }

        public void exec(int cnt)
        {
            Console.WriteLine("\tstart: " + _src);
            for (int i = 0; i < cnt; i++)
            {
                _src = execOnce(_code["galaxy"]);
                Console.WriteLine("\t[" + i + "]: " + _src);
            }
        }
        private bool isNamed(string item){
            return (item[0] == ':');
        }
        private E_FUNC FuncSel(string item){
            E_FUNC eRet = E_FUNC.Nop;
            /* 名前付き関数のとき */
            if(isNamed(item)){
                return E_FUNC.Nop;
            }
            /* 数値のとき */
            int output;
            bool result = Int32.TryParse(item, out output);
            if(result){
                return E_FUNC.Nop;
            }
            /* 関数のとき */
            switch(item){
                case "inc":
                    eRet = E_FUNC.Succ;
                    break;
                case "dec":
                    eRet = E_FUNC.Decc;
                    break;
                case "add":
                    eRet = E_FUNC.Sum;
                    break;
                case "mul":
                    eRet = E_FUNC.Prod;
                    break;
                case "div":
                    eRet = E_FUNC.Div;
                    break;
                case "eq":
                    eRet = E_FUNC.Eq;
                    break;
                case "lt":
                    eRet = E_FUNC.Lt;
                    break;
                case "mod":
                    eRet = E_FUNC.Mod;
                    break;
                case "dem":
                    eRet = E_FUNC.Dem;
                    break;
                case "send":
                    eRet = E_FUNC.Send;
                    break;
                case "neg":
                    eRet = E_FUNC.Neg;
                    break;
                case "ap":
                    eRet = E_FUNC.Ap;
                    break;
                case "s":
                    eRet = E_FUNC.S;
                    break;
                case "c":
                    eRet = E_FUNC.C;
                    break;
                case "b":
                    eRet = E_FUNC.B;
                    break;
                case "t":
                    eRet = E_FUNC.T;
                    break;
                case "f":
                    eRet = E_FUNC.F;
                    break;
                case "pwr2":
                    eRet = E_FUNC.Pow2;
                    break;
                case "i":
                    eRet = E_FUNC.I;
                    break;
                case "cons":
                    eRet = E_FUNC.Cons;
                    break;
                case "car":
                    eRet = E_FUNC.Car;
                    break;
                case "cdr":
                    eRet = E_FUNC.Cdr;
                    break;
                case "nil":
                    eRet = E_FUNC.Nil;
                    break;
                case "isnil":
                    eRet = E_FUNC.IsNil;
                    break;
                case "vec":
                    eRet = E_FUNC.Vec;
                    break;
                case "draw":
                    eRet = E_FUNC.Draw;
                    break;
                case "checkerboard":
                    eRet = E_FUNC.Checker;
                    break;
                case "multipledraw":
                    eRet = E_FUNC.MDraw;
                    break;
                case "if0":
                    eRet = E_FUNC.IsZero;
                    break;
                case "interact":
                    eRet = E_FUNC.Interact;
                    break;
                case "default":
                    eRet = E_FUNC.Nop;
                    break;
            }
            return eRet;
        }
        private string execOnce(string src)
        {
            if (src == "")
            {
                return src;
            }

            dynamic d = src.Split(new char[] { ' ' }, 2);
            while(isNamed(d[0])){
                d[0] = _code[d[0]];
                if(2 == d.Length)
                {
                    d[0] = d[0] + d[1];
                }
                d = d[0].Split(new char[] { ' ' }, 2);
            }
            if(d.Length == 1){
                return src;
            }
            return Exec[(int)FuncSel(d[0])].iFunc(d[1]);
        }
        private string Nop(string src)
        {
            return src;
        }
        private string Ap(string src)
        {
            return execOnce(src);
        }
        private string Succ(string src){
            
            return src;
        }
        private string Decc(string src){
            return src;
        }
        private string Sum(string src){
            return src;
        }
        private string Prod(string src){
            return src;
        }
        private string Div(string src){
            return src;
        }
        private string Eq(string src){
            return src;
        }
        private string Lt(string src){
            return src;
        }
        private string Mod(string src){
            return src;
        }
        private string Dem(string src){
            return src;
        }
        private string Send(string src){
            return src;
        }
        private string Neg(string src){
            return src;
        }
        private string S(string src){
            return src;
        }
        private string C3(string s1, string s2, string s3)
        {
            return s1 + s3 + s2;
        }
        private Func<string,string> C2(string s1, string s2)
        {
            return (c) => C3(s1, s2, c);
        }
        private Func<string, Func<string, string>> C1(string s1)
        {
            return (c) => C2(s1, c);
        }
        private string C(string src){
            dynamic d = src.Split(new char[] { ' ' }, 2);
            if(d.Length == 1){
                return "c " + src;
            }
            string str1 = Exec[(int)FuncSel(d[0])].iFunc(d[1]);

            d = src.Split(new char[] { ' ' }, 2);
            if(d.Length == 1){
                return "c " + src;
            }
            string str2 = Exec[(int)FuncSel(d[0])].iFunc(d[1]);

            d = src.Split(new char[] { ' ' }, 2);
            if (d.Length != 1)
            {
                string str3 = Exec[(int)FuncSel(d[0])].iFunc(d[1]);
            }
            

            return src;
        }
        private string B(string src){
            return src;
        }
        private string T(string src){
            return src;
        }
        private string F(string src){
            return src;
        }
        private string Pow2(string src){
            return src;
        }
        private string I(string src){
            return src;
        }
        private string Cons(string src){
            return src;
        }
        private string Car(string src){
            return src;
        }
        private string Cdr(string src){
            return src;
        }
        private string Nil(string src){
            return src;
        }
        private string IsNil(string src){
            return src;
        }
        private string Lst(string src){
            return src;
        }
        private string Vec(string src){
            return src;
        }
        private string Draw(string src){
            return src;
        }
        private string Checker(string src){
            return src;
        }
        private string MDraw(string src){
            return src;
        }
        private string MList(string src){
            return src;
        }
        private string IsZero(string src){
            return src;
        }
        private string Interact(string src){
            return src;
        }
        public void import(string filepath)
        {
            StreamReader sr = new StreamReader(@filepath, Encoding.GetEncoding("UTF-8"));
            string _src = sr.ReadToEnd();
            sr.Close();
            _src = _src.Trim('\n', ' ', '\t', '\r');
        }
        public void export(string filepath)
        {
            StreamWriter sw = new StreamWriter(@filepath, false, Encoding.GetEncoding("UTF-8"));
            sw.Write(_src);
            sw.Close();
        }
        public void input(string src)
        {
            _src = src;
        }
        public void printHelp()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Version ver = asm.GetName().Version;
            Console.WriteLine("\t------------------------------");
            Console.WriteLine("\tarnie machine ver." + ver);
            Console.WriteLine("\t------------------------------");
            Console.WriteLine("\t[usage]");
            Console.WriteLine("\tOneMachine.exe <program string> or <file path>");
            Console.WriteLine("");

            Console.WriteLine("\t[program syntax]");
            Console.WriteLine("\tQx -> x");
            Console.WriteLine("\tCx -> yQy (x -> y)");
            Console.WriteLine("\tRx -> yy (x -> y)");
            Console.WriteLine("\tVx -> inverted y (x -> y)");
            Console.WriteLine("\tPx -> y inverted y (x -> y)");
            Console.WriteLine("\tMx -> roteted y (x -> y)");
            Console.WriteLine("");

            Console.WriteLine("\t[commands]");
            Console.WriteLine("\texec(e) (option)<execute count>: execute program");
            Console.WriteLine("\tinput(i) <program string>: input program(overwrite old program)");
            Console.WriteLine("\tread(r) <filepath>: read program from file(overwrite old program)");
            Console.WriteLine("\twrite(w) <filepath>: write program");
            Console.WriteLine("\tprint(p): toggle print level");
            Console.WriteLine("\tquit(q): quit machine");
            Console.WriteLine("\thelp(h): print this help");
            Console.WriteLine("");

            Console.WriteLine("\t[example]");
            Console.WriteLine("\tQC -> C");
            Console.WriteLine("\tCQC -> CQC");
            Console.WriteLine("\tCCQCC -> CCQCC Q CCQCC");
            Console.WriteLine("\tCCCQCCC -> CCCQCCC Q CCCQCCC Q CCCQCCC Q CCCQCCC");
            Console.WriteLine("\tCQΘC is x -> Θ(x) (Fixed point)");
        }
    }
}
