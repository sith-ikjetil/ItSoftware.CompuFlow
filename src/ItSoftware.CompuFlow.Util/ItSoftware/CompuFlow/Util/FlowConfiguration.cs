using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Util
{
    public static class FlowConfiguration
    {
        #region Public Static Methods
        /// <summary>
        /// Merges two flow configuration files into one resulting.
        /// </summary>
        /// <param name="xdSource"></param>
        /// <param name="xdOutput"></param>
        /// <returns></returns>
        public static XmlDocument Merge( XmlDocument xdConfig1, XmlDocument xdConfig2 )
        {
            XmlDocument xdResult = new XmlDocument( );
            XmlDeclaration xdDecl = xdResult.CreateXmlDeclaration( "1.0", "utf-8", null );
            xdResult.AppendChild( xdDecl );
            XmlElement xeConfiguration = xdResult.CreateElement( "configuration" );
            xdResult.AppendChild( xeConfiguration );

            ImportNodes( xeConfiguration, xdConfig1 );
            ImportNodes( xeConfiguration, xdConfig2 );

            NormalizeDuplicates2Level( xeConfiguration );

            return xdResult;
        }
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Imports all nodes from data to configuration element.
        /// </summary>
        /// <param name="xnConfiguration"></param>
        /// <param name="xdData"></param>
        private static void ImportNodes( XmlNode xnConfiguration, XmlDocument xdData )
        {
            XmlNode xnConfigurationData = xdData.SelectSingleNode( "/configuration" );
            XmlNode xnConfigurationDataImported = xnConfiguration.OwnerDocument.ImportNode( xnConfigurationData, true );

            while ( xnConfigurationDataImported.ChildNodes.Count > 0 ) {
                xnConfiguration.AppendChild( xnConfigurationDataImported.ChildNodes[0] );
            }
        }
        /// <summary>
        /// Removes duplicates at 2 level. Should be good enough for flow configuration files.
        /// </summary>
        /// <param name="xnConfiguration"></param>
        private static void NormalizeDuplicates2Level( XmlNode xnConfiguration )
        {
            if ( xnConfiguration.ChildNodes.Count <= 1 ) {
                return;
            }

            for ( int i = 0; i < xnConfiguration.ChildNodes.Count; i++ ) {
                XmlNode xn1 = xnConfiguration.ChildNodes[i];
                for ( int j = i + 1; j < xnConfiguration.ChildNodes.Count; j++ ) {
                    if ( xn1.Name == xnConfiguration.ChildNodes[j].Name ) {
                        XmlNode xn2 = xnConfiguration.ChildNodes[j];
                        //
                        // Move all child nodes under xn2 to under xn1 and then kill xn2.
                        //
                        int index = 0;
                        while ( xn2.ChildNodes.Count > index ) {
                            if ( !DoesNodeExist( xn1, xn2.ChildNodes[index] ) ) {
                                xn1.AppendChild( xn2.ChildNodes[index] );
                            }
                            else {
                                index++;
                            }
                        }
                        xn2.ParentNode.RemoveChild( xn2 );
                    }
                }
            }
        }
        /// <summary>
        /// Check if node exist as child node to other node.
        /// </summary>
        /// <param name="xn1"></param>
        /// <param name="xn2"></param>
        /// <returns></returns>
        private static bool DoesNodeExist( XmlNode xn1, XmlNode xn2 )
        {
            foreach ( XmlNode node in xn1.ChildNodes ) {
                if ( node.OuterXml == xn2.OuterXml ) {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }// class
}// namespace
