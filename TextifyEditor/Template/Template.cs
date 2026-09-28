public static class Templates
{
    public static string GetTemplateText(TemplateInfo template, string fileNameNoExt)
    {
        if (template == null) return "";

        switch (template.Name)
        {
            case "C# Console":
                return GetCSharpConsole(fileNameNoExt);

            case "C++ Console":
                return GetCppConsole(fileNameNoExt);

            case "HTML Basic":
                return GetHtmlBasic(fileNameNoExt);

            case "Python Console":
                return CreatePythonTemplate(fileNameNoExt);

            case "Java Console":
                return CreateJavaTemplate(fileNameNoExt);

            default:
                return "";
        }
    }

    public static string CreateJavaTemplate(string name)
    {
        return
    @"public class Main {
    public static void main(String[] args) {
        System.out.println(""Hello, World!"");
    }
}";
    }

    private static string GetCSharpConsole(string name)
    {
        return
$@"using System;

namespace {name}
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            Console.WriteLine(""{name}"");
            Console.ReadKey();
        }}
    }}
}}";
    }

    private static string GetCppConsole(string name)
    {
        return
$@"#include <iostream>
using namespace std;

int main()
{{
    cout << ""{name}"" << endl;
    return 0;
}}";
    }

    private static string GetHtmlBasic(string name)
    {
        return
$@"<!DOCTYPE html>
<html lang=""en"">
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{name}</title>
</head>
<body>
    <h1>{name}</h1>

    <footer>
          <p>{name}</p>
    </footer>
</body>
</html>";
    }

    public static string CreatePythonTemplate(string name)
    {
        return
    @"def main():
    print(""Hello, World!"")
    
if __name__ == ""__main__"":
    main()";
    }
}
