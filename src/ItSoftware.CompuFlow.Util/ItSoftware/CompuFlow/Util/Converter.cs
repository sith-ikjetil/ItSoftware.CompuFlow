using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
namespace ItSoftware.CompuFlow.Util
{
    public static class Converter
    {
        /// <summary>
        /// Convert a Dictionary<string,string> to a StringDictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static StringDictionary ToStringDictionary( Dictionary<string, string> dictionary )
        {
            StringDictionary sdResult = new StringDictionary( );
            foreach ( string key in dictionary.Keys ) {
                sdResult.Add( key, dictionary[key] );
            }
            return sdResult;
        }
        /// <summary>
        /// Convert a StringDictionary to a Dictionary<string,string>
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToStringDictionary( StringDictionary dictionary )
        {
            Dictionary<string, string> sdResult = new Dictionary<string, string>( );
            foreach ( string key in dictionary.Keys ) {
                sdResult.Add( key, dictionary[key] );
            }
            return sdResult;
        }
    }
}
