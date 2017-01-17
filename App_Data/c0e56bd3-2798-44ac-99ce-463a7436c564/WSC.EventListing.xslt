<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:msxml="urn:schemas-microsoft-com:xslt"
	xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:Examine="urn:Examine" xmlns:tagsLib="urn:tagsLib" xmlns:BlogLibrary="urn:BlogLibrary" xmlns:ucomponents.cms="urn:ucomponents.cms" xmlns:ucomponents.dates="urn:ucomponents.dates" xmlns:ucomponents.email="urn:ucomponents.email" xmlns:ucomponents.io="urn:ucomponents.io" xmlns:ucomponents.media="urn:ucomponents.media" xmlns:ucomponents.members="urn:ucomponents.members" xmlns:ucomponents.nodes="urn:ucomponents.nodes" xmlns:ucomponents.random="urn:ucomponents.random" xmlns:ucomponents.request="urn:ucomponents.request" xmlns:ucomponents.search="urn:ucomponents.search" xmlns:ucomponents.strings="urn:ucomponents.strings" xmlns:ucomponents.urls="urn:ucomponents.urls" xmlns:ucomponents.xml="urn:ucomponents.xml" 
	exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets Examine tagsLib BlogLibrary ucomponents.cms ucomponents.dates ucomponents.email ucomponents.io ucomponents.media ucomponents.members ucomponents.nodes ucomponents.random ucomponents.request ucomponents.search ucomponents.strings ucomponents.urls ucomponents.xml ">


<xsl:output method="xml" omit-xml-declaration="yes"/>
<xsl:include href="WSC.Pagination.xslt" />

<xsl:param name="currentPage"/>
<xsl:param name="dateFormat" select="string(/macro/dateFormat)" />
<xsl:variable name="type" select="string(umbraco.library:RequestQueryString('type'))"/>
<xsl:variable name="curDate" select="umbraco.library:CurrentDate()"/>

<xsl:template match="/">
    <!-- get the parameters -->
    <xsl:variable name="recordsPerPage">
    <xsl:choose>
      <xsl:when test="number(/macro/recordsPerPage) > 0">
        <xsl:value-of select="/macro/recordsPerPage"/>
      </xsl:when>
      <xsl:otherwise>10</xsl:otherwise>
    </xsl:choose>
    </xsl:variable>
    <xsl:variable name="showArchiveLink" select="boolean(/macro/showArchiveLink)" />
    <xsl:variable name="showTags" select="boolean(/macro/showTags)" />
    <xsl:variable name="tag" >
    <xsl:choose>
      <xsl:when test="string(/macro/tag) != ''">
        <xsl:value-of select="/macro/tag"/>
      </xsl:when>
      <xsl:when test="string(umbraco.library:RequestQueryString('tag')) != ''">
        <xsl:value-of select="umbraco.library:RequestQueryString('tag')"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="''"/>
      </xsl:otherwise>
    </xsl:choose>
    </xsl:variable>
    <!-- Set the variables -->
    <xsl:variable name="pageNumber" >
    <xsl:choose>
      <xsl:when test="number(umbraco.library:RequestQueryString('page')) > 0">
        <xsl:value-of select="umbraco.library:RequestQueryString('page')"/>
      </xsl:when>
      <xsl:otherwise>1</xsl:otherwise>
    </xsl:choose>
    </xsl:variable>
    <!-- Get the nodes into a variable -->
    <xsl:variable name="nodes">
    <xsl:choose>
    <!--
   -->
      <xsl:when test="$tag != ''">
        <xsl:copy-of select="$currentPage/* [@isDoc and umbraco.library:DateGreaterThanOrEqual(startDate, $curDate) and contains(tags, $tag)]" />
      </xsl:when>
      <xsl:when test="$type = 'archive'">
        <xsl:copy-of select="$currentPage/* [@isDoc and umbraco.library:DateGreaterThanOrEqual(startDate, $curDate) and (string(endDate)!='' and umbraco.library:DateGreaterThan($curDate, endDate))]" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:copy-of select="$currentPage/* [@isDoc and umbraco.library:DateGreaterThanOrEqual(startDate, $curDate)]" />
      </xsl:otherwise>
    </xsl:choose>
    </xsl:variable>
    <!-- How many records did we find -->
    <xsl:variable name="numberOfRecords" select="count(msxml:node-set($nodes)/*)" />
    

    <xsl:choose>
      <xsl:when test="$numberOfRecords > 0">
        <!-- Create the news items -->
        <xsl:apply-templates select="msxml:node-set($nodes)/*" >
          <xsl:sort select="startDate" />
          <xsl:with-param name="pageNumber" select="$pageNumber" />
          <xsl:with-param name="recordsPerPage" select="$recordsPerPage" />
        </xsl:apply-templates>
        
        <!-- Add the pagination links -->
        <xsl:call-template name="pagination">
          <xsl:with-param name="pageNumber" select="$pageNumber" />
          <xsl:with-param name="recordsPerPage" select="$recordsPerPage" />
          <xsl:with-param name="numberOfRecords" select="$numberOfRecords" />
          <xsl:with-param name="url">
            <xsl:value-of select="umbraco.library:NiceUrl($currentPage/@id)" />
            <xsl:if test="$type='archive'">
              <xsl:text>?type=archive</xsl:text>
            </xsl:if>
          </xsl:with-param>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <p><xsl:text>No articles available, please check back soon.</xsl:text></p>
      </xsl:otherwise>
    </xsl:choose>
    <!-- Show Archive / Current News links -->
    <xsl:if test="$showArchiveLink">
      <div class="newsArchiveLink"> <a>
        <xsl:attribute name="href"> <xsl:value-of select="umbraco.library:NiceUrl($currentPage/@id)" />
        <xsl:if test="$type != 'archive'">
          <xsl:text>?type=archive</xsl:text>
        </xsl:if>
        </xsl:attribute>
        <xsl:text>View </xsl:text>
        <xsl:choose>
          <xsl:when test="$type='archive' or $tag !=''">
            Current
          </xsl:when>
          <xsl:otherwise>
            Archives
          </xsl:otherwise>
        </xsl:choose>
        </a> </div>
    </xsl:if>
    <!-- Show the tags -->
    <xsl:if test="$showTags = 1 and count(tagsLib:getAllTagsInGroup('News')/tags/tag) &gt; 0">
      <ul class="tags">
        <xsl:for-each select="tagsLib:getAllTagsInGroup('News')/tags/tag">
          <li> <a href="{umbraco.library:NiceUrl(@id)}?tag={current()}"><xsl:value-of select="current()"/></a> (<xsl:value-of select="@nodesTagged"/>) </li>
        </xsl:for-each>
      </ul>
    </xsl:if>
  </xsl:template>
	  
  <!-- Event template -->
  <xsl:template name="section" match="* [@isDoc]" >
    <xsl:param name="pageNumber"/>
    <xsl:param name="recordsPerPage"/>
    <xsl:if test="position() &gt; $recordsPerPage * (number($pageNumber)-1) and position() &lt;= number($recordsPerPage * (number($pageNumber)-1) + $recordsPerPage )">
	  	<a href="{umbraco.library:NiceUrl(@id)}" class="event">
			<span class="name"><xsl:value-of select="@nodeName" disable-output-escaping="yes"/></span>
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
			<span class="summary"><xsl:value-of select="umbraco.library:ReplaceLineBreaks(summary)" disable-output-escaping="yes"/></span>
			<span class="more">Read More</span>
		</a>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>