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
        private delegate string iFunction(ref string src);
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
                case "inc":/* 未使用 */
                    eRet = E_FUNC.Succ;
                    break;
                case "dec":/* 未使用 */
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
                case "mod":/* 未使用 */
                    eRet = E_FUNC.Mod;
                    break;
                case "dem":/* 未使用 */
                    eRet = E_FUNC.Dem;
                    break;
                case "send":/* 未使用 */
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
                case "pwr2":/* 未使用 */
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
                case "vec":/* 未使用 */
                    eRet = E_FUNC.Vec;
                    break;
                case "draw":/* 未使用 */
                    eRet = E_FUNC.Draw;
                    break;
                case "checkerboard":/* 未使用 */
                    eRet = E_FUNC.Checker;
                    break;
                case "multipledraw":/* 未使用 */
                    eRet = E_FUNC.MDraw;
                    break;
                case "if0":/* 未使用 */
                    eRet = E_FUNC.IsZero;
                    break;
                case "interact":/* 未使用 */
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
            src = d[1];
            return Exec[(int)FuncSel(d[0])].iFunc(ref src);
        }
        private string Nop(ref string src)
        {
            return src;
        }
        private string Ap(ref string src)
        {
            return execOnce(src);
        }
        private string Succ(ref string src){
            string s1 = getarg(ref src);
            try
            {
               int i = Int32.Parse(s1); 
               i++;
               s1 = i.ToString();
            }
            catch
            {
                Console.WriteLine("Succ Parse Error.");
            }
            return s1;
        }
        private string Decc(ref string src){
            string s1 = getarg(ref src);
            try
            {
               int i = Int32.Parse(s1); 
               i--;
               s1 = i.ToString();
            }
            catch
            {
                Console.WriteLine("Decc Parse Error.");
            }
            return s1;
        }
        private string Sum(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            try
            {
               int i1 = Int32.Parse(s1); 
               int i2 = Int32.Parse(s2); 

               s1 = (i1 + i2).ToString();
            }
            catch
            {
                Console.WriteLine("Sum Parse Error.");
            }
            return s1;
        }
        private string Prod(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            try
            {
               int i1 = Int32.Parse(s1); 
               int i2 = Int32.Parse(s2); 

               s1 = (i1 * i2).ToString();
            }
            catch
            {
                Console.WriteLine("Prod Parse Error.");
            }
            return s1;
        }
        private string Div(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            try
            {
               int i1 = Int32.Parse(s1); 
               int i2 = Int32.Parse(s2); 

               s1 = (i1 / i2).ToString();
            }
            catch
            {
                Console.WriteLine("Div Parse Error.");
            }
            return s1;
        }
        private string Eq(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);

            return (s1 == s2) ? "t" : "f";
        }
        private string Lt(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            int i1, i2;
            try
            {
               i1 = Int32.Parse(s1); 
               i2 = Int32.Parse(s2); 
            }
            catch
            {
                Console.WriteLine("Lt Parse Error.");
                return "t";
            }
            return (i1 < i2) ? "t" : "f";
        }
        private string Mod(ref string src){
            return src;
        }
        private string Dem(ref string src){
            return src;
        }
        private string Send(ref string src){
            return src;
        }
        private string Neg(ref string src){
            string s1 = getarg(ref src);
            try
            {
               int i1 = Int32.Parse(s1); 
               s1 = (-i1).ToString();
            }
            catch
            {
                Console.WriteLine("Div Parse Error.");
            }
            return s1;
        }
        private string S(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            string s3 = getarg(ref src);            
            return "ap ap " + s1 + " " + s3 + " ap " + s2 + " " + s3;
        }
        private string C(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            string s3 = getarg(ref src);
            return "ap ap " + s1 + " " + s3 + " " +s2;
        }
        private string B(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            string s3 = getarg(ref src);
            return"ap " + s1 + " ap " + s2 + " " + s3;
        }
        private string T(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            return s1;
        }
        private string F(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            return s2;
        }
        private string Pow2(ref string src){
            return src;
        }
        private string I(ref string src){
            string s1 = getarg(ref src);
            return s1;
        }
        private string Cons(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            string s3 = getarg(ref src);
            return "ap ap " + s3 + " " + s1 + " " + s2;
        }
        private string Car(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            string s3 = getarg(ref src);
            if(s2 == ""){
                s2 = s1;
            }
            if(s3 == ""){
                return s2 + " t";
            }
            return s2;
        }
        private string Cdr(ref string src){
            string s1 = getarg(ref src);
            string s2 = getarg(ref src);
            string s3 = getarg(ref src);
            if(s2 == ""){
                s2 = s1;
            }
            if(s3 == ""){
                return s2 + " f";
            }
            return s3;
        }
        private string Nil(ref string src){
            string s1 = getarg(ref src);
            return "t";
        }
        private string IsNil(ref string src){
            string s1 = getarg(ref src);
            return (s1 == "nil") ? "t" : "f";
        }
        private string Lst(ref string src){
            return src;
        }
        private string Vec(ref string src){
            return Cons(ref src);
        }
        private string Draw(ref string src){
            return src;
        }
        private string Checker(ref string src){
            return src;
        }
        private string MDraw(ref string src){
            return src;
        }
        private string MList(ref string src){
            return src;
        }
        private string IsZero(ref string src){
            return src;
        }
        private string Interact(ref string src){
            return src;
        }
        private string getarg(ref string src){
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
            }else{
                src = d[1];
            }
            E_FUNC eFunc = FuncSel(d[0]);
            return (eFunc != E_FUNC.Ap) ? d[0] : Exec[(int)eFunc].iFunc(ref src);
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
