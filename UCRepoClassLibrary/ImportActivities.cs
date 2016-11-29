/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportActivities.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 12 $

	Date last modified: $Modtime: 11/05/12 11:26a $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportActivities.cs 12    11/05/12 11:31a Ronp $

$History: ImportActivities.cs $
 * 
 * *****************  Version 12  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary
 * Update keyword expansion
 * 
 * *****************  Version 11  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:17a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary

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
    internal class ImportActivities
    {
        public class Swimlane
        {
            public int m_iL = 0;
            public int m_iR = 0;
            public int m_iT = 0;
            public int m_iB = 0;
            public static int m_NoSwimlanes = 0;
            public EA.DiagramObject m_eaDiagramObject = null;
        }

        public class Step
        {
            public ImportFieldsValidator m_oFields;
            public static int m_noSteps = 0;
            public string m_strOperationName = "";
            public string m_strPrimaryActor = "";
            public string m_strSecondaryActor = "";
            public string m_strFirstStep = "";
            public string m_strLastStep = "";
            public EA.Element m_eaElActivity = null;
        }

        private Hashtable m_LoadedSteps;
        private Hashtable m_Swimlanes;

        private int m_NoActivities = 0;

        int iActivityHorizOffset = 100;
        int iActivityHorizDistBetweenActivities = 250;
        int iActivityWidth = 150;
        int iActivityHeight = 75;

        int m_iSwimlaneHeight = 200;
        int m_iSwimlaneHorizOffset = 20;
        int m_iSwimlaneVertOffset = 20;

        private int AddConnector(EA.Diagram diagram, EA.Element eaElSource, EA.Element eaElDest, string strText, string strControlType, string strPath = "")
        {
            string strLogInfo = "Connector for operation: " + strText;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            EA.Connector eConnector = eaElSource.Connectors.AddNew(strText, strControlType);
            if (eConnector == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 0;
            }

            // eConnector.Notes = strNotes;
            eConnector.SupplierID = eaElDest.ElementID;
            eConnector.Width = 2;
            eConnector.Update();

            EA.DiagramLink eDiagramLink = diagram.DiagramLinks.AddNew(strText, strControlType);
            if (eDiagramLink == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 0;
            }

            eDiagramLink.ConnectorID = eConnector.ConnectorID;
            if (strPath != "")
            {
                eDiagramLink.Path = strPath;
            }
            eDiagramLink.Update();

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
            return 0;
        }

        private EA.DiagramObject AddDiagramObject(EA.Diagram diagram, int iLinkedElementID, string strPos)
        {
            string strLogInfo = "diagram object for link ID: " + iLinkedElementID.ToString();
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            EA.DiagramObject diagramObj = diagram.DiagramObjects.AddNew(strPos, "");
            if (diagramObj == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return null;
            }

            diagramObj.ElementID = iLinkedElementID;
            diagramObj.Update();
            diagram.DiagramObjects.Refresh();

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);

            return diagramObj;
        }

        private int AddSwimlane(EA.Diagram diagram, EA.Package thePackage, string strActorName)
        {
            string strLogInfo = "Swimlane: " + strActorName;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            Swimlane oSwimlane = (Swimlane)m_Swimlanes[strActorName];

            if (oSwimlane != null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                return 0;
            }

            EA.Element eElObj = thePackage.Elements.AddNew(strActorName, "ActivityPartition");
            if (eElObj == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 1;
            }

            eElObj.Update();
            thePackage.Elements.Refresh();

            oSwimlane = new Swimlane();

            m_Swimlanes[strActorName] = oSwimlane;

            oSwimlane.m_iL = m_iSwimlaneHorizOffset;
            oSwimlane.m_iR = 500;
            oSwimlane.m_iT = (Swimlane.m_NoSwimlanes * m_iSwimlaneHeight) + m_iSwimlaneVertOffset;
            oSwimlane.m_iB = oSwimlane.m_iT + m_iSwimlaneHeight;

            string strPos = "l=" + oSwimlane.m_iL.ToString() + ";r=" + oSwimlane.m_iR.ToString() + ";t=" + oSwimlane.m_iT.ToString() + ";b=" + oSwimlane.m_iB.ToString() + ";";

            oSwimlane.m_eaDiagramObject = AddDiagramObject(diagram, eElObj.ElementID, strPos);
            if (oSwimlane.m_eaDiagramObject == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return 1;
            }

            Swimlane.m_NoSwimlanes++;

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);

            return 0;

        }

        private EA.Element AddDataObjectToSwimlane(EA.Diagram diagram, EA.Package thePackage, string strInfoExchanged, string strActorName)
        {
            string strLogInfo = "Actor: " + strActorName + " info: " + strInfoExchanged;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            Swimlane oSwimlane = (Swimlane)m_Swimlanes[strActorName];

            int iL = (iActivityHorizDistBetweenActivities * (m_NoActivities - 1)) + iActivityHorizOffset;
            int iR = iL + iActivityWidth;
            int iT = (((oSwimlane.m_iB - oSwimlane.m_iT) / 2) + oSwimlane.m_iT) - (iActivityHeight / 2);
            int iB = iT + iActivityHeight;

            string strPos = "l=" + iL.ToString() + ";r=" + iR.ToString() + ";t=" + iT.ToString() + ";b=" + iB.ToString() + ";";

            EA.Element eElObj = AddDataObject(diagram, thePackage, strInfoExchanged, strActorName, strPos);
            if (eElObj == null)
            {
                throw new EpriException("Could not add actor \"" + strActorName + "\" info \"" + strInfoExchanged + "\" to activity diagram", "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Could not add actor \"" + strActorName + "\" info \"" + strInfoExchanged + "\" to activity diagram");
                return null;
            }

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);

            return eElObj;
        }


        private EA.Element AddActivityToSwimlane(EA.Diagram diagram, EA.Package thePackage, string strActivityName, string strActorName, ImportFieldsValidator oFields)
        {
            string strLogInfo = "Add activity to swimlane. Actor: " + strActorName + " activity: " + strActivityName;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            Swimlane oSwimlane = (Swimlane)m_Swimlanes[strActorName];

            int iL = (iActivityHorizDistBetweenActivities * m_NoActivities) + iActivityHorizOffset;
            int iR = iL + iActivityWidth;
            int iT = (((oSwimlane.m_iB - oSwimlane.m_iT) / 2) + oSwimlane.m_iT) - (iActivityHeight / 2);
            int iB = iT + iActivityHeight;

            string strPos = "l=" + iL.ToString() + ";r=" + iR.ToString() + ";t=" + iT.ToString() + ";b=" + iB.ToString() + ";";

            EA.Element eElObj = AddActivity(diagram, thePackage, strActivityName, strActorName, strPos, oFields);

            if (eElObj == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return null;
            }

            m_NoActivities++;

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
            return eElObj;
        }

        private EA.Element AddDataObject(EA.Diagram diagram, EA.Package thePackage, string strInfoExchanged, string strActorName, string strPos)
        {
            string strLogInfo = "Activity Info Exchanged: " + strInfoExchanged;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            EA.Element eElObj = thePackage.Elements.AddNew(strInfoExchanged, "Object");
            if (eElObj == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return null;
            }

            try
            {
                eElObj.ClassifierID = EAImporter.m_InfoModels.Get(strInfoExchanged).ElementID;
            }
            catch (Exception ex)
            {
                throw new EpriException("Information exchanged \"" + strInfoExchanged + "\" used in a sequence step is not defined in the Use Case or in the Repository", "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Information exchanged \"" + strInfoExchanged + "\" used in a sequence step is not defined in the Use Case or in the Repository");
                string Temp = ex.Message;
                return null;
            }

            EA.TaggedValue eaTgVal = eElObj.TaggedValues.AddNew("ActorName", "TaggedValue");
            eaTgVal.Value = strActorName;
            eaTgVal.Update();

            eElObj.Update();

            if (AddDiagramObject(diagram, eElObj.ElementID, strPos) == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return null;
            }

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);

            return eElObj;
        }

        private EA.Element AddActivity(EA.Diagram diagram, EA.Package thePackage, string strActivityName, string strActorName, string strPos, ImportFieldsValidator oFields)
        {
            string strLogInfo = "Activity: " + strActivityName;

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            EA.Element eElObj = null;

            if (strActivityName.StartsWith("Repeat till"))
            {
                eElObj = thePackage.Elements.AddNew(strActivityName, "Decision");
            }
            else if (strActivityName.StartsWith("Wait till"))
            {
                eElObj = thePackage.Elements.AddNew(strActivityName, "Event");
            }
            else if (strActivityName == "ActivityInitial")
            {
                // eElObj = thePackage.Elements.AddNew("", "ActivityInitial"); // hmm does not seem to work
                eElObj = thePackage.Elements.AddNew(strActivityName, "Activity");
            }
            else if (strActivityName == "ActivityFinal")
            {
                //eElObj = thePackage.Elements.AddNew("", "ActivityFinal"); // hmm does not seem to work
                eElObj = thePackage.Elements.AddNew(strActivityName, "Activity");
            }
            else
            {
                eElObj = thePackage.Elements.AddNew(strActivityName, "Activity");
            }

            if (AddDiagramObject(diagram, eElObj.ElementID, strPos) == null)
            {
                throw new EpriException(strLogInfo, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, strLogInfo);
                return null;
            }

            if (oFields != null)
            {
                eElObj.Notes = oFields["description"].Value;

                EA.TaggedValue eaTgVal = eElObj.TaggedValues.AddNew("event", "TaggedValue");
                eaTgVal.Value = oFields["event"].Value;
                eaTgVal.Update();

                eaTgVal = eElObj.TaggedValues.AddNew("number", "TaggedValue");
                eaTgVal.Value = oFields["number"].Value;
                eaTgVal.Update();

                eaTgVal = eElObj.TaggedValues.AddNew("requirements", "TaggedValue");
                eaTgVal.Value = oFields["requirements"].Value;
                eaTgVal.Update();

                eaTgVal = eElObj.TaggedValues.AddNew("ActorName", "TaggedValue");
                eaTgVal.Value = strActorName;
                eaTgVal.Update();

                eElObj.Update();
            }

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);

            return eElObj;
        }

        private ImportFieldsValidator LoadFields(XElement elStep, ref string strOperationName, ref string strPrimaryActor, ref string strSecondaryActor, ref string strFirstStep, ref string strLastStep)
        {
            ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

            oFieldsValidator.Add(new FieldInfo("service", elStep));
            oFieldsValidator.Add(new FieldInfo("number", elStep));
            // oFieldsValidator.Add(new FieldInfo("Requirement", elStep.Element("Requirement"),"id",false));

            string strRequirements = "";

            EA.Package oRequirements = EAImporter.m_Packages.Add("UseCaseRepository", "RequirementLibrary");

            IEnumerable<XElement> elRequirements = elStep.Elements("Requirement");
            foreach (XElement elRequirement in elRequirements)
            {
                string strID = elRequirement.Element("id").Value;

                strRequirements = strRequirements + "," + strID;

                EAImporter.m_Requirements.AddInformationClass(oRequirements, strID, "");

            }

            if (strRequirements != "")
            {
                strRequirements = strRequirements.Remove(0, 1);
            }

            oFieldsValidator.Add(new FieldInfo("requirements", strRequirements));

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
                    throw new EpriException("REPEAT invalid arguments : " + oFieldsValidator["service"].Value, "");
                    //EAImporter.LogError(EAImporter.LogErrorLevel.A, "REPEAT invalid arguments : " + oFieldsValidator["service"].Value);
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
                //    oFieldsValidator.Add(new FieldInfo("TechRequirementsID", elStep.Element("Requirement"), "name", false));
            }
            else
            {
                oFieldsValidator.Add(new FieldInfo("name", elStep));
                oFieldsValidator.Add(new FieldInfo("Producer", elStep.Element("InformationProducer"), "name"));
                oFieldsValidator.Add(new FieldInfo("Receiver", elStep.Element("InformationReceiver"), "name"));
                oFieldsValidator.Add(new FieldInfo("InformationExchanged", elStep.Element("InformationModel"), "name"));
                oFieldsValidator.Add(new FieldInfo("Step", elStep, "number"));
                //    oFieldsValidator.Add(new FieldInfo("TechRequirementsID", elStep.Element("Requirement"), "name", false));
            }


            if (oFieldsValidator.ParseAndValidateFields("Scenario") == 0)
            {
                if (strOperationName == "CREATE")
                {
                    strPrimaryActor = oFieldsValidator["Producer"].Value;
                    strSecondaryActor = oFieldsValidator["Receiver"].Value;
                }
                else if ((strOperationName == "READ") || (strOperationName == "GET"))
                {
                    strPrimaryActor = oFieldsValidator["Receiver"].Value;
                    strSecondaryActor = oFieldsValidator["Producer"].Value;
                }
                else if ((strOperationName == "UPDATE") || (strOperationName == "CHANGE"))
                {
                    strPrimaryActor = oFieldsValidator["Producer"].Value;
                    strSecondaryActor = oFieldsValidator["Receiver"].Value;
                }
                else if (strOperationName == "EXECUTE")
                {
                    strPrimaryActor = oFieldsValidator["Producer"].Value;
                    strSecondaryActor = oFieldsValidator["Receiver"].Value;
                }
                else if (strOperationName == "DELETE")
                {
                    strPrimaryActor = oFieldsValidator["Producer"].Value;
                    strSecondaryActor = oFieldsValidator["Receiver"].Value;
                }
                else if ((strOperationName == "REPORT") || (strOperationName == "REPLY"))
                {
                    strPrimaryActor = oFieldsValidator["Producer"].Value;
                    strSecondaryActor = oFieldsValidator["Receiver"].Value;
                }
                else if (strOperationName == "TIMER")
                {
                    strPrimaryActor = oFieldsValidator["Producer"].Value;
                    //strSecondaryActor = oFieldsValidator["Receiver"].Value;
                }
                else if (strOperationName == "REPEAT")
                {
                    strPrimaryActor = "";
                }
            }

            return oFieldsValidator;
        }

        private int LoadStep(XElement elStep)
        {
            int iErrorCount = 0;

            Step oStep = new Step();

            oStep.m_oFields = LoadFields(elStep, ref oStep.m_strOperationName, ref oStep.m_strPrimaryActor, ref oStep.m_strSecondaryActor, ref oStep.m_strFirstStep, ref oStep.m_strLastStep);

            if (oStep.m_oFields.ErrorCount == 0)
            {
                m_LoadedSteps[Step.m_noSteps] = oStep;
                Step.m_noSteps++;
            }
            else
            {
                iErrorCount = iErrorCount + oStep.m_oFields.ErrorCount;
            }

            return iErrorCount;
        }

        private int ProcessLoadedSteps(EA.Package theStepsPackage, EA.Diagram diagram)
        {
            string strActorForNextActivity = "";
            string strActorForThisActivity = "";
            string strActorForInfoObj = "";
            string strTypeForNextConnector = "";
            string strOperationNameforNextTransition = "";

            EA.Element eaElActivityA = null;
            EA.Element eaElActivityB = null;
            EA.Element eaElInfoObj = null;

            if (AddSwimlane(diagram, theStepsPackage, ((Step)m_LoadedSteps[1]).m_strPrimaryActor) != 0)
            {
                return 1;
            }

            eaElActivityA = AddActivityToSwimlane(diagram, theStepsPackage, "ActivityInitial", ((Step)m_LoadedSteps[1]).m_strPrimaryActor, null);
            if (eaElActivityA == null)
            {
                return 1;
            }

            strTypeForNextConnector = "ControlFlow";

            strActorForNextActivity = ((Step)m_LoadedSteps[1]).m_strPrimaryActor;
            strOperationNameforNextTransition = "";

            for (int iStep = 1; iStep < Step.m_noSteps - 1; iStep++)
            {
                Step oStep = (Step)m_LoadedSteps[iStep];
                Step oNextStep = (Step)m_LoadedSteps[iStep + 1];

                strActorForThisActivity = strActorForNextActivity;

                if (AddSwimlane(diagram, theStepsPackage, strActorForThisActivity) != 0)
                {
                    return 1;
                }

                if ((oStep.m_strOperationName != "REPEAT") && (oStep.m_strOperationName != "TIMER"))
                {
                    eaElActivityB = AddActivityToSwimlane(diagram, theStepsPackage, oStep.m_oFields["name"].Value, strActorForThisActivity, oStep.m_oFields);
                    if (eaElActivityB == null)
                    {
                        return 1;
                    }

                    oStep.m_eaElActivity = eaElActivityB;
                }
                else if (oStep.m_strOperationName == "REPEAT")
                {
                    eaElActivityB = AddActivityToSwimlane(diagram, theStepsPackage, "Repeat till: " + oStep.m_oFields["event"].Value, strActorForThisActivity, oStep.m_oFields);
                    if (eaElActivityB == null)
                    {
                        return 1;
                    }
                }
                else if (oStep.m_strOperationName == "TIMER")
                {
                    eaElActivityB = AddActivityToSwimlane(diagram, theStepsPackage, "Wait till: " + oStep.m_oFields["InformationExchanged"].Value + " name=" + oStep.m_oFields["name"].Value, strActorForThisActivity, oStep.m_oFields);
                    if (eaElActivityB == null)
                    {
                        return 1;
                    }
                }

                if (oStep.m_oFields["InformationExchanged"] != null)
                {
                    strActorForInfoObj = oStep.m_strSecondaryActor;

                    if (oStep.m_strOperationName != "TIMER")
                    {
                        if (AddSwimlane(diagram, theStepsPackage, strActorForInfoObj) != 0)
                        {
                            return 1;
                        }

                        eaElInfoObj = AddDataObjectToSwimlane(diagram, theStepsPackage, oStep.m_oFields["InformationExchanged"].Value, strActorForInfoObj);
                        if (eaElInfoObj == null)
                        {
                            return 1;
                        }
                    }
                }

                // Make connector 1
                string strMsg = "Make connector 1 from Activity: " + eaElActivityA.Name + " to Activity: " + eaElActivityB.Name + " operation: " + strOperationNameforNextTransition;
                EAImporter.LogMsg(EAImporter.LogMsgType.Info, strMsg);

                if (AddConnector(diagram, eaElActivityA, eaElActivityB, "", strTypeForNextConnector) != 0)
                {
                    return 1;
                }

                // Make connector 2
                if ((oStep.m_strOperationName != "TIMER") && (oStep.m_strOperationName != "REPEAT"))
                {
                    if (oStep.m_oFields["InformationExchanged"] != null)
                    {
                        strMsg = "Make connector 2 from Activity: " + eaElActivityB.Name + " to InfoObj: " + eaElInfoObj.Name + " operation: " + oStep.m_strOperationName;
                        EAImporter.LogMsg(EAImporter.LogMsgType.Info, strMsg);

                        string strOperation = oStep.m_strOperationName + "(" + oStep.m_oFields["InformationExchanged"].Value + ")";

                        if ((oStep.m_strOperationName == "READ") || (oStep.m_strOperationName == "GET"))
                        {
                            if (AddConnector(diagram, eaElInfoObj, eaElActivityB, strOperation, "ObjectFlow") != 0)
                            {
                                return 1;
                            }
                        }
                        else
                        {
                            if (AddConnector(diagram, eaElActivityB, eaElInfoObj, strOperation, "ObjectFlow") != 0)
                            {
                                return 1;
                            }
                        }
                    }
                }


                if (oStep.m_strOperationName == "REPEAT")
                {
                    int iFirstStep = Convert.ToInt16(oStep.m_strFirstStep);
                    int iLastStep = Convert.ToInt16(oStep.m_strLastStep);
                    Step oFirstStep = ((Step)m_LoadedSteps[iFirstStep]);

                    int iLastPosition = (((iLastStep + 1) * iActivityHorizDistBetweenActivities) + iActivityHorizOffset) + iActivityWidth / 2;
                    int iFirstPosition = ((iFirstStep * iActivityHorizDistBetweenActivities) + iActivityHorizOffset) + iActivityWidth / 2;

                    string strPath = iLastPosition.ToString() + ":" + "-30;" + iFirstPosition.ToString() + ":-30;"; ;

                    if (AddConnector(diagram, eaElActivityB, oFirstStep.m_eaElActivity, "", "ControlFlow", strPath) != 0)
                    {
                        return 1;
                    }
                }

                // now setup for next iteration

                //default:
                eaElActivityA = eaElActivityB;
                strTypeForNextConnector = "ControlFlow";
                strActorForNextActivity = oNextStep.m_strPrimaryActor;
                strOperationNameforNextTransition = oStep.m_strOperationName;

                if (oStep.m_strOperationName == "CREATE")
                {
                    if (oStep.m_oFields["Receiver"].Value == oNextStep.m_strPrimaryActor)
                    {
                        eaElActivityA = eaElInfoObj;
                        strTypeForNextConnector = "ObjectFlow";
                    }
                }

                if (oNextStep.m_strOperationName == "TIMER")
                {
                    strActorForNextActivity = oNextStep.m_oFields["Producer"].Value;
                }

                if (oNextStep.m_strOperationName == "REPEAT")
                {
                    strActorForNextActivity = strActorForThisActivity;
                }
            }

            eaElActivityB = AddActivityToSwimlane(diagram, theStepsPackage, "ActivityFinal", strActorForThisActivity, null);
            if (eaElActivityB == null)
            {
                return 1;
            }

            string strMsg2 = "Make connector 1 from Activity: " + eaElActivityA.Name + " to Activity: " + eaElActivityB.Name + " operation: " + strOperationNameforNextTransition;
            EAImporter.LogMsg(EAImporter.LogMsgType.Info, strMsg2);

            if (AddConnector(diagram, eaElActivityA, eaElActivityB, "", strTypeForNextConnector) != 0)
            {
                return 1;
            }

            return 0;
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

                Swimlane.m_NoSwimlanes = 0;
                Step.m_noSteps = 0;
                m_NoActivities = 0;

                m_LoadedSteps = new Hashtable();
                m_Swimlanes = new Hashtable();

                Application.DoEvents();

                ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                oFieldsValidator.Add(new FieldInfo("name", elSequence));

                int iErrCnt = oFieldsValidator.ParseAndValidateFields("Scenario");

                iErrorCount = iErrorCount + iErrCnt;

                if (iErrCnt != 0)
                {
                    // assign temporary name to sequence to process steps for error reporting
                    oFieldsValidator["name"].Value = "DUMMY";
                }

                EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Importing Activity: " + oFieldsValidator["name"].Value);

                EA.Package theActivityPackage = EAImporter.m_Packages.Add(strParentPackageName, oFieldsValidator["name"].Value, "", elSequence);

                string strPackageName = strParentPackageName + "/" + oFieldsValidator["name"].Value;

                EA.Package theStepsPackage = EAImporter.m_Packages.Add(strPackageName, "Activity", "");
                if (theStepsPackage != null)
                {
                    EA.Diagram diagram = EAImporter.AddDiagram(theStepsPackage, "Activity Diagram", "Activity");

                    IEnumerable<XElement> elSteps = elSequence.Elements("Step");

                    Step oStep = new Step();
                    oStep.m_strOperationName = "INITIALSTATE";
                    m_LoadedSteps[Step.m_noSteps] = oStep;
                    Step.m_noSteps++;

                    foreach (XElement elStep in elSteps)
                    {
                        Application.DoEvents();

                        iErrorCount = iErrorCount + LoadStep(elStep);
                    }

                    if (iErrorCount == 0)
                    {
                        oStep = new Step();
                        oStep.m_strOperationName = "FINALSTATE";
                        m_LoadedSteps[Step.m_noSteps] = oStep;
                        Step.m_noSteps++;

                        iErrorCount = iErrorCount + ProcessLoadedSteps(theStepsPackage, diagram);

                        // now extend swimlanes to cover all activities
                        foreach (DictionaryEntry deSwimlane in m_Swimlanes)
                        {
                            Swimlane oSwimlane = (Swimlane)deSwimlane.Value;

                            oSwimlane.m_eaDiagramObject.right = iActivityHorizOffset + (iActivityHorizDistBetweenActivities * (m_NoActivities - 1)) + iActivityWidth;
                            oSwimlane.m_eaDiagramObject.Update();
                            diagram.Update();
                        }
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
