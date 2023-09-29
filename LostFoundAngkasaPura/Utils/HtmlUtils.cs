using DocumentFormat.OpenXml.EMMA;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Utils
{
    public class HtmlUtils
    {
        private UploadLocation _uploadLocation;
        public HtmlUtils(UploadLocation uploadLocation)
        {
            _uploadLocation = uploadLocation;
        }

        public string BaseReport(string body)
        {
            return "" +
                $"<!DOCTYPE html>" +
                $"<html lang='en'>" +
                $"  <head>" +
                $"    <meta charset='UTF-8' />" +
                $"    <meta name='viewport' content='width=device-width, initial-scale=1.0' />" +
                $"    <title>Report</title>" +
                $"    <link rel='preconnect' href='https://fonts.googleapis.com' />" +
                $"    <link rel='preconnect' href='https://fonts.gstatic.com' crossorigin />" +
                $"    <!-- font style yang dipakai -->" +
                $"    <link href='https://fonts.googleapis.com/css2?family=PT+Sans+Caption:wght@400;700&display=swap' rel='stylesheet' />" +
                $"    <link href='https://fonts.googleapis.com/css2?family=Montserrat:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap' rel='stylesheet' />" +
                $"    <link href='https://fonts.googleapis.com/css2?family=DM+Sans:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;0,1000;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900;1,1000&display=swap' rel='stylesheet' />" +
                $"    <link href='https://fonts.googleapis.com/css2?family=Ephesis&display=swap' rel='stylesheet' />\r\n    <link href='https://fonts.googleapis.com/css2?family=Raleway:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap' rel='stylesheet' />" +
                $"    <link href='https://fonts.googleapis.com/css2?family=Engagement&display=swap' rel='stylesheet' />" +
                $"   <style>" +
                $"      page {{" +
                $"        box-shadow: 0 0 0.5cm rgb(0 0 0 / 8%);" +
                $"      }}" +
                $"      .text-center{{" +
                $"        text-align: center;" +
                $"      }}" +
                $"      .w-100{{" +
                $"        width: 100%;" +
                $"      }}" +
                $"      @page        {{ " +
                $"          size: auto;   /* auto is the initial value */ " +
                $"          /* this affects the margin in the printer settings */ " +
                $"          margin: 25mm 25mm 25mm 25mm;  " +
                $"      }} " +
                $"      .pb-50{{" +
                $"        padding-bottom: 50px;" +
                $"      }}" +
                $"      td{{" +
                $"        vertical-align: top;" +
                $"      }}" +
                $"      img{{" +
                $"          width:auto;" +
                $"          height:200px;" +
                $"      }}" +
                $"    </style>" +
                $"  </head>" +
                $"  <body onload='window.print()'>" +
                $"    <!-- PAGE: cover -->" +
                $"      <table class='w-100'>" +
                $"        <tr>" +
                $"          <th colspan='4' class='text-center pb-50'>Laporan Lost & Found Bandara Sams Sepinggan Balikpapan</th>" +
                $"        </tr>" +
                            body +
                $"      </table>" +
                $"  </body>" +
                $"  <script language='javascript' type='text/javascript'>" +
                $"    window.setTimeout('window.close()',2000);" +
                $"  </script>" +
                $"</html>" +
                $"" +
                $"";
        }

        public string HtmlBarang(DAL.Model.ItemFound itemFound)
        {
            return
                $"        <!--start detail barang-->" +
                $"        <tr>" +
                $"          <td style='font-weight: bold;' colspan=3>Detail Barang</td>" +
                $"        </tr>" +
                $"        <tr>" +
                $"          <td width='200px' height='25px'>Nama Barang</td>" +
                $"          <td width='300px' height='25px'>{itemFound.Name}</td>" +
                $"          <td colspan='2' rowspan='5'>" +
                $"             <img src='{_uploadLocation.Url(itemFound.Image)}'/>" +
                $"          </td>" +
                $"        </tr>" +
                $"        <tr>" +
                $"          <td width='200px' height='25px'>Status</td>" +
                $"          <td width='300px' height='25px'>{itemFound.Status}</td>" +
                $"        </tr>" +
                $"        <tr>" +
                $"          <td width='200px' height='25px'>Kategori</td>" +
                $"          <td width='300px' height='25px'>{itemFound.Category}</td>" +
                $"        </tr>" +
                $"        <tr>" +
                $"          <td width='200px' height='25px'>Description</td>" +
                $"          <td width='300px' height='25px'>{itemFound.Description}</td>" +
                $"        </tr>" +
                $"        <tr><td></td></tr>" +
                $"        <!--end detail barang-->";
        }

        public string HtmlCustomer(DAL.Model.User user)
        {
            return
            $"        <!--start detail customer-->" +
            $"        <tr>" +
            $"          <td style='font-weight: bold;vertical-align: bottom;' height='80px' colspan=3>Detail Customer</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Nama Customer</td>" +
            $"          <td width='300px' height='25px'>{user.Name}</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>No. Hp</td>" +
            $"          <td width='300px' height='25px'>{user.Phone}</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Email</td>" +
            $"          <td width='300px' height='25px'>{user.Email}</td>" +
            $"        </tr>" +
            $"        <!--end detail customer-->";
        }

        public string HtmlClaim(DAL.Model.ItemClaim itemClaim)
        {
            return
            $"        <!--start detail claim-->" +
            $"        <tr>" +
            $"          <td style='font-weight: bold;vertical-align: bottom;' height='80px' colspan=3>Detail Claim</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>No Identitas</td>" +
            $"          <td width='300px' height='25px'>{itemClaim.IdentityNumber}</td>" +
            $"          <td colspan='2' rowspan='5'>" +
            $"            <img src='{_uploadLocation.Url(itemClaim.ProofImage)}'/>" +
            $"          </td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Description</td>" +
            $"          <td width='300px' height='25px'>{itemClaim.ProofDescription}</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Rating</td>" +
            $"          <td width='300px' height='25px'>{itemClaim.Rating}</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Rating Komentar</td>" +
            $"          <td width='300px' height='25px'>{itemClaim.RatingComentar}</td>" +
            $"        </tr>" +
            $"        <tr><td></td></tr>" +
            $"        <!--end detail customer-->";
        }

        public string HtmlApproval(DAL.Model.ItemClaimApproval approval)
        {
            return $"        <!--start detail approval-->" +
            $"        <tr>" +
            $"          <td style='font-weight: bold;vertical-align: bottom;' height='80px'colspan=3>Detail Approval</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Approval</td>" +
            $"          <td width='300px' height='25px'>{approval.Status}</td>" +
            $"        </tr>" +
            approval.Status==ItemFoundStatus.Rejected?
            (
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Alasan Reject</td>" +
            $"          <td width='300px' height='25px'>{approval.RejectReason}</td>" +
            $"        </tr>"
            )
            :(
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Lokasi Pengambilan</td>" +
            $"          <td width='300px' height='25px'>{approval.ClaimLocation}</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Tanggal Pengambilan</td>" +
            $"          <td width='300px' height='25px'>{approval.ClaimLocation}</td>" +
            $"        </tr>")+
            $"        <!--end detail approval-->";
        }

        public string HtmlClosing(DAL.Model.ClosingDocumentation closing)
        {
            var imageUrl = _uploadLocation.Url(closing.TakingItemImage);
            var beritaAcaraUrl = _uploadLocation.Url(closing.NewsDocumentation);
            return

            $"        <!--start detail closing-->" +
            $"        <tr>" +
            $"          <td style='font-weight: bold;vertical-align: bottom;' height='80px' colspan=3>Detail Closing</td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Nama Petugas</td>" +
            $"          <td width='300px' height='25px'>{closing.ClosingAgent}</td>" +
            $"          <td colspan='2' rowspan='5'>" +
            $"            <img src='{_uploadLocation.Url(closing.TakingItemImage)}'/>" +
            $"          </td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td width='200px' height='25px'>Url Berita Acara</td>" +
            $"          <td width='300px' height='25px'><a href='{beritaAcaraUrl}'>{beritaAcaraUrl}</a></td>" +
            $"        </tr>" +
            $"        <tr>" +
            $"          <td></td>" +
            $"        </tr>" +
            $"        <!--end detail closing-->";
        }
    }
}
