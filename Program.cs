using System.Text;

namespace SB4ScriptSplitter
{
    struct Script
    {
        public int line;
        public string name;
    }

    internal class Program
    {


        static void Main(string[] args)
        {
            string path = @"D:\Рабочий стол\CoopAndreas\scm\main.txt";
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            string basePath = Path.GetDirectoryName(path) ?? "";
            if(basePath == "") 
            {
                Console.Error.WriteLine("cant get directory from path");
                return;
            }

            string outPath = Path.Combine(basePath, "out");

            List<Script> scripts = new List<Script>();
            string scriptName = "";
            for (int i = 0; i < lines.Length; i++)
            {
                if(lines[i].TrimStart().StartsWith("script_name"))
                {
                    int firstBracket = lines[i].IndexOf('\'');
                    if(firstBracket == -1)
                    {
                        Console.Error.WriteLine($"missing \"'\" at line {i + 1}");
                        return;
                    }

                    int secondBracket = lines[i].IndexOf('\'', firstBracket + 1);
                    if(secondBracket == -1)
                    {
                        Console.Error.WriteLine($"missing \"'\" at line {i + 1}");
                        return;
                    }

                    scriptName = lines[i].Substring(firstBracket + 1, secondBracket - firstBracket -1);

                    Console.WriteLine(scriptName);
                    int labelLine = -1;
                    for (int j = i; j >= Math.Max(i - 10, 0); j--)
                    {
                        if (lines[j].TrimStart().StartsWith("//-------------"))
                        {
                            Console.WriteLine(lines[j]);
                            labelLine = j;
                            break;
                        }
                    }
                    if(labelLine == -1)
                    {
                        for (int j = i; j >= 0; j--)
                        {
                            if (lines[j].Trim() == ":"+scriptName)
                            {
                                Console.WriteLine(lines[j]);
                                labelLine = j;
                                break;
                            }
                        }
                    }

                    if (labelLine == -1)
                    {
                        labelLine = i;
                    }
                    scripts.Add(new Script { line = labelLine, name = scriptName});
                }
            }

            if(!scripts.Any())
            {
                Console.WriteLine("no scripts were found");
                return;
            }

            string scriptsPath = Path.Combine(outPath, "scripts");

            Directory.CreateDirectory(outPath);
            Directory.CreateDirectory(scriptsPath);

            for (int i = 0; i < scripts.Count; i++)
            {
                int nextScriptLine = lines.Length - 1;
                for (int j = i+1; j < scripts.Count; j++)
                {
                    nextScriptLine = scripts[j].line;
                    break;
                }

                string[] scriptLines = lines[scripts[i].line..nextScriptLine];
                File.WriteAllLines(Path.Combine(scriptsPath, scripts[i].name + ".txt"), scriptLines);
            }

            File.WriteAllLines(Path.Combine(outPath, Path.GetFileName(path)), lines[0..scripts[0].line]);

            List<string> includes = new List<string>();
            foreach (var script in scripts)
            {
                includes.Add($"{{$INCLUDE scripts/{script.name}.txt}}");
            }
            File.AppendAllLines(Path.Combine(outPath, Path.GetFileName(path)), includes);
        }
    }
}
