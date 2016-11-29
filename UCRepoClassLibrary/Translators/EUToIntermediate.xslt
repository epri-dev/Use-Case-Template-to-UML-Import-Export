<?xml version="1.0" encoding="UTF-8"?>
<?altova_samplexml file:///E:/DriveE/_project/EPRI/C18322-EPRIUseCaseRepository/Source/FilledInTemplates/IEC_UseCaseTemplateExampleGreenButton_TIDY.xml?>
<xsl:stylesheet version="1.0" xmlns:xhtml="http://www.w3.org/1999/xhtml" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xsi:schemaLocation="http://www.w3.org/1999/xhtml xhtml1-transitional.dtd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
  <xsl:template match="/">
    <UC:UseCaseRepository xsi:schemaLocation="http://www.TC8.org/IEC62559/UseCaseTemplate/V01 IEC62559UseCaseTemplate_V01.xsd" xmlns:UC="http://www.TC8.org/IEC62559/UseCaseTemplate/V01" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
      <ActorLibrary>
        <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
          <xsl:variable name="TablePos" select="position()"/>
          <xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space('Actors')">
            <xsl:element name="Domain">
              <xsl:attribute name="location">
                <xsl:call-template name="FindHeaderOfTable" />
              </xsl:attribute>
              <xsl:element name="name">
                <xsl:attribute name="location">
                  <xsl:call-template name="FindHeaderOfTable" />
                </xsl:attribute>
                <xsl:value-of select="xhtml:tr[3]/xhtml:td[1]//*[not(*)]/text()"/>
              </xsl:element>
              <xsl:call-template name="ParseTable">
                <xsl:with-param name="RowName" select="'Actor'"/>
                <xsl:with-param name="FirstRowOffset" select="'4'"/>
                <xsl:with-param name="ElName1" select="'description'"/>
                <xsl:with-param name="ColIdx1" select="3"/>
                <xsl:with-param name="ElName2" select="'furtherInformation'"/>
                <xsl:with-param name="ColIdx2" select="4"/>
                <xsl:with-param name="ElName3" select="'name'"/>
                <xsl:with-param name="ColIdx3" select="1"/>
                <xsl:with-param name="ElName4" select="'type'"/>
                <xsl:with-param name="ColIdx4" select="2"/>
              </xsl:call-template>
            </xsl:element>
          </xsl:if>
        </xsl:for-each>
      </ActorLibrary>
      <DomainLibrary>
        <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
          <xsl:variable name="TablePos" select="position()"/>
          <xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space('Actors')">
            <xsl:element name="Domain">
              <xsl:attribute name="location">
                <xsl:call-template name="FindHeaderOfTable" />
              </xsl:attribute>
              <xsl:element name="description">
                <xsl:attribute name="location">
                  <xsl:call-template name="FindHeaderOfTable" />
                </xsl:attribute>
                <xsl:apply-templates select="xhtml:tr[3]/xhtml:td[2]//*[not(*)]"/>
              </xsl:element>

              <xsl:element name="name">
                <xsl:attribute name="location">
                  <xsl:call-template name="FindHeaderOfTable" />
                </xsl:attribute>
                <xsl:value-of select="xhtml:tr[3]/xhtml:td[1]//*[not(*)]/text()"/>
              </xsl:element>
            </xsl:element>
          </xsl:if>
        </xsl:for-each>
      </DomainLibrary>
      <InformationModelLibrary>
        <xsl:call-template name="FindAndParseTable">
          <xsl:with-param name="TableIdentifier" select="'Information Exchanged'"/>
          <xsl:with-param name="RowName" select="'InformationModel'"/>
          <xsl:with-param name="FirstRowOffset" select="'2'"/>
          <xsl:with-param name="ElName1" select="'description'"/>
          <xsl:with-param name="ColIdx1" select="2"/>
          <xsl:with-param name="ElName2" select="'name'"/>
          <xsl:with-param name="ColIdx2" select="1"/>
          <xsl:with-param name="ElName3" select="'referencedRequirement'"/>
          <xsl:with-param name="ColIdx3" select="3"/>
          <xsl:with-param name="SplitHead3" select="'&lt;referencedRequirement&gt;&lt;id&gt;'"/>
          <xsl:with-param name="SplitTail3" select="'&lt;/id&gt;&lt;/referencedRequirement&gt;'"/>
        </xsl:call-template>
      </InformationModelLibrary>
      <UseCaseLibrary>
        <UseCase>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Classification Information'"/>
            <xsl:with-param name="FirstRowOffset" select="'8'"/>
            <xsl:with-param name="LastRowOffset" select="'10'"/>
            <xsl:with-param name="ElName1" select="'classification'"/>
            <xsl:with-param name="ColIdx1" select="1"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Use Case Identification'"/>
            <xsl:with-param name="FirstRowOffset" select="'2'"/>
            <xsl:with-param name="LastRowOffset" select="'4'"/>
            <xsl:with-param name="ElName1" select="'id'"/>
            <xsl:with-param name="ColIdx1" select="1"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Classification Information'"/>
            <xsl:with-param name="FirstRowOffset" select="'12'"/>
            <xsl:with-param name="LastRowOffset" select="'14'"/>
            <xsl:with-param name="ElName1" select="'keywords'"/>
            <xsl:with-param name="ColIdx1" select="1"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Classification Information'"/>
            <xsl:with-param name="FirstRowOffset" select="'4'"/>
            <xsl:with-param name="LastRowOffset" select="'6'"/>
            <xsl:with-param name="ElName1" select="'levelOfDepth'"/>
            <xsl:with-param name="ColIdx1" select="1"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Use Case Identification'"/>
            <xsl:with-param name="FirstRowOffset" select="'2'"/>
            <xsl:with-param name="LastRowOffset" select="'4'"/>
            <xsl:with-param name="ElName1" select="'name'"/>
            <xsl:with-param name="ColIdx1" select="3"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Classification Information'"/>
            <xsl:with-param name="FirstRowOffset" select="'6'"/>
            <xsl:with-param name="LastRowOffset" select="'8'"/>
            <xsl:with-param name="ElName1" select="'prioritisation'"/>
            <xsl:with-param name="ColIdx1" select="1"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Scope and Objectives of Use Case'"/>
            <xsl:with-param name="FirstRowOffset" select="'2'"/>
            <xsl:with-param name="LastRowOffset" select="'4'"/>
            <xsl:with-param name="ElName1" select="'scope'"/>
            <xsl:with-param name="ColIdx1" select="2"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Classification Information'"/>
            <xsl:with-param name="FirstRowOffset" select="10"/>
            <xsl:with-param name="LastRowOffset" select="12"/>
            <xsl:with-param name="ElName1" select="'viewPoint'"/>
            <xsl:with-param name="ColIdx1" select="1"/>
          </xsl:call-template>
          <!--<BusinessCase/name>-->
          <xsl:call-template name="CommaDelimValue">
            <xsl:with-param name="TableIdentifier" select="'Scope and Objectives of Use Case'"/>
            <xsl:with-param name="FirstRowOffset" select="'1'"/>
            <xsl:with-param name="LastRowOffset" select="'3'"/>
            <xsl:with-param name="ColIdx1" select="2"/>
            <xsl:with-param name="head" select="'&lt;BusinessCase&gt;&lt;name&gt;'"/>
            <xsl:with-param name="tail" select="'&lt;/name&gt;&lt;/BusinessCase&gt;'"/>
          </xsl:call-template>
          <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
            <xsl:variable name="TablePos" select="position()"/>
            <xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space('Use Case Conditions')">
              <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table[$TablePos]">
                <xsl:for-each select="xhtml:tr[(position() > 2) and (position() &lt; 1000)]">
                  <xsl:if test="count(xhtml:td//*[not(*)][string-length(normalize-space(text()))>0])">
                    <xsl:element name="Condition">
                      <xsl:attribute name="location">
                        <xsl:call-template name="FindHeaderOfTable2" />
                      </xsl:attribute>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="4"/>
                        <xsl:with-param name="ElName" select="'assumption'"/>
                      </xsl:call-template>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="2"/>
                        <xsl:with-param name="ElName" select="'trigerringEvent'"/>
                      </xsl:call-template>

                      <xsl:if test="count(xhtml:td[3]//*[not(*)][string-length(normalize-space(text()))>0])">
                        <xsl:for-each select="xhtml:td[3]/xhtml:p">
                          <xsl:element name="PreCondition">
                            <xsl:element name="content">
                              <xsl:attribute name="location">
                                <xsl:call-template name="FindHeaderOfTable2" />
                              </xsl:attribute>
                              <xsl:apply-templates select="."/>
                            </xsl:element>
                          </xsl:element>
                        </xsl:for-each>
                      </xsl:if>
                      <referencedActor>
                        <xsl:call-template name="ParseCol">
                          <xsl:with-param name="ColIdx" select="1"/>
                          <xsl:with-param name="ElName" select="'name'"/>
                        </xsl:call-template>
                      </referencedActor>
                    </xsl:element>
                  </xsl:if>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:if>
          </xsl:for-each>
          <GeneralRemark>
            <xsl:call-template name="FindAndParseTable">
              <xsl:with-param name="TableIdentifier" select="'General Remarks'"/>
              <xsl:with-param name="FirstRowOffset" select="'1'"/>
              <xsl:with-param name="LastRowOffset" select="'3'"/>
              <xsl:with-param name="ElName1" select="'content'"/>
              <xsl:with-param name="ColIdx1" select="1"/>
            </xsl:call-template>
          </GeneralRemark>
          <Narrative>
            <xsl:call-template name="FindAndParseTable">
              <xsl:with-param name="TableIdentifier" select="'Narrative of Use Case'"/>
              <xsl:with-param name="FirstRowOffset" select="'4'"/>
              <xsl:with-param name="LastRowOffset" select="'6'"/>
              <xsl:with-param name="ElName1" select="'completeDescription'"/>
              <xsl:with-param name="EmitHTML1" select="1"/>
            </xsl:call-template>
            <xsl:call-template name="FindAndParseTable">
              <xsl:with-param name="TableIdentifier" select="'Narrative of Use Case'"/>
              <xsl:with-param name="FirstRowOffset" select="'2'"/>
              <xsl:with-param name="LastRowOffset" select="'4'"/>
              <xsl:with-param name="ElName1" select="'shortDescription'"/>
            </xsl:call-template>
          </Narrative>
          <!--<RelatedObjective/name>-->
          <xsl:call-template name="CommaDelimValue">
            <xsl:with-param name="TableIdentifier" select="'Scope and Objectives of Use Case'"/>
            <xsl:with-param name="FirstRowOffset" select="'3'"/>
            <xsl:with-param name="LastRowOffset" select="'5'"/>
            <xsl:with-param name="ColIdx1" select="2"/>
            <xsl:with-param name="head" select="'&lt;RelatedObjective&gt;&lt;name&gt;'"/>
            <xsl:with-param name="tail" select="'&lt;/name&gt;&lt;/RelatedObjective&gt;'"/>
          </xsl:call-template>
          <xsl:variable name="Domains">
            <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
              <xsl:variable name="TablePos" select="position()"/>
              <xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space('Use Case Identification')">
                <xsl:value-of select="xhtml:tr[3]/xhtml:td[2]//*[not(*)]/text()"/>
              </xsl:if>
            </xsl:for-each>
          </xsl:variable>
          <PrimaryDomain>
            <xsl:element name="name">
              <xsl:attribute name="location">
                <xsl:call-template name="FindHeaderOfTable" />
              </xsl:attribute>
              <xsl:choose>
                <xsl:when test="contains($Domains,',')">
                  <xsl:value-of select="substring-before($Domains,',')"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="$Domains"/>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:element>
          </PrimaryDomain>
          <xsl:call-template name="split">
            <xsl:with-param name="list" select="substring-after($Domains,',')"/>
            <xsl:with-param name="separator" select="','"/>
            <xsl:with-param name="outhead" select="'&lt;AdditionalDomain&gt;&lt;name&gt;'"/>
            <xsl:with-param name="outtail" select="'&lt;/name&gt;&lt;/AdditionalDomain&gt;'"/>
          </xsl:call-template>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'References'"/>
            <xsl:with-param name="RowName" select="'Reference'"/>
            <xsl:with-param name="FirstRowOffset" select="'2'"/>
            <xsl:with-param name="ElName1" select="'description'"/>
            <xsl:with-param name="ColIdx1" select="3"/>
            <xsl:with-param name="ElName2" select="'id'"/>
            <xsl:with-param name="ColIdx2" select="1"/>
            <xsl:with-param name="ElName3" select="'impact'"/>
            <xsl:with-param name="ColIdx3" select="5"/>
            <xsl:with-param name="ElName4" select="'originatorOrganisation'"/>
            <xsl:with-param name="ColIdx4" select="6"/>
            <xsl:with-param name="ElName5" select="'status'"/>
            <xsl:with-param name="ColIdx5" select="4"/>
            <xsl:with-param name="ElName6" select="'type'"/>
            <xsl:with-param name="ColIdx6" select="2"/>
            <xsl:with-param name="ElName7" select="'URI'"/>
            <xsl:with-param name="ColIdx7" select="7"/>
          </xsl:call-template>
          <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
            <xsl:if test="xhtml:tr[1]/xhtml:td[1]//*[not(*)][normalize-space(text())=normalize-space('Scenario')]">
              <xsl:variable name="thisScenario">
                <xsl:for-each select="xhtml:tr[2]/xhtml:td[2]/xhtml:p">
                  <xsl:apply-templates select="."/>
                </xsl:for-each>
              </xsl:variable>
              <xsl:element name="Scenario">
                <xsl:attribute name="location">
                  <xsl:call-template name="FindHeaderOfTable" />
                </xsl:attribute>
                <xsl:element name="name">
                  <xsl:attribute name="location">
                    <xsl:call-template name="FindHeaderOfTable" />
                  </xsl:attribute>
                  <xsl:value-of select="$thisScenario"/>
                </xsl:element>
                <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
                  <xsl:variable name="TablePos" select="position()"/>
                  <xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space('Scenario Conditions')">
                    <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table[$TablePos]">
                      <xsl:for-each select="xhtml:tr[(position() > 2) and (position() &lt; 1000)]">
                        <xsl:if test="count(xhtml:td//*[not(*)][string-length(normalize-space(text()))>0])">
                          <xsl:variable name="ScenarioName">
                            <xsl:if test="count(xhtml:td[2]//*[not(*)][string-length(normalize-space(text()))>0])">
                              <xsl:for-each select="xhtml:td[2]/xhtml:p">
                                <xsl:apply-templates select="."/>
                              </xsl:for-each>
                            </xsl:if>
                          </xsl:variable>
                          <xsl:if test="$ScenarioName = $thisScenario">
                            <xsl:call-template name="ParseCol">
                              <xsl:with-param name="ColIdx" select="1"/>
                              <xsl:with-param name="ElName" select="'number'"/>
                            </xsl:call-template>
                            <xsl:if test="count(xhtml:td[6]//*[not(*)][string-length(normalize-space(text()))>0])">
                              <xsl:for-each select="xhtml:td[6]/xhtml:p">
                                <xsl:element name="postCondition">
                                  <xsl:attribute name="location">
                                    <xsl:call-template name="FindHeaderOfTable2" />
                                  </xsl:attribute>
                                  <xsl:apply-templates select="."/>
                                </xsl:element>
                              </xsl:for-each>
                            </xsl:if>
                            <xsl:if test="count(xhtml:td[5]//*[not(*)][string-length(normalize-space(text()))>0])">
                              <xsl:for-each select="xhtml:td[5]/xhtml:p">
                                <xsl:element name="preCondition">
                                  <xsl:attribute name="location">
                                    <xsl:call-template name="FindHeaderOfTable2" />
                                  </xsl:attribute>
                                  <xsl:apply-templates select="."/>
                                </xsl:element>
                              </xsl:for-each>
                            </xsl:if>
                            <xsl:call-template name="ParseCol">
                              <xsl:with-param name="ColIdx" select="4"/>
                              <xsl:with-param name="ElName" select="'triggeringEvent'"/>
                            </xsl:call-template>
                            <PrimaryActor>
                              <xsl:call-template name="ParseCol">
                                <xsl:with-param name="ColIdx" select="3"/>
                                <xsl:with-param name="ElName" select="'name'"/>
                              </xsl:call-template>
                            </PrimaryActor>
                          </xsl:if>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:if>
                </xsl:for-each>
                <xsl:for-each select="xhtml:tr[(position() > 3) and (position() &lt; 1000)]">
                  <xsl:if test="count(xhtml:td//*[not(*)][string-length(normalize-space(text()))>0])">
                    <xsl:element name="Step">
                      <xsl:attribute name="location">
                        <xsl:call-template name="FindHeaderOfTable2" />
                      </xsl:attribute>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="4"/>
                        <xsl:with-param name="ElName" select="'description'"/>
                      </xsl:call-template>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="2"/>
                        <xsl:with-param name="ElName" select="'event'"/>
                      </xsl:call-template>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="3"/>
                        <xsl:with-param name="ElName" select="'name'"/>
                      </xsl:call-template>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="1"/>
                        <xsl:with-param name="ElName" select="'number'"/>
                      </xsl:call-template>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="5"/>
                        <xsl:with-param name="ElName" select="'service'"/>
                      </xsl:call-template>
                      <xsl:if test="count(xhtml:td[7]//*[not(*)][string-length(normalize-space(text()))>0])">
                        <InformationReceiver>
                          <xsl:call-template name="ParseCol">
                            <xsl:with-param name="ColIdx" select="7"/>
                            <xsl:with-param name="ElName" select="'name'"/>
                          </xsl:call-template>
                        </InformationReceiver>
                      </xsl:if>
                      <xsl:if test="count(xhtml:td[6]//*[not(*)][string-length(normalize-space(text()))>0])">
                        <InformationProducer>
                          <xsl:call-template name="ParseCol">
                            <xsl:with-param name="ColIdx" select="6"/>
                            <xsl:with-param name="ElName" select="'name'"/>
                          </xsl:call-template>
                        </InformationProducer>
                      </xsl:if>
                      <xsl:if test="count(xhtml:td[8]//*[not(*)][string-length(normalize-space(text()))>0])">
                        <InformationModel>
                          <xsl:call-template name="ParseCol">
                            <xsl:with-param name="ColIdx" select="8"/>
                            <xsl:with-param name="ElName" select="'name'"/>
                          </xsl:call-template>
                        </InformationModel>
                      </xsl:if>
                      <xsl:call-template name="ParseCol">
                        <xsl:with-param name="ColIdx" select="9"/>
                        <xsl:with-param name="ElName" select="'id'"/>
                        <xsl:with-param name="SplitHead" select="'&lt;Requirement&gt;&lt;id&gt;'"/>
                        <xsl:with-param name="SplitTail" select="'&lt;/id&gt;&lt;/Requirement&gt;'"/>
                      </xsl:call-template>
                    </xsl:element>
                  </xsl:if>
                </xsl:for-each>
              </xsl:element>
            </xsl:if>
            <!--
						<xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space('Scenario Conditions')">
							<xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table[$TablePos]">
								<xsl:for-each select="xhtml:tr[(position() > 2) and (position() &lt; 1000)]">
									<xsl:if test="count(xhtml:td//*[not(*)][string-length(normalize-space(text()))>0])">
										<xsl:variable name="ScenarioName">
											<xsl:if test="count(xhtml:td[2]//*[not(*)][string-length(normalize-space(text()))>0])">
												<xsl:for-each select="xhtml:td[2]/xhtml:p">
													<xsl:apply-templates select="."/>
												</xsl:for-each>
											</xsl:if>
										</xsl:variable>
										<xsl:if test="$ScenarioName != ''">
											<xsl:element name="Scenario">
												<xsl:attribute name="location">
													<xsl:call-template name="FindHeaderOfTable2" />
												</xsl:attribute>
												<name>
													<xsl:value-of select="$ScenarioName"/>
												</name>
												<xsl:call-template name="ParseCol">
													<xsl:with-param name="ColIdx" select="1"/>
													<xsl:with-param name="ElName" select="'number'"/>
												</xsl:call-template>
												<xsl:if test="count(xhtml:td[6]//*[not(*)][string-length(normalize-space(text()))>0])">
													<xsl:for-each select="xhtml:td[6]/xhtml:p">
														<xsl:element name="postCondition">
															<xsl:attribute name="location">
																<xsl:call-template name="FindHeaderOfTable2" />
															</xsl:attribute>
															<xsl:apply-templates select="."/>				
														</xsl:element>
													</xsl:for-each>
												</xsl:if>											
												<xsl:if test="count(xhtml:td[5]//*[not(*)][string-length(normalize-space(text()))>0])">
													<xsl:for-each select="xhtml:td[5]/xhtml:p">
														<xsl:element name="preCondition">
															<xsl:attribute name="location">
																<xsl:call-template name="FindHeaderOfTable2" />
															</xsl:attribute>
															<xsl:apply-templates select="."/>			
														</xsl:element>
													</xsl:for-each>
												</xsl:if>																						
												<xsl:call-template name="ParseCol">
													<xsl:with-param name="ColIdx" select="4"/>
													<xsl:with-param name="ElName" select="'triggeringEvent'"/>
												</xsl:call-template>
												<PrimaryActor>
													<xsl:call-template name="ParseCol">
														<xsl:with-param name="ColIdx" select="3"/>
														<xsl:with-param name="ElName" select="'name'"/>
													</xsl:call-template>
												</PrimaryActor>
												<xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
													<xsl:if test="xhtml:tr[1]/xhtml:td[1]//*[not(*)][normalize-space(text())=normalize-space('Scenario')]">
														<xsl:variable name="thisScenario">
															<xsl:for-each select="xhtml:tr[2]/xhtml:td[2]/xhtml:p">
																<xsl:apply-templates select="."/>
															</xsl:for-each>
														</xsl:variable>
														<xsl:if test="$thisScenario=$ScenarioName">
															<xsl:for-each select="xhtml:tr[(position() > 3) and (position() &lt; 1000)]">
																<xsl:if test="count(xhtml:td//*[not(*)][string-length(normalize-space(text()))>0])">
																	<xsl:element name="Step">
																		<xsl:attribute name="location">
																			<xsl:call-template name="FindHeaderOfTable2" />
																		</xsl:attribute>
																		<xsl:call-template name="ParseCol">
																			<xsl:with-param name="ColIdx" select="4"/>
																			<xsl:with-param name="ElName" select="'description'"/>
																		</xsl:call-template>
																		<xsl:call-template name="ParseCol">
																			<xsl:with-param name="ColIdx" select="2"/>
																			<xsl:with-param name="ElName" select="'event'"/>
																		</xsl:call-template>
																		<xsl:call-template name="ParseCol">
																			<xsl:with-param name="ColIdx" select="3"/>
																			<xsl:with-param name="ElName" select="'name'"/>
																		</xsl:call-template>
																		<xsl:call-template name="ParseCol">
																			<xsl:with-param name="ColIdx" select="1"/>
																			<xsl:with-param name="ElName" select="'number'"/>
																		</xsl:call-template>
																		<xsl:call-template name="ParseCol">
																			<xsl:with-param name="ColIdx" select="5"/>
																			<xsl:with-param name="ElName" select="'service'"/>
																		</xsl:call-template>
																		<xsl:if test="count(xhtml:td[7]//*[not(*)][string-length(normalize-space(text()))>0])">
																			<InformationReceiver>
																				<xsl:call-template name="ParseCol">
																					<xsl:with-param name="ColIdx" select="7"/>
																					<xsl:with-param name="ElName" select="'name'"/>
																				</xsl:call-template>
																			</InformationReceiver>
																		</xsl:if>
																		<xsl:if test="count(xhtml:td[6]//*[not(*)][string-length(normalize-space(text()))>0])">
																			<InformationProducer>
																				<xsl:call-template name="ParseCol">
																					<xsl:with-param name="ColIdx" select="6"/>
																					<xsl:with-param name="ElName" select="'name'"/>
																				</xsl:call-template>
																			</InformationProducer>
																		</xsl:if>
																		<xsl:if test="count(xhtml:td[8]//*[not(*)][string-length(normalize-space(text()))>0])">
																			<InformationModel>
																				<xsl:call-template name="ParseCol">
																					<xsl:with-param name="ColIdx" select="8"/>
																					<xsl:with-param name="ElName" select="'name'"/>
																				</xsl:call-template>
																			</InformationModel>
																		</xsl:if>
																		<xsl:call-template name="ParseCol">
																			<xsl:with-param name="ColIdx" select="9"/>
																			<xsl:with-param name="ElName" select="'id'"/>
																			<xsl:with-param name="SplitHead" select="'&lt;Requirement&gt;&lt;id&gt;'"/>
																			<xsl:with-param name="SplitTail" select="'&lt;/id&gt;&lt;/Requirement&gt;'"/>																					
																		</xsl:call-template>																										
																	</xsl:element>
																</xsl:if>
															</xsl:for-each>
														</xsl:if>
													</xsl:if>
												</xsl:for-each>
											</xsl:element>
										</xsl:if>
									</xsl:if>
								</xsl:for-each>
							</xsl:for-each>
						</xsl:if>
						-->


          </xsl:for-each>
          <UseCaseRelation>
            <type>UNKNOWN... hard coded in xslt</type>
            <!--<RelatedUseCase/name>-->
            <xsl:call-template name="CommaDelimValue">
              <xsl:with-param name="TableIdentifier" select="'Classification Information'"/>
              <xsl:with-param name="FirstRowOffset" select="'2'"/>
              <xsl:with-param name="LastRowOffset" select="'4'"/>
              <xsl:with-param name="ColIdx1" select="1"/>
              <xsl:with-param name="head" select="'&lt;RelatedUseCase&gt;&lt;name&gt;'"/>
              <xsl:with-param name="tail" select="'&lt;/name&gt;&lt;/RelatedUseCase&gt;'"/>
            </xsl:call-template>
          </UseCaseRelation>
          <xsl:call-template name="FindAndParseTable">
            <xsl:with-param name="TableIdentifier" select="'Version Management'"/>
            <xsl:with-param name="RowName" select="'VersionInformation'"/>
            <xsl:with-param name="FirstRowOffset" select="2"/>
            <xsl:with-param name="ElName1" select="'approvalStatus'"/>
            <xsl:with-param name="ColIdx1" select="7"/>
            <xsl:with-param name="ElName2" select="'areaOfExpertise'"/>
            <xsl:with-param name="ColIdx2" select="5"/>
            <xsl:with-param name="ElName3" select="'changes'"/>
            <xsl:with-param name="ColIdx3" select="1"/>
            <xsl:with-param name="EmitHTML3" select="1"/>
            <xsl:with-param name="ElName4" select="'date'"/>
            <xsl:with-param name="ColIdx4" select="2"/>
            <xsl:with-param name="ElName5" select="'domainExpert'"/>
            <xsl:with-param name="ColIdx5" select="4"/>
            <xsl:with-param name="ElName6" select="'name'"/>
            <xsl:with-param name="ColIdx6" select="3"/>
            <xsl:with-param name="ElName7" select="'title'"/>
            <xsl:with-param name="ColIdx7" select="6"/>
          </xsl:call-template>
          <!--	
					<Documentation>
						<Drawings>
							<xsl:call-template name="FindAndParseTable">
								<xsl:with-param name="TableIdentifier" select="'Drawings'"/>
								<xsl:with-param name="RowName" select="'Drawing'"/>
								<xsl:with-param name="FirstRowOffset" select="'2'"/>
								<xsl:with-param name="ElName1" select="'Drawing'"/>
							</xsl:call-template>
						</Drawings>
					</Documentation>
					-->
        </UseCase>
      </UseCaseLibrary>
      <Locations>
        <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:h1 | /xhtml:html/xhtml:body/xhtml:h2">
          <xsl:element name="Location">
            <id>
              <xsl:value-of select="count(preceding-sibling::*) + 1"/>
            </id>
            <name>
              <xsl:apply-templates select="."/>
            </name>
          </xsl:element>
        </xsl:for-each>
      </Locations>
    </UC:UseCaseRepository>
  </xsl:template>
  <!-- 	
