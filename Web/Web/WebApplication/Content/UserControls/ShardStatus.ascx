<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShardStatus.ascx.cs" Inherits="Web.UserControls.ShardStatus" %>
<table class="ShardStatusBlock Center">
    <thead>
        <tr>
            <th class="BlockTitle" colspan="2">
                Shard Status
            </th>
        </tr>
    </thead>
    <tfoot />
    <tbody>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleDescription" runat="server" Text="Description:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailDescription" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleIP" runat="server" Text="IP:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailIP" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitlePort" runat="server" Text="Port:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailPort" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleClient" runat="server" Text="Client Requirement:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailClient" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleOnlineUsers" runat="server" Text="Online Users:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailOnlineUsers" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleMobiles" runat="server" Text="Total Mobiles:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailMobiles" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleItems" runat="server" Text="Total Items:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailItems" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleUptime" runat="server" Text="Server Uptime:" CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailUptime" runat="server" Text="-" />
            </td>
        </tr>
        <tr>
            <th class="LeftItem">
                <asp:Label ID="lblTitleLastInfoUpdate" runat="server" Text="Updated information on:"
                    CssClass="Bold" />
            </th>
            <td class="RightItem">
                <asp:Label ID="lblDetailLastInfoUpdate" runat="server" Text="-" />
            </td>
        </tr>
    </tbody>
</table>
