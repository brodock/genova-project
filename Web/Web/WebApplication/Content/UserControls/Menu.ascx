<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu.ascx.cs" Inherits="Web.UserControls.Menu" %>
<div class="Menu">
    <div class="TitleMenu">
        Principal
    </div>
    <div class="ItemMenu">
        <asp:LinkButton ID="btnHome" runat="server" PostBackUrl="~/Default.aspx" Text="Home" />
    </div>
    <div id="notLoggedBlock" runat="server" class="ItemMenu">
        <asp:LinkButton ID="linkRegisterWebACC" runat="server" PostBackUrl="~/Business/WebUser/Register.aspx"
            Text="Register web account" />
    </div>
</div>
<br />
<div id="loggedBlock" runat="server" class="Menu">
    <div class="TitleMenu">
        Shard
    </div>
    <div class="ItemMenu">
        <asp:LinkButton ID="btnShardACC" runat="server" PostBackUrl="~/Business/ShardUser/Register.aspx"
            Text="Create account or Associate" />
    </div>
    <div class="ItemMenu">
        <asp:LinkButton ID="btnCharacters" runat="server" PostBackUrl="~/Business/ShardUser/CharactersView.aspx"
            Text="Characters" />
    </div>
    <div class="ItemMenu">
        <asp:LinkButton ID="btnCreatures" runat="server" PostBackUrl="~/Business/Creature/ViewAll.aspx"
            Text="Creatures" />
    </div>
    <div class="ItemMenu">
        <asp:LinkButton ID="btnLoots" runat="server" PostBackUrl="~/Business/Creature/ViewLoots.aspx"
            Text="Generic Loot's" />
    </div>
    <div class="ItemMenu">
        <asp:LinkButton ID="btnLogConnection" runat="server" PostBackUrl="~/Business/Log/AccountView.aspx"
            Text="Log Connection" />
    </div>
</div>
