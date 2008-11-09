<%@ Page Language="C#" MasterPageFile="~/layoutdefault.master" AutoEventWireup="true"
    CodeFile="Register.aspx.cs" Inherits="Web.Business.ShardUser.Register" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="detailBody" runat="Server">
    <div>
        <table>
            <caption>
                SHARD ACCOUNT
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
                        <asp:Label ID="lblLogin" runat="server" Text="Login ID:" />&nbsp;
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtLogin" runat="server" Width="200px" MaxLength="20" />&nbsp;<asp:Image
                            ID="r1" runat="server" ImageUrl="~/Content/Images/required.png" AlternateText="*" />
                    </td>
                </tr>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblPassword" runat="server" Text="Password:" />&nbsp;
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtPassword" runat="server" Width="200px" MaxLength="12" TextMode="Password" />&nbsp;<asp:Image
                            ID="r2" runat="server" ImageUrl="~/Content/Images/required.png" AlternateText="*" />
                    </td>
                </tr>
                <tr>
                    <th class="LeftItem" style="vertical-align: top;">
                        <asp:Label ID="lblAction" runat="server" Text="What do you want?" />&nbsp;
                    </th>
                    <td class="RightItem">
                        <table cellpadding="0" cellspacing="0">
                            <tbody>
                                <tr>
                                    <td style="width: 170px;">
                                        <asp:RadioButtonList ID="rblAction" runat="server" CssClass="List" Width="170px">
                                            <asp:ListItem Text="Create a new account in shard" Value="1" Selected="True" />
                                            <asp:ListItem Text="Involving an existing account" Value="2" />
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
            </tbody>
        </table>
    </div>
</asp:Content>
