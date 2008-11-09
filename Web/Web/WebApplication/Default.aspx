<%@ Page Language="C#" MasterPageFile="~/layoutdefault.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Web.Default" %>

<%@ Register TagPrefix="UC" TagName="WebUser" Src="~/Content/UserControls/WebUser.ascx" %>
<%@ Register TagPrefix="UC" TagName="ShardStatus" Src="~/Content/UserControls/ShardStatus.ascx" %>
<%@ Register TagPrefix="UC" TagName="AccountStatus" Src="~/Content/UserControls/AccountStatus.ascx" %>
<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="Server">
    <link href="Content/CSS/Default/initialPage.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="detailBody" runat="Server">
    <div>
        <table>
            <thead />
            <tfoot />
            <tbody>
                <tr>
                    <td class="DivisionBlock" rowspan="2" style="width: 30%;">
                        <div>
                            <UC:WebUser ID="ucUser" runat="server" />
                        </div>
                    </td>
                    <td class="DivisionBlock">
                        <div>
                            <UC:ShardStatus ID="ucShardStatus" runat="server" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="DivisionBlock">
                        <UC:AccountStatus ID="ucAccountStatus" runat="server" />
                        &nbsp;
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
