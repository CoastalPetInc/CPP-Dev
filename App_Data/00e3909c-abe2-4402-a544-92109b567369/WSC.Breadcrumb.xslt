<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxml="urn:schemas-microsoft-com:xslt"
  xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:tagsLib="urn:tagsLib"
  exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets tagsLib ">

  <xsl:output method="xml" omit-xml-declaration="yes"/>
  <xsl:param name="currentPage"/>
  <xsl:variable name="minLevel">
	  <xsl:choose>
		<xsl:when test="/macro/showHome = '1'">
		  <xsl:value-of select="number(0)" />
		</xsl:when>
		<xsl:otherwise>
		  <xsl:value-of select="number(1)" />
		</xsl:otherwise>
	  </xsl:choose>
	  </xsl:variable>
	  <xsl:variable name="separator">
	  <xsl:choose>
		<xsl:when test="/macro/separator != ''">
		  <xsl:value-of select="string(/macro/separator)" />
		</xsl:when>
		<xsl:otherwise>
		  <xsl:text>/</xsl:text>
		</xsl:otherwise>
	  </xsl:choose>
  </xsl:variable>
	  
  <xsl:template match="/">
    <xsl:if test="$currentPage/@level &gt; $minLevel">
      <div class="breadcrumb">
        <xsl:for-each select="$currentPage/ancestor::* [@isDoc][@level > $minLevel and string(umbracoNaviHide) != '1']">
          <a href="{umbraco.library:NiceUrl(@id)}">
          <xsl:call-template name="Name" />
          </a> <xsl:value-of select="$separator" />
        </xsl:for-each>
        <!-- print currentpage -->
        <span><xsl:apply-templates select="$currentPage" /></span>
	   </div>
    </xsl:if>
  </xsl:template>
	  
  <xsl:template name="Name" match="* [@isDoc]">
    <xsl:choose>
      <xsl:when test="./pageNavigationName != ''">
        <xsl:value-of select="./pageNavigationName" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="@nodeName"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>