using System;
using static System.Console;
using System.Collections.Generic;
using static System.IO.File;
using System.IO;
using System.Diagnostics;


class Program
{
    static void Main(string[] args)
    {
        EnterTerm();
        EnterDocumentPath();
    }
    

    static void EnterTerm()
    {
        bool write_term= true;
        List<string> terms = new List<string>();
        while(write_term)
        {
            WriteLine($"\nWrite the term, in case of exit write: EXIT");
            string term = ReadLine().Trim();
            if(term == "EXIT")
            {
                write_term=false;
            }
            else
            {
                if(!terms.Contains(term))
                    terms.Add(term);
            }
            
        }
        
        foreach (string name in terms)
        {
            Console.WriteLine(name);
        }

    }

    static void EnterDocumentPath()
    { 
        bool write_doc= true;
        List<string> docs = new List<string>();
        while(write_doc)
        {
            WriteLine($"\nWrite the path to the documnet, in case of exit write: EXIT");
            string doc = ReadLine().Trim();
            if(doc == "EXIT")
            {
                write_doc=false;
            }
            else
            {
                if(File.Exists(doc) && !docs.Contains(doc))
                {
                    docs.Add(doc);
                }
                else
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine($"\nThe file does not exist or has already been added. Check if the path is correct and try again.");
                    ResetColor();
                }
            }
            
        }

        foreach (string name in docs)
        {
            Console.WriteLine(name);
        }

    }



}

