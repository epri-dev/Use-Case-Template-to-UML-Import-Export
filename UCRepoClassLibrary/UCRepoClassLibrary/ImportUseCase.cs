/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportUseCase.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 9 $

	Date last modified: $Modtime: 11/05/12 11:27a $

	Project	: 
	Target	: 
	Tabs	: 

	Abstract:

Copyright (c) 2012 EPRI

Licensed by EPRI under one or more contributor license agreements.

See the NOTICE file distributed with this work for additional 
information regarding copyright ownership. EPRI licenses this 
file to you under the Apache License, Version 2.0 (the "; License; "); 
you may not use this file except in compliance with the License.
You may obtain a copy of the License at:
http://www.apache.org/licenses/LICENSE-2.0
  
Unless required by applicable law or agreed to in writing, software 
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
implied.  See the License for the specific language governing 
permissions and limitations under the License.
--------------------------------------------------------------------------
**********************	 Revision History.   *****************************
--------------------------------------------------------------------------
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportUseCase.cs 9     11/05/12 11:31a Ronp $

$History: ImportUseCase.cs $
 * 
 * *****************  Version 9  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary
 * Update keyword expansion

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Collections;



namespace UCRepoClassLibrary
{
    internal class ImportUseCase
    {
        #region Properties

        public ImportSequences m_Sequences = new ImportSequences();
        public ImportActivities m_Activities = new ImportActivities();

        private int m_NoUseCaseComponents = 0;
        private int m_NoUseCaseActors = 0;
        #endregion
       
        private int AddUCComponentToDiagram(EA.Package eAMainPackage, EA.Diagram diagram, int iElementID)
        {
            int iTopOffset = 10;
            int iHeightOneStep = 200;
            int iSpaceBetweenSteps = 20;

            int iL = 100;
            int iR = 200;
            int iT = iTopOffset + (iHeightOneStep * m_NoUseCaseComponents) + iSpaceBetweenSteps;
            int iB = iTopOffset + (iHeightOneStep * (m_NoUseCaseComponents + 1));

            string strPos = "l=" + iL.ToString() + ";r=" + iR.ToString() + ";t=" + iT.ToString() + ";b=" + iB.ToString() + ";";

            EA.DiagramObject diagramObj = diagram.DiagramObjects.AddNew(strPos, "");
            if (diagramObj == null)
            {
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 1;
            }

            diagramObj.ElementID = iElementID;
            diagramObj.Update();
            diagram.DiagramObjects.Refresh();
            eAMainPackage.Diagrams.Refresh();
            diagram.Update();

            m_NoUseCaseComponents++;

            return 0;
        }

        private int ProcessModelObject(string strLinksFolder, EA.Package eAMainPackage, EA.Package eaPackage, EA.Diagram diagram, ImportFieldsValidator oFieldsValidator, int iBaseClassID, string strImportingData, string strObjName = "")
        {
            int iErrCnt = oFieldsValidator.ParseAndValidateFields(strImportingData);

            if (iErrCnt == 0)
            {
                oFieldsValidator.ReplaceImageLinks(strLinksFolder);

                string strLogInfo = "";

                if (strObjName == "")
                {
                    strLogInfo = strImportingData + ": " + (string)oFieldsValidator[oFieldsValidator.GetIDStrName()].Value;
                }
                else
                {
                    strLogInfo = strImportingData + ": " + strObjName;
                }

                EA.Element element1 = null;
                EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

                if (strObjName == "")
                {
                    element1 = eaPackage.Elements.AddNew((string)oFieldsValidator[oFieldsValidator.GetIDStrName()].Value, "Object");
                }
                else
                {
                    element1 = eaPackage.Elements.AddNew(strObjName, "Object");
                }
                if (oFieldsValidator.GetNotesStrName() != "")
                {
                    element1.Notes = oFieldsValidator[oFieldsValidator.GetNotesStrName()].Value;
                }
                element1.ClassifierID = iBaseClassID;

                string strRunState = "";

                foreach (DictionaryEntry deFieldIndo in oFieldsValidator.GetFields())
                {
                    FieldInfo oFieldInfo = (FieldInfo)deFieldIndo.Value;
                    if ((string)oFieldInfo.Value != "")
                    {
                        if (!oFieldInfo.bIsID && !oFieldInfo.bIsNote)
                        {
                            strRunState = strRunState + "@VAR;Variable=" + oFieldInfo.ValueName + ";Value=" + (string)oFieldInfo.Value + ";Op==;@ENDVAR;";
                        }
                        if (oFieldInfo.bIsNote)
                        {
                            strRunState = strRunState + "@VAR;Variable=" + oFieldInfo.ValueName + ";Value=See notes for this object;Op==;@ENDVAR;";
                        }
                    }
                }
                element1.RunState = strRunState;
                element1.Update();

                eaPackage.Elements.Refresh();

                AddUCComponentToDiagram(eAMainPackage, diagram, element1.ElementID);
            }

            return iErrCnt;
        }

