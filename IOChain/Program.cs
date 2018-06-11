using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleEx = My.Utils.CustomConsole.Console;



namespace IOChain
{
    class CProgram
    {
        static Tuple<string[], string[]> TestData1 = new Tuple<string[], string[]>
        (
            new string[]
            {
                "10.0",
                "1 97",
                "2",
                "1 20",
                "2",
                "1 26",
                "1 20",
                "2",
                "3",
                "1 91",
                "3",
            },
            new string[]
            {
                "26",
                "91",
            }
        );



        static void Main (string[] args)
        {
            TextReader       istream = MakeInput  (TestData1.Item1);
            CWriterMediator  ostream = MakeOutput (TestData1.Item2);

            TextReader std_in  = Console.In;
            TextWriter std_out = Console.Out;

            Console.SetIn  (istream);
            Console.SetOut (ostream);


            //   IO-цепочки для ввода-вывода. 
            CIO io = new CIO ();
            if (io.In(io.Number(out int n_vals), io.InEOL()))
            {
                while (n_vals > 0)
                {
                    if (io.Number(out int cmd_code).Success)
                    {
                        switch (cmd_code)
                        {
                            case 1:
                            {
                                if (io.In(io.OneOf(" "), io.Number(out int x_val)))
                                {
                                    ConsoleEx.MsgBox($"c = {cmd_code}, val = {x_val}");
                                }
                                else
                                    goto InvalidInput;
                            }
                            break;

                            case 2:
                            {
                                ConsoleEx.MsgBox($"c = {cmd_code}");
                            }
                            break;

                            case 3:
                            {
                                ConsoleEx.MsgBox($"c = {cmd_code}");
                            }
                            break;

                            default: goto InvalidInput;
                        }

                        if (! io.InEOL().Success) goto InvalidInput;
                    }
                    else
                        goto InvalidInput;

                    n_vals--;
                }

                io.Out(io.Number(26), io.OutEOL());
                io.Out(io.Number(91), io.OutEOL());
            }



            //   Обычный подход к вводу-выводу: 
            //if (int.TryParse(Console.ReadLine(), out int n_vals))
            //{
            //    Console.WriteLine("26");
            //    Console.WriteLine("91");
            //}

            //  Намеренное нарушение конечного вывода, чтобы контрольный
            //  результат не совпал с выводом логики. 
            //Console.WriteLine("ERROR");



            Console.SetIn  (std_in);
            Console.SetOut (std_out);
            Console.WriteLine($"Result Mismatches = {ostream.Mismatches}");
            ConsoleEx.ReadAnyKey();
            return;

            InvalidInput:

            Console.SetIn  (std_in);
            Console.SetOut (std_out);
            Console.WriteLine($"Invalid Input");
            ConsoleEx.ReadAnyKey();
        }





        public static TextReader MakeInput (string[] data)
        {
            var stream = new MemoryStream (1024 * 16);

            var writer = new StreamWriter (stream);

            foreach (string line in data)
            {
                writer.WriteLine(line);
            }

            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader (stream);
            return reader;
        }

        public static CWriterMediator MakeOutput (string[] control_data)
        {
            return new CWriterMediator(Console.Out, MakeInput(control_data));
        }

        public class CWriterMediator : TextWriter
        {
            TextWriter _BaseWriter;
            TextReader _ControlReader;

            public CWriterMediator (TextWriter base_writer, TextReader control_reader)
            {
                _BaseWriter = base_writer;
                _ControlReader = control_reader;

                Mismatches = 0;
            }

            public override Encoding Encoding
            {
                get { return _BaseWriter.Encoding; }
            }

            public override void Write (char value)
            {
                _BaseWriter.Write(value);

                int char_value = _ControlReader.Read();
                if (char_value >= 0)
                {
                    char s = Convert.ToChar(char_value);
                    if (s != value)
                    {
                        this.Mismatches++;
                    }
                }
                else
                    this.Mismatches++;
            }

            public int Mismatches { get; private set; }
        }
    }
}
