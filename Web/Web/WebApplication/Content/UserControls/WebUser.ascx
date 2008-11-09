<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WebUser.ascx.cs" Inherits="Web.UserControls.WebUser" %>
<table class="WebAccountBlock Center">
    <thead>
        <tr>
            <th colspan="2" class="BlockTitle Bold">
                Web Account
            </th>
        </tr>
    </thead>
    <tfoot>
        <tr>
            <td id="notLoggedConfirm" runat="server" colspan="2" class="Center Submit" visible="true">
                <asp:Button ID="btnLogin" runat="server" Text="Enter" OnClick="btnLogin_Click" />
            </td>
            <td id="loggedConfirm" runat="server" colspan="2" class="Center Submit" visible="false">
                <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click" />
            </td>
        </tr>
    </tfoot>
    <tbody>
        <tr>
            <td colspan="2" class="Center">
                <asp:Image ID="imgUserAvatar" runat="server" ImageAlign="Middle" AlternateText="Unknown User" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblLogin" runat="server" Text="Login:" CssClass="Bold" />
            </th>
            <td id="notLoggedLogin" runat="server" class="RightItem" visible="true">
                <asp:TextBox ID="txtLogin" runat="server" />&nbsp;<asp:Image ID="r1" runat="server"
                    ImageUrl="~/Content/Images/required.png" AlternateText="*" />
            </td>
            <td id="loggedLogin" runat="server" class="RightItem" visible="false">
                <asp:Label ID="lblUserLogin" runat="server" />
            </td>
        </tr>
        <tr id="notLoggedPassword" runat="server" visible="true">
            <th class="LeftItem">
                <asp:Label ID="lblPassword" runat="server" Text="Password:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />&nbsp;<asp:Image ID="r2" runat="server"
                    ImageUrl="~/Content/Images/required.png" AlternateText="*" />
            </td>
        </tr>
        <tr id="loggedPassword" runat="server" visible="false">
            <th class="LeftItem">
                <asp:Label ID="lblTypeAccount" runat="server" Text="Level:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblUserType" runat="server" />
            </td>
        </tr>
        <tr id="notLoggedInfo" runat="server" visible="true">
            <td colspan="2" class="Center">
                <asp:HyperLink ID="linkCreateWebAccount" runat="server" Text="Click here to create an web account"
                    NavigateUrl="~/Business/WebUser/Register.aspx" />
            </td>
        </tr>
    </tbody>
</table>
