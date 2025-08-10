using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class ProcessOrderOperationDto
    {
        [JsonProperty("results")]
        public List<ProcessOrderOperationResultDto> Results { get; set; }
    }

    public class ProcessOrderOperationResultDto
    {
        [JsonProperty("__metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("OrderInternalBillOfOperations")]
        public string OrderInternalBillOfOperations { get; set; }

        [JsonProperty("OrderIntBillOfOperationsItem")]
        public string OrderIntBillOfOperationsItem { get; set; }

        [JsonProperty("ManufacturingOrder")]
        public string ManufacturingOrder { get; set; }

        [JsonProperty("ManufacturingOrderSequence")]
        public string ManufacturingOrderSequence { get; set; }

        [JsonProperty("MfgOrderSequenceText")]
        public string MfgOrderSequenceText { get; set; }

        [JsonProperty("ManufacturingOrderOperation")]
        public string ManufacturingOrderOperation { get; set; }

        [JsonProperty("ManufacturingOrderSubOperation")]
        public string ManufacturingOrderSubOperation { get; set; }

        [JsonProperty("ManufacturingOrderCategory")]
        public string ManufacturingOrderCategory { get; set; }

        [JsonProperty("ManufacturingOrderType")]
        public string ManufacturingOrderType { get; set; }

        [JsonProperty("MfgOrderOperationText")]
        public string MfgOrderOperationText { get; set; }

        [JsonProperty("OrderOperationLongText")]
        public string OrderOperationLongText { get; set; }

        [JsonProperty("MfgOrderOperationIsPhase")]
        public bool MfgOrderOperationIsPhase { get; set; }

        [JsonProperty("MfgOrderPhaseSuperiorOperation")]
        public string MfgOrderPhaseSuperiorOperation { get; set; }

        [JsonProperty("OperationIsCreated")]
        public string OperationIsCreated { get; set; }

        [JsonProperty("OperationIsReleased")]
        public string OperationIsReleased { get; set; }

        [JsonProperty("OperationIsPrinted")]
        public string OperationIsPrinted { get; set; }

        [JsonProperty("OperationIsConfirmed")]
        public string OperationIsConfirmed { get; set; }

        [JsonProperty("OperationIsPartiallyConfirmed")]
        public string OperationIsPartiallyConfirmed { get; set; }

        [JsonProperty("OperationIsDeleted")]
        public string OperationIsDeleted { get; set; }

        [JsonProperty("OperationIsTechlyCompleted")]
        public string OperationIsTechlyCompleted { get; set; }

        [JsonProperty("OperationIsClosed")]
        public string OperationIsClosed { get; set; }

        [JsonProperty("OperationIsScheduled")]
        public string OperationIsScheduled { get; set; }

        [JsonProperty("OperationIsPartiallyDelivered")]
        public string OperationIsPartiallyDelivered { get; set; }

        [JsonProperty("OperationIsDelivered")]
        public string OperationIsDelivered { get; set; }

        [JsonProperty("ProductionPlant")]
        public string ProductionPlant { get; set; }

        [JsonProperty("WorkCenterInternalID")]
        public string WorkCenterInternalID { get; set; }

        [JsonProperty("WorkCenterTypeCode")]
        public string WorkCenterTypeCode { get; set; }

        [JsonProperty("WorkCenter")]
        public string WorkCenter { get; set; }

        [JsonProperty("OperationControlProfile")]
        public string OperationControlProfile { get; set; }

        [JsonProperty("OpErlstSchedldExecStrtDte")]
        public string OpErlstSchedldExecStrtDte { get; set; }

        [JsonProperty("OpErlstSchedldExecStrtTme")]
        public string OpErlstSchedldExecStrtTme { get; set; }

        [JsonProperty("OpErlstSchedldExecEndDte")]
        public string OpErlstSchedldExecEndDte { get; set; }

        [JsonProperty("OpErlstSchedldExecEndTme")]
        public string OpErlstSchedldExecEndTme { get; set; }

        [JsonProperty("OpActualExecutionStartDate")]
        public string OpActualExecutionStartDate { get; set; }

        [JsonProperty("OpActualExecutionStartTime")]
        public string OpActualExecutionStartTime { get; set; }

        [JsonProperty("OpActualExecutionEndDate")]
        public string OpActualExecutionEndDate { get; set; }

        [JsonProperty("OpActualExecutionEndTime")]
        public string OpActualExecutionEndTime { get; set; }

        [JsonProperty("ErlstSchedldExecDurnInWorkdays")]
        public int ErlstSchedldExecDurnInWorkdays { get; set; }

        [JsonProperty("OpActualExecutionDays")]
        public int OpActualExecutionDays { get; set; }

        [JsonProperty("OperationUnit")]
        public string OperationUnit { get; set; }

        [JsonProperty("OperationUnitISOCode")]
        public string OperationUnitISOCode { get; set; }

        [JsonProperty("OperationUnitSAPCode")]
        public string OperationUnitSAPCode { get; set; }

        [JsonProperty("OpPlannedTotalQuantity")]
        public string OpPlannedTotalQuantity { get; set; }

        [JsonProperty("OpTotalConfirmedYieldQty")]
        public string OpTotalConfirmedYieldQty { get; set; }

        [JsonProperty("LastChangeDateTime")]
        public string LastChangeDateTime { get; set; }

        [JsonProperty("DestinoRecetaDeControl")]
        public string DestinoRecetaDeControl { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }
    }
}
