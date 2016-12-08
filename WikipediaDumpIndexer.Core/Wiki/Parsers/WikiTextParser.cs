using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WikipediaDumpIndexer.Core.Wiki.Parsers
{
    /// <summary>
    ///     For internal use only -- Used by the {@link WikiPage} class.
    ///     Can also be used as a stand alone class to parse wiki formatted text.
    /// </summary>
    sealed class WikiTextParser
    {
        private static readonly Regex StubPattern = new Regex("\\-stub\\}\\}");
        private static readonly Regex DisambCatPattern = new Regex("\\{\\{[Dd]isambig(uation)?\\}\\}"); // the first letter of pages is case-insensitive
        private static readonly Regex RedirectPattern = new Regex("#REDIRECT\\s+\\[\\[(.*?)\\]\\]", RegexOptions.IgnoreCase);

        private List<string> _pageCats;
        private List<string> _pageLinks;
        private readonly bool _disambiguation;
        private readonly bool _redirect;
        private readonly bool _stub;
        private readonly string _redirectString;
        private readonly string _wikiText;

        public WikiTextParser(string wtext)
        {
            _wikiText = wtext;

            var matcher = RedirectPattern.Match(_wikiText);
            if (matcher.Success)
            {
                _redirect = true;
                if (matcher.Groups.Count == 1)
                    _redirectString = matcher.Groups[1].Value;
            }

            matcher = StubPattern.Match(_wikiText);
            _stub = matcher.Success;
            matcher = DisambCatPattern.Match(_wikiText);
            _disambiguation = matcher.Success;
        }

        public bool IsRedirect()
        {
            return _redirect;
        }

        public bool IsStub()
        {
            return _stub;
        }

        public string GetRedirectText()
        {
            return _redirectString;
        }

        public string GetText()
        {
            return _wikiText;
        }

        public List<string> GetCategories()
        {
            if (_pageCats == null) ParseCategories();
            return _pageCats;
        }

        public List<string> GetLinks()
        {
            if (_pageLinks == null) ParseLinks();
            return _pageLinks;
        }

        private void ParseCategories()
        {
            _pageCats = new List<string>();
            var catPattern = new Regex("\\[\\[[Cc]ategory:(.*?)\\]\\]", RegexOptions.Multiline);
            var matcher = catPattern.Match(_wikiText);
            while (matcher.Success)
            {
                /*String[] temp = matcher.Groups[1].Value.split("\\|");
                pageCats.Add(temp[0]);*/
            }
        }

        private void ParseLinks()
        {
            _pageLinks = new List<string>();

            var catPattern = new Regex("\\[\\[(.*?)\\]\\]", RegexOptions.Multiline);
            var matcher = catPattern.Match(_wikiText);
            while (matcher.Success)
            {
                /*String[] temp = matcher.Groups[1].Value.split("\\|");
                if (temp == null || temp.Length == 0) continue;
                String link = temp[0];
                if (link.Contains(":") == false)
                {
                    pageLinks.Add(link);
                }*/
            }
        }

        public string GetPlainText()
        {
            var text = _wikiText.Replace("&gt;", ">");
            text = text.Replace("&lt;", "<");
            text = text.Replace("<ref>.*?</ref>", " ");
            text = text.Replace("</?.*?>", " ");
            text = text.Replace("\\{\\{.*?\\}\\}", " ");
            text = text.Replace("\\[\\[.*?:.*?\\]\\]", " ");
            text = text.Replace("\\[\\[(.*?)\\]\\]", "$1");
            text = text.Replace("\\s(.*?)\\|(\\w+\\s)", " $2");
            text = text.Replace("\\[.*?\\]", " ");
            text = text.Replace("\\'+", "");
            return text;
        }

        private string StripCite(string text)
        {
            var CITE_CONST_STR = "{{cite";
            var startPos = text.IndexOf(CITE_CONST_STR, StringComparison.Ordinal);

            if (startPos < 0)
                return text;

            var bracketCount = 2;
            var endPos = startPos + CITE_CONST_STR.Length;

            for (; endPos < text.Length; endPos++)
            {
                switch (text[endPos])
                {
                    case '}':
                        bracketCount--;
                        break;
                    case '{':
                        bracketCount++;
                        break;
                }
                if (bracketCount == 0) break;
            }

            text = text.Substring(0, startPos - 1) + text.Substring(endPos);
            return StripCite(text);
        }

        public bool IsDisambiguationPage()
        {
            return _disambiguation;
        }

        public string GetTranslatedTitle(string languageCode)
        {
            var regex = new Regex("^\\[\\[" + languageCode + ":(.*?)\\]\\]$", RegexOptions.Multiline);
            var matcher = regex.Match(_wikiText);

            if (matcher.Success)
            {
                return matcher.Groups[1].Value;
            }

            return null;
        }
    }
}