using static System.Console;

class Program
{
    static void Main()
    {
        bool do_search = true;
        while (do_search)
        {
            WriteLine($"\nTo search enter SEARCH, to exit enter EXIT:");
            string command = ReadLine().Trim();
            if (command == "EXIT")
            {
                do_search = false;
            }
            else if (command == "SEARCH")
            {
                string[] query = EnterQuery();

                List<string> documents = EnterDocumentPaths();

                // Побудова вектора TF-IDF для кожного документа та пошукового запиту
                Dictionary<string, Dictionary<string, double>> tfidfVectors = CalculateTFIDFVectors(documents, query);

                // Обчислення косинусної схожості між документами та запитом
                Dictionary<string, double> similarities = CalculateCosineSimilarities(tfidfVectors);

                Console.WriteLine("Search results for the query \"{0}\":", string.Join(" ", query));
                foreach (var similarity in similarities.OrderByDescending(kv => kv.Value))
                {
                    if (similarity.Key != "Query")
                        WriteLine("Document: {0}, Similarity: {1}", similarity.Key, similarity.Value);
                }
            }
            else
            {
                WriteLine("Unknown command. Try again");
            }
        }


    }

    static List<string> EnterDocumentPaths()
    {
        bool write_doc = true;
        List<string> docs = new List<string>();
        while (write_doc)
        {
            WriteLine($"\nWrite the path to the documnet, in case of exit write: EXIT");
            string doc = ReadLine().Trim();
            if (doc.Equals("EXIT", StringComparison.OrdinalIgnoreCase) && docs.Count != 0)
            {
                write_doc = false;
            }
            else if (doc.Equals("EXIT", StringComparison.OrdinalIgnoreCase) && docs.Count == 0)
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

    static string[] EnterQuery()
    {
        string queryInput = "";
        bool enterQuery = true;
        while (enterQuery)
        {
            WriteLine($"\nEnter words to search with a space:");
            queryInput = ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(queryInput))
            {
                WriteLine("You did not enter a search query, please try again.");

            }
            else
            {
                enterQuery = false;
            }
        }
        return queryInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    static List<string> ReadDocumentFile(string docPath)
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

    static Dictionary<string, Dictionary<string, double>> CalculateTFIDFVectors(List<string> documents, string[] query)
    {
        Dictionary<string, Dictionary<string, double>> tfidfVectors = new Dictionary<string, Dictionary<string, double>>();

        foreach (var document in documents)
        {
            Dictionary<string, double> tfidfVector = new Dictionary<string, double>();
            List<string> terms = ReadDocumentFile(document.ToLower());
            tfidfVector = CalculateTF(terms, tfidfVector);
            tfidfVector = MultiplyTF_IDF(documents, tfidfVector);
            tfidfVectors.Add(document, tfidfVector);
        }


        Dictionary<string, double> queryTFIDFVector = new Dictionary<string, double>();
        List<string> queryList = query.ToList();
        queryTFIDFVector = CalculateTF(queryList, queryTFIDFVector);
        queryTFIDFVector = MultiplyTF_IDF(documents, queryTFIDFVector);
        tfidfVectors.Add("Query", queryTFIDFVector);

        return tfidfVectors;
    }

    static double CalculateIDF(string term, List<string> documents)
    {
        int documentFrequency = documents.Count(d => ReadDocumentFile(d).Contains(term.ToLower()));
        if (documentFrequency == 0)
        {
            return 0;
        }
        return documentFrequency;
    }

    static Dictionary<string, double> CalculateTF(List<string> terms, Dictionary<string, double> tfidfVector)
    {
        foreach (var term in terms)
        {
            if (!tfidfVector.ContainsKey(term))
            {
                tfidfVector.Add(term, 0);
            }
            tfidfVector[term]++;
        }
        return tfidfVector;
    }

    static Dictionary<string, double> MultiplyTF_IDF(List<string> documents, Dictionary<string, double> tfIdfVector)
    {
        foreach (var term in tfIdfVector.Keys.ToList())
        {
            tfIdfVector[term] = Math.Log(1 + tfIdfVector[term]);                        // Logarithmic TF
            double idf = CalculateIDF(term, documents);
            double smoothIDF = Math.Log((double)(documents.Count + 1) / (idf + 1)) + 1; // Smooth IDF
            tfIdfVector[term] *= smoothIDF;
        }
        return tfIdfVector;
    }

    static Dictionary<string, double> CalculateCosineSimilarities(Dictionary<string, Dictionary<string, double>> tfidfVectors)
    {
        Dictionary<string, double> similarities = new Dictionary<string, double>();

        // Обчислення косинусної схожості між кожним документом та запитом
        foreach (var documentVector in tfidfVectors)
        {
            string document = documentVector.Key;
            Dictionary<string, double> vector = documentVector.Value;

            // Обчислення косинусної схожості між векторами документу та запиту
            double similarity = CalculateCosineSimilarity(vector, tfidfVectors["Query"]);
            similarities.Add(document, similarity);
        }

        return similarities;
    }

    static double CalculateCosineSimilarity(Dictionary<string, double> vector1, Dictionary<string, double> vector2)
    {
        double dotProduct = 0;
        double magnitude1 = 0;
        double magnitude2 = 0;

        foreach (var entry1 in vector1)
        {
            string term = entry1.Key;
            double value1 = entry1.Value;
            magnitude1 += value1 * value1;

            if (vector2.ContainsKey(term))
            {
                dotProduct += value1 * vector2[term];
            }
        }

        foreach (var value in vector2.Values)
        {
            magnitude2 += value * value;
        }

        magnitude1 = Math.Sqrt(magnitude1);
        magnitude2 = Math.Sqrt(magnitude2);

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0;
        }

        return dotProduct / (magnitude1 * magnitude2);
    }
}
