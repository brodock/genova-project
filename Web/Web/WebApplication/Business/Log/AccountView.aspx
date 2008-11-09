<%@ Page Language="C#" MasterPageFile="~/layoutdefault.master" AutoEventWireup="true"
    CodeFile="AccountView.aspx.cs" Inherits="Web.Business.Log.AccountView" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="detailBody" runat="Server">
    <div>
        <table>
            <caption>
                LOG CONNECTION
            </caption>
            <thead>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblAccount" runat="server" Text="Account:" />&nbsp;
                    </th>
                    <td class="RightItem">
                        <asp:DropDownList ID="ddlAccount" runat="server" CssClass="List">
                            <asp:ListItem Text="..:: No account to select ::.." Value="0" Selected="True" />
                        </asp:DropDownList>
                        &nbsp;<asp:Image ID="r1" runat="server" ImageUrl="~/Content/Images/required.png"
                            AlternateText="*" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="Center Submit">
                        <asp:Button ID="btnList" runat="server" Text="List Records" OnClick="btnList_Click" />
                    </td>
                </tr>
            </thead>
            <tfoot />
            <tbody>
                <tr>
                    <td id="divGridView" colspan="2" class="Center">
                        <asp:GridView ID="gridLog" runat="server" AutoGenerateColumns="False" AllowPaging="True"
                            PageSize="20" OnPageIndexChanging="gridLog_PageIndexChanging" Width="80%">
                            <HeaderStyle CssClass="GridHeader" />
                            <RowStyle CssClass="GridRow" />
                            <PagerSettings NextPageText="Next Page" PreviousPageText="Back Page" Mode="NextPrevious" />
                            <PagerStyle CssClass="GridPaging" />
                            <Columns>
                                <asp:BoundField DataField="Entrada" HeaderText="Start Time" />
                                <asp:BoundField DataField="Saida" HeaderText="End Time" />
                                <asp:BoundField DataField="IP" HeaderText="IP" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
