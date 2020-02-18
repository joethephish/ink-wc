using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if( args.Length != 1 ) {
            Console.WriteLine("Expected the root ink filename as an argument");
            return;
        }
        
        var filename = args[0];

        // Change working directory to wherever the main ink file is
        string absRootFilename;
        if( !Path.IsPathRooted(filename) ) {
            absRootFilename = Path.Combine(Directory.GetCurrentDirectory(), filename);
        } else {
            absRootFilename = filename;
        }

        Directory.SetCurrentDirectory(Path.GetDirectoryName(absRootFilename));
        filename = Path.GetFileName(absRootFilename);
        
        // Load up the ink
        var inkText = File.ReadAllText(filename);

        // No error handler, will just have an exception if it doesn't compile
        var parser = new Ink.InkParser(inkText, filename, fileHandler: new InkFileHandler());

        var story = parser.Parse();
        if( !story ) Console.WriteLine("ERROR: No story was produced?");

        // Find all the text content
        var allStrings = story.FindAll<Ink.Parsed.Text>();

        // Count all the words across all strings
        var totalWords = 0;
        foreach(var text in allStrings) {

            var wordsInThisStr = 0;
            var wasWhiteSpace = true;
            foreach(var c in text.text) {
                if( c == ' ' || c == '\t' || c == '\n' || c == '\r' ) {
                    wasWhiteSpace = true;
                } else if( wasWhiteSpace ) {
                    wordsInThisStr++;
                    wasWhiteSpace = false;
                }
            }

            totalWords += wordsInThisStr;
        }

        Console.WriteLine("Total words: "+totalWords);
    }

    class InkFileHandler : Ink.IFileHandler {
        public string ResolveInkFilename (string includeName)
        {
            var workingDir = Directory.GetCurrentDirectory ();
            var fullRootInkPath = Path.Combine (workingDir, includeName);
            return fullRootInkPath;
        }

        public string LoadInkFileContents (string fullFilename)
        {
        	return File.ReadAllText (fullFilename);
        }
    }
}
