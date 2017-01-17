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
<xsl:param name="distance" select="macro/distance" />
<xsl:param name="zipCode" select="umbraco.library:Request('zip')" />

		
<xsl:template match="/">
	<xsl:value-of select="umbraco.library:RegisterJavaScriptFile('GoogleMaps', 'https://maps.googleapis.com/maps/api/js?libraries=geometry')"/>
	<xsl:value-of select="umbraco.library:RegisterJavaScriptFile('Locations', '/inc/js/locations.js')"/>
	
	<div id="locations">
		<div>Find a location <input type="text" placeholder="Search by Zip Code" />
			<select>
				<option value="10">10 Miles</option>
				<option value="25">25 Miles</option>
				<option value="50">50 Miles</option>
			</select>
			<input type="button" value="Search" />
			<a href="#" class="view-all">View All</a>
		</div>
		<div id="map-canvas"></div>
	
		<div class="locations">
		<xsl:for-each select="$currentPage/ *[@isDoc]" >
			<xsl:variable name="geoAddress" select="wsc.library:JsonToXml(address)" />
			<div class="location" id="location-{@id}" data-lat="{$geoAddress//Lat}" data-lon="{$geoAddress//Lon}">
				<span class="name"><xsl:value-of select="@nodeName" /></span>
				<span class="address"><xsl:value-of select="umbraco.library:ReplaceLineBreaks($geoAddress//Address)" disable-output-escaping="yes"/></span>
				<span class="phone"><xsl:value-of select="phone" /></span>
				<span class="hours"><xsl:value-of select="umbraco.library:ReplaceLineBreaks(hours)" /></span>
			</div>
			</xsl:for-each>
			<p style="display:none;">No results found</p>
		</div>
	</div>


</xsl:template>

</xsl:stylesheet>