*************************************************************************************************

*************************************************************************************************
-->
  <xsl:template name="CommaDelimValue">
    <xsl:param name="TableIdentifier" select="''"/>
    <xsl:param name="FirstRowOffset" select="'0'"/>
    <xsl:param name="LastRowOffset" select="'0'"/>
    <xsl:param name="ColIdx1" select="0"/>
    <xsl:param name="head" select="0"/>
    <xsl:param name="tail" select="0"/>

    <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
      <xsl:variable name="TablePos" select="position()"/>
      <xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space($TableIdentifier)">
        <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table[$TablePos]">
          <xsl:for-each select="xhtml:tr[(position() &gt; $FirstRowOffset) and (position() &lt; $LastRowOffset)]">
            <!-- first cell must be present as it is the ID -->
            <xsl:if test="count(xhtml:td[$ColIdx1]//*[not(*)][string-length(normalize-space(text()))>0])">
              <xsl:variable name="Contents">
                <xsl:value-of select="xhtml:td[$ColIdx1]//*[not(*)]/text()"/>
              </xsl:variable>
              <xsl:choose>
                <xsl:when test="contains($Contents,',')">
                  <xsl:call-template name="split">
                    <xsl:with-param name="list" select="$Contents"/>
                    <xsl:with-param name="separator" select="','"/>
                    <xsl:with-param name="outhead" select="$head"/>
                    <xsl:with-param name="outtail" select="$tail"/>
                  </xsl:call-template>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="$head" disable-output-escaping="yes"/>
                  <xsl:value-of select="$Contents" disable-output-escaping="yes"/>
                  <xsl:value-of select="$tail" disable-output-escaping="yes"/>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:if>
          </xsl:for-each>
        </xsl:for-each>
      </xsl:if>
    </xsl:for-each>


  </xsl:template>
  <!-- 	
