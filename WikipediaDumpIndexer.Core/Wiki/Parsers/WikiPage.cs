using System.Collections.Generic;

namespace WikipediaDumpIndexer.Core.Wiki.Parsers
{
    /// <summary>
    /// Data structures for a wikipedia page.
    /// </summary>
    sealed class WikiPage
    {
        private WikiTextParser _wikiTextParser;
        private string _title;
        private string _id;

        /// <summary>
        /// Set the page title. This is not intended for direct use.
        /// </summary>
        /// <param name="title">The title.</param>
        public void SetTitle(string title)
        {
            _title = title;
        }

        /// <summary>
        /// Set the wiki text associated with this page.
        /// This setter also introduces side effects. This is not intended for direct use.
        /// </summary>
        /// <param name="wtext">The wtext.</param>
        public void SetWikiText(string wtext)
        {
            _wikiTextParser = new WikiTextParser(wtext);
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <returns>a string containing the page title.</returns>
        public string GetTitle()
        {
            return _title;
        }

        /// <summary>
        /// Gets the translated title.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns>a string containing the title translated in the given languageCode.</returns>
        public string GetTranslatedTitle(string languageCode)
        {
            return _wikiTextParser.GetTranslatedTitle(languageCode);
        }

        /// <summary>
        /// Determines whether [is disambiguation page].
        /// </summary>
        /// <returns>if this a disambiguation page.</returns>
        public bool IsDisambiguationPage()
        {
            if (_title.Contains("(disambiguation)") || _wikiTextParser.IsDisambiguationPage())
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether [is special page].
        /// </summary>
        /// <returns>for "special pages" -- like Category:, Wikipedia:, etc</returns>
        public bool IsSpecialPage()
        {
            return _title.Contains(":");
        }

        /// <summary>
        /// Use this method to get the wiki text associated with this page.
        /// Useful for custom processing the wiki text.
        /// </summary>
        /// <returns>a string containing the wiki text.</returns>
        public string GetWikiText()
        {
            return _wikiTextParser.GetText();
        }

        /// <summary>
        /// Determines whether this instance is redirect.
        /// </summary>
        /// <returns>if this is a redirection page</returns>
        public bool IsRedirect()
        {
            return _wikiTextParser.IsRedirect();
        }

        /// <summary>
        /// Determines whether this instance is stub.
        /// </summary>
        /// <returns>if this is a stub page</returns>
        public bool IsStub()
        {
            return _wikiTextParser.IsStub();
        }

        /// <summary>
        /// Gets the redirect page.
        /// </summary>
        /// <returns>the title of the page being redirected to.</returns>
        public string GetRedirectPage()
        {
            return _wikiTextParser.GetRedirectText();
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns>plain text stripped of all wiki formatting.</returns>
        public string GetText()
        {
            return _wikiTextParser.GetPlainText();
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>a list of categories the page belongs to, null if this a redirection/disambiguation page</returns>
        public List<string> GetCategories()
        {
            return _wikiTextParser.GetCategories();
        }

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <returns>a list of links contained in the page</returns>
        public List<string> GetLinks()
        {
            return _wikiTextParser.GetLinks();
        }

        public void SetId(string id)
        {
            this._id = id;
        }

        public string GetId()
        {
            return _id;
        }
    }
}