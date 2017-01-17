<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:msxml="urn:schemas-microsoft-com:xslt"
	xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:Examine="urn:Examine" xmlns:tagsLib="urn:tagsLib" xmlns:BlogLibrary="urn:BlogLibrary" xmlns:wsc.library="urn:wsc.library" xmlns:ucomponents.cms="urn:ucomponents.cms" xmlns:ucomponents.dates="urn:ucomponents.dates" xmlns:ucomponents.email="urn:ucomponents.email" xmlns:ucomponents.io="urn:ucomponents.io" xmlns:ucomponents.media="urn:ucomponents.media" xmlns:ucomponents.members="urn:ucomponents.members" xmlns:ucomponents.nodes="urn:ucomponents.nodes" xmlns:ucomponents.random="urn:ucomponents.random" xmlns:ucomponents.request="urn:ucomponents.request" xmlns:ucomponents.search="urn:ucomponents.search" xmlns:ucomponents.strings="urn:ucomponents.strings" xmlns:ucomponents.urls="urn:ucomponents.urls" xmlns:ucomponents.xml="urn:ucomponents.xml" 
	exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets Examine tagsLib BlogLibrary wsc.library ucomponents.cms ucomponents.dates ucomponents.email ucomponents.io ucomponents.media ucomponents.members ucomponents.nodes ucomponents.random ucomponents.request ucomponents.search ucomponents.strings ucomponents.urls ucomponents.xml ">


<xsl:output method="html" omit-xml-declaration="yes"/>

<xsl:param name="currentPage"/>
<xsl:param name="width" select="/macro/imageWidth" />
<xsl:param name="height" select="/macro/imageHeight" />

<xsl:template match="/">
	<xsl:variable name="features" select="$currentPage/features/items/item" />
	<xsl:if test="count($features) > 0">
		<div class="features">
			<xsl:for-each select="$features">
				<xsl:apply-templates select="." />
			</xsl:for-each>
		</div>
	</xsl:if>
</xsl:template>

<xsl:template match="item[link != '']">
	<a href="{link}" class="feature">
		<xsl:apply-templates select="image" />
	</a>
</xsl:template>
		
<xsl:template match="item[link = '']">
	<div class="feature">
		<xsl:apply-templates select="image" />
	</div>
</xsl:template>
		
<xsl:template match="image">
	<img src="{concat(umbraco.library:GetMedia(text(), 0)/umbracoFile,'?w=', $width, '&amp;h=', $height)}" />
</xsl:template>
		
</xsl:stylesheet>