*************************************************************************************************

*************************************************************************************************
-->
  <xsl:template name="split">
    <xsl:param name="list" select="''"/>
    <xsl:param name="separator" select="','"/>
    <xsl:param name="outhead" select="''"/>
    <xsl:param name="outtail" select="''"/>
    <xsl:if test="not($list = '' or $separator = '')">
      <xsl:variable name="head" select="substring-before(concat($list, $separator), $separator)"/>
      <xsl:variable name="tail" select="substring-after($list, $separator)"/>
      <!-- insert payload function here -->
      <xsl:value-of select="$outhead" disable-output-escaping="yes"/>
      <xsl:value-of select="normalize-space($head)"/>
      <xsl:value-of select="$outtail" disable-output-escaping="yes"/>
      <xsl:call-template name="split">
        <xsl:with-param name="list" select="$tail"/>
        <xsl:with-param name="separator" select="$separator"/>
        <xsl:with-param name="outhead" select="$outhead"/>
        <xsl:with-param name="outtail" select="$outtail"/>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>
  <!-- 	
*************************************************************************************************

*************************************************************************************************
-->
  <xsl:template name="FindAndParseTable">
    <xsl:param name="TableIdentifier"/>
    <xsl:param name="RowName" select="''"/>
    <xsl:param name="FirstRowOffset" select="0"/>
    <xsl:param name="LastRowOffset" select="1000"/>
    <xsl:param name="ElName1" select="''"/>
    <xsl:param name="ElName2" select="''"/>
    <xsl:param name="ElName3" select="''"/>
    <xsl:param name="ElName4" select="''"/>
    <xsl:param name="ElName5" select="''"/>
    <xsl:param name="ElName6" select="''"/>
    <xsl:param name="ElName7" select="''"/>
    <xsl:param name="ElName8" select="''"/>
    <xsl:param name="ElName9" select="''"/>
    <xsl:param name="ElName10" select="''"/>
    <xsl:param name="ColIdx1" select="1"/>
    <xsl:param name="ColIdx2" select="2"/>
    <xsl:param name="ColIdx3" select="3"/>
    <xsl:param name="ColIdx4" select="4"/>
    <xsl:param name="ColIdx5" select="5"/>
    <xsl:param name="ColIdx6" select="6"/>
    <xsl:param name="ColIdx7" select="7"/>
    <xsl:param name="ColIdx8" select="8"/>
    <xsl:param name="ColIdx9" select="9"/>
    <xsl:param name="ColIdx10" select="10"/>
    <xsl:param name="EmitHTML1" select="0"/>
    <xsl:param name="EmitHTML2" select="0"/>
    <xsl:param name="EmitHTML3" select="0"/>
    <xsl:param name="EmitHTML4" select="0"/>
    <xsl:param name="EmitHTML5" select="0"/>
    <xsl:param name="EmitHTML6" select="0"/>
    <xsl:param name="EmitHTML7" select="0"/>
    <xsl:param name="EmitHTML8" select="0"/>
    <xsl:param name="EmitHTML9" select="0"/>
    <xsl:param name="EmitHTML10" select="0"/>
    <xsl:param name="SplitHead1" select="0"/>
    <xsl:param name="SplitHead2" select="0"/>
    <xsl:param name="SplitHead3" select="0"/>
    <xsl:param name="SplitHead4" select="0"/>
    <xsl:param name="SplitHead5" select="0"/>
    <xsl:param name="SplitHead6" select="0"/>
    <xsl:param name="SplitHead7" select="0"/>
    <xsl:param name="SplitHead8" select="0"/>
    <xsl:param name="SplitHead9" select="0"/>
    <xsl:param name="SplitHead10" select="0"/>
    <xsl:param name="SplitTail1" select="0"/>
    <xsl:param name="SplitTail2" select="0"/>
    <xsl:param name="SplitTail3" select="0"/>
    <xsl:param name="SplitTail4" select="0"/>
    <xsl:param name="SplitTail5" select="0"/>
    <xsl:param name="SplitTail6" select="0"/>
    <xsl:param name="SplitTail7" select="0"/>
    <xsl:param name="SplitTail8" select="0"/>
    <xsl:param name="SplitTail9" select="0"/>
    <xsl:param name="SplitTail10" select="0"/>
    <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table">
      <xsl:variable name="TablePos" select="position()"/>
      <xsl:if test="normalize-space(xhtml:tr[1]/xhtml:td[1]//*[not(*)]/text())=normalize-space($TableIdentifier)">
        <xsl:for-each select="/xhtml:html/xhtml:body/xhtml:table[$TablePos]">
          <xsl:call-template name="ParseTable">
            <xsl:with-param name="RowName" select="$RowName"/>
            <xsl:with-param name="FirstRowOffset" select="$FirstRowOffset"/>
            <xsl:with-param name="LastRowOffset" select="$LastRowOffset"/>
            <xsl:with-param name="ElName1" select="$ElName1"/>
            <xsl:with-param name="ElName2" select="$ElName2"/>
            <xsl:with-param name="ElName3" select="$ElName3"/>
            <xsl:with-param name="ElName4" select="$ElName4"/>
            <xsl:with-param name="ElName5" select="$ElName5"/>
            <xsl:with-param name="ElName6" select="$ElName6"/>
            <xsl:with-param name="ElName7" select="$ElName7"/>
            <xsl:with-param name="ElName8" select="$ElName8"/>
            <xsl:with-param name="ElName9" select="$ElName9"/>
            <xsl:with-param name="ElName10" select="$ElName10"/>
            <xsl:with-param name="ColIdx1" select="$ColIdx1"/>
            <xsl:with-param name="ColIdx2" select="$ColIdx2"/>
            <xsl:with-param name="ColIdx3" select="$ColIdx3"/>
            <xsl:with-param name="ColIdx4" select="$ColIdx4"/>
            <xsl:with-param name="ColIdx5" select="$ColIdx5"/>
            <xsl:with-param name="ColIdx6" select="$ColIdx6"/>
            <xsl:with-param name="ColIdx7" select="$ColIdx7"/>
            <xsl:with-param name="ColIdx8" select="$ColIdx8"/>
            <xsl:with-param name="ColIdx9" select="$ColIdx9"/>
            <xsl:with-param name="ColIdx10" select="$ColIdx10"/>
            <xsl:with-param name="EmitHTML1" select="$EmitHTML1"/>
            <xsl:with-param name="EmitHTML2" select="$EmitHTML2"/>
            <xsl:with-param name="EmitHTML3" select="$EmitHTML3"/>
            <xsl:with-param name="EmitHTML4" select="$EmitHTML4"/>
            <xsl:with-param name="EmitHTML5" select="$EmitHTML5"/>
            <xsl:with-param name="EmitHTML6" select="$EmitHTML6"/>
            <xsl:with-param name="EmitHTML7" select="$EmitHTML7"/>
            <xsl:with-param name="EmitHTML8" select="$EmitHTML8"/>
            <xsl:with-param name="EmitHTML9" select="$EmitHTML9"/>
            <xsl:with-param name="EmitHTML10" select="$EmitHTML10"/>
            <xsl:with-param name="SplitHead1" select="$SplitHead1"/>
            <xsl:with-param name="SplitHead2" select="$SplitHead2"/>
            <xsl:with-param name="SplitHead3" select="$SplitHead3"/>
            <xsl:with-param name="SplitHead4" select="$SplitHead4"/>
            <xsl:with-param name="SplitHead5" select="$SplitHead5"/>
            <xsl:with-param name="SplitHead6" select="$SplitHead6"/>
            <xsl:with-param name="SplitHead7" select="$SplitHead7"/>
            <xsl:with-param name="SplitHead8" select="$SplitHead8"/>
            <xsl:with-param name="SplitHead9" select="$SplitHead9"/>
            <xsl:with-param name="SplitHead10" select="$SplitHead10"/>
            <xsl:with-param name="SplitTail1" select="$SplitTail1"/>
            <xsl:with-param name="SplitTail2" select="$SplitTail2"/>
            <xsl:with-param name="SplitTail3" select="$SplitTail3"/>
            <xsl:with-param name="SplitTail4" select="$SplitTail4"/>
            <xsl:with-param name="SplitTail5" select="$SplitTail5"/>
            <xsl:with-param name="SplitTail6" select="$SplitTail6"/>
            <xsl:with-param name="SplitTail7" select="$SplitTail7"/>
            <xsl:with-param name="SplitTail8" select="$SplitTail8"/>
            <xsl:with-param name="SplitTail9" select="$SplitTail9"/>
            <xsl:with-param name="SplitTail10" select="$SplitTail10"/>
          </xsl:call-template>
        </xsl:for-each>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>
  <!-- 	
