namespace LoginMS.Models
{
    public class EmailMetadata
    {
        public string vls_toaddress { get; set; }
        public string vls_subject { get; set; }
        public string? vls_body { get; set; }
        public string? vls_attachmentpath { get; set; }

        public EmailMetadata(string psi_toaddress, string psi_subject, string? psi_body = "",
            string? psi_attachmentpath = "")
        {
            vls_toaddress = psi_toaddress;
            vls_subject = psi_subject;
            vls_body = psi_body;
            vls_attachmentpath = psi_attachmentpath;
        }
    }
}
