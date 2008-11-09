<%@ Page Language="C#" MasterPageFile="~/layoutdefault.master" AutoEventWireup="true"
    CodeFile="CharactersView.aspx.cs" Inherits="Web.Business.ShardUser.CharactersView" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="cBody" ContentPlaceHolderID="detailBody" runat="Server">
    <div>
        <table>
            <caption>
                CHARACTERS INFORMATION
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
                        <asp:Button ID="btnList" runat="server" Text="List Characters" OnClick="btnList_Click" />
                    </td>
                </tr>
            </thead>
            <tfoot />
            <tbody>
                <tr>
                    <td id="divGridView" colspan="2" class="Center">
                        <asp:GridView ID="gridCharacters" runat="server" AutoGenerateColumns="False" Width="80%"
                            OnRowDataBound="gridCharacters_RowDataBound">
                            <HeaderStyle CssClass="GridHeader" />
                            <RowStyle CssClass="GridRow" />
                            <Columns>
                                <asp:BoundField DataField="NomePersonagem" HeaderText="Character" />
                                <asp:BoundField DataField="DataCadastro" HeaderText="Created" />
                                <asp:TemplateField HeaderText="Active">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatus" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Confirmado") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="PontosXP" HeaderText="Points of Experience" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