*************************************************************************************************

*************************************************************************************************
-->
  <xsl:template name="ParseTable">
    <xsl:param name="RowName"/>
    <xsl:param name="FirstRowOffset" select="0"/>
    <xsl:param name="LastRowOffset" select="1000"/>
    <xsl:param name="ElName1" select="''"/>
    <xsl:param name="ElName2" select="''"/>
    <xsl:param name="ElName3" select="''"/>
    <xsl:param name="ElName4" select="''"/>
    <xsl:param name="ElName5" select="''"/>
    <xsl:param name="ElName6" select="''"/>
    <xsl:param name="ElName7" select="''"/>
    <xsl:param name="ElName8" select="''"/>
    <xsl:param name="ElName9" select="''"/>
    <xsl:param name="ElName10" select="''"/>
    <xsl:param name="ColIdx1" select="1"/>
    <xsl:param name="ColIdx2" select="2"/>
    <xsl:param name="ColIdx3" select="3"/>
    <xsl:param name="ColIdx4" select="4"/>
    <xsl:param name="ColIdx5" select="5"/>
    <xsl:param name="ColIdx6" select="6"/>
    <xsl:param name="ColIdx7" select="7"/>
    <xsl:param name="ColIdx8" select="8"/>
    <xsl:param name="ColIdx9" select="9"/>
    <xsl:param name="ColIdx10" select="10"/>
    <xsl:param name="EmitHTML1" select="0"/>
    <xsl:param name="EmitHTML2" select="0"/>
    <xsl:param name="EmitHTML3" select="0"/>
    <xsl:param name="EmitHTML4" select="0"/>
    <xsl:param name="EmitHTML5" select="0"/>
    <xsl:param name="EmitHTML6" select="0"/>
    <xsl:param name="EmitHTML7" select="0"/>
    <xsl:param name="EmitHTML8" select="0"/>
    <xsl:param name="EmitHTML9" select="0"/>
    <xsl:param name="EmitHTML10" select="0"/>
    <xsl:param name="SplitHead1" select="0"/>
    <xsl:param name="SplitHead2" select="0"/>
    <xsl:param name="SplitHead3" select="0"/>
    <xsl:param name="SplitHead4" select="0"/>
    <xsl:param name="SplitHead5" select="0"/>
    <xsl:param name="SplitHead6" select="0"/>
    <xsl:param name="SplitHead7" select="0"/>
    <xsl:param name="SplitHead8" select="0"/>
    <xsl:param name="SplitHead9" select="0"/>
    <xsl:param name="SplitHead10" select="0"/>
    <xsl:param name="SplitTail1" select="0"/>
    <xsl:param name="SplitTail2" select="0"/>
    <xsl:param name="SplitTail3" select="0"/>
    <xsl:param name="SplitTail4" select="0"/>
    <xsl:param name="SplitTail5" select="0"/>
    <xsl:param name="SplitTail6" select="0"/>
    <xsl:param name="SplitTail7" select="0"/>
    <xsl:param name="SplitTail8" select="0"/>
    <xsl:param name="SplitTail9" select="0"/>
    <xsl:param name="SplitTail10" select="0"/>
    <xsl:for-each select="xhtml:tr[(position() > $FirstRowOffset) and (position() &lt; $LastRowOffset)]">
      <!-- first cell must be present as it is the ID -->
      <xsl:if test="count(xhtml:td//*[not(*)][string-length(normalize-space(text()))>0])">
        <xsl:if test="$RowName !=''">
          <xsl:text disable-output-escaping="yes">&lt;</xsl:text>
          <xsl:value-of select="$RowName"/>
          <xsl:call-template name="FindHeader2" />
          <xsl:text disable-output-escaping="yes">&gt;</xsl:text>
        </xsl:if>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx1"/>
          <xsl:with-param name="ElName" select="$ElName1"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML1"/>
          <xsl:with-param name="SplitHead" select="$SplitHead1"/>
          <xsl:with-param name="SplitTail" select="$SplitTail1"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx2"/>
          <xsl:with-param name="ElName" select="$ElName2"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML2"/>
          <xsl:with-param name="SplitHead" select="$SplitHead2"/>
          <xsl:with-param name="SplitTail" select="$SplitTail2"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx3"/>
          <xsl:with-param name="ElName" select="$ElName3"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML3"/>
          <xsl:with-param name="SplitHead" select="$SplitHead3"/>
          <xsl:with-param name="SplitTail" select="$SplitTail3"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx4"/>
          <xsl:with-param name="ElName" select="$ElName4"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML4"/>
          <xsl:with-param name="SplitHead" select="$SplitHead4"/>
          <xsl:with-param name="SplitTail" select="$SplitTail4"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx5"/>
          <xsl:with-param name="ElName" select="$ElName5"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML5"/>
          <xsl:with-param name="SplitHead" select="$SplitHead5"/>
          <xsl:with-param name="SplitTail" select="$SplitTail5"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx6"/>
          <xsl:with-param name="ElName" select="$ElName6"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML6"/>
          <xsl:with-param name="SplitHead" select="$SplitHead6"/>
          <xsl:with-param name="SplitTail" select="$SplitTail6"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx7"/>
          <xsl:with-param name="ElName" select="$ElName7"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML7"/>
          <xsl:with-param name="SplitHead" select="$SplitHead7"/>
          <xsl:with-param name="SplitTail" select="$SplitTail7"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx8"/>
          <xsl:with-param name="ElName" select="$ElName8"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML8"/>
          <xsl:with-param name="SplitHead" select="$SplitHead8"/>
          <xsl:with-param name="SplitTail" select="$SplitTail8"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx9"/>
          <xsl:with-param name="ElName" select="$ElName9"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML9"/>
          <xsl:with-param name="SplitHead" select="$SplitHead9"/>
          <xsl:with-param name="SplitTail" select="$SplitTail9"/>
        </xsl:call-template>
        <xsl:call-template name="ParseCol">
          <xsl:with-param name="ColIdx" select="$ColIdx10"/>
          <xsl:with-param name="ElName" select="$ElName10"/>
          <xsl:with-param name="EmitHTML" select="$EmitHTML10"/>
          <xsl:with-param name="SplitHead" select="$SplitHead10"/>
          <xsl:with-param name="SplitTail" select="$SplitTail10"/>
        </xsl:call-template>
        <xsl:if test="$RowName !=''">
          <xsl:text disable-output-escaping="yes">&lt;/</xsl:text>
          <xsl:value-of select="$RowName"/>
          <xsl:text disable-output-escaping="yes">&gt;</xsl:text>
        </xsl:if>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>
  <!-- 	