        /// <summary>
        /// Adds version info to version info package
        /// </summary>
        /// <param name="elUseCase"></param>
        private int ProcessReferenceInfo(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;

            EA.Package eaPackageReferences = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageReferences == null) { iErrorCount++; };

            if (eaPackageReferences != null)
            {
                IEnumerable<XElement> elReferneces = elUseCase.Elements("Reference");

                foreach (XElement elReference in elReferneces)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                    oFieldsValidator.Add(new FieldInfo("description", elReference, "", true, false, true));
                    oFieldsValidator.Add(new FieldInfo("id", elReference, "", true, true, false));
                    oFieldsValidator.Add(new FieldInfo("impact", elReference));
                    oFieldsValidator.Add(new FieldInfo("originatorOrganisation", elReference));
                    oFieldsValidator.Add(new FieldInfo("status", elReference, "", false));
                    oFieldsValidator.Add(new FieldInfo("type", elReference));
                    oFieldsValidator.Add(new FieldInfo("URI", elReference));

                    int iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageReferences, diagram, oFieldsValidator, EAImporter.m_iReferenceInfoBaseClsID, "ReferenceInformation");

                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }



        /// <summary>
        /// Adds version info to version info package
        /// </summary>
        /// <param name="elUseCase"></param>
        private int ProcessVersionInfo(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;

            EA.Package eaPackageVersions = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageVersions == null) { iErrorCount++; };

