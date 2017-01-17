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
<xsl:param name="maxNodes" select="macro/maxNodes" />
<xsl:param name="random" select="macro/random" />		
<xsl:param name="width" select="macro/imageWidth" />		
<xsl:param name="height" select="macro/imageHeight" />		
<xsl:param name="nodes" select="$currentPage/ancestor-or-self::root/Ads/Ad" />

<xsl:template match="/">
	<xsl:if test="count($nodes) > 0">
	<div class="ads">
		<xsl:choose>
			<xsl:when test="$random = 1">
				<xsl:for-each select="$nodes">
					<xsl:sort select="ucomponents.random:GetRandomNumber()" data-type="number" />
					<xsl:if test="position() &lt;= $maxNodes">
						<xsl:apply-templates select="." />
					</xsl:if>
				</xsl:for-each>	
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="$nodes">
					<xsl:if test="position() &lt;= $maxNodes">
						<xsl:apply-templates select="." />
					</xsl:if>
				</xsl:for-each>				
			</xsl:otherwise>
		</xsl:choose>		
	</div>
	</xsl:if>	
</xsl:template>

<xsl:template match="Ad [link/url-picker/url != '']">
	<a class="ad ad-{count(preceding-sibling::*) + 1}">
		<xsl:attribute name="href">
			<xsl:choose>
				<xsl:when test="number(link//node-id)" >
					<xsl:value-of select="umbraco.library:NiceUrl(link//node-id)" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="link//url" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
		<xsl:if test="link//new-window = 'True'">
			<xsl:attribute name="target">_blank</xsl:attribute>
		</xsl:if>
		<xsl:apply-templates select="image" />
	</a>
</xsl:template>

<xsl:template match="Ad [link/url-picker/url = '']">
	<span class="ad ad-{count(preceding-sibling::*) + 1}">
		<xsl:apply-templates select="image" />
	</span>
</xsl:template>

<xsl:template match="image">
	<img src="{concat(text(), '?w=', $width, '&amp;h=', $height)}" />
</xsl:template>
		
</xsl:stylesheet>