namespace TQM.Backoffice.Application.DTOs.Common
{
    public class MasterDataTemplate
    {
        public List<BaseDdl> SaleStatusList { get; set; } = new();
        public List<BaseDdl> PolicyStatusList { get; set; } = new();
        public List<BaseDdl> PRBStatusList { get; set; } = new();
        public List<BaseDdl> PaymentStatusList { get; set; } = new();
    }
}