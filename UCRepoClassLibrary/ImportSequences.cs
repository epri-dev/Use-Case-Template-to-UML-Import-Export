/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportSequences.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 10 $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportSequences.cs 10    11/05/12 11:31a Ronp $

$History: ImportSequences.cs $
 * 
 * *****************  Version 10  *****************
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
using System.Collections;

using System.Windows.Forms; // used for Application.DoEvents -- may need to remove.

namespace UCRepoClassLibrary
{
    internal class ImportSequences
    {
        int m_iObjCenterOffset = 200;
        int m_iObjWidth = 100;
        int m_iLOffsetFromObjCenter = 100;
        int m_iROffsetFromObjCenter = 100;
        int m_iTopOffset = 120;
        int m_iHeightOneStep = 35;

        /// <summary>
        /// AddOperation
        /// </summary>
        /// <param name="eElement"></param>
        /// <param name="strOperationName"></param>
        /// <param name="strParamType"></param>
        private static void AddOperation(EA.Element eElement, string strOperationName, string strParamType)
        {
            string strLogInfo = "Operation: " + strOperationName + ":" + strParamType;

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);
            // first see if operation name already exits
            bool bFound = false;

            for (short i = 0; i < eElement.Methods.Count; i++)
            {
                Application.DoEvents();

                EA.Method eOperationTst = eElement.Methods.GetAt(i);

                for (short j = 0; j < eOperationTst.Parameters.Count; j++)
                {
                    Application.DoEvents();

                    EA.Parameter eParamTst = eOperationTst.Parameters.GetAt(j);

                    if (eParamTst.Type == strParamType)
                    {
                        EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                        bFound = true;
                        return;
                    }
                }
            }

            if (!bFound)
            {
                EA.Method eOperation = eElement.Methods.AddNew(strOperationName, "");
                eOperation.Update();
                eElement.Methods.Refresh();

                EA.Parameter eParam = eOperation.Parameters.AddNew(strParamType, "");
                eParam.Name = strParamType;
                eParam.Kind = "in";
                eParam.Type = strParamType;
                eParam.Update();
                eOperation.Update();
                eElement.Update();
                eElement.Methods.Refresh();

                EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
            }
        }


        private EA.Element AddObjIntfcInstance(EA.Diagram diagram, EA.Package thePackage, string strActorIntcName, int iActorIntefaceElID, ref int iDiagObjNo)
        {
            string strLogInfo = "Object for actor: " + strActorIntcName;

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            EA.Element eElObj = null;

            try
            {
                eElObj = thePackage.Elements.GetByName(strActorIntcName);
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, ex.Message);
            }