*************************************************************************************************

*************************************************************************************************
-->
  <xsl:template name="ParseCol">
    <xsl:param name="ColIdx"/>
    <xsl:param name="ElName"/>
    <xsl:param name="EmitHTML" select="0"/>
    <xsl:param name="SplitHead" select="0"/>
    <xsl:param name="SplitTail" select="0"/>
    <xsl:if test="$ElName != ''">
      <xsl:if test="count(xhtml:td[$ColIdx]//*[not(*)][string-length(normalize-space(text()))>0])">
        <xsl:choose>
          <xsl:when test="$SplitHead != 0">
            <xsl:variable name="CellValue">
              <xsl:for-each select="xhtml:td[$ColIdx]/xhtml:p">
                <xsl:apply-templates select="."/>
              </xsl:for-each>
            </xsl:variable>
            <xsl:call-template name="split">
              <xsl:with-param name="list" select="$CellValue"/>
              <xsl:with-param name="separator" select="','"/>
              <xsl:with-param name="outhead" select="$SplitHead"/>
              <xsl:with-param name="outtail" select="$SplitTail"/>
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text disable-output-escaping="yes">&lt;</xsl:text>
            <xsl:value-of select="$ElName"/>
            <xsl:call-template name="FindHeader2" />
            <xsl:text disable-output-escaping="yes">&gt;</xsl:text>
            <xsl:choose>
              <xsl:when test="$EmitHTML != 0">
                <xsl:for-each select="xhtml:td[$ColIdx]">
                  <xsl:apply-templates select="." mode="RawHTML"/>
                </xsl:for-each>
              </xsl:when>
              <xsl:otherwise>
                <xsl:for-each select="xhtml:td[$ColIdx]/xhtml:p">
                  <xsl:apply-templates select="."/>
                </xsl:for-each>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:text disable-output-escaping="yes">&lt;/</xsl:text>
            <xsl:value-of select="$ElName"/>
            <xsl:text disable-output-escaping="yes">&gt;</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:if>
    </xsl:if>
  </xsl:template>
  <!-- 	
