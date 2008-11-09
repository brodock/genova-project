<%@ Page Language="C#" MasterPageFile="~/layoutdefault.master" AutoEventWireup="true"
    CodeFile="Register.aspx.cs" Inherits="Web.Business.WebUser.Register" %>

<asp:Content ID="cHeader" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="detailBody" runat="Server">
    <div>
        <table>
            <caption>
                REGISTER WEB ACCOUNT
            </caption>
            <thead />
            <tfoot>
                <tr>
                    <td colspan="2" class="Center Submit">
                        <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" />
                    </td>
                </tr>
            </tfoot>
            <tbody>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblName" runat="server" Text="Full Name:" CssClass="Bold" />
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtName" runat="server" Width="200px" MaxLength="50" />&nbsp;<asp:Image
                            ID="r1" runat="server" ImageUrl="~/Content/Images/required.png" AlternateText="*" />
                    </td>
                </tr>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblEmail" runat="server" Text="E-mail:" CssClass="Bold" />
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtEmail" runat="server" Width="200px" MaxLength="50" />&nbsp;<asp:Image
                            ID="r2" runat="server" ImageUrl="~/Content/Images/required.png" AlternateText="*" />
                    </td>
                </tr>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblSex" runat="server" Text="Sex:" CssClass="Bold" />
                    </th>
                    <td class="RightItem">
                        <table cellpadding="0" cellspacing="0">
                            <tbody>
                                <tr>
                                    <td style="width: 120px;">
                                        <asp:RadioButtonList ID="rblSex" runat="server" CssClass="List" RepeatColumns="2"
                                            Width="120px">
                                            <asp:ListItem Text="Man" Value="M" Selected="True" />
                                            <asp:ListItem Text="Woman" Value="F" />
                                        </asp:RadioButtonList>
                                    </td>
                                    <td>
                                        &nbsp;<asp:Image ID="r3" runat="server" ImageUrl="~/Content/Images/required.png"
                                            AlternateText="*" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblBirthDate" runat="server" Text="Birth Date:" CssClass="Bold" />
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtBirthDate" runat="server" Width="200px" MaxLength="10" ToolTip="Format: dd/MM/yyyy" />&nbsp;<asp:Image
                            ID="r4" runat="server" ImageUrl="~/Content/Images/required.png" AlternateText="*" />
                    </td>
                </tr>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblLogin" runat="server" Text="Login:" CssClass="Bold" />
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtLogin" runat="server" Width="200px" MaxLength="15" />&nbsp;<asp:Image
                            ID="r5" runat="server" ImageUrl="~/Content/Images/required.png" AlternateText="*" />
                    </td>
                </tr>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblPassword" runat="server" Text="Password:" CssClass="Bold" />
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtPassword" runat="server" Width="200px" MaxLength="12" TextMode="Password" />&nbsp;<asp:Image
                            ID="r6" runat="server" ImageUrl="~/Content/Images/required.png" AlternateText="*" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
