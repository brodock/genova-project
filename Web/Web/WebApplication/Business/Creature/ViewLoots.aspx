<%@ Page Language="C#" MasterPageFile="~/layoutdefault.master" AutoEventWireup="true"
    CodeFile="ViewLoots.aspx.cs" Inherits="Web.Business.Creature.ViewLoots" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="detailBody" runat="Server">
    <div>
        <table>
            <caption>
                LOOTS INFORMATION
            </caption>
            <thead>
                <tr>
                    <th class="LeftItem">
                        <asp:Label ID="lblCreature" runat="server" Text="Name of the Creature:" />&nbsp;
                    </th>
                    <td class="RightItem">
                        <asp:TextBox ID="txtCreature" runat="server" Width="300px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="Center Submit">
                        This feature will only verify the existence of Loot maintained by the database server.
                        Loot's default emulator will not appear here.
                    </td>
                </tr>
            </thead>
            <tfoot>
                <tr>
                    <td colspan="2" class="Center">
                        <asp:Button ID="btnFindCreature" runat="server" Text="Find Creature" OnClick="btnFindCreature_Click" />
                    </td>
                </tr>
            </tfoot>
            <tbody>
                <tr>
                    <td id="divGridView" colspan="2" class="Center">
                        <asp:GridView ID="gridLoot" runat="server" AutoGenerateColumns="False" Width="80%">
                            <HeaderStyle CssClass="GridHeader" />
                            <RowStyle CssClass="GridRow" />
                            <Columns>
                                <asp:BoundField DataField="Item" HeaderText="Item" />
                                <asp:BoundField DataField="Quantidade" HeaderText="Quantity" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
