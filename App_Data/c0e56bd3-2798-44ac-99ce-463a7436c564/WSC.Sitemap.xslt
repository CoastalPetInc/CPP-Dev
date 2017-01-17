<?xml version="1.0" encoding="UTF-8"?><!DOCTYPE xsl:Stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]><xsl:stylesheet  version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:msxml="urn:schemas-microsoft-com:xslt"  xmlns:msxsl="urn:schemas-microsoft-com:xslt"  xmlns:umbraco.library="urn:umbraco.library"  xmlns:scripts="urn:scripts"    xmlns:meta="http://www.google.com/schemas/meta/0.9"  exclude-result-prefixes="msxml umbraco.library scripts meta"><xsl:output method="html" omit-xml-declaration="yes" encoding="UTF-8"/><xsl:param name="currentPage"/><xsl:variable name="url" select="concat('http://', umbraco.library:RequestServerVariables('HTTP_HOST'), '/sitemapxml.aspx?meta=true')" /><xsl:template match="/">  <xsl:variable name="sitemapXML" select="umbraco.library:GetXmlDocumentByUrl($url)" />  <ul class="site map">        <xsl:for-each select="$sitemapXML//url">            <xsl:if test="position() &gt; 1 and ./@level = preceding-sibling::url[1]/@level">                <xsl:text disable-output-escaping="yes">&lt;/li&gt;</xsl:text>            </xsl:if>                <xsl:if test="position() &gt; 1 and ./@level &gt; preceding-sibling::url[1]/@level">                <xsl:text disable-output-escaping="yes">&lt;ul&gt;</xsl:text>            </xsl:if>                <xsl:call-template name="drawNode" />                <xsl:if test="position() &gt; 1 and ./@level &gt; following-sibling::url[1]/@level">            <xsl:text disable-output-escaping="yes">&lt;/li&gt;</xsl:text>                <xsl:call-template name="for-loop">                    <xsl:with-param name="count" select="./@level - following-sibling::url[1]/@level"/>                </xsl:call-template>            </xsl:if>            <xsl:if test="position() = last()">                <xsl:text disable-output-escaping="yes">&lt;/li&gt;</xsl:text>                <xsl:call-template name="for-loop">                    <xsl:with-param name="count" select="./@level - 1"/>                </xsl:call-template>            </xsl:if>        </xsl:for-each>    </ul>        </xsl:template><xsl:template name="drawNode">  <xsl:text disable-output-escaping="yes">&lt;li&gt;</xsl:text>        <a>        <xsl:attribute name="href">                <xsl:value-of select="./loc" />            </xsl:attribute>            <xsl:value-of select="./@name" />        </a></xsl:template><xsl:template name="for-loop">    <xsl:param name="count" select="1"/>    <xsl:if test="$count > 0">      <xsl:text disable-output-escaping="yes">&lt;/ul&gt;</xsl:text>        <xsl:text disable-output-escaping="yes">&lt;/li&gt;</xsl:text>        <xsl:call-template name="for-loop">            <xsl:with-param name="count" select="$count - 1"/>        </xsl:call-template>    </xsl:if></xsl:template></xsl:stylesheet>