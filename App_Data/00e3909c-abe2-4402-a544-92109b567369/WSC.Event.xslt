<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:msxml="urn:schemas-microsoft-com:xslt"
	xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:Examine="urn:Examine" xmlns:tagsLib="urn:tagsLib" xmlns:BlogLibrary="urn:BlogLibrary" xmlns:ucomponents.cms="urn:ucomponents.cms" xmlns:ucomponents.dates="urn:ucomponents.dates" xmlns:ucomponents.email="urn:ucomponents.email" xmlns:ucomponents.io="urn:ucomponents.io" xmlns:ucomponents.media="urn:ucomponents.media" xmlns:ucomponents.members="urn:ucomponents.members" xmlns:ucomponents.nodes="urn:ucomponents.nodes" xmlns:ucomponents.random="urn:ucomponents.random" xmlns:ucomponents.request="urn:ucomponents.request" xmlns:ucomponents.search="urn:ucomponents.search" xmlns:ucomponents.strings="urn:ucomponents.strings" xmlns:ucomponents.urls="urn:ucomponents.urls" xmlns:ucomponents.xml="urn:ucomponents.xml" 
	exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets Examine tagsLib BlogLibrary ucomponents.cms ucomponents.dates ucomponents.email ucomponents.io ucomponents.media ucomponents.members ucomponents.nodes ucomponents.random ucomponents.request ucomponents.search ucomponents.strings ucomponents.urls ucomponents.xml ">


<xsl:output method="xml" omit-xml-declaration="yes"/>

<xsl:param name="currentPage"/>
<xsl:param name="dateFormat" select="string(/macro/dateFormat)" />

<xsl:template match="/">
	<xsl:apply-templates select="$currentPage" />
</xsl:template>
		
<xsl:template match="Event">
		<h1><xsl:value-of select="@nodeName" disable-output-escaping="yes"/></h1>
		<span class="date">
			<xsl:value-of select="umbraco.library:FormatDateTime(startDate, $dateFormat)"/>
			<xsl:if test="startDate != endDate">
				<xsl:text> - </xsl:text>
				<xsl:choose>
					<xsl:when test="umbraco.library:DateDiff(endDate, startDate, 'm') >= 1440">
						<xsl:value-of select="umbraco.library:FormatDateTime(endDate, $dateFormat)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="umbraco.library:FormatDateTime(endDate, 'h:mm tt')"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</span>
		<xsl:value-of select="bodyText" disable-output-escaping="yes"/>
</xsl:template>

</xsl:stylesheet>