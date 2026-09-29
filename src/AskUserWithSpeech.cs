using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using Microsoft.CognitiveServices.Speech;

namespace HabitTracker;

public static class AskUserWithSpeech
{
    private static readonly SpeechConfig Config;

    static AskUserWithSpeech()
    {
        var settings = ConfigSettings.LoadConfig();
        Config = SpeechConfig.FromSubscription(settings.Key, settings.Region);
    }
    
    public static async Task<T> Ask<T>(string[] optionTexts) where T : IParsable<T>
    {
        bool productRecognized = true;
        T? result = default;
        do
        {
            WriteNotRecognizedOption(productRecognized);
            AskUser.WriteOption(optionTexts);

            using var recognizer = new SpeechRecognizer(Config);
            var speechRecognized = await recognizer.RecognizeOnceAsync();
            if (speechRecognized.Reason == ResultReason.RecognizedSpeech)
            {
                // Specific parser for DateTime
                if (typeof(T) == typeof(DateTime))
                {
                    string userChoiceDate =  DateTimeStringCleaner(speechRecognized.Text);
                    Console.WriteLine($"Text recognized : {userChoiceDate}");
                    productRecognized = DateTime.TryParse(userChoiceDate, CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out var parsedDate);
                    
                    result = (T)(object)parsedDate;
                }
                else
                {
                    // Remove everything from what was heard except letters and numbers
                    string userChoice = Regex.Replace(speechRecognized.Text.Trim().ToUpper(), @"[^A-Z0-9\s]", "");
                    Console.WriteLine($"Text recognized : {userChoice}");
                    productRecognized = T.TryParse(userChoice, CultureInfo.CurrentCulture, out result);
                }
            }
            else
            {
                productRecognized = false;
            }
        } while (!productRecognized);

        // productRecognized is true when result get correctly parsed so != null
        return result!;
    }
    
    
    public static async Task<T?> AskOptional<T>(string[] optionTexts) where T : IParsable<T>
    {
        bool productRecognized = true;
        T? result = default;
        do
        {
            WriteNotRecognizedOption(productRecognized);
            AskUser.WriteOption([.. optionTexts, "Say null instead of writing nothing"]);

            using var recognizer = new SpeechRecognizer(Config);
            var speechRecognized = await recognizer.RecognizeOnceAsync();
            if (speechRecognized.Reason == ResultReason.RecognizedSpeech)
            {
                // Specific parser for DateTime
                if (typeof(T) == typeof(DateTime))
                {
                    string userChoiceDate = DateTimeStringCleaner(speechRecognized.Text);
                    Console.WriteLine($"Text recognized : {userChoiceDate}");
                    productRecognized = DateTime.TryParse(userChoiceDate, CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out var parsedDate);
                    
                    result = (T)(object)parsedDate;
                }
                else
                {
                    // Remove everything from what was heard except letters and numbers
                    string userChoice = Regex.Replace(speechRecognized.Text.Trim().ToUpper(), @"[^A-Z0-9\s]", "");
                    Console.WriteLine($"Text recognized : {userChoice}");
                    if (userChoice.StartsWith("NULL"))
                    {
                        return default;
                    }
                    productRecognized = T.TryParse(userChoice, CultureInfo.CurrentCulture, out result);
                }
            }
            else
            {
                productRecognized = false;
            }
        } while (!productRecognized);

        // productRecognized is true when result get correctly parsed so != null
        return result!;
    }
    
    public static async Task<string?> AskOptional(string[] optionTexts)
    {
        bool productRecognized = true;
        string result = "";
        do
        {
            WriteNotRecognizedOption(productRecognized);
            AskUser.WriteOption([.. optionTexts, "Say null instead of writing nothing"]);

            using var recognizer = new SpeechRecognizer(Config);
            var speechRecognized = await recognizer.RecognizeOnceAsync();
            if (speechRecognized.Reason == ResultReason.RecognizedSpeech)
            {
                // Remove everything from what was heard except letters and numbers
                result = Regex.Replace(speechRecognized.Text.Trim().ToUpper(), @"[^A-Z0-9\s]", "");
                if (result.StartsWith("NULL"))
                {
                    return null;
                }
            }
            else
            {
                productRecognized = false;
            }
        } while (!productRecognized);

        return result;
    }


    private static void WriteNotRecognizedOption(bool isValid)
    {
        if (!isValid)
        {
            Console.WriteLine("Option not recognized");
        }
    }

    private static string DateTimeStringCleaner(string input)
    {
        string cleanText = input.Trim().ToUpper();
        // Remove ST, TH etc... (so 1ST => 1)
        cleanText = Regex.Replace(cleanText, @"(?<=\b\d+)(ST|ND|RD|TH)\b", "");
        cleanText = Regex.Replace(cleanText, @"\bOF\b", "");
        cleanText = Regex.Replace(cleanText, @"[^\w\s/.-]", "");
        
        return cleanText;
    }
    
}