<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxml="urn:schemas-microsoft-com:xslt"
  xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets"
  exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets ">

  <xsl:output method="xml" omit-xml-declaration="yes"/>
  
  <xsl:param name="currentPage"/>
  <xsl:param name="title" select="/macro/title" />
  <xsl:param name="class" select="/macro/myClass" />
  <xsl:param name="dateFormat" select="string(/macro/dateFormat)" />
  <xsl:param name="maxNodes">
    <xsl:choose>
      <xsl:when test="/macro/maxNodes != ''">
          <xsl:value-of select="/macro/maxNodes" />
        </xsl:when>
        <xsl:otherwise>3</xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  <xsl:param name="sourceId" select="/macro/source" />
  
  <xsl:variable name="curDate" select="umbraco.library:CurrentDate()"/>
  <xsl:variable name="nodes" select="umbraco.library:GetXmlNodeById($sourceId)//NewsItem [umbraco.library:DateGreaterThanOrEqual($curDate, ./startDate) and (string(./endDate)='' or umbraco.library:DateGreaterThanToday(./endDate))]" />
  
  <xsl:template match="/">
    <xsl:if test="number($sourceId)">
    <dl class="{$class}">
      <dt><a href="{umbraco.library:NiceUrl($sourceId)}"><xsl:value-of select="$title" /></a></dt>
      <xsl:choose>
        <xsl:when test="count($nodes) > 0">
          <xsl:for-each select="$nodes" >
            <xsl:sort select="./startDate" order="descending" />
            <xsl:if test="position() &lt; $maxNodes">
				<xsl:apply-templates select="." />
            </xsl:if>
          </xsl:for-each>
        </xsl:when>
        <xsl:otherwise>
        </xsl:otherwise>
      </xsl:choose>
    </dl>
    </xsl:if>
	
  </xsl:template>
	  
  <xsl:template match="NewsItem">
	  <dd>
		  <a href="{umbraco.library:NiceUrl(@id)}">
			  <strong class="title">
				  <xsl:value-of select="@nodeName" />
			  </strong>
			  <span class="date">
				  <xsl:value-of select="umbraco.library:FormatDateTime(startDate, $dateFormat)" />
			  </span>
			  <span class="summary">
				  <xsl:value-of select="summary" />
			  </span>
			  <span class="more">Read More &gt;</span>
		  </a>
	  </dd>
  </xsl:template>
</xsl:stylesheet>