<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxml="urn:schemas-microsoft-com:xslt"
  xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets"
  exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets ">
  
  <xsl:output method="html" omit-xml-declaration="yes" indent="yes"/>
  
  <xsl:param name="currentPage"/>
  <!-- Get parameters from Macro -->
  <xsl:param name="location" select="Exslt.ExsltStrings:lowercase(/macro/location)"/>
  <xsl:param name="level" select="/macro/level"/>
  <xsl:param name="css" select="/macro/css"/>
  <xsl:param name="ancestorAlias" select="/macro/ancestorAlias"/>
  <xsl:param name="ancestorLevel" select="/macro/ancestorLevel"/>
  <xsl:param name="nodeID" select="/macro/nodeID"/>
  <!-- Set some variables -->
  <xsl:variable name="startNodeRaw">
  <xsl:choose>
    <xsl:when test="$nodeID != ''">
      <xsl:copy-of select="$currentPage/ancestor-or-self::root//* [@isDoc and @id=$nodeID]"/>
    </xsl:when>
    <xsl:when test="$ancestorAlias != ''">
      <xsl:copy-of select="$currentPage/ancestor-or-self::* [name() = $ancestorAlias and @isDoc]"/>
    </xsl:when>
	<xsl:when test="$ancestorLevel != ''">
      <xsl:copy-of select="$currentPage/ancestor-or-self::* [@level = $ancestorLevel and @isDoc]"/>
    </xsl:when>
    <xsl:otherwise>
      <xsl:copy-of select="$currentPage/ancestor-or-self::root/*" />
    </xsl:otherwise>
  </xsl:choose>
  </xsl:variable>
  <xsl:variable name="startNode" select="msxml:node-set($startNodeRaw)/*" />
  <xsl:variable name="maxLevel">
  <xsl:choose>
    <xsl:when test="$level=0">
      <xsl:value-of select="0"/>
    </xsl:when>
    <xsl:when test="$level != ''">
      <xsl:value-of select="$level + $startNode/@level"/>
    </xsl:when>
    <xsl:otherwise>
      <xsl:value-of select="$startNode/@level + 1" />
    </xsl:otherwise>
  </xsl:choose>
  </xsl:variable>
  
  <!-- Begin Template -->
  <xsl:template match="/">
    <xsl:if test="$startNode/* [@isDoc and ((string(./umbracoNaviHide) != '1' and @level &lt;= $maxLevel and contains(Exslt.ExsltStrings:lowercase(./pageNavigation), $location)) or $maxLevel=0)]" >
      <ul>
        <xsl:if test="$css != ''">
          <xsl:attribute name="class"> <xsl:value-of select="$css" /> </xsl:attribute>
        </xsl:if>
        <xsl:choose>
          <xsl:when test="$maxLevel=0">
            <xsl:call-template name="drawNodesAll">
              <xsl:with-param name="parent" select="$startNode"/>
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="drawNodes">
              <xsl:with-param name="parent" select="$startNode"/>
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </ul>
    </xsl:if>
  </xsl:template>
  
  <xsl:template name="drawNodes">
    <xsl:param name="parent"/>
    <xsl:for-each select="$parent/* [string(umbracoNaviHide) != '1' and @level &lt;= $maxLevel and contains(Exslt.ExsltStrings:lowercase(pageNavigation), $location)]">
      <li>
        <xsl:attribute name="class"><xsl:text>navigation_</xsl:text><xsl:value-of select="@id" />
        <xsl:if test="$currentPage/ancestor-or-self::*/@id = current()/@id">
          <!-- we're under the item - you can do your own styling here -->
          <xsl:text> selected</xsl:text>
        </xsl:if>
        </xsl:attribute>
        <xsl:apply-templates select="." />
        <xsl:if test="count(current()/* [string(umbracoNaviHide) != '1' and @level &lt;= $maxLevel and contains(Exslt.ExsltStrings:lowercase(pageNavigation), $location)]) &gt; 0 ">
          <ul>
            <xsl:call-template name="drawNodes">
              <xsl:with-param name="parent" select="current()"/>
            </xsl:call-template>
          </ul>
        </xsl:if>
      </li>
    </xsl:for-each>
  </xsl:template>
  
  <xsl:template name="drawNodesAll">
    <xsl:param name="parent"/>
    <xsl:for-each select="$parent//* [@isDoc][string(./umbracoNaviHide) != '1' and contains(Exslt.ExsltStrings:lowercase(pageNavigation), Exslt.ExsltStrings:lowercase($location))]">
      <li>
        <xsl:attribute name="class"><xsl:text>navigation_</xsl:text><xsl:value-of select="@id" />
        <xsl:if test="$currentPage/ancestor-or-self::*/@id = current()/@id">
          <!-- we're under the item - you can do your own styling here -->
          <xsl:text>selected</xsl:text>
        </xsl:if>
        </xsl:attribute>
        <xsl:apply-templates select="." />
      </li>
    </xsl:for-each>
  </xsl:template>
    
  <xsl:template match="* [@isDoc]" name="drawLink">
    <a>
      <xsl:choose>
      <xsl:when test="local-name(.) = 'Link'">
        <xsl:attribute name="href">
          <xsl:choose>
              <xsl:when test="url/url-picker/@mode='URL'">
                <xsl:value-of select="url/url-picker/url"/>
                </xsl:when>
              <xsl:when test="number(url/url-picker/node-id)">
                <xsl:value-of select="umbraco.library:NiceUrl(url/url-picker/node-id)"/>
                </xsl:when>
            </xsl:choose>
        </xsl:attribute>
      </xsl:when>
      <xsl:when test="local-name(.) = 'ExternalLink'">
        <xsl:attribute name="href"><xsl:value-of select="./URL" /></xsl:attribute>
		<xsl:if test="starts-with(./URL, 'http')">
		  <xsl:attribute name="target">_blank</xsl:attribute>
		</xsl:if>
      </xsl:when>
      <xsl:when test="string(./target) != ''">
        <xsl:attribute name="target"><xsl:value-of select="./target" /></xsl:attribute>
      </xsl:when>
      <xsl:otherwise>
        <xsl:attribute name="href"><xsl:value-of select="umbraco.library:NiceUrl(@id)" /></xsl:attribute>
      </xsl:otherwise>
    </xsl:choose>
    <!-- Add the display name -->
    <xsl:choose>
      <xsl:when test="string(./pageNavigationName) != ''">
        <xsl:value-of select="./pageNavigationName"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="@nodeName"/>
      </xsl:otherwise>
    </xsl:choose>
    </a>
  </xsl:template>
</xsl:stylesheet>