            if (eaPackageVersions != null)
            {

                IEnumerable<XElement> elVersions = elUseCase.Elements("VersionInformation");

                foreach (XElement elVersionInformation in elVersions)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                    oFieldsValidator.Add(new FieldInfo("approvalStatus", elVersionInformation));
                    oFieldsValidator.Add(new FieldInfo("areaOfExpertise", elVersionInformation));
                    oFieldsValidator.Add(new FieldInfo("changes", elVersionInformation, "", true, false, true));
                    oFieldsValidator.Add(new FieldInfo("date", elVersionInformation, "", true, true, false));
                    oFieldsValidator.Add(new FieldInfo("domainExpert", elVersionInformation));
                    oFieldsValidator.Add(new FieldInfo("name", elVersionInformation,"",false));
                    oFieldsValidator.Add(new FieldInfo("title", elVersionInformation));

                    int iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageVersions, diagram, oFieldsValidator, EAImporter.m_iVersionInfoBaseClsID, "VersionInformation");

                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }

        private int ProcessRemarks(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;

            EA.Package eaPackageVersions = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageVersions == null) { iErrorCount++; };

            if (eaPackageVersions != null)
            {
                IEnumerable<XElement> elVersions = elUseCase.Elements("GeneralRemark");

                foreach (XElement elVersionInformation in elVersions)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                    oFieldsValidator.Add(new FieldInfo("content", elVersionInformation, "", true, true, true));

                    int iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageVersions, diagram, oFieldsValidator, EAImporter.m_iGeneralRemarkBaseClsID, "GeneralRemark");

                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }

        private int ProcessObjectives(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;

            EA.Package eaPackageVersions = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageVersions == null) { iErrorCount++; };

            if (eaPackageVersions != null)
            {
                IEnumerable<XElement> elVersions = elUseCase.Elements("RelatedObjective");

                foreach (XElement elVersionInformation in elVersions)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                    oFieldsValidator.Add(new FieldInfo("name", elVersionInformation, "", true, true, false));

                    int iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageVersions, diagram, oFieldsValidator, EAImporter.m_iObjectiveBaseClsID, "Objective");

                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }

        private int ProcessBusinessCases(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;

            EA.Package eaPackageVersions = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageVersions == null) { iErrorCount++; };

            if (eaPackageVersions != null)
            {
                IEnumerable<XElement> elVersions = elUseCase.Elements("BusinessCase");

                foreach (XElement elVersionInformation in elVersions)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                    oFieldsValidator.Add(new FieldInfo("name", elVersionInformation, "", true, true, false));

                    int iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageVersions, diagram, oFieldsValidator, EAImporter.m_iBusinessCaseBaseClsID, "BusinessCase");

                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }

        private int ProcessAdditionalDomains(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;
            EA.Package eaPackageVersions = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageVersions == null) { iErrorCount++; };

            if (eaPackageVersions != null)
            {
                IEnumerable<XElement> elVersions = elUseCase.Elements("AdditionalDomain");

                foreach (XElement elVersionInformation in elVersions)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                    oFieldsValidator.Add(new FieldInfo("name", elVersionInformation, "", true, true, false));

                    int iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageVersions, diagram, oFieldsValidator, EAImporter.m_iRef_DomainBaseClsID, "AdditionalDomain");

                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }

        private int ProcessRelations(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;
            EA.Package eaPackageUseCaseRelations = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageUseCaseRelations == null) { iErrorCount++; };

            if (eaPackageUseCaseRelations != null)
            {
                IEnumerable<XElement> elUseCaseRelations = elUseCase.Elements("UseCaseRelation");
                int idx = 1;
                foreach (XElement elUseCaseRelation in elUseCaseRelations)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();
                    oFieldsValidator.Add(new FieldInfo("type", elUseCaseRelation, "", false));

                    int iErrCnt = oFieldsValidator.ParseAndValidateFields("Use Case Relations");

                    if (iErrCnt == 0)
                    {
                        string strRelationName = "UseCaseRelation_" + idx;

                        EA.Package eaPackageRelation = EAImporter.m_Packages.Add(strUseCasePath + "/UseCaseRelations", strRelationName, "");
                        if (eaPackageRelation == null) { iErrorCount++; };

                        if (eaPackageRelation != null)
                        {
                            iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageRelation, diagram, oFieldsValidator, EAImporter.m_iUseCaseRelationsClsID, "UseCaseRelation", strRelationName);

                            iErrorCount = iErrorCount + iErrCnt;
                        }

                        IEnumerable<XElement> elRelatedUseCases = elUseCaseRelation.Elements("RelatedUseCase");
                        int idx2 = 1;
                        foreach (XElement elRelatedUseCase in elRelatedUseCases)
                        {
                            ImportFieldsValidator oFieldsValidator2 = new ImportFieldsValidator();
                            oFieldsValidator2.Add(new FieldInfo("name", elRelatedUseCase, "", true, true));

                            iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageRelation, diagram, oFieldsValidator2, EAImporter.m_iRefUseCaseClsID, "RelatedUseCase");

                            iErrorCount = iErrorCount + iErrCnt;

                            idx2++;
                        }

                    }

                    iErrorCount = iErrorCount + iErrCnt;

                    idx++;
                }
            }

            return iErrorCount;
        }

        private int ProcessConditions(string strUseCasePath, string strPackageName, EA.Package eAMainPackage, XElement elUseCase, EA.Diagram diagram)
        {
            int iErrorCount = 0;
            EA.Package eaPackageConditions = EAImporter.m_Packages.Add(strUseCasePath, strPackageName, "");
            if (eaPackageConditions == null) { iErrorCount++; };

            if (eaPackageConditions != null)
            {
                IEnumerable<XElement> elConditions = elUseCase.Elements("Condition");
                int idx = 1;
                foreach (XElement elCondition in elConditions)
                {
                    ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();
                    oFieldsValidator.Add(new FieldInfo("assumption", elCondition, "", false));
                    oFieldsValidator.Add(new FieldInfo("trigerringEvent", elCondition, "", false));

                    int iErrCnt = oFieldsValidator.ParseAndValidateFields("Use Case Conditions");

                    if (iErrCnt == 0)
                    {
                        string strConditionName = "Condition_" + idx;

                        EA.Package eaPackageCondition = EAImporter.m_Packages.Add(strUseCasePath + "/Conditions", strConditionName, "");
                        if (eaPackageCondition == null) { iErrorCount++; };

                        if (eaPackageCondition != null)
                        {
 //                           iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName,eAMainPackage, eaPackageCondition, diagram, oFieldsValidator, EAImporter.m_iConditionBaseClsID, "Condition", strConditionName);
                            iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName,eAMainPackage, eaPackageCondition, diagram, oFieldsValidator, EAImporter.m_iConditionBaseClsID, "Condition", "Condition");

                            iErrorCount = iErrorCount + iErrCnt;
                        }

                        IEnumerable<XElement> elPreConditions = elCondition.Elements("PreCondition");
                        int idx2 = 1;
                        foreach (XElement elPreCondition in elPreConditions)
                        {
                            ImportFieldsValidator oFieldsValidator2 = new ImportFieldsValidator();
                            oFieldsValidator2.Add(new FieldInfo("content", elPreCondition));

                            string strPreConditionName = "PreCondition_" + idx2;

                            //iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageCondition, diagram, oFieldsValidator2, EAImporter.m_iPreConditionBaseClsID, "PreCondition", strPreConditionName);
                            iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageCondition, diagram, oFieldsValidator2, EAImporter.m_iPreConditionBaseClsID, "PreCondition", "PreCondition");

                            iErrorCount = iErrorCount + iErrCnt;

                            idx2++;
                        }

                        IEnumerable<XElement> elRefActors = elCondition.Elements("referencedActor");
                        int idx3 = 1;
                        foreach (XElement elRefActor in elRefActors)
                        {
                            ImportFieldsValidator oFieldsValidator3 = new ImportFieldsValidator();
                            oFieldsValidator3.Add(new FieldInfo("name", elRefActor));

                            string strRefActorName = "ReferencedActor_" + idx3;

                            //iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageCondition, diagram, oFieldsValidator3, EAImporter.m_iRef_ActorClsID, "ReferencedActor", strRefActorName);
                            iErrCnt = ProcessModelObject(strUseCasePath + "/" + strPackageName, eAMainPackage, eaPackageCondition, diagram, oFieldsValidator3, EAImporter.m_iRef_ActorClsID, "ReferencedActor", "ReferencedActor");

                            iErrorCount = iErrorCount + iErrCnt;

                            idx3++;
                        }

                    }

                    iErrorCount = iErrorCount + iErrCnt;

                    idx++;
                }
            }

            return iErrorCount;
        }

        private int ProcessNarrative(EA.Package eaPackageEnterprise, EA.Package eaPackageUseCase, ImportFieldsValidator oFieldsValidator, EA.Diagram diagram)
        {
            string strRunState = "";

            EA.Element element2 = eaPackageEnterprise.Elements.AddNew("Narrative", "Object");
            element2.Notes = oFieldsValidator["completeDescription"].Value;
            element2.ClassifierID = EAImporter.m_iNarrativeBaseClsID;
            strRunState = "";

            //strRunState = strRunState + "@VAR;Variable=" + "shortDescription" + ";Value=" + (string)oFieldsValidator["shortDescription"].Value + ";Op==;@ENDVAR;";
            strRunState = strRunState + oFieldsValidator["shortDescription"].GetRunStateString();
            strRunState = strRunState + "@VAR;Variable=" + "completeDescription" + ";Value=See object notes;Op==;@ENDVAR;";

            element2.RunState = strRunState;
            element2.Update();

            AddUCComponentToDiagram(eaPackageUseCase, diagram, element2.ElementID);

            return 0;
        }

        private int ProcessUseCaseFlatAttributes(EA.Package eaPackageEnterprise, EA.Package eaPackageUseCase, ImportFieldsValidator oFieldsValidator, EA.Diagram diagram, ref EA.Element elUCClassElement)
        {

            elUCClassElement = eaPackageEnterprise.Elements.AddNew("UseCase", "Object");
            elUCClassElement.ClassifierID = EAImporter.m_UseCaseBaseClsID;
            string strRunState = "";

            strRunState = strRunState + oFieldsValidator["name"].GetRunStateString();
            strRunState = strRunState + oFieldsValidator["id"].GetRunStateString();
            strRunState = strRunState + oFieldsValidator["classification"].GetRunStateString();
            strRunState = strRunState + oFieldsValidator["keywords"].GetRunStateString();
            strRunState = strRunState + oFieldsValidator["levelOfDepth"].GetRunStateString();
            strRunState = strRunState + oFieldsValidator["prioritisation"].GetRunStateString();
            strRunState = strRunState + oFieldsValidator["scope"].GetRunStateString();
            strRunState = strRunState + oFieldsValidator["viewPoint"].GetRunStateString();

            elUCClassElement.RunState = strRunState;
            elUCClassElement.Update();

            AddUCComponentToDiagram(eaPackageUseCase, diagram, elUCClassElement.ElementID);


            return 0;
        }

        private int AddActorToUCDiagram(EA.Package eaPackage, EA.Diagram diagram, int iElementID)
        {
            int iTopOffset = 10;
            int iHeightOneStep = 200;
            int iSpaceBetweenSteps = 20;

            int iL = 100;
            int iR = 200;
            int iT = iTopOffset + (iHeightOneStep * m_NoUseCaseActors) + iSpaceBetweenSteps;
            int iB = iTopOffset + (iHeightOneStep * (m_NoUseCaseActors + 1));

            string strPos = "l=" + iL.ToString() + ";r=" + iR.ToString() + ";t=" + iT.ToString() + ";b=" + iB.ToString() + ";";

            EA.DiagramObject diagramObj = diagram.DiagramObjects.AddNew(strPos, "");
            if (diagramObj == null)
            {
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 1;
            }

            diagramObj.ElementID = iElementID;
            diagramObj.Update();
            diagram.DiagramObjects.Refresh();
            eaPackage.Diagrams.Refresh();
            diagram.Update();

            m_NoUseCaseActors++;

            return 0;
        }

        private int AddUCToUCDiagram(EA.Package eaPackage, EA.Diagram diagram, int iUCElementID, int iUCClassElementID, int iVertPos)
        {
            int iTopOffset = 10;
            int iHeightOneStep = 200;
            int iSpaceBetweenSteps = 20;

            int iL = 300;
            int iR = 400;
            int iT = iTopOffset + (iHeightOneStep * iVertPos) + iSpaceBetweenSteps;
            int iB = iTopOffset + (iHeightOneStep * (iVertPos + 1));

            string strPos = "l=" + iL.ToString() + ";r=" + iR.ToString() + ";t=" + iT.ToString() + ";b=" + iB.ToString() + ";";

            EA.DiagramObject diagramObj = diagram.DiagramObjects.AddNew(strPos, "");
            if (diagramObj == null)
            {
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 1;
            }

            diagramObj.ElementID = iUCElementID;
            diagramObj.Update();

            iL = 700;
            iR = 800;
            iT = iTopOffset + (iHeightOneStep * iVertPos) + iSpaceBetweenSteps;
            iB = iTopOffset + (iHeightOneStep * (iVertPos + 1));

            strPos = "l=" + iL.ToString() + ";r=" + iR.ToString() + ";t=" + iT.ToString() + ";b=" + iB.ToString() + ";";

            EA.DiagramObject diagramObj2 = diagram.DiagramObjects.AddNew(strPos, "");
            if (diagramObj2 == null)
            {
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 1;
            }

            diagramObj2.ElementID = iUCClassElementID;
            diagramObj2.Update();


            diagram.DiagramObjects.Refresh();
            eaPackage.Diagrams.Refresh();
            diagram.Update();

            return 0;
        }

        private int CreateUseCaseDigram(string strUseCaseName, EA.Package eaPackage, XElement elUseCase, EA.Element eaElUCClassElement)
        {
            int iErrorCount = 0;

            EA.Diagram UCdiagram = EAImporter.AddDiagram(eaPackage, "Use Case Diagram", "Use Case");

            Hashtable UCRefActors = EAImporter.m_Actors.GetActorUseCaseRefs();

            EA.Element eaElUseCase = eaPackage.Elements.AddNew(strUseCaseName, "Use Case");

            int iErrCnt = AddUCToUCDiagram(eaPackage, UCdiagram, eaElUseCase.ElementID, eaElUCClassElement.ElementID, UCRefActors.Count / 2);

            EA.Connector con2 = eaElUCClassElement.Connectors.AddNew("describes", "Dependancy");
            con2.SupplierID = eaElUseCase.ElementID;
            con2.Update();
            eaElUCClassElement.Connectors.Refresh();

            m_NoUseCaseActors = 0;

            foreach (DictionaryEntry deActorRef1 in UCRefActors)
            {
                string strActorName = (string)deActorRef1.Key;

                EA.Element eaElActor = EAImporter.m_Actors.GetActor(strActorName);

                iErrorCount = iErrorCount + AddActorToUCDiagram(eaPackage, UCdiagram, eaElActor.ElementID);

                EA.Connector con1 = eaElUseCase.Connectors.AddNew("", "UseCase");
                con1.SupplierID = eaElActor.ElementID;
                con1.Update();

                EA.ConnectorTag eaTgVal = con1.TaggedValues.AddNew("furtherInformation", "TaggedValue");

                eaTgVal.Value = ((ImportFieldsValidator)deActorRef1.Value)["furtherInformation"].Value;
                eaTgVal.Update();

                con1.Update();

                eaElUseCase.Connectors.Refresh();
            }

            return iErrorCount;
        }

        /// <summary>
        /// Adds new package for use case and populates all children
        /// </summary>
        /// <param name="elUseCase"></param>
        public int ProcessUseCase(XElement elUseCase)
        {
            int iErrorCount = 0;
            // check if use case node exists --

            if (elUseCase == null)
            {
                return iErrorCount;
            }

            ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

            // Only Process Use Case "flat Elements" here
            oFieldsValidator.Add(new FieldInfo("id", elUseCase));
            oFieldsValidator.Add(new FieldInfo("name", elUseCase));
            oFieldsValidator.Add(new FieldInfo("classification", elUseCase));
            oFieldsValidator.Add(new FieldInfo("keywords", elUseCase));
            oFieldsValidator.Add(new FieldInfo("levelOfDepth", elUseCase));
            oFieldsValidator.Add(new FieldInfo("prioritisation", elUseCase));
            oFieldsValidator.Add(new FieldInfo("scope", elUseCase,"",false,false,true));
            oFieldsValidator.Add(new FieldInfo("viewPoint", elUseCase));
            // Narrative
            oFieldsValidator.Add(new FieldInfo("completeDescription", elUseCase.Element("Narrative"),"",true,false,true));
            oFieldsValidator.Add(new FieldInfo("shortDescription", elUseCase.Element("Narrative")));
            // PrimaryDomain
            oFieldsValidator.Add(new FieldInfo("PrimaryDomain", elUseCase.Element("PrimaryDomain"), "name"));

            iErrorCount = iErrorCount + oFieldsValidator.ParseAndValidateFields("general use case information");

            //if (iErrCnt == 0)
            //{
                string strPrimaryDomain = "";
                string strID = "";
                string strName = "";

                if (oFieldsValidator["PrimaryDomain"].Value == "")
                {
                    strPrimaryDomain = "STUB";
                }
                else
                {
                    strPrimaryDomain = oFieldsValidator["PrimaryDomain"].Value;
                }

                if (oFieldsValidator["id"].Value == "")
                {
                    strID = "STUB";
                }
                else
                {
                    strID = oFieldsValidator["id"].Value;
                }

                if(oFieldsValidator["name"].Value == "")
                {
                    strName = "STUB";
                }
                else
                {
                    strName = oFieldsValidator["name"].Value;
                }

                string strUseCasePath = "UseCaseRepository/UseCaseLibrary/ImportedUseCase/" + strPrimaryDomain + "/" + strID + " " + strName;

                oFieldsValidator.ReplaceImageLinks(strUseCasePath);

                if (EAImporter.m_Packages.Add("UseCaseRepository/UseCaseLibrary", "ImportedUseCase") == null) { iErrorCount++; };

                if (EAImporter.m_Packages.Add("UseCaseRepository/UseCaseLibrary/ImportedUseCase", strPrimaryDomain) == null) { iErrorCount++; };

                // see if use case package already exists
                try
                {
                    EA.Package oPackage = ((EA.Package)EAImporter.m_Packages["UseCaseRepository/UseCaseLibrary/ImportedUseCase/" + strPrimaryDomain]).Packages.GetByName(strID + " " + strName);
                    if (oPackage != null)
                    {
                        EAImporter.LogError(EAImporter.LogErrorLevel.A, "Use case package " + "UseCaseRepository/UseCaseLibrary/ImportedUseCase/" + strPrimaryDomain + "/" + strID + " " + strName + " already exists.");
                        strName = "STUB"; // allow processing to continue
                        strUseCasePath = "UseCaseRepository/UseCaseLibrary/ImportedUseCase/" + strPrimaryDomain + "/" + strID + " " + strName;
                        iErrorCount++;
                    }
                }
                catch (Exception ex)
                {
                    //EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, ex.Message);
                }

                EA.Package eaPackageUseCase = EAImporter.m_Packages.Add("UseCaseRepository/UseCaseLibrary/ImportedUseCase/" + strPrimaryDomain, strID + " " + strName);
                if (eaPackageUseCase == null) { iErrorCount++; };

                EA.Package eaPackageEnterprise = EAImporter.m_Packages.Add(strUseCasePath, "Enterprise", "");
                if (eaPackageEnterprise == null) { iErrorCount++; }; 

                if (EAImporter.m_Packages.Add(strUseCasePath, "Computation", "") == null) { iErrorCount++; };
                
                if (EAImporter.m_Packages.Add(strUseCasePath, "Technology", "") == null) { iErrorCount++; };
                
                if (EAImporter.m_Packages.Add(strUseCasePath, "Engineering", "") == null) { iErrorCount++; };
                
                if (EAImporter.m_Packages.Add(strUseCasePath, "Information", "") == null) { iErrorCount++; };
 

                // the following commented code was used prototyping artifacts and notes as attached rtf files.

                //EA.Element eaArtifact = m_Packages["UseCaseRepository/UseCaseLibrary/ImportedUseCase"].Elements.AddNew("Notes", "Artifact");
                //string strTemp = oFieldsValidator["completeDescription"].Value;
                ///*strTemp.Replace("&lt;", "<");
                //strTemp.Replace("&gt;", ">");*/
                ////strTemp.Replace("\n", Convert.ToString(13)+Convert.ToString(10));
                //eaArtifact.Notes = strTemp;

                // // Create/import rtf linked document
                ////string strHTMLHead = File.ReadAllText(@"D:\_project\EPRI\C18322-EPRIUseCaseRepository\Source\Template\ArtifactFragmentTemplateHead.html");
                ////string strHTMLTail = File.ReadAllText(@"D:\_project\EPRI\C18322-EPRIUseCaseRepository\Source\Template\ArtifactFragmentTemplateTail.html");

                ////string strFullHTML = strHTMLHead + oFieldsValidator["Description"].Value + strHTMLTail;

                ////File.WriteAllText(@"D:\_project\EPRI\C18322-EPRIUseCaseRepository\Source\Template\Artifact1.html", strFullHTML);

                ////m_oWord.Visible = false;
                ////m_oWord.Documents.Open(@"D:\_project\EPRI\C18322-EPRIUseCaseRepository\Source\Template\Artifact1.html");
                ////m_oWord.Documents[1].SaveAs(@"D:\_project\EPRI\C18322-EPRIUseCaseRepository\Source\Template\Artifact1.rtf", Word.WdSaveFormat.wdFormatRTF);
                ////m_oWord.Documents.Close();
                ////m_oWord.Visible = false;
                ////eaArtifact.LoadLinkedDocument(@"D:\_project\EPRI\C18322-EPRIUseCaseRepository\Source\Template\Artifact1.rtf");
                //eaArtifact.Update();

                // create diagram for all UC elements to display attribute values
                EA.Diagram diagram = EAImporter.AddDiagram(eaPackageUseCase, "Use Case Components", "Object");
                
                // process sequences here
                iErrorCount = iErrorCount + m_Sequences.Import(strUseCasePath + "/Computation", elUseCase);

                // process Activities here
                iErrorCount = iErrorCount + m_Activities.Import(strUseCasePath + "/Computation", elUseCase);

                // process Use Case Elements
                EA.Element elUCClassElement = null;
                iErrorCount = iErrorCount + ProcessUseCaseFlatAttributes(eaPackageEnterprise, eaPackageUseCase, oFieldsValidator, diagram, ref elUCClassElement);
 
                // Process Narrative
                iErrorCount = iErrorCount + ProcessNarrative(eaPackageEnterprise, eaPackageUseCase, oFieldsValidator, diagram);

                // Process Version Information
                iErrorCount = iErrorCount + ProcessVersionInfo(strUseCasePath + "/Enterprise", "Versions", eaPackageEnterprise, elUseCase, diagram);

                // Process References Information
                iErrorCount = iErrorCount + ProcessReferenceInfo(strUseCasePath + "/Enterprise", "References", eaPackageEnterprise, elUseCase, diagram);

                // Conditions
                iErrorCount = iErrorCount + ProcessConditions(strUseCasePath + "/Enterprise", "Conditions", eaPackageEnterprise, elUseCase, diagram);

                // General Remarks
                iErrorCount = iErrorCount + ProcessRemarks(strUseCasePath + "/Enterprise", "GeneralRemarks", eaPackageEnterprise, elUseCase, diagram);

                // RelatedObjectives
                iErrorCount = iErrorCount + ProcessObjectives(strUseCasePath + "/Enterprise", "RelatedObjectives", eaPackageEnterprise, elUseCase, diagram);

                // RelatedBusinessCases
                iErrorCount = iErrorCount + ProcessBusinessCases(strUseCasePath + "/Enterprise", "RelatedBusinessCases", eaPackageEnterprise, elUseCase, diagram);

                // Additional Domains
                iErrorCount = iErrorCount + ProcessAdditionalDomains(strUseCasePath + "/Enterprise", "AdditionalDomains", eaPackageEnterprise, elUseCase, diagram);

                // UseCaseRelation
                iErrorCount = iErrorCount + ProcessRelations(strUseCasePath + "/Enterprise", "UseCaseRelations", eaPackageEnterprise, elUseCase, diagram);

                // UseCase Diagram
                iErrorCount = iErrorCount + CreateUseCaseDigram(strID + " " + strName, eaPackageEnterprise, elUseCase, elUCClassElement);
                
                EAImporter.m_Packages["UseCaseRepository/UseCaseLibrary/ImportedUseCase"].Elements.Refresh();
            //}

            //iErrorCount = iErrorCount + iErrCnt;

            return iErrorCount;
        }

    

    }
}
