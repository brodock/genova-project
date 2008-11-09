<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountStatus.ascx.cs"
    Inherits="Web.UserControls.AccountStatus" %>
<table id="activeControl" runat="server" class="AccountStatusBlock Center">
    <thead>
        <tr>
            <th class="BlockTitle">
                Account Status
            </th>
        </tr>
    </thead>
    <tfoot />
    <tbody>
        <tr>
            <td id="divGridView">
                <asp:GridView ID="gridAccounts" runat="server" AutoGenerateColumns="False" Width="100%"
                    BorderStyle="None" BorderWidth="0px" OnRowDataBound="gridAccounts_RowDataBound">
                    <HeaderStyle Font-Bold="true" />
                    <RowStyle CssClass="GridRow" />
                    <Columns>
                        <asp:BoundField DataField="Login" HeaderText="Login" HeaderStyle-HorizontalAlign="Left"
                            ItemStyle-HorizontalAlign="Left" />
                        <asp:TemplateField HeaderText="Active">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Ativo") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </tbody>
</table>
