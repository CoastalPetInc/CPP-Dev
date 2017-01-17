<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:msxml="urn:schemas-microsoft-com:xslt"
	xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:Examine="urn:Examine" xmlns:tagsLib="urn:tagsLib" xmlns:BlogLibrary="urn:BlogLibrary" xmlns:wsc.library="urn:wsc.library" xmlns:ucomponents.cms="urn:ucomponents.cms" xmlns:ucomponents.dates="urn:ucomponents.dates" xmlns:ucomponents.email="urn:ucomponents.email" xmlns:ucomponents.io="urn:ucomponents.io" xmlns:ucomponents.media="urn:ucomponents.media" xmlns:ucomponents.members="urn:ucomponents.members" xmlns:ucomponents.nodes="urn:ucomponents.nodes" xmlns:ucomponents.random="urn:ucomponents.random" xmlns:ucomponents.request="urn:ucomponents.request" xmlns:ucomponents.search="urn:ucomponents.search" xmlns:ucomponents.strings="urn:ucomponents.strings" xmlns:ucomponents.urls="urn:ucomponents.urls" xmlns:ucomponents.xml="urn:ucomponents.xml" 
	exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets Examine tagsLib BlogLibrary wsc.library ucomponents.cms ucomponents.dates ucomponents.email ucomponents.io ucomponents.media ucomponents.members ucomponents.nodes ucomponents.random ucomponents.request ucomponents.search ucomponents.strings ucomponents.urls ucomponents.xml ">


<xsl:output method="xml" omit-xml-declaration="yes"/>

<xsl:param name="currentPage"/>
		
<xsl:template match="/">	
	<xsl:apply-templates select="$currentPage" />
</xsl:template>

<xsl:template match="Product">
	<xsl:variable name="images" select="umbraco.library:GetMedia(images, 1)/* [umbracoFile != '']" />
	<xsl:if test="count($images) > 0">
		<img src="{$images[position() = 1]/umbracoFile}?w=200&amp;h=200" class="main-image" />
	</xsl:if>
	<h1><xsl:value-of select="@nodeName" /></h1>
	<p class="sku">Sku: <xsl:value-of select="sku" /></p>
	<div class="description"><xsl:value-of select="bodyText" disable-output-escaping="yes" /></div>
	<xsl:if test="count($images) > 1">
	<div class="gallery">
		<xsl:for-each select="$images[position() > 1]">
			<a href="{umbracoFile}"><img src="{umbracoFile}?w=100&amp;h=100&amp;mode=crop" /></a>
		</xsl:for-each>
	</div>
	</xsl:if>
</xsl:template>
		
</xsl:stylesheet>