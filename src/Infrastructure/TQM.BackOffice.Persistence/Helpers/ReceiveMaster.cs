using TQM.Backoffice.Application.DTOs.Common;
namespace TQM.BackOffice.Persistence.Helpers
{
    public class ReceiveMaster
    {
        public static List<BaseDdl> GetVoidReasonList()
        {
            List<BaseDdl>  ListData = new();
            ListData.Add( new BaseDdl{ Id = "1", Title = "ระบุวันที่ TQM รับเงินผิด"});
            ListData.Add( new BaseDdl{ Id = "2", Title = "ระบุวันที่ลูกค้าชำระผิด"});
            ListData.Add( new BaseDdl{ Id = "3", Title = "ระบุวันที่ PayIn ผิด"});
            ListData.Add( new BaseDdl{ Id = "4", Title = "ระบุข้อมูล TID ไม่ครบ หรือผิด"});
            ListData.Add( new BaseDdl{ Id = "5", Title = "ระบุข้อมูล Cost ไม่ครบ หรือผิด"});
            ListData.Add( new BaseDdl{ Id = "6", Title = "ตัดยอดผิด ไม่ตรงกับที่รับชำระจริง"});
            ListData.Add( new BaseDdl{ Id = "7", Title = "ตัดผิดบุ๊ครับเงิน"});
            ListData.Add( new BaseDdl{ Id = "8", Title = "ตัดผิดงวด"});
            ListData.Add( new BaseDdl{ Id = "9", Title = "ตัดลูกค้าผิดราย"});
            ListData.Add( new BaseDdl{ Id = "10", Title = "ไม่ส่งสลิป หรือไม่สรุปยอดโอนเงิน"});
            ListData.Add( new BaseDdl{ Id = "11", Title = "กรณีแบ่งชำระ : ตัดยอดแยก SaleID"});
            ListData.Add( new BaseDdl{ Id = "12", Title = "อื่นๆ โปรดระบุ"});
            return ListData;
        }

        public static List<BaseDdl> GetCloneReasonList()
        {
            List<BaseDdl>  ListData = new();
            ListData.Add( new BaseDdl{ Id = "1", Title = "ไม่ Clone"});
            ListData.Add( new BaseDdl{ Id = "2", Title = "Clone Auto ภายใน book เดียวกัน"});
            ListData.Add( new BaseDdl{ Id = "3", Title = "Clone Auto แบบข้าม book"});
            ListData.Add( new BaseDdl{ Id = "4", Title = "ยกเลิก"});
            return ListData;
        }

        public static List<BaseDdl> GetReceiveAmountTypeList()
        {
            List<BaseDdl>  ListData = new();
            ListData.Add( new BaseDdl{ Id = "SUM", Title = "รับยอดรวม"});
            ListData.Add( new BaseDdl{ Id = "INSTALLMENT", Title = "รับรายงวด"});
            return ListData;
        }
    }
}