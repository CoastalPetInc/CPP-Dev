<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:msxml="urn:schemas-microsoft-com:xslt"
	xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:Examine="urn:Examine" xmlns:tagsLib="urn:tagsLib" xmlns:BlogLibrary="urn:BlogLibrary" xmlns:ucomponents.cms="urn:ucomponents.cms" xmlns:ucomponents.dates="urn:ucomponents.dates" xmlns:ucomponents.email="urn:ucomponents.email" xmlns:ucomponents.io="urn:ucomponents.io" xmlns:ucomponents.media="urn:ucomponents.media" xmlns:ucomponents.members="urn:ucomponents.members" xmlns:ucomponents.nodes="urn:ucomponents.nodes" xmlns:ucomponents.random="urn:ucomponents.random" xmlns:ucomponents.request="urn:ucomponents.request" xmlns:ucomponents.search="urn:ucomponents.search" xmlns:ucomponents.strings="urn:ucomponents.strings" xmlns:ucomponents.urls="urn:ucomponents.urls" xmlns:ucomponents.xml="urn:ucomponents.xml" xmlns:wsc.library="urn:wsc.library" 
	exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets Examine tagsLib BlogLibrary ucomponents.cms ucomponents.dates ucomponents.email ucomponents.io ucomponents.media ucomponents.members ucomponents.nodes ucomponents.random ucomponents.request ucomponents.search ucomponents.strings ucomponents.urls ucomponents.xml wsc.library ">


<xsl:output method="xml" omit-xml-declaration="yes"/>

<xsl:param name="currentPage"/>
<xsl:variable name="home" select="$currentPage/ancestor-or-self::* [@isDoc and @level=1]" />

<xsl:template match="/">
	<title>
		<xsl:variable name="metaPageTitle" select="$currentPage/ancestor-or-self::* [@isDoc and metaPageTitle]/metaPageTitle" />
		<xsl:if test="$metaPageTitle != ''">
			<xsl:value-of select="concat($metaPageTitle, ' - ')" />
		</xsl:if>
		<xsl:value-of select="$home/siteName" />
	</title>
	
	<xsl:variable name="metaKeywords" select="$currentPage/ancestor-or-self::* [@isDoc and metaKeywords]/metaKeywords" />
	<xsl:if test="$metaKeywords != ''">
		<meta name="keywords" content="{$metaKeywords}" />
	</xsl:if>
	
	<xsl:variable name="metaDescription" select="$currentPage/ancestor-or-self::* [@isDoc and metaDescription]/metaDescription" />
	<xsl:if test="$metaDescription != ''">
		<meta name="description" content="{$metaDescription}" />
	</xsl:if>

</xsl:template>

</xsl:stylesheet>