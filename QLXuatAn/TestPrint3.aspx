﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestPrint3.aspx.cs" Inherits="QLXuatAn_TestPrint3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body{
            font-family: Arial;
        }
        .tbl{margin: 0 auto;}
        .tbl2{
            width:100%;
        }
        .tbl td, .tbl2 td {
            padding: 5px;
        }
        .tdCenter{text-align:center;}
        .tdLeft {
            text-align: left;
        }
        .headerText{
            font-weight:bold;
            font-size:18px;
            text-transform:uppercase;
        }
        .textStrong{font-weight:bold;}
        .hr{
            height:2px;
            width:60%;
            margin: 0 auto;
            display:block;
            background:#808080;
        }
        .ptext {
            font-size: 20px;
            font-weight: 700;
            text-transform: uppercase;
        }
        .textPrice {
            font-size: 18px;
            font-weight: 700;
            text-transform: uppercase;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <table class="tbl">
        <tr>
            <td class="tdCenter">
                <span class="headerText">
                    CÔNG TY CP DỊCH VỤ HÀNG HÓA HÀNG KHÔNG VN
                </span>
            </td>
        </tr>
        <tr>
            <td class="tdCenter">
                Địa chỉ: Cảng hàng không quốc tế Nội Bài, X.Phú
                Cường, H.Sóc Sơn, TP.Hà Nội, Việt Nam
            </td>
        </tr>
        <tr>
            <td class="tdCenter">
                <span class="textStrong">
                    Mã số thuế: 0106825508
                </span>
            </td>
        </tr>
        <tr>
            <td>
                <span class="hr" >&nbsp;</span>
            </td>
        </tr>
        <tr>
            <td class="tdCenter">
                <p class="ptext">
                    VÉ ĐỖ SÂN Ô TÔ
                </p>
                <p>
                    Thẻ lượt ô tô
                </p>
                <p>
                    Vé lượt
                </p>
                <p>
                    Mẫu số: 01/VE009; &nbsp;&nbsp;&nbsp; KH: AC/17T
                </p>
               <p>
                   09439409
               </p>
            </td>
        </tr>
        <tr>
            <td>
                <table class="tbl2">
                    <tr>
                        <th class="tdLeft">Ngày vào: </th>
                        <td>13/07/2017</td>
                        <th class="tdLeft">Ngày ra: </th>
                        <td>13/07/2017</td>
                    </tr>
                    <tr>
                        <th class="tdLeft">Giờ vào</th>
                        <td>15:14:00</td>
                        <th class="tdLeft">Giờ ra</th>
                        <td>22:18:36</td>
                    </tr>
                    <tr>
                        <th class="tdLeft">BKS:</th>
                        <td>30A5568</td>
                        <td colspan="2"></td>
                    </tr>
                    <tr>
                        <th class="tdLeft">Thời gian đỗ:</th>
                        <td>7h4m36s</td>
                        <td colspan="2"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="tdCenter">
                <p class="textPrice">
                    GIÁ TIỀN: 380,000 đ
                </p>
                <p>
                    <i>Đã bao gồm thuế GTGT</i>
                </p>
            </td>
        </tr>
        <tr>
            <td>
                <span class="hr">&nbsp;</span>
            </td>
        </tr>
        <tr>
            <td class="tdCenter">
                <p>
                    <i>Liên lưu</i>
                </p>
            </td>
        </tr>
        <tr>
            <td class="tdCenter">
                <p>Đơn vị cung cấp phần mềm: Công ty CP thương</p>
                <p>mại và công nghệ kỹ thuật ECOME</p>
                <p>Mã số thuế: 0106114624</p>
            </td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