            if (eElObj == null)
            {
                eElObj = thePackage.Elements.AddNew(strActorIntcName, "Object");
                //eElObj.Stereotype = "interface";
                eElObj.ClassifierID = iActorIntefaceElID;
                eElObj.Update();
                thePackage.Elements.Refresh();

                AddDiagramObject(diagram, eElObj.ElementID, iDiagObjNo);

                iDiagObjNo++;

                EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
            }
            else
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
            }

            return eElObj;
        }

        private EA.DiagramObject AddDiagramObject(EA.Diagram diagram, int iLinkedElementID, int iObjNo)
        {
            string strLogInfo = "diagram object for link ID: " + iLinkedElementID.ToString();

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            EA.DiagramObject diagramObj;

            int iL = iObjNo * m_iObjCenterOffset;
            int iR = iL + m_iObjWidth;
            string strPos = "l=" + iL.ToString() + ";r=" + iR.ToString() + ";t=50;b=400;";

            diagramObj = diagram.DiagramObjects.AddNew(strPos, "");
            diagramObj.ElementID = iLinkedElementID;
            diagramObj.Update();
            diagram.DiagramObjects.Refresh();

            return diagramObj;
        }

        private EA.DiagramObject AddDiagramInteractionFragment(EA.Diagram diagram, int iLinkedElementID, int iFirstObject, int iLastObject, int iFirstStep, int iLastStep)
        {
            string strLogInfo = "Diagram InteractionFragment for link ID: " + iLinkedElementID.ToString();

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            int iLeft = (iFirstObject * m_iObjCenterOffset) - m_iLOffsetFromObjCenter;
            if (iLeft < 10) iLeft = 10;
            int iRight = (iLastObject * m_iObjCenterOffset) + m_iROffsetFromObjCenter;

            int iTop = m_iTopOffset + (m_iHeightOneStep * iFirstStep);
            int iBottom = m_iTopOffset + (m_iHeightOneStep * (iLastStep + 1));

            string strPos = "l=" + iLeft.ToString() + ";r=" + iRight.ToString() + ";t=" + iTop + ";b=" + iBottom + ";";

            EA.DiagramObject diagramObj = diagram.DiagramObjects.AddNew(strPos, "");
            diagramObj.ElementID = iLinkedElementID;
            diagramObj.Update();
            diagram.DiagramObjects.Refresh();

            return diagramObj;
        }

        /// <summary>
        /// AddConnector
        /// </summary>
        /// <param name="diagram"></param>
        /// <param name="eElement"></param>
        /// <param name="strFullOpName"></param>
        /// <param name="strEvent"></param>
        /// <param name="iSupplierID"></param>
        /// <param name="iSeqNo"></param>
        private void AddConnector(EA.Diagram diagram, EA.Element eElement, string strFullOpName, string strEvent, int iSupplierID, int iSeqNo, string strNotes)
        {
            string strLogInfo = "Connector for operation: " + strFullOpName;

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            EA.Connector eConnector = null;

            //for (short i = 0; i < eElement.Connectors.Count; i++)
            //{
            //    Application.DoEvents();

            //    eConnector = eElement.Connectors.GetAt(i);

            //    if (eConnector.Name == strFullOpName)
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        eConnector = null;
            //    }
            //}

            if (eConnector == null)
            {
                eConnector = eElement.Connectors.AddNew(strFullOpName, "Sequence");
                eConnector.Notes = strNotes;

                eConnector.SequenceNo = iSeqNo;
                eConnector.SupplierID = iSupplierID;
                eConnector.TransitionAction = "Call";
                eConnector.TransitionEvent = "Synchronus";
                eConnector.TransitionGuard = "params=;paramsDlg=param1;";
                eConnector.Update();

                eConnector.StartPointY = eConnector.StartPointY + 1;
                eConnector.Update();

                EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
            }
            else
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
            }

            EA.DiagramLink eDiagramLink = diagram.DiagramLinks.AddNew(strEvent, "sequence");
            eDiagramLink.ConnectorID = eConnector.ConnectorID;
            eDiagramLink.Update();

            diagram.Update();

        }

        private ImportFieldsValidator SequenceStepGetFields(XElement elStep, ref string strOperationName, ref string strFirstStep, ref string strLastStep)
        {
            ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

            oFieldsValidator.Add(new FieldInfo("service", elStep));

            if (oFieldsValidator.ParseAndValidateFields("Scenario") != 0)
            {
                // "Service" is required
                return oFieldsValidator;
            }

            if (oFieldsValidator["service"].Value.StartsWith("REPEAT"))
            {
                strOperationName = "REPEAT";
                string strServiceStripped = oFieldsValidator["service"].Value;
                strServiceStripped = strServiceStripped.Replace("REPEAT", "");
                strServiceStripped = strServiceStripped.Replace("(", "");
                strServiceStripped = strServiceStripped.Replace(")", "");

                string[] strSteps = strServiceStripped.Split('-');

                if (strSteps.Length != 2)
                {
                    throw new EpriException("REPEAT invalid arguments : " + oFieldsValidator["Service"].Value, elStep.getLocation());
                    //EAImporter.LogError(EAImporter.LogErrorLevel.A, "REPEAT invalid arguments : " + oFieldsValidator["Service"].Value);
                    return oFieldsValidator;
                }
                else
                {
                    strFirstStep = strSteps[0];
                    strLastStep = strSteps[1];
                }
            }
            else
            {
                strOperationName = oFieldsValidator["service"].Value;
            }

            oFieldsValidator.Add(new FieldInfo("event", elStep, "", false));
            oFieldsValidator.Add(new FieldInfo("description", elStep));

            if (strOperationName == "REPEAT")
            {
                // no other fields required
            }
            else if (strOperationName == "TIMER")
            {
                oFieldsValidator.Add(new FieldInfo("name", elStep));
                oFieldsValidator.Add(new FieldInfo("Producer", elStep.Element("InformationProducer"), "name"));
                oFieldsValidator.Add(new FieldInfo("InformationExchanged", elStep.Element("InformationModel"), "name"));
                oFieldsValidator.Add(new FieldInfo("Step", elStep, "number"));
                //oFieldsValidator.Add(new FieldInfo("TechRequirementsID", elStep.Element("Requirement"), "name", false));
            }
            else
            {
                oFieldsValidator.Add(new FieldInfo("name", elStep));
                oFieldsValidator.Add(new FieldInfo("Producer", elStep.Element("InformationProducer"), "name"));
                oFieldsValidator.Add(new FieldInfo("Receiver", elStep.Element("InformationReceiver"), "name"));
                oFieldsValidator.Add(new FieldInfo("InformationExchanged", elStep.Element("InformationModel"), "name"));
                oFieldsValidator.Add(new FieldInfo("Step", elStep, "number"));
                //oFieldsValidator.Add(new FieldInfo("TechRequirementsID", elStep.Element("Requirement"), "name", false));
            }

            oFieldsValidator.ParseAndValidateFields("Scenario");

            return oFieldsValidator;
        }

        private int ProcessSequenceStep(EA.Package theStepsPackage, EA.Diagram diagram, XElement elStep, ref int iSeqNo, ref int iDiagObjNo)
        {
            int iErrorCount = 0;
            string strOperationName = "";
            string strFirstStep = "";
            string strLastStep = "";

            ImportFieldsValidator oFieldsValidator = SequenceStepGetFields(elStep, ref strOperationName, ref strFirstStep, ref strLastStep);

            if (oFieldsValidator.ErrorCount == 0)
            {
                if (strOperationName == "REPEAT")
                {
                    EA.Element theFragment = theStepsPackage.Elements.AddNew("RepeatSteps", "InteractionFragment");
                    theFragment.Stereotype = "loop";
                    theFragment.Notes = oFieldsValidator["event"].Value;
                    theFragment.Update();
                    theStepsPackage.Elements.Refresh();

                    int iFirstObject = 0;
                    int iLastObject = iDiagObjNo - 1;
                    int iFirstStep = int.Parse(strFirstStep) - 1;
                    int iLastStep = int.Parse(strLastStep) - 1;

                    AddDiagramInteractionFragment(diagram, theFragment.ElementID, iFirstObject, iLastObject, iFirstStep, iLastStep);
                }
                else if (strOperationName == "TIMER")
                {
                    bool bValid = true;
                    EA.Element eActorIntfc_Producer_Source = EAImporter.m_Actors.GetActorInterface(oFieldsValidator["Producer"].Value);
                    if (eActorIntfc_Producer_Source == null)
                    {
                        throw new EpriException("Invalid information producer \"" + oFieldsValidator["Producer"].Value + "\" for element:" + EAImporter.Recurse(elStep), elStep.getLocation());
                        //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Invalid information producer \"" + oFieldsValidator["Producer"].Value + "\" for element:" + EAImporter.Recurse(elStep));
                        iErrorCount++;
                        bValid = false;
                    }

                    if (bValid)
                    {
                        // add operation to target actor interface
                        AddOperation(eActorIntfc_Producer_Source, strOperationName, oFieldsValidator["InformationExchanged"].Value);

                        EA.Element eElObjProducer = null;

                        eElObjProducer = AddObjIntfcInstance(diagram, theStepsPackage, oFieldsValidator["Producer"].Value, eActorIntfc_Producer_Source.ElementID, ref iDiagObjNo);

                        string strFullOpName = strOperationName + "(" + oFieldsValidator["InformationExchanged"].Value + ")";

                        AddConnector(diagram, eElObjProducer, strFullOpName, oFieldsValidator["event"].Value, eElObjProducer.ElementID, iSeqNo, oFieldsValidator["description"].Value);

                        EAImporter.m_Actors.MarkActorUseCaseRef(oFieldsValidator["Producer"].Value);
                    }
                }
                else
                {
                    bool bValid = true;
                    EA.Element eActorIntfc_Producer_Source = EAImporter.m_Actors.GetActorInterface(oFieldsValidator["Producer"].Value);
                    if (eActorIntfc_Producer_Source == null)
                    {
                        throw new EpriException("Invalid information producer \"" + oFieldsValidator["Producer"].Value + "\" for element:" + EAImporter.Recurse(elStep), elStep.getLocation());
                        //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Invalid information producer \"" + oFieldsValidator["Producer"].Value + "\" for element:" + EAImporter.Recurse(elStep));
                        iErrorCount++;
                        bValid = false;
                    }

                    EA.Element eActorIntfc_Receiver_Target = EAImporter.m_Actors.GetActorInterface(oFieldsValidator["Receiver"].Value);
                    if (eActorIntfc_Receiver_Target == null)
                    {
                        throw new EpriException("Invalid information receiver \"" + oFieldsValidator["Receiver"].Value + "\" for element:" + EAImporter.Recurse(elStep), elStep.getLocation());
                        //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Invalid information receiver \"" + oFieldsValidator["Receiver"].Value + "\" for element:" + EAImporter.Recurse(elStep));
                        iErrorCount++;
                        bValid = false;
                    }
                    if (bValid)
                    {
                        // check if info objec exists:
                        if (EAImporter.m_InfoModels.Get(oFieldsValidator["InformationExchanged"].Value) == null)
                        {
                            throw new EpriException("Invalid information exchanged \"" + oFieldsValidator["InformationExchanged"].Value + "\" for element:" + EAImporter.Recurse(elStep), elStep.getLocation());
                            //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Invalid information exchanged \"" + oFieldsValidator["InformationExchanged"].Value + "\" for element:" + EAImporter.Recurse(elStep));
                            iErrorCount++;
                        }
                        else
                        {
                            // add operation to target actor interface
                            AddOperation(eActorIntfc_Receiver_Target, strOperationName, oFieldsValidator["InformationExchanged"].Value);

                            EA.Element eElObjProducer = null;
                            EA.Element eElObjReceiver = null;

                            eElObjProducer = AddObjIntfcInstance(diagram, theStepsPackage, oFieldsValidator["Producer"].Value, eActorIntfc_Producer_Source.ElementID, ref iDiagObjNo);

                            eElObjReceiver = AddObjIntfcInstance(diagram, theStepsPackage, oFieldsValidator["Receiver"].Value, eActorIntfc_Receiver_Target.ElementID, ref iDiagObjNo);

                            string strFullOpName = strOperationName + "(" + oFieldsValidator["InformationExchanged"].Value + ")";

                            AddConnector(diagram, eElObjProducer, strFullOpName, oFieldsValidator["event"].Value, eElObjReceiver.ElementID, iSeqNo, oFieldsValidator["description"].Value);

                            EAImporter.m_Actors.MarkActorUseCaseRef(oFieldsValidator["Producer"].Value);
                            EAImporter.m_Actors.MarkActorUseCaseRef(oFieldsValidator["Receiver"].Value);
                        }

                    }
                }

                diagram.Update();

                theStepsPackage.Diagrams.Refresh();
                diagram.DiagramObjects.Refresh();

                iSeqNo++;
            }
            else
            {
                iErrorCount = iErrorCount + oFieldsValidator.ErrorCount;
            }

            return iErrorCount;
        }
        /// <summary>
        /// ProcessSequences
        /// </summary>
        /// <param name="strParentPackageName"></param>
        /// <param name="el"></param>
        public int Import(string strParentPackageName, XElement el)
        {
            int iErrorCount = 0;
            if (el == null)
            {
                return iErrorCount;
            }

            IEnumerable<XElement> elSequences = el.Elements("Scenario");

            foreach (XElement elSequence in elSequences)
            {
                Application.DoEvents();

                ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                oFieldsValidator.Add(new FieldInfo("name", elSequence));
                oFieldsValidator.Add(new FieldInfo("PrimaryActorName", elSequence.Element("PrimaryActor"), "name"));
                oFieldsValidator.Add(new FieldInfo("triggeringEvent", elSequence));
                oFieldsValidator.Add(new FieldInfo("preCondition", elSequence, "", true, false, false, true));
                oFieldsValidator.Add(new FieldInfo("postCondition", elSequence, "", true, false, false, true));
                oFieldsValidator.Add(new FieldInfo("number", elSequence));

                int iErrCnt = oFieldsValidator.ParseAndValidateFields("scenario conditions");

                iErrorCount = iErrorCount + iErrCnt;

                if (iErrCnt != 0)
                {
                    // assign temporary name to sequence to process steps for error reporting
                    oFieldsValidator["name"].Value = "DUMMY";
                }

                EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Importing Sequence: " + oFieldsValidator["name"].Value);

                EA.Package theSeqPackage = EAImporter.m_Packages.Add(strParentPackageName, oFieldsValidator["name"].Value, "", elSequence);

                string strPackageName = strParentPackageName + "/" + oFieldsValidator["name"].Value;

                EA.Package theStepsPackage = EAImporter.m_Packages.Add(strPackageName, "Sequence", "");

                if (theStepsPackage != null)
                {
                    {
                        EA.Element elScenarioObj = theSeqPackage.Elements.AddNew("Scenario", "Object");
                        elScenarioObj.ClassifierID = EAImporter.m_ScenarioBaseClsID;
                        string strRunState = "";

                        strRunState = strRunState + oFieldsValidator["number"].GetRunStateString();
                        strRunState = strRunState + oFieldsValidator["name"].GetRunStateString();
                        strRunState = strRunState + oFieldsValidator["triggeringEvent"].GetRunStateString();
                        strRunState = strRunState + oFieldsValidator["preCondition"].GetRunStateString();
                        strRunState = strRunState + oFieldsValidator["postCondition"].GetRunStateString();

                        elScenarioObj.RunState = strRunState;
                        elScenarioObj.Update();

                    }

                    {


                        EA.Element elActor = EAImporter.m_Actors.GetActor(oFieldsValidator["PrimaryActorName"].Value);

                        if (elActor != null)
                        {
                            EA.Element elPrimActorObj = theSeqPackage.Elements.AddNew("PrimaryActor_" + oFieldsValidator["PrimaryActorName"].Value, "Object");
                            elPrimActorObj.ClassifierID = elActor.ElementID;
                            elPrimActorObj.Update();
                        }
                        else
                        {
                            throw new EpriException("\"PrimaryActorName\" missing from scenario" + oFieldsValidator["PrimaryActorName"].Value, oFieldsValidator["PrimaryActorName"].XElement.getLocation());
                            //EAImporter.LogError(EAImporter.LogErrorLevel.A, "\"PrimaryActorName\" missing from scenario" + oFieldsValidator["PrimaryActorName"].Value);
                            iErrorCount++;
                        }

                    }

                    EA.Diagram diagram = EAImporter.AddDiagram(theStepsPackage, "Sequence Diagram", "Sequence");

                    IEnumerable<XElement> elSteps = elSequence.Elements("Step");

                    int iSeqNo = 0;
                    int iDiagObjNo = 0;
                    foreach (XElement elStep in elSteps)
                    {
                        Application.DoEvents();

                        iErrorCount = iErrorCount + ProcessSequenceStep(theStepsPackage, diagram, elStep, ref iSeqNo, ref iDiagObjNo);
                    }

                }
                else
                {
                    iErrorCount++;
                }

            }

            return iErrorCount;
        }

    }
}
