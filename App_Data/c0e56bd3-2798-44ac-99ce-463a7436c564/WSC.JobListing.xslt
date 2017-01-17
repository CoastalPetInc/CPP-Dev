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
<xsl:variable name="nodes" select="$currentPage/* [@isDoc]" />
		
<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="count($nodes) > 0">
			<div class="jobs">
			<xsl:for-each select="$nodes">
				<xsl:sort select="postDate" />
				<a href="{umbraco.library:NiceUrl(@id)}" class="job">
					<span class="title"><xsl:value-of select="@nodeName" /></span>
					<xsl:if test="type != ''"><span class="type"><xsl:value-of select="type" /></span></xsl:if>
					<xsl:if test="location != ''"><span class="location"><xsl:value-of select="location" /></span></xsl:if>
					<span class="summary"><xsl:value-of select="summary" /></span>
					<span class="more">Read More</span>
				</a>
			</xsl:for-each>
			</div>
		</xsl:when>
		<xsl:otherwise>
			
		</xsl:otherwise>
	</xsl:choose>
	
</xsl:template>

</xsl:stylesheet>