*************************************************************************************************

*************************************************************************************************
-->
  <xsl:template name="ParseColBulletList">
    <xsl:param name="ColIdx"/>
    <xsl:param name="ElName"/>
    <xsl:param name="EmitHTML" select="0"/>
    <xsl:if test="$ElName != ''">
      <xsl:if test="count(xhtml:td[$ColIdx]//*[not(*)][string-length(normalize-space(text()))>0])">
        <xsl:for-each select="xhtml:td[$ColIdx]/xhtml:p">
          <!--<xsl:element name="$ElName">-->
          <xsl:text disable-output-escaping="yes">&lt;</xsl:text>
          <xsl:value-of select="$ElName"/>
          <xsl:text disable-output-escaping="yes">&gt;</xsl:text>
          <xsl:apply-templates select="."/>
          <xsl:text disable-output-escaping="yes">&lt;/</xsl:text>
          <xsl:value-of select="$ElName"/>
          <xsl:text disable-output-escaping="yes">&gt;</xsl:text>
          <!--</xsl:element>-->
        </xsl:for-each>
      </xsl:if>
    </xsl:if>
  </xsl:template>

  <xsl:template name="FindHeader">
    <xsl:variable name="h1Index" select="count(preceding-sibling::xhtml:h1[1]/preceding-sibling::*)"/>
    <xsl:variable name="h2Index" select="count(preceding-sibling::xhtml:h2[1]/preceding-sibling::*)"/>
    <xsl:choose>
      <xsl:when test="$h1Index > $h2Index">
        <xsl:text disable-output-escaping="yes"> location="</xsl:text>
        <xsl:value-of select="count(preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">"</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text disable-output-escaping="yes"> location="</xsl:text>
        <xsl:value-of select="count(preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">.</xsl:text>
        <xsl:value-of select="count(preceding-sibling::xhtml:h2[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">"</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="FindHeader2">
    <xsl:variable name="h1Index" select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h1[1]/preceding-sibling::*)"/>
    <xsl:variable name="h2Index" select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h2[1]/preceding-sibling::*)"/>
    <xsl:choose>
      <xsl:when test="$h1Index > $h2Index">
        <xsl:text disable-output-escaping="yes"> location="</xsl:text>
        <xsl:value-of select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">"</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text disable-output-escaping="yes"> location="</xsl:text>
        <xsl:value-of select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">.</xsl:text>
        <xsl:value-of select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h2[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">"</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="FindHeaderOfTable">
    <xsl:variable name="h1Index" select="count(preceding-sibling::xhtml:h1[1]/preceding-sibling::*)"/>
    <xsl:variable name="h2Index" select="count(preceding-sibling::xhtml:h2[1]/preceding-sibling::*)"/>
    <xsl:choose>
      <xsl:when test="$h1Index > $h2Index">
        <!-- <xsl:text disable-output-escaping="yes"> location="</xsl:text> -->
        <xsl:value-of select="count(preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <!-- <xsl:text disable-output-escaping="yes">"</xsl:text> -->
      </xsl:when>
      <xsl:otherwise>
        <!-- <xsl:text disable-output-escaping="yes"> location="</xsl:text> -->
        <xsl:value-of select="count(preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">.</xsl:text>
        <xsl:value-of select="count(preceding-sibling::xhtml:h2[1]/preceding-sibling::*) + 1"/>
        <!-- <xsl:text disable-output-escaping="yes">"</xsl:text> -->
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="FindHeaderOfTable2">
    <xsl:variable name="h1Index" select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h1[1]/preceding-sibling::*)"/>
    <xsl:variable name="h2Index" select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h2[1]/preceding-sibling::*)"/>
    <xsl:choose>
      <xsl:when test="$h1Index > $h2Index">
        <!-- <xsl:text disable-output-escaping="yes"> location="</xsl:text> -->
        <xsl:value-of select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <!-- <xsl:text disable-output-escaping="yes">"</xsl:text> -->
      </xsl:when>
      <xsl:otherwise>
        <!-- <xsl:text disable-output-escaping="yes"> location="</xsl:text> -->
        <xsl:value-of select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h1[1]/preceding-sibling::*) + 1"/>
        <xsl:text disable-output-escaping="yes">.</xsl:text>
        <xsl:value-of select="count(ancestor::xhtml:table/preceding-sibling::xhtml:h2[1]/preceding-sibling::*) + 1"/>
        <!-- <xsl:text disable-output-escaping="yes">"</xsl:text> -->
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="xhtml:h3">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:h4">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:h5">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:h6">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoHeading7']">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:b">
    <xsl:choose>
      <xsl:when test="EmitHTML !=0">
        <b>
          <xsl:apply-templates select="node()"/>
        </b>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="node()"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="xhtml:i">
    <xsl:apply-templates select="node()"/>
  </xsl:template>
  <xsl:template match="xhtml:u">
    <xsl:apply-templates select="node()"/>
  </xsl:template>
  <xsl:template match="xhtml:s">
    <xsl:apply-templates select="node()"/>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpFirst c10']">
    <xsl:text disable-output-escaping="yes">&lt;ul&gt;</xsl:text>
    <li>
      <xsl:apply-templates select="node()"/>
    </li>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpMiddle c10']">
    <li>
      <xsl:apply-templates select="node()"/>
    </li>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpMiddle c11']">
    <li>
      <xsl:apply-templates select="node()"/>
    </li>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpMiddle c12']">
    <li>
      <xsl:apply-templates select="node()"/>
    </li>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpLast c10']">
    <li>
      <xsl:apply-templates select="node()"/>
    </li>
    <xsl:text disable-output-escaping="yes">&lt;/ul&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpFirst c11']">
    <xsl:text disable-output-escaping="yes">&lt;ul&gt;</xsl:text>
    <li>
      <xsl:apply-templates select="node()"/>
    </li>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpLast c11']">
    <li>
      <xsl:apply-templates select="node()"/>
    </li>
    <xsl:text disable-output-escaping="yes">&lt;/ul&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='dtContentDescription']">
    <xsl:apply-templates select="node()"/>
  </xsl:template>
  <xsl:template match="xhtml:span">
    <xsl:apply-templates select="node()"/>
  </xsl:template>
  <xsl:template match="xhtml:span[@class='c2']">
    <xsl:apply-templates select="node()"/>
  </xsl:template>
  <xsl:template match="text()">
    <xsl:value-of select="normalize-space()"/>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoNormal']">
    <xsl:apply-templates select="node()"/>
  </xsl:template>
  <xsl:template match="xhtml:img">
    <xsl:text>&lt;img width="</xsl:text>
    <xsl:value-of select="@width"/>
    <xsl:text>" height="</xsl:text>
    <xsl:value-of select="@height"/>
    <xsl:text>" src="</xsl:text>
    <xsl:value-of select="@src"/>
    <xsl:text>" id="</xsl:text>
    <xsl:value-of select="@id"/>
    <xsl:text>"</xsl:text>
    <xsl:if test="../@align">
      <xsl:text> align="</xsl:text>
      <xsl:value-of select="../@align"/>
      <xsl:text>"</xsl:text>
    </xsl:if>
    <xsl:if test="../@style">
      <xsl:text> style="</xsl:text>
      <xsl:value-of select="../@style"/>
      <xsl:text>"</xsl:text>
    </xsl:if>
    <xsl:text>&gt;</xsl:text>
  </xsl:template>
  <!--
*************************************************************

*************************************************************
-->
  <xsl:template match="xhtml:h3" mode="RawHTML">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:h4" mode="RawHTML">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:h5" mode="RawHTML">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:h6" mode="RawHTML">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoHeading7']" mode="RawHTML">
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
    <!-- cr -->
  </xsl:template>
  <xsl:template match="xhtml:b" mode="RawHTML">
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/b&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:i" mode="RawHTML">
    <xsl:apply-templates select="node()" mode="RawHTML"/>
  </xsl:template>
  <xsl:template match="xhtml:u" mode="RawHTML">
    <xsl:text>&lt;u&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/u&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:sup" mode="RawHTML">
    <xsl:text>&lt;sup&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/sup&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:sub" mode="RawHTML">
    <xsl:text>&lt;sub&gt;</xsl:text>
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:text>&lt;/sub&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:s" mode="RawHTML">
    <xsl:apply-templates select="node()" mode="RawHTML"/>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpFirst']" mode="RawHTML">
    <xsl:if test="./xhtml:span[@style='font-family:Symbol']">
      <xsl:text>&lt;ul&gt;</xsl:text>
    </xsl:if>
    <xsl:if test="not(./xhtml:span[@style='font-family:Symbol'])">
      <xsl:text>&lt;ol&gt;</xsl:text>
    </xsl:if>
    <xsl:text>&lt;li&gt;</xsl:text>
    <xsl:for-each select="node()">
      <xsl:if test="position()>1">
        <xsl:apply-templates select="." mode="RawHTML"/>
      </xsl:if>
    </xsl:for-each>
    <xsl:text>&lt;/li&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpMiddle']" mode="RawHTML">
    <xsl:text>&lt;li&gt;</xsl:text>
    <xsl:for-each select="node()">
      <xsl:if test="position()>1">
        <xsl:apply-templates select="." mode="RawHTML"/>
      </xsl:if>
    </xsl:for-each>
    <xsl:text>&lt;/li&gt;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoListParagraphCxSpLast']" mode="RawHTML">
    <xsl:text>&lt;li&gt;</xsl:text>
    <xsl:for-each select="node()">
      <xsl:if test="position()>1">
        <xsl:apply-templates select="." mode="RawHTML"/>
      </xsl:if>
    </xsl:for-each>
    <xsl:text>&lt;/li&gt;</xsl:text>
    <xsl:if test="./xhtml:span[@style='font-family:Symbol']">
      <xsl:text>&lt;/ul&gt;</xsl:text>
    </xsl:if>
    <xsl:if test="not(./xhtml:span[@style='font-family:Symbol'])">
      <xsl:text>&lt;/ol&gt;</xsl:text>
    </xsl:if>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='dtContentDescription']" mode="RawHTML">
    <xsl:apply-templates select="node()" mode="RawHTML"/>
  </xsl:template>
  <xsl:template match="xhtml:span[contains(@style,'color:')]" mode="RawHTML">
    <xsl:variable name="StrColor" select="substring-before(substring-after(@style,'color:'),';')"/>
    <xsl:choose>
      <xsl:when test="$StrColor='red'">
        <xsl:text>&lt;font color="#ff0000"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Dark Red'">
        <xsl:text>&lt;font color="#C00000"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Red'">
        <xsl:text>&lt;font color="#FF0000"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Orange'">
        <xsl:text>&lt;font color="#FFC000"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='yellow'">
        <xsl:text>&lt;font color="#FFFF00"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Light Green'">
        <xsl:text>&lt;font color="#92D050"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Green'">
        <xsl:text>&lt;font color="#00B050"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Light Blue'">
        <xsl:text>&lt;font color="#00B0F0"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Blue'">
        <xsl:text>&lt;font color="#0070C0"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Dark Blue'">
        <xsl:text>&lt;font color="#002060"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:when test="$StrColor='Purple'">
        <xsl:text>&lt;font color="#7030A0"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>&lt;font color="</xsl:text>
        <xsl:value-of select="$StrColor"/>
        <xsl:text>"&gt;</xsl:text>
        <xsl:apply-templates select="node()" mode="RawHTML"/>
        <xsl:text>&lt;/font&gt;</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="xhtml:span" mode="RawHTML">
    <xsl:apply-templates select="node()" mode="RawHTML"/>
  </xsl:template>
  <xsl:template match="xhtml:span[@class='c2']" mode="RawHTML">
    <xsl:apply-templates select="node()" mode="RawHTML"/>
  </xsl:template>
  <xsl:template match="text()" mode="RawHTML">
    <xsl:value-of select="."/>
  </xsl:template>
  <xsl:template match="xhtml:p[@class='MsoNormal']" mode="RawHTML">
    <xsl:apply-templates select="node()" mode="RawHTML"/>
    <xsl:if test=". = ' '">
      <xsl:text> </xsl:text>
    </xsl:if>
    <xsl:text disable-output-escaping="yes">&amp;#13;&amp;#10;</xsl:text>
  </xsl:template>
  <xsl:template match="xhtml:img" mode="RawHTML">
    <xsl:text>&lt;img width="</xsl:text>
    <xsl:value-of select="@width"/>
    <xsl:text>" height="</xsl:text>
    <xsl:value-of select="@height"/>
    <xsl:text>" src="</xsl:text>
    <xsl:value-of select="@src"/>
    <xsl:text>" id="</xsl:text>
    <xsl:value-of select="@id"/>
    <xsl:text>"</xsl:text>
    <xsl:if test="../@align">
      <xsl:text>" align="</xsl:text>
      <xsl:value-of select="../@align"/>
    </xsl:if>
    <xsl:text>"</xsl:text>
    <xsl:if test="../@style">
      <xsl:text>" style="</xsl:text>
      <xsl:value-of select="../@style"/>
    </xsl:if>
    <xsl:text>"</xsl:text>
    <xsl:text>&gt;</xsl:text>
  </xsl:template>
</xsl:stylesheet>
