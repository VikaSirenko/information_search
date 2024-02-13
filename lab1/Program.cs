using static System.Console;

class Program
{
    static void Main(string[] args)
    {

        List<string> terms = EnterTerm();
        List<string> docs = EnterDocumentPath();
        List<Dictionary<string, List<string>>> set_of_terms = CheckTerms(docs, terms);
        EnterSearchQuery(terms, set_of_terms);
    }


    static List<string> EnterTerm()
    {
        bool write_term = true;
        List<string> terms = new List<string>();
        while (write_term)
        {
            WriteLine($"\nWrite the term, in case of exit write: EXIT");
            string term = ReadLine().Trim();
            if (term == "EXIT")
            {
                write_term = false;
            }
            else
            {
                if (!terms.Contains(term))
                    terms.Add(term);
            }

        }

        return terms;

    }

    static List<string> EnterDocumentPath()
    {
        bool write_doc = true;
        List<string> docs = new List<string>();
        while (write_doc)
        {
            WriteLine($"\nWrite the path to the documnet, in case of exit write: EXIT");
            string doc = ReadLine().Trim();
            if (doc == "EXIT" && docs.Count != 0)
            {
                write_doc = false;
            }
            else if (doc == "EXIT" && docs.Count == 0)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("You have not specified a path to any file. The search will not be possible. Enter the path.");
                ResetColor();
            }
            else
            {
                if (File.Exists(doc) && !docs.Contains(doc))
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

        return docs;
    }

    static List<string> ReadDocFile(string docPath)
    {
        List<string> wordsList = new List<string>();
        string[] lines = File.ReadAllLines(docPath);
        foreach (string line in lines)
        {
            string[] words = line.Split(new char[] { ' ', ',', '.', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
            wordsList.AddRange(words);
        }
        return wordsList;

    }


    static bool QueryVerification(List<string> terms, List<string> search_words)
    {
        foreach (string word in search_words)
        {
            if (!terms.Contains(word))
            {
                WriteLine($"The '{word}' is not in the list of index terms. Try again.");
                return false;
            }
        }

        return true;
    }

    static List<Dictionary<string, List<string>>> CheckTerms(List<string> docs, List<string> terms)
    {
        List<Dictionary<string, List<string>>> set_of_terms = new List<Dictionary<string, List<string>>>();
        foreach (string path in docs)
        {
            List<string> words_in_doc = ReadDocFile(path);
            Dictionary<string, List<string>> buff = new Dictionary<string, List<string>>();
            List<string> buff_words = new List<string>();
            foreach (string word in terms)
            {
                if (words_in_doc.Contains(word))
                {
                    buff_words.Add(word);
                }
            }
            buff.Add(path, buff_words);
            set_of_terms.Add(buff);
        }

        return set_of_terms;

    }

    static void EnterSearchQuery(List<string> terms, List<Dictionary<string, List<string>>> set_of_terms)
    {
        bool do_search = true;
        while (do_search)
        {
            WriteLine($"\nEnter a search query (using AND)or EXIT:");
            string search_query = ReadLine().Trim();
            if (search_query == "EXIT")
            {
                do_search = false;
            }
            else
            {
                List<string> search_words = new List<string>();
                search_words.AddRange(search_query.Split(new string[] { "AND" }, StringSplitOptions.None).Select(word => word.Trim()));
                if (QueryVerification(terms, search_words) == true)
                {
                    List<string> result = Search(search_words, set_of_terms);
                    if (result != null)
                    {
                        string commaSeparatedResult = string.Join(", ", result);
                        WriteLine($"The search query '{search_query}' was found in the following documents: " + commaSeparatedResult);
                    }
                    else
                    {
                        WriteLine($"The search query '{search_query}' was not found in any document");
                    }

                }

            }

        }
    }

    static List<string> Search(List<string> search_words, List<Dictionary<string, List<string>>> set_of_terms)
    {
        Dictionary<string, List<string>> foundInDictionaries = new Dictionary<string, List<string>>();

        foreach (string word in search_words)
        {
            foreach (var dictionary in set_of_terms)
            {
                foreach (var pair in dictionary)
                {
                    if (pair.Value.Contains(word))
                    {
                        if (!foundInDictionaries.ContainsKey(word))
                        {
                            foundInDictionaries.Add(word, new List<string>());
                        }

                        foundInDictionaries[word].Add(pair.Key);
                    }
                }
            }
        }
        if (foundInDictionaries.Count == search_words.Count)
        {
            List<string> search_result = FindCommonValues(foundInDictionaries);
            return search_result;
        }
        else
        {
            return null;
        }

    }

    static List<string> FindCommonValues(Dictionary<string, List<string>> dictionary)
    {

        HashSet<string> commonValues = new HashSet<string>(dictionary.First().Value);

        foreach (var pair in dictionary.Skip(1))
        {
            commonValues.IntersectWith(pair.Value);
        }

        return commonValues.ToList();
    }
}







