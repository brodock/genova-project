<%@ Page Language="C#" MasterPageFile="~/layoutdefault.master" AutoEventWireup="true"
    CodeFile="ViewAll.aspx.cs" Inherits="Web.Business.Creature.ViewAll" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="detailBody" runat="Server">
    <div>
        <table>
            <caption>
                CREATURES INFORMATION
            </caption>
            <thead>
                <tr>
                    <td class="Center">
                        Below you can check the list of all creatures exist on the server. Some of these
                        securities and other creatures have not. If any creature is not on this list, which
                        means you will not find in the world.
                    </td>
                </tr>
            </thead>
            <tfoot />
            <tbody>
                <tr>
                    <td id="divGridView" colspan="2" class="Center">
                        <asp:GridView ID="gridCreatures" runat="server" AutoGenerateColumns="False" Width="80%">
                            <HeaderStyle CssClass="GridHeader" />
                            <RowStyle CssClass="GridRow" />
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText=" Name of the Creature" />
                                <asp:BoundField DataField="Title" HeaderText="Title" />
                            </Columns>
                        </asp:GridView>
                        <asp:Label ID="lblNotFound" runat="server" Text="There is not inserted creatures in the world." Visible="